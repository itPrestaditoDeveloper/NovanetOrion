using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Contratista
{
    public class SolicitudesDeTrabajo_PorIDClienteViewModel
    {
        public int fiIDContratistaSolicitud { get; set; }
        public int fiIDContratista { get; set; }
        public int fiIDSolicitud { get; set; }
        public int fiIDAgenciaInstalacion { get; set; }
        public int fiNoOrden { get; set; }
        public int fiIDUsuarioCreador { get; set; }
        public DateTime fdFechaInstalacion { get; set; }
        public int fiIDEstadoInstalacion { get; set; }
        public string fcDescripcion { get; set; }
        public int fiIDTecnicoAsignado { get; set; }
        public DateTime fdFechaInstalacionFinal { get; set; }
    }
}