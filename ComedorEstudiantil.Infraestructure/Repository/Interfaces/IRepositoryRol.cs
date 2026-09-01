using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComedorEstudiantil.Infraestructure.Models;

namespace ComedorEstudiantil.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryRol
    {
        Task<List<Rol>> ListarAsync();
        Task<Rol?> BuscarPorIdAsync(int idRol);
        Task<Rol?> BuscarPorNombreAsync(string nombre);
    }
}
