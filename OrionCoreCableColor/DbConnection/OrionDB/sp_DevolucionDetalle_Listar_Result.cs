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
    
    public partial class sp_DevolucionDetalle_Listar_Result
    {
        public int fiIDDevolucionDetalle { get; set; }
        public int fiIDDevolucionMaestro { get; set; }
        public Nullable<int> fiIDProducto { get; set; }
        public string fcProducto { get; set; }
        public string fcCodigoSerie1 { get; set; }
        public string fcCodigoSerie2 { get; set; }
        public decimal fnCantidad { get; set; }
        public string fcDescripcion { get; set; }
        public int fiIDUsuarioCreado { get; set; }
        public System.DateTime fdFechaCreacion { get; set; }
        public string fcToken { get; set; }
        public Nullable<int> fiIDInventarioMovimientoDetalle { get; set; }
    }
}
