using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComedorEstudiantil.Application.DTOs
{
    public class CodigoBarrasUsuarioDTO
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Identificacion { get; set; } = string.Empty;
        public string CodigoBarras { get; set; } = string.Empty;
    }
}