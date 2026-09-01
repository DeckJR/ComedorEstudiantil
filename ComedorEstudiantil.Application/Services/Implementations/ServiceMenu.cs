using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComedorEstudiantil.Application.DTOs;
using ComedorEstudiantil.Application.Services.Interfaces;
using ComedorEstudiantil.Infraestructure.Models;
using ComedorEstudiantil.Infraestructure.Repository.Interfaces;

namespace ComedorEstudiantil.Application.Services.Implementations
{
    public class ServiceMenu : IServiceMenu
    {
        private readonly IRepositoryMenu _repositoryMenu;
        private readonly IRepositoryTipoComida _repositoryTipoComida;
        private readonly IRepositoryActividad _repositoryActividad;

        public ServiceMenu(
            IRepositoryMenu repositoryMenu,
            IRepositoryTipoComida repositoryTipoComida,
            IRepositoryActividad repositoryActividad)
        {
            _repositoryMenu = repositoryMenu;
            _repositoryTipoComida = repositoryTipoComida;
            _repositoryActividad = repositoryActividad;
        }

        public async Task<List<MenuListaDTO>> ListarAsync()
        {
            List<Menu> menus = await _repositoryMenu.ListarAsync();

            return menus.Select(MapearLista).ToList();
        }

        public async Task<MenuPublicoDTO> ListarPublicadosAsync(
            DateOnly fecha)
        {
            List<Menu> menus =
                await _repositoryMenu.ListarPublicadosPorFechaAsync(
                    fecha);

            return new MenuPublicoDTO
            {
                FechaSeleccionada = fecha,
                Menus = menus.Select(MapearLista).ToList()
            };
        }

        public async Task<MenuFormularioDTO> PrepararNuevoAsync()
        {
            var formulario = new MenuFormularioDTO
            {
                Fecha = DateOnly.FromDateTime(DateTime.Today)
            };

            await CargarCatalogosAsync(formulario);

            return formulario;
        }

        public async Task<MenuFormularioDTO?> ObtenerParaEditarAsync(
            int idMenu)
        {
            Menu? menu = await _repositoryMenu.BuscarPorIdAsync(
                idMenu);

            if (menu is null)
            {
                return null;
            }

            var formulario = new MenuFormularioDTO
            {
                IdMenu = menu.IdMenu,
                IdTipoComida = menu.IdTipoComida,
                Fecha = menu.Fecha,
                Descripcion = menu.Descripcion,
                IdActividad = menu.IdActividad,
                Publicado = menu.Publicado
            };

            await CargarCatalogosAsync(formulario);

            if (menu.IdActividadNavigation is not null &&
                formulario.Actividades.All(actividad =>
                    actividad.Id != menu.IdActividad))
            {
                formulario.Actividades.Add(new CatalogoDTO
                {
                    Id = menu.IdActividadNavigation.IdActividad,
                    Nombre =
                        $"{menu.IdActividadNavigation.Fecha:dd/MM/yyyy} - {menu.IdActividadNavigation.Nombre}"
                });
            }

            if (formulario.TiposComida.All(tipo =>
                tipo.Id != menu.IdTipoComida))
            {
                formulario.TiposComida.Add(new CatalogoDTO
                {
                    Id = menu.IdTipoComidaNavigation.IdTipoComida,
                    Nombre = menu.IdTipoComidaNavigation.Nombre
                });
            }

            return formulario;
        }

        public async Task<ResultadoOperacionDTO> CrearAsync(
            MenuFormularioDTO formulario,
            int idUsuarioCreador)
        {
            ResultadoOperacionDTO? validacion =
                await ValidarFormularioAsync(formulario, null);

            if (validacion is not null)
            {
                return validacion;
            }

            var menu = new Menu
            {
                IdTipoComida = formulario.IdTipoComida,
                Fecha = formulario.Fecha,
                Descripcion = formulario.Descripcion.Trim(),
                IdActividad = formulario.IdActividad,
                Publicado = formulario.Publicado,
                IdUsuarioCreador = idUsuarioCreador,
                FechaCreacion = DateTime.Now
            };

            await _repositoryMenu.AgregarAsync(menu);

            return ResultadoOperacionDTO.Correcto(
                "El menú fue creado correctamente.");
        }

        public async Task<ResultadoOperacionDTO> EditarAsync(
            MenuFormularioDTO formulario)
        {
            Menu? menu =
                await _repositoryMenu.BuscarPorIdParaEdicionAsync(
                    formulario.IdMenu);

            if (menu is null)
            {
                return ResultadoOperacionDTO.Error(
                    "El menú solicitado no existe.");
            }

            ResultadoOperacionDTO? validacion =
                await ValidarFormularioAsync(
                    formulario,
                    menu.IdMenu);

            if (validacion is not null)
            {
                return validacion;
            }

            menu.IdTipoComida = formulario.IdTipoComida;
            menu.Fecha = formulario.Fecha;
            menu.Descripcion = formulario.Descripcion.Trim();
            menu.IdActividad = formulario.IdActividad;
            menu.Publicado = formulario.Publicado;

            await _repositoryMenu.GuardarCambiosAsync();

            return ResultadoOperacionDTO.Correcto(
                "El menú fue actualizado correctamente.");
        }

        public async Task<ResultadoOperacionDTO> CambiarPublicacionAsync(
            int idMenu)
        {
            Menu? menu =
                await _repositoryMenu.BuscarPorIdParaEdicionAsync(
                    idMenu);

            if (menu is null)
            {
                return ResultadoOperacionDTO.Error(
                    "El menú solicitado no existe.");
            }

            if (!menu.Publicado)
            {
                var formulario = new MenuFormularioDTO
                {
                    IdMenu = menu.IdMenu,
                    IdTipoComida = menu.IdTipoComida,
                    Fecha = menu.Fecha,
                    Descripcion = menu.Descripcion,
                    IdActividad = menu.IdActividad,
                    Publicado = true
                };

                ResultadoOperacionDTO? validacion =
                    await ValidarFormularioAsync(
                        formulario,
                        menu.IdMenu);

                if (validacion is not null)
                {
                    return validacion;
                }
            }

            menu.Publicado = !menu.Publicado;

            await _repositoryMenu.GuardarCambiosAsync();

            string mensaje = menu.Publicado
                ? "El menú fue publicado correctamente."
                : "El menú dejó de estar publicado.";

            return ResultadoOperacionDTO.Correcto(mensaje);
        }

        private async Task<ResultadoOperacionDTO?>
            ValidarFormularioAsync(
                MenuFormularioDTO formulario,
                int? idMenuExcluir)
        {
            Tipocomida? tipoComida =
                await _repositoryTipoComida.BuscarPorIdAsync(
                    formulario.IdTipoComida);

            if (tipoComida is null ||
                tipoComida.Activo != true)
            {
                return ResultadoOperacionDTO.Error(
                    "El tipo de comida seleccionado no existe o está inactivo.");
            }

            if (formulario.IdActividad.HasValue)
            {
                Actividad? actividad =
                    await _repositoryActividad.BuscarPorIdAsync(
                        formulario.IdActividad.Value);

                if (actividad is null ||
                    actividad.Activo != true)
                {
                    return ResultadoOperacionDTO.Error(
                        "La actividad seleccionada no existe o está inactiva.");
                }

                if (actividad.Fecha != formulario.Fecha)
                {
                    return ResultadoOperacionDTO.Error(
                        "La actividad y el menú deben corresponder a la misma fecha.");
                }

                if (await _repositoryMenu.ExisteAsync(
                    formulario.Fecha,
                    formulario.IdTipoComida,
                    formulario.IdActividad.Value,
                    idMenuExcluir))
                {
                    return ResultadoOperacionDTO.Error(
                        "Ya existe un menú de ese tipo asociado con la actividad seleccionada.");
                }
            }

            return null;
        }

        private async Task CargarCatalogosAsync(
            MenuFormularioDTO formulario)
        {
            formulario.TiposComida =
                (await _repositoryTipoComida.ListarActivosAsync())
                .Select(tipo => new CatalogoDTO
                {
                    Id = tipo.IdTipoComida,
                    Nombre = tipo.Nombre
                })
                .ToList();

            formulario.Actividades =
                await new ServiceActividad(_repositoryActividad)
                    .ListarActivasAsync();
        }

        private static MenuListaDTO MapearLista(Menu menu)
        {
            return new MenuListaDTO
            {
                IdMenu = menu.IdMenu,
                Fecha = menu.Fecha,
                TipoComida = menu.IdTipoComidaNavigation.Nombre,
                Descripcion = menu.Descripcion,
                Actividad = menu.IdActividadNavigation?.Nombre,
                HoraLimiteMarcar =
                    menu.IdTipoComidaNavigation.HoraLimiteMarcar,
                Publicado = menu.Publicado,
                CreadoPor =
                    $"{menu.IdUsuarioCreadorNavigation.Nombre} {menu.IdUsuarioCreadorNavigation.Apellidos}",
                FechaCreacion = menu.FechaCreacion
            };
        }
    }
}
