using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Prestamo
{
    public class sp_Prestamos_ClientesaPagarPorFecha_Result
    {
        public string fcIdentidad { get; set; }
        public string fcNombreSAF { get; set; }
        public string fcTelefonos { get; set; }
        public Nullable<decimal> fnValorCuota { get; set; }
        public System.DateTime fdFechadeCuota { get; set; }
        public Nullable<decimal> fnSaldoCuota { get; set; }
        public short fiCuota { get; set; }
        public string fcArticulosdelContrato { get; set; }
        public string fcTipoSolicitud { get; set; }
        public string fcEstadoFicoLink { get; set; }
        public int fiIDEstadoInstalacion { get; set; }
        public System.DateTime fdFechaInstalacionFinal { get; set; }

        public Nullable<decimal> fnValorCuotaDolares { get; set; }
        public Nullable<decimal> fnValorCuotaLempiras { get; set; }

        public Nullable<decimal> fnSaldoCuotaDolares { get; set; }
        public Nullable<decimal> fnSaldoCuotaLempiras { get; set; }
        public Nullable<decimal> CuotaMatriz { get; set; }
    }
}