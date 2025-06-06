using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.BaseCallCenter
{
    public class ClientesPotencialesViewModel
    {
        public int fiIdClientesPotenciales { get; set; }
        public string fcNombreCliente { get; set; }
        public string fcNumeroTelefono { get; set; }
        public string fcDireccionCliente { get; set; }
        public string fcRazonEscribir { get; set; }
        public int fiIdFuenteCliente { get; set; }
        public string fcUrlEnlace { get; set; }
        public int fiIdUsuarioGestion { get; set; }
        public int fiIdUsuario { get; set; }
        public DateTime fdFecha { get; set; }
        public int fiEstatus { get; set; }
        public bool fbEstadoActivo { get; set; }
        public string fcomentario { get; set; }

    }
}