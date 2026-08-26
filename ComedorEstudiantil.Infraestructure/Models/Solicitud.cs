using System;
using System.Collections.Generic;

namespace ComedorEstudiantil.Infraestructure.Models;

public partial class Solicitud
{
    public int IdSolicitud { get; set; }

    public int IdUsuario { get; set; }

    public int IdMenu { get; set; }

    public DateTime FechaHoraSolicitud { get; set; }

    public sbyte Estado { get; set; }

    public sbyte MetodoMarcado { get; set; }

    public int? IdUsuarioMarco { get; set; }

    public virtual Entrega? Entrega { get; set; }

    public virtual Menu IdMenuNavigation { get; set; } = null!;

    public virtual Usuario? IdUsuarioMarcoNavigation { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
