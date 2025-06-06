using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Mantenimiento
{
    public class ListCatalogoPlazos
    {
        public int fiIDPlazo { get; set; }
        public string fcDescripcionPlazo { get; set; }
        public int fiTipoPlazo { get; set; }
        public bool fbEstadoPlazo { get; set; }
        public int fiMeses { get; set; }
    }
}