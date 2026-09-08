using ComedorEstudiantil.Application.DTOs;
using ComedorEstudiantil.Application.Services.Interfaces;
using ComedorEstudiantil.Infraestructure.Models;
using ComedorEstudiantil.Infraestructure.Repository.Interfaces;

namespace ComedorEstudiantil.Application.Services.Implementations
{
    public class ServiceGradoSeccion :
        IServiceGradoSeccion
    {
        private readonly IRepositoryGradoSeccion
            _repositoryGradoSeccion;

        public ServiceGradoSeccion(
            IRepositoryGradoSeccion repositoryGradoSeccion)
        {
            _repositoryGradoSeccion =
                repositoryGradoSeccion;
        }

        public async Task<List<GradoSeccionListaDTO>>
            ListarAsync()
        {
            List<Gradoseccion> gradosSecciones =
                await _repositoryGradoSeccion
                    .ListarTodosAsync();

            return gradosSecciones
                .Select(gradoSeccion =>
                    new GradoSeccionListaDTO
                    {
                        IdGradoSeccion =
                            gradoSeccion.IdGradoSeccion,
                        Grado =
                            gradoSeccion.Grado,
                        Seccion =
                            gradoSeccion.Seccion,
                        NombreCompleto =
                            $"{gradoSeccion.Grado}-{gradoSeccion.Seccion}",
                        Activo =
                            gradoSeccion.Activo == true
                    })
                .ToList();
        }

        public async Task<GradoSeccionFormularioDTO?>
            ObtenerParaEditarAsync(
                int idGradoSeccion)
        {
            Gradoseccion? gradoSeccion =
                await _repositoryGradoSeccion
                    .BuscarPorIdAsync(
                        idGradoSeccion);

            if (gradoSeccion is null)
            {
                return null;
            }

            return new GradoSeccionFormularioDTO
            {
                IdGradoSeccion =
                    gradoSeccion.IdGradoSeccion,
                Grado =
                    gradoSeccion.Grado,
                Seccion =
                    gradoSeccion.Seccion,
                Activo =
                    gradoSeccion.Activo == true
            };
        }

        public async Task<ResultadoOperacionDTO>
            CrearAsync(
                GradoSeccionFormularioDTO formulario)
        {
            string grado =
                NormalizarValor(formulario.Grado);

            string seccion =
                NormalizarValor(formulario.Seccion);

            bool combinacionExistente =
                await _repositoryGradoSeccion
                    .ExisteCombinacionAsync(
                        grado,
                        seccion);

            if (combinacionExistente)
            {
                return ResultadoOperacionDTO.Error(
                    $"Ya existe el grado y sección {grado}-{seccion}.");
            }

            var gradoSeccion = new Gradoseccion
            {
                Grado = grado,
                Seccion = seccion,
                Activo = formulario.Activo
            };

            await _repositoryGradoSeccion
                .AgregarAsync(gradoSeccion);

            return ResultadoOperacionDTO.Correcto(
                $"El grado y sección {grado}-{seccion} fue creado correctamente.");
        }

        public async Task<ResultadoOperacionDTO>
            EditarAsync(
                GradoSeccionFormularioDTO formulario)
        {
            Gradoseccion? gradoSeccion =
                await _repositoryGradoSeccion
                    .BuscarPorIdParaEdicionAsync(
                        formulario.IdGradoSeccion);

            if (gradoSeccion is null)
            {
                return ResultadoOperacionDTO.Error(
                    "El grado y sección no existe.");
            }

            string grado =
                NormalizarValor(formulario.Grado);

            string seccion =
                NormalizarValor(formulario.Seccion);

            bool combinacionExistente =
                await _repositoryGradoSeccion
                    .ExisteCombinacionAsync(
                        grado,
                        seccion,
                        formulario.IdGradoSeccion);

            if (combinacionExistente)
            {
                return ResultadoOperacionDTO.Error(
                    $"Ya existe otro registro con el grado y sección {grado}-{seccion}.");
            }

            gradoSeccion.Grado = grado;
            gradoSeccion.Seccion = seccion;
            gradoSeccion.Activo = formulario.Activo;

            await _repositoryGradoSeccion
                .GuardarCambiosAsync();

            return ResultadoOperacionDTO.Correcto(
                $"El grado y sección {grado}-{seccion} fue actualizado correctamente.");
        }

        public async Task<ResultadoOperacionDTO>
            CambiarEstadoAsync(
                int idGradoSeccion)
        {
            Gradoseccion? gradoSeccion =
                await _repositoryGradoSeccion
                    .BuscarPorIdParaEdicionAsync(
                        idGradoSeccion);

            if (gradoSeccion is null)
            {
                return ResultadoOperacionDTO.Error(
                    "El grado y sección no existe.");
            }

            gradoSeccion.Activo =
                gradoSeccion.Activo != true;

            await _repositoryGradoSeccion
                .GuardarCambiosAsync();

            string nombreCompleto =
                $"{gradoSeccion.Grado}-{gradoSeccion.Seccion}";

            string estado =
                gradoSeccion.Activo == true
                    ? "activado"
                    : "desactivado";

            return ResultadoOperacionDTO.Correcto(
                $"El grado y sección {nombreCompleto} fue {estado} correctamente.");
        }

        private static string NormalizarValor(
            string valor)
        {
            return valor.Trim().ToUpperInvariant();
        }
    }
}