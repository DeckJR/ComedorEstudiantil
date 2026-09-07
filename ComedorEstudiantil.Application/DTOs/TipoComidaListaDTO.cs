namespace ComedorEstudiantil.Application.DTOs
{
    public class TipoComidaListaDTO
    {
        public int IdTipoComida { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public TimeOnly HoraLimiteMarcar { get; set; }
        public bool Activo { get; set; }
    }
}