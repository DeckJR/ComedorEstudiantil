using System.Security.Claims;
using ComedorEstudiantil.Application.DTOs;
using ComedorEstudiantil.Application.Services.Interfaces;
using ComedorEstudiantil.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComedorEstudiantil.Web.Controllers
{
    public class SolicitudController : Controller
    {
        private readonly IServiceSolicitud _serviceSolicitud;
        private readonly ILogger<SolicitudController> _logger;

        public SolicitudController(
            IServiceSolicitud serviceSolicitud,
            ILogger<SolicitudController> logger)
        {
            _serviceSolicitud = serviceSolicitud;
            _logger = logger;
        }

        [Authorize(Policy = PoliticasAutorizacion.UsuarioAutenticado)]
        [HttpGet]
        public async Task<IActionResult> MisSolicitudes()
        {
            List<SolicitudListaDTO> solicitudes =
                await _serviceSolicitud.ListarPropiasAsync(
                    ObtenerIdUsuarioActual());

            return View(solicitudes);
        }

        [Authorize(Policy = PoliticasAutorizacion.UsuarioAutenticado)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Solicitar(int idMenu)
        {
            ResultadoOperacionDTO resultado =
                await _serviceSolicitud.SolicitarAsync(
                    idMenu,
                    ObtenerIdUsuarioActual(),
                    null,
                    false);

            GuardarResultado(resultado);

            return RedirectToAction(
                "Index",
                "Menu");
        }

        [Authorize(Policy = PoliticasAutorizacion.UsuarioAutenticado)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(
            int idSolicitud,
            string? returnUrl = null)
        {
            ResultadoOperacionDTO resultado =
                await _serviceSolicitud.CancelarAsync(
                    idSolicitud,
                    ObtenerIdUsuarioActual());

            GuardarResultado(resultado);

            if (!string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction(nameof(MisSolicitudes));
        }

        [Authorize(Policy = PoliticasAutorizacion.RegistrarSolicitudAjena)]
        [HttpGet]
        public async Task<IActionResult> RegistrarParaOtraPersona(
            int idMenu)
        {
            SolicitudAjenaDTO? formulario =
                await _serviceSolicitud
                    .PrepararSolicitudAjenaAsync(idMenu);

            if (formulario is null)
            {
                return NotFound();
            }

            return View(formulario);
        }

        [Authorize(Policy = PoliticasAutorizacion.RegistrarSolicitudAjena)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarParaOtraPersona(
            SolicitudAjenaDTO formulario)
        {
            if (!ModelState.IsValid)
            {
                return View(formulario);
            }

            ResultadoOperacionDTO resultado =
                await _serviceSolicitud
                    .SolicitarParaOtraPersonaAsync(
                        formulario,
                        ObtenerIdUsuarioActual());

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                SolicitudAjenaDTO? datosMenu =
                    await _serviceSolicitud
                        .PrepararSolicitudAjenaAsync(
                            formulario.IdMenu);

                if (datosMenu is not null)
                {
                    formulario.FechaMenu =
                        datosMenu.FechaMenu;
                    formulario.TipoComida =
                        datosMenu.TipoComida;
                    formulario.DescripcionMenu =
                        datosMenu.DescripcionMenu;
                }

                return View(formulario);
            }

            _logger.LogInformation(
                "El usuario {IdUsuarioMarco} registró manualmente una solicitud para la identificación {Identificacion}.",
                ObtenerIdUsuarioActual(),
                formulario.Identificacion);

            TempData["MensajeExito"] = resultado.Mensaje;

            return RedirectToAction(
                "Index",
                "Menu",
                new
                {
                    fecha = formulario.FechaMenu
                        .ToString("yyyy-MM-dd")
                });
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

        private void GuardarResultado(
            ResultadoOperacionDTO resultado)
        {
            if (resultado.Exitoso)
            {
                TempData["MensajeExito"] = resultado.Mensaje;
            }
            else
            {
                TempData["MensajeError"] = resultado.Mensaje;
            }
        }
    }
}