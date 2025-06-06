using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Home
{
    public class VentasMensualesViewModel
    {
        public int fiAno { get; set; }
        public int fiMes { get; set; }
        public string fcMes { get; set; }
        public int fiConteoVentas { get; set; }
        public double fnMontoVentas { get; set; }
    }
}