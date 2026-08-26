using System;
using System.Collections.Generic;

namespace ComedorEstudiantil.Infraestructure.Models;

public partial class Gradoseccion
{
    public int IdGradoSeccion { get; set; }

    public string Grado { get; set; } = null!;

    public string Seccion { get; set; } = null!;

    public virtual ICollection<Estudiante> Estudiante { get; set; } = new List<Estudiante>();
}
