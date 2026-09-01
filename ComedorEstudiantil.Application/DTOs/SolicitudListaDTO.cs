using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComedorEstudiantil.Application.DTOs
{
    public class SolicitudListaDTO
    {
        public int IdSolicitud { get; set; }
        public DateOnly FechaMenu { get; set; }
        public string TipoComida { get; set; } = string.Empty;
        public string DescripcionMenu { get; set; } = string.Empty;
        public string? Actividad { get; set; }
        public DateTime FechaHoraSolicitud { get; set; }
        public string Estado { get; set; } = string.Empty;
        public bool Activa { get; set; }
        public bool PuedeCancelar { get; set; }
        public bool Entregada { get; set; }
    }
}
