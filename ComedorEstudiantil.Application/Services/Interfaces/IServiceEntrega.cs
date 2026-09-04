using ComedorEstudiantil.Application.DTOs;

namespace ComedorEstudiantil.Application.Services.Interfaces
{
    public interface IServiceEntrega
    {
        Task<List<EntregaListaDTO>> ListarDelDiaAsync();

        Task<RegistroEntregaDTO> PrepararRegistroAsync(
            string? identificacion,
            int? idMenuSeleccionado);

        Task<ResultadoOperacionDTO> RegistrarPorFuncionarioAsync(
            int idSolicitud,
            int idUsuarioFuncionario);

        Task<ResultadoEscaneoEntregaDTO> RegistrarPorCodigoBarrasAsync(
            string codigoBarras,
            int idMenu,
            int idUsuarioFuncionario);

        Task<ResultadoEscaneoEntregaDTO> PrepararRepeticionManualAsync(
            int idEntrega);

        Task<ResultadoOperacionDTO> RegistrarRepeticionManualAsync(
            int idEntrega,
            int idUsuarioFuncionario);

        Task<ResultadoOperacionDTO> RegistrarRepeticionCodigoBarrasAsync(
            int idEntrega,
            int idUsuarioFuncionario);
    }
}