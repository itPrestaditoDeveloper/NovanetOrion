using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Manto
{
    public class DetalleInstalacionViewModel
    {
        public int index { get; set; }
        public int fiIDContratista_SolicitudInstalacion_Detalle { get; set; }
        public int fiIDContratista_SolicitudInstalacion { get; set; }
        public int fiIDProducto { get; set; }
        public string fcProducto { get; set; }
        public int fiIDInventario { get; set; }
        public decimal fnCantidad { get; set; }
        public string fcSerie { get; set; }
        public string fcMac { get; set; }
        public bool fbNuevo { get; set; }
    }
}