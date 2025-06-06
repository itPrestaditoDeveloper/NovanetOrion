using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Tecnico
{
    public class TerminarInstalacionYOrdenTrabajo
    {

        public int FiIDContratistaSolicitud { get; set; }
        public int FiIDContratista { get; set; }
        public int FiIDSolicitud { get; set; }
        public string FcTelefono { get; set; }
        public string FcDireccionDetallada { get; set; }
        public int FiIDEstadoInstalacion { get; set; }
        public string FcDescripcion { get; set; }
        public string FcClase { get; set; }
        public string FcRGB { get; set; }
        public string FcArticulosdelContrato { get; set; }

        public string ListaMateriales { get; set; }
        public string ListadoFotos { get; set; }

        public List<Materiales> flListaMaterialesList
        {
            get => string.IsNullOrEmpty(ListaMateriales) ? null : JsonConvert.DeserializeObject<List<Materiales>>(ListaMateriales);
        }
        public List<Imagenes> flListadoFotosList
        {
            get => string.IsNullOrEmpty(ListadoFotos) ? null : JsonConvert.DeserializeObject<List<Imagenes>>(ListadoFotos);
        }

        public string fcVinietaCliente { get; set; }
        public string fcComentarioInstalacion { get; set; }
    }


    public class Materiales
    {
        public int FiProducto { get; set; }
        public string FcProducto { get; set; }
        public decimal Cantidad { get; set; }
        public bool FbCodigo { get; set; }
        public string Series { get; set; }
        public List<SerialesdeMateriales> ListaSeries
        {
            get => string.IsNullOrEmpty(Series) ? null : JsonConvert.DeserializeObject<List<SerialesdeMateriales>>(Series);
        }
    }


    public class Imagenes
    {
        public int FiIDCatalogo_FotosInstalacion { get; set; }
        public string FcNombreFoto { get; set; }
        public string FcDescripcion { get; set; }
        public string base64 { get; set; }

    }

    public class ImagenesaEnviarProcedimiento
    {
        public int fiIDSolicitudContratista { get; set; }
        public int fiIDCatalogoFotoInstalacion { get; set; }
        public string fcUrlDocumento { get; set; }
        public string fcNombreDocumento { get; set; }
        public string fcExtension { get; set; }
    }

    public class SerialesdeMateriales
    {
        public int FiProducto { get; set; }
        public int Serie { get; set; } //aqui es el id del movimiento pero se pone serie por que Isaac lo tiene como serie
    }


}