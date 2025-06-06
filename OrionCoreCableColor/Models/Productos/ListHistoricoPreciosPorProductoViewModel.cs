using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Productos
{
    public class ListHistoricoPreciosPorProductoViewModel
    {
        public int fiIdProductoPreciosDetalle { get; set; }
        public decimal fnValorProductoMN { get; set; }
        public decimal fnValorProductoME { get; set; }
    }
}