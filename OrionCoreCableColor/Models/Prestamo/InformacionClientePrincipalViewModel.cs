using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Prestamo
{
    public class InformacionClientePrincipalViewModel
    {
        public string idPrestamo { get; set; }
        public string idSolicitud { get; set; }
        public string fcIDCliente { get; set; }
        public string fcCentrodeCosto { get; set; }
        public string fcIdentidad { get; set; }
        public string fcNombreSAF { get; set; }
        public string fcTelefonos { get; set; }
        public int fiIDGestorCallCenter { get; set; }
        public Nullable<System.DateTime> fdUltimaGestion { get; set; }
        public Nullable<byte> fiIDGestion { get; set; }
        public byte fiPagaconTDC { get; set; }
        public byte fiIDCarteraInicial { get; set; }
        public short fiInicialDiasAtraso { get; set; }
        public short fiInicialCuotasAtraso { get; set; }
        public short fiInicialCuotasPagadas { get; set; }
        public decimal fnInicialValorCuota { get; set; }
        public decimal fnInicialCapitalOtorgado { get; set; }
        public decimal fnInicialSaldoActual { get; set; }
        public decimal fnInicialCapitalVencido { get; set; }
        public decimal fnInicialInteresesVencidos { get; set; }
        public decimal fnInicialRecargos { get; set; }
        public decimal fnInicialInteresMora { get; set; }
        public decimal fnInicialSaldoPonerAlDia { get; set; }
        public decimal fnInicialTotalCuenta { get; set; }
        public string fcGestion { get; set; }
        public Nullable<decimal> fnPagarUSD { get; set; }
        public string fcAlerta1 { get; set; }
        public int fnMontoPromocional { get; set; }
        public int fiClienteenRuta { get; set; }
        public string fcDescripcionPerfil { get; set; }
        public string fcScriptTelefonico { get; set; }
        public string fcIconoCliente { get; set; }
        public Nullable<byte> fiTieneTrabajo { get; set; }
        public string fcRubro { get; set; }
        public string fcTipodeEmpleo { get; set; }
        public byte fiTieneSeguro { get; set; }
        public string fcImagenSeguro { get; set; }
        public int fiIDSolicitudActiva { get; set; }
        public string fcMensajeAlerta { get; set; }
        public string fcDepartamento { get; set; }
        public string fcMunicipio { get; set; }
        public string fcBarrio { get; set; }
        public string fcDireccionDetallada { get; set; }
        public Nullable<int> fiAntiguedadLaboral { get; set; }
        public Nullable<int> fiRangoSalarial { get; set; }
        public string fcDescripcion_RangosSalariales { get; set; }
        public string fcDescripcionAntiguedad { get; set; }
        public string fcLugarTrabajo { get; set; }
        public string fcIDPrestamo { get; set; }
        public string fcPrestamoSAF { get; set; }
        public string fcClienteSAF { get; set; }
        public string fcCorreo { get; set; }
        

    }
}