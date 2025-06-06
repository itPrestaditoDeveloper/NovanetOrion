using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Tecnico
{
    public class ListadoTecnicoViewModel
    {
        public int fiIDSolicitudInstalacion { get; set; }
        public int fiIDSolicitud { get; set; }
        public string fcNombreCliente { get; set; }
        public string fcTelefonoCliente { get; set; }
        public string fcPaquete { get; set; }
        public string fcNombreEmpresa { get; set; }
        public int fiNoOrden { get; set; }
        public DateTime fdFechaInstalacion { get; set; }
        public string fcComentario { get; set; }
        public string fcubicacion { get; set; }
        public string fcDescripcion { get; set; }
        public string fcClase { get; set; }
        public int fiQRAcceso { get; set; }
        public int fiIDTecnicoAsignado { get; set; }
        public int fiIDEstadoInstalacion { get; set; }
        public string fcCodigoCliente { get; set; }
        public string fcNumeroOrdenCfeus { get; set; }
        public int fiIDTipoSolicitud { get; set; }
        public bool fbPermisoInstalacion { get; set; }
        public bool fbArticulosExtra { get; set; }
        public bool fbPermisoValidarSoporte { get; set; }
        public string fcDepartamento { get; set; }
        public string fcDireccionDetallada { get; set; }
        public string fcArticulosdelContrato { get; set; }
      
    }

    public class RequesApiListaTecnico
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<ListadoTecnicoViewModel> Data { get; set; }
    }
}