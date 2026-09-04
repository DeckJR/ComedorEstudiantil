using System.Security.Claims;
using ComedorEstudiantil.Application.DTOs;
using ComedorEstudiantil.Application.Services.Interfaces;
using ComedorEstudiantil.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComedorEstudiantil.Web.Controllers
{
    public class EntregaController : Controller
    {
        private readonly IServiceEntrega _serviceEntrega;
        private readonly ILogger<EntregaController> _logger;

        public EntregaController(
            IServiceEntrega serviceEntrega,
            ILogger<EntregaController> logger)
        {
            _serviceEntrega = serviceEntrega;
            _logger = logger;
        }

        [Authorize(Policy = PoliticasAutorizacion.RegistrarEntrega)]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<EntregaListaDTO> entregas =
                await _serviceEntrega.ListarDelDiaAsync();

            return View(entregas);
        }

        [Authorize(Policy = PoliticasAutorizacion.RegistrarEntrega)]
        [HttpGet]
        public async Task<IActionResult> Registrar(
            string? identificacion,
            int? idMenu)
        {
            RegistroEntregaDTO modelo =
                await _serviceEntrega.PrepararRegistroAsync(
                    identificacion,
                    idMenu);

            return View(modelo);
        }

        [Authorize(Policy = PoliticasAutorizacion.RegistrarEntrega)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(
            int idSolicitud,
            string identificacion,
            int? idMenu)
        {
            ResultadoOperacionDTO resultado =
                await _serviceEntrega
                    .RegistrarPorFuncionarioAsync(
                        idSolicitud,
                        ObtenerIdUsuarioActual());

            GuardarResultado(resultado);

            return RedirectToAction(
                nameof(Registrar),
                new
                {
                    identificacion,
                    idMenu
                });
        }

        [Authorize(Policy = PoliticasAutorizacion.RegistrarEntrega)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarCodigoBarras(
            int? idMenu,
            string? codigoBarras)
        {
            if (!idMenu.HasValue)
            {
                TempData["MensajeError"] =
                    "Debe seleccionar el menú que está entregando.";

                return RedirectToAction(
                    nameof(Registrar));
            }

            ResultadoEscaneoEntregaDTO resultado =
                await _serviceEntrega
                    .RegistrarPorCodigoBarrasAsync(
                        codigoBarras ?? string.Empty,
                        idMenu.Value,
                        ObtenerIdUsuarioActual());

            if (resultado.RequiereConfirmarRepeticion)
            {
                GuardarConfirmacionRepeticion(
                    resultado,
                    "CodigoBarras",
                    null);
            }
            else
            {
                GuardarResultadoEscaneo(resultado);
            }

            return RedirectToAction(
                nameof(Registrar),
                new
                {
                    idMenu = idMenu.Value
                });
        }

        [Authorize(Policy = PoliticasAutorizacion.RegistrarEntrega)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrepararRepeticionManual(
            int idEntrega,
            string identificacion,
            int? idMenu)
        {
            ResultadoEscaneoEntregaDTO resultado =
                await _serviceEntrega
                    .PrepararRepeticionManualAsync(
                        idEntrega);

            if (resultado.RequiereConfirmarRepeticion)
            {
                GuardarConfirmacionRepeticion(
                    resultado,
                    "Manual",
                    identificacion);
            }
            else
            {
                GuardarResultadoEscaneo(resultado);
            }

            return RedirectToAction(
                nameof(Registrar),
                new
                {
                    identificacion,
                    idMenu
                });
        }

        [Authorize(Policy = PoliticasAutorizacion.RegistrarEntrega)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarRepeticionManual(
            int idEntrega,
            string identificacion,
            int? idMenu)
        {
            ResultadoOperacionDTO resultado =
                await _serviceEntrega
                    .RegistrarRepeticionManualAsync(
                        idEntrega,
                        ObtenerIdUsuarioActual());

            GuardarResultado(resultado);

            return RedirectToAction(
                nameof(Registrar),
                new
                {
                    identificacion,
                    idMenu
                });
        }

        [Authorize(Policy = PoliticasAutorizacion.RegistrarEntrega)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            RegistrarRepeticionCodigoBarras(
                int idEntrega,
                int idMenu)
        {
            ResultadoOperacionDTO resultado =
                await _serviceEntrega
                    .RegistrarRepeticionCodigoBarrasAsync(
                        idEntrega,
                        ObtenerIdUsuarioActual());

            GuardarResultado(resultado);

            return RedirectToAction(
                nameof(Registrar),
                new
                {
                    idMenu
                });
        }

        private void GuardarConfirmacionRepeticion(
            ResultadoEscaneoEntregaDTO resultado,
            string origen,
            string? identificacionBusqueda)
        {
            TempData["ConfirmarRepeticion"] = true;
            TempData["IdEntregaRepeticion"] =
                resultado.IdEntrega;
            TempData["NombreRepeticion"] =
                resultado.NombreUsuario;
            TempData["IdentificacionRepeticion"] =
                resultado.Identificacion;
            TempData["CantidadRepeticiones"] =
                resultado.CantidadRepeticiones;
            TempData["MensajeRepeticion"] =
                resultado.Mensaje;
            TempData["OrigenRepeticion"] =
                origen;
            TempData["IdentificacionBusqueda"] =
                identificacionBusqueda;
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

        private void GuardarResultado(
            ResultadoOperacionDTO resultado)
        {
            if (resultado.Exitoso)
            {
                TempData["MensajeExito"] =
                    resultado.Mensaje;
            }
            else
            {
                TempData["MensajeError"] =
                    resultado.Mensaje;
            }
        }

        private void GuardarResultadoEscaneo(
            ResultadoEscaneoEntregaDTO resultado)
        {
            if (resultado.Exitoso)
            {
                TempData["MensajeExito"] =
                    resultado.Mensaje;
            }
            else
            {
                TempData["MensajeError"] =
                    resultado.Mensaje;
            }
        }
    }
}