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
    public class RepositoryRol : IRepositoryRol
    {
        private readonly ComedorEstudiantilContext _context;

        public RepositoryRol(ComedorEstudiantilContext context)
        {
            _context = context;
        }

        public async Task<List<Rol>> ListarAsync()
        {
            return await _context.Set<Rol>()
                .AsNoTracking()
                .OrderBy(rol => rol.Nombre)
                .ToListAsync();
        }

        public async Task<Rol?> BuscarPorIdAsync(int idRol)
        {
            return await _context.Set<Rol>()
                .AsNoTracking()
                .FirstOrDefaultAsync(rol => rol.IdRol == idRol);
        }

        public async Task<Rol?> BuscarPorNombreAsync(string nombre)
        {
            return await _context.Set<Rol>()
                .AsNoTracking()
                .FirstOrDefaultAsync(rol => rol.Nombre == nombre);
        }
    }
}
