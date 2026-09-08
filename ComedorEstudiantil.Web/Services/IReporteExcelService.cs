using ComedorEstudiantil.Application.DTOs;

namespace ComedorEstudiantil.Web.Services
{
    public interface IReporteExcelService
    {
        byte[] GenerarSolicitudes(
            ReporteGeneralDTO reporte);

        byte[] GenerarEntregas(
            ReporteGeneralDTO reporte);
    }
}