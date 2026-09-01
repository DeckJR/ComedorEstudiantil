using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComedorEstudiantil.Infraestructure.Models;

namespace ComedorEstudiantil.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryReporte
    {
        Task<List<Solicitud>> ListarSolicitudesAsync(
            DateOnly fechaInicio,
            DateOnly fechaFin);

        Task<List<Entrega>> ListarEntregasAsync(
            DateTime fechaInicio,
            DateTime fechaFin);
    }
}