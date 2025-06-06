using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Soporte
{
    public class sp_ONUsYOLTs_Listar_Result
    {
        public int fiIDDetalleCliente_Servicio { get; set; }
        public string fcTipoArticulo { get; set; }
        public int fiIDOrionSolicitud { get; set; }
        public string fcSolicitudes { get; set; }
        public Nullable<int> fiIDEquifax { get; set; }
        public string fcIDClientes { get; set; }
        public string fcNombre { get; set; }
        public string fcMac { get; set; }
        public string fcIP { get; set; }
        public string fcNombreWifi { get; set; }
        public string fcPom { get; set; }
        public string fcCodigoCepheus { get; set; }
        public Nullable<bool> fbConectado { get; set; }
        public string fcGeolocalizacion { get; set; }
        public string fcIDPrestamo { get; set; }
        public string fcTelefono { get; set; }
        public decimal fnSaldoActualCapital { get; set; }
        public int fiEstadoServicio { get; set; }
        public decimal fnCapitalAtrasado { get; set; }
        public int fiDiasAtraso { get; set; }
        public DateTime fdFechaRegitro { get; set; }
        public string fcEmpresa { get; set; }
        public string fcUsuarioONU { get; set; }
        public string fcContrasenia { get; set; }
        public string fcURLLogin { get; set; }
        public string fcEstadoPing { get; set; }
    }
}