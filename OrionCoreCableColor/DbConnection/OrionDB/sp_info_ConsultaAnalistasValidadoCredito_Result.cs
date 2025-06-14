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
    
    public partial class sp_info_ConsultaAnalistasValidadoCredito_Result
    {
        public int fiIDEquifax { get; set; }
        public string fcIdentidad { get; set; }
        public string fcNombre { get; set; }
        public byte fiOcupacion { get; set; }
        public string fcOcupacion { get; set; }
        public string fcTelefono { get; set; }
        public decimal fnCuotaBanca { get; set; }
        public decimal fnCuotaComercio { get; set; }
        public decimal fnCuotaTarjeta { get; set; }
        public Nullable<decimal> fnTotalObligaciones { get; set; }
        public decimal fnIngresos { get; set; }
        public Nullable<decimal> fnCapacidadDisponible { get; set; }
        public decimal fnSaldoBanca { get; set; }
        public decimal fnSaldoComercio { get; set; }
        public decimal fnSaldoTarjeta { get; set; }
        public Nullable<decimal> fnTotalSaldos { get; set; }
        public byte fiReglaCalculoCapacidad { get; set; }
        public byte fiResultadoPrecalificado { get; set; }
        public string fcMensajePrecalificado { get; set; }
        public Nullable<byte> fiEstadoActualPrecalificado { get; set; }
        public string fcMensaje { get; set; }
        public byte fiIHSS { get; set; }
        public byte fiCNE { get; set; }
        public byte fiCallCenter { get; set; }
        public short fiScorePromedio { get; set; }
        public short fiScoreActual { get; set; }
        public int fiScoreBajo { get; set; }
        public byte fiMoraComunicaciones { get; set; }
        public byte fiMoraMayor { get; set; }
        public byte fiMoraMenor { get; set; }
        public byte fiMoraHistorica { get; set; }
        public byte fiSobregiro { get; set; }
        public byte fiIncobrable { get; set; }
        public byte fiIrrecuperable { get; set; }
        public byte fiJuridicoLegal { get; set; }
        public byte fiSaldosCastigados { get; set; }
        public System.DateTime fdFechaHoraOperacion { get; set; }
        public string fcCentrodeCosto { get; set; }
        public string fcNombreAgencia { get; set; }
        public string fcOficialAgencia { get; set; }
        public System.DateTime fdFechaPrimerConsulta { get; set; }
        public System.DateTime fdFechaUltimaActualizacionEquifax { get; set; }
        public int fiIDUsuarioCreditos { get; set; }
        public string fcNombreUsuarioCreditos { get; set; }
        public byte fiResolucionCreditos { get; set; }
        public string fcResultadoPrecalificado { get; set; }
        public string fcObservacionesCreditos { get; set; }
        public Nullable<System.DateTime> fdAntiguedadBuroEquifax { get; set; }
        public Nullable<decimal> fiEndeudamiento { get; set; }
        public string fcDescripcionCapacidadePago { get; set; }
        public int fiIDProducto { get; set; }
        public string fcProducto { get; set; }
        public byte fiTipodeCliente { get; set; }
        public string fcOrigenPrimario { get; set; }
        public string fcOrigenSecundario { get; set; }
        public string fcReferenciaOrigen { get; set; }
        public byte fiListaNegra { get; set; }
        public string fcRespuestaCredito { get; set; }
        public string fcPrimerNombre { get; set; }
        public string fcSegundoNombre { get; set; }
        public string fcPrimerApellido { get; set; }
        public string fcSegundoApellido { get; set; }
        public System.DateTime fdFechadeNacimiento { get; set; }
    }
}
