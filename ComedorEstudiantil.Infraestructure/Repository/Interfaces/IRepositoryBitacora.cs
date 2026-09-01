using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComedorEstudiantil.Infraestructure.Models;

namespace ComedorEstudiantil.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryBitacora
    {
        Task AgregarAsync(Bitacora bitacora);

        Task<List<Bitacora>> ListarAsync(
            DateTime fechaInicio,
            DateTime fechaFinExclusiva,
            string? usuario,
            string? accion,
            string? entidad);
    }
}