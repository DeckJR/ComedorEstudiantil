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
    public class ServiceReporte : IServiceReporte
    {
        private readonly IRepositoryReporte _repositoryReporte;

        public ServiceReporte(
            IRepositoryReporte repositoryReporte)
        {
            _repositoryReporte = repositoryReporte;
        }

        public async Task<ReporteGeneralDTO> GenerarAsync(
            FiltroReporteDTO filtro)
        {
            ValidarFiltro(filtro);

            List<Solicitud> solicitudes =
                await _repositoryReporte.ListarSolicitudesAsync(
                    filtro.FechaInicio,
                    filtro.FechaFin);

            DateTime fechaInicio =
                filtro.FechaInicio.ToDateTime(TimeOnly.MinValue);

            DateTime fechaFinExclusiva =
                filtro.FechaFin
                    .AddDays(1)
                    .ToDateTime(TimeOnly.MinValue);

            List<Entrega> entregas =
                await _repositoryReporte.ListarEntregasAsync(
                    fechaInicio,
                    fechaFinExclusiva);

            List<string> tiposComida = solicitudes
                .Select(solicitud =>
                    solicitud.IdMenuNavigation
                        .IdTipoComidaNavigation.Nombre)
                .Concat(entregas.Select(entrega =>
                    entrega.IdSolicitudNavigation
                        .IdMenuNavigation
                        .IdTipoComidaNavigation.Nombre))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(nombre => nombre)
                .ToList();

            if (!string.IsNullOrWhiteSpace(filtro.TipoComida))
            {
                solicitudes = solicitudes
                    .Where(solicitud =>
                        solicitud.IdMenuNavigation
                            .IdTipoComidaNavigation.Nombre
                            .Equals(
                                filtro.TipoComida,
                                StringComparison.OrdinalIgnoreCase))
                    .ToList();

                entregas = entregas
                    .Where(entrega =>
                        entrega.IdSolicitudNavigation
                            .IdMenuNavigation
                            .IdTipoComidaNavigation.Nombre
                            .Equals(
                                filtro.TipoComida,
                                StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (filtro.Estado.HasValue)
            {
                solicitudes = solicitudes
                    .Where(solicitud =>
                        solicitud.Estado == filtro.Estado.Value)
                    .ToList();

                if (filtro.Estado.Value ==
                    (sbyte)EstadoSolicitud.Cancelada)
                {
                    entregas.Clear();
                }
            }

            List<ReporteSolicitudDTO> solicitudesDTO =
                solicitudes
                    .Select(MapearSolicitud)
                    .ToList();

            List<ReporteEntregaDTO> entregasDTO =
                entregas
                    .Select(MapearEntrega)
                    .ToList();

            return new ReporteGeneralDTO
            {
                Filtro = filtro,
                TiposComida = tiposComida,
                Solicitudes = solicitudesDTO,
                Entregas = entregasDTO,
                TotalSolicitudes = solicitudesDTO.Count,
                TotalActivas = solicitudesDTO.Count(
                    solicitud =>
                        solicitud.Estado == "Activa"),
                TotalCanceladas = solicitudesDTO.Count(
                    solicitud =>
                        solicitud.Estado == "Cancelada"),
                TotalEntregadas = solicitudesDTO.Count(
                    solicitud =>
                        solicitud.Entregada),
                TotalPendientes = solicitudesDTO.Count(
                    solicitud =>
                        solicitud.Estado == "Activa" &&
                        !solicitud.Entregada)
            };
        }

        private static ReporteSolicitudDTO MapearSolicitud(
            Solicitud solicitud)
        {
            Usuario usuario =
                solicitud.IdUsuarioNavigation;

            Estudiante? estudiante =
                usuario.Estudiante;

            return new ReporteSolicitudDTO
            {
                IdSolicitud = solicitud.IdSolicitud,
                FechaMenu =
                    solicitud.IdMenuNavigation.Fecha,
                TipoComida =
                    solicitud.IdMenuNavigation
                        .IdTipoComidaNavigation.Nombre,
                DescripcionMenu =
                    solicitud.IdMenuNavigation.Descripcion,
                Identificacion =
                    usuario.Identificacion,
                NombreUsuario =
                    $"{usuario.Nombre} {usuario.Apellidos}",
                Rol =
                    usuario.IdRolNavigation.Nombre,
                TipoBeneficiario =
                    estudiante?.IdTipoBeneficiarioNavigation.Nombre
                    ?? "No aplica",
                GradoSeccion =
                    estudiante?.IdGradoSeccionNavigation is null
                        ? "No aplica"
                        : $"{estudiante.IdGradoSeccionNavigation.Grado}-{estudiante.IdGradoSeccionNavigation.Seccion}",
                FechaHoraSolicitud =
                    solicitud.FechaHoraSolicitud,
                Estado =
                    solicitud.Estado ==
                    (sbyte)EstadoSolicitud.Activa
                        ? "Activa"
                        : "Cancelada",
                MetodoMarcado =
                    ObtenerMetodoMarcado(
                        solicitud.MetodoMarcado),
                MarcadoPor =
                    solicitud.IdUsuarioMarcoNavigation is null
                        ? "Usuario"
                        : $"{solicitud.IdUsuarioMarcoNavigation.Nombre} {solicitud.IdUsuarioMarcoNavigation.Apellidos}",
                Entregada =
                    solicitud.Entrega is not null
            };
        }

        private static ReporteEntregaDTO MapearEntrega(
            Entrega entrega)
        {
            Solicitud solicitud =
                entrega.IdSolicitudNavigation;

            Usuario usuario =
                solicitud.IdUsuarioNavigation;

            Estudiante? estudiante =
                usuario.Estudiante;

            return new ReporteEntregaDTO
            {
                IdEntrega = entrega.IdEntrega,
                FechaMenu =
                    solicitud.IdMenuNavigation.Fecha,
                TipoComida =
                    solicitud.IdMenuNavigation
                        .IdTipoComidaNavigation.Nombre,
                DescripcionMenu =
                    solicitud.IdMenuNavigation.Descripcion,
                Identificacion =
                    usuario.Identificacion,
                NombreUsuario =
                    $"{usuario.Nombre} {usuario.Apellidos}",
                Rol =
                    usuario.IdRolNavigation.Nombre,
                TipoBeneficiario =
                    estudiante?.IdTipoBeneficiarioNavigation.Nombre
                    ?? "No aplica",
                GradoSeccion =
                    estudiante?.IdGradoSeccionNavigation is null
                        ? "No aplica"
                        : $"{estudiante.IdGradoSeccionNavigation.Grado}-{estudiante.IdGradoSeccionNavigation.Seccion}",
                FechaHoraEntrega =
                    entrega.FechaHoraEntrega,
                MetodoEntrega =
                    ObtenerMetodoEntrega(
                        entrega.MetodoEntrega),
                EntregadoPor =
                    $"{entrega.IdUsuarioEntregoNavigation.Nombre} {entrega.IdUsuarioEntregoNavigation.Apellidos}"
            };
        }

        private static string ObtenerMetodoMarcado(
            sbyte metodo)
        {
            return metodo switch
            {
                0 => "Web",
                1 => "QR",
                2 => "Código de barras",
                3 => "Manual",
                _ => "Desconocido"
            };
        }

        private static string ObtenerMetodoEntrega(
            sbyte metodo)
        {
            return metodo switch
            {
                0 => "QR",
                1 => "Código de barras",
                2 => "Manual",
                _ => "Desconocido"
            };
        }

        private static void ValidarFiltro(
            FiltroReporteDTO filtro)
        {
            if (filtro.FechaInicio == default ||
                filtro.FechaFin == default)
            {
                throw new ArgumentException(
                    "Debe indicar la fecha inicial y final.");
            }

            if (filtro.FechaInicio > filtro.FechaFin)
            {
                throw new ArgumentException(
                    "La fecha inicial no puede ser posterior a la fecha final.");
            }

            if (filtro.FechaFin.DayNumber -
                filtro.FechaInicio.DayNumber > 366)
            {
                throw new ArgumentException(
                    "El periodo del reporte no puede superar un año.");
            }
        }
    }
}