using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.BaseCallCenter
{
    public class FuenteClientePotencialesViewModel
    {

        public int fiIdFuenteCliente { get; set; }
        public string fcNombrefuenteOrigen { get; set; }
        public string fcDescripcionFuenteOrigen { get; set; }
        public int fiIdUsuarioCreacion { get; set; }
        public string fcUsuarioCreacion { get; set; }
        public DateTime fdFechaCreacion { get; set; }
        public int fiUsuarioModificacion { get; set; }
        public string fcUsuarioModificacion { get; set; }
        public DateTime fdFechaModificacion { get; set; }
        public bool fiEstadoActivo { get; set; }
        public int fiLiugarDondeCrea { get; set; }

    }
}