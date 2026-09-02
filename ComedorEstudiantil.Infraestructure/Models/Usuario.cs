using System;
using System.Collections.Generic;

namespace ComedorEstudiantil.Infraestructure.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Identificacion { get; set; } = null!;

    public string CodigoBarras { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string ContrasenaHash { get; set; } = null!;

    public int IdRol { get; set; }

    public bool? Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public bool DebeCambiarContrasena { get; set; }

    public DateTime? FechaUltimoCambioContrasena { get; set; }

    public virtual ICollection<Bitacora> Bitacora { get; set; } = new List<Bitacora>();

    public virtual ICollection<Entrega> Entrega { get; set; } = new List<Entrega>();

    public virtual Estudiante? Estudiante { get; set; }

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<Menu> Menu { get; set; } = new List<Menu>();

    public virtual ICollection<Repeticionentrega> Repeticionentrega { get; set; } = new List<Repeticionentrega>();

    public virtual ICollection<Solicitud> SolicitudIdUsuarioMarcoNavigation { get; set; } = new List<Solicitud>();

    public virtual ICollection<Solicitud> SolicitudIdUsuarioNavigation { get; set; } = new List<Solicitud>();
}
