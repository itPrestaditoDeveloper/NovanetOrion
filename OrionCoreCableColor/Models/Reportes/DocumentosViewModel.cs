using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Reportes
{
    public class DocumentosViewModel
    {
        public int IDSolicitud { get; set; }
        public int IDDocumento { get; set; }
        public int IDFirma { get; set; }
        public string Comentario { get; set; }
        public string NombreCliente { get; set; }
        public string fcComentario { get; set; }
        public HttpPostedFileBase pdf { get; set; }
        public string fcNombreArchivo { get; set; }
        public int fiIDRenovacion { get; set; }
    }
}