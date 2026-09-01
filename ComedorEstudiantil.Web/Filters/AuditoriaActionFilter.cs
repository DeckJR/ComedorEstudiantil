using System.Security.Claims;
using ComedorEstudiantil.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ComedorEstudiantil.Web.Filters
{
    public class AuditoriaActionFilter : IAsyncActionFilter
    {
        private readonly IServiceBitacora _serviceBitacora;
        private readonly ILogger<AuditoriaActionFilter> _logger;

        public AuditoriaActionFilter(
            IServiceBitacora serviceBitacora,
            ILogger<AuditoriaActionFilter> logger)
        {
            _serviceBitacora = serviceBitacora;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            ActionExecutedContext resultado =
                await next();

            if (!DebeAuditar(context))
            {
                return;
            }

            try
            {
                int? idUsuario =
                    ObtenerIdUsuario(
                        context.HttpContext.User);

                string controlador =
                    context.RouteData.Values["controller"]?
                        .ToString()
                    ?? "Desconocido";

                string accion =
                    context.RouteData.Values["action"]?
                        .ToString()
                    ?? "Desconocida";

                int? idEntidad =
                    ObtenerIdEntidad(
                        context.ActionArguments);

                bool operacionExitosa =
                    EsOperacionExitosa(resultado);

                string detalle = operacionExitosa
                    ? $"Operación HTTP {context.HttpContext.Request.Method} completada correctamente."
                    : $"Operación HTTP {context.HttpContext.Request.Method} rechazada o no completada.";

                await _serviceBitacora.RegistrarAsync(
                    idUsuario,
                    operacionExitosa
                        ? accion
                        : $"IntentoFallido{accion}",
                    controlador,
                    idEntidad,
                    detalle,
                    context.HttpContext.Connection
                        .RemoteIpAddress?
                        .ToString());
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "No fue posible registrar la operación en la bitácora.");
            }
        }

        private static bool DebeAuditar(
            ActionExecutingContext context)
        {
            string metodo =
                context.HttpContext.Request.Method;

            bool modificaInformacion =
                HttpMethods.IsPost(metodo) ||
                HttpMethods.IsPut(metodo) ||
                HttpMethods.IsPatch(metodo) ||
                HttpMethods.IsDelete(metodo);

            if (!modificaInformacion)
            {
                return false;
            }

            string controlador =
                context.RouteData.Values["controller"]?
                    .ToString()
                ?? string.Empty;

            return !controlador.Equals(
                       "Cuenta",
                       StringComparison.OrdinalIgnoreCase) &&
                   !controlador.Equals(
                       "Bitacora",
                       StringComparison.OrdinalIgnoreCase);
        }

        private static bool EsOperacionExitosa(
            ActionExecutedContext context)
        {
            if (context.Exception is not null &&
                !context.ExceptionHandled)
            {
                return false;
            }

            if (context.Controller is Controller controller)
            {
                if (!controller.ModelState.IsValid)
                {
                    return false;
                }

                if (controller.TempData.ContainsKey(
                    "MensajeError"))
                {
                    return false;
                }
            }

            if (context.Result is StatusCodeResult statusCode)
            {
                return statusCode.StatusCode < 400;
            }

            if (context.Result is ObjectResult objectResult &&
                objectResult.StatusCode.HasValue)
            {
                return objectResult.StatusCode.Value < 400;
            }

            return true;
        }

        private static int? ObtenerIdUsuario(
            ClaimsPrincipal usuario)
        {
            string? valor =
                usuario.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            return int.TryParse(
                valor,
                out int idUsuario)
                ? idUsuario
                : null;
        }

        private static int? ObtenerIdEntidad(
            IDictionary<string, object?> argumentos)
        {
            string[] nombres =
            {
                "id",
                "idUsuario",
                "idEstudiante",
                "idMenu",
                "idActividad",
                "idSolicitud",
                "idEntrega"
            };

            foreach (string nombre in nombres)
            {
                KeyValuePair<string, object?> argumento =
                    argumentos.FirstOrDefault(item =>
                        item.Key.Equals(
                            nombre,
                            StringComparison.OrdinalIgnoreCase));

                if (argumento.Value is int id)
                {
                    return id;
                }

                if (argumento.Value is not null &&
                    int.TryParse(
                        argumento.Value.ToString(),
                        out int idConvertido))
                {
                    return idConvertido;
                }
            }

            return null;
        }
    }
}