using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Contratista
{
    public class contratistaAsignacionViewModel
    {
        public int fiIDSolicitudInstalacion { get; set; }
        public int fiIDTecnico { get; set; }
        public int fiIDContratista { get; set; }
        public string cliente  { get; set; }
        public string ubicacion { get; set; }
    }
}