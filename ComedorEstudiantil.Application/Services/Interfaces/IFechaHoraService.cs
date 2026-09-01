using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComedorEstudiantil.Application.Services.Interfaces
{
    public interface IFechaHoraService
    {
        DateTime ObtenerAhora();
        DateOnly ObtenerFechaActual();
    }
}
