using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.DatosCliente
{
    public class fotografiaCliente
    {
        public int IdSolicitud { get; set; }
        public HttpPostedFileBase Archivo { get; set; }
        public  int IdDocumento { get; set; }


    }
}