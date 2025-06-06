using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.App_Services.PingService
{
    public class RespuestaApiCepheusViewModel
    {
        public int codigo { get; set; }
        public string mensaje { get; set; }
        public bool estadoServicio { get; set; }
    }
}