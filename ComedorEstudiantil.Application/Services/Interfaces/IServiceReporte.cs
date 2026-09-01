using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComedorEstudiantil.Application.DTOs;

namespace ComedorEstudiantil.Application.Services.Interfaces
{
    public interface IServiceReporte
    {
        Task<ReporteGeneralDTO> GenerarAsync(
            FiltroReporteDTO filtro);
    }
}