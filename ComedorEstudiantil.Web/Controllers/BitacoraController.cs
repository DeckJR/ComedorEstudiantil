using ComedorEstudiantil.Application.DTOs;
using ComedorEstudiantil.Application.Services.Interfaces;
using ComedorEstudiantil.Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComedorEstudiantil.Web.Controllers
{
    [Authorize(
        Policy = PoliticasAutorizacion.ConsultarBitacora)]
    public class BitacoraController : Controller
    {
        private readonly IServiceBitacora _serviceBitacora;
        private readonly IFechaHoraService _fechaHoraService;

        public BitacoraController(
            IServiceBitacora serviceBitacora,
            IFechaHoraService fechaHoraService)
        {
            _serviceBitacora = serviceBitacora;
            _fechaHoraService = fechaHoraService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            BitacoraFiltroDTO? filtro)
        {
            filtro ??= new BitacoraFiltroDTO();

            if (filtro.FechaInicio == default ||
                filtro.FechaFin == default)
            {
                DateOnly fechaActual =
                    _fechaHoraService.ObtenerFechaActual();

                filtro.FechaInicio =
                    fechaActual.AddDays(-30);

                filtro.FechaFin =
                    fechaActual;
            }

            BitacoraConsultaDTO consulta =
                await _serviceBitacora.ConsultarAsync(
                    filtro);

            return View(consulta);
        }
    }
}