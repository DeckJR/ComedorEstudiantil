using System;
using System.Collections.Generic;

namespace ComedorEstudiantil.Infraestructure.Models;

public partial class Actividad
{
    public int IdActividad { get; set; }

    public string Nombre { get; set; } = null!;

    public DateOnly Fecha { get; set; }

    public string? Descripcion { get; set; }

    public ulong Activo { get; set; }

    public virtual ICollection<Menu> Menu { get; set; } = new List<Menu>();
}
