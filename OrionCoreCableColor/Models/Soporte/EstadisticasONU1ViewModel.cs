using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Soporte
{
    public class EstadisticasONU1ViewModel
    {
        // WAN Interface Status
        public string WANName { get; set; }
        public string WANMAC { get; set; }
        public string Mode { get; set; }
        public string IPAddress { get; set; }
        public string Netmask { get; set; }
        public string Gateway { get; set; }
        public string DNSServer { get; set; }

        // WLAN Status (2.4G)
        public string Mode24G { get; set; }
        public string Band24G { get; set; }
        public string SSID24G { get; set; }
        public string ChannelNumber24G { get; set; }
        public string Encryption24G { get; set; }
        public string MAC24G { get; set; }
        public int AssociatedClients24G { get; set; }

        // WLAN Status (5G)
        public string Mode5G { get; set; }
        public string Band5G { get; set; }
        public string SSID5G { get; set; }
        public string ChannelNumber5G { get; set; }
        public string Encryption5G { get; set; }
        public string MAC5G { get; set; }
        public int AssociatedClients5G { get; set; }

        // System
        public string SystemMACAddress { get; set; }
        public string CWMPProductClass { get; set; }
        public string FirmwareVersion { get; set; }
        public string HardwareVersion { get; set; }
        public string CPUUsage { get; set; }
        public string MemoryUsage { get; set; }
        public string Uptime { get; set; }

        // PON Status
        public string Temperature { get; set; }
        public string Voltage { get; set; }
        public string TxPower { get; set; }
        public string RxPower { get; set; }
        public string BiasCurrent { get; set; }

        // EPON LLID Status
        public string EPONLLIDIndex { get; set; }
        public string RegisterStatus { get; set; }

        // CATV Status
        public string CATVReceivePower { get; set; }
        public string CATVRFLevel { get; set; }
        public string CATVControlPower { get; set; }
        public string CATVSystemChannel { get; set; }

        // CATV Configuration
        public string CATVEnable { get; set; }

        // UPnP Configuration
        public string UPnP { get; set; }

        //MENSAJE
        public bool Estado { get; set; }
        public string Mensaje { get; set; }
        public int fiIDUsuario { get; set; }
    }
}