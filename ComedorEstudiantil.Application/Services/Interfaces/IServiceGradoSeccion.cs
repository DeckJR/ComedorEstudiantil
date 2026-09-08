using ComedorEstudiantil.Application.DTOs;

namespace ComedorEstudiantil.Application.Services.Interfaces
{
    public interface IServiceGradoSeccion
    {
        Task<List<GradoSeccionListaDTO>> ListarAsync();

        Task<GradoSeccionFormularioDTO?>
            ObtenerParaEditarAsync(
                int idGradoSeccion);

        Task<ResultadoOperacionDTO> CrearAsync(
            GradoSeccionFormularioDTO formulario);

        Task<ResultadoOperacionDTO> EditarAsync(
            GradoSeccionFormularioDTO formulario);

        Task<ResultadoOperacionDTO> CambiarEstadoAsync(
            int idGradoSeccion);
    }
}