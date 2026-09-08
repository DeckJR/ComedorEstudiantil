using System.Security.Claims;
using ComedorEstudiantil.Application.DTOs;
using ComedorEstudiantil.Application.Services.Interfaces;
using ComedorEstudiantil.Web.Authorization;
using ComedorEstudiantil.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComedorEstudiantil.Web.Controllers
{
    [Authorize(
        Policy = PoliticasAutorizacion.VerReportes)]
    public class ReporteController : Controller
    {
        private readonly IServiceReporte
            _serviceReporte;

        private readonly IReportePdfService
            _reportePdfService;

        private readonly IReporteExcelService
            _reporteExcelService;

        private readonly IFechaHoraService
            _fechaHoraService;

        private readonly IServiceBitacora
            _serviceBitacora;

        private readonly ILogger<ReporteController>
            _logger;

        public ReporteController(
            IServiceReporte serviceReporte,
            IReportePdfService reportePdfService,
            IReporteExcelService reporteExcelService,
            IFechaHoraService fechaHoraService,
            IServiceBitacora serviceBitacora,
            ILogger<ReporteController> logger)
        {
            _serviceReporte = serviceReporte;
            _reportePdfService = reportePdfService;
            _reporteExcelService = reporteExcelService;
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
                await _serviceReporte.GenerarAsync(
                    filtro);

            return View(reporte);
        }

        [HttpGet]
        public async Task<IActionResult> SolicitudesPdf(
            FiltroReporteDTO filtro)
        {
            ReporteGeneralDTO reporte =
                await _serviceReporte.GenerarAsync(
                    filtro);

            byte[] archivo =
                _reportePdfService
                    .GenerarSolicitudes(reporte);

            string nombreArchivo =
                $"solicitudes-{filtro.FechaInicio:yyyyMMdd}-{filtro.FechaFin:yyyyMMdd}.pdf";

            await RegistrarDescargaAsync(
                "DescargaReporteSolicitudesPdf",
                filtro);

            return File(
                archivo,
                "application/pdf",
                nombreArchivo);
        }

        [HttpGet]
        public async Task<IActionResult> EntregasPdf(
            FiltroReporteDTO filtro)
        {
            ReporteGeneralDTO reporte =
                await _serviceReporte.GenerarAsync(
                    filtro);

            byte[] archivo =
                _reportePdfService
                    .GenerarEntregas(reporte);

            string nombreArchivo =
                $"entregas-{filtro.FechaInicio:yyyyMMdd}-{filtro.FechaFin:yyyyMMdd}.pdf";

            await RegistrarDescargaAsync(
                "DescargaReporteEntregasPdf",
                filtro);

            return File(
                archivo,
                "application/pdf",
                nombreArchivo);
        }

        [HttpGet]
        public async Task<IActionResult> SolicitudesExcel(
            FiltroReporteDTO filtro)
        {
            ReporteGeneralDTO reporte =
                await _serviceReporte.GenerarAsync(
                    filtro);

            byte[] archivo =
                _reporteExcelService
                    .GenerarSolicitudes(reporte);

            string nombreArchivo =
                $"solicitudes-{filtro.FechaInicio:yyyyMMdd}-{filtro.FechaFin:yyyyMMdd}.xlsx";

            await RegistrarDescargaAsync(
                "DescargaReporteSolicitudesExcel",
                filtro);

            return File(
                archivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                nombreArchivo);
        }

        [HttpGet]
        public async Task<IActionResult> EntregasExcel(
            FiltroReporteDTO filtro)
        {
            ReporteGeneralDTO reporte =
                await _serviceReporte.GenerarAsync(
                    filtro);

            byte[] archivo =
                _reporteExcelService
                    .GenerarEntregas(reporte);

            string nombreArchivo =
                $"entregas-{filtro.FechaInicio:yyyyMMdd}-{filtro.FechaFin:yyyyMMdd}.xlsx";

            await RegistrarDescargaAsync(
                "DescargaReporteEntregasExcel",
                filtro);

            return File(
                archivo,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                nombreArchivo);
        }

        private async Task RegistrarDescargaAsync(
            string accion,
            FiltroReporteDTO filtro)
        {
            try
            {
                int? idUsuario = ObtenerIdUsuarioActual();

                await _serviceBitacora.RegistrarAsync(
                    idUsuario,
                    accion,
                    "Reporte",
                    null,
                    $"Periodo consultado: {filtro.FechaInicio:dd/MM/yyyy} al {filtro.FechaFin:dd/MM/yyyy}. Tipo de comida: {ObtenerTipoComida(filtro)}. Estado: {ObtenerEstado(filtro)}.",
                    HttpContext.Connection
                        .RemoteIpAddress?
                        .ToString());
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "No fue posible registrar la descarga del reporte {Accion}.",
                    accion);
            }
        }

        private int? ObtenerIdUsuarioActual()
        {
            string? valor =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (int.TryParse(
                valor,
                out int idUsuario))
            {
                return idUsuario;
            }

            return null;
        }

        private static string ObtenerTipoComida(
            FiltroReporteDTO filtro)
        {
            return string.IsNullOrWhiteSpace(
                filtro.TipoComida)
                ? "Todos"
                : filtro.TipoComida;
        }

        private static string ObtenerEstado(
            FiltroReporteDTO filtro)
        {
            return filtro.Estado switch
            {
                0 => "Activa",
                1 => "Cancelada",
                _ => "Todos"
            };
        }
    }
}