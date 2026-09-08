namespace ComedorEstudiantil.Application.DTOs
{
    public class EstudianteListaDTO
    {
        public int IdUsuario { get; set; }
        public int IdEstudiante { get; set; }
        public string Identificacion { get; set; } = string.Empty;
        public string CodigoBarras { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string TipoBeneficiario { get; set; } = string.Empty;
        public string GradoSeccion { get; set; } = string.Empty;
        public short AnioIngreso { get; set; }
        public bool Activo { get; set; }
    }
}