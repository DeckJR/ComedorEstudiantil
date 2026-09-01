using System.Security.Claims;

namespace ComedorEstudiantil.Web.Middleware
{
    public class CambioContrasenaMiddleware
    {
        private readonly RequestDelegate _next;

        public CambioContrasenaMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (DebeRedirigir(context))
            {
                context.Response.Redirect(
                    "/Cuenta/CambiarContrasena");

                return;
            }

            await _next(context);
        }

        private static bool DebeRedirigir(
            HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            string? valor = context.User.FindFirstValue(
                "DebeCambiarContrasena");

            bool debeCambiar = string.Equals(
                valor,
                bool.TrueString,
                StringComparison.OrdinalIgnoreCase);

            if (!debeCambiar)
            {
                return false;
            }

            PathString ruta = context.Request.Path;

            if (ruta.StartsWithSegments(
                    "/Cuenta/CambiarContrasena") ||
                ruta.StartsWithSegments(
                    "/Cuenta/CerrarSesion") ||
                ruta.StartsWithSegments(
                    "/Home/ErrorHandler") ||
                ruta.StartsWithSegments("/css") ||
                ruta.StartsWithSegments("/js") ||
                ruta.StartsWithSegments("/lib") ||
                ruta.StartsWithSegments("/favicon.ico"))
            {
                return false;
            }

            return true;
        }
    }
}