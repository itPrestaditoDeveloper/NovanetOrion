//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OrionCoreCableColor.DbConnection.OrionDB
{
    using System;
    
    public partial class sp_Producto_Precios_Detalle_Listar_Result
    {
        public int fiIdProductoPreciosDetalle { get; set; }
        public Nullable<int> fiIDProducto { get; set; }
        public Nullable<decimal> fnValorProductoMN { get; set; }
        public Nullable<decimal> fnValorProductoME { get; set; }
        public Nullable<decimal> fnPorcentajeImpuesto1 { get; set; }
        public Nullable<decimal> fnPorcentajeImpuesto2 { get; set; }
        public Nullable<decimal> fnValorCuotaMensual { get; set; }
        public Nullable<decimal> fnValorCuotaCapital { get; set; }
        public Nullable<decimal> fnValorCuotaInteres { get; set; }
        public Nullable<decimal> fnTasaAnualPlana { get; set; }
        public Nullable<System.DateTime> fdFechaCreacion { get; set; }
        public Nullable<int> fiIDUsuarioCreacion { get; set; }
        public Nullable<System.DateTime> fdFechaUltimaModificacion { get; set; }
        public Nullable<int> fiIDUsuarioUltimaModificacion { get; set; }
        public string fcToken { get; set; }
        public Nullable<bool> fbEstadoPrecioActual { get; set; }
        public Nullable<bool> fbEstadoPrecio { get; set; }
        public Nullable<decimal> fnValorCuotaMensual12Nuevo { get; set; }
        public Nullable<decimal> fnValorCuotaMensual24Nuevo { get; set; }
        public Nullable<decimal> fnValorCuotaMensual12Cliente { get; set; }
        public Nullable<decimal> fnValorCuotaMensual24Cliente { get; set; }
    }
}
