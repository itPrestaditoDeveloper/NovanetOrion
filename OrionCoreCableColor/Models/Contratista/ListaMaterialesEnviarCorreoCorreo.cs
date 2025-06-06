using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OrionCoreCableColor.Models.Contratista
{
    public class ListaMaterialesEnviarCorreoCorreo
    {
        public int fiIDSolicitud { get; set; }
        public int IdCliente { get; set; }
        public string Observaciones { get; set; }
        public string NombreCliente { get; set; }
        public string Opcion { get; set; }

        public int fiIDDetalle { get; set; }
        public int fiIDProducto { get; set; }

        public string fcProductoAntes { get; set; }
        public double fnCantidadAntes { get; set; }
        public string fcProductoDespues { get; set; }
        public double fnCantidadDespues { get; set; }

    }
}