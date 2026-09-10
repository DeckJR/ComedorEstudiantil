using ComedorEstudiantil.Application.DTOs;
using ComedorEstudiantil.Application.Services.Interfaces;
using ComedorEstudiantil.Infraestructure.Models;
using ComedorEstudiantil.Infraestructure.Repository.Interfaces;

namespace ComedorEstudiantil.Application.Services.Implementations
{
    public class ServiceEstudiante
        : IServiceEstudiante
    {
        private const string RolEstudiante =
            "Estudiante";

        private readonly IRepositoryEstudiante
            _repositoryEstudiante;

        private readonly IRepositoryRol
            _repositoryRol;

        private readonly IRepositoryTipoBeneficiario
            _repositoryTipoBeneficiario;

        private readonly IRepositoryGradoSeccion
            _repositoryGradoSeccion;

        private readonly IServiceUsuario
            _serviceUsuario;

        public ServiceEstudiante(
            IRepositoryEstudiante repositoryEstudiante,
            IRepositoryRol repositoryRol,
            IRepositoryTipoBeneficiario
                repositoryTipoBeneficiario,
            IRepositoryGradoSeccion
                repositoryGradoSeccion,
            IServiceUsuario serviceUsuario)
        {
            _repositoryEstudiante =
                repositoryEstudiante;

            _repositoryRol =
                repositoryRol;

            _repositoryTipoBeneficiario =
                repositoryTipoBeneficiario;

            _repositoryGradoSeccion =
                repositoryGradoSeccion;

            _serviceUsuario =
                serviceUsuario;
        }

        public async Task<List<EstudianteListaDTO>>
    ListarAsync(
        bool incluirArchivados = false)
        {
            List<Estudiante> estudiantes =
                await _repositoryEstudiante.ListarAsync(
                    incluirArchivados);

            return estudiantes
                .Select(estudiante =>
                    new EstudianteListaDTO
                    {
                        IdUsuario =
                            estudiante.IdUsuario,

                        IdEstudiante =
                            estudiante.IdEstudiante,

                        Identificacion =
                            estudiante
                                .IdUsuarioNavigation
                                .Identificacion,

                        CodigoBarras =
                            estudiante
                                .IdUsuarioNavigation
                                .CodigoBarras,

                        NombreCompleto =
                            $"{estudiante.IdUsuarioNavigation.Nombre} {estudiante.IdUsuarioNavigation.Apellidos}",

                        Correo =
                            estudiante
                                .IdUsuarioNavigation
                                .Correo,

                        TipoBeneficiario =
                            estudiante
                                .IdTipoBeneficiarioNavigation
                                .Nombre,

                        GradoSeccion =
                            estudiante
                                .IdGradoSeccionNavigation
                                is null
                                ? "Sin asignar"
                                : $"{estudiante.IdGradoSeccionNavigation.Grado}-{estudiante.IdGradoSeccionNavigation.Seccion}",

                        AnioIngreso =
                            estudiante.AnioIngreso,

                        Activo =
                            estudiante.Activo == true &&
                            estudiante
                                .IdUsuarioNavigation
                                .Activo == true
                    })
                .ToList();
        }

        public async Task<EstudianteFormularioDTO>
            PrepararNuevoAsync()
        {
            var formulario =
                new EstudianteFormularioDTO
                {
                    Activo = true,
                    AnioIngreso =
                        (short)DateTime.Now.Year
                };

            await CargarCatalogosAsync(formulario);

            return formulario;
        }

        public async Task<EstudianteFormularioDTO?>
            ObtenerParaEditarAsync(
                int idEstudiante)
        {
            Estudiante? estudiante =
                await _repositoryEstudiante
                    .BuscarPorIdAsync(idEstudiante);

            if (estudiante is null)
            {
                return null;
            }

            if (!EsCuentaEstudiante(estudiante))
            {
                return null;
            }

            var formulario =
                new EstudianteFormularioDTO
                {
                    IdUsuario =
                        estudiante.IdUsuario,

                    IdEstudiante =
                        estudiante.IdEstudiante,

                    Nombre =
                        estudiante
                            .IdUsuarioNavigation
                            .Nombre,

                    Apellidos =
                        estudiante
                            .IdUsuarioNavigation
                            .Apellidos,

                    Identificacion =
                        estudiante
                            .IdUsuarioNavigation
                            .Identificacion,

                    Correo =
                        estudiante
                            .IdUsuarioNavigation
                            .Correo,

                    IdTipoBeneficiario =
                        estudiante.IdTipoBeneficiario,

                    IdGradoSeccion =
                        estudiante.IdGradoSeccion,

                    AnioIngreso =
                        estudiante.AnioIngreso,

                    Activo =
                        estudiante.Activo == true &&
                        estudiante
                            .IdUsuarioNavigation
                            .Activo == true
                };

            await CargarCatalogosAsync(formulario);

            return formulario;
        }

        public async Task<CodigoBarrasUsuarioDTO?>
    ObtenerCodigoBarrasAsync(
        int idEstudiante)
        {
            Estudiante? estudiante =
                await _repositoryEstudiante
                    .BuscarPorIdAsync(idEstudiante);

            if (estudiante is null ||
                !EsCuentaEstudiante(estudiante))
            {
                return null;
            }

            Usuario usuario =
                estudiante.IdUsuarioNavigation;

            return new CodigoBarrasUsuarioDTO
            {
                IdUsuario = usuario.IdUsuario,
                NombreCompleto =
                    $"{usuario.Nombre} {usuario.Apellidos}",
                Identificacion =
                    usuario.Identificacion,
                CodigoBarras =
                    usuario.CodigoBarras
            };
        }

        public async Task<ResultadoOperacionDTO>
            CrearAsync(
                EstudianteFormularioDTO formulario)
        {
            Rol? rolEstudiante =
                await _repositoryRol
                    .BuscarPorNombreAsync(
                        RolEstudiante);

            if (rolEstudiante is null)
            {
                return ResultadoOperacionDTO.Error(
                    "No se encontró el rol Estudiante.");
            }

            if (string.IsNullOrWhiteSpace(
                formulario.Contrasena))
            {
                return ResultadoOperacionDTO.Error(
                    "La contraseña es obligatoria para crear el estudiante.");
            }

            UsuarioFormularioDTO usuarioFormulario =
                ConvertirFormulario(
                    formulario,
                    rolEstudiante.IdRol);

            ResultadoOperacionDTO resultado =
                await _serviceUsuario.CrearAsync(
                    usuarioFormulario,
                    false);

            if (!resultado.Exitoso)
            {
                return resultado;
            }

            return ResultadoOperacionDTO.Correcto(
                "El estudiante fue creado correctamente.");
        }

        public async Task<ResultadoOperacionDTO>
            EditarAsync(
                EstudianteFormularioDTO formulario)
        {
            Estudiante? estudiante =
                await _repositoryEstudiante
                    .BuscarPorIdAsync(
                        formulario.IdEstudiante);

            if (estudiante is null ||
                estudiante.IdUsuario !=
                    formulario.IdUsuario)
            {
                return ResultadoOperacionDTO.Error(
                    "El estudiante solicitado no existe.");
            }

            if (!EsCuentaEstudiante(estudiante))
            {
                return ResultadoOperacionDTO.Error(
                    "La cuenta indicada no pertenece a un estudiante.");
            }

            Rol? rolEstudiante =
                await _repositoryRol
                    .BuscarPorNombreAsync(
                        RolEstudiante);

            if (rolEstudiante is null)
            {
                return ResultadoOperacionDTO.Error(
                    "No se encontró el rol Estudiante.");
            }

            UsuarioFormularioDTO usuarioFormulario =
                ConvertirFormulario(
                    formulario,
                    rolEstudiante.IdRol);

            ResultadoOperacionDTO resultado =
                await _serviceUsuario.EditarAsync(
                    usuarioFormulario,
                    false);

            if (!resultado.Exitoso)
            {
                return resultado;
            }

            return ResultadoOperacionDTO.Correcto(
                "El estudiante fue actualizado correctamente.");
        }

        public async Task<ResultadoOperacionDTO>
            CambiarEstadoAsync(
                int idEstudiante,
                int idUsuarioActual)
        {
            Estudiante? estudiante =
                await _repositoryEstudiante
                    .BuscarPorIdAsync(idEstudiante);

            if (estudiante is null)
            {
                return ResultadoOperacionDTO.Error(
                    "El estudiante solicitado no existe.");
            }

            if (!EsCuentaEstudiante(estudiante))
            {
                return ResultadoOperacionDTO.Error(
                    "La cuenta indicada no pertenece a un estudiante.");
            }

            return await _serviceUsuario
                .CambiarEstadoAsync(
                    estudiante.IdUsuario,
                    idUsuarioActual,
                    false);
        }

        private async Task CargarCatalogosAsync(
            EstudianteFormularioDTO formulario)
        {
            formulario.TiposBeneficiario =
                (await _repositoryTipoBeneficiario
                    .ListarAsync())
                .Select(tipo =>
                    new CatalogoDTO
                    {
                        Id =
                            tipo.IdTipoBeneficiario,
                        Nombre =
                            tipo.Nombre
                    })
                .ToList();

            formulario.GradosSecciones =
                (await _repositoryGradoSeccion
                    .ListarAsync())
                .Select(grado =>
                    new CatalogoDTO
                    {
                        Id =
                            grado.IdGradoSeccion,
                        Nombre =
                            $"{grado.Grado}-{grado.Seccion}"
                    })
                .ToList();
        }

        private static UsuarioFormularioDTO
            ConvertirFormulario(
                EstudianteFormularioDTO formulario,
                int idRolEstudiante)
        {
            return new UsuarioFormularioDTO
            {
                IdUsuario =
                    formulario.IdUsuario,

                Nombre =
                    formulario.Nombre,

                Apellidos =
                    formulario.Apellidos,

                Identificacion =
                    formulario.Identificacion,

                Correo =
                    formulario.Correo,

                IdRol =
                    idRolEstudiante,

                Contrasena =
                    formulario.Contrasena,

                ConfirmarContrasena =
                    formulario.ConfirmarContrasena,

                IdTipoBeneficiario =
                    formulario.IdTipoBeneficiario,

                IdGradoSeccion =
                    formulario.IdGradoSeccion,

                AnioIngreso =
                    formulario.AnioIngreso,

                Activo =
                    formulario.Activo
            };
        }

        private static bool EsCuentaEstudiante(
            Estudiante estudiante)
        {
            return string.Equals(
                estudiante
                    .IdUsuarioNavigation
                    .IdRolNavigation
                    .Nombre,
                RolEstudiante,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}