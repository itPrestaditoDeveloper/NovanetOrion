using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Home
{
    public class ListProductosPorUbicacionViewModel
    {
        public int fiIDMovimiento { get; set; }
        public int fiIDUbicacion { get; set; }
        public string fcUbicacion { get; set; }
        public int fiIDProducto { get; set; }
        public string fcProducto { get; set; }
        public string fcCodigoSerie1 { get; set; }
        public string fcCodigoSerie2 { get; set; }
        public decimal fnCantidad { get; set; }
        public string fcReferenciaMovimiento { get; set; }
        public bool fbUsuarioPrincipal { get; set; }
        public bool fbSeleccionado { get; set; }
        public bool fbConsumible { get; set; }
    }
}