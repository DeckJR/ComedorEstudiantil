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
    public class RepositoryMenu : IRepositoryMenu
    {
        private readonly ComedorEstudiantilContext _context;

        public RepositoryMenu(ComedorEstudiantilContext context)
        {
            _context = context;
        }

        public async Task<List<Menu>> ListarAsync()
        {
            return await _context.Set<Menu>()
                .AsNoTracking()
                .Include(menu => menu.IdTipoComidaNavigation)
                .Include(menu => menu.IdActividadNavigation)
                .Include(menu => menu.IdUsuarioCreadorNavigation)
                .OrderByDescending(menu => menu.Fecha)
                .ThenBy(menu =>
                    menu.IdTipoComidaNavigation.HoraLimiteMarcar)
                .ToListAsync();
        }

        public async Task<List<Menu>> ListarPublicadosPorFechaAsync(
            DateOnly fecha)
        {
            return await _context.Set<Menu>()
                .AsNoTracking()
                .Include(menu => menu.IdTipoComidaNavigation)
                .Include(menu => menu.IdActividadNavigation)
                .Include(menu => menu.IdUsuarioCreadorNavigation)
                .Where(menu =>
                    menu.Fecha == fecha &&
                    menu.Publicado &&
                    menu.IdTipoComidaNavigation.Activo == true &&
                    (menu.IdActividadNavigation == null ||
                     menu.IdActividadNavigation.Activo == true))
                .OrderBy(menu =>
                    menu.IdTipoComidaNavigation.HoraLimiteMarcar)
                .ToListAsync();
        }

        public async Task<Menu?> BuscarPorIdAsync(int idMenu)
        {
            return await _context.Set<Menu>()
                .AsNoTracking()
                .Include(menu => menu.IdTipoComidaNavigation)
                .Include(menu => menu.IdActividadNavigation)
                .Include(menu => menu.IdUsuarioCreadorNavigation)
                .FirstOrDefaultAsync(menu =>
                    menu.IdMenu == idMenu);
        }

        public async Task<Menu?> BuscarPorIdParaEdicionAsync(
            int idMenu)
        {
            return await _context.Set<Menu>()
                .Include(menu => menu.IdTipoComidaNavigation)
                .Include(menu => menu.IdActividadNavigation)
                .FirstOrDefaultAsync(menu =>
                    menu.IdMenu == idMenu);
        }

        public async Task<bool> ExisteAsync(
            DateOnly fecha,
            int idTipoComida,
            int idActividad,
            int? idMenuExcluir = null)
        {
            return await _context.Set<Menu>()
                .AnyAsync(menu =>
                    menu.Fecha == fecha &&
                    menu.IdTipoComida == idTipoComida &&
                    menu.IdActividad == idActividad &&
                    (!idMenuExcluir.HasValue ||
                     menu.IdMenu != idMenuExcluir.Value));
        }

        public async Task AgregarAsync(Menu menu)
        {
            await _context.Set<Menu>().AddAsync(menu);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}