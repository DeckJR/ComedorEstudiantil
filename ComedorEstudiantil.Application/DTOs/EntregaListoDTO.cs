using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComedorEstudiantil.Application.DTOs
{
    public class EntregaListaDTO
    {
        public int IdEntrega { get; set; }
        public string Identificacion { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string TipoComida { get; set; } = string.Empty;
        public string DescripcionMenu { get; set; } = string.Empty;
        public DateTime FechaHoraEntrega { get; set; }
        public string EntregadoPor { get; set; } = string.Empty;
        public string MetodoEntrega { get; set; } = string.Empty;
    }
}
