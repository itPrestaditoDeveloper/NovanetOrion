using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Reportes
{
    public class ReporteHistorialToken
    {
        public DateTime fdFechaSolicitud { get; set; }
        public string fcDescripcionAplicacion { get; set; }
        public string fcToken { get; set; }
        public string fcEstadodelToken { get; set; } 
        public DateTime fdFechaAplicacion { get; set; }
        public string UsuarioAplicado { get; set; }
        public string UsuarioGenerado { get; set; } 

    }
}