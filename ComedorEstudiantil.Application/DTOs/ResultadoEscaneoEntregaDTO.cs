namespace ComedorEstudiantil.Application.DTOs
{
    public class ResultadoEscaneoEntregaDTO
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public bool RequiereConfirmarRepeticion { get; set; }
        public int? IdEntrega { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Identificacion { get; set; }
    }
}