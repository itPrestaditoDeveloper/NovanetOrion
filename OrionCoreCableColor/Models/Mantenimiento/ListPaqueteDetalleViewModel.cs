using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Mantenimiento
{
    public class ListPaqueteDetalleViewModel
    {
        public int fiIdPaquete { get; set; }
        public int fiIDProducto { get; set; }
        public string fcProducto { get; set; }
        public int fiCantidad { get; set; }
        public string fcToken { get; set; }
        public bool fbSeleccionado { get; set; }
    }
}