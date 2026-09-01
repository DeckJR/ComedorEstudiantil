using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComedorEstudiantil.Application.DTOs;
using ComedorEstudiantil.Application.Enums;
using ComedorEstudiantil.Application.Services.Interfaces;
using ComedorEstudiantil.Infraestructure.Models;
using ComedorEstudiantil.Infraestructure.Repository.Interfaces;

namespace ComedorEstudiantil.Application.Services.Implementations
{
    public class ServiceSolicitud : IServiceSolicitud
    {
        private readonly IRepositorySolicitud _repositorySolicitud;
        private readonly IRepositoryMenu _repositoryMenu;
        private readonly IRepositoryUsuario _repositoryUsuario;
        private readonly IFechaHoraService _fechaHoraService;

        public ServiceSolicitud(
            IRepositorySolicitud repositorySolicitud,
            IRepositoryMenu repositoryMenu,
            IRepositoryUsuario repositoryUsuario,
            IFechaHoraService fechaHoraService)
        {
            _repositorySolicitud = repositorySolicitud;
            _repositoryMenu = repositoryMenu;
            _repositoryUsuario = repositoryUsuario;
            _fechaHoraService = fechaHoraService;
        }

        public async Task AplicarEstadoSolicitudesAsync(
            List<MenuListaDTO> menus,
            int? idUsuario)
        {
            DateTime ahora = _fechaHoraService.ObtenerAhora();

            foreach (MenuListaDTO menu in menus)
            {
                menu.PuedeSolicitar = EstaDentroDelPlazo(
                    menu.Fecha,
                    menu.HoraLimiteMarcar,
                    ahora);

                if (!menu.PuedeSolicitar)
                {
                    menu.MensajeDisponibilidad =
                        ObtenerMensajeFueraDePlazo(
                            menu.Fecha,
                            ahora);
                }
                else if (!idUsuario.HasValue)
                {
                    menu.MensajeDisponibilidad =
                        "Debe iniciar sesión para solicitar comida.";
                }
            }

            if (!idUsuario.HasValue || menus.Count == 0)
            {
                return;
            }

            List<int> idsMenus = menus
                .Select(menu => menu.IdMenu)
                .ToList();

            List<Solicitud> solicitudes =
                await _repositorySolicitud
                    .ListarPorUsuarioYMenusAsync(
                        idUsuario.Value,
                        idsMenus);

            foreach (MenuListaDTO menu in menus)
            {
                Solicitud? solicitud = solicitudes
                    .FirstOrDefault(item =>
                        item.IdMenu == menu.IdMenu);

                if (solicitud is null)
                {
                    continue;
                }

                menu.IdSolicitud = solicitud.IdSolicitud;
                menu.SolicitudActiva =
                    solicitud.Estado ==
                    (sbyte)EstadoSolicitud.Activa;
            }
        }

        public async Task<List<SolicitudListaDTO>>
            ListarPropiasAsync(int idUsuario)
        {
            List<Solicitud> solicitudes =
                await _repositorySolicitud
                    .ListarPorUsuarioAsync(idUsuario);

            DateTime ahora = _fechaHoraService.ObtenerAhora();

            return solicitudes.Select(solicitud =>
            {
                Menu menu = solicitud.IdMenuNavigation;
                bool activa = solicitud.Estado ==
                    (sbyte)EstadoSolicitud.Activa;

                return new SolicitudListaDTO
                {
                    IdSolicitud = solicitud.IdSolicitud,
                    FechaMenu = menu.Fecha,
                    TipoComida =
                        menu.IdTipoComidaNavigation.Nombre,
                    DescripcionMenu = menu.Descripcion,
                    Actividad =
                        menu.IdActividadNavigation?.Nombre,
                    FechaHoraSolicitud =
                        solicitud.FechaHoraSolicitud,
                    Estado = activa
                        ? "Activa"
                        : "Cancelada",
                    Activa = activa,
                    PuedeCancelar = activa &&
                        solicitud.Entrega is null &&
                        EstaDentroDelPlazo(
                            menu.Fecha,
                            menu.IdTipoComidaNavigation.HoraLimiteMarcar,
                            ahora),
                    Entregada = solicitud.Entrega is not null,
                    PuedeRegistrarEntrega = activa &&
                        solicitud.Entrega is null &&
                        menu.Fecha == DateOnly.FromDateTime(ahora)
                };
            }).ToList();
        }

        public async Task<ResultadoOperacionDTO> SolicitarAsync(
            int idMenu,
            int idUsuario,
            int? idUsuarioMarco,
            bool esSolicitudManual)
        {
            Menu? menu = await _repositoryMenu.BuscarPorIdAsync(
                idMenu);

            ResultadoOperacionDTO? validacion =
                ValidarMenuParaSolicitud(menu);

            if (validacion is not null)
            {
                return validacion;
            }

            Solicitud? solicitudExistente =
                await _repositorySolicitud
                    .BuscarPorUsuarioYMenuAsync(
                        idUsuario,
                        idMenu);

            if (solicitudExistente is not null &&
                solicitudExistente.Estado ==
                (sbyte)EstadoSolicitud.Activa)
            {
                return ResultadoOperacionDTO.Error(
                    "La persona ya tiene una solicitud activa para este menú.");
            }

            DateTime ahora = _fechaHoraService.ObtenerAhora();
            sbyte metodo = esSolicitudManual
                ? (sbyte)MetodoMarcado.Manual
                : (sbyte)MetodoMarcado.Web;

            if (solicitudExistente is null)
            {
                var solicitud = new Solicitud
                {
                    IdUsuario = idUsuario,
                    IdMenu = idMenu,
                    FechaHoraSolicitud = ahora,
                    Estado = (sbyte)EstadoSolicitud.Activa,
                    MetodoMarcado = metodo,
                    IdUsuarioMarco = idUsuarioMarco
                };

                await _repositorySolicitud.AgregarAsync(
                    solicitud);
            }
            else
            {
                solicitudExistente.FechaHoraSolicitud = ahora;
                solicitudExistente.Estado =
                    (sbyte)EstadoSolicitud.Activa;
                solicitudExistente.MetodoMarcado = metodo;
                solicitudExistente.IdUsuarioMarco =
                    idUsuarioMarco;

                await _repositorySolicitud
                    .GuardarCambiosAsync();
            }

            return ResultadoOperacionDTO.Correcto(
                "La solicitud fue registrada correctamente.");
        }

        public async Task<ResultadoOperacionDTO> CancelarAsync(
            int idSolicitud,
            int idUsuario)
        {
            Solicitud? solicitud =
                await _repositorySolicitud
                    .BuscarPorIdYUsuarioAsync(
                        idSolicitud,
                        idUsuario);

            if (solicitud is null)
            {
                return ResultadoOperacionDTO.Error(
                    "La solicitud no existe o no pertenece al usuario.");
            }

            if (solicitud.Estado ==
                (sbyte)EstadoSolicitud.Cancelada)
            {
                return ResultadoOperacionDTO.Error(
                    "La solicitud ya se encuentra cancelada.");
            }

            if (solicitud.Entrega is not null)
            {
                return ResultadoOperacionDTO.Error(
                    "No se puede cancelar una solicitud que ya fue entregada.");
            }

            Menu menu = solicitud.IdMenuNavigation;
            DateTime ahora = _fechaHoraService.ObtenerAhora();

            if (!EstaDentroDelPlazo(
                menu.Fecha,
                menu.IdTipoComidaNavigation.HoraLimiteMarcar,
                ahora))
            {
                return ResultadoOperacionDTO.Error(
                    "La hora límite para cancelar la solicitud ya finalizó.");
            }

            solicitud.Estado =
                (sbyte)EstadoSolicitud.Cancelada;

            await _repositorySolicitud.GuardarCambiosAsync();

            return ResultadoOperacionDTO.Correcto(
                "La solicitud fue cancelada correctamente.");
        }

        public async Task<SolicitudAjenaDTO?>
            PrepararSolicitudAjenaAsync(int idMenu)
        {
            Menu? menu = await _repositoryMenu.BuscarPorIdAsync(
                idMenu);

            if (menu is null)
            {
                return null;
            }

            return new SolicitudAjenaDTO
            {
                IdMenu = menu.IdMenu,
                FechaMenu = menu.Fecha,
                TipoComida =
                    menu.IdTipoComidaNavigation.Nombre,
                DescripcionMenu = menu.Descripcion
            };
        }

        public async Task<ResultadoOperacionDTO>
            SolicitarParaOtraPersonaAsync(
                SolicitudAjenaDTO formulario,
                int idUsuarioMarco)
        {
            Usuario? usuario =
                await _repositoryUsuario
                    .BuscarPorIdentificacionAsync(
                        formulario.Identificacion.Trim());

            if (usuario is null)
            {
                return ResultadoOperacionDTO.Error(
                    "No existe un usuario activo con esa identificación.");
            }

            return await SolicitarAsync(
                formulario.IdMenu,
                usuario.IdUsuario,
                idUsuarioMarco,
                true);
        }

        private ResultadoOperacionDTO? ValidarMenuParaSolicitud(
            Menu? menu)
        {
            if (menu is null)
            {
                return ResultadoOperacionDTO.Error(
                    "El menú solicitado no existe.");
            }

            if (!menu.Publicado)
            {
                return ResultadoOperacionDTO.Error(
                    "El menú solicitado no está publicado.");
            }

            if (menu.IdTipoComidaNavigation.Activo != true)
            {
                return ResultadoOperacionDTO.Error(
                    "El tipo de comida se encuentra inactivo.");
            }

            if (menu.IdActividadNavigation is not null &&
                menu.IdActividadNavigation.Activo != true)
            {
                return ResultadoOperacionDTO.Error(
                    "La actividad relacionada se encuentra inactiva.");
            }

            DateTime ahora = _fechaHoraService.ObtenerAhora();

            if (!EstaDentroDelPlazo(
                menu.Fecha,
                menu.IdTipoComidaNavigation.HoraLimiteMarcar,
                ahora))
            {
                return ResultadoOperacionDTO.Error(
                    "La hora límite para solicitar este menú ya finalizó.");
            }

            return null;
        }

        private static bool EstaDentroDelPlazo(
            DateOnly fechaMenu,
            TimeOnly horaLimite,
            DateTime ahora)
        {
            DateOnly fechaActual = DateOnly.FromDateTime(ahora);

            if (fechaMenu < fechaActual)
            {
                return false;
            }

            if (fechaMenu > fechaActual)
            {
                return true;
            }

            TimeOnly horaActual = TimeOnly.FromDateTime(ahora);

            return horaActual <= horaLimite;
        }

        private static string ObtenerMensajeFueraDePlazo(
            DateOnly fechaMenu,
            DateTime ahora)
        {
            DateOnly fechaActual = DateOnly.FromDateTime(ahora);

            return fechaMenu < fechaActual
                ? "Este menú corresponde a una fecha pasada."
                : "La hora límite para solicitar este menú ya finalizó.";
        }
    }
}