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
    
    public partial class sp_Inventario_Movimiento_Detalle_BuscarPorPrestamo_Result
    {
        public int fiIDInventarioMovimientoDetalle { get; set; }
        public int fiIDInventarioMovimientoMaestro { get; set; }
        public int fiIDMovimiento { get; set; }
        public Nullable<int> fiIDProducto { get; set; }
        public string fcProducto { get; set; }
        public int fiIDTipoMovimiento { get; set; }
        public Nullable<short> fiTipoAfectacion { get; set; }
        public string fcTipoAfectacion { get; set; }
        public int fiIDUbicacionInicial { get; set; }
        public string fcUbicacionInicial { get; set; }
        public int fiIDUbicacionDestino { get; set; }
        public string fcUbicacionDestino { get; set; }
        public string fcCodigoSerie1 { get; set; }
        public string fcCodigoSerie2 { get; set; }
        public decimal fnCantidad { get; set; }
        public string fcDescripcion { get; set; }
        public int fiIDUsuarioCreado { get; set; }
        public string fcUsuarioCreacion { get; set; }
        public System.DateTime fdFechaCreacion { get; set; }
        public string fcToken { get; set; }
        public string fcPrecios { get; set; }
        public string fcIDPrestamo { get; set; }
        public string fcIdentidadCliente { get; set; }
        public string fcPrimerNombreCliente { get; set; }
        public string fcSegundoNombreCliente { get; set; }
        public string fcPrimerApellidoCliente { get; set; }
        public string fcSegundoApellidoCliente { get; set; }
        public string fcTelefonoPrimarioCliente { get; set; }
        public string fcRTN { get; set; }
        public string fcTipoProducto { get; set; }
        public string fcModelo { get; set; }
        public string fcMarca { get; set; }
    }
}
