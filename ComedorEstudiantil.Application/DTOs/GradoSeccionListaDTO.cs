namespace ComedorEstudiantil.Application.DTOs
{
    public class GradoSeccionListaDTO
    {
        public int IdGradoSeccion { get; set; }
        public string Grado { get; set; } = string.Empty;
        public string Seccion { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}