using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Productos
{
    public class ListProductosBodegaViewModel
    {
        public int fiIDProducto { get; set; }
        public string fcProducto { get; set; }
        public int fiIDUbicacion { get; set; }
        public string fcUbicacion { get; set; }
        public decimal fnCantidad { get; set; }
        public decimal fnStockMinimo { get; set; }
        public bool fbStockMinimo { get; set; }
    }
}