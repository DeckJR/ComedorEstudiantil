using ComedorEstudiantil.Infraestructure.Data;
using ComedorEstudiantil.Infraestructure.Models;
using ComedorEstudiantil.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ComedorEstudiantil.Infraestructure.Repository.Implementations
{
    public class RepositoryEntrega : IRepositoryEntrega
    {
        private readonly ComedorEstudiantilContext _context;

        public RepositoryEntrega(
            ComedorEstudiantilContext context)
        {
            _context = context;
        }

        public async Task<List<Entrega>> ListarPorPeriodoAsync(
            DateTime inicio,
            DateTime final)
        {
            return await _context.Set<Entrega>()
                .AsNoTracking()
                .Include(entrega =>
                    entrega.IdSolicitudNavigation)
                    .ThenInclude(solicitud =>
                        solicitud.IdUsuarioNavigation)
                .Include(entrega =>
                    entrega.IdSolicitudNavigation)
                    .ThenInclude(solicitud =>
                        solicitud.IdMenuNavigation)
                    .ThenInclude(menu =>
                        menu.IdTipoComidaNavigation)
                .Include(entrega =>
                    entrega.IdUsuarioEntregoNavigation)
                .Include(entrega =>
                    entrega.Repeticionentrega)
                .Where(entrega =>
                    entrega.FechaHoraEntrega >= inicio &&
                    entrega.FechaHoraEntrega < final)
                .OrderByDescending(entrega =>
                    entrega.FechaHoraEntrega)
                .ToListAsync();
        }

        public async Task<Entrega?> BuscarPorIdAsync(
            int idEntrega)
        {
            return await _context.Set<Entrega>()
                .AsNoTracking()
                .Include(entrega =>
                    entrega.IdSolicitudNavigation)
                    .ThenInclude(solicitud =>
                        solicitud.IdUsuarioNavigation)
                .Include(entrega =>
                    entrega.IdSolicitudNavigation)
                    .ThenInclude(solicitud =>
                        solicitud.IdMenuNavigation)
                    .ThenInclude(menu =>
                        menu.IdTipoComidaNavigation)
                    .Include(entrega =>
                        entrega.Repeticionentrega)
                .FirstOrDefaultAsync(entrega =>
                    entrega.IdEntrega == idEntrega);
        }

        public async Task<Entrega?> BuscarPorSolicitudAsync(
    int idSolicitud)
        {
            return await _context.Set<Entrega>()
                .AsNoTracking()
                .Include(entrega =>
                    entrega.Repeticionentrega)
                .FirstOrDefaultAsync(entrega =>
                    entrega.IdSolicitud == idSolicitud);
        }

        public async Task AgregarAsync(
            Entrega entrega)
        {
            await _context.Set<Entrega>()
                .AddAsync(entrega);

            await _context.SaveChangesAsync();
        }
    }
}