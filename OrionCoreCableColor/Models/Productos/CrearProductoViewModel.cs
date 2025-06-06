using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Productos
{
    public class CrearProductoViewModel
    {
        public int fiIDProducto { get; set; }
        [DisplayName("Codigo de Barras")]
        [Required]
        public string fcCodigodeBarras { get; set; }
        [DisplayName("Producto")]
        [Required]
        public string fcProducto { get; set; }
        [DisplayName("Imagen")]
        public string fcImagenProducto { get; set; }
        public HttpPostedFileBase ImgenProducto { get; set; }
        [DisplayName("Marca")]
        [Required]
        public int fiIdMarca { get; set; }
        [DisplayName("Modelo")]
        [Required]
        public int fiIdModelo { get; set; }
        [DisplayName("Tipo de producto")]
        public int fiIdTipoProducto { get; set; }
        [DisplayName("Valor en Lempiras")]
        public decimal fnValorProductoMN { get; set; }
        [DisplayName("Valor en Dolares")]
        public decimal fnValorProductoME { get; set; }
        [DisplayName("Impuesto 1")]
        [Required]
        public decimal fnPorcentajeImpuesto1 { get; set; }
        [DisplayName("Impuesto 2")]
        public decimal fnPorcentajeImpuesto2 { get; set; }
        public int fiEstadoProducto { get; set; }
        [DisplayName("Inventariable")]
        public int fiProductoInventariable { get; set; }
        
        [DisplayName("Paquete")]
        public int fiProductoEnPaquete { get; set; }

        [DisplayName("Valor Cuota Mensual")]
        public decimal fnValorCuotaMensual { get; set; }
        public bool fbEditar { get; set; }

        public DateTime fdFechaUltimaModificacion { get; set; }
        [DisplayName("Valor Cuota Mensual 12 Meses Cliente Nuevo")]
        public decimal fnValorCuotaMensual12Nuevo { get; set; }
        [DisplayName("Valor Cuota Mensual 24 Meses Cliente Nuevo")]
        public decimal fnValorCuotaMensual24Nuevo { get; set; }
        [DisplayName("Valor Cuota Mensual 12 Meses Cliente Activo")]
        public decimal fnValorCuotaMensual12Cliente { get; set; }
        [DisplayName("Valor Cuota Mensual 24 Meses Cliente Activo")]
        public decimal fnValorCuotaMensual24Cliente { get; set; }
        [DisplayName("Articulo extra")]
        public bool fbProductoSeleccionablePorCliente { get; set; }
        [DisplayName("Producto Por Cantidad")]
        public bool fbProductoGenerico { get; set; }
        [DisplayName("Producto de Instalacion")]
        public bool fbProductoDeInstalacion { get; set; }

    }
}