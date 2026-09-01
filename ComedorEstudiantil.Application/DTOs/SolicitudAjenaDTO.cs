using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ComedorEstudiantil.Application.DTOs
{
    public class SolicitudAjenaDTO
    {
        [Range(1, int.MaxValue)]
        public int IdMenu { get; set; }

        [Required(ErrorMessage = "La identificación es obligatoria.")]
        [StringLength(20, ErrorMessage = "La identificación no puede superar los 20 caracteres.")]
        [Display(Name = "Identificación de la persona")]
        public string Identificacion { get; set; } = string.Empty;

        public string TipoComida { get; set; } = string.Empty;
        public string DescripcionMenu { get; set; } = string.Empty;
        public DateOnly FechaMenu { get; set; }
    }
}