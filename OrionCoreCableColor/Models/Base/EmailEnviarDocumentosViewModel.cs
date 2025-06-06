using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Base
{
    public class EmailEnviarDocumentosViewModel
    {
        public int fiIdSolicitud { get; set; }
        public int fiIdContratistaSolicitud { get; set; }
        public string fcEmailDestino { get; set; }
        public List<string> fcEmailcopiacorreo { get; set; }
        public string fcNombreMostrarArchivo { get; set; }
        public string fcNombreARchivoAEditar { get; set; }
        public string fcRutaGuardarARchivo { get; set; }
        public int fiIdEmailTemplate { get; set; }
        public int fiIdCliente { get; set; }

    }
}