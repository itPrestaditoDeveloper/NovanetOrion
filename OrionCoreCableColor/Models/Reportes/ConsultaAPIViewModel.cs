using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Reportes
{
    public class ConsultaAPIViewModel
    {
        public int fiIDSolicitudAPI { get; set; }
        public int fiIDSolicitud { get; set; }
        public string fcIdentidad { get; set; }
        public string fcNombres { get; set; }
        public string fcApellidos { get; set; }
        public string fcNombre { get; set; }
        public string fcRtnEmpresaJuridico { get; set; }
        public string fcDireccionDetallada { get; set; }
        public string fcTelefono { get; set; }
        public Nullable<System.DateTime> fdFechadeNacimiento { get; set; }
        public string fcMensaje { get; set; }
    }
}