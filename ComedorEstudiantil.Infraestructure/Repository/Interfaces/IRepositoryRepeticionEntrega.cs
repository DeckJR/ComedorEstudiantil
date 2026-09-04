using ComedorEstudiantil.Infraestructure.Models;

namespace ComedorEstudiantil.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryRepeticionEntrega
    {
        Task<bool> ExisteRepeticionRecienteAsync(
            int idEntrega,
            DateTime fechaDesde);

        Task AgregarAsync(
            Repeticionentrega repeticion);
    }
}