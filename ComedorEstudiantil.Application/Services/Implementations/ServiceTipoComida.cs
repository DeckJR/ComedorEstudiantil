using ComedorEstudiantil.Application.DTOs;
using ComedorEstudiantil.Application.Services.Interfaces;
using ComedorEstudiantil.Infraestructure.Models;
using ComedorEstudiantil.Infraestructure.Repository.Interfaces;

namespace ComedorEstudiantil.Application.Services.Implementations
{
    public class ServiceTipoComida : IServiceTipoComida
    {
        private readonly IRepositoryTipoComida
            _repositoryTipoComida;

        public ServiceTipoComida(
            IRepositoryTipoComida repositoryTipoComida)
        {
            _repositoryTipoComida =
                repositoryTipoComida;
        }

        public async Task<List<TipoComidaListaDTO>>
            ListarAsync()
        {
            List<Tipocomida> tiposComida =
                await _repositoryTipoComida.ListarAsync();

            return tiposComida
                .Select(tipoComida =>
                    new TipoComidaListaDTO
                    {
                        IdTipoComida =
                            tipoComida.IdTipoComida,
                        Nombre =
                            tipoComida.Nombre,
                        HoraLimiteMarcar =
                            tipoComida.HoraLimiteMarcar,
                        Activo =
                            tipoComida.Activo == true
                    })
                .ToList();
        }

        public async Task<TipoComidaFormularioDTO?>
            ObtenerParaEditarAsync(
                int idTipoComida)
        {
            Tipocomida? tipoComida =
                await _repositoryTipoComida
                    .BuscarPorIdAsync(idTipoComida);

            if (tipoComida is null)
            {
                return null;
            }

            return new TipoComidaFormularioDTO
            {
                IdTipoComida =
                    tipoComida.IdTipoComida,
                Nombre =
                    tipoComida.Nombre,
                HoraLimiteMarcar =
                    tipoComida.HoraLimiteMarcar,
                Activo =
                    tipoComida.Activo == true
            };
        }

        public async Task<ResultadoOperacionDTO>
            CrearAsync(
                TipoComidaFormularioDTO formulario)
        {
            string nombre = formulario.Nombre.Trim();

            if (!formulario.HoraLimiteMarcar.HasValue)
            {
                return ResultadoOperacionDTO.Error(
                    "Debe indicar la hora límite.");
            }

            bool nombreExistente =
                await _repositoryTipoComida
                    .ExisteNombreAsync(nombre);

            if (nombreExistente)
            {
                return ResultadoOperacionDTO.Error(
                    "Ya existe un tipo de comida con ese nombre.");
            }

            var tipoComida = new Tipocomida
            {
                Nombre = nombre,
                HoraLimiteMarcar =
                    formulario.HoraLimiteMarcar.Value,
                Activo = formulario.Activo
            };

            await _repositoryTipoComida
                .AgregarAsync(tipoComida);

            return ResultadoOperacionDTO.Correcto(
                "El horario de comida fue creado correctamente.");
        }

        public async Task<ResultadoOperacionDTO>
            EditarAsync(
                TipoComidaFormularioDTO formulario)
        {
            Tipocomida? tipoComida =
                await _repositoryTipoComida
                    .BuscarPorIdParaEdicionAsync(
                        formulario.IdTipoComida);

            if (tipoComida is null)
            {
                return ResultadoOperacionDTO.Error(
                    "El horario de comida no existe.");
            }

            if (!formulario.HoraLimiteMarcar.HasValue)
            {
                return ResultadoOperacionDTO.Error(
                    "Debe indicar la hora límite.");
            }

            string nombre = formulario.Nombre.Trim();

            bool nombreExistente =
                await _repositoryTipoComida
                    .ExisteNombreAsync(
                        nombre,
                        formulario.IdTipoComida);

            if (nombreExistente)
            {
                return ResultadoOperacionDTO.Error(
                    "Ya existe otro tipo de comida con ese nombre.");
            }

            tipoComida.Nombre = nombre;
            tipoComida.HoraLimiteMarcar =
                formulario.HoraLimiteMarcar.Value;
            tipoComida.Activo = formulario.Activo;

            await _repositoryTipoComida
                .GuardarCambiosAsync();

            return ResultadoOperacionDTO.Correcto(
                "El horario de comida fue actualizado correctamente.");
        }

        public async Task<ResultadoOperacionDTO>
            CambiarEstadoAsync(
                int idTipoComida)
        {
            Tipocomida? tipoComida =
                await _repositoryTipoComida
                    .BuscarPorIdParaEdicionAsync(
                        idTipoComida);

            if (tipoComida is null)
            {
                return ResultadoOperacionDTO.Error(
                    "El horario de comida no existe.");
            }

            tipoComida.Activo =
                tipoComida.Activo != true;

            await _repositoryTipoComida
                .GuardarCambiosAsync();

            string estado =
                tipoComida.Activo == true
                    ? "activado"
                    : "desactivado";

            return ResultadoOperacionDTO.Correcto(
                $"El horario de comida fue {estado} correctamente.");
        }
    }
}