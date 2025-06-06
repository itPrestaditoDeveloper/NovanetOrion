using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Base
{
    public class ListONUViewModel
    {
        public int fiIDONU { get; set; }
        public int fiIDOrionSolicitud { get; set; }
        public int fiIDEquifax { get; set; }
        public string fcNombre { get; set; }
        public string fcMac { get; set; }
        public string fcIP { get; set; }
        public string fcNombreWifi { get; set; }
        public bool fbConectado { get; set; }
        public string fcIPOLT { get; set; }
        public string CodigoCepheus { get; set; }
        public string fcPon { get; set; }
        public DateTime fdUltimaActualizacion { get; set; }
        public bool fbCambioConexion { get; set; }
        public string fcEmpresa { get; set; }
        public int fiStatusPing { get; set; }
        public string fcUsuarioONU { get; set; }
        public string fcContrasenia { get; set; }
        public string fcURLLogin { get; set; }
        public string fcStatusPing { get; set; }
    }
}