using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Mensajeria
{
    public class ListChatBotDetalleViewModel
    {
        public int fiIDChatBot { get; set; }
        public int fiIDBot { get; set; }
        public string fcNombreChat { get; set; }
        public string fcNumeroTelefono { get; set; }
        public string fcChatID { get; set; }
        public DateTime fdFechaCreacion { get; set; }
        public int fiEstadoChat { get; set; }
        public int fiChatAceptado { get; set; }
    }
}