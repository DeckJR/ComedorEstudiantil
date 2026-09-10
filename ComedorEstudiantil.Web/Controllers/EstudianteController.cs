using System.Security.Claims;
using ComedorEstudiantil.Application.DTOs;
using ComedorEstudiantil.Application.Services.Interfaces;
using ComedorEstudiantil.Web.Authorization;
using ComedorEstudiantil.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComedorEstudiantil.Web.Controllers
{
    [Authorize(
        Policy =
            PoliticasAutorizacion.GestionarEstudiantes)]
    public class EstudianteController : Controller
    {
        private readonly IServiceEstudiante
            _serviceEstudiante;

        private readonly ICodigoBarrasService
            _codigoBarrasService;

        private readonly ILogger<EstudianteController>
            _logger;

        public EstudianteController(
            IServiceEstudiante serviceEstudiante,
            ICodigoBarrasService codigoBarrasService,
            ILogger<EstudianteController> logger)
        {
            _serviceEstudiante =
                serviceEstudiante;

            _codigoBarrasService =
                codigoBarrasService;

            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
    bool incluirArchivados = false)
        {
            List<EstudianteListaDTO> estudiantes =
                await _serviceEstudiante.ListarAsync(
                    incluirArchivados);

            ViewData["IncluirArchivados"] =
                incluirArchivados;

            return View(estudiantes);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            EstudianteFormularioDTO formulario =
                await _serviceEstudiante
                    .PrepararNuevoAsync();

            return View(formulario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            EstudianteFormularioDTO formulario)
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogosAsync(formulario);

                return View(formulario);
            }

            ResultadoOperacionDTO resultado =
                await _serviceEstudiante
                    .CrearAsync(formulario);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                await CargarCatalogosAsync(formulario);

                return View(formulario);
            }

            _logger.LogInformation(
                "El usuario {IdUsuarioActual} creó al estudiante con identificación {Identificacion}.",
                ObtenerIdUsuarioActual(),
                formulario.Identificacion);

            TempData["MensajeExito"] =
                resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(
            int id)
        {
            EstudianteFormularioDTO? formulario =
                await _serviceEstudiante
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
            EstudianteFormularioDTO formulario)
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogosAsync(formulario);

                return View(formulario);
            }

            ResultadoOperacionDTO resultado =
                await _serviceEstudiante
                    .EditarAsync(formulario);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                await CargarCatalogosAsync(formulario);

                return View(formulario);
            }

            _logger.LogInformation(
                "El usuario {IdUsuarioActual} actualizó al estudiante {IdEstudiante}.",
                ObtenerIdUsuarioActual(),
                formulario.IdEstudiante);

            TempData["MensajeExito"] =
                resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(
    int id,
    bool incluirArchivados = false)
        {
            ResultadoOperacionDTO resultado =
                await _serviceEstudiante.CambiarEstadoAsync(
                    id,
                    ObtenerIdUsuarioActual());

            if (resultado.Exitoso)
            {
                _logger.LogInformation(
                    "El usuario {IdUsuarioActual} archivó o restauró al estudiante {IdEstudiante}.",
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

            return RedirectToAction(
                nameof(Index),
                new
                {
                    incluirArchivados
                });
        }

        [HttpGet]
        public async Task<IActionResult> CodigoBarras(
            int id)
        {
            CodigoBarrasUsuarioDTO? estudiante =
                await _serviceEstudiante
                    .ObtenerCodigoBarrasAsync(id);

            if (estudiante is null)
            {
                return NotFound();
            }

            ViewData["IdEstudiante"] = id;

            return View(estudiante);
        }

        [HttpGet]
        public async Task<IActionResult>
            ImagenCodigoBarras(
                int id)
        {
            CodigoBarrasUsuarioDTO? estudiante =
                await _serviceEstudiante
                    .ObtenerCodigoBarrasAsync(id);

            if (estudiante is null)
            {
                return NotFound();
            }

            byte[] imagen =
                _codigoBarrasService.GenerarPng(
                    estudiante.CodigoBarras);

            return File(
                imagen,
                "image/png");
        }

        private int ObtenerIdUsuarioActual()
        {
            string? valor =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!int.TryParse(
                valor,
                out int idUsuario))
            {
                throw new InvalidOperationException(
                    "No fue posible identificar al usuario autenticado.");
            }

            return idUsuario;
        }

        private async Task CargarCatalogosAsync(
            EstudianteFormularioDTO formulario)
        {
            EstudianteFormularioDTO catalogos =
                await _serviceEstudiante
                    .PrepararNuevoAsync();

            formulario.TiposBeneficiario =
                catalogos.TiposBeneficiario;

            formulario.GradosSecciones =
                catalogos.GradosSecciones;
        }
    }
}