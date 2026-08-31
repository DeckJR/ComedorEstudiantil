using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComedorEstudiantil.Infraestructure.Models;

namespace ComedorEstudiantil.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryUsuario
    {
        Task<Usuario?> BuscarPorIdentificacionAsync(string identificacion);
        Task<Usuario?> BuscarPorIdAsync(int idUsuario);
        Task<bool> ExisteIdentificacionAsync(string identificacion);
        Task<bool> ExisteCorreoAsync(string correo);
        Task AgregarAsync(Usuario usuario);
        Task ActualizarAsync(Usuario usuario);
    }
}
