using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Tecnico
{
    public class InstalacionViewModel
    {
        public int FiIDContratistaSolicitud { get; set; }
        public int FiIDContratista { get; set; }
        public int FiIDSolicitud { get; set; }
        public int FiIDEquifax { get; set; }
        public string FcIdentidad { get; set; }
        public string FcNombre { get; set; }
        public string FcTelefono { get; set; }
        public string FcDireccionDetallada { get; set; }
        public int FiIDEstadoInstalacion { get; set; }
        public string FcDescripcion { get; set; }
        public string FcClase { get; set; }
        public string FcRGB { get; set; }
        public string FcArticulosdelContrato { get; set; }

        public string ListaMateriales { get; set; }
        public string ListadoFotos { get; set; }
        public List<Material> flListaMaterialesList
        {
            get => string.IsNullOrEmpty(ListaMateriales) ? null : JsonConvert.DeserializeObject<List<Material>>(ListaMateriales);
        }
        public List<Foto> flListadoFotosList 
        {
            get => string.IsNullOrEmpty(ListadoFotos) ? null : JsonConvert.DeserializeObject<List<Foto>>(ListadoFotos);
        }


        public string fcVinietaCliente { get; set; }
        public string fcComentarioInstalacion { get; set; }
        public bool fbArticulosExtra { get; set; } //ya estaria 
        public string DireccionDetalladaCliente { get; set; }//ya esta
        public string fcGeolocalizacion { get; set; }

    }
    public class Material
    {
        public int FiIDProducto { get; set; }
        public string FcProducto { get; set; }
        public int FiIDMarca { get; set; }
        public bool FbCodigo { get; set; }
        public decimal fnCantidad { get; set; }
        public string Series { get; set; }
        public List<SeriesMateriales> ListaSeries
        {
            get => string.IsNullOrEmpty(Series) ? null : JsonConvert.DeserializeObject<List<SeriesMateriales>>(Series);
        }
    }

    public class Foto
    {
        public int FiIDCatalogo_FotosInstalacion { get; set; }
        public string FcNombreFoto { get; set; }
        public string FcDescripcion { get; set; }
        public string base64 { get; set; }
        public string fcUrlDocumento { get; set; }
    }


    public class SeriesMateriales
    {
        public int FiProducto { get; set; }
        public int Serie { get; set; } //aqui es el id del movimiento pero se pone serie por que Isaac lo tiene como serie
    }
    public class RequesApiInstalacion
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public List<InstalacionViewModel> Data { get; set; }
    }
}