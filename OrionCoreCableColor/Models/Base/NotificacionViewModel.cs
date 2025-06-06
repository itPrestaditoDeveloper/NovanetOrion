using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Base
{
    public class NotificacionViewModel
    {
        public int fiIDUsuario { get; set; }
        public string fcUsuario { get; set; }
        public DateTime fdFechaTransaccion { get; set; }
        public string fcOperacion { get; set; }
        public string fcTipoTransaccion { get; set; }
        public string fcClase { get; set; }
    }
}