using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Precalificado
{
    public class ListProductosPorSolicitud
    {
        public int fiIDSolicitud { get; set; }
        public int fiIDProducto { get; set; }
        public string fcProducto { get; set; }
        public string fcEnPaquete { get; set; }
        public decimal fnCantidadInstalado { get; set; }
        public decimal fnCantidadInventario { get; set; }
    }
}