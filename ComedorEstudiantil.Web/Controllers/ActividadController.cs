using System.Security.Claims;
using ComedorEstudiantil.Application.DTOs;
using ComedorEstudiantil.Application.Services.Interfaces;
using ComedorEstudiantil.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComedorEstudiantil.Web.Controllers
{
    [Authorize(Policy = PoliticasAutorizacion.GestionarActividades)]
    public class ActividadController : Controller
    {
        private readonly IServiceActividad _serviceActividad;
        private readonly ILogger<ActividadController> _logger;

        public ActividadController(
            IServiceActividad serviceActividad,
            ILogger<ActividadController> logger)
        {
            _serviceActividad = serviceActividad;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<ActividadDTO> actividades =
                await _serviceActividad.ListarAsync();

            return View(actividades);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View(new ActividadDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            ActividadDTO actividadDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(actividadDTO);
            }

            ResultadoOperacionDTO resultado =
                await _serviceActividad.CrearAsync(
                    actividadDTO);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                return View(actividadDTO);
            }

            _logger.LogInformation(
                "El usuario {IdUsuario} creó la actividad {NombreActividad}.",
                ObtenerIdUsuarioActual(),
                actividadDTO.Nombre);

            TempData["MensajeExito"] = resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            ActividadDTO? actividadDTO =
                await _serviceActividad.BuscarPorIdAsync(id);

            if (actividadDTO is null)
            {
                return NotFound();
            }

            return View(actividadDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            ActividadDTO actividadDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(actividadDTO);
            }

            ResultadoOperacionDTO resultado =
                await _serviceActividad.EditarAsync(
                    actividadDTO);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                return View(actividadDTO);
            }

            _logger.LogInformation(
                "El usuario {IdUsuario} actualizó la actividad {IdActividad}.",
                ObtenerIdUsuarioActual(),
                actividadDTO.IdActividad);

            TempData["MensajeExito"] = resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id)
        {
            ResultadoOperacionDTO resultado =
                await _serviceActividad.CambiarEstadoAsync(id);

            if (resultado.Exitoso)
            {
                _logger.LogInformation(
                    "El usuario {IdUsuario} cambió el estado de la actividad {IdActividad}.",
                    ObtenerIdUsuarioActual(),
                    id);

                TempData["MensajeExito"] = resultado.Mensaje;
            }
            else
            {
                TempData["MensajeError"] = resultado.Mensaje;
            }

            return RedirectToAction(nameof(Index));
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
    }
}