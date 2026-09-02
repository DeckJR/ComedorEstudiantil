using System;
using System.Collections.Generic;

namespace ComedorEstudiantil.Infraestructure.Models;

public partial class Entrega
{
    public int IdEntrega { get; set; }

    public int IdSolicitud { get; set; }

    public DateTime FechaHoraEntrega { get; set; }

    public int IdUsuarioEntrego { get; set; }

    public sbyte MetodoEntrega { get; set; }

    public virtual Solicitud IdSolicitudNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioEntregoNavigation { get; set; } = null!;

    public virtual ICollection<Repeticionentrega> Repeticionentrega { get; set; } = new List<Repeticionentrega>();
}
