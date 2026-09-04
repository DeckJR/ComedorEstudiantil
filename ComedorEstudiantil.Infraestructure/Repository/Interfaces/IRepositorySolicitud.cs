using ComedorEstudiantil.Infraestructure.Models;

namespace ComedorEstudiantil.Infraestructure.Repository.Interfaces
{
    public interface IRepositorySolicitud
    {
        Task<List<Solicitud>> ListarPorUsuarioAsync(int idUsuario);

        Task<List<Solicitud>> ListarPorUsuarioYMenusAsync(
            int idUsuario,
            List<int> idsMenus);

        Task<List<Solicitud>> ListarPorUsuarioYFechaAsync(
            int idUsuario,
            DateOnly fecha);

        Task<List<Solicitud>> ListarActivasPorUsuarioYFechaAsync(
            int idUsuario,
            DateOnly fecha);

        Task<Solicitud?> BuscarPorUsuarioYMenuAsync(
            int idUsuario,
            int idMenu);

        Task<Solicitud?> BuscarPorIdYUsuarioAsync(
            int idSolicitud,
            int idUsuario);

        Task<Solicitud?> BuscarPorIdAsync(int idSolicitud);
        Task AgregarAsync(Solicitud solicitud);
        Task GuardarCambiosAsync();
    }
}