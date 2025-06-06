using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Soporte
{
    public class ListInventarioViewModel
    {
        public int fiIDProducto { get; set; }
        public string fcProducto { get; set; }
        public decimal fnCantidadSistema { get; set; }
        public decimal fnCantidadFisico { get; set; }
        public decimal fnDiferencia { get; set; }
        public string fcComentario { get; set; }
        public bool fbGenerico { get; set; }
        public List<ListSeriesViewModel> Series { get; set; }
    }
}