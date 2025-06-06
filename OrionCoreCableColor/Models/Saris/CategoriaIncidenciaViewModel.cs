using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Saris
{
    public class CategoriaIncidenciaViewModel
    {
        public int fiIdTicket_Categoria { get; set; }
        public string fcDescripcionCategoria { get; set; }
        public bool fbEstado { get; set; }
        public int fiIdUsuarioCreacion { get; set; }
        public string fcUsuarioCreacion { get; set; }
        public DateTime fdFechacreacion { get; set; }
        public int fiIdUsuarioModificacion { get; set; }
        public string fcUsuarioModificacion { get; set; }
        public DateTime fdFechaModificacion { get; set; }
    }
}