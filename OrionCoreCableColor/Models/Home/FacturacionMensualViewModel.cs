using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Home
{
    public class FacturacionMensualViewModel
    {
        public int fiAno { get; set; }
        public int fiMes { get; set; }
        public string fcMes { get; set; }
        public double fnValorAbono { get; set; }
        public double fnValorAbonoAcumulado { get; set; }
        public double fnPorcentajeCambio { get; set; }
    }
}