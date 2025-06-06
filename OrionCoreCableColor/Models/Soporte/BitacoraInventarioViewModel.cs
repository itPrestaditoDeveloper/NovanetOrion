using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Soporte
{
    public class BitacoraInventarioViewModel
    {
        public int fiIDUbicacion { get; set; }
        public string fcObservacion { get; set; }
        public List<ListInventarioViewModel> Existencia { get; set; }
    }
}