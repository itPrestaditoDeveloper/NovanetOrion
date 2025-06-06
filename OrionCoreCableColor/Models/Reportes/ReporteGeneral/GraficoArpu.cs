using System.Collections.Generic;

namespace OrionCoreCableColor.Models.Reportes.ReporteGeneral
{
    public class GraficoArpu
    {

        public class ArpuSemana
        {
            public string fcSemana { get; set; }
            public decimal fnArpuSemanal { get; set; }
            public decimal fnCuotaMaximaSemanal { get; set; }
            public decimal fnCuotaMinimaSemanal { get; set; }
        }

        public class ArpuMes
        {
            public int fiMes { get; set; }
            public string fcMes { get; set; }
            public decimal fnArpuMensual { get; set; }
            public decimal fnCuotaMaxima { get; set; }
            public decimal fnCuotaMinima { get; set; }
            public List<ArpuSemana> semanas { get; set; }
        }

        public class ArpuResponse
        {
            public int anio { get; set; }
            public List<ArpuMes> meses { get; set; }
            public string tipoAnio { get; set; }
        }
    }
}