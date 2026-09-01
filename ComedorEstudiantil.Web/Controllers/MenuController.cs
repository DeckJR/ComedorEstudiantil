using System.Security.Claims;
using ComedorEstudiantil.Application.DTOs;
using ComedorEstudiantil.Application.Services.Interfaces;
using ComedorEstudiantil.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComedorEstudiantil.Web.Controllers
{
    public class MenuController : Controller
    {
        private readonly IServiceMenu _serviceMenu;
        private readonly ILogger<MenuController> _logger;

        public MenuController(
            IServiceMenu serviceMenu,
            ILogger<MenuController> logger)
        {
            _serviceMenu = serviceMenu;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index(DateOnly? fecha)
        {
            DateOnly fechaSeleccionada = fecha ??
                DateOnly.FromDateTime(DateTime.Today);

            MenuPublicoDTO modelo =
                await _serviceMenu.ListarPublicadosAsync(
                    fechaSeleccionada);

            return View(modelo);
        }

        [Authorize(Policy = PoliticasAutorizacion.GestionarMenus)]
        [HttpGet]
        public async Task<IActionResult> Administrar()
        {
            List<MenuListaDTO> menus =
                await _serviceMenu.ListarAsync();

            return View(menus);
        }

        [Authorize(Policy = PoliticasAutorizacion.GestionarMenus)]
        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            MenuFormularioDTO formulario =
                await _serviceMenu.PrepararNuevoAsync();

            return View(formulario);
        }

        [Authorize(Policy = PoliticasAutorizacion.GestionarMenus)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            MenuFormularioDTO formulario)
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogosAsync(formulario);
                return View(formulario);
            }

            ResultadoOperacionDTO resultado =
                await _serviceMenu.CrearAsync(
                    formulario,
                    ObtenerIdUsuarioActual());

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                await CargarCatalogosAsync(formulario);
                return View(formulario);
            }

            _logger.LogInformation(
                "El usuario {IdUsuario} creó un menú para la fecha {Fecha}.",
                ObtenerIdUsuarioActual(),
                formulario.Fecha);

            TempData["MensajeExito"] = resultado.Mensaje;

            return RedirectToAction(nameof(Administrar));
        }

        [Authorize(Policy = PoliticasAutorizacion.GestionarMenus)]
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            MenuFormularioDTO? formulario =
                await _serviceMenu.ObtenerParaEditarAsync(id);

            if (formulario is null)
            {
                return NotFound();
            }

            return View(formulario);
        }

        [Authorize(Policy = PoliticasAutorizacion.GestionarMenus)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            MenuFormularioDTO formulario)
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogosAsync(formulario);
                return View(formulario);
            }

            ResultadoOperacionDTO resultado =
                await _serviceMenu.EditarAsync(formulario);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensaje);

                await CargarCatalogosAsync(formulario);
                return View(formulario);
            }

            _logger.LogInformation(
                "El usuario {IdUsuario} actualizó el menú {IdMenu}.",
                ObtenerIdUsuarioActual(),
                formulario.IdMenu);

            TempData["MensajeExito"] = resultado.Mensaje;

            return RedirectToAction(nameof(Administrar));
        }

        [Authorize(Policy = PoliticasAutorizacion.GestionarMenus)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPublicacion(int id)
        {
            ResultadoOperacionDTO resultado =
                await _serviceMenu.CambiarPublicacionAsync(id);

            if (resultado.Exitoso)
            {
                TempData["MensajeExito"] = resultado.Mensaje;
            }
            else
            {
                TempData["MensajeError"] = resultado.Mensaje;
            }

            return RedirectToAction(nameof(Administrar));
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

        private async Task CargarCatalogosAsync(
            MenuFormularioDTO formulario)
        {
            MenuFormularioDTO catalogos =
                await _serviceMenu.PrepararNuevoAsync();

            formulario.TiposComida = catalogos.TiposComida;
            formulario.Actividades = catalogos.Actividades;
        }
    }
}