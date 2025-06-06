using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Soporte
{
    public class ObservacionAuditoriaViewModel
    {
        public int fiIDSolicitud { get; set; }
        public string fcNombre { get; set; }
        public int fiIDProducto { get; set; }
        [DisplayName("Cantidad")]
        public decimal fnCantidad { get; set; }
        [DisplayName("Observacion")]
        public string fcObservacion { get; set; }
        [DisplayName("Audio")]
        public HttpPostedFileBase Documento { get; set; }
        public HttpPostedFileBase Fotografia { get; set; }

        public string fcGeolocalizacionInicio { get; set; }
        public string fcGeolocalizacionFinal { get; set; }
    }
}