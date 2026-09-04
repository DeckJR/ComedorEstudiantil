using System.Security.Claims;
using ComedorEstudiantil.Application.DTOs;
using ComedorEstudiantil.Application.Services.Interfaces;
using ComedorEstudiantil.Web.Authorization;
using ComedorEstudiantil.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComedorEstudiantil.Web.Controllers
{
    [Authorize(Policy = PoliticasAutorizacion.GestionarUsuarios)]
    public class UsuarioController : Controller
    {
        private readonly IServiceUsuario _serviceUsuario;
        private readonly ICodigoBarrasService _codigoBarrasService;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(
            IServiceUsuario serviceUsuario,
            ICodigoBarrasService codigoBarrasService,
            ILogger<UsuarioController> logger)
        {
            _serviceUsuario = serviceUsuario;
            _codigoBarrasService = codigoBarrasService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<UsuarioListaDTO> usuarios =
                await _serviceUsuario.ListarAsync();

            return View(usuarios);
        }

        [HttpGet]
        public async Task<IActionResult> CodigoBarras(int id)
        {
            CodigoBarrasUsuarioDTO? usuario =
                await _serviceUsuario.ObtenerCodigoBarrasAsync(id);

            if (usuario is null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        [HttpGet]
        public async Task<IActionResult> ImagenCodigoBarras(int id)
        {
            CodigoBarrasUsuarioDTO? usuario =
                await _serviceUsuario.ObtenerCodigoBarrasAsync(id);

            if (usuario is null)
            {
                return NotFound();
            }

            byte[] imagen =
                _codigoBarrasService.GenerarPng(
                    usuario.CodigoBarras);

            return File(imagen, "image/png");
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            UsuarioFormularioDTO formulario =
                await _serviceUsuario.PrepararNuevoAsync(
                    PuedeAsignarAdministrador());

            return View(formulario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            UsuarioFormularioDTO formulario)
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogosAsync(formulario);
                return View(formulario);
            }

            ResultadoOperacionDTO resultado =
                await _serviceUsuario.CrearAsync(
                    formulario,
                    PuedeAsignarAdministrador());

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                await CargarCatalogosAsync(formulario);
                return View(formulario);
            }

            _logger.LogInformation(
                "El usuario {IdUsuarioActual} creó un nuevo usuario con identificación {Identificacion}.",
                ObtenerIdUsuarioActual(),
                formulario.Identificacion);

            TempData["MensajeExito"] =
                resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            UsuarioFormularioDTO? formulario =
                await _serviceUsuario.ObtenerParaEditarAsync(
                    id,
                    PuedeAsignarAdministrador());

            if (formulario is null)
            {
                return NotFound();
            }

            return View(formulario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            UsuarioFormularioDTO formulario)
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogosAsync(formulario);
                return View(formulario);
            }

            ResultadoOperacionDTO resultado =
                await _serviceUsuario.EditarAsync(
                    formulario,
                    PuedeAsignarAdministrador());

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                await CargarCatalogosAsync(formulario);
                return View(formulario);
            }

            _logger.LogInformation(
                "El usuario {IdUsuarioActual} actualizó al usuario {IdUsuarioModificado}.",
                ObtenerIdUsuarioActual(),
                formulario.IdUsuario);

            TempData["MensajeExito"] =
                resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            ResultadoOperacionDTO resultado =
                await _serviceUsuario.CambiarEstadoAsync(
                    id,
                    ObtenerIdUsuarioActual(),
                    PuedeAsignarAdministrador());

            if (resultado.Exitoso)
            {
                _logger.LogInformation(
                    "El usuario {IdUsuarioActual} cambió el estado del usuario {IdUsuarioModificado}.",
                    ObtenerIdUsuarioActual(),
                    id);

                TempData["MensajeExito"] =
                    resultado.Mensaje;
            }
            else
            {
                TempData["MensajeError"] =
                    resultado.Mensaje;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> RestablecerContrasena(
            int id)
        {
            RestablecerContrasenaDTO? formulario =
                await _serviceUsuario
                    .PrepararRestablecimientoAsync(
                        id,
                        PuedeAsignarAdministrador());

            if (formulario is null)
            {
                return NotFound();
            }

            return View(formulario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestablecerContrasena(
            RestablecerContrasenaDTO formulario)
        {
            if (!ModelState.IsValid)
            {
                return View(formulario);
            }

            ResultadoOperacionDTO resultado =
                await _serviceUsuario
                    .RestablecerContrasenaAsync(
                        formulario,
                        PuedeAsignarAdministrador());

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                return View(formulario);
            }

            _logger.LogInformation(
                "El usuario {IdUsuarioActual} restableció la contraseña del usuario {IdUsuarioModificado}.",
                ObtenerIdUsuarioActual(),
                formulario.IdUsuario);

            TempData["MensajeExito"] =
                resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }

        private bool PuedeAsignarAdministrador()
        {
            return User.IsInRole("Administrador");
        }

        private int ObtenerIdUsuarioActual()
        {
            string? valor =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!int.TryParse(valor, out int idUsuario))
            {
                throw new InvalidOperationException(
                    "No fue posible identificar al usuario autenticado.");
            }

            return idUsuario;
        }

        private async Task CargarCatalogosAsync(
            UsuarioFormularioDTO formulario)
        {
            UsuarioFormularioDTO catalogos =
                await _serviceUsuario.PrepararNuevoAsync(
                    PuedeAsignarAdministrador());

            formulario.IdRolEstudiante =
                catalogos.IdRolEstudiante;
            formulario.Roles =
                catalogos.Roles;
            formulario.TiposBeneficiario =
                catalogos.TiposBeneficiario;
            formulario.GradosSecciones =
                catalogos.GradosSecciones;
        }
    }
}