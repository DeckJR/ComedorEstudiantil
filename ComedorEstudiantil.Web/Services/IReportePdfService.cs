using ComedorEstudiantil.Application.DTOs;

namespace ComedorEstudiantil.Web.Services
{
    public interface IReportePdfService
    {
        byte[] GenerarSolicitudes(
            ReporteGeneralDTO reporte);

        byte[] GenerarEntregas(
            ReporteGeneralDTO reporte);
    }
}