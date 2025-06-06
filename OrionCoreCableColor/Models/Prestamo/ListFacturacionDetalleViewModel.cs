using OrionCoreCableColor.Models.Productos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Prestamo
{
    public class ListFacturacionDetalleViewModel
    {
        public string Foto { get; set; }
        public int fiIDFacturacionDetalle { get; set; }
        public int fiIDFacturacionMaestro { get; set; }
        public int fiIDProducto { get; set; }
        public string fcProducto { get; set; }
        public decimal fnCantidad { get; set; }
        public decimal fnExistencia { get; set; }
        [DisplayName("VALOR DE CONTADO:")]
        public decimal fnValorVentaDeContado { get; set; }
        public decimal fnPorcentajeImpuesto1 { get; set; }
        public decimal fnPorcentajeImpuesto2 { get; set; }
        public decimal fnValorProductoME { get; set; }
        public decimal fnSubTotal { get; set; }
        public decimal fnTotal { get; set; }
        public string fcToken { get; set; }
        public bool fbAplicaImpuesto { get; set; }
        public decimal fnValorImpuesto { get; set; }
        public bool fbSeleccionado { get; set; }
        public bool fbEditar { get; set; }
        public int fiIDInventarioMovimientoMaestro { get; set; }
        public List<ListInventarioMovimientoDetalleViewModel> DetalleInventario { get; set; }
    }
}