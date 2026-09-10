using ComedorEstudiantil.Infraestructure.Data;
using ComedorEstudiantil.Infraestructure.Models;
using ComedorEstudiantil.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ComedorEstudiantil.Infraestructure.Repository.Implementations
{
    public class RepositoryEstudiante
        : IRepositoryEstudiante
    {
        private readonly ComedorEstudiantilContext
            _context;

        public RepositoryEstudiante(
            ComedorEstudiantilContext context)
        {
            _context = context;
        }

        public async Task<List<Estudiante>> ListarAsync(
    bool incluirArchivados = false)
        {
            IQueryable<Estudiante> consulta =
                _context.Set<Estudiante>()
                    .AsNoTracking()
                    .Include(estudiante =>
                        estudiante.IdUsuarioNavigation)
                    .Include(estudiante =>
                        estudiante
                            .IdTipoBeneficiarioNavigation)
                    .Include(estudiante =>
                        estudiante
                            .IdGradoSeccionNavigation);

            if (!incluirArchivados)
            {
                consulta = consulta.Where(estudiante =>
                    estudiante.Activo == true &&
                    estudiante.IdUsuarioNavigation.Activo == true);
            }

            return await consulta
                .OrderBy(estudiante =>
                    estudiante
                        .IdUsuarioNavigation
                        .Apellidos)
                .ThenBy(estudiante =>
                    estudiante
                        .IdUsuarioNavigation
                        .Nombre)
                .ToListAsync();
        }

        public async Task<Estudiante?>
            BuscarPorIdAsync(
                int idEstudiante)
        {
            return await _context.Set<Estudiante>()
                .AsNoTracking()
                .Include(estudiante =>
                    estudiante.IdUsuarioNavigation)
                    .ThenInclude(usuario =>
                        usuario.IdRolNavigation)
                .Include(estudiante =>
                    estudiante
                        .IdTipoBeneficiarioNavigation)
                .Include(estudiante =>
                    estudiante
                        .IdGradoSeccionNavigation)
                .FirstOrDefaultAsync(estudiante =>
                    estudiante.IdEstudiante ==
                    idEstudiante);
        }

        public async Task<Estudiante?>
            BuscarPorIdParaEdicionAsync(
                int idEstudiante)
        {
            return await _context.Set<Estudiante>()
                .Include(estudiante =>
                    estudiante.IdUsuarioNavigation)
                    .ThenInclude(usuario =>
                        usuario.IdRolNavigation)
                .Include(estudiante =>
                    estudiante
                        .IdTipoBeneficiarioNavigation)
                .Include(estudiante =>
                    estudiante
                        .IdGradoSeccionNavigation)
                .FirstOrDefaultAsync(estudiante =>
                    estudiante.IdEstudiante ==
                    idEstudiante);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}