using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Base
{
    public class ListOLTViewModel
    {
        public int fiIDOLT { get; set; }
        public string fcIP { get; set; }
        public string fcNombre { get; set; }
        public List<ListONUViewModel> listaONUS { get; set; }
        public bool fbConectado { get; set; }
        public DateTime fdUltimaActualizacion { get; set; }
        public bool fbCambioConexion { get; set; }
        public int fiStatusPing { get; set; }
        public string fcStatusPing { get; set; }
    }
}