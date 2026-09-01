using ComedorEstudiantil.Application.DTOs;
using ComedorEstudiantil.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ComedorEstudiantil.Web.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IServiceAutenticacion _serviceAutenticacion;
        private readonly ILogger<CuentaController> _logger;

        public CuentaController(IServiceAutenticacion serviceAutenticacion,ILogger<CuentaController> logger)
        {
            _serviceAutenticacion = serviceAutenticacion;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult IniciarSesion(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            var modelo = new LoginDTO
            {
                ReturnUrl = returnUrl
            };

            return View(modelo);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IniciarSesion(LoginDTO login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            UsuarioSesionDTO? usuario = await _serviceAutenticacion.AutenticarAsync(login);

            if (usuario is null)
            {
                _logger.LogWarning("Intento de inicio de sesión fallido para la identificación {Identificacion}.",login.Identificacion);

                ModelState.AddModelError(string.Empty,"La identificación o la contraseña son incorrectas.");

                return View(login);
            }

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    usuario.IdUsuario.ToString()),
                new Claim(
                    ClaimTypes.Name,
                    usuario.NombreCompleto),
                new Claim(
                    ClaimTypes.Email,
                    usuario.Correo),
                new Claim(
                    ClaimTypes.Role,
                    usuario.Rol),
                new Claim(
                    "Identificacion",
                    usuario.Identificacion),
                new Claim(
                    "DebeCambiarContrasena",
                    usuario.DebeCambiarContrasena.ToString())
            };

            var identidad = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identidad);

            var propiedades = new AuthenticationProperties
            {
                IsPersistent = login.Recordarme,
                AllowRefresh = true
            };

            if (login.Recordarme)
            {
                propiedades.ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7);
            }

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                propiedades);

            _logger.LogInformation(
                "El usuario {IdUsuario} inició sesión correctamente.",
                usuario.IdUsuario);
            if (usuario.DebeCambiarContrasena)
            {
                return RedirectToAction(
                    nameof(CambiarContrasena),
                    "Cuenta");
            }

            if (!string.IsNullOrWhiteSpace(login.ReturnUrl) &&
                Url.IsLocalUrl(login.ReturnUrl))
            {
                return LocalRedirect(login.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        [HttpGet]
        public IActionResult CambiarContrasena()
        {
            var formulario = new CambiarContrasenaDTO
            {
                CambioObligatorio = DebeCambiarContrasena()
            };

            return View(formulario);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarContrasena(
            CambiarContrasenaDTO formulario)
        {
            formulario.CambioObligatorio =
                DebeCambiarContrasena();

            if (!ModelState.IsValid)
            {
                return View(formulario);
            }

            ResultadoOperacionDTO resultado =
                await _serviceAutenticacion.CambiarContrasenaAsync(
                    ObtenerIdUsuarioActual(),
                    formulario);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                return View(formulario);
            }

            _logger.LogInformation(
                "El usuario {IdUsuario} cambió su contraseña.",
                ObtenerIdUsuarioActual());

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["MensajeExito"] =
                "La contraseña fue actualizada. Inicie sesión nuevamente.";

            return RedirectToAction(nameof(IniciarSesion));
        }

        private int ObtenerIdUsuarioActual()
        {
            string? valor = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(valor, out int idUsuario))
            {
                throw new InvalidOperationException(
                    "No fue posible identificar al usuario autenticado.");
            }

            return idUsuario;
        }

        private bool DebeCambiarContrasena()
        {
            string? valor = User.FindFirstValue(
                "DebeCambiarContrasena");

            return string.Equals(
                valor,
                bool.TrueString,
                StringComparison.OrdinalIgnoreCase);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarSesion()
        {
            string? idUsuario = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            _logger.LogInformation(
                "El usuario {IdUsuario} cerró sesión.",
                idUsuario);

            return RedirectToAction(
                nameof(IniciarSesion),
                "Cuenta");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}
