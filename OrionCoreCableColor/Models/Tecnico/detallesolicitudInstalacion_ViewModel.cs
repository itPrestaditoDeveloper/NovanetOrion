using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Tecnico
{
    public class detallesolicitudInstalacion_ViewModel
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

        public string fcComentarioInstalacion { get; set; }
        public string fcVinietaCliente { get; set; }
        public List<lista_detallesolicitudInstalacion_ViewModel> listadoFotos { get; set; }

        public List<Listado_materiales_viewModel> listadoMateriales { get; set; }


    }

    public class lista_detallesolicitudInstalacion_ViewModel
    {
        //public int fiIDSolicitudInstalacion { get; set; }
        //public int fiIDCatalogoFotoInstalacion { get; set; }
        //public string fcUrlDocumento { get; set; }

        //public string fcNombreDocumento { get; set; }
        //public string fcExtension { get; set; }
        //public int MyProperty { get; set; }
        public int fiIDCatalogo_FotosInstalacion { get; set; }
        public string fcNombreFoto { get; set; }
        public string fcDescripcion { get; set; }

        public HttpPostedFileBase Archivo { get; set; }


    }

    public class Listado_materiales_viewModel
    {
        public int fiIDMarca { get; set; }
        public int fiIDProducto { get; set; }
        public string  fcProducto { get; set; }
        public int fnCantidad { get; set; }
        public bool fbCodigo { get; set; }

        public List<Listado_Serie_Producto> CodigoBarra { get; set; }
    }

    public class Listado_Serie_Producto
    {
        public int fiIDProducto { get; set; }
        public int fiIDMovimiento { get; set; }
        public string serie { get; set; }
    }
}