using System.Collections.Generic;

namespace OrionCoreCableColor.Models.Precalificado
{
    public class MatrizSecundariaValidacionViewModel
    {
        public CondicionViewModel Condicion { get; set; }
        public List<DireccionesSolicitudesViewModel> Direcciones { get; set; }
    }


    public class CondicionViewModel
    {
        public int fiMesesCondicion { get; set; }
        public decimal fnMontoCondicion { get; set; }
    }


    public class DireccionesSolicitudesViewModel
    {
        public int fiIDSolicitud { get; set; }
        public int fiCodColonia { get; set; }
        public string fcBarrio { get; set; }
        public int fiCodDepartamento { get; set; }
        public string fcDepartamento { get; set; }
        public int fiCodMunicipio { get; set; }
        public string fcMunicipio { get; set; }
        public string fcDireccionDetallada { get; set; }
    }


    public class ExistenciaInventarioViewModel
    {
        public int FiIndex { get; set; } // Índice de la fila
        public string FcProducto { get; set; } // Nombre del producto
        public string Ubicacion { get; set; } // Nombre de la ubicación
        public decimal FnCantidad { get; set; } // Cantidad en esa ubicación
    }
}