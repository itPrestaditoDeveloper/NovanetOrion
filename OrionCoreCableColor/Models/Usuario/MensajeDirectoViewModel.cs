using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Usuario
{
    public class MensajeDirectoViewModel
    {
        public int fiIDUsuarioMensajero { get; set; }
        public string fcUsuarioMensajero { get; set; }
        public int fiIDUsuarioReceptor { get; set; }
        public string fcUsuarioReceptor { get; set; }
        public string fcMensajeEnviado { get; set; }
        public string fcMensaje { get; set; }
    }
}