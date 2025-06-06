using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Base
{
    public class ListUbicacionesPorTipo
    {
        public int fiIDUbicacion { get; set; }
        public string fcUbicacion { get; set; }
        public string fcTipoUbicacion { get; set; }
        public string fcNombreCorto { get; set; }
        public string fcNombreComercial { get; set; }
        public bool fbUbicacionExterna { get; set; }
        
    }
}