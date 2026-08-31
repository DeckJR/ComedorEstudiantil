using System;
using System.Collections.Generic;

namespace ComedorEstudiantil.Infraestructure.Models;

public partial class Menu
{
    public int IdMenu { get; set; }

    public int IdTipoComida { get; set; }

    public DateOnly Fecha { get; set; }

    public string Descripcion { get; set; } = null!;

    public int? IdActividad { get; set; }

    public bool Publicado { get; set; }

    public int IdUsuarioCreador { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Actividad? IdActividadNavigation { get; set; }

    public virtual Tipocomida IdTipoComidaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioCreadorNavigation { get; set; } = null!;

    public virtual ICollection<Solicitud> Solicitud { get; set; } = new List<Solicitud>();
}
