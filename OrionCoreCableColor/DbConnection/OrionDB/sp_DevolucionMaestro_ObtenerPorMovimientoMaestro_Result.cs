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
    
    public partial class sp_DevolucionMaestro_ObtenerPorMovimientoMaestro_Result
    {
        public int fiIDDevolucionMaestro { get; set; }
        public int fiIDUbicacionInicial { get; set; }
        public int fiIDUbicacionDestino { get; set; }
        public int fiIDUsuarioCreado { get; set; }
        public System.DateTime fdFechaCreacion { get; set; }
        public int fiIDUsuarioUltimaModificacion { get; set; }
        public System.DateTime fdFechaUltimaModificacion { get; set; }
        public int fiFirma { get; set; }
        public string fcDocumentoFirma { get; set; }
        public int fiEstadoInventairoMovimientoMaestro { get; set; }
        public string fcToken { get; set; }
        public string fcMotivoDevolucion { get; set; }
        public Nullable<int> fiIDInventarioMovimientoMaestro { get; set; }
    }
}
