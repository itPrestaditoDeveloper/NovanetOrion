using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Mensajeria
{
    public class ListAttachmentViewModel
    {
        public HttpPostedFileBase fvArchivo { get; set; }
        public string fcNombreArchivo { get; set; }
        ///por si esta en un servidor publico el archivo
        public string fcUrlArchivo { get; set; }
    }
}