using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Mensajeria
{
    public class TelegramMensajeViewModel
    {
        public string fcCodigo { get; set; }
        public string fcTelefono { get; set; }
        public string fcMensaje { get; set; }
        public string fcChatID { get; set; }
        public List<TelegramArchivosViewModel> fvArchivos { get; set; }
    }
}