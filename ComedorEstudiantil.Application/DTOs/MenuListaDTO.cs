using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComedorEstudiantil.Application.DTOs
{
    public class MenuListaDTO
    {
        public int IdMenu { get; set; }
        public DateOnly Fecha { get; set; }
        public string TipoComida { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? Actividad { get; set; }
        public TimeOnly HoraLimiteMarcar { get; set; }
        public bool Publicado { get; set; }
        public string CreadoPor { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public bool SolicitudActiva { get; set; }
        public int? IdSolicitud { get; set; }
        public bool PuedeSolicitar { get; set; }
        public string MensajeDisponibilidad { get; set; } = string.Empty;
    }
}