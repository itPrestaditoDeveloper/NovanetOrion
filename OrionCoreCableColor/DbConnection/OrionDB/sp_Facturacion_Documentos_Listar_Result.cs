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
    
    public partial class sp_Facturacion_Documentos_Listar_Result
    {
        public int fiIDFacturacionDocumento { get; set; }
        public Nullable<int> fiIDFacturacionMaestro { get; set; }
        public string fcNombreArchivo { get; set; }
        public string fcExtension { get; set; }
        public string fcRuta { get; set; }
        public Nullable<int> fiIDUsuarioCreacion { get; set; }
        public Nullable<System.DateTime> fdFechaCreacion { get; set; }
        public Nullable<int> fiIDUsuarioUltimaModificacion { get; set; }
        public Nullable<System.DateTime> fdFechaUltimaModificacion { get; set; }
        public Nullable<int> fiEstadoFacturacion { get; set; }
        public string fcToken { get; set; }
    }
}
