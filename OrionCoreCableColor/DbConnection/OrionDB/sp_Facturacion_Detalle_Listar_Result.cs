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
    
    public partial class sp_Facturacion_Detalle_Listar_Result
    {
        public int fiIDFacturacionDetalle { get; set; }
        public Nullable<int> fiIDFacturacionMaestro { get; set; }
        public Nullable<byte> fiIDTipoProducto { get; set; }
        public string fcTipoProducto { get; set; }
        public string fcMarca { get; set; }
        public string fcModelo { get; set; }
        public Nullable<int> fiIDProducto { get; set; }
        public string fcProducto { get; set; }
        public Nullable<decimal> fnCantidad { get; set; }
        public Nullable<decimal> fnValorVentaDeContado { get; set; }
        public Nullable<decimal> fnPorcentajeImpuesto1 { get; set; }
        public Nullable<decimal> fnPorcentajeImpuesto2 { get; set; }
        public Nullable<decimal> fnValorProductoME { get; set; }
        public Nullable<decimal> fnTotal { get; set; }
        public Nullable<int> fiIDUsuarioCreacion { get; set; }
        public string fcUsuarioCreacion { get; set; }
        public Nullable<System.DateTime> fdFechaCreacion { get; set; }
        public Nullable<int> fiIDUsuarioUltimaModificacion { get; set; }
        public string fcUsuarioUltimaModificacion { get; set; }
        public Nullable<System.DateTime> fdFechaUltimaModificacion { get; set; }
        public string fcToken { get; set; }
        public Nullable<int> fiIDInventarioMovimientoDetalle { get; set; }
        public Nullable<bool> fbAplicaImpuesto { get; set; }
        public Nullable<decimal> fnValorImpuesto { get; set; }
    }
}
