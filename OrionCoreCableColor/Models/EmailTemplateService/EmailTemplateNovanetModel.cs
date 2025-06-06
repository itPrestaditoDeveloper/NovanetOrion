using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.EmailTemplateService
{
    public class EmailTemplateNovanetModel
    {
        public string fcUsuario { get; set; }
        public DateTime fdFechaCreacion { get; set; } 
        public string fcNombreCorto { get; set; }
        public int fiIDEstadoRequerimiento { get; set; }
        public string fcCorreoElectronico { get; set; }
        public string fcTelefonoMovil { get; set; }
        public string fcNuevaContrasenia { get; set; }
    }
}