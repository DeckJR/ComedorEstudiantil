using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ComedorEstudiantil.Application.DTOs
{
    public class MenuFormularioDTO
    {
        public int IdMenu { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de comida.")]
        [Display(Name = "Tipo de comida")]
        public int IdTipoComida { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [DataType(DataType.Date)]
        public DateOnly Fecha { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Required(ErrorMessage = "La descripción del menú es obligatoria.")]
        [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
        [Display(Name = "Descripción del menú")]
        public string Descripcion { get; set; } = string.Empty;

        [Display(Name = "Actividad especial")]
        public int? IdActividad { get; set; }

        public bool Publicado { get; set; }
        public List<CatalogoDTO> TiposComida { get; set; } = new();
        public List<CatalogoDTO> Actividades { get; set; } = new();
    }
}