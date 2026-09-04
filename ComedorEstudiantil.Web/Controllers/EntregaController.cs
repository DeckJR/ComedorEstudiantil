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
            string identificacion)
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
                    identificacion
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

                return RedirectToAction(nameof(Registrar));
            }

            ResultadoOperacionDTO resultado =
                await _serviceEntrega
                    .RegistrarPorCodigoBarrasAsync(
                        codigoBarras ?? string.Empty,
                        idMenu.Value,
                        ObtenerIdUsuarioActual());

            GuardarResultado(resultado);

            return RedirectToAction(
                nameof(Registrar),
                new
                {
                    idMenu = idMenu.Value
                });
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
    }
}