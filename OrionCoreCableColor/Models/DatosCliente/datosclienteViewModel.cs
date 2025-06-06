using OrionCoreCableColor.Models.Precalificado;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.DatosCliente
{
    public class datosclienteViewModel
    {
        public int fiIDVendedorRepartidor { get; set; }
        public bool fbEditar { get; set; }
        public int fiIDEnvioFormulario { get; set; }
        public int fiIDEquifax { get; set; }
        [Required(ErrorMessage = "El número de identidad es obligatorio.")]
        [StringLength(13, ErrorMessage = "El número de identidad no puede exceder los 13 dígitos.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "El número de identidad solo debe contener números.")]
        [Display(Name = "Número de identidad")]
        public string fcIdentidad { get; set; }
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [Display(Name = "Nombre completo")]
        public string fcNombreCompleto { get; set; }

        [Required(ErrorMessage = "El Primer nombre es obligatorio.")]
        [Display(Name = "Primer nombre")]
        public string fcPrimerNombre { get; set; }
       
        [Display(Name = "Segundo nombre")]
        public string fcSegundoNombre { get; set; }

        [Required(ErrorMessage = "El Primer apellido es obligatorio.")]
        [Display(Name = "Primer apellido")]
        public string fcPrimerApellido { get; set; }
       
        [Display(Name = "Segundo apellido")]
        public string fcSegundoApellido { get; set; }

        [Required(ErrorMessage = "La ocupación es obligatoria.")]
        [Display(Name = "Ocupación")]
        public int fiIDOcupacion { get; set; }

        [Display(Name = "Ocupación")]
        public string fcOcupacion { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [Display(Name = "Dirección Exacta")]
        public string fcDireccion { get; set; }

        public string fcLongitud { get; set; }
        public string fcLatitud { get; set; }


        [Required(ErrorMessage = "El departamento es obligatorio.")]
        [Display(Name = "Seleccione departamento donde reside")]
        public int fiDepartamento { get; set; }

        [Required(ErrorMessage = "El municipio es obligatorio.")]
        [Display(Name = "Seleccione municipio")]
        public int fiMunicipio { get; set; }
        public string fcMunicipio { get; set; }

        [Required]
        [Display(Name = "Seleccione poblado")]
        public int fiPoblado { get; set; }
        public string fcPoblado { get; set; }

        [Required(ErrorMessage = "La colonia es obligatoria.")]
        [Display(Name = "Seleccione barrio/colonia")]
        public int fiColonia { get; set; }

        [Display(Name = "Lugar de trabajo")]
        public string fcLugarTrabajo { get; set; }
        [Display(Name = "Seleccione dia de pago")]
        public int fcDiaPago { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [Display(Name = "Fecha nacimiento Día/Mes/Año")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime fdFechaNacimiento { get; set; }


        //helper para dividir fecha de nacimiento en dia mes año
        public int diaNacimiento { get; set; }
        public int MesNacimiento { get; set; }
        public int AnioNacimento { get; set; }

       
        [Required]
        [Display(Name = "Seleccione Rango salarial")]
        public int fiRangoSalarial { get; set; }
        [Required]
        [Display(Name = "Seleccione Antiguedad laboral")]
        public int fiAntiguedadLaboral { get; set; }

        [Required]
        [Display(Name = "Nombre Referencia #1")]
        public string fcNombreReferencia1 { get; set; }
        [Required]
        [Display(Name = "Apellido")]
        public string fcApellidoReferencia1 { get; set; }
        [Required]
        [Display(Name = "No. telefono")]
        public string fcTelefonoReferencia1 { get; set; }
        [Required]
        [Display(Name = "Seleccione parentesco")]
        public int fcParentescoReferencia1 { get; set; }
        [Required]
        [Display(Name = "Nombre Referencia #2")]
        public string fcNombreReferencia2 { get; set; }
        [Required]
        [Display(Name = "Apellido")]
        public string fcApellidoReferencia2 { get; set; }
        [Required]
        [Display(Name = "No. telefono")]
        public string fcTelefonoReferencia2 { get; set; }
        [Required]
        [Display(Name = "Seleccione parentesco")]
        public int fcParentescoReferencia2 { get; set; }
        [Required]
        [Display(Name = "Referencia #3")]
        public string fcNombreReferencia3 { get; set; }
        [Required]
        [Display(Name = "Apellido")]
        public string fcApellidoReferencia3 { get; set; }
        [Required]
        [Display(Name = "No. telefono")]
        public string fcTelefonoReferencia3 { get; set; }
        [Required]
        [Display(Name = "Seleccione parentesco")]
        public int fcParentescoReferencia3 { get; set; }

        [Required]
        [Display(Name = " Estado civil")]
        public int fiEstadoCivil { get; set; }
        [Required]
        [Display(Name = "Tiene hijos?")]
        public bool fbTieneHijos { get; set; }
        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        [Display(Name = "Ciudad")]
        public string fcCiudad { get; set; }
        [Required]
        [Display(Name = "Dirección detallada.")]
        public string fcDireccionDetallada { get; set; }
        [Required]
        [Display(Name = "Profesión u oficio")]
        public string fcProfesion { get; set; }
        [Required]
        [Display(Name = "Tipo de vivienda")]
        public int fcTipoVivienda { get; set; }
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [Display(Name = "Correo electronico")]
        public string fcCorreo { get; set; }
        [Required]
        [Display(Name = "Ubicacion Maps")]
        public string fcGeolocalizacion { get; set; }

        public string fcColonia { get; set; }
        //helper
        public int tieneHijosHelper { get; set; }
        public List<ProductoListarViewModel> DatosProductos { get; set; }
        public List<VendedorDocumentosViewModel> DatosDocumentoListado { get; set; }
        public int fiTipoSolicitud { get; set; }

        [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
        [Display(Name = "Teléfono")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El número de teléfono debe tener exactamente 8 dígitos.")]
        public string fcTelefono { get; set; }

        [Display(Name = "Telefono Secundario")]
        public string fcTelefonoSecundario { get; set; }


        public int fiCodDepartamento { get; set; }
        public int fiCodMunicipio { get; set; }
        public int fiCodColonia { get; set; }


        public string SoliticutDeServicio { get; set; }
        public string fcDepartamento { get; set; }
    }
    public class ListParentesco
    {
        public int fiIDParentesco { get; set; }
        public string fcDescripcionParentesco { get; set; }
    }
    public class estadoCivilViewModel
    {
        public int id { get; set; }
        public string descripcion { get; set; }
    }
    public class tieneHijosViewModel
    {
        public int id { get; set; }
        public string descripcion { get; set; }
    }
    public class tipoViviendaViewModel
    {
        public int id { get; set; }
        public string descripcion { get; set; }
    }
}