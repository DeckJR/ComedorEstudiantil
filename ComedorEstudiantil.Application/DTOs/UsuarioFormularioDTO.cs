using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComedorEstudiantil.Application.DTOs
{
    public class UsuarioFormularioDTO
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [StringLength(100, ErrorMessage = "Los apellidos no pueden superar los 100 caracteres.")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "La identificación es obligatoria.")]
        [StringLength(20, ErrorMessage = "La identificación no puede superar los 20 caracteres.")]
        [Display(Name = "Identificación")]
        public string Identificacion { get; set; } = string.Empty;

        [Display(Name = "Código de barras")]
        public string? CodigoBarras { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres.")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar un rol.")]
        [Display(Name = "Rol")]
        public int IdRol { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe contener al menos 8 caracteres.")]
        [Display(Name = "Contraseña")]
        public string? Contrasena { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Contrasena), ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña")]
        public string? ConfirmarContrasena { get; set; }

        [Display(Name = "Tipo de beneficiario")]
        public int? IdTipoBeneficiario { get; set; }

        [Display(Name = "Grado y sección")]
        public int? IdGradoSeccion { get; set; }

        [Range(2000, 2100, ErrorMessage = "El año de ingreso no es válido.")]
        [Display(Name = "Año de ingreso")]
        public short? AnioIngreso { get; set; }

        public bool Activo { get; set; } = true;
        public int IdRolEstudiante { get; set; }
        public List<CatalogoDTO> Roles { get; set; } = new();
        public List<CatalogoDTO> TiposBeneficiario { get; set; } = new();
        public List<CatalogoDTO> GradosSecciones { get; set; } = new();
    }
}