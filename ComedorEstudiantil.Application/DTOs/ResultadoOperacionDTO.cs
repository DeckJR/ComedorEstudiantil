using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComedorEstudiantil.Application.DTOs
{
    public class ResultadoOperacionDTO
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;

        public static ResultadoOperacionDTO Correcto(string mensaje)
        {
            return new ResultadoOperacionDTO
            {
                Exitoso = true,
                Mensaje = mensaje
            };
        }

        public static ResultadoOperacionDTO Error(string mensaje)
        {
            return new ResultadoOperacionDTO
            {
                Exitoso = false,
                Mensaje = mensaje
            };
        }
    }
}
