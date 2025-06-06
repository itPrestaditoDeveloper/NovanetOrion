using OrionCoreCableColor.Models.Solicitudes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Prestamo
{
    public class InformacionCreditoClienteViewModel
    {
        [DisplayName("Nombre Clienye")]
        public string fcNombre { get; set; }
        [DisplayName("Prestamo")]
        public string fcPrestamo { get; set; }

        [DisplayName("Tipo Producto")]
        public string fcProducto { get; set; }

        [DisplayName("Agencia")]
        public string fcNombreAgencia { get; set; }

        [DisplayName("Gestor")]
        public string fcGestorCobros { get; set; }

        [DisplayName("Vendedor")]
        public string fcOficial { get; set; }


        [DisplayName("IDCliente")]
        public string fcIDCliente { get; set; }

        [DisplayName("Dia Atraso")]
        public int fiDiasAtraso { get; set; }

        [DisplayName("Cuota Atraso")]
        public short fiCuotasAtraso { get; set; }

        [DisplayName("Cuotas Pagadas")]
        public short fiCuotasPagadas { get; set; }

        [DisplayName("Total Cuotas")]
        public short fiTotalCuotas { get; set; }

        [DisplayName("Valor Cuota")]
        public decimal fnValorCuota { get; set; }

        [DisplayName("Capital")]
        public decimal fnCapitalOtorgado { get; set; }

        [DisplayName("Saldo Actual")]
        public decimal fnSaldoActual { get; set; }

        [DisplayName("Capital Vencido")]
        public decimal fnCapitalVencido { get; set; }

        [DisplayName("Interes vencido")]
        public decimal fnInteresesVencidos { get; set; }

        [DisplayName("Recargo")]
        public decimal fnRecargos { get; set; }

        [DisplayName("Saldo a ponerse al dia ")]
        public Nullable<decimal> fnSaldoPonerAlDia { get; set; }

        [DisplayName("Total cuenta")]
        public decimal fnTotalCuenta { get; set; }

        [DisplayName("Inception Date")]
        public System.DateTime fdFechaInicioPrestamo { get; set; }
        [DisplayName("First Payment Date")]
        public System.DateTime fdFechaPrimerPagoPrestamo { get; set; }
        [DisplayName("Final Payment Date")]
        public System.DateTime fdFechaVencePrestamo { get; set; }

        [DisplayName("Last Payment Received")]
        public System.DateTime fdUltimoAbono { get; set; }

        public decimal fnValorUltimoAbono { get; set; }



        [DisplayName("Customer Name")]
        public string CustomerName { get; set; }

        [DisplayName("Phone Number")]
        public string Telephone { get; set; }


        [DisplayName("Total Paid")]
        public decimal TotalAmountReceived { get; set; }

        [DisplayName("Principal Paid")]
        public decimal Amount_Principal { get; set; }

        [DisplayName("Interest Paid")]
        public decimal Amount_Interest { get; set; }

        [DisplayName("Late Fees Paid")]
        public decimal Amount_LateFees { get; set; }


        public string Frecuency { get; set; }

        [DisplayName("APR")]
        public decimal APR { get; set; }

        [DisplayName("Flat Rate")]
        public decimal FlatRate { get; set; }

        [DisplayName("Payments Quantity")]
        public int PaymentsQuantity { get; set; }

        [DisplayName("Discount")]
        public decimal Discount { get; set; }



        [DisplayName("Payments Dealer Recourse")]
        public int PaymentsTotalDealerRecourse { get; set; }

        public int PaymentsAvailableDealerRecourse { get; set; }

        [DisplayName("Garantia")]
        public decimal DownPayment { get; set; }

        [DisplayName("Valor Total de Garantia")]
        public decimal ValorMercadoGarantia { get; set; }

        public decimal fnValorAPrestar { get; set; }
        public decimal fnCostoGPS { get; set; }
        public decimal fnGastosDeCierre { get; set; }
        public decimal fnValorTotalSeguro { get; set; }
        public decimal fnValorTotalFinanciamiento { get; set; }
        public decimal fnValorTotalContrato { get; set; }
        public string fcTipoDePlazo { get; set; }
        public string fcPrestamoSAF { get; set; }
        public string fcClienteSAF { get; set; }

        public Nullable<int> SolicitudMaestroNovanet { get; set; }

        public List<SolicitudesGarantiaViewModel> ListaGarantia { get; set; }
        public List<ListImagenesInstalacionViewModel> ListaImagenesInstalacion { get; set; }
        public sp_OrionSolicitud_Detalle_ClienteListarViewModel DatosCLientes { get; set; }

        // public InfoDetailAutoLoanViewModel AutoLoanInfo { get; set; }
    }

    public class ListImagenesInstalacionViewModel
    {
        public int fiIDSolicitud { get; set; }

        public int fiIDWarrantyControlType { get; set; }

        public string fcDescription { get; set; }

        public string fcUrlFile { get; set; }

        public string fcFileName { get; set; }

    }
}