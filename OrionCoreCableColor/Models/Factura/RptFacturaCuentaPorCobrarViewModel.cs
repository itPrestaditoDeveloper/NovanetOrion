using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Factura
{
    public class RptFacturaCuentaPorCobrarViewModel
    {

        public string Logo { get; set; }
        public string Empresa { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Domicilio { get; set; }
        public string RTNEmpresa { get; set; }
        public string Factura { get; set; }
        public string CAI { get; set; }
        public int IdCliente { get; set; }
        public string Cliente { get; set; }
        public string DireccionCliente { get; set; }
        public DateTime FechaLimiteEmision { get; set; }
        public string RangoAutorizacion { get; set; }
        public string NoDeclaracion { get; set; }
        public string RTNCliente { get; set; }
        //public List<RptFacturaDetalleCuentaPorCobrarViewModel> ListaDetalle { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Exento { get; set; }
        public decimal Exonerado { get; set; }
        public decimal Gravado { get; set; }
        public decimal ISV { get; set; }
        public decimal Total { get; set; }
        public string TotalLetras { get; set; }
        public string TipoReporte { get; set; }
        public string Moneda { get; set; }
        public bool EsAnulado { get; set; }
        public DateTime Fecha { get; set; }
        public string fcConcepto { get; set; }
        public string fcProductoDescripcion { get; set; }
        public string fcSimboloMoneda { get; set; }
        public decimal fnTasaDeCambio { get; set; }
        public decimal fnTotalLempiras { get; set; }
    }
}