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
    
    public partial class sp_Clientes_Potenciales_Listado_ById_Result
    {
        public int fiIdClientesPotenciales { get; set; }
        public string fcNombreCliente { get; set; }
        public string fcNumeroTelefono { get; set; }
        public string fcDireccionCliente { get; set; }
        public string fcRazonEscribir { get; set; }
        public Nullable<int> fiIdFuenteCliente { get; set; }
        public string fcNombrefuenteOrigen { get; set; }
        public string fcUrlEnlace { get; set; }
        public Nullable<int> fiIdUsuarioGestion { get; set; }
        public string fcUsuarioGestion { get; set; }
        public Nullable<int> fiIdUsuarioCreacion { get; set; }
        public string fcUsuarioCreacion { get; set; }
        public Nullable<System.DateTime> fdFechaCreacion { get; set; }
        public Nullable<int> fiUsuarioModificacion { get; set; }
        public string fcUsuarioModificacion { get; set; }
        public Nullable<System.DateTime> fdFechaModificacion { get; set; }
        public Nullable<int> fiEstatus { get; set; }
        public string fcNombreEstatus { get; set; }
        public string fcClase { get; set; }
        public string fcRGB { get; set; }
        public string fcHexadecimal { get; set; }
        public Nullable<bool> fbEstadoActivo { get; set; }
        public Nullable<bool> fbPuedeVerlo { get; set; }
        public Nullable<bool> fbPuedeEliminarlo { get; set; }
    }
}
