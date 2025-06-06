using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Productos
{
    public class GuardarMovimientoInventarioViewModel
    {
        public int fiIDMovimientoMaestro { get; set; }
        [DisplayName("Moneda")]
        public int fiIDMoneda { get; set; }
        [DisplayName("Numero de factura")]
        public string fcNumeroFactura { get; set; }
        [DisplayName("Fecha de la factura")]
        public DateTime fdFechaFactura { get; set; }
        [DisplayName("Referencia")]
        public int fiIDProveedor { get; set; }
        [DisplayName("Referencia")]
        public string fcReferenciaFactura { get; set; }
        [DisplayName("Descripcion")]
        public string fcDescripcionFactura { get; set; }
        [DisplayName("CAI")]
        public string fcCAI { get; set; }
        [DisplayName("Tipo de Movimiento")]
        public int fiIDTipoMovimiento { get; set; }
        public int fiTipoAfectacion { get; set; }
        [DisplayName("Importe Gravado")]
        public decimal fnImporteGravado { get; set; }
        [DisplayName("Importe Exento")]
        public decimal fnImporteExento { get; set; }
        [DisplayName("Importe Exonerado")]
        public decimal fnImporteExonerado { get; set; }
        [DisplayName("Total sin ISV")]
        public decimal fnSaldo { get; set; }
        [DisplayName("ISV")]
        public decimal fnISV { get; set; }
        [DisplayName("Total")]
        public decimal fnTotalFactura { get; set; }
        public HttpPostedFileBase DocumentoFactura { get; set; }
        [DisplayName("Foto de la factura")]
        public string fcDocumentoFactura { get; set; }
        public string fcDocumentoExtension { get; set; }
        [DisplayName("Ubicacion de Inventario")]
        public int fiIDUbicacion { get; set; }
        public List<ListMovimientoDetalleViewModel> DetalleMovimiento { get; set; }
        public decimal TotalCantidad { get; set; }
        public decimal TotalPrecioMN { get; set; }
        public decimal TotalPrecioME { get; set; }
        public string fcSignoMoneda { get; set; }
        
        [DisplayName("Valor cuota mensual")]
        public decimal fnValorCuotaMensual { get; set; }
        public bool fbEditar { get; set; }
        
    }
}