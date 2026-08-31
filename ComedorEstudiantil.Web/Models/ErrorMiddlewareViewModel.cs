namespace ComedorEstudiantil.Web.Models
{
    public class ErrorMiddlewareViewModel
    {
        public string IdEvento { get; set; } = string.Empty;

        public string Mensaje { get; set; } =
            "Ocurrió un error inesperado al procesar la solicitud.";
    }
}