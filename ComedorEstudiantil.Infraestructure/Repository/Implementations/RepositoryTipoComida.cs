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
    public class RepositoryTipoComida : IRepositoryTipoComida
    {
        private readonly ComedorEstudiantilContext _context;

        public RepositoryTipoComida(
            ComedorEstudiantilContext context)
        {
            _context = context;
        }

        public async Task<List<Tipocomida>> ListarActivosAsync()
        {
            return await _context.Set<Tipocomida>()
                .AsNoTracking()
                .Where(tipo => tipo.Activo == true)
                .OrderBy(tipo => tipo.HoraLimiteMarcar)
                .ToListAsync();
        }

        public async Task<Tipocomida?> BuscarPorIdAsync(
            int idTipoComida)
        {
            return await _context.Set<Tipocomida>()
                .AsNoTracking()
                .FirstOrDefaultAsync(tipo =>
                    tipo.IdTipoComida == idTipoComida);
        }
    }
}