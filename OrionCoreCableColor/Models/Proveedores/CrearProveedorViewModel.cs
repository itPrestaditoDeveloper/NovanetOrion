using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Proveedores
{
    public class CrearProveedorViewModel
    {
        public int fiIDProveedor { get; set; }

        //[Required]
        //[Display(Name ="Codigo")]
        //public int fiIDProveedor { get; set; }
        public int fiIDEmpresa { get; set; }
        [Display(Name = "Status")]
        public byte fiEstado { get; set; }
        [Required]
        [Display(Name = "Nombre Proveedor")]
        public string fcNombre { get; set; }
        [Display(Name = "Es persona juridica")]
        public bool TipoPersona { get; set; }
        [Display(Name = "Tipo de Persona")]
        [Required]
        public int fiTipoPersona { get; set; }
        [Required]
        [Display(Name = "RTN")]
        public string fcRTN { get; set; }
        [Display(Name = "CAI")]
        public string fcCAI { get; set; }
        [Display(Name = "Fecha Limite Emision Factura")]
        public Nullable<System.DateTime> fcFechaLimiteEmisionFactura { get; set; }
        [Display(Name = "Rango Inicial Factura")]
        public Nullable<int> fcRangoInicialFactura { get; set; }
        [Display(Name = "Rango Final")]
        public Nullable<int> fcRangoFinalFactura { get; set; }
        [Display(Name = "Prefijo Factura")]
        public string fcPrefijoFactura { get; set; }
        [Display(Name = "Correo")]
        public string fcCorreoElectronico { get; set; }
        [Display(Name = "Contacto")]
        public string fcContacto { get; set; }
        [Display(Name = "Telefono")]
        public string fcTelefono { get; set; }
        [Display(Name = "Celular")]
        public string fcTelefonoMovil { get; set; }
        [Display(Name = "Numero de fax")]
        public string fcTelefonoFax { get; set; }
        [Display(Name = "Pais")]
        public short fiCodPais { get; set; }
        [Display(Name = "Departamento")]
        public int fiCodDepartamento { get; set; }
        [Display(Name = "Ciudad")]
        public int fiCodCiudad { get; set; }
        [Display(Name = "Direccion")]
        public string fcDireccion1 { get; set; }



        [Display(Name = "Aldea/Poblado")]
        public int? fiCodAldeaPoblado { get; set; }

        [Display(Name = "Referencia de Direccion")]
        public string fcDIreccion2 { get; set; }
        public int fiUsuarioCreado { get; set; }
        public System.DateTime fdFechaCreado { get; set; }
        public Nullable<int> fiIDUsuarioModificador { get; set; }
        public Nullable<System.DateTime> fdFechaModificado { get; set; }
        public bool fbActivo { get; set; }
        public string fcComentario { get; set; }
        public bool EsEditar { get; set; }
        [Display(Name = "Tiempo de credito")]
        public Nullable<int> fiDiasLimiteCredito { get; set; }
        public string fcPais { get; set; }
        public string fcDepartamento { get; set; }
        public string fcMunicipio { get; set; }
        public string fcPoblado { get; set; }

    }
}