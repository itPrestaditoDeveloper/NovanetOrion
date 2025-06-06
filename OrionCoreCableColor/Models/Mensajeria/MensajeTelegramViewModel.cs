using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Mensajeria
{
    public class MensajeTelegramViewModel
    {
        public string fcCodigo { get; set; }
        public List<int> fiIDDestinatarios { get; set; }
        public string fcMensaje { get; set; }
        public List<HttpPostedFileBase> fvArchivos { get; set; }
    }
}