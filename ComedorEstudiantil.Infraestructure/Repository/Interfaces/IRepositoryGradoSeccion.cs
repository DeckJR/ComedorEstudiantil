using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComedorEstudiantil.Infraestructure.Models;

namespace ComedorEstudiantil.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryGradoSeccion
    {
        Task<List<Gradoseccion>> ListarAsync();
        Task<bool> ExisteAsync(int idGradoSeccion);
    }
}
