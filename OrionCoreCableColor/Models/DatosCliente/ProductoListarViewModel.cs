using OrionCoreCableColor.DbConnection.OrionDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.DatosCliente
{
    public class ProductoListarViewModel 
    {
        public int fiIDProducto { get; set; }
        public string fcProducto { get; set; }
        public decimal fnValorProducto { get; set; }
        public string fcTipoProducto { get; set; }
        public string fcImagenProducto { get; set; }
        public bool Seleccionado { get; set; }
        public decimal fnValorProductoMN { get; set; }
        public decimal fnValorProductoME { get; set; }
        public decimal fnPorcentajeImpuesto1 { get; set; }
        public decimal fnPorcentajeImpuesto2 { get; set; }
        public int IDSolicitud  { get; set; }
        public decimal fnValorCuotaMensual { get; set; }
        public int fiIDPaquete { get; set; }
        public decimal fnValorMensual { get; set; }       
        public int fiMesesCondicion { get; set; }
        public int fiIDPlazo { get; set; }
        public decimal fnValorPaquete { get; set; }
        public int fiMeses { get; set; }

    }
}