using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComedorEstudiantil.Application.DTOs
{
    public class MenuPublicoDTO
    {
        public DateOnly FechaSeleccionada { get; set; }
        public List<MenuListaDTO> Menus { get; set; } = new();
    }
}
