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
    public class ServiceEntrega : IServiceEntrega
    {
        private readonly IRepositoryEntrega _repositoryEntrega;
        private readonly IRepositorySolicitud _repositorySolicitud;
        private readonly IRepositoryUsuario _repositoryUsuario;
        private readonly IFechaHoraService _fechaHoraService;

        public ServiceEntrega(
            IRepositoryEntrega repositoryEntrega,
            IRepositorySolicitud repositorySolicitud,
            IRepositoryUsuario repositoryUsuario,
            IFechaHoraService fechaHoraService)
        {
            _repositoryEntrega = repositoryEntrega;
            _repositorySolicitud = repositorySolicitud;
            _repositoryUsuario = repositoryUsuario;
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
                    DescripcionMenu = menu.Descripcion,
                    FechaHoraEntrega = entrega.FechaHoraEntrega,
                    EntregadoPor =
                        $"{entrega.IdUsuarioEntregoNavigation.Nombre} {entrega.IdUsuarioEntregoNavigation.Apellidos}",
                    MetodoEntrega =
                        ObtenerNombreMetodo(entrega.MetodoEntrega)
                };
            }).ToList();
        }

        public async Task<RegistroEntregaDTO>
            BuscarSolicitudesPendientesAsync(
                string? identificacion)
        {
            var resultado = new RegistroEntregaDTO
            {
                Identificacion = identificacion
            };

            if (string.IsNullOrWhiteSpace(identificacion))
            {
                return resultado;
            }

            resultado.BusquedaRealizada = true;

            Usuario? usuario =
                await _repositoryUsuario.BuscarPorIdentificacionAsync(
                    identificacion.Trim());

            if (usuario is null)
            {
                return resultado;
            }

            resultado.NombreUsuario =
                $"{usuario.Nombre} {usuario.Apellidos}";

            DateOnly fechaActual =
                _fechaHoraService.ObtenerFechaActual();

            List<Solicitud> solicitudes =
                await _repositorySolicitud
                    .ListarActivasPorUsuarioYFechaAsync(
                        usuario.IdUsuario,
                        fechaActual);

            resultado.Solicitudes = solicitudes
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
                idUsuarioFuncionario);
        }

        private async Task<ResultadoOperacionDTO> RegistrarAsync(
            Solicitud solicitud,
            int idUsuarioEntrego)
        {
            if (solicitud.Estado !=
                (sbyte)EstadoSolicitud.Activa)
            {
                return ResultadoOperacionDTO.Error(
                    "La solicitud se encuentra cancelada.");
            }

            if (solicitud.Entrega is not null ||
                await _repositoryEntrega.BuscarPorSolicitudAsync(
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
                IdSolicitud = solicitud.IdSolicitud,
                FechaHoraEntrega = ahora,
                IdUsuarioEntrego = idUsuarioEntrego,
                MetodoEntrega =
                    (sbyte)MetodoEntrega.Manual
            };

            await _repositoryEntrega.AgregarAsync(entrega);

            return ResultadoOperacionDTO.Correcto(
                "La entrega fue registrada correctamente.");
        }

        private static string ObtenerNombreMetodo(
            sbyte metodo)
        {
            return metodo switch
            {
                (sbyte)MetodoEntrega.QR => "QR",
                (sbyte)MetodoEntrega.CodigoBarras =>
                    "Código de barras",
                (sbyte)MetodoEntrega.Manual => "Manual",
                _ => "Desconocido"
            };
        }
    }
}