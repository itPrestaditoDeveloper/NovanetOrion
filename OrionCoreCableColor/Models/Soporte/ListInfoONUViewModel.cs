using OrionCoreCableColor.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Soporte
{
    public class ListInfoONUViewModel : ListONUViewModel
    {
        public string fcPrestamo { get; set; }
        public string fcTelefono { get; set; }
        public string fcGeolocalizacion { get; set; }
        public bool fbCancelado { get; set; }
        public bool? fbEstado { get; set; }
        public string fcPon { get; set; }
        public string fcCodigoCepheus { get; set; }
    }
}