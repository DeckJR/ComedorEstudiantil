using System.ComponentModel.DataAnnotations;

namespace ComedorEstudiantil.Application.DTOs
{
    public class RegistroEntregaDTO
    {
        [StringLength(20, ErrorMessage = "La identificación no puede superar los 20 caracteres.")]
        [Display(Name = "Identificación")]
        public string? Identificacion { get; set; }

        [Display(Name = "Menú que se está entregando")]
        public int? IdMenuSeleccionado { get; set; }

        [StringLength(50, ErrorMessage = "El código de barras no puede superar los 50 caracteres.")]
        [Display(Name = "Código de barras")]
        public string? CodigoBarras { get; set; }

        public string? NombreUsuario { get; set; }
        public string? MensajeBusqueda { get; set; }
        public bool BusquedaRealizada { get; set; }
        public List<CatalogoDTO> MenusDisponibles { get; set; } = new();
        public List<SolicitudPendienteEntregaDTO> Solicitudes { get; set; } = new();
    }
}