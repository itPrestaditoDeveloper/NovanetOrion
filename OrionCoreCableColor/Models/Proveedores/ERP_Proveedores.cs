using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Proveedores
{
    public class ERP_Proveedores
    {
        public int fiIDEmpresa { get; set; }
        public int fiIDProveedor { get; set; }
        public string fcNombre { get; set; }
        public byte fiTipoPersona { get; set; }
        public string fcRTN { get; set; }
        public string fcContacto { get; set; }
        public byte fiEstado { get; set; }
        public short fiCodPais { get; set; }
        public int fiCodDepartamento { get; set; }
        public int fiCodCiudad { get; set; }
        public int fiCodAldeaPoblado { get; set; }
        public string fcDireccion1 { get; set; }
        public string fcDIreccion2 { get; set; }
        public string fcTelefono { get; set; }
        public string fcTelefonoMovil { get; set; }
        public string fcTelefonoFax { get; set; }
        public string fcCorreoElectronico { get; set; }
        public string fcCreadoPor { get; set; }
        public System.DateTime fdFechaCreado { get; set; }
        public string fcUltimaActualizacion { get; set; }
        public int fiIDCuentaPorPagar { get; set; }
        public System.DateTime fdFechaIngreso { get; set; }
        public int fiUsuarioCreador { get; set; }
        public string fcCAI { get; set; }
        public Nullable<System.DateTime> fcFechaLimiteEmisionFactura { get; set; }
        public Nullable<int> fcRangoInicialFactura { get; set; }
        public Nullable<int> fcRangoFinalFactura { get; set; }
        public string fcPrefijoFactura { get; set; }
        public string fcComentario { get; set; }
        public Nullable<int> fiDiasLimiteCredito { get; set; }
    }
}