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
    
    public partial class sp_InfoClientePorIdEquifax_Result
    {
        public int fiIDEquifax { get; set; }
        public string fcIdentidad { get; set; }
        public string fcNombre { get; set; }
        public Nullable<System.DateTime> fdFechaNacimiento { get; set; }
        public byte fiTieneHistorialEquifax { get; set; }
        public byte fiCargaInicial { get; set; }
        public short fiScorePromedio { get; set; }
        public short fiScoreActual { get; set; }
        public System.DateTime fdFechaPrimerConsulta { get; set; }
        public System.DateTime fdFechaUltimaActualizacionEquifax { get; set; }
        public short fiIDApp { get; set; }
        public string fcCentrodeCostoConsultante { get; set; }
        public int fiIDUsuario { get; set; }
        public string fcCentrodeCostoAsignado { get; set; }
        public int fiIDUsuarioAsignado { get; set; }
        public System.DateTime fdFechaUltimaConsulta { get; set; }
        public int fiIDUsuarioUltimaConsulta { get; set; }
        public decimal fnIngresos { get; set; }
        public byte fiOcupacion { get; set; }
        public string fcTelefono { get; set; }
        public short fcCiudadResidencia { get; set; }
        public Nullable<System.DateTime> fdAntiguedadBuroEquifax { get; set; }
        public Nullable<System.DateTime> fdAntiguedadBuroPrestadito { get; set; }
        public string fcArchivoXML { get; set; }
        public decimal fnCuotaBanca { get; set; }
        public decimal fnCuotaComercio { get; set; }
        public decimal fnCuotaTarjeta { get; set; }
        public decimal fnSaldoBanca { get; set; }
        public decimal fnSaldoComercio { get; set; }
        public decimal fnSaldoTarjeta { get; set; }
        public byte fiReglaCalculoCapacidad { get; set; }
        public byte fiResultadoPrecalificado { get; set; }
        public byte fiResultadoDetallePrecalificado { get; set; }
        public byte fiResolucionCreditos { get; set; }
        public Nullable<int> fiIDUsuarioCreditos { get; set; }
        public Nullable<System.DateTime> fdResolucionCreditos { get; set; }
        public string fcObservacionesCreditos { get; set; }
        public Nullable<byte> fiEstadoActualPrecalificado { get; set; }
        public int fiIDProducto { get; set; }
        public decimal fnMontoSeleccionado { get; set; }
        public short fiPlazoSeleccionado { get; set; }
        public Nullable<decimal> fnMontoAprobado { get; set; }
        public Nullable<short> fiPlazoAprobado { get; set; }
        public Nullable<byte> fiResolucionGeneral { get; set; }
        public byte fiClienteCC { get; set; }
        public byte fiIHSS { get; set; }
        public byte fiCNE { get; set; }
        public byte fiCallCenter { get; set; }
        public byte fiMoraComunicaciones { get; set; }
        public byte fiMoraMayor { get; set; }
        public byte fiMoraMenor { get; set; }
        public byte fiMoraHistorica { get; set; }
        public byte fiSobregiro { get; set; }
        public byte fiIncobrable { get; set; }
        public byte fiIrrecuperable { get; set; }
        public byte fiJuridicoLegal { get; set; }
        public byte fiSaldosCastigados { get; set; }
    }
}
