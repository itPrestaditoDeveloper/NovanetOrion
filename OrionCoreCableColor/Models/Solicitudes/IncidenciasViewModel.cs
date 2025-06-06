using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Solicitudes
{
    public class IncidenciasViewModel
    {
        public int fiIDSolicitudInstalacion { get; set; }
        public int fiIDSolicitud { get; set; }
        public string fcNombreCliente { get; set; }
        public string fcTelefonoCliente { get; set; }
        public string fcPaquete { get; set; }
        public string fcNombreEmpresa { get; set; }
        public string fcNombreTecnico { get; set; }
        public int fiNoOrden { get; set; }
        public DateTime fdFechaInstalacion { get; set; }
        public string fcComentario { get; set; }
        public int fiIDContratista { get; set; }
        public string fcubicacion { get; set; }
        public string fcDescripcion { get; set; }
        public string fcClase { get; set; }
        public string fcCodigoCliente { get; set; }
        public string fcNumeroOrdenCfeus { get; set; }
        public string fcDepartamento { get; set; }
    }
}