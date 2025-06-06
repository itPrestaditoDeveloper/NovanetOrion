using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Mensajeria
{
    public class SendEmailViewModel
    {
        public string fcCodigo { get; set; }
        public List<string> fcDestinatarios { get; set; }
        public List<string> fcDestinatariosCC { get; set; }
        public List<ListAttachmentViewModel> fvArchivos { get; set; }
        public string fcEmailName { get; set; }
        public string fcAsunto { get; set; }
        public string fcCuerpoCorreo { get; set; }
    }
}