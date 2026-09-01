using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComedorEstudiantil.Application.DTOs;

namespace ComedorEstudiantil.Application.Services.Interfaces
{
    public interface IServiceSolicitud
    {
        Task AplicarEstadoSolicitudesAsync(List<MenuListaDTO> menus,int? idUsuario);
        Task<List<SolicitudListaDTO>> ListarPropiasAsync(int idUsuario);
        Task<ResultadoOperacionDTO> SolicitarAsync(int idMenu,int idUsuario,int? idUsuarioMarco,bool esSolicitudManual);
        Task<ResultadoOperacionDTO> CancelarAsync(int idSolicitud,int idUsuario);
        Task<SolicitudAjenaDTO?> PrepararSolicitudAjenaAsync(int idMenu);
        Task<ResultadoOperacionDTO> SolicitarParaOtraPersonaAsync(SolicitudAjenaDTO formulario,int idUsuarioMarco);
    }
}