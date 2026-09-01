using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComedorEstudiantil.Application.DTOs
{
    public class BitacoraListaDTO
    {
        public long IdBitacora { get; set; }
        public int? IdUsuario { get; set; }
        public string IdentificacionUsuario { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string Entidad { get; set; } = string.Empty;
        public int? IdEntidad { get; set; }
        public string Detalle { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        public string IpOrigen { get; set; } = string.Empty;
    }
}