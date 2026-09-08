using System.Reflection;
using System.Security.Claims;
using ComedorEstudiantil.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ComedorEstudiantil.Web.Filters
{
    public class AuditoriaActionFilter : IAsyncActionFilter
    {
        private static readonly string[] NombresIdentificadores =
        {
            "id",
            "idUsuario",
            "idEstudiante",
            "idMenu",
            "idActividad",
            "idSolicitud",
            "idEntrega",
            "idRepeticionEntrega",
            "idTipoComida",
            "idGradoSeccion"
        };

        private readonly IServiceBitacora
            _serviceBitacora;

        private readonly ILogger<AuditoriaActionFilter>
            _logger;

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

                string accionBitacora =
                    operacionExitosa
                        ? accion
                        : $"IntentoFallido{accion}";

                string detalle =
                    CrearDetalle(
                        context.HttpContext.Request.Method,
                        controlador,
                        accion,
                        operacionExitosa);

                await _serviceBitacora.RegistrarAsync(
                    idUsuario,
                    accionBitacora,
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

            if (context.Result is ObjectResult objectResult)
            {
                if (objectResult.StatusCode.HasValue &&
                    objectResult.StatusCode.Value >= 400)
                {
                    return false;
                }

                bool? resultadoOperacion =
                    ObtenerResultadoOperacion(
                        objectResult.Value);

                if (resultadoOperacion.HasValue)
                {
                    return resultadoOperacion.Value;
                }
            }

            if (context.Result is JsonResult jsonResult)
            {
                bool? resultadoOperacion =
                    ObtenerResultadoOperacion(
                        jsonResult.Value);

                if (resultadoOperacion.HasValue)
                {
                    return resultadoOperacion.Value;
                }
            }

            return true;
        }

        private static bool? ObtenerResultadoOperacion(
            object? resultado)
        {
            if (resultado is null)
            {
                return null;
            }

            PropertyInfo? propiedad =
                resultado.GetType().GetProperty(
                    "Exitoso",
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.IgnoreCase);

            if (propiedad?.PropertyType != typeof(bool))
            {
                return null;
            }

            return propiedad.GetValue(resultado) as bool?;
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
            int? identificadorDirecto =
                BuscarIdentificadorDirecto(
                    argumentos);

            if (identificadorDirecto.HasValue)
            {
                return identificadorDirecto;
            }

            foreach (object? argumento
                in argumentos.Values)
            {
                int? identificador =
                    BuscarIdentificadorEnObjeto(
                        argumento);

                if (identificador.HasValue)
                {
                    return identificador;
                }
            }

            return null;
        }

        private static int? BuscarIdentificadorDirecto(
            IDictionary<string, object?> argumentos)
        {
            foreach (string nombre
                in NombresIdentificadores)
            {
                KeyValuePair<string, object?> argumento =
                    argumentos.FirstOrDefault(item =>
                        item.Key.Equals(
                            nombre,
                            StringComparison.OrdinalIgnoreCase));

                int? identificador =
                    ConvertirIdentificador(
                        argumento.Value);

                if (identificador.HasValue)
                {
                    return identificador;
                }
            }

            return null;
        }

        private static int? BuscarIdentificadorEnObjeto(
            object? argumento)
        {
            if (argumento is null ||
                argumento is string)
            {
                return null;
            }

            Type tipo = argumento.GetType();

            if (tipo.IsPrimitive ||
                tipo.IsEnum)
            {
                return null;
            }

            foreach (string nombre
                in NombresIdentificadores)
            {
                PropertyInfo? propiedad =
                    tipo.GetProperty(
                        nombre,
                        BindingFlags.Public |
                        BindingFlags.Instance |
                        BindingFlags.IgnoreCase);

                if (propiedad is null)
                {
                    continue;
                }

                object? valor =
                    propiedad.GetValue(argumento);

                int? identificador =
                    ConvertirIdentificador(valor);

                if (identificador.HasValue)
                {
                    return identificador;
                }
            }

            return null;
        }

        private static int? ConvertirIdentificador(
            object? valor)
        {
            if (valor is int id &&
                id > 0)
            {
                return id;
            }

            if (valor is not null &&
                int.TryParse(
                    valor.ToString(),
                    out int idConvertido) &&
                idConvertido > 0)
            {
                return idConvertido;
            }

            return null;
        }

        private static string CrearDetalle(
            string metodo,
            string controlador,
            string accion,
            bool operacionExitosa)
        {
            string resultado =
                operacionExitosa
                    ? "completada correctamente"
                    : "rechazada o no completada";

            return
                $"Operación {metodo} {controlador}/{accion} {resultado}.";
        }
    }
}