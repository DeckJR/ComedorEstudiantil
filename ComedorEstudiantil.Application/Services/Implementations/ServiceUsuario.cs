using ComedorEstudiantil.Application.DTOs;
using ComedorEstudiantil.Application.Services.Interfaces;
using ComedorEstudiantil.Infraestructure.Models;
using ComedorEstudiantil.Infraestructure.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ComedorEstudiantil.Application.Services.Implementations
{
    public class ServiceUsuario : IServiceUsuario
    {
        private const string RolAdministrador = "Administrador";
        private const string RolEstudiante = "Estudiante";

        private readonly IRepositoryUsuario _repositoryUsuario;
        private readonly IRepositoryRol _repositoryRol;
        private readonly IRepositoryGradoSeccion _repositoryGradoSeccion;
        private readonly IRepositoryTipoBeneficiario _repositoryTipoBeneficiario;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public ServiceUsuario(
            IRepositoryUsuario repositoryUsuario,
            IRepositoryRol repositoryRol,
            IRepositoryGradoSeccion repositoryGradoSeccion,
            IRepositoryTipoBeneficiario repositoryTipoBeneficiario,
            IPasswordHasher<Usuario> passwordHasher)
        {
            _repositoryUsuario = repositoryUsuario;
            _repositoryRol = repositoryRol;
            _repositoryGradoSeccion = repositoryGradoSeccion;
            _repositoryTipoBeneficiario = repositoryTipoBeneficiario;
            _passwordHasher = passwordHasher;
        }

        public async Task<List<UsuarioListaDTO>> ListarAsync()
        {
            List<Usuario> usuarios = await _repositoryUsuario.ListarAsync();

            return usuarios.Select(usuario => new UsuarioListaDTO
            {
                IdUsuario = usuario.IdUsuario,
                Identificacion = usuario.Identificacion,
                NombreCompleto = $"{usuario.Nombre} {usuario.Apellidos}",
                Correo = usuario.Correo,
                Rol = usuario.IdRolNavigation.Nombre,
                Activo = usuario.Activo == true,
                TipoBeneficiario = usuario.Estudiante?
                    .IdTipoBeneficiarioNavigation.Nombre,
                GradoSeccion = usuario.Estudiante?
                    .IdGradoSeccionNavigation is null
                        ? null
                        : $"{usuario.Estudiante.IdGradoSeccionNavigation.Grado}-{usuario.Estudiante.IdGradoSeccionNavigation.Seccion}"
            }).ToList();
        }

        public async Task<UsuarioFormularioDTO> PrepararNuevoAsync(
            bool puedeAsignarAdministrador)
        {
            var formulario = new UsuarioFormularioDTO
            {
                Activo = true,
                AnioIngreso = (short)DateTime.Now.Year
            };

            await CargarCatalogosAsync(
                formulario,
                puedeAsignarAdministrador);

            return formulario;
        }

        public async Task<UsuarioFormularioDTO?> ObtenerParaEditarAsync(
            int idUsuario,
            bool puedeAsignarAdministrador)
        {
            Usuario? usuario = await _repositoryUsuario
                .BuscarPorIdAsync(idUsuario);

            if (usuario is null)
            {
                return null;
            }

            if (usuario.IdRolNavigation.Nombre == RolAdministrador &&
                !puedeAsignarAdministrador)
            {
                return null;
            }

            var formulario = new UsuarioFormularioDTO
            {
                IdUsuario = usuario.IdUsuario,
                Nombre = usuario.Nombre,
                Apellidos = usuario.Apellidos,
                Identificacion = usuario.Identificacion,
                Correo = usuario.Correo,
                IdRol = usuario.IdRol,
                Activo = usuario.Activo == true,
                IdTipoBeneficiario = usuario.Estudiante?
                    .IdTipoBeneficiario,
                IdGradoSeccion = usuario.Estudiante?
                    .IdGradoSeccion,
                CodigoAcceso = usuario.Estudiante?
                    .CodigoAcceso,
                AnioIngreso = usuario.Estudiante?
                    .AnioIngreso
            };

            await CargarCatalogosAsync(
                formulario,
                puedeAsignarAdministrador);

            return formulario;
        }

        public async Task<ResultadoOperacionDTO> CrearAsync(
            UsuarioFormularioDTO formulario,
            bool puedeAsignarAdministrador)
        {
            Rol? rol = await _repositoryRol.BuscarPorIdAsync(
                formulario.IdRol);

            ResultadoOperacionDTO? validacion = await ValidarFormularioAsync(
                formulario,
                rol,
                null,
                puedeAsignarAdministrador,
                true);

            if (validacion is not null)
            {
                return validacion;
            }

            var usuario = new Usuario
            {
                Nombre = formulario.Nombre.Trim(),
                Apellidos = formulario.Apellidos.Trim(),
                Identificacion = formulario.Identificacion.Trim(),
                Correo = formulario.Correo.Trim().ToLowerInvariant(),
                IdRol = formulario.IdRol,
                Activo = formulario.Activo,
                FechaCreacion = DateTime.Now
            };

            usuario.ContrasenaHash = _passwordHasher.HashPassword(
                usuario,
                formulario.Contrasena!);

            if (rol!.Nombre == RolEstudiante)
            {
                usuario.Estudiante = CrearEstudiante(formulario);
            }

            await _repositoryUsuario.AgregarAsync(usuario);

            return ResultadoOperacionDTO.Correcto(
                "El usuario fue creado correctamente.");
        }

        public async Task<ResultadoOperacionDTO> EditarAsync(
            UsuarioFormularioDTO formulario,
            bool puedeAsignarAdministrador)
        {
            Usuario? usuario = await _repositoryUsuario
                .BuscarPorIdParaEdicionAsync(formulario.IdUsuario);

            if (usuario is null)
            {
                return ResultadoOperacionDTO.Error(
                    "El usuario solicitado no existe.");
            }

            if (usuario.IdRolNavigation.Nombre == RolAdministrador &&
                !puedeAsignarAdministrador)
            {
                return ResultadoOperacionDTO.Error(
                    "No tiene permisos para modificar administradores.");
            }

            Rol? rol = await _repositoryRol.BuscarPorIdAsync(
                formulario.IdRol);

            ResultadoOperacionDTO? validacion = await ValidarFormularioAsync(
                formulario,
                rol,
                usuario.IdUsuario,
                puedeAsignarAdministrador,
                false);

            if (validacion is not null)
            {
                return validacion;
            }

            usuario.Nombre = formulario.Nombre.Trim();
            usuario.Apellidos = formulario.Apellidos.Trim();
            usuario.Identificacion = formulario.Identificacion.Trim();
            usuario.Correo = formulario.Correo.Trim().ToLowerInvariant();
            usuario.IdRol = formulario.IdRol;
            usuario.Activo = formulario.Activo;

            if (!string.IsNullOrWhiteSpace(formulario.Contrasena))
            {
                usuario.ContrasenaHash = _passwordHasher.HashPassword(
                    usuario,
                    formulario.Contrasena);
            }

            if (rol!.Nombre == RolEstudiante)
            {
                if (usuario.Estudiante is null)
                {
                    usuario.Estudiante = CrearEstudiante(formulario);
                }
                else
                {
                    ActualizarEstudiante(
                        usuario.Estudiante,
                        formulario);
                }
            }
            else if (usuario.Estudiante is not null)
            {
                _repositoryUsuario.EliminarEstudiante(
                    usuario.Estudiante);

                usuario.Estudiante = null;
            }

            await _repositoryUsuario.GuardarCambiosAsync();

            return ResultadoOperacionDTO.Correcto(
                "El usuario fue actualizado correctamente.");
        }

        public async Task<ResultadoOperacionDTO> CambiarEstadoAsync(
            int idUsuario,
            int idUsuarioActual,
            bool esAdministradorActual)
        {
            if (idUsuario == idUsuarioActual)
            {
                return ResultadoOperacionDTO.Error(
                    "No puede desactivar su propio usuario.");
            }

            Usuario? usuario = await _repositoryUsuario
                .BuscarPorIdParaEdicionAsync(idUsuario);

            if (usuario is null)
            {
                return ResultadoOperacionDTO.Error(
                    "El usuario solicitado no existe.");
            }

            if (usuario.IdRolNavigation.Nombre == RolAdministrador &&
                !esAdministradorActual)
            {
                return ResultadoOperacionDTO.Error(
                    "No tiene permisos para modificar administradores.");
            }

            bool nuevoEstado = usuario.Activo != true;
            usuario.Activo = nuevoEstado;

            if (usuario.Estudiante is not null)
            {
                usuario.Estudiante.Activo = nuevoEstado;
            }

            await _repositoryUsuario.GuardarCambiosAsync();

            string mensaje = nuevoEstado
                ? "El usuario fue activado correctamente."
                : "El usuario fue desactivado correctamente.";

            return ResultadoOperacionDTO.Correcto(mensaje);
        }

        public async Task<RestablecerContrasenaDTO?> PrepararRestablecimientoAsync(
            int idUsuario,
            bool esAdministradorActual)
        {
            Usuario? usuario = await _repositoryUsuario
                .BuscarPorIdAsync(idUsuario);

            if (usuario is null)
            {
                return null;
            }

            if (usuario.IdRolNavigation.Nombre == RolAdministrador &&
                !esAdministradorActual)
            {
                return null;
            }

            return new RestablecerContrasenaDTO
            {
                IdUsuario = usuario.IdUsuario,
                NombreCompleto =
                    $"{usuario.Nombre} {usuario.Apellidos}"
            };
        }

        public async Task<ResultadoOperacionDTO> RestablecerContrasenaAsync(
            RestablecerContrasenaDTO formulario,
            bool esAdministradorActual)
        {
            Usuario? usuario = await _repositoryUsuario
                .BuscarPorIdAsync(formulario.IdUsuario);

            if (usuario is null)
            {
                return ResultadoOperacionDTO.Error(
                    "El usuario solicitado no existe.");
            }

            if (usuario.IdRolNavigation.Nombre == RolAdministrador &&
                !esAdministradorActual)
            {
                return ResultadoOperacionDTO.Error(
                    "No tiene permisos para modificar administradores.");
            }

            string contrasenaHash = _passwordHasher.HashPassword(
                usuario,
                formulario.NuevaContrasena);

            await _repositoryUsuario.EstablecerContrasenaAsync(
    usuario.IdUsuario,
    contrasenaHash,
    true,
    DateTime.Now);

            return ResultadoOperacionDTO.Correcto(
                "La contraseña fue restablecida correctamente.");
        }

        private async Task CargarCatalogosAsync(
            UsuarioFormularioDTO formulario,
            bool puedeAsignarAdministrador)
        {
            List<Rol> roles = await _repositoryRol.ListarAsync();
            Rol? rolEstudiante = roles.FirstOrDefault(
                rol => rol.Nombre == RolEstudiante);

            formulario.IdRolEstudiante =
                rolEstudiante?.IdRol ?? 0;

            formulario.Roles = roles
                .Where(rol =>
                    puedeAsignarAdministrador ||
                    rol.Nombre != RolAdministrador)
                .Select(rol => new CatalogoDTO
                {
                    Id = rol.IdRol,
                    Nombre = rol.Nombre
                })
                .ToList();

            formulario.TiposBeneficiario =
                (await _repositoryTipoBeneficiario.ListarAsync())
                .Select(tipo => new CatalogoDTO
                {
                    Id = tipo.IdTipoBeneficiario,
                    Nombre = tipo.Nombre
                })
                .ToList();

            formulario.GradosSecciones =
                (await _repositoryGradoSeccion.ListarAsync())
                .Select(grado => new CatalogoDTO
                {
                    Id = grado.IdGradoSeccion,
                    Nombre = $"{grado.Grado}-{grado.Seccion}"
                })
                .ToList();
        }

        private async Task<ResultadoOperacionDTO?> ValidarFormularioAsync(
            UsuarioFormularioDTO formulario,
            Rol? rol,
            int? idUsuarioExcluir,
            bool puedeAsignarAdministrador,
            bool esCreacion)
        {
            if (rol is null)
            {
                return ResultadoOperacionDTO.Error(
                    "El rol seleccionado no existe.");
            }

            if (rol.Nombre == RolAdministrador &&
                !puedeAsignarAdministrador)
            {
                return ResultadoOperacionDTO.Error(
                    "No tiene permisos para asignar el rol Administrador.");
            }

            if (esCreacion &&
                string.IsNullOrWhiteSpace(formulario.Contrasena))
            {
                return ResultadoOperacionDTO.Error(
                    "La contraseña es obligatoria para crear el usuario.");
            }

            if (await _repositoryUsuario.ExisteIdentificacionAsync(
                formulario.Identificacion.Trim(),
                idUsuarioExcluir))
            {
                return ResultadoOperacionDTO.Error(
                    "Ya existe un usuario con esa identificación.");
            }

            if (await _repositoryUsuario.ExisteCorreoAsync(
                formulario.Correo.Trim().ToLowerInvariant(),
                idUsuarioExcluir))
            {
                return ResultadoOperacionDTO.Error(
                    "Ya existe un usuario con ese correo.");
            }

            if (rol.Nombre == RolEstudiante)
            {
                if (!formulario.IdTipoBeneficiario.HasValue)
                {
                    return ResultadoOperacionDTO.Error(
                        "Debe seleccionar el tipo de beneficiario.");
                }

                if (!formulario.AnioIngreso.HasValue)
                {
                    return ResultadoOperacionDTO.Error(
                        "Debe indicar el año de ingreso.");
                }

                if (!await _repositoryTipoBeneficiario.ExisteAsync(
                    formulario.IdTipoBeneficiario.Value))
                {
                    return ResultadoOperacionDTO.Error(
                        "El tipo de beneficiario seleccionado no existe.");
                }

                if (formulario.IdGradoSeccion.HasValue &&
                    !await _repositoryGradoSeccion.ExisteAsync(
                        formulario.IdGradoSeccion.Value))
                {
                    return ResultadoOperacionDTO.Error(
                        "El grado y sección seleccionados no existen.");
                }
            }

            return null;
        }

        private static Estudiante CrearEstudiante(
            UsuarioFormularioDTO formulario)
        {
            return new Estudiante
            {
                IdTipoBeneficiario =
                    formulario.IdTipoBeneficiario!.Value,
                IdGradoSeccion = formulario.IdGradoSeccion,
                CodigoAcceso = LimpiarTextoOpcional(
                    formulario.CodigoAcceso),
                AnioIngreso = formulario.AnioIngreso!.Value,
                Activo = formulario.Activo
            };
        }

        private static void ActualizarEstudiante(
            Estudiante estudiante,
            UsuarioFormularioDTO formulario)
        {
            estudiante.IdTipoBeneficiario =
                formulario.IdTipoBeneficiario!.Value;
            estudiante.IdGradoSeccion =
                formulario.IdGradoSeccion;
            estudiante.CodigoAcceso = LimpiarTextoOpcional(
                formulario.CodigoAcceso);
            estudiante.AnioIngreso =
                formulario.AnioIngreso!.Value;
            estudiante.Activo = formulario.Activo;
        }

        private static string? LimpiarTextoOpcional(string? texto)
        {
            return string.IsNullOrWhiteSpace(texto)
                ? null
                : texto.Trim();
        }
    }
}