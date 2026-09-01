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
    public class RepositoryGradoSeccion : IRepositoryGradoSeccion
    {
        private readonly ComedorEstudiantilContext _context;

        public RepositoryGradoSeccion(ComedorEstudiantilContext context)
        {
            _context = context;
        }

        public async Task<List<Gradoseccion>> ListarAsync()
        {
            return await _context.Set<Gradoseccion>()
                .AsNoTracking()
                .OrderBy(grado => grado.Grado)
                .ThenBy(grado => grado.Seccion)
                .ToListAsync();
        }

        public async Task<bool> ExisteAsync(int idGradoSeccion)
        {
            return await _context.Set<Gradoseccion>()
                .AnyAsync(grado => grado.IdGradoSeccion == idGradoSeccion);
        }
    }
}
