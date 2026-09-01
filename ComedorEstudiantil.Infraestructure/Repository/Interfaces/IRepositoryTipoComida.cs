using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComedorEstudiantil.Infraestructure.Models;

namespace ComedorEstudiantil.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryTipoComida
    {
        Task<List<Tipocomida>> ListarActivosAsync();
        Task<Tipocomida?> BuscarPorIdAsync(int idTipoComida);
    }
}
