using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Precalificado
{
    public class ListInventarioPorCliente
    {
        public int fiIDEquifax { get; set; }
        public string fcNombre { get; set; }
        public List<ListProductosPorSolicitud> Productos { get; set; }
    }
}