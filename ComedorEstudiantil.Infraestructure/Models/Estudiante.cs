using System;
using System.Collections.Generic;

namespace ComedorEstudiantil.Infraestructure.Models;

public partial class Estudiante
{
    public int IdEstudiante { get; set; }

    public int IdUsuario { get; set; }

    public int IdTipoBeneficiario { get; set; }

    public int? IdGradoSeccion { get; set; }

    public string? CodigoAcceso { get; set; }

    public short AnioIngreso { get; set; }

    public ulong Activo { get; set; }

    public virtual Gradoseccion? IdGradoSeccionNavigation { get; set; }

    public virtual Tipobeneficiario IdTipoBeneficiarioNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
