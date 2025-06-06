using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Prestamo
{
    public class FacturacionDocumentoViewModel
    {
        public int fiIDFacturacionDocumento { get; set; }
        public int fiIDFacturacionMaestro { get; set; }
        public string fcNombre { get; set; }
        public string fcExtension { get; set; }
        public string fcRuta { get; set; }
        [DisplayName("CUENTA DE BANCO")]
        public string fcCuentaContableBanco { get; set; }
        [DisplayName("VALOR DE PAGO")]
        public decimal fnValorPago { get; set; }
        public HttpPostedFileBase Documento { get; set; }
        [DisplayName("FECHA DE PAGO")]
        public DateTime fdFechaDePago {get;set;}
        [DisplayName("REFERENCIA DE PAGO")]
        public string fcReferencia { get; set; }
        [DisplayName("CONCEPTO")]
        public string fcConceptoPago { get; set; }
    }
}