using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComedorEstudiantil.Application.DTOs;

namespace ComedorEstudiantil.Application.Services.Interfaces
{
    public interface IServiceMenu
    {
        Task<List<MenuListaDTO>> ListarAsync();
        Task<MenuPublicoDTO> ListarPublicadosAsync(DateOnly fecha);
        Task<MenuFormularioDTO> PrepararNuevoAsync();
        Task<MenuFormularioDTO?> ObtenerParaEditarAsync(int idMenu);
        Task<ResultadoOperacionDTO> CrearAsync(
            MenuFormularioDTO formulario,
            int idUsuarioCreador);
        Task<ResultadoOperacionDTO> EditarAsync(
            MenuFormularioDTO formulario);
        Task<ResultadoOperacionDTO> CambiarPublicacionAsync(int idMenu);
    }
}
