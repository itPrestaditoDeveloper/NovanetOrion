using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Contabilidad
{
    public class sp_CuentasContables_Listar_Model
    {
        public int fiIDEmpresa { get; set; }
        public string fcCuentaContable { get; set; }
        public byte fiRubro { get; set; }
        public string fcNivelC { get; set; }
        public byte fiBalanza { get; set; }
        public string fcAceptaPosteo { get; set; }
        public string fcDescripcionCuenta { get; set; }
        public string fcTipo { get; set; }
        public string fcCuentaContablePadre { get; set; }
        public string fcCuentaVieja { get; set; }
        public string fcCuentaPresupuestoP { get; set; }
        public string fcCuentaPresupuestoD { get; set; }
        public string fcCuentaPresupuestoC { get; set; }
        public byte fiTieneAuxiliar { get; set; }
        public string fcModuloAuxiliar { get; set; }
        public byte fiCuentaDebito { get; set; }
        public byte fiCuentaCredito { get; set; }
        public byte fiDolar { get; set; }
        public byte fiReferencia { get; set; }
        public byte fiLiquida { get; set; }
        public string fcMonedaEquivalenter { get; set; }
        public short fiMoneda { get; set; }
        public int fiNumero { get; set; }
        public int fiNumeroLimite { get; set; }
        public string fcCuentaBloqueada { get; set; }
        public string fcEmpresa { get; set; }
        public System.DateTime fdFechaCreado { get; set; }
        public string fcUltimaActualizacion { get; set; }
        public string fcToken { get; set; }
        public string fcCentrodeCosto { get; set; }
        public string fcCuentaEspejo { get; set; }
        public string fcCompaniaEspejo { get; set; }
        public byte fiEsEncaje { get; set; }
        public int fiIDUsuarioCreador { get; set; }
        public int fiIDCatalogoCuentaContable { get; set; }
    }
}