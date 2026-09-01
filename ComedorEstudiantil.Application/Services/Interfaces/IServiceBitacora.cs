using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComedorEstudiantil.Application.DTOs;

namespace ComedorEstudiantil.Application.Services.Interfaces
{
    public interface IServiceBitacora
    {
        Task RegistrarAsync(
            int? idUsuario,
            string accion,
            string entidad,
            int? idEntidad,
            string? detalle,
            string? ipOrigen);

        Task<BitacoraConsultaDTO> ConsultarAsync(
            BitacoraFiltroDTO filtro);
    }
}