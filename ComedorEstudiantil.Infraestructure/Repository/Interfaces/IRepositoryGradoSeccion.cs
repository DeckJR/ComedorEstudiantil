using ComedorEstudiantil.Infraestructure.Models;

namespace ComedorEstudiantil.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryGradoSeccion
    {
        Task<List<Gradoseccion>> ListarAsync();
        Task<List<Gradoseccion>> ListarTodosAsync();
        Task<Gradoseccion?> BuscarPorIdAsync(int idGradoSeccion);

        Task<Gradoseccion?> BuscarPorIdParaEdicionAsync(
            int idGradoSeccion);

        Task<bool> ExisteAsync(int idGradoSeccion);

        Task<bool> ExisteCombinacionAsync(
            string grado,
            string seccion,
            int? idGradoSeccionExcluir = null);

        Task AgregarAsync(Gradoseccion gradoSeccion);
        Task GuardarCambiosAsync();
    }
}