using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public ServiceAutenticacion(IRepositoryUsuario repositoryUsuario, IPasswordHasher<Usuario> passwordHasher)
        {
            _repositoryUsuario = repositoryUsuario;
            _passwordHasher = passwordHasher;
        }

        public async Task<UsuarioSesionDTO?> AutenticarAsync(LoginDTO login)
        {
            string identificacion = login.Identificacion.Trim();

            Usuario? usuario = await _repositoryUsuario.BuscarPorIdentificacionAsync(identificacion);

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
                usuario.ContrasenaHash = _passwordHasher.HashPassword(usuario, login.Contrasena);

                await _repositoryUsuario.ActualizarAsync(usuario);
            }

            return new UsuarioSesionDTO
            {
                IdUsuario = usuario.IdUsuario,
                Identificacion = usuario.Identificacion,
                NombreCompleto = $"{usuario.Nombre} {usuario.Apellidos}",
                Correo = usuario.Correo,
                Rol = usuario.IdRolNavigation.Nombre
            };
        }
    }
}
