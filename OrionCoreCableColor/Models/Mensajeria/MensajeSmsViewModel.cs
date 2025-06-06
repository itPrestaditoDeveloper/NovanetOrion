using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Mensajeria
{
    public class MensajeSmsViewModel
    {
        public string fcCodigo { get; set; }
        public List<string> fcTelefono { get; set; }
        public string fcMensaje { get; set; }
    }
}