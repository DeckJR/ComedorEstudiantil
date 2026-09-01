using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComedorEstudiantil.Infraestructure.Data;
using ComedorEstudiantil.Infraestructure.Models;
using ComedorEstudiantil.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ComedorEstudiantil.Infraestructure.Repository.Implementations
{
    public class RepositoryUsuario : IRepositoryUsuario
    {
        private readonly ComedorEstudiantilContext _context;

        public RepositoryUsuario(ComedorEstudiantilContext context)
        {
            _context = context;
        }

        public async Task<List<Usuario>> ListarAsync()
        {
            return await _context.Set<Usuario>()
                .AsNoTracking()
                .Include(usuario => usuario.IdRolNavigation)
                .Include(usuario => usuario.Estudiante)
                    .ThenInclude(estudiante =>
                        estudiante!.IdTipoBeneficiarioNavigation)
                .Include(usuario => usuario.Estudiante)
                    .ThenInclude(estudiante =>
                        estudiante!.IdGradoSeccionNavigation)
                .OrderBy(usuario => usuario.Apellidos)
                .ThenBy(usuario => usuario.Nombre)
                .ToListAsync();
        }

        public async Task<Usuario?> BuscarPorIdentificacionAsync(
            string identificacion)
        {
            return await _context.Set<Usuario>()
                .AsNoTracking()
                .Include(usuario => usuario.IdRolNavigation)
                .FirstOrDefaultAsync(usuario =>
                    usuario.Identificacion == identificacion &&
                    usuario.Activo == true);
        }

        public async Task<Usuario?> BuscarPorIdAsync(int idUsuario)
        {
            return await _context.Set<Usuario>()
                .AsNoTracking()
                .Include(usuario => usuario.IdRolNavigation)
                .Include(usuario => usuario.Estudiante)
                    .ThenInclude(estudiante =>
                        estudiante!.IdTipoBeneficiarioNavigation)
                .Include(usuario => usuario.Estudiante)
                    .ThenInclude(estudiante =>
                        estudiante!.IdGradoSeccionNavigation)
                .FirstOrDefaultAsync(usuario =>
                    usuario.IdUsuario == idUsuario);
        }

        public async Task<Usuario?> BuscarPorIdParaEdicionAsync(
            int idUsuario)
        {
            return await _context.Set<Usuario>()
                .Include(usuario => usuario.IdRolNavigation)
                .Include(usuario => usuario.Estudiante)
                .FirstOrDefaultAsync(usuario =>
                    usuario.IdUsuario == idUsuario);
        }

        public async Task<bool> ExisteIdentificacionAsync(
            string identificacion,
            int? idUsuarioExcluir = null)
        {
            return await _context.Set<Usuario>()
                .AnyAsync(usuario =>
                    usuario.Identificacion == identificacion &&
                    (!idUsuarioExcluir.HasValue ||
                     usuario.IdUsuario != idUsuarioExcluir.Value));
        }

        public async Task<bool> ExisteCorreoAsync(
            string correo,
            int? idUsuarioExcluir = null)
        {
            return await _context.Set<Usuario>()
                .AnyAsync(usuario =>
                    usuario.Correo == correo &&
                    (!idUsuarioExcluir.HasValue ||
                     usuario.IdUsuario != idUsuarioExcluir.Value));
        }

        public async Task AgregarAsync(Usuario usuario)
        {
            await _context.Set<Usuario>().AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarHashContrasenaAsync(
    int idUsuario,
    string contrasenaHash)
        {
            await _context.Set<Usuario>()
                .Where(usuario => usuario.IdUsuario == idUsuario)
                .ExecuteUpdateAsync(actualizacion =>
                    actualizacion.SetProperty(
                        usuario => usuario.ContrasenaHash,
                        contrasenaHash));
        }

        public async Task EstablecerContrasenaAsync(
            int idUsuario,
            string contrasenaHash,
            bool debeCambiarContrasena,
            DateTime fechaCambio)
        {
            await _context.Set<Usuario>()
                .Where(usuario => usuario.IdUsuario == idUsuario)
                .ExecuteUpdateAsync(actualizacion => actualizacion
                    .SetProperty(
                        usuario => usuario.ContrasenaHash,
                        contrasenaHash)
                    .SetProperty(
                        usuario => usuario.DebeCambiarContrasena,
                        debeCambiarContrasena)
                    .SetProperty(
                        usuario => usuario.FechaUltimoCambioContrasena,
                        fechaCambio));
        }

        public void EliminarEstudiante(Estudiante estudiante)
        {
            _context.Set<Estudiante>().Remove(estudiante);
        }
    }
}
