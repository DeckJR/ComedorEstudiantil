using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ComedorEstudiantil.Application.DTOs
{
    public class ReporteGeneralDTO
    {
        public FiltroReporteDTO Filtro { get; set; } = new();
        public List<string> TiposComida { get; set; } = new();
        public List<ReporteSolicitudDTO> Solicitudes { get; set; } = new();
        public List<ReporteEntregaDTO> Entregas { get; set; } = new();
        public int TotalSolicitudes { get; set; }
        public int TotalActivas { get; set; }
        public int TotalCanceladas { get; set; }
        public int TotalEntregadas { get; set; }
        public int TotalPendientes { get; set; }
    }
}