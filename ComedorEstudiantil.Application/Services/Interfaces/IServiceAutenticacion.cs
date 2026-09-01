using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComedorEstudiantil.Application.DTOs;

namespace ComedorEstudiantil.Application.Services.Interfaces
{
    public interface IServiceAutenticacion
    {
        Task<UsuarioSesionDTO?> AutenticarAsync(LoginDTO login);

        Task<ResultadoOperacionDTO> CambiarContrasenaAsync(int idUsuario,CambiarContrasenaDTO formulario);
    }
}
