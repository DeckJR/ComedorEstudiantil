using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComedorEstudiantil.Application.DTOs;

namespace ComedorEstudiantil.Application.Services.Interfaces
{
    public interface IServiceUsuario
    {
        Task<List<UsuarioListaDTO>> ListarAsync();

        Task<CodigoBarrasUsuarioDTO?> ObtenerCodigoBarrasAsync(
            int idUsuario);

        Task<UsuarioFormularioDTO> PrepararNuevoAsync(
            bool puedeAsignarAdministrador);

        Task<UsuarioFormularioDTO?> ObtenerParaEditarAsync(
            int idUsuario,
            bool puedeAsignarAdministrador);

        Task<ResultadoOperacionDTO> CrearAsync(
            UsuarioFormularioDTO formulario,
            bool puedeAsignarAdministrador);

        Task<ResultadoOperacionDTO> EditarAsync(
            UsuarioFormularioDTO formulario,
            bool puedeAsignarAdministrador);

        Task<ResultadoOperacionDTO> CambiarEstadoAsync(
            int idUsuario,
            int idUsuarioActual,
            bool esAdministradorActual);

        Task<RestablecerContrasenaDTO?> PrepararRestablecimientoAsync(
            int idUsuario,
            bool esAdministradorActual);

        Task<ResultadoOperacionDTO> RestablecerContrasenaAsync(
            RestablecerContrasenaDTO formulario,
            bool esAdministradorActual);
    }
}