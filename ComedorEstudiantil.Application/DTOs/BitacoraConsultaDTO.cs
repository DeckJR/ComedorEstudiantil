using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComedorEstudiantil.Application.DTOs
{
    public class BitacoraConsultaDTO
    {
        public BitacoraFiltroDTO Filtro { get; set; } = new();
        public List<BitacoraListaDTO> Registros { get; set; } = new();
    }
}