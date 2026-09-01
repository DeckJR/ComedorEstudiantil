using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComedorEstudiantil.Infraestructure.Data;
using ComedorEstudiantil.Infraestructure.Models;
using ComedorEstudiantil.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ComedorEstudiantil.Infraestructure.Repository.Implementations
{
    public class RepositoryReporte : IRepositoryReporte
    {
        private readonly ComedorEstudiantilContext _context;

        public RepositoryReporte(
            ComedorEstudiantilContext context)
        {
            _context = context;
        }

        public async Task<List<Solicitud>> ListarSolicitudesAsync(
            DateOnly fechaInicio,
            DateOnly fechaFin)
        {
            return await _context.Set<Solicitud>()
                .AsNoTracking()
                .Include(solicitud =>
                    solicitud.IdUsuarioNavigation)
                    .ThenInclude(usuario =>
                        usuario.IdRolNavigation)
                .Include(solicitud =>
                    solicitud.IdUsuarioNavigation)
                    .ThenInclude(usuario =>
                        usuario.Estudiante)
                    .ThenInclude(estudiante =>
                        estudiante!.IdTipoBeneficiarioNavigation)
                .Include(solicitud =>
                    solicitud.IdUsuarioNavigation)
                    .ThenInclude(usuario =>
                        usuario.Estudiante)
                    .ThenInclude(estudiante =>
                        estudiante!.IdGradoSeccionNavigation)
                .Include(solicitud =>
                    solicitud.IdUsuarioMarcoNavigation)
                .Include(solicitud =>
                    solicitud.IdMenuNavigation)
                    .ThenInclude(menu =>
                        menu.IdTipoComidaNavigation)
                .Include(solicitud =>
                    solicitud.Entrega)
                .Where(solicitud =>
                    solicitud.IdMenuNavigation.Fecha >= fechaInicio &&
                    solicitud.IdMenuNavigation.Fecha <= fechaFin)
                .OrderByDescending(solicitud =>
                    solicitud.IdMenuNavigation.Fecha)
                .ThenBy(solicitud =>
                    solicitud.IdMenuNavigation
                        .IdTipoComidaNavigation.Nombre)
                .ThenBy(solicitud =>
                    solicitud.IdUsuarioNavigation.Apellidos)
                .ToListAsync();
        }

        public async Task<List<Entrega>> ListarEntregasAsync(
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            return await _context.Set<Entrega>()
                .AsNoTracking()
                .Include(entrega =>
                    entrega.IdSolicitudNavigation)
                    .ThenInclude(solicitud =>
                        solicitud.IdUsuarioNavigation)
                    .ThenInclude(usuario =>
                        usuario.IdRolNavigation)
                .Include(entrega =>
                    entrega.IdSolicitudNavigation)
                    .ThenInclude(solicitud =>
                        solicitud.IdUsuarioNavigation)
                    .ThenInclude(usuario =>
                        usuario.Estudiante)
                    .ThenInclude(estudiante =>
                        estudiante!.IdTipoBeneficiarioNavigation)
                .Include(entrega =>
                    entrega.IdSolicitudNavigation)
                    .ThenInclude(solicitud =>
                        solicitud.IdUsuarioNavigation)
                    .ThenInclude(usuario =>
                        usuario.Estudiante)
                    .ThenInclude(estudiante =>
                        estudiante!.IdGradoSeccionNavigation)
                .Include(entrega =>
                    entrega.IdSolicitudNavigation)
                    .ThenInclude(solicitud =>
                        solicitud.IdMenuNavigation)
                    .ThenInclude(menu =>
                        menu.IdTipoComidaNavigation)
                .Include(entrega =>
                    entrega.IdUsuarioEntregoNavigation)
                .Where(entrega =>
                    entrega.FechaHoraEntrega >= fechaInicio &&
                    entrega.FechaHoraEntrega < fechaFin)
                .OrderByDescending(entrega =>
                    entrega.FechaHoraEntrega)
                .ToListAsync();
        }
    }
}