using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models.DatosCliente;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Solicitudes
{
    public class SolicitudesViewModel
    {
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string TelefonoSecundario { get; set; }
        public int IDFirma { get; set; }
        public int fiIDSolicitudRenovacion { get; set; }
        public int Cuota { get; set; }
        public string Identidad { get; set; }
        public int IdCliente { get; set; }
        public int IDSolicitud { get; set; }
        [Display(Name = "Agencia")]
        public Nullable<int> fiIDAgencia { get; set; }
        public string fcNombreAgencia { get; set; }
        [Display(Name = "Contratista")]
        public Nullable<int> fiIDContratista { get; set; }
        public string fcDescripcionOLT { get; set; }
        [Display(Name = "OLT")]
        public int fiIDOLT { get; set; }
        [Display(Name = "Gestion")]
        public int fiIDGestion { get; set; }
        public string fcGestion { get; set; }
        public int fiIDRazon { get; set; }
        public string fcRazonDelcinar { get; set; }
        [Display(Name = "Gestion Anterior")]
        public int fiIDGestionAnterior { get; set; }
        public string fcGestionAnterior { get; set; }
        public int fiIDUsuario { get; set; }
        public string fcNombreEmpresa { get; set; }
        public string fcComentarioPagoInicial { get; set; }
        [Display(Name = "Correo")]
        public string fcCorreo { get; set; }
        public string ParametroEncriptado { get; set; }
        [Display(Name = "Gestion Actividad")]
        public string codigo { get; set; }
        // public  MyProperty { get; set; }

        // Datos Servicio de la Red
        public int fiIDOrionSolicitud { get; set; }
        [Display(Name = "MAC")]
        public string fcMac { get; set; }
        [Display(Name = "IP")]
        public string fcIP { get; set; }
        [Display(Name = "EPOM")]
        public string fcPom { get; set; }
        [Display(Name = "Nombre de la red(Wifi)")]
        public string fcNombreWifi { get; set; }
        public int fiUsuarioCreacion { get; set; }
        public System.DateTime fdFechaCreacion { get; set; }
        public System.DateTime FechaMaxima { get; set; }

        public System.DateTime fdfechaMaximaPago { get; set; }

        public Nullable<int> fiUsuarioModificacion { get; set; }
        public System.DateTime fdFechaModificacion { get; set; }
        public string CodigoCliente { get; set; }
        public string NumeroOrdenCepheus { get; set; }
        public string NumeroOrdenTrabajoCepheus { get; set; }
        public System.DateTime fdFechaInstalacionAsignada { get; set; }
        public string fcComentarioOrdenIsntalacion { get; set; }
        public string fcDescripcion { get; set; }
        public int fiIDEstadoInstalacion { get; set; }
        public bool fbEditar { get; set; }
        [Display(Name = "Cuota en Lempiras")]
        public decimal CuotaEnLempiras { get; set; }
        [Display(Name = "Origen Cuenta")]
        public string fcCuentaContable { get; set; }
        public string fcDescripcionCuenta { get; set; }
        public int IDToken { get; set; }
        /*public int fiRespuesta { get; set; }*/
        [Display(Name = "Token")]
        public string fcMensaje { get; set; }       
        public string NombreOficial { get; set; }
        public string CodigoSeccionToken { get; set; }
        public int fiIDSolicitudInstalacion { get; set; }
        public decimal fnCuotaMensualMonedaNacional { get; set; }
        public decimal fnCuotaMensualMonedaNacionalCondicion { get; set; }
        public int fiIdOperacionSolicitud { get; set; }
        [Display(Name = "Solicitud Anterior")]
        public int fiSolicitudAnterior { get; set; }
        [Display(Name = "Paquete Anterior")]
        public string fcArticulosdelContratoAnterior { get; set; }
        [DisplayName("Articulos Actual")]
        public string fcArticulosdelContrato { get; set; }
        public int fiMesesCondicion { get; set; }
        [Display(Name = "Codigo")]
        public string fcCodigoVendedor { get; set; }
        public int fiPlazoSeleccionado { get; set; }
        public string FicoLink { get; set; }
        public string fcCodigoUnico { get; set; }
        [Display(Name = "Meta")]
        public decimal fnMetaComision { get; set; }
        [Display(Name = "Arpu")]
        public decimal fnArpuMeta { get; set; }

        public List<SolicitudesReferenciaViewModel> ListaReferencia { get; set; }
        public List<SolicitudesGarantiaViewModel> ListaGarantia { get; set; }
        public List<SolicitudesInventarioPorUbicacionViewModel> ListaInventarioPorUbicacion { get; set; } 
        public sp_OrionSolicitud_Detalle_ClienteListarViewModel DatosCLientes { get; set; }
        public sp_OrionSolicitud_InformacionDocumentacion_ViewModel DatosContrato { get; set; }
        public sp_OrionSolicitud_Documento_ViewModel DatosDocumento { get; set; }
        public List<sp_OrionSolicitud_DocumentoListado_ViewModel> DatosDocumentoListado { get; set; }
        public List<sp_OrionListaBitacoras_Clientes_ViewModel> BitacorasClienteSoporte { get; set; }        
        public List<sp_OrionListaBitacoras_Clientes_ViewModel> BitacorasClienteMantoSolicitudes { get; set; }
        public List<sp_OrionSolicitud_Tiempo_SolicitudViewModel> ListaProcesoSolicitud { get; set; }
        public List<ProductoListarViewModel> DatosProductos { get; set; }
        public List<SolicitudesCliente_ViewModel> ListaSolicitudCliente { get; set; }

        //public string fcMensaje { get; set; }
       

    }
    public class SolicitudesGarantiaViewModel
    {
        public int fiIDEquifaxGarantia { get; set; }
        public int fiIDEquifax { get; set; }
        public string fcIDentidad { get; set; }
        public int fiIDSolicitud { get; set; }
        public string fcProducto { get; set; }
        public int fiIDProducto { get; set; }
        public decimal fnValorProductoME { get; set; }
        public decimal fnValorProductoMN { get; set; }
        public decimal fnPorcentajeImpuesto1 { get; set; }
        public decimal fnPorcentajeImpuesto2 { get; set; }
        public int fiIDPaquete { get; set; }

    }
    public class SolicitudesInventarioPorUbicacionViewModel
    {
        
        public int fiIDEquifax { get; set; }        
        public int fiIDSolicitud { get; set; }
        public int fiIDProducto { get; set; }
        public string fcProducto { get; set; }
        public string fcTipoProducto { get; set; }
        public string fcMarca { get; set; }
        public string fcModelo { get; set; }
        public string fcNumerodeSerie1 { get; set; }
        public string fcNumerodeSerie2 { get; set; }      
        public decimal fnCantidad { get; set; }
        public int fiIDPaquete { get; set; }

    }
    public class SolicitudesReferenciaViewModel
    {
        public int fiIDReferencia { get; set; }
        public int fiIDEquifax { get; set; }
        public string NombreRefrencia { get; set; }
        public string fcTelefonoReferencia { get; set; }
        public string fcDescripcionParentesco { get; set; }

    }

    public class sp_OrionSolicitud_Detalle_ClienteListarViewModel
    {
        
        public int fiIDSolicitud { get; set; }
        public int IdcontratistaSolicitud { get; set; }
        public int fiIDCliente { get; set; }
        public Nullable<int> fiPlazoSeleccionado { get; set; }
        public int fiEstadoSolicitud { get; set; }
        public Nullable<decimal> fnValorGarantia { get; set; }
        public Nullable<decimal> fnMontoFinalFinanciar { get; set; }
        public string fcComentarioResolucion { get; set; }
        public Nullable<System.DateTime> fdFechaCreacionSolicitud { get; set; }
        public Nullable<System.DateTime> fdFinFirma { get; set; }
        public Nullable<decimal> fnTasaAnual { get; set; }
        [DisplayName("Cuota Mensual")]
        public Nullable<decimal> fnCuotaMensual { get; set; }
        public Nullable<decimal> fnTasaMensual { get; set; }
        public Nullable<int> fiIDMoneda { get; set; }
        public Nullable<int> fiIDFondo { get; set; }
        public Nullable<int> fiIDEstadoPago { get; set; }
        public Nullable<decimal> fnTotalValorContrato { get; set; }
        [DisplayName("Lugar Trabajo")]
        public string fcLugarTrabajo { get; set; }
        public Nullable<int> fiEstadoCivil { get; set; }
        [DisplayName("Estado Civil")]
        public string fcDescripcionEstadoCivil { get; set; }
        public Nullable<bool> fbTieneHijos { get; set; }
        [DisplayName("Fecha Nacimiento")]
        public Nullable<System.DateTime> fdFechaNacimiento { get; set; }
        public string fcCiudad { get; set; }
        [DisplayName("Direccion detallada")]
        public string fcDireccionDetallada { get; set; }
        [DisplayName("Correo")]
        public string fcCorreo { get; set; }
        public string fcGeolocalizacion { get; set; }
        [DisplayName("Departamento")]
        public string fcDepartamento { get; set; }
        [DisplayName("Municipio")]
        public string fcMunicipio { get; set; }
        public string fcPoblado { get; set; }
        [DisplayName("Barrio")]
        public string fcBarrio { get; set; }
        [DisplayName("Identidad")]
        public string fcIdentidad { get; set; }
        //public int fiTipoVivienda { get; set; }
        [DisplayName("Tipo Vivienda")]
        public string fcDescripcionTipoVivienda { get; set; }
        [DisplayName("Origen")]
        public string fcOrigen { get; set; }
        [DisplayName("Codigo Vendedor")]
        public string fcCodigoVendedor { get; set; }
        [DisplayName("Tipo Cliente")]
        public int fiIDTipoCliente { get; set; }
        [DisplayName("Nombre Empresa Juridica")]
        public string fcNombreEmpresaJuridica { get; set; }
        [DisplayName("Rtn Empresa Juridica")]
        public string fcRtnEmpresaJuridico { get; set; }

        public string fcNombre { get; set; }
        public string fcEstadoSolicitud { get; set; }
        public int fiTipoSolicitud { get; set; }
        public string fcTelefono { get; set; }
        public string fcTelefonoSecundario { get; set; }

        [DisplayName("Dia de pago")]
        public int fiDIa { get; set; }

        [Required]
        [Display(Name = "Seleccione departamento donde reside")]
        public int fiDepartamento { get; set; }
        [Required]
        [Display(Name = "Seleccione municipio")]
        public int fiMunicipio { get; set; }
        [Required]
        [Display(Name = "Seleccione poblado")]
        public int fiPoblado { get; set; }
        [Required]
        [Display(Name = "Seleccione barrio/colonia")]
        public int fiColonia { get; set; }
        [Display(Name = "Valor moneda Nacional")]
        public decimal fnCuotaMensualMonedaNacional { get; set; }
        [Display(Name = "Numero Orden Cepheus")]
        public string fcNumeroOrdenCfeus { get; set; }
        [Display(Name = "Codigo Cliente Cepheus")]
        public string fcCodigoCliente { get; set; }
        public decimal fnValorPagadoPrimerPago { get; set; }
        public decimal fnValorPagadoPrimerPagoLempiras { get; set; }

        [Display(Name = "Orden de trabajo")]
        public string fcNumeroOrdenTrabajoChepeus { get; set; }
        [Display(Name = "Mes Promocion")]
        public string fcMesGratis { get; set; }
        
        public int fiIDPaquete { get; set; }
        public string fcPaquete { get; set; }
        public int fiIDEstadoInstalacion { get; set; }
    }

    public class sp_OrionSolicitud_Documento_ViewModel
    {
        
        public int fiIDOrionSolicitud { get; set; }
        public int fiIDOrion_Documento { get; set; }
        public int fiIDSolicitud { get; set; }
        public int fiIDCliente { get; set; }
        public string fcNombreArchivo { get; set; }
        public string fcExtension { get; set; }
        public string fcRutaArchivo { get; set; }
        public string fcURL { get; set; }
        public int fiIDDocumento { get; set; }
        public string fcComentario { get; set; }
        public int fiArchivoActivo { get; set; }
        public HttpPostedFileBase ImgenProducto { get; set; }

    }
    public class sp_OrionSolicitud_DocumentoListado_ViewModel
    {
        public int IdcontratistaSolicitud { get; set; }
        public int fiIDOrion_Documento { get; set; }
        public int fiIDOrionSolicitud { get; set; }        
        public string fcNombreArchivo { get; set; }   
        public string fcRuta { get; set; }
        public string fcTipoDocumento { get; set; }
        public string fcComentario { get; set; }
        public int fiIDSolicitudFirma { get; set; }
        public int fiIDTipoArchivo { get; set; }
        public string fcDescripcion{ get; set; }
        public bool fbDocumentoExiste { get; set; }

    }

    public class sp_OrionSolicitud_InformacionDocumentacion_ViewModel
    {
        [DisplayName("Nombre")]
        public string fcNombre { get; set; }
        [DisplayName("Identidad")]
        public string fcIdentidad { get; set; }
        [DisplayName("Telefono")]
        public string fcTelefono { get; set; }
        [DisplayName("Lugar del Trabajo")]
        public string fcLugarTrabajo { get; set; }
        [DisplayName("Direccion Detallada")]
        public string fcDireccionDetallada { get; set; }
        [DisplayName("Estado Civil")]
        public string fcDescripcionEstadoCivil { get; set; }
        [DisplayName("Articulos")]
        public string fcArticulosdelContrato { get; set; }
        [DisplayName("Producto(TV o PS5)")]
        public string fcDescripcionTelevisor { get; set; }
        [DisplayName("Plazo Seleccionado")]
        public int fiPlazoSeleccionado { get; set; }
        [DisplayName("Valor Garantia")]
        public decimal fnValorGarantia { get; set; }
        [DisplayName("Monto Final a Financiar")]
        public decimal fnMontoFinalFinanciar { get; set; }
        [DisplayName("Cuota Mensual")]
        public decimal fnCuotaMensual { get; set; }
        [DisplayName("Tasa Anual")]
        public decimal fnTasaAnual { get; set; }
        [DisplayName("Correo")]
        public string fcCorreo { get; set; }
        [DisplayName("Valor Contrato")]
        public decimal fnTotalValorContrato { get; set; }
        [DisplayName("Fecha Inicio Credito")]
        public DateTime fdFechaIniciodeCredito { get; set; }
        public int fiIDPaquete { get; set; }
        public string fcPaquete { get; set; }

        public string fcMensaje { get; set; }
    }

    public  class sp_OrionListaBitacoras_Clientes_ViewModel
    {
        public string fcIDCliente { get; set; }
        public int fiIDUsuario { get; set; }
        public string fcBitacora { get; set; }
        public byte fiIDGestion { get; set; }
        public System.DateTime fdGestion { get; set; }
        public string fcTelefono { get; set; }
        public Nullable<System.DateTime> fdFechaVolveraLlamar { get; set; }
        public byte fiOrigenGestion { get; set; }
        public byte fiUbicacionGestion { get; set; }
        public string fcLatitud { get; set; }
        public string fcLongitud { get; set; }
        public byte fiTelefonoContesta { get; set; }
        public Nullable<byte> fiSMSEnviado { get; set; }
        public Nullable<byte> fiWhatsAppEnviado { get; set; }
        public int fiIDSolicitud { get; set; }
        public string fcGestion { get; set; }
        public string fcPrimerNombre { get; set; }
        public string fcNombreCorto { get; set; }
        public string UrlImagen { get; set; }
        public bool fiPaymentConfirm { get; set; }


    }
    public class sp_OrionSolicitud_Tiempo_SolicitudViewModel
    {
        public Nullable<System.DateTime> fdFechaInicioFirma { get; set; }
        public Nullable<System.DateTime> fdFechaFinFirma { get; set; }
        public Nullable<System.DateTime> fdInicioSolicitud { get; set; }
        public Nullable<System.DateTime> fdFechaAprovacion { get; set; }
        public Nullable<System.DateTime> fdFechaCreacionInstalacion { get; set; }
        public Nullable<System.DateTime> fdfechaInicioInstalacion { get; set; }
        public Nullable<System.DateTime> fdFechaFinalInstalacion { get; set; }
        public int TiempoDiaCreacionSolicitud { get; set; }
        public Nullable<int> HoraTiempoCreacionSolicitud { get; set; }
        public Nullable<int> MinutosCreacionSolicitud { get; set; }
        public int TiempoDiaFirmaSolicitud { get; set; }
        public Nullable<int> HoraTiempoFirmaSolicitud { get; set; }
        public Nullable<int> MinutosFirmaSolicitud { get; set; }
        public int TiempoDiaInstalacionSolicitud { get; set; }
        public Nullable<int> HoraTiempoInstalacionSolicitud { get; set; }
        public Nullable<int> MinutosInstalacionSolicitud { get; set; }

    }
    public class SolicitudesCliente_ViewModel
    {
        public System.DateTime fdFechaCreacionSolicitud { get; set; }
        public int fiIDSolicitud { get; set; }
        public Nullable<decimal> fnCuotaMensual { get; set; }
        public string NombreOficial { get; set; }
        public string fcDescripcion { get; set; }
        public System.DateTime fdFechaInstalacionFinal { get; set; }

    }
}