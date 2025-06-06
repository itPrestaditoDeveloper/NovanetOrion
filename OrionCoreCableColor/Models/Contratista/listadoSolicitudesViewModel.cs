using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Contratista
{
    public class listadoSolicitudesViewModel
    {
        public int fiIDContratistaSolicitud { get; set; }
        public string fcNombreCliente { get; set; }
        public string fcTelefono { get; set; }
        public string fcArticulosdelContrato { get; set; }
        public string fcUbicacion { get; set; }
        public string  fcEstado { get; set; }
        public int AccesoQR { get; set; }
    }
}