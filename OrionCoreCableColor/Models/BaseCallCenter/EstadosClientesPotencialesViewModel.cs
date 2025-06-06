using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.BaseCallCenter
{
    public class EstadosClientesPotencialesViewModel
    {
        public int fiIdEstatusCliente { get; set; }
        public string fcNombreEstatus { get; set; }
        public string fcdescripcionEstatus { get; set; }
        public int fcClase { get; set; }
        public string fcRGB { get; set; }
        public string fcHexadecimal { get; set; }
        public bool fbEstadoActivo { get; set; }
        public int fiIdUsuarioCreacion { get; set; }
        public string fcUsuarioCreacion { get; set; }
        public DateTime fdFechaCreacion { get; set; }
        public int fiIdUsuarioModificacion { get; set; }
        public string fcUsuarioModificacion { get; set; }
        public DateTime fdFechaModificacion { get; set; }
        
	}
}