using ComedorEstudiantil.Application.Services.Implementations;
using ComedorEstudiantil.Application.Services.Interfaces;
using ComedorEstudiantil.Infraestructure.Data;
using ComedorEstudiantil.Infraestructure.Models;
using ComedorEstudiantil.Infraestructure.Repository.Implementations;
using ComedorEstudiantil.Infraestructure.Repository.Interfaces;
using ComedorEstudiantil.Web.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using ComedorEstudiantil.Web.Authorization;
using ComedorEstudiantil.Web.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("MariaDbConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "No se encontró la cadena de conexión MariaDbConnection.");
}

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
                "{Level:u3}] {Message:lj}" +
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
                        "{Level:u3}] {Message:lj}" +
                        "{NewLine}{Exception}");
        });
});

builder.Services.AddDbContext<ComedorEstudiantilContext>(options =>
{
    options.UseMySql(
        connectionString,
        new MariaDbServerVersion(
            new Version(10, 4, 32)));

    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
});

builder.Services.AddSingleton<IFechaHoraService,FechaHoraService>();

builder.Services.AddScoped<IRepositoryUsuario,RepositoryUsuario>();
builder.Services.AddScoped<IRepositoryRol, RepositoryRol>();
builder.Services.AddScoped<IRepositoryGradoSeccion, RepositoryGradoSeccion>();
builder.Services.AddScoped<IRepositoryTipoBeneficiario, RepositoryTipoBeneficiario>();
builder.Services.AddScoped<IRepositoryActividad, RepositoryActividad>();
builder.Services.AddScoped<IRepositoryTipoComida, RepositoryTipoComida>();
builder.Services.AddScoped<IRepositoryMenu, RepositoryMenu>();
builder.Services.AddScoped<IRepositorySolicitud, RepositorySolicitud>();
builder.Services.AddScoped<IRepositoryEntrega, RepositoryEntrega>();

builder.Services.AddScoped<IServiceEntrega, ServiceEntrega>();
builder.Services.AddScoped<IServiceSolicitud, ServiceSolicitud>();
builder.Services.AddScoped<IServiceMenu, ServiceMenu>();
builder.Services.AddScoped<IServiceActividad, ServiceActividad>();
builder.Services.AddScoped<IServiceUsuario, ServiceUsuario>();
builder.Services.AddScoped<IServiceAutenticacion,ServiceAutenticacion>();
builder.Services.AddScoped<IPasswordHasher<Usuario>,PasswordHasher<Usuario>>();

builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/IniciarSesion";
        options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
        options.Cookie.Name = "ComedorEstudiantil.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy =
            CookieSecurePolicy.Always;
        options.Cookie.SameSite =
            SameSiteMode.Lax;
        options.ExpireTimeSpan =
            TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        PoliticasAutorizacion.UsuarioAutenticado,
        policy =>
        {
            policy.RequireAuthenticatedUser();
        });

    options.AddPolicy(
        PoliticasAutorizacion.RegistrarSolicitudAjena,
        policy =>
        {
            policy.RequireRole(
                "Cocina",
                "Direccion",
                "Director",
                "Administrador");
        });

    options.AddPolicy(
        PoliticasAutorizacion.RegistrarEntrega,
        policy =>
        {
            policy.RequireRole(
                "Cocina",
                "Direccion",
                "Director",
                "Administrador");
        });

    options.AddPolicy(
        PoliticasAutorizacion.GestionarMenus,
        policy =>
        {
            policy.RequireRole(
                "Cocina",
                "Direccion",
                "Director",
                "Administrador");
        });

    options.AddPolicy(
        PoliticasAutorizacion.GestionarActividades,
        policy =>
        {
            policy.RequireRole(
                "Cocina",
                "Direccion",
                "Director",
                "Administrador");
        });

    options.AddPolicy(
        PoliticasAutorizacion.GestionarUsuarios,
        policy =>
        {
            policy.RequireRole(
                "Direccion",
                "Director",
                "Administrador");
        });

    options.AddPolicy(
        PoliticasAutorizacion.GestionarEstudiantes,
        policy =>
        {
            policy.RequireRole(
                "Auxiliar",
                "Orientador",
                "Direccion",
                "Director",
                "Administrador");
        });

    options.AddPolicy(
        PoliticasAutorizacion.VerReportes,
        policy =>
        {
            policy.RequireRole(
                "Direccion",
                "Director",
                "Administrador");
        });

    options.AddPolicy(
        PoliticasAutorizacion.ConsultarBitacora,
        policy =>
        {
            policy.RequireRole(
                "Director",
                "Administrador");
        });

    options.AddPolicy(
        PoliticasAutorizacion.ConfiguracionTecnica,
        policy =>
        {
            policy.RequireRole("Administrador");
        });
});
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate =
        "Solicitud HTTP {RequestMethod} {RequestPath} " +
        "respondió {StatusCode} en {Elapsed:0.0000} ms";

    options.GetLevel = (
        httpContext,
        elapsed,
        exception) =>
    {
        if (exception is not null ||
            httpContext.Response.StatusCode >= 500)
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

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseRouting();

app.UseAuthentication();
app.UseMiddleware<CambioContrasenaMiddleware>();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern:
        "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();