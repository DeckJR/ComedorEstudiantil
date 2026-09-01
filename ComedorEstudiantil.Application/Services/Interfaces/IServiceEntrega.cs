using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComedorEstudiantil.Application.DTOs;

namespace ComedorEstudiantil.Application.Services.Interfaces
{
    public interface IServiceEntrega
    {
        Task<List<EntregaListaDTO>> ListarDelDiaAsync();
        Task<RegistroEntregaDTO> BuscarSolicitudesPendientesAsync(
            string? identificacion);
        Task<ResultadoOperacionDTO> RegistrarPorFuncionarioAsync(
            int idSolicitud,
            int idUsuarioFuncionario);
    }
}