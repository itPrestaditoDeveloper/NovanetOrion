using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Precalificado
{
    public class ListDistribuidorViewModel
    {
        public int fiIDDistribuidor { get; set; }
        [Display(Name = "Empresa")]
        public string fcNombreComercial { get; set; }
        [Required]
        [Display(Name = "RTN")]
        [StringLength(14, ErrorMessage = "El n�mero de rtn no puede exceder los 14 d�gitos.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El n�mero de rtn solo debe contener n�meros.")]
        public string fcRTN { get; set; }
        public string fcComentario { get; set; }
        [Display(Name = "Nombre Completo")]
        public string NombreRepresentante { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string fcPrimerNombre { get; set; }
        [Required]
        [Display(Name = "Segundo Nombre")]
        public string fcSegundoNombre { get; set; }
        [Required]
        [Display(Name = "Apelllido")]
        public string fcPrimerApellido { get; set; }

        [Required]
        [Display(Name = "Segundo Apellido")]
        public string fcSegundoApellido { get; set; }
        
        [Display(Name = "Identidad")]
        [Required(ErrorMessage = "El n�mero de identidad es obligatorio.")]
        [StringLength(13, ErrorMessage = "El n�mero de identidad no puede exceder los 13 d�gitos.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El n�mero de identidad solo debe contener n�meros.")]
        public string fcIdentidadRepresentante { get; set; }
        [Required]
        [Display(Name = "Direcci�n")]
        public string fcDireccion { get; set; }
       
        [Display(Name = "Tel�fono")]
        [Required(ErrorMessage = "El n�mero de tel�fono es obligatorio.")]      
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El n�mero de tel�fono debe tener exactamente 8 d�gitos.")]
        public string fcTelefono { get; set; }
      
        [Display(Name = "Tel�fono")]
        [Required(ErrorMessage = "El n�mero de tel�fono es obligatorio.")]  
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El n�mero de tel�fono debe tener exactamente 8 d�gitos.")]
        public string fcTelefonoMovil { get; set; }
        [Required]
        [Display(Name = "Correo")]
        public string fcCorreoElectronico { get; set; }
        public Nullable<int> fiIDUsuarioCreacion { get; set; }
        public Nullable<System.DateTime> fdFechaCreacion { get; set; }
        public Nullable<int> fiIDUsuarioUltimaModificacion { get; set; }
        public Nullable<System.DateTime> fdFechaUltimaModificacion { get; set; }
        public Nullable<int> fiIDEstado { get; set; }

        public bool EsEditar { get; set; }
        public string fcFirmaErick { get; set; }
        public int fiIDEstadoFirma { get; set; }
        public int fiIDFirmado { get; set; }

        public int fiIDDocumento { get; set; }
        public List<VendedorDocumentosViewModel> DatosDocumentoListado { get; set; }
        public List<ConfigDistribuidorVendedor> ConfigDistribuidorVendedor { get; set; }

        public string fcDocumentosJSON { get; set; }

    }


    public class ConfigDistribuidorVendedor
    {
        public int fiIDOrion_DocumentoVendedor { get; set; }
        public int fiIDOrion_DocumentoDistribuidor { get; set; }
        public int fiVendedor { get; set; }
        public int fiDistribuidor { get; set; }
        public int fiIDOrion_DocumentoIdentidadVendedor { get; set; }
        public int fiIDTipoVendedor { get; set; }
    }

}