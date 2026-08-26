using System;
using System.Collections.Generic;

namespace ComedorEstudiantil.Infraestructure.Models;

public partial class Bitacora
{
    public long IdBitacora { get; set; }

    public int? IdUsuario { get; set; }

    public string Accion { get; set; } = null!;

    public string Entidad { get; set; } = null!;

    public int? IdEntidad { get; set; }

    public string? Detalle { get; set; }

    public DateTime FechaHora { get; set; }

    public string? IpOrigen { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
