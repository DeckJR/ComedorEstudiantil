using System.ComponentModel.DataAnnotations;

namespace ComedorEstudiantil.Application.DTOs
{
    public class GradoSeccionFormularioDTO
    {
        public int IdGradoSeccion { get; set; }

        [Required(
            ErrorMessage = "El grado es obligatorio.")]
        [StringLength(
            20,
            ErrorMessage =
                "El grado no puede superar los 20 caracteres.")]
        public string Grado { get; set; } = string.Empty;

        [Required(
            ErrorMessage = "La sección es obligatoria.")]
        [StringLength(
            10,
            ErrorMessage =
                "La sección no puede superar los 10 caracteres.")]
        [Display(Name = "Sección")]
        public string Seccion { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
    }
}