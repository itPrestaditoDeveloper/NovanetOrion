using System;

namespace OrionCoreCableColor.Models.Prestamo
{
    public class PrestamosIdentidadViewModel
    {
        public string fcIDCLiente { get; set; }
        public int fiActivo { get; set; }
        public string fcPrestamo { get; set; }
        public string fcProducto { get; set; }
        public string fcNombreAgencia { get; set; }
        public string fcGestor { get; set; }
        public string fcOficial { get; set; }
        public int fiDiasAtraso { get; set; }
        public Decimal fnCapital { get; set; }
        public Decimal fnValorCuota { get; set; }
        public short fiIDProducto { get; set; }
        public string fcFrecuencia { get; set; }
        public string fcTipoDePlazo { get; set; }
        public string fcNombreOficial { get; set; }
        public string fcAgenteCallCenter { get; set; }
        public Decimal tasaInteres { get; set; }
        public DateTime fdFechaDesembolso { get; set; }
        public string fcClaseEstadoInventario { get; set; }
        public string fcEstadoInventario { get; set; }
        public string fcClaseEstadoSeguro { get; set; }
        public string fcEstadoSeguro { get; set; }
        public Decimal fnSaldoActual { get; set; }
        public int fiIDSolicitud { get; set; }
        public Decimal fnValorAPrestar { get; set; }
        public string fcEmpresaPrestamo { get; set; }
        public int fiCantidadCuotasEnAtraso { get; set; }

    }


    public class DetalleInstalacionesCliente
    {
        public int fiIDContratistaSolicitud { get; set; }
        public int fiIDContratista { get; set; }
        public int fiIDSolicitud { get; set; }
        public int fiIDAgenciaInstalacion { get; set; }
        public int fiNoOrden { get; set; }
        public int fiIDUsuarioCreador { get; set; }
        public DateTime fdFechaInstalacion { get; set; }
        public int fiIDEstadoInstalacion { get; set; }
        public string fcDescripcion { get; set; }
        public int fiIDTecnicoAsignado { get; set; }
        public DateTime fdFechaInstalacionFinal { get; set; }
        public string fcCodigoCliente { get; set; }
        public string ComentarioAsesor { get; set; }
        public string fcNumeroOrdenCfeus { get; set; }
        public string fcComentarioInstalacion { get; set; }
        public int fiIDTipoSolicitud { get; set; }
        public string fcTipoSolicitudTrabajo { get; set; }
        public string fcNombre { get; set; }
        public string fcNombreCorto { get; set; }
        public string fcNombreCorto2 { get; set; }
        public string fcNombreEmpresa { get; set; }
        public string DireccionDetalladaCliente { get; set; }

    }
}