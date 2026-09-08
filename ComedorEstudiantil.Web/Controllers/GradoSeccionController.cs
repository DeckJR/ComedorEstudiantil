using ComedorEstudiantil.Application.DTOs;
using ComedorEstudiantil.Application.Services.Interfaces;
using ComedorEstudiantil.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComedorEstudiantil.Web.Controllers
{
    [Authorize(
        Policy = PoliticasAutorizacion.GestionarEstudiantes)]
    public class GradoSeccionController : Controller
    {
        private readonly IServiceGradoSeccion
            _serviceGradoSeccion;

        private readonly ILogger<GradoSeccionController>
            _logger;

        public GradoSeccionController(
            IServiceGradoSeccion serviceGradoSeccion,
            ILogger<GradoSeccionController> logger)
        {
            _serviceGradoSeccion =
                serviceGradoSeccion;

            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<GradoSeccionListaDTO> gradosSecciones =
                await _serviceGradoSeccion.ListarAsync();

            return View(gradosSecciones);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            var formulario =
                new GradoSeccionFormularioDTO
                {
                    Activo = true
                };

            return View(formulario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            GradoSeccionFormularioDTO formulario)
        {
            if (!ModelState.IsValid)
            {
                return View(formulario);
            }

            ResultadoOperacionDTO resultado =
                await _serviceGradoSeccion.CrearAsync(
                    formulario);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                return View(formulario);
            }

            _logger.LogInformation(
                "Se creó el grado y sección {Grado}-{Seccion}.",
                formulario.Grado,
                formulario.Seccion);

            TempData["MensajeExito"] =
                resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            GradoSeccionFormularioDTO? formulario =
                await _serviceGradoSeccion
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
            GradoSeccionFormularioDTO formulario)
        {
            if (!ModelState.IsValid)
            {
                return View(formulario);
            }

            ResultadoOperacionDTO resultado =
                await _serviceGradoSeccion.EditarAsync(
                    formulario);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                return View(formulario);
            }

            _logger.LogInformation(
                "Se actualizó el grado y sección {IdGradoSeccion}.",
                formulario.IdGradoSeccion);

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
                await _serviceGradoSeccion
                    .CambiarEstadoAsync(id);

            if (resultado.Exitoso)
            {
                _logger.LogInformation(
                    "Se cambió el estado del grado y sección {IdGradoSeccion}.",
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