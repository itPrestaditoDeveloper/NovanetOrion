using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Contratista
{
    public class detallesolicitudInstalacioViewModel
    {
        public int fiIDSolicitudContratista { get; set; }
        public int fiIDTecnicoAsignado { get; set; }
        [Display(Name = "Articulos de la instalacion")]
        public string fcArticulosAsignados { get; set; }
        [Display(Name = "Nombre Cliente")]
        public string fcCliente { get; set; }
        [Display(Name = "Estado Actual de instalacion")]
        public string fcDescripcionEstado { get; set; }
        public string fcClase { get; set; }
        public int fiIDEstadoInstalacion { get; set; }
        [Display(Name = "Metros de fibra utilizados")]
        public decimal fnMetrosFibra { get; set; }
        public int AccesoQR { get; set; }
        public List<lista_fotosInstalacion> listadoFotos { get; set; }

        [Display(Name = "Comentario de instalación")]
        public string fcComentarioInstalacion { get; set; }
    }
    public class lista_fotosInstalacion
    {
        //public int fiIDSolicitudInstalacion { get; set; }
        //public int fiIDCatalogoFotoInstalacion { get; set; }
        //public string fcUrlDocumento { get; set; }

        //public string fcNombreDocumento { get; set; }
        //public string fcExtension { get; set; }
        //public int MyProperty { get; set; }
        public int fiIDSolicitudInstalacion { get; set; }
        public Nullable<int> fiIDContratistaSolicitud { get; set; }
        public Nullable<int> fiIDCatalogoFotoInstalacion { get; set; }
        public string fcUrlDocumento { get; set; }
        public string fcNombreDocumento { get; set; }
        public string fcExtension { get; set; }
        public string DescripcionDocumento { get; set; }


    }
    public class BitacorasInstalacion
    {
        public List<listaBitacora> ListadoBitacoras { get; set; }
        public string nombreCliente { get; set; }
        public int fiIDSolicitudInstalacion { get; set; }
    }
    public class listaBitacora
    {
        public int fiIDSolicitudInstalacion { get; set; }
        public string usuario { get; set; }
        public DateTime fechaCreacion { get; set; }
        public string comentario { get; set; }

    }
}