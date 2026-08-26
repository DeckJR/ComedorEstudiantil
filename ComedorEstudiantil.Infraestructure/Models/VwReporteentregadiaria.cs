using System;
using System.Collections.Generic;

namespace ComedorEstudiantil.Infraestructure.Models;

public partial class VwReporteentregadiaria
{
    public DateOnly Fecha { get; set; }

    public string TipoComida { get; set; } = null!;

    public string Cedula { get; set; } = null!;

    public string NombreCompleto { get; set; } = null!;

    public string Rol { get; set; } = null!;

    public string? TipoBeneficiario { get; set; }

    public DateTime FechaHoraEntrega { get; set; }

    public sbyte MetodoEntrega { get; set; }

    public string EntregadoPor { get; set; } = null!;
}
