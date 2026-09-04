using ComedorEstudiantil.Application.DTOs;
using ComedorEstudiantil.Application.Enums;
using ComedorEstudiantil.Application.Services.Interfaces;
using ComedorEstudiantil.Infraestructure.Models;
using ComedorEstudiantil.Infraestructure.Repository.Interfaces;

namespace ComedorEstudiantil.Application.Services.Implementations
{
    public class ServiceEntrega : IServiceEntrega
    {
        private readonly IRepositoryEntrega _repositoryEntrega;
        private readonly IRepositorySolicitud _repositorySolicitud;
        private readonly IRepositoryUsuario _repositoryUsuario;
        private readonly IRepositoryMenu _repositoryMenu;
        private readonly IFechaHoraService _fechaHoraService;

        public ServiceEntrega(
            IRepositoryEntrega repositoryEntrega,
            IRepositorySolicitud repositorySolicitud,
            IRepositoryUsuario repositoryUsuario,
            IRepositoryMenu repositoryMenu,
            IFechaHoraService fechaHoraService)
        {
            _repositoryEntrega = repositoryEntrega;
            _repositorySolicitud = repositorySolicitud;
            _repositoryUsuario = repositoryUsuario;
            _repositoryMenu = repositoryMenu;
            _fechaHoraService = fechaHoraService;
        }

        public async Task<List<EntregaListaDTO>> ListarDelDiaAsync()
        {
            DateTime ahora = _fechaHoraService.ObtenerAhora();
            DateTime inicio = ahora.Date;
            DateTime final = inicio.AddDays(1);

            List<Entrega> entregas =
                await _repositoryEntrega.ListarPorPeriodoAsync(
                    inicio,
                    final);

            return entregas.Select(entrega =>
            {
                Solicitud solicitud =
                    entrega.IdSolicitudNavigation;

                Usuario usuario =
                    solicitud.IdUsuarioNavigation;

                Menu menu =
                    solicitud.IdMenuNavigation;

                return new EntregaListaDTO
                {
                    IdEntrega = entrega.IdEntrega,
                    Identificacion = usuario.Identificacion,
                    NombreUsuario =
                        $"{usuario.Nombre} {usuario.Apellidos}",
                    TipoComida =
                        menu.IdTipoComidaNavigation.Nombre,
                    DescripcionMenu =
                        menu.Descripcion,
                    FechaHoraEntrega =
                        entrega.FechaHoraEntrega,
                    EntregadoPor =
                        $"{entrega.IdUsuarioEntregoNavigation.Nombre} {entrega.IdUsuarioEntregoNavigation.Apellidos}",
                    MetodoEntrega =
                        ObtenerNombreMetodo(
                            entrega.MetodoEntrega)
                };
            }).ToList();
        }

        public async Task<RegistroEntregaDTO> PrepararRegistroAsync(
            string? identificacion,
            int? idMenuSeleccionado)
        {
            DateOnly fechaActual =
                _fechaHoraService.ObtenerFechaActual();

            List<Menu> menus =
                await _repositoryMenu
                    .ListarPublicadosPorFechaAsync(
                        fechaActual);

            var resultado = new RegistroEntregaDTO
            {
                Identificacion = identificacion,
                IdMenuSeleccionado = idMenuSeleccionado,
                MenusDisponibles = menus.Select(menu =>
                    new CatalogoDTO
                    {
                        Id = menu.IdMenu,
                        Nombre =
                            $"{menu.IdTipoComidaNavigation.Nombre} - {menu.Descripcion}"
                    })
                    .ToList()
            };

            if (string.IsNullOrWhiteSpace(identificacion))
            {
                return resultado;
            }

            resultado.BusquedaRealizada = true;

            Usuario? usuario =
                await _repositoryUsuario
                    .BuscarPorIdentificacionAsync(
                        identificacion.Trim());

            if (usuario is null)
            {
                resultado.MensajeBusqueda =
                    "No se encontró un usuario activo con esa identificación.";

                return resultado;
            }

            resultado.NombreUsuario =
                $"{usuario.Nombre} {usuario.Apellidos}";

            List<Solicitud> solicitudes =
                await _repositorySolicitud
                    .ListarPorUsuarioYFechaAsync(
                        usuario.IdUsuario,
                        fechaActual);

            if (solicitudes.Count == 0)
            {
                resultado.MensajeBusqueda =
                    $"{resultado.NombreUsuario}, cédula {usuario.Identificacion}, no ha realizado ninguna solicitud para hoy.";

                return resultado;
            }

            resultado.Solicitudes = solicitudes
                .Where(solicitud =>
                    solicitud.Estado ==
                        (sbyte)EstadoSolicitud.Activa &&
                    solicitud.Entrega is null)
                .Select(solicitud =>
                    new SolicitudPendienteEntregaDTO
                    {
                        IdSolicitud =
                            solicitud.IdSolicitud,
                        FechaMenu =
                            solicitud.IdMenuNavigation.Fecha,
                        TipoComida =
                            solicitud.IdMenuNavigation
                                .IdTipoComidaNavigation.Nombre,
                        DescripcionMenu =
                            solicitud.IdMenuNavigation.Descripcion,
                        FechaHoraSolicitud =
                            solicitud.FechaHoraSolicitud
                    })
                .ToList();

            if (resultado.Solicitudes.Count == 0)
            {
                bool tieneEntrega =
                    solicitudes.Any(solicitud =>
                        solicitud.Entrega is not null);

                bool tieneCancelada =
                    solicitudes.Any(solicitud =>
                        solicitud.Estado ==
                        (sbyte)EstadoSolicitud.Cancelada);

                if (tieneEntrega)
                {
                    resultado.MensajeBusqueda =
                        "El usuario no tiene solicitudes pendientes; al menos una de sus comidas ya fue entregada.";
                }
                else if (tieneCancelada)
                {
                    resultado.MensajeBusqueda =
                        "El usuario no tiene solicitudes pendientes; sus solicitudes se encuentran canceladas.";
                }
                else
                {
                    resultado.MensajeBusqueda =
                        "El usuario no tiene solicitudes pendientes para hoy.";
                }
            }

            return resultado;
        }

        public async Task<ResultadoOperacionDTO>
            RegistrarPorFuncionarioAsync(
                int idSolicitud,
                int idUsuarioFuncionario)
        {
            Solicitud? solicitud =
                await _repositorySolicitud.BuscarPorIdAsync(
                    idSolicitud);

            if (solicitud is null)
            {
                return ResultadoOperacionDTO.Error(
                    "La solicitud indicada no existe.");
            }

            return await RegistrarAsync(
                solicitud,
                idUsuarioFuncionario,
                MetodoEntrega.Manual);
        }

        public async Task<ResultadoOperacionDTO>
            RegistrarPorCodigoBarrasAsync(
                string codigoBarras,
                int idMenu,
                int idUsuarioFuncionario)
        {
            if (string.IsNullOrWhiteSpace(codigoBarras))
            {
                return ResultadoOperacionDTO.Error(
                    "Debe escanear un código de barras.");
            }

            Menu? menu =
                await _repositoryMenu.BuscarPorIdAsync(
                    idMenu);

            if (menu is null)
            {
                return ResultadoOperacionDTO.Error(
                    "El menú seleccionado no existe.");
            }

            DateOnly fechaActual =
                _fechaHoraService.ObtenerFechaActual();

            if (menu.Fecha != fechaActual)
            {
                return ResultadoOperacionDTO.Error(
                    "Solo se pueden registrar entregas de los menús del día actual.");
            }

            if (!menu.Publicado)
            {
                return ResultadoOperacionDTO.Error(
                    "El menú seleccionado no está publicado.");
            }

            if (menu.IdTipoComidaNavigation.Activo != true)
            {
                return ResultadoOperacionDTO.Error(
                    "El tipo de comida seleccionado está inactivo.");
            }

            if (menu.IdActividadNavigation is not null &&
                menu.IdActividadNavigation.Activo != true)
            {
                return ResultadoOperacionDTO.Error(
                    "La actividad asociada con el menú está inactiva.");
            }

            Usuario? usuario =
                await _repositoryUsuario
                    .BuscarPorCodigoBarrasAsync(
                        codigoBarras.Trim());

            if (usuario is null)
            {
                return ResultadoOperacionDTO.Error(
                    "No existe un usuario activo asociado con el código escaneado.");
            }

            Solicitud? solicitud =
                await _repositorySolicitud
                    .BuscarPorUsuarioYMenuAsync(
                        usuario.IdUsuario,
                        idMenu);

            string nombreCompleto =
                $"{usuario.Nombre} {usuario.Apellidos}";

            if (solicitud is null)
            {
                return ResultadoOperacionDTO.Error(
                    $"{nombreCompleto}, cédula {usuario.Identificacion}, no ha realizado una solicitud para este menú.");
            }

            if (solicitud.Estado !=
                (sbyte)EstadoSolicitud.Activa)
            {
                return ResultadoOperacionDTO.Error(
                    $"La solicitud de {nombreCompleto}, cédula {usuario.Identificacion}, está cancelada.");
            }

            if (solicitud.Entrega is not null ||
                await _repositoryEntrega
                    .BuscarPorSolicitudAsync(
                        solicitud.IdSolicitud) is not null)
            {
                return ResultadoOperacionDTO.Error(
                    $"La comida de {nombreCompleto}, cédula {usuario.Identificacion}, ya fue entregada.");
            }

            return await RegistrarAsync(
                solicitud,
                idUsuarioFuncionario,
                MetodoEntrega.CodigoBarras);
        }

        private async Task<ResultadoOperacionDTO> RegistrarAsync(
            Solicitud solicitud,
            int idUsuarioEntrego,
            MetodoEntrega metodoEntrega)
        {
            if (solicitud.Estado !=
                (sbyte)EstadoSolicitud.Activa)
            {
                return ResultadoOperacionDTO.Error(
                    "La solicitud se encuentra cancelada.");
            }

            if (solicitud.Entrega is not null ||
                await _repositoryEntrega
                    .BuscarPorSolicitudAsync(
                        solicitud.IdSolicitud) is not null)
            {
                return ResultadoOperacionDTO.Error(
                    "La solicitud ya fue entregada.");
            }

            DateTime ahora =
                _fechaHoraService.ObtenerAhora();

            DateOnly fechaActual =
                DateOnly.FromDateTime(ahora);

            if (solicitud.IdMenuNavigation.Fecha != fechaActual)
            {
                return ResultadoOperacionDTO.Error(
                    "La entrega únicamente puede registrarse el día correspondiente al menú.");
            }

            var entrega = new Entrega
            {
                IdSolicitud =
                    solicitud.IdSolicitud,
                FechaHoraEntrega =
                    ahora,
                IdUsuarioEntrego =
                    idUsuarioEntrego,
                MetodoEntrega =
                    (sbyte)metodoEntrega
            };

            await _repositoryEntrega.AgregarAsync(
                entrega);

            Usuario usuario =
                solicitud.IdUsuarioNavigation;

            return ResultadoOperacionDTO.Correcto(
                $"La entrega de {usuario.Nombre} {usuario.Apellidos}, cédula {usuario.Identificacion}, se realizó correctamente.");
        }

        private static string ObtenerNombreMetodo(
            sbyte metodo)
        {
            return metodo switch
            {
                (sbyte)MetodoEntrega.QR =>
                    "QR",
                (sbyte)MetodoEntrega.CodigoBarras =>
                    "Código de barras",
                (sbyte)MetodoEntrega.Manual =>
                    "Manual",
                _ =>
                    "Desconocido"
            };
        }
    }
}