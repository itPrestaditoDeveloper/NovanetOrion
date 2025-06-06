using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Home
{
    public class ListUbicacionesPorUsuarioViewModel
    {
        public int fiIDUbicacion { get; set; }
        public string fcUbicacion { get; set; }
        public List<ListProductosPorUbicacionViewModel> Productos { get; set; }
        public bool fbUsuarioPrincipal { get; set; }
    }
}