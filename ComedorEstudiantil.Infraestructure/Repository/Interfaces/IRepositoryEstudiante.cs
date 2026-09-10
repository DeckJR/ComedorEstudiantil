using ComedorEstudiantil.Infraestructure.Models;

namespace ComedorEstudiantil.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryEstudiante
    {
        Task<List<Estudiante>> ListarAsync(
    bool incluirArchivados = false);

        Task<Estudiante?> BuscarPorIdAsync(
            int idEstudiante);

        Task<Estudiante?> BuscarPorIdParaEdicionAsync(
            int idEstudiante);

        Task GuardarCambiosAsync();
    }
}