using OrionCoreCableColor.Models.Prestamo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Productos
{
    public class CrearFacturacionInventarioViewModel
    {
        [Display(Name = "Factura")]
        public string fcFactura { get; set; }
        [Display(Name = "Numero Prestamo")]
        public string fcIDPrestamo { get; set; }
        [Display(Name = "Identidad")]
        public string fcIdentidad { get; set; }
        [Display(Name = "Cliente")]
        public string fcNombreCliente { get; set; }
        [Display(Name = "Telefono")]
        public string fcTelefonoCliente { get; set; }
        [Display(Name = "Correo")]
        public string fcCorreoCliente { get; set; }
        public int fiIDInventarioMovimientoMaestro { get; set; }
        [Display(Name = "Origen")]
        public int fiIDUbicacionOrigen { get; set; }
        public string fcUbicacionOrigen { get; set; }
        [Display(Name = "Destino")]
        public int fiIDUbicacionDestino { get; set; }
        public int fiIDFacturacionMaestro { get; set; }
        public string fcUbicacionDestion { get; set; }
        public bool fbEditar { get; set; }
        public List<ListFacturacionDetalleViewModel> DetalleFactura { get; set; }
        public List<ListInventarioMovimientoDetalleViewModel> ListaInventarioDetalle { get; set; }
    }
}