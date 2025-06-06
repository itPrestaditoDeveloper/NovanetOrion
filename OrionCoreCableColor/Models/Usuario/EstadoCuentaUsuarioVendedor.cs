using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Usuario
{
    public class EstadoCuentaUsuarioVendedor
    {
       // public DateTime fdFechaMovimiento { get; set; }
       // public string fcTipoMovimiento  { get; set; }
        //public string fcReferencia { get; set; }
        //public decimal fnValorMovimiento { get; set; }
        public int fiIDSolicitud { get; set; }
        public string fcNombre { get; set; }
        public string fcIdentidad { get; set; }
        public Nullable<System.DateTime> fdFechaCreacionSolicitud { get; set; }
        public System.DateTime fdFechaInstalacionFinal { get; set; }
        public string NombreOficial { get; set; }
        public string fcArticulosdelContrato { get; set; }
        public string fcInternet { get; set; }
        public string fcProductosDeContratista { get; set; }
        public decimal fnCuotaMensual { get; set; }
        public decimal fnCuotaMensualMonedaNacional { get; set; }
        public decimal fnComision { get; set; }
        public int fiIDMoneda { get; set; }
        public string fcMoneda { get; set; }
        public string DepartamentoVenta { get; set; }
        public string fcPaquete { get; set; }
        public string fcOperacionTipoSolicitud { get; set; }
        public List<DetalleEstadoCuentaUsuarioVendedor> DetalleCuentaVendedor { get; set; }

    }

    public class DetalleEstadoCuentaUsuarioVendedor
    {
        public DateTime fdFechaMovimiento { get; set; }
        public string fcTipoMovimiento { get; set; }
        public string fcReferencia { get; set; }
        public decimal fnValorMovimiento { get; set; }

    }
}