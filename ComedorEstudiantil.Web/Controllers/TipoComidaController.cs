using ComedorEstudiantil.Application.DTOs;
using ComedorEstudiantil.Application.Services.Interfaces;
using ComedorEstudiantil.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComedorEstudiantil.Web.Controllers
{
    [Authorize(Policy = PoliticasAutorizacion.GestionarHorarios)]
    public class TipoComidaController : Controller
    {
        private readonly IServiceTipoComida
            _serviceTipoComida;

        private readonly ILogger<TipoComidaController>
            _logger;

        public TipoComidaController(
            IServiceTipoComida serviceTipoComida,
            ILogger<TipoComidaController> logger)
        {
            _serviceTipoComida = serviceTipoComida;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<TipoComidaListaDTO> tiposComida =
                await _serviceTipoComida.ListarAsync();

            return View(tiposComida);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            var formulario =
                new TipoComidaFormularioDTO
                {
                    Activo = true
                };

            return View(formulario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            TipoComidaFormularioDTO formulario)
        {
            if (!ModelState.IsValid)
            {
                return View(formulario);
            }

            ResultadoOperacionDTO resultado =
                await _serviceTipoComida.CrearAsync(
                    formulario);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                return View(formulario);
            }

            _logger.LogInformation(
                "Se creó el tipo de comida {Nombre}.",
                formulario.Nombre);

            TempData["MensajeExito"] =
                resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            TipoComidaFormularioDTO? formulario =
                await _serviceTipoComida
                    .ObtenerParaEditarAsync(id);

            if (formulario is null)
            {
                return NotFound();
            }

            return View(formulario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            TipoComidaFormularioDTO formulario)
        {
            if (!ModelState.IsValid)
            {
                return View(formulario);
            }

            ResultadoOperacionDTO resultado =
                await _serviceTipoComida.EditarAsync(
                    formulario);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                return View(formulario);
            }

            _logger.LogInformation(
                "Se actualizó el tipo de comida {IdTipoComida}.",
                formulario.IdTipoComida);

            TempData["MensajeExito"] =
                resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(
            int id)
        {
            ResultadoOperacionDTO resultado =
                await _serviceTipoComida
                    .CambiarEstadoAsync(id);

            if (resultado.Exitoso)
            {
                _logger.LogInformation(
                    "Se cambió el estado del tipo de comida {IdTipoComida}.",
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
    }
}