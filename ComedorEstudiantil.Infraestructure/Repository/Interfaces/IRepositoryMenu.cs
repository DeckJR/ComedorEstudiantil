using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComedorEstudiantil.Infraestructure.Models;

namespace ComedorEstudiantil.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryMenu
    {
        Task<List<Menu>> ListarAsync();
        Task<List<Menu>> ListarPublicadosPorFechaAsync(DateOnly fecha);
        Task<Menu?> BuscarPorIdAsync(int idMenu);
        Task<Menu?> BuscarPorIdParaEdicionAsync(int idMenu);
        Task<bool> ExisteAsync(
            DateOnly fecha,
            int idTipoComida,
            int idActividad,
            int? idMenuExcluir = null);
        Task AgregarAsync(Menu menu);
        Task GuardarCambiosAsync();
    }
}