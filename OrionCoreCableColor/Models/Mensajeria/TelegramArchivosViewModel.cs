using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Mensajeria
{
    public class TelegramArchivosViewModel
    {
        public HttpPostedFileBase fvArchivo { get; set; }
        public string fcCaption { get; set; }
        public string fcUrlArchivo { get; set; }
        public string fcNombreArchivo { get; set; }
    }
}