using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.Models.Productos
{
    public class ListPreciosProductosViewModel
    {
        public int fiIDProducto { get; set; }
        public List<SelectListItem> Precios { get; set; }
        public List<ListHistoricoPreciosPorProductoViewModel> HistoricoPrecios { get; set; }
    }
}