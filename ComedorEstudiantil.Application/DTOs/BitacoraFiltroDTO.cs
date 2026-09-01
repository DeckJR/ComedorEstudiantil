using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ComedorEstudiantil.Application.DTOs
{
    public class BitacoraFiltroDTO
    {
        [Display(Name = "Fecha inicial")]
        [DataType(DataType.Date)]
        public DateOnly FechaInicio { get; set; }

        [Display(Name = "Fecha final")]
        [DataType(DataType.Date)]
        public DateOnly FechaFin { get; set; }

        [Display(Name = "Usuario")]
        public string? Usuario { get; set; }

        [Display(Name = "Acción")]
        public string? Accion { get; set; }

        [Display(Name = "Entidad")]
        public string? Entidad { get; set; }
    }
}