using System.Collections.Generic;

namespace OrionCoreCableColor.Models.Reportes.ReporteGeneral
{
    public class GraficoMora
    {

        public class DataMoraResponse
        {
            public int Anio { get; set; }

            public List<MoraMes> Meses { get; set; }

            public string TipoAnio { get; set; }
        }

        public class MoraMes
        {
            public int FiMes { get; set; }

            public string FcMes { get; set; }

            public int FiClientesEnMora { get; set; }

            public int FiTotalClientes { get; set; }

            public decimal FnPorcentajedeMora { get; set; }
        }
    }
}