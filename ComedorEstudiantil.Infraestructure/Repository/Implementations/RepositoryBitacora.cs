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
    public class RepositoryBitacora : IRepositoryBitacora
    {
        private readonly ComedorEstudiantilContext _context;

        public RepositoryBitacora(
            ComedorEstudiantilContext context)
        {
            _context = context;
        }

        public async Task AgregarAsync(
            Bitacora bitacora)
        {
            await _context.Set<Bitacora>().AddAsync(bitacora);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Bitacora>> ListarAsync(
            DateTime fechaInicio,
            DateTime fechaFinExclusiva,
            string? usuario,
            string? accion,
            string? entidad)
        {
            IQueryable<Bitacora> consulta =
                _context.Set<Bitacora>()
                    .AsNoTracking()
                    .Include(bitacora =>
                        bitacora.IdUsuarioNavigation)
                    .Where(bitacora =>
                        bitacora.FechaHora >= fechaInicio &&
                        bitacora.FechaHora < fechaFinExclusiva);

            if (!string.IsNullOrWhiteSpace(usuario))
            {
                string texto = usuario.Trim();

                consulta = consulta.Where(bitacora =>
                    bitacora.IdUsuarioNavigation != null &&
                    (bitacora.IdUsuarioNavigation.Identificacion.Contains(texto) ||
                     bitacora.IdUsuarioNavigation.Nombre.Contains(texto) ||
                     bitacora.IdUsuarioNavigation.Apellidos.Contains(texto)));
            }

            if (!string.IsNullOrWhiteSpace(accion))
            {
                string texto = accion.Trim();

                consulta = consulta.Where(bitacora =>
                    bitacora.Accion.Contains(texto));
            }

            if (!string.IsNullOrWhiteSpace(entidad))
            {
                string texto = entidad.Trim();

                consulta = consulta.Where(bitacora =>
                    bitacora.Entidad.Contains(texto));
            }

            return await consulta
                .OrderByDescending(bitacora =>
                    bitacora.FechaHora)
                .Take(2000)
                .ToListAsync();
        }
    }
}