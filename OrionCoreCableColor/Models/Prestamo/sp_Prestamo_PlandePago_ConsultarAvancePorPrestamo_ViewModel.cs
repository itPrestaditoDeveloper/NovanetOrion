using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Prestamo
{
    public class sp_Prestamo_PlandePago_ConsultarAvancePorPrestamo_ViewModel
    {
        public DateTime fdFechaCuota { get; set; }
        public decimal fnCapitalInicial { get; set; }

        public decimal fnCapitalPactado { get; set; }
        public decimal fnInteresPactado { get; set; }
        public decimal fnSeguro1 { get; set; }
        public decimal fnSeguro2 { get; set; }
        public decimal fnServicio1 { get; set; }
        public decimal fnValorCuota { get; set; }
        public decimal fnSaldoCapital { get; set; }
        public decimal fnSaldoInteres { get; set; }
        public decimal fnSaldoSeguro1 { get; set; }
        public decimal fnSaldoSeguro2 { get; set; }
        public decimal fnSaldoServicio1 { get; set; }
        public decimal fnSaldoCuota { get; set; }

        public int ItemNumber { get; set; }
    }
}