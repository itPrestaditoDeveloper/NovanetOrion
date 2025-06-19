using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.App_Services.PingService
{
    public class IntervaloConexionViewModel
    {
        public int fiIDOrionSolicitud { get; set; }
        public string fcTipoDispositivo { get; set; }
        public string fcNombreDispositivo { get; set; }
        public bool fbConectado { get; set; }
        public string fcMac { get; set; }
        public string fcCodigoCableColor { get; set; }
        public int fiIndex { get; set; }
        public bool fbEstado { get; set; }
        public DateTime fdTiempoInicial { get; set; }
        public DateTime fdTiempoFinal { get; set; }
        public TimeSpan fdIntervalo { get; set; }
        public string fcIntervalor { get; set; }
    }
}