using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ComedorEstudiantil.Application.DTOs
{
    public class RegistroEntregaDTO
    {
        [StringLength(20, ErrorMessage = "La identificación no puede superar los 20 caracteres.")]
        [Display(Name = "Identificación")]
        public string? Identificacion { get; set; }

        public string? NombreUsuario { get; set; }
        public bool BusquedaRealizada { get; set; }
        public List<SolicitudPendienteEntregaDTO> Solicitudes { get; set; } = new();
    }
}