using ComedorEstudiantil.Infraestructure.Data;
using ComedorEstudiantil.Infraestructure.Models;
using ComedorEstudiantil.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ComedorEstudiantil.Infraestructure.Repository.Implementations
{
    public class RepositoryRepeticionEntrega
        : IRepositoryRepeticionEntrega
    {
        private readonly ComedorEstudiantilContext _context;

        public RepositoryRepeticionEntrega(
            ComedorEstudiantilContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteRepeticionRecienteAsync(
            int idEntrega,
            DateTime fechaDesde)
        {
            return await _context.Set<Repeticionentrega>()
                .AsNoTracking()
                .AnyAsync(repeticion =>
                    repeticion.IdEntrega == idEntrega &&
                    repeticion.FechaHoraRepeticion >= fechaDesde);
        }

        public async Task AgregarAsync(
            Repeticionentrega repeticion)
        {
            await _context.Set<Repeticionentrega>()
                .AddAsync(repeticion);

            await _context.SaveChangesAsync();
        }
    }
}