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
        Task<List<Usuario>> ListarAsync();
        Task<Usuario?> BuscarPorIdentificacionAsync(string identificacion);
        Task<Usuario?> BuscarPorCodigoBarrasAsync(string codigoBarras);
        Task<Usuario?> BuscarPorIdAsync(int idUsuario);
        Task<Usuario?> BuscarPorIdParaEdicionAsync(int idUsuario);
        Task<bool> ExisteIdentificacionAsync(string identificacion, int? idUsuarioExcluir = null);
        Task<bool> ExisteCorreoAsync(string correo, int? idUsuarioExcluir = null);
        Task<bool> ExisteCodigoBarrasAsync(string codigoBarras);
        Task AgregarAsync(Usuario usuario);
        Task GuardarCambiosAsync();
        Task ActualizarHashContrasenaAsync(int idUsuario, string contrasenaHash);
        Task EstablecerContrasenaAsync(int idUsuario,string contrasenaHash,bool debeCambiarContrasena,DateTime fechaCambio);
        void EliminarEstudiante(Estudiante estudiante);
    }
}