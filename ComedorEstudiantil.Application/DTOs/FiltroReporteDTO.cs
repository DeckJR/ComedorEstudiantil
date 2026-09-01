using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ComedorEstudiantil.Application.DTOs
{
    public class FiltroReporteDTO
    {
        [Display(Name = "Fecha inicial")]
        [DataType(DataType.Date)]
        public DateOnly FechaInicio { get; set; }

        [Display(Name = "Fecha final")]
        [DataType(DataType.Date)]
        public DateOnly FechaFin { get; set; }

        [Display(Name = "Tipo de comida")]
        public string? TipoComida { get; set; }

        [Display(Name = "Estado")]
        public sbyte? Estado { get; set; }
    }
}