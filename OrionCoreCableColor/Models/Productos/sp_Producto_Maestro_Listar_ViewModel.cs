using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.Models.Productos
{
    public class sp_Producto_Maestro_Listar_ViewModel
    {
        public int fiIDProducto { get; set; }
        public string fcCodigodeBarras { get; set; }
        public string fcProducto { get; set; }
        public string fcImagenProducto { get; set; }
        public byte fiIDMarca { get; set; }
        public string fcMarca { get; set; }
        public byte fiIDModelo { get; set; }
        public string fcModelo { get; set; }
        public byte fiIDTipoProducto { get; set; }
        public string fcTipoProducto { get; set; }
        public decimal fnValorProductoMN { get; set; }
        public decimal fnValorProductoME { get; set; }
        public decimal fnPorcentajeImpuesto1 { get; set; }
        public decimal fnPorcentajeImpuesto2 { get; set; }
        public byte fiEstadoProducto { get; set; }
        public byte fiProductoInventariable { get; set; }
        public System.DateTime fdFechaCreacion { get; set; }
        public int fiIDUsuarioCreacion { get; set; }
        public System.DateTime fdFechaUltimaModificacion { get; set; }
        public int fiIDUsuarioUltimaModificacion { get; set; }
        public string fcToken { get; set; }
        public decimal fnValorCuotaMensual { get; set; }
        public Nullable<decimal> fnValorCuotaCapital { get; set; }
        public Nullable<decimal> fnValorCuotaInteres { get; set; }
        public Nullable<decimal> fnTasaAnualPlana { get; set; }
        public Nullable<int> fiProductoEnPaquete { get; set; }
        public Nullable<System.DateTime> fdFechaUltimaActualizacion { get; set; }
        public int fiCantidad { get; set; }
        public Nullable<int> fiEstadoPrecioActual { get; set; }
        public int fiIdProductoPreciosDetalleActual { get; set; }
        public List<SelectListItem> ListPrecios { get; set; }
    }

    

    
}