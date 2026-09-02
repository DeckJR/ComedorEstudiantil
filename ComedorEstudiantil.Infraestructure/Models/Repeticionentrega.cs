using System;
using System.Collections.Generic;

namespace ComedorEstudiantil.Infraestructure.Models;

public partial class Repeticionentrega
{
    public int IdRepeticionEntrega { get; set; }

    public int IdEntrega { get; set; }

    public DateTime FechaHoraRepeticion { get; set; }

    public int IdUsuarioRegistro { get; set; }

    public sbyte MetodoRegistro { get; set; }

    public virtual Entrega IdEntregaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioRegistroNavigation { get; set; } = null!;
}
