using ComedorEstudiantil.Infraestructure.Models;

namespace ComedorEstudiantil.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryEntrega
    {
        Task<List<Entrega>> ListarPorPeriodoAsync(
            DateTime inicio,
            DateTime final);

        Task<Entrega?> BuscarPorIdAsync(
            int idEntrega);

        Task<Entrega?> BuscarPorSolicitudAsync(
            int idSolicitud);

        Task AgregarAsync(
            Entrega entrega);
    }
}