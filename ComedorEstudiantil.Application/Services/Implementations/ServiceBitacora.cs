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
    public class ServiceBitacora : IServiceBitacora
    {
        private readonly IRepositoryBitacora _repositoryBitacora;
        private readonly IFechaHoraService _fechaHoraService;

        public ServiceBitacora(
            IRepositoryBitacora repositoryBitacora,
            IFechaHoraService fechaHoraService)
        {
            _repositoryBitacora = repositoryBitacora;
            _fechaHoraService = fechaHoraService;
        }

        public async Task RegistrarAsync(
            int? idUsuario,
            string accion,
            string entidad,
            int? idEntidad,
            string? detalle,
            string? ipOrigen)
        {
            if (string.IsNullOrWhiteSpace(accion))
            {
                throw new ArgumentException(
                    "La acción de la bitácora es obligatoria.");
            }

            if (string.IsNullOrWhiteSpace(entidad))
            {
                throw new ArgumentException(
                    "La entidad de la bitácora es obligatoria.");
            }

            var bitacora = new Bitacora
            {
                IdUsuario = idUsuario,
                Accion = Recortar(accion.Trim(), 100),
                Entidad = Recortar(entidad.Trim(), 50),
                IdEntidad = idEntidad,
                Detalle = string.IsNullOrWhiteSpace(detalle)
                    ? null
                    : detalle.Trim(),
                FechaHora = _fechaHoraService.ObtenerAhora(),
                IpOrigen = string.IsNullOrWhiteSpace(ipOrigen)
                    ? null
                    : Recortar(ipOrigen.Trim(), 45)
            };

            await _repositoryBitacora.AgregarAsync(bitacora);
        }

        public async Task<BitacoraConsultaDTO> ConsultarAsync(
            BitacoraFiltroDTO filtro)
        {
            ValidarFiltro(filtro);

            DateTime fechaInicio =
                filtro.FechaInicio.ToDateTime(
                    TimeOnly.MinValue);

            DateTime fechaFinExclusiva =
                filtro.FechaFin
                    .AddDays(1)
                    .ToDateTime(TimeOnly.MinValue);

            List<Bitacora> registros =
                await _repositoryBitacora.ListarAsync(
                    fechaInicio,
                    fechaFinExclusiva,
                    filtro.Usuario,
                    filtro.Accion,
                    filtro.Entidad);

            return new BitacoraConsultaDTO
            {
                Filtro = filtro,
                Registros = registros.Select(bitacora =>
                    new BitacoraListaDTO
                    {
                        IdBitacora =
                            bitacora.IdBitacora,
                        IdUsuario =
                            bitacora.IdUsuario,
                        IdentificacionUsuario =
                            bitacora.IdUsuarioNavigation?
                                .Identificacion
                            ?? "No identificado",
                        NombreUsuario =
                            bitacora.IdUsuarioNavigation is null
                                ? "No identificado"
                                : $"{bitacora.IdUsuarioNavigation.Nombre} {bitacora.IdUsuarioNavigation.Apellidos}",
                        Accion =
                            bitacora.Accion,
                        Entidad =
                            bitacora.Entidad,
                        IdEntidad =
                            bitacora.IdEntidad,
                        Detalle =
                            bitacora.Detalle
                            ?? "Sin detalle",
                        FechaHora =
                            bitacora.FechaHora,
                        IpOrigen =
                            bitacora.IpOrigen
                            ?? "No disponible"
                    })
                    .ToList()
            };
        }

        private static void ValidarFiltro(
            BitacoraFiltroDTO filtro)
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
                    "El periodo consultado no puede superar un año.");
            }
        }

        private static string Recortar(
            string texto,
            int longitudMaxima)
        {
            return texto.Length <= longitudMaxima
                ? texto
                : texto[..longitudMaxima];
        }
    }
}