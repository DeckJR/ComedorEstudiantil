using System.Globalization;
using System.Security.Claims;
using ComedorEstudiantil.Infraestructure.Models;
using ComedorEstudiantil.Infraestructure.Repository.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ComedorEstudiantil.Web.Authentication
{
    public class ValidacionCookieEvents
        : CookieAuthenticationEvents
    {
        private readonly IRepositoryUsuario
            _repositoryUsuario;

        private readonly ILogger<ValidacionCookieEvents>
            _logger;

        public ValidacionCookieEvents(
            IRepositoryUsuario repositoryUsuario,
            ILogger<ValidacionCookieEvents> logger)
        {
            _repositoryUsuario = repositoryUsuario;
            _logger = logger;
        }

        public override async Task ValidatePrincipal(
            CookieValidatePrincipalContext context)
        {
            ClaimsPrincipal? principal =
                context.Principal;

            string? valorIdUsuario =
                principal?.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (!int.TryParse(
                valorIdUsuario,
                out int idUsuario))
            {
                await RechazarSesionAsync(
                    context,
                    null,
                    "La cookie no contiene un identificador válido.");

                return;
            }

            Usuario? usuario =
                await _repositoryUsuario
                    .BuscarPorIdAsync(idUsuario);

            if (usuario is null)
            {
                await RechazarSesionAsync(
                    context,
                    idUsuario,
                    "El usuario ya no existe.");

                return;
            }

            if (usuario.Activo != true)
            {
                await RechazarSesionAsync(
                    context,
                    idUsuario,
                    "El usuario fue desactivado.");

                return;
            }

            string rolCookie =
                principal?.FindFirstValue(
                    ClaimTypes.Role)
                ?? string.Empty;

            string rolActual =
                usuario.IdRolNavigation?.Nombre
                ?? string.Empty;

            if (!string.Equals(
                rolCookie,
                rolActual,
                StringComparison.OrdinalIgnoreCase))
            {
                await RechazarSesionAsync(
                    context,
                    idUsuario,
                    "El rol del usuario fue modificado.");

                return;
            }

            string valorCambioObligatorio =
                principal?.FindFirstValue(
                    "DebeCambiarContrasena")
                ?? string.Empty;

            bool.TryParse(
                valorCambioObligatorio,
                out bool cambioObligatorioCookie);

            if (cambioObligatorioCookie !=
                usuario.DebeCambiarContrasena)
            {
                await RechazarSesionAsync(
                    context,
                    idUsuario,
                    "El estado de la contraseña fue modificado.");

                return;
            }

            string fechaCookie =
                principal?.FindFirstValue(
                    "FechaUltimoCambioContrasena")
                ?? string.Empty;

            string fechaActual =
                usuario.FechaUltimoCambioContrasena?
                    .Ticks
                    .ToString(
                        CultureInfo.InvariantCulture)
                ?? string.Empty;

            if (!string.Equals(
                fechaCookie,
                fechaActual,
                StringComparison.Ordinal))
            {
                await RechazarSesionAsync(
                    context,
                    idUsuario,
                    "La contraseña del usuario fue modificada.");
            }
        }

        private async Task RechazarSesionAsync(
            CookieValidatePrincipalContext context,
            int? idUsuario,
            string motivo)
        {
            _logger.LogWarning(
                "Se invalidó la sesión del usuario {IdUsuario}. Motivo: {Motivo}",
                idUsuario,
                motivo);

            context.RejectPrincipal();

            await context.HttpContext.SignOutAsync(
                CookieAuthenticationDefaults
                    .AuthenticationScheme);
        }
    }
}