using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Prestamo
{
    public class CrearFacturacionViewModel
    {
        public int fiIDFacturacionMaestro { get; set; }
        [DisplayName("Cliente")]
        public int fiIDEquifax { get; set; }
        [DisplayName("Fecha Transaccion")]
        public DateTime fdFechaTransaccion { get; set; }
        [DisplayName("Correlativo")] ///EJEMPLO CXC-0000
        public string fcCorrelativoDocumento { get; set; }
        [DisplayName("Factura")] /////
        public string fcReferenciaDocumento { get; set; }
        [DisplayName("CAI")]
        public string fcCAI { get; set; }
        [DisplayName("Sub Total")]
        public decimal fnSubTotal { get; set; }
        [DisplayName("Importe Exento")]
        public decimal fnImporteExento { get; set; }
        [DisplayName("Importe Exonerado")]
        public decimal fnImporteExonerado { get; set; }
        [DisplayName("Importe Gravado")]
        public decimal fnImporteGravado { get; set; }
        [DisplayName("Concepto")]
        public string fcConcepto { get; set; }
        [DisplayName("Fecha Vencimiento")]
        public DateTime fdFechaVencimiento { get; set; }
        [DisplayName("Centro de Costo")]
        public string fcCentrodeCosto { get; set; }

        public string fcPartidaLempiras { get; set; }
        public string fcPartidaDolares { get; set; }
        public decimal fnTasadeCambio { get; set; }
        public string fcToken { get; set; }
        [DisplayName("Nombre")]
        public string fcNombreCliente { get; set; }
        [DisplayName("Correo")]
        public string fcCorreoCliente { get; set; }
        [DisplayName("Telefono")]
        public string fcTelefonoCliente { get; set; }
        [DisplayName("Identidad")]
        public string fcIdentidad { get; set; }
        [DisplayName("ISV")]
        public decimal fnImpuestos { get; set; }
        [DisplayName("Total")]
        public decimal fnTotalFactura { get; set; }
        [DisplayName("Descuento")]
        public decimal fnDescuento { get; set; }
        public DateTime fdFechaRangoInicialFacturacion { get; set; }
        public DateTime fdFechaRangoFinalFacturacion { get; set; }
        public string fcRangoInicialFacturacion { get; set; }
        public string fcRangoFinalFacturacion { get; set; }
        [DisplayName("RTN")]
        public string fcRTN { get; set; }
        public string fcValorEnLetras { get; set; }
        public decimal fnValorLempiras { get; set; }
        public List<ListFacturacionDetalleViewModel> DetalleFacturacion { get; set; }
        public int fiIDInventarioMovimientoMaestro { get; set; }
        public bool fbEditar { get; set; }
        public int fiMoneda { get; set; }
    }
}