using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Prestamo
{
    public class CustomerStatementDetailViewModel
    {
        public string fcIDCliente { get; set; }
        public string fcAgencia { get; set; }
        public string fcDocumento { get; set; }
        public string fcOperador { get; set; }
        public DateTime fdFechaTran { get; set; }
        public string fcReferencia { get; set; }
        public decimal fnOtrosCargos { get; set; }
        public decimal fnSegurosDeuda { get; set; }
        public decimal fnSegurosDanos { get; set; }
        public decimal fnInteresOrdinario { get; set; }
        public decimal fnInteresMoratorio { get; set; }
        public decimal fnCapital { get; set; }
        public decimal fnTotal { get; set; }

        public decimal SaldoDelPeriodo { get; set; }
        public decimal fnServicio1 { get; set; }

        //helper
        public int ItemNumber { get; set; }
    }
}