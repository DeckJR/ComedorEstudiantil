using ComedorEstudiantil.Application.Services.Interfaces;

namespace ComedorEstudiantil.Web.Services
{
    public class FechaHoraService : IFechaHoraService
    {
        private readonly TimeZoneInfo _zonaHoraria;

        public FechaHoraService()
        {
            string identificadorZonaHoraria =
                OperatingSystem.IsWindows()
                    ? "Central America Standard Time"
                    : "America/Costa_Rica";

            _zonaHoraria = TimeZoneInfo.FindSystemTimeZoneById(
                identificadorZonaHoraria);
        }

        public DateTime ObtenerAhora()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                _zonaHoraria);
        }

        public DateOnly ObtenerFechaActual()
        {
            DateTime ahora = ObtenerAhora();

            return DateOnly.FromDateTime(ahora);
        }
    }
}