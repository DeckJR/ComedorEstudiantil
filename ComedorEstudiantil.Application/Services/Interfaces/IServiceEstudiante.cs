using ComedorEstudiantil.Application.DTOs;

namespace ComedorEstudiantil.Application.Services.Interfaces
{
    public interface IServiceEstudiante
    {
        Task<List<EstudianteListaDTO>> ListarAsync(
    bool incluirArchivados = false);

        Task<EstudianteFormularioDTO>
            PrepararNuevoAsync();

        Task<EstudianteFormularioDTO?>
            ObtenerParaEditarAsync(
                int idEstudiante);

        Task<CodigoBarrasUsuarioDTO?>
            ObtenerCodigoBarrasAsync(
                int idEstudiante);

        Task<ResultadoOperacionDTO> CrearAsync(
            EstudianteFormularioDTO formulario);

        Task<ResultadoOperacionDTO> EditarAsync(
            EstudianteFormularioDTO formulario);

        Task<ResultadoOperacionDTO> CambiarEstadoAsync(
            int idEstudiante,
            int idUsuarioActual);
    }
}