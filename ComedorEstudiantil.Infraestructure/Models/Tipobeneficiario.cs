using System;
using System.Collections.Generic;

namespace ComedorEstudiantil.Infraestructure.Models;

public partial class Tipobeneficiario
{
    public int IdTipoBeneficiario { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Estudiante> Estudiante { get; set; } = new List<Estudiante>();
}
