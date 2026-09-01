using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComedorEstudiantil.Infraestructure.Models;

namespace ComedorEstudiantil.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryEntrega
    {
        Task<List<Entrega>> ListarPorPeriodoAsync(
            DateTime inicio,
            DateTime final);
        Task<Entrega?> BuscarPorSolicitudAsync(int idSolicitud);
        Task AgregarAsync(Entrega entrega);
    }
}