using ComedorEstudiantil.Application.DTOs;

namespace ComedorEstudiantil.Application.Services.Interfaces
{
    public interface IServiceTipoComida
    {
        Task<List<TipoComidaListaDTO>> ListarAsync();

        Task<TipoComidaFormularioDTO?>
            ObtenerParaEditarAsync(
                int idTipoComida);

        Task<ResultadoOperacionDTO> CrearAsync(
            TipoComidaFormularioDTO formulario);

        Task<ResultadoOperacionDTO> EditarAsync(
            TipoComidaFormularioDTO formulario);

        Task<ResultadoOperacionDTO> CambiarEstadoAsync(
            int idTipoComida);
    }
}