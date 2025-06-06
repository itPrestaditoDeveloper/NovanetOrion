using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.DashBoard
{
    public class BulletViewModel
    {
        public double fnIngresosdelGlobales { get; set; }
        public double fnIngresosdelMes { get; set; }
        public int fiClientesPrecalificados { get; set; }
        public int fiClientesNuevosMes { get; set; }
        public int fiClientesEnAtraso { get; set; }
        public int fiClientesReconectados { get; set; }
        public int fiClientesCortados { get; set; }
        public int fiClientesActivos { get; set; }
        public int fiClientesNetos { get; set; }

    }
}