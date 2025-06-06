using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Productos
{
    public class ListInventarioMovimientoDetalleViewModel
    {
        public int fiIDInventarioMovimientoDetalle { get; set; }
        public int fiIDInventarioMovimientoMaestro { get; set; }
        public int fiIDTipoMovimiento { get; set; }
        public int fiIDMovimiento { get; set; } //el id en el Productos_Movimientos_Detalle
        public int fiIDUbicacionInicial { get; set; }
        public int fiIDUbicacionDestino { get; set; }
        public int fiIdProducto { get; set; }
        public string fcProducto { get; set; }
        public string fcCodigoSerie1 { get; set; }
        public string fcCodigoSerie2 { get; set; }
        public decimal fnCantidad { get; set; }
        public string fcDescripcion { get; set; }
        public bool fbPorCodigo { get; set; }
        public bool fbEditado { get; set; }
        public string fcToken { get; set; }
        public int index { get; set; }
        public string fcUbicacionInicial { get; set; }
        public string fcUbicacionDestino { get; set; }
        public string fcPrecios { get; set; }
        public bool fbCheckeado { get; set; }
    }
}