using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComedorEstudiantil.Application.DTOs
{
    public class SolicitudPendienteEntregaDTO
    {
        public int IdSolicitud { get; set; }
        public DateOnly FechaMenu { get; set; }
        public string TipoComida { get; set; } = string.Empty;
        public string DescripcionMenu { get; set; } = string.Empty;
        public DateTime FechaHoraSolicitud { get; set; }
    }
}