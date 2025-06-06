using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Productos
{
    public class ListMovimientoDetalleViewModel
    {
        public int fiIDMovimiento { get; set; }
        public int fiIDMovimientoMaestro { get; set; }
        public string fcNumeroFactura { get; set; }
        public int fiIDProducto { get; set; }
        [DisplayName("Producto")]
        public string fcProducto { get; set; }
        public int fiIdProductoPreciosDetalleActual { get; set; }
       
        [DisplayName("Ubicacion")]
        public int fiIDUbicacion { get; set; }
        [DisplayName("Solicitud")]
        public int? fiIDSolicitud { get; set; }
        [DisplayName("Serie")]
        public string fcCodigoSerie1 { get; set; }
        [DisplayName("Serie2/Mac/IMEI")]
        public string fcCodigoSerie2 { get; set; }
        [DisplayName("Referencia")]
        public string fcReferenciaMovimiento { get; set; }
        [DisplayName("Cantidad")]
        public decimal fnCantidad { get; set; }
        public string fcToken { get; set; }
        public bool fbEditar { get; set; }
        public bool fbEscaneado { get; set; }
        public bool fbEditado { get; set; }

        public string fcUbicacion { get; set; }
        public DateTime fdFechaMovimiento { get; set; }
        [DisplayName("Tipo de producto")]
        public string fcTipoProducto { get; set; }
        public bool fbConsumible { get; set; }
        public bool fbFechaMovimiento { get; set; }
        public decimal fnValorProductoME { get; set; }
        public decimal fnValorProductoMN { get; set; }
    }
}