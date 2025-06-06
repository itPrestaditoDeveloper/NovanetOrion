using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Saris
{
    public class AreasViewModel
    {
        public int fiIDArea { get; set; }
        public string fcDescripcion { get; set; }
        public string fcCorreoElectronico { get; set; }
        public int fiIdUsuarioResponsable { get; set; }
        public string fcUsuarioResponsable { get; set; }
        public int fiIDGerencia { get; set; }
        public string fcNombreGerencia { get; set; }
        public int fiUsuarioCreacion { get; set; }
        public string fcUsuarioCreacion { get; set; }
        public DateTime fdFechacreacion { get; set; }
        public int fiUsuarioModificacion { get; set; }
        public string fcUsuarioModificacion { get; set; }
        public DateTime fdFechaModificacion { get; set; }
    }
}