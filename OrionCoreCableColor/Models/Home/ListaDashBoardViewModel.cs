using OrionCoreCableColor.Models.Reportes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Home
{
    public class ListaDashBoardViewModel
    {
        public List<FacturacionMensualViewModel>  facturacionmensual { get; set; }

        public List<VentasMensualesViewModel> ventasMensuales { get; set; }
        public List<ArpusViewModel> Arpumensuales { get; set; }
        public List<ProyeccionyActualViewModel> proyecciones { get; set; }
    }
}