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
    public class RepositorySolicitud : IRepositorySolicitud
    {
        private readonly ComedorEstudiantilContext _context;

        public RepositorySolicitud(
            ComedorEstudiantilContext context)
        {
            _context = context;
        }

        public async Task<List<Solicitud>> ListarPorUsuarioAsync(
            int idUsuario)
        {
            return await _context.Set<Solicitud>()
                .AsNoTracking()
                .Include(solicitud => solicitud.IdMenuNavigation)
                    .ThenInclude(menu =>
                        menu.IdTipoComidaNavigation)
                .Include(solicitud => solicitud.IdMenuNavigation)
                    .ThenInclude(menu =>
                        menu.IdActividadNavigation)
                .Include(solicitud => solicitud.Entrega)
                .Where(solicitud =>
                    solicitud.IdUsuario == idUsuario)
                .OrderByDescending(solicitud =>
                    solicitud.IdMenuNavigation.Fecha)
                .ThenByDescending(solicitud =>
                    solicitud.FechaHoraSolicitud)
                .ToListAsync();
        }

        public async Task<List<Solicitud>>
            ListarPorUsuarioYMenusAsync(
                int idUsuario,
                List<int> idsMenus)
        {
            return await _context.Set<Solicitud>()
                .AsNoTracking()
                .Where(solicitud =>
                    solicitud.IdUsuario == idUsuario &&
                    idsMenus.Contains(solicitud.IdMenu))
                .ToListAsync();
        }

        public async Task<Solicitud?> BuscarPorUsuarioYMenuAsync(
            int idUsuario,
            int idMenu)
        {
            return await _context.Set<Solicitud>()
                .Include(solicitud => solicitud.IdMenuNavigation)
                    .ThenInclude(menu =>
                        menu.IdTipoComidaNavigation)
                .Include(solicitud => solicitud.IdMenuNavigation)
                    .ThenInclude(menu =>
                        menu.IdActividadNavigation)
                .FirstOrDefaultAsync(solicitud =>
                    solicitud.IdUsuario == idUsuario &&
                    solicitud.IdMenu == idMenu);
        }

        public async Task<Solicitud?> BuscarPorIdYUsuarioAsync(int idSolicitud,int idUsuario)
        {
            return await _context.Set<Solicitud>()
                .Include(solicitud => solicitud.IdMenuNavigation)
                    .ThenInclude(menu =>
                        menu.IdTipoComidaNavigation)
                .Include(solicitud => solicitud.IdMenuNavigation)
                    .ThenInclude(menu =>
                        menu.IdActividadNavigation)
                .Include(solicitud => solicitud.Entrega)
                .FirstOrDefaultAsync(solicitud =>
                    solicitud.IdSolicitud == idSolicitud &&
                    solicitud.IdUsuario == idUsuario);
        }

        public async Task AgregarAsync(Solicitud solicitud)
        {
            await _context.Set<Solicitud>().AddAsync(solicitud);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<List<Solicitud>>
    ListarActivasPorUsuarioYFechaAsync(
        int idUsuario,
        DateOnly fecha)
        {
            return await _context.Set<Solicitud>()
                .AsNoTracking()
                .Include(solicitud => solicitud.IdMenuNavigation)
                    .ThenInclude(menu =>
                        menu.IdTipoComidaNavigation)
                .Include(solicitud => solicitud.Entrega)
                .Where(solicitud =>
                    solicitud.IdUsuario == idUsuario &&
                    solicitud.IdMenuNavigation.Fecha == fecha &&
                    solicitud.Estado == 0 &&
                    solicitud.Entrega == null)
                .OrderBy(solicitud =>
                    solicitud.IdMenuNavigation
                        .IdTipoComidaNavigation
                        .HoraLimiteMarcar)
                .ToListAsync();
        }

        public async Task<Solicitud?> BuscarPorIdAsync(
            int idSolicitud)
        {
            return await _context.Set<Solicitud>()
                .Include(solicitud => solicitud.IdMenuNavigation)
                    .ThenInclude(menu =>
                        menu.IdTipoComidaNavigation)
                .Include(solicitud => solicitud.IdUsuarioNavigation)
                .Include(solicitud => solicitud.Entrega)
                .FirstOrDefaultAsync(solicitud =>
                    solicitud.IdSolicitud == idSolicitud);
        }
    }
}
