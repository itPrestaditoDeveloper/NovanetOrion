using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Mantenimiento
{
    public class ListPlazoPorPaqueteViewModel
    {
        public int fiIDPaqueteMaestroPlazos { get; set; }
        public int fiIDPaquete { get; set; }
        public int fiIDPlazo { get; set; }
        public decimal fnValorPlazo { get; set; }
        public int fiEstadoPlazo { get; set; }
        public string fcToken { get; set; }
    }
}