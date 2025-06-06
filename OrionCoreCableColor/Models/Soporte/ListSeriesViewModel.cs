using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Soporte
{
    public class ListSeriesViewModel
    {
        public int fiIDProducto { get; set; }
        public string fcCodigoSerie1 { get; set; }
        public string fcCodigoSerie2 { get; set; }
        public bool fbFaltante { get; set; }
        public int fiIDMovimiento { get; set; }
    }
}