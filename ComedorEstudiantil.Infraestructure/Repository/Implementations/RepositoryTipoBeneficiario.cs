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
    public class RepositoryTipoBeneficiario : IRepositoryTipoBeneficiario
    {
        private readonly ComedorEstudiantilContext _context;

        public RepositoryTipoBeneficiario(
            ComedorEstudiantilContext context)
        {
            _context = context;
        }

        public async Task<List<Tipobeneficiario>> ListarAsync()
        {
            return await _context.Set<Tipobeneficiario>()
                .AsNoTracking()
                .OrderBy(tipo => tipo.Nombre)
                .ToListAsync();
        }

        public async Task<bool> ExisteAsync(int idTipoBeneficiario)
        {
            return await _context.Set<Tipobeneficiario>()
                .AnyAsync(tipo =>
                    tipo.IdTipoBeneficiario == idTipoBeneficiario);
        }
    }
}
