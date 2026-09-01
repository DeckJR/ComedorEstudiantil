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
    public class RepositoryActividad : IRepositoryActividad
    {
        private readonly ComedorEstudiantilContext _context;

        public RepositoryActividad(ComedorEstudiantilContext context)
        {
            _context = context;
        }

        public async Task<List<Actividad>> ListarAsync()
        {
            return await _context.Set<Actividad>()
                .AsNoTracking()
                .OrderByDescending(actividad => actividad.Fecha)
                .ThenBy(actividad => actividad.Nombre)
                .ToListAsync();
        }

        public async Task<List<Actividad>> ListarActivasAsync()
        {
            return await _context.Set<Actividad>()
                .AsNoTracking()
                .Where(actividad => actividad.Activo == true)
                .OrderBy(actividad => actividad.Fecha)
                .ThenBy(actividad => actividad.Nombre)
                .ToListAsync();
        }

        public async Task<Actividad?> BuscarPorIdAsync(int idActividad)
        {
            return await _context.Set<Actividad>()
                .FirstOrDefaultAsync(actividad =>
                    actividad.IdActividad == idActividad);
        }

        public async Task<bool> ExisteAsync(
            string nombre,
            DateOnly fecha,
            int? idActividadExcluir = null)
        {
            return await _context.Set<Actividad>()
                .AnyAsync(actividad =>
                    actividad.Nombre == nombre &&
                    actividad.Fecha == fecha &&
                    (!idActividadExcluir.HasValue ||
                     actividad.IdActividad != idActividadExcluir.Value));
        }

        public async Task AgregarAsync(Actividad actividad)
        {
            await _context.Set<Actividad>().AddAsync(actividad);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}