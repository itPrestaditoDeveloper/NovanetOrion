using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Contabilidad
{
    public class CrearDetallePartidaViewModel
    {
        public int fiIDEmpresa { get; set; }
        public string fcPartida { get; set; }
        public System.DateTime fdFechaPartida { get; set; }

        [Display(Name = "Cuenta Contable")]
        public string fcCuentaContable { get; set; }

        [Display(Name = "Tiene auxiliar")]
        public bool TieneAuxiliar { get; set; }

        [Display(Name = "Auxiliar")]
        public string fcAuxiliar { get; set; }

        [Display(Name = "Cod Auxiliar")]
        public string fcAuxiliarCodigo { get; set; }

        [Display(Name = "Debito")]
        public decimal fnDebito { get; set; }

        [Display(Name = "Credito")]
        public decimal fnCredito { get; set; }

        public string fcDebitooCredito { get; set; }

        public decimal fnTasadeCambio { get; set; }

        public string fcEstado { get; set; }

        public string fcTipoOperacion { get; set; }


        [Display(Name = "Fondo")]
        public string fcFondo { get; set; }


        [Display(Name = "Centro Costo")]
        public string fcCentrodeCosto { get; set; }


        [Display(Name = "Programa")]
        public string fcPrograma { get; set; }

        public string fcDocumento { get; set; }
        public string fcOrigen { get; set; }

        public System.DateTime fdFechaMigracion { get; set; }
        public System.DateTime fdFechaCreacionPartida { get; set; }


        [Display(Name = "Cuenta")]
        public string fcCuenta { get; set; }
        public string fcPartidaAnterior { get; set; }
        public byte fiPresu_VistoBueno { get; set; }
        public byte fiConvertida { get; set; }
        public int fiIDFila { get; set; }
        public string fcCierreSAF { get; set; }
        public string fcEmpresaSAF { get; set; }
        public System.DateTime fdUltimaActualizacion { get; set; }
        public string fcToken { get; set; }
        public string fcAutoGUID { get; set; }

        [Display(Name = "Referencia")]
        public string fcReferenciaDetalle { get; set; }
        public string fcSAFMigradoPor { get; set; }
        public string fcSAFOperador { get; set; }
        // public string fcPartidaSeguimiento { get; set; }
        public int fiIDUsuario { get; set; }

        public byte fiPartidaMayorizada { get; set; }
        public System.DateTime fdFechadeMayorizacion { get; set; }
        public int fiIDUsuarioMayorizacion { get; set; }
        public System.Guid IDGUID { get; set; }


        //Helpers
        public bool EsEditar { get; set; }
        public decimal TotalPartida { get; set; }
        public decimal TotalDebito { get; set; }
        public decimal TotalCredito { get; set; }

        public int IdMoneda { get; set; }
        public bool EsDolar { get; set; }

        public string DescripcionCuenta { get; set; }

        public int FilaBloqueada { get; set; }

        public bool EsCuentaDeBanco { get; set; }

        public bool RevisionRequerida { get; set; }
        public bool Exento { get; set; }

        public bool EsCuentaPorCobrar { get; set; }

        public int IdRegistroAuxiliar { get; set; }

        public string NombreCliente { get; set; }

        public string fcIDLoan { get; set; }
        [Display(Name = "Tipo Movimiento")]
        public string fcTipoMovimiento { get; set; }
        public int fiIdTipoMovimiento { get; set; }

        public List<CuentasContablesParaSelect> SelectCuentasContables = new List<CuentasContablesParaSelect>();

        public decimal fiCantidad { get; set; }
        public decimal fnPrecioUnitario { get; set; }
    }


    public class CuentasContablesParaSelect
    {
        public string id { get; set; }
        public string text { get; set; }
        public bool disabled { get; set; }
    }
}