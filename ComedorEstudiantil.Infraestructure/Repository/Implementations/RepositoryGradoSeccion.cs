using ComedorEstudiantil.Infraestructure.Data;
using ComedorEstudiantil.Infraestructure.Models;
using ComedorEstudiantil.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ComedorEstudiantil.Infraestructure.Repository.Implementations
{
    public class RepositoryGradoSeccion :
        IRepositoryGradoSeccion
    {
        private readonly ComedorEstudiantilContext _context;

        public RepositoryGradoSeccion(
            ComedorEstudiantilContext context)
        {
            _context = context;
        }

        public async Task<List<Gradoseccion>> ListarAsync()
        {
            return await _context.Set<Gradoseccion>()
                .AsNoTracking()
                .Where(gradoSeccion =>
                    gradoSeccion.Activo == true)
                .OrderBy(gradoSeccion =>
                    gradoSeccion.Grado.Length)
                .ThenBy(gradoSeccion =>
                    gradoSeccion.Grado)
                .ThenBy(gradoSeccion =>
                    gradoSeccion.Seccion)
                .ToListAsync();
        }

        public async Task<List<Gradoseccion>>
            ListarTodosAsync()
        {
            return await _context.Set<Gradoseccion>()
                .AsNoTracking()
                .OrderBy(gradoSeccion =>
                    gradoSeccion.Grado.Length)
                .ThenBy(gradoSeccion =>
                    gradoSeccion.Grado)
                .ThenBy(gradoSeccion =>
                    gradoSeccion.Seccion)
                .ToListAsync();
        }

        public async Task<Gradoseccion?> BuscarPorIdAsync(
            int idGradoSeccion)
        {
            return await _context.Set<Gradoseccion>()
                .AsNoTracking()
                .FirstOrDefaultAsync(gradoSeccion =>
                    gradoSeccion.IdGradoSeccion ==
                    idGradoSeccion);
        }

        public async Task<Gradoseccion?>
            BuscarPorIdParaEdicionAsync(
                int idGradoSeccion)
        {
            return await _context.Set<Gradoseccion>()
                .FirstOrDefaultAsync(gradoSeccion =>
                    gradoSeccion.IdGradoSeccion ==
                    idGradoSeccion);
        }

        public async Task<bool> ExisteAsync(
            int idGradoSeccion)
        {
            return await _context.Set<Gradoseccion>()
                .AnyAsync(gradoSeccion =>
                    gradoSeccion.IdGradoSeccion ==
                    idGradoSeccion);
        }

        public async Task<bool> ExisteCombinacionAsync(
            string grado,
            string seccion,
            int? idGradoSeccionExcluir = null)
        {
            return await _context.Set<Gradoseccion>()
                .AnyAsync(gradoSeccion =>
                    gradoSeccion.Grado == grado &&
                    gradoSeccion.Seccion == seccion &&
                    (!idGradoSeccionExcluir.HasValue ||
                     gradoSeccion.IdGradoSeccion !=
                     idGradoSeccionExcluir.Value));
        }

        public async Task AgregarAsync(
            Gradoseccion gradoSeccion)
        {
            await _context.Set<Gradoseccion>()
                .AddAsync(gradoSeccion);

            await _context.SaveChangesAsync();
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}