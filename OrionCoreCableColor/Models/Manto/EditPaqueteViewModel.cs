using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Manto
{
    public class EditPaqueteViewModel
    {
        public int fiIDSolicitud { get; set; }
        public int fiIDPaquete { get; set; }
        public string fcPaquete { get; set; }
        public List<ListProductosPorPaquete> Productos { get; set; }
    }


    public class ListProductosPorPaquete
    {
        public int fiIDProducto { get; set; }
        public string fcProducto { get; set; }
        public int fiCantidad { get; set; }
        public bool fbIncluido { get; set; }
        public bool fbEditar { get; set; }
    }
}