using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Productos
{
    public class AjusteDeConsumibleViewModel
    {
        public int fiIDProducto { get; set; }
        public int fiIDUbicacionIncial { get; set; }
        public int fiIDSolicitudInicial { get; set; }
        public int fiIDUbicacionDestino { get; set; }
        public int fiIDSolicitudDestino { get; set; }
        public decimal fnCantidad { get; set; }
    }
}