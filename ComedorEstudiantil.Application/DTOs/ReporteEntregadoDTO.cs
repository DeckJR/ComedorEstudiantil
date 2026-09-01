using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ComedorEstudiantil.Application.DTOs
{
    public class ReporteEntregaDTO
    {
        public int IdEntrega { get; set; }
        public DateOnly FechaMenu { get; set; }
        public string TipoComida { get; set; } = string.Empty;
        public string DescripcionMenu { get; set; } = string.Empty;
        public string Identificacion { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string TipoBeneficiario { get; set; } = string.Empty;
        public string GradoSeccion { get; set; } = string.Empty;
        public DateTime FechaHoraEntrega { get; set; }
        public string MetodoEntrega { get; set; } = string.Empty;
        public string EntregadoPor { get; set; } = string.Empty;
    }
}