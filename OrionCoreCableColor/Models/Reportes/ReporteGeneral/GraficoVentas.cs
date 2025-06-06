using System.Collections.Generic;

namespace OrionCoreCableColor.Models.Reportes.ReporteGeneral
{
    public class GraficoVentas
    {
        public class VentaAnual
        {
            public int Anio { get; set; }
            public string TipoAnio { get; set; }
            public List<VentaMensual> Meses { get; set; }
        }

        public class VentaMensual
        {
            public int FiMes { get; set; }
            public string FcMes { get; set; }
            public int FiConteoMensual { get; set; }
            public decimal FnMontoMensual { get; set; }
            public List<VentaSemanal> Semanas { get; set; }
        }

        public class VentaSemanal
        {
            public string FcSemana { get; set; }
            public int FiConteoVentas { get; set; }
            public decimal FnMontoVentas { get; set; }
        }
    }
}