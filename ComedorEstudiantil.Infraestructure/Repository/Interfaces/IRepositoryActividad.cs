using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComedorEstudiantil.Infraestructure.Models;

namespace ComedorEstudiantil.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryActividad
    {
        Task<List<Actividad>> ListarAsync();
        Task<List<Actividad>> ListarActivasAsync();
        Task<Actividad?> BuscarPorIdAsync(int idActividad);
        Task<bool> ExisteAsync(string nombre,DateOnly fecha,int? idActividadExcluir = null);
        Task AgregarAsync(Actividad actividad);
        Task GuardarCambiosAsync();
    }
}
