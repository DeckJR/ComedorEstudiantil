using ComedorEstudiantil.Infraestructure.Models;

namespace ComedorEstudiantil.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryTipoComida
    {
        Task<List<Tipocomida>> ListarAsync();
        Task<List<Tipocomida>> ListarActivosAsync();
        Task<Tipocomida?> BuscarPorIdAsync(int idTipoComida);
        Task<Tipocomida?> BuscarPorIdParaEdicionAsync(int idTipoComida);
        Task<bool> ExisteNombreAsync(string nombre,int? idTipoComidaExcluir = null);
        Task AgregarAsync(Tipocomida tipoComida);
        Task GuardarCambiosAsync();
    }
}