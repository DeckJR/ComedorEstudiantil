using System;
using System.Collections.Generic;

namespace ComedorEstudiantil.Infraestructure.Models;

public partial class Tipocomida
{
    public int IdTipoComida { get; set; }

    public string Nombre { get; set; } = null!;

    public TimeOnly HoraLimiteMarcar { get; set; }

    public ulong Activo { get; set; }

    public virtual ICollection<Menu> Menu { get; set; } = new List<Menu>();
}
