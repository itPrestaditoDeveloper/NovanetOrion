using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Soporte
{
    public class RespuestaApiCpheusViewModel
    {
        public string fcCodigoCliente { get; set; }
        public string fcNombre { get; set; }
        public string fcNumeroOrdenCfeus { get; set; }
        public DateTime fdFechaSolicitud { get; set; }
        public DateTime fdFechaEjecucion { get; set; }
        public string fcTipoProceso { get; set; }
        public string fcRespuestaServicio { get; set; }
        public string fcEstado { get; set; }
    }
}