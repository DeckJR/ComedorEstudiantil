using System.ComponentModel.DataAnnotations;

namespace ComedorEstudiantil.Application.DTOs
{
    public class TipoComidaFormularioDTO
    {
        public int IdTipoComida { get; set; }

        [Required(
            ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(
            30,
            ErrorMessage =
                "El nombre no puede superar los 30 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(
            ErrorMessage = "La hora límite es obligatoria.")]
        [Display(Name = "Hora límite para solicitar")]
        [DataType(DataType.Time)]
        public TimeOnly? HoraLimiteMarcar { get; set; }

        public bool Activo { get; set; } = true;
    }
}