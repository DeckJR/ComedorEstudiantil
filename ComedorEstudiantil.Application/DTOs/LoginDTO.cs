using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComedorEstudiantil.Application.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "La identificación es obligatoria.")]
        [StringLength(20, ErrorMessage = "La identificación no puede superar los 20 caracteres.")]
        [Display(Name = "Identificación")]
        public string Identificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; } = string.Empty;

        [Display(Name = "Mantener la sesión iniciada")]
        public bool Recordarme { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
