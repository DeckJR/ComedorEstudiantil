using ComedorEstudiantil.Application.DTOs;
using ComedorEstudiantil.Application.Services.Interfaces;
using ComedorEstudiantil.Infraestructure.Models;
using ComedorEstudiantil.Infraestructure.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ComedorEstudiantil.Application.Services.Implementations
{
    public class ServiceAutenticacion : IServiceAutenticacion
    {
        private readonly IRepositoryUsuario _repositoryUsuario;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public ServiceAutenticacion(
            IRepositoryUsuario repositoryUsuario,
            IPasswordHasher<Usuario> passwordHasher)
        {
            _repositoryUsuario = repositoryUsuario;
            _passwordHasher = passwordHasher;
        }

        public async Task<UsuarioSesionDTO?> AutenticarAsync(LoginDTO login)
        {
            string identificacion = login.Identificacion.Trim();

            Usuario? usuario = await _repositoryUsuario
                .BuscarPorIdentificacionAsync(identificacion);

            if (usuario is null)
            {
                return null;
            }

            PasswordVerificationResult resultado = _passwordHasher
                .VerifyHashedPassword(
                    usuario,
                    usuario.ContrasenaHash,
                    login.Contrasena);

            if (resultado == PasswordVerificationResult.Failed)
            {
                return null;
            }

            if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
            {
                string nuevoHash = _passwordHasher.HashPassword(
                    usuario,
                    login.Contrasena);

                await _repositoryUsuario.ActualizarHashContrasenaAsync(
                    usuario.IdUsuario,
                    nuevoHash);
            }

            return new UsuarioSesionDTO
            {
                IdUsuario = usuario.IdUsuario,
                Identificacion = usuario.Identificacion,
                NombreCompleto = $"{usuario.Nombre} {usuario.Apellidos}",
                Correo = usuario.Correo,
                Rol = usuario.IdRolNavigation.Nombre,
                DebeCambiarContrasena = usuario.DebeCambiarContrasena
            };
        }

        public async Task<ResultadoOperacionDTO> CambiarContrasenaAsync(
            int idUsuario,
            CambiarContrasenaDTO formulario)
        {
            Usuario? usuario = await _repositoryUsuario
                .BuscarPorIdAsync(idUsuario);

            if (usuario is null || usuario.Activo != true)
            {
                return ResultadoOperacionDTO.Error(
                    "El usuario solicitado no existe o se encuentra inactivo.");
            }

            PasswordVerificationResult resultado = _passwordHasher
                .VerifyHashedPassword(
                    usuario,
                    usuario.ContrasenaHash,
                    formulario.ContrasenaActual);

            if (resultado == PasswordVerificationResult.Failed)
            {
                return ResultadoOperacionDTO.Error(
                    "La contraseña actual es incorrecta.");
            }

            if (formulario.ContrasenaActual ==
                formulario.NuevaContrasena)
            {
                return ResultadoOperacionDTO.Error(
                    "La nueva contraseña debe ser diferente de la actual.");
            }

            string nuevoHash = _passwordHasher.HashPassword(
                usuario,
                formulario.NuevaContrasena);

            await _repositoryUsuario.EstablecerContrasenaAsync(
                usuario.IdUsuario,
                nuevoHash,
                false,
                DateTime.Now);

            return ResultadoOperacionDTO.Correcto(
                "La contraseña fue actualizada correctamente.");
        }
    }
}