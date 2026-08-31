using ComedorEstudiantil.Infraestructure.Data;
using ComedorEstudiantil.Web.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("MariaDbConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("No se encontró la cadena de conexión MariaDbConnection.");
}

// Configuración de Serilog
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .MinimumLevel.Information()
        .MinimumLevel.Override(
            "Microsoft",
            LogEventLevel.Warning)
        .MinimumLevel.Override(
            "Microsoft.EntityFrameworkCore.Database.Command",
            LogEventLevel.Information)
        .Enrich.FromLogContext()
        .Enrich.WithProperty(
            "Aplicacion",
            "ComedorEstudiantil")
        .WriteTo.Console()
        .WriteTo.File(
            path: "Logs/general-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 30,
            shared: true,
            outputTemplate:
                "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} " +
                "{Level:u3}] " +
                "{Message:lj}" +
                "{NewLine}{Exception}")
        .WriteTo.Logger(errorLogger =>
        {
            errorLogger
                .Filter.ByIncludingOnly(logEvent =>
                    logEvent.Level >= LogEventLevel.Error)
                .WriteTo.File(
                    path: "Logs/error-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 90,
                    shared: true,
                    outputTemplate:
                        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} " +
                        "{Level:u3}] " +
                        "{Message:lj}" +
                        "{NewLine}{Exception}");
        });
});

// Configuración de MariaDB
builder.Services.AddDbContext<ComedorEstudiantilContext>(options =>
{
    options.UseMySql(connectionString,new MariaDbServerVersion(new Version(10, 4, 32)));

    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
});

// Servicios MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

// Registra información resumida de cada solicitud HTTP.
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "Solicitud HTTP {RequestMethod} {RequestPath} " + "respondió {StatusCode} en {Elapsed:0.0000} ms";

    options.GetLevel = ( httpContext,elapsed,exception) =>
    {
        if (exception is not null || httpContext.Response.StatusCode >= 500)
        {
            return LogEventLevel.Error;
        }

        if (httpContext.Response.StatusCode >= 400)
        {
            return LogEventLevel.Warning;
        }

        return LogEventLevel.Information;
    };
});

// Captura excepciones de controladores, servicios y repositorios.
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(name: "default",pattern:"{controller=Home}/{action=Index}/{id?}").WithStaticAssets();

app.Run();