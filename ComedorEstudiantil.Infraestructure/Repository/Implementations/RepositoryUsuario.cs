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

        public async Task<Usuario?> BuscarPorIdentificacionAsync(string identificacion)
        {
            return await _context.Set<Usuario>().AsNoTracking().Include(usuario => usuario.IdRolNavigation).FirstOrDefaultAsync(usuario =>usuario.Identificacion == identificacion &&usuario.Activo == true);
        }

        public async Task<Usuario?> BuscarPorIdAsync(int idUsuario)
        {
            return await _context.Set<Usuario>().AsNoTracking().Include(usuario => usuario.IdRolNavigation).FirstOrDefaultAsync(usuario =>usuario.IdUsuario == idUsuario &&usuario.Activo == true);
        }

        public async Task<bool> ExisteIdentificacionAsync(string identificacion)
        {
            return await _context.Set<Usuario>().AnyAsync(usuario => usuario.Identificacion == identificacion);
        }

        public async Task<bool> ExisteCorreoAsync(string correo)
        {
            return await _context.Set<Usuario>().AnyAsync(usuario => usuario.Correo == correo);
        }

        public async Task AgregarAsync(Usuario usuario)
        {
            await _context.Set<Usuario>().AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Usuario usuario)
        {
            _context.Set<Usuario>().Update(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
