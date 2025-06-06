using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Productos
{
    public class DetallePaqueteViewModel
    {
        public int fiIDPaquete { get; set; }
        public string fcPaquete { get; set; }
        public decimal fnValorMensual { get; set; }
        public decimal fnTasadeInteresAnual { get; set; }
        public int fiIDUsuarioCreador { get; set; }
        public string fcUsuarioCreacion { get; set; }
        public System.DateTime fdFechaCreacion { get; set; }
        public int fiIDUsuarioUltimaModificacion { get; set; }
        public string fcUsuarioModificacion { get; set; }
        public System.DateTime fdFechaUltimaModificacion { get; set; }
        public string fcToken { get; set; }
        public string fcProductos { get; set; }
        public bool Seleccionado { get; set; }
        public decimal fnValorCuotaMensual { get; set; }
        public int fiTieneProducto { get; set; }
        public List<ListaDetallePaqueteViewModel> ListaDetallePaquete { get; set; }

    }
    public class ListaDetallePaqueteViewModel
    {
        public int fiIDPaquete { get; set; }
        public int fiIDProducto { get; set; }
        public byte fiIDTipoProducto { get; set; }
        public string fcTipoProducto { get; set; }
        public int fiCatidad { get; set; }
        public string fcToken { get; set; }
        public string fcProducto { get; set; }
        public decimal fnValorProductoMN { get; set; }
        public decimal fnValorProductoME { get; set; }
        public decimal fnPorcentajeImpuesto1 { get; set; }
        public decimal fnPorcentajeImpuesto2 { get; set; }
        public string fcMarca { get; set; }  
        public bool Seleccionado { get; set; }
        public decimal fnValorProducto { get; set; }
        public decimal fnValorMensual { get; set; }
        public decimal fnValorCuotaMensual { get; set; }
        public int fiMesesCondicion { get; set; }
        public int fiIDPlazo { get; set; }
        public decimal fnValorPaquete { get; set; }
        public int fiMeses { get; set; }

    }
}