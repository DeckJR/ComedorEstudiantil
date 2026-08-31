using System.Diagnostics;

namespace ComedorEstudiantil.Web.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await ProcesarErrorAsync(context, ex);
            }
        }

        private async Task ProcesarErrorAsync(
            HttpContext context,
            Exception exception)
        {
            string idEvento =
                Activity.Current?.Id ??
                context.TraceIdentifier ??
                Guid.NewGuid().ToString();

            string? ipOrigen =
                context.Connection.RemoteIpAddress?.ToString();

            _logger.LogError(
                exception,
                """
                Error no controlado.
                IdEvento: {IdEvento}
                Método: {Metodo}
                Ruta: {Ruta}
                QueryString: {QueryString}
                IpOrigen: {IpOrigen}
                """,
                idEvento,
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString.Value,
                ipOrigen);

            if (context.Response.HasStarted)
            {
                _logger.LogWarning(
                    "No fue posible mostrar la página de error porque la respuesta ya había comenzado. IdEvento: {IdEvento}",
                    idEvento);

                throw exception;
            }

            context.Response.Clear();

            string idEventoCodificado =
                Uri.EscapeDataString(idEvento);

            context.Response.Redirect(
                $"/Home/ErrorHandler?idEvento={idEventoCodificado}");

            await Task.CompletedTask;
        }
    }
}