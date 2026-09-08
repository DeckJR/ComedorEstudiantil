namespace ComedorEstudiantil.Application.DTOs
{
    public class UsuarioSesionDTO
    {
        public int IdUsuario { get; set; }
        public string Identificacion { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public bool DebeCambiarContrasena { get; set; }
        public DateTime? FechaUltimoCambioContrasena { get; set; }
    }
}