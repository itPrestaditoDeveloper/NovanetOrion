using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.App_Services.PingService
{
    public class DispositivoViewModel
    {
        public string fcTipoDispositivo { get; set; }
        public string fcIPOLT { get; set; }
        public string fcIP { get; set; }
        public int fiIDOrionSolicitud { get; set; }
        public string fcPon { get; set; }
        public string fcWifi { get; set; }
        public string fcNombreDispositivo { get; set; }
        public bool fbConectado { get; set; }
        public string fcMac { get; set; }
        public string fcCodigoCableColor { get; set; }
        public bool fbEstadoServicio { get; set; }
    }
}