using System.Collections.Generic;

namespace OrionCoreCableColor.Models.Reportes
{
    public class ComparativaInstaladoViewModel
    {
        public int fiIDContratistaSolicitud { get; set; }
        public string fcNombre { get; set; }
        public string fcIdentidad { get; set; }
        public List<ProductosComparativa> jsonArticulosEntregados { get; set; }
        public List<ProductosComparativa> jsonArticulosPaquete { get; set; }

    }

    public class ProductosComparativa
    {
        public int fiIDProducto { get; set; }
        public string fcProducto { get; set; }
        public decimal fnCantidad { get; set; }
    }

    public class ProductoConfirmacionDTO
    {
        public int fiProducto { get; set; }
        public decimal fnCantidad { get; set; }
        public string fcSerie { get; set; }
        public string fcMac { get; set; }
    }

    public class ConfirmacionClienteRequest
    {
        public int solicitudId { get; set; }
        public List<ProductoConfirmacionDTO> productos { get; set; }
    }
}