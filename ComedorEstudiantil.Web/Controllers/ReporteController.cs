using ComedorEstudiantil.Application.DTOs;
using ComedorEstudiantil.Application.Services.Interfaces;
using ComedorEstudiantil.Web.Authorization;
using ComedorEstudiantil.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComedorEstudiantil.Web.Controllers
{
    [Authorize(Policy = PoliticasAutorizacion.VerReportes)]
    public class ReporteController : Controller
    {
        private readonly IServiceReporte _serviceReporte;
        private readonly IReportePdfService _reportePdfService;
        private readonly IFechaHoraService _fechaHoraService;
        private readonly IServiceBitacora _serviceBitacora;
        private readonly ILogger<ReporteController> _logger;

        public ReporteController(
            IServiceReporte serviceReporte,
            IReportePdfService reportePdfService,
            IFechaHoraService fechaHoraService,
            IServiceBitacora serviceBitacora,
            ILogger<ReporteController> logger)
        {
            _serviceReporte = serviceReporte;
            _reportePdfService = reportePdfService;
            _fechaHoraService = fechaHoraService;
            _serviceBitacora = serviceBitacora;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            FiltroReporteDTO? filtro)
        {
            filtro ??= new FiltroReporteDTO();

            if (filtro.FechaInicio == default ||
                filtro.FechaFin == default)
            {
                DateOnly fechaActual =
                    _fechaHoraService.ObtenerFechaActual();

                filtro.FechaInicio = fechaActual;
                filtro.FechaFin = fechaActual;
            }

            ReporteGeneralDTO reporte =
                await _serviceReporte.GenerarAsync(filtro);

            return View(reporte);
        }

        [HttpGet]
        public async Task<IActionResult> SolicitudesPdf(
            FiltroReporteDTO filtro)
        {
            ReporteGeneralDTO reporte =
                await _serviceReporte.GenerarAsync(filtro);

            byte[] archivo =
                _reportePdfService.GenerarSolicitudes(reporte);

            string nombre =
                $"solicitudes-{filtro.FechaInicio:yyyyMMdd}-{filtro.FechaFin:yyyyMMdd}.pdf";

            await RegistrarDescargaAsync(
    "DescargaReporteSolicitudesPdf",
    filtro);

            return File(
                archivo,
                "application/pdf",
                nombre);
        }

        [HttpGet]
        public async Task<IActionResult> EntregasPdf(
            FiltroReporteDTO filtro)
        {
            ReporteGeneralDTO reporte =
                await _serviceReporte.GenerarAsync(filtro);

            byte[] archivo =
                _reportePdfService.GenerarEntregas(reporte);

            string nombre =
                $"entregas-{filtro.FechaInicio:yyyyMMdd}-{filtro.FechaFin:yyyyMMdd}.pdf";

            await RegistrarDescargaAsync(
    "DescargaReporteSolicitudesPdf",
    filtro);

            return File(
                archivo,
                "application/pdf",
                nombre);
        }
        private async Task RegistrarDescargaAsync(
    string accion,
    FiltroReporteDTO filtro)
        {
            try
            {
                int? idUsuario = null;

                string? valor =
                    User.FindFirst(
                        System.Security.Claims.ClaimTypes
                            .NameIdentifier)?
                        .Value;

                if (int.TryParse(
                    valor,
                    out int idConvertido))
                {
                    idUsuario = idConvertido;
                }

                await _serviceBitacora.RegistrarAsync(
                    idUsuario,
                    accion,
                    "Reporte",
                    null,
                    $"Periodo consultado: {filtro.FechaInicio:dd/MM/yyyy} al {filtro.FechaFin:dd/MM/yyyy}.",
                    HttpContext.Connection
                        .RemoteIpAddress?
                        .ToString());
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "No fue posible registrar la descarga del reporte.");
            }
        }
    }
}