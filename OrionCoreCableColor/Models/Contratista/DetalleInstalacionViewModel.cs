using Newtonsoft.Json;
using OrionCoreCableColor.Models.Solicitudes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Contratista
{
    public class DetalleInstalacionViewModel
    {
        public int fiIDContratistaSolicitud { get; set; }
        public int fiIDContratista { get; set; }
        public int fiIDSolicitud { get; set; }
        public int fiIDEquifax { get; set; }
        public string fcIdentidad { get; set; }
        [Display(Name = "Cliente")]
        public string fcNombre { get; set; }
        public string fcTelefono { get; set; }
        public string fcDireccionDetallada { get; set; }
        public int fiIDEstadoInstalacion { get; set; }
        public int fiIDTipoSolicitud { get; set; }
        public string fcDescripcion { get; set; }
        public string fcClase { get; set; }
        public string fcRGB { get; set; }
        public bool fbArticulosExtra { get; set; }
        [Display(Name = "Artículos")]
        public string fcArticulosdelContrato { get; set; }
        [Display(Name = "Viñeta Cliente")]
        public string fcNumeroVineta { get; set; }
        public string ListaMateriales { get; set; }
        public string ListadoFotos { get; set; }
        public string ListadoDocumentos { get; set; }
        public string ListadoBitacora { get; set; }
        public List<ListaMaterialesDetalleInstalacion> fjListaMateriales { 
            get => string.IsNullOrEmpty(ListaMateriales) ? null : JsonConvert.DeserializeObject<List<ListaMaterialesDetalleInstalacion>>(ListaMateriales);
        }
        public List<ListadoDeFoto> fjListadoFotos { 
            get => string.IsNullOrEmpty(ListadoFotos) ? null : JsonConvert.DeserializeObject<List<ListadoDeFoto>>(ListadoFotos);  
        }
        public List<ListadoDocumento> fjListadoDocumentos { 
            get => string.IsNullOrEmpty(ListadoDocumentos) ? null : JsonConvert.DeserializeObject<List<ListadoDocumento>>(ListadoDocumentos);  
        }
        public List<ListadoBitacora> fjListadoBitacora
        {
            get => string.IsNullOrEmpty(ListadoBitacora) ? null : JsonConvert.DeserializeObject<List<ListadoBitacora>>(ListadoBitacora);
        }
        public ListadoBitacora Bitacora { get; set; }
    }

    public class ListaMaterialesDetalleInstalacion
    {
        public int fiIDContratista_SolicitudInstalacion_Detalle { get; set; }
        public int fiIDProducto { get; set; }
        public string fcProducto { get; set; }
        public double fnCantidad { get; set; }
    }

    public class ListadoDeFoto
    {
        public int fiIDCatalogo_FotosInstalacion { get; set; }
        public string fcNombreFoto { get; set; }
        public string fcDescripcion { get; set; }
        public string fcUrlDocumento { get; set; }
    }
    public class ListadoDocumento
    {
        public int fiIDOrionSolicitud { get; set; }
        public string fcNombreArchivo { get; set; }
        public string fcRuta { get; set; }
        public string fcTipoDocumento { get; set; }
        public string fcComentario { get; set; }
        public int fiIDSolicitudFirma { get; set; }
        public int fiIDTipoArchivo { get; set; }
        public string fcDescripcion { get; set; }
        
    }
    public class ListadoBitacora
    {
        public int fiIDbitacoraSolicitudInstalacion { get; set; }
        public int fiIDSolicitudInstalacion { get; set; }
        [Display(Name = "Comentario")]
        public string fcComentario { get; set; }
        public int fiIdUsuarioCreacion { get; set; }
        public string fcUsuarioCreacion { get; set; }
        public DateTime fdFechaCreacion { get; set; }
        
    }


}