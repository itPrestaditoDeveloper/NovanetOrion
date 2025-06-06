using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Contratista
{
    public class SolicitudInstalacion_Confirmacion_Listar
    {
        public int fiIDSolicitud { get; set; }
        public int fiIDContratistaSolicitud { get; set; }
        public int fiIDProducto { get; set; }
        public string fcProducto { get; set; }
        public Decimal fnCantidad { get; set; }
    }
}