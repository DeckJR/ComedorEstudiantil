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
    public class ServiceActividad : IServiceActividad
    {
        private readonly IRepositoryActividad _repositoryActividad;

        public ServiceActividad(
            IRepositoryActividad repositoryActividad)
        {
            _repositoryActividad = repositoryActividad;
        }

        public async Task<List<ActividadDTO>> ListarAsync()
        {
            List<Actividad> actividades =
                await _repositoryActividad.ListarAsync();

            return actividades.Select(actividad => new ActividadDTO
            {
                IdActividad = actividad.IdActividad,
                Nombre = actividad.Nombre,
                Fecha = actividad.Fecha,
                Descripcion = actividad.Descripcion,
                Activo = actividad.Activo == true
            }).ToList();
        }

        public async Task<List<CatalogoDTO>> ListarActivasAsync()
        {
            List<Actividad> actividades =
                await _repositoryActividad.ListarActivasAsync();

            return actividades.Select(actividad => new CatalogoDTO
            {
                Id = actividad.IdActividad,
                Nombre = $"{actividad.Fecha:dd/MM/yyyy} - {actividad.Nombre}"
            }).ToList();
        }

        public async Task<ActividadDTO?> BuscarPorIdAsync(
            int idActividad)
        {
            Actividad? actividad =
                await _repositoryActividad.BuscarPorIdAsync(
                    idActividad);

            if (actividad is null)
            {
                return null;
            }

            return new ActividadDTO
            {
                IdActividad = actividad.IdActividad,
                Nombre = actividad.Nombre,
                Fecha = actividad.Fecha,
                Descripcion = actividad.Descripcion,
                Activo = actividad.Activo == true
            };
        }

        public async Task<ResultadoOperacionDTO> CrearAsync(
            ActividadDTO actividadDTO)
        {
            string nombre = actividadDTO.Nombre.Trim();

            if (await _repositoryActividad.ExisteAsync(
                nombre,
                actividadDTO.Fecha))
            {
                return ResultadoOperacionDTO.Error(
                    "Ya existe una actividad con ese nombre en la fecha seleccionada.");
            }

            var actividad = new Actividad
            {
                Nombre = nombre,
                Fecha = actividadDTO.Fecha,
                Descripcion = LimpiarTextoOpcional(
                    actividadDTO.Descripcion),
                Activo = actividadDTO.Activo
            };

            await _repositoryActividad.AgregarAsync(actividad);

            return ResultadoOperacionDTO.Correcto(
                "La actividad fue creada correctamente.");
        }

        public async Task<ResultadoOperacionDTO> EditarAsync(
            ActividadDTO actividadDTO)
        {
            Actividad? actividad =
                await _repositoryActividad.BuscarPorIdAsync(
                    actividadDTO.IdActividad);

            if (actividad is null)
            {
                return ResultadoOperacionDTO.Error(
                    "La actividad solicitada no existe.");
            }

            string nombre = actividadDTO.Nombre.Trim();

            if (await _repositoryActividad.ExisteAsync(
                nombre,
                actividadDTO.Fecha,
                actividad.IdActividad))
            {
                return ResultadoOperacionDTO.Error(
                    "Ya existe otra actividad con ese nombre en la fecha seleccionada.");
            }

            actividad.Nombre = nombre;
            actividad.Fecha = actividadDTO.Fecha;
            actividad.Descripcion = LimpiarTextoOpcional(
                actividadDTO.Descripcion);
            actividad.Activo = actividadDTO.Activo;

            await _repositoryActividad.GuardarCambiosAsync();

            return ResultadoOperacionDTO.Correcto(
                "La actividad fue actualizada correctamente.");
        }

        public async Task<ResultadoOperacionDTO> CambiarEstadoAsync(
            int idActividad)
        {
            Actividad? actividad =
                await _repositoryActividad.BuscarPorIdAsync(
                    idActividad);

            if (actividad is null)
            {
                return ResultadoOperacionDTO.Error(
                    "La actividad solicitada no existe.");
            }

            bool nuevoEstado = actividad.Activo != true;
            actividad.Activo = nuevoEstado;

            await _repositoryActividad.GuardarCambiosAsync();

            string mensaje = nuevoEstado
                ? "La actividad fue activada correctamente."
                : "La actividad fue desactivada correctamente.";

            return ResultadoOperacionDTO.Correcto(mensaje);
        }

        private static string? LimpiarTextoOpcional(string? texto)
        {
            return string.IsNullOrWhiteSpace(texto)
                ? null
                : texto.Trim();
        }
    }
}