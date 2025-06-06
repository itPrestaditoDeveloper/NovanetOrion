using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.App_Services.FileKMZService
{
    public class PlacemarkDataViewModel
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>(); // Propiedades adicionales
    }
}