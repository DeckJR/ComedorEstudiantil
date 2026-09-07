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

        public async Task<List<Tipocomida>> ListarAsync()
        {
            return await _context.Set<Tipocomida>()
                .AsNoTracking()
                .OrderBy(tipoComida =>
                    tipoComida.HoraLimiteMarcar)
                .ThenBy(tipoComida =>
                    tipoComida.Nombre)
                .ToListAsync();
        }

        public async Task<List<Tipocomida>> ListarActivosAsync()
        {
            return await _context.Set<Tipocomida>()
                .AsNoTracking()
                .Where(tipoComida =>
                    tipoComida.Activo == true)
                .OrderBy(tipoComida =>
                    tipoComida.HoraLimiteMarcar)
                .ThenBy(tipoComida =>
                    tipoComida.Nombre)
                .ToListAsync();
        }

        public async Task<Tipocomida?> BuscarPorIdAsync(
            int idTipoComida)
        {
            return await _context.Set<Tipocomida>()
                .AsNoTracking()
                .FirstOrDefaultAsync(tipoComida =>
                    tipoComida.IdTipoComida ==
                    idTipoComida);
        }

        public async Task<Tipocomida?>
            BuscarPorIdParaEdicionAsync(
                int idTipoComida)
        {
            return await _context.Set<Tipocomida>()
                .FirstOrDefaultAsync(tipoComida =>
                    tipoComida.IdTipoComida ==
                    idTipoComida);
        }

        public async Task<bool> ExisteNombreAsync(
            string nombre,
            int? idTipoComidaExcluir = null)
        {
            return await _context.Set<Tipocomida>()
                .AnyAsync(tipoComida =>
                    tipoComida.Nombre == nombre &&
                    (!idTipoComidaExcluir.HasValue ||
                     tipoComida.IdTipoComida !=
                     idTipoComidaExcluir.Value));
        }

        public async Task AgregarAsync(
            Tipocomida tipoComida)
        {
            await _context.Set<Tipocomida>()
                .AddAsync(tipoComida);

            await _context.SaveChangesAsync();
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}