using OrionCoreCableColor.Models.Contratista;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Contratista
{
    public class AdicionEquipoViewModel
    {
        public string Identidad { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string  fcProducto { get; set; }       
        public decimal CuotaMensualMonedaNacional { get; set; }
        public List<EquipoViewModel> equiposCliente { get; set; }
        public string fcIDPrestamo { get; set; }
        public int fiIDEquifax { get; set; }
        public int fiIDProducto { get; set; }
        public int fiIDSolicitud { get; set; }
        public int fiIDFirma { get; set; }
    }

    public class EquipoViewModel
    {
        public int fiIDProducto { get; set; }
        public int fiIDSolicitud { get; set; }
        public string Articulos { get; set; }
        public int fiCantidad { get; set; }
        public decimal CuotaMensualMonedaNacional { get; set; }
        public decimal CuotaMensualMonedaE { get; set; }
        public decimal PrecioLempiras { get; set; }
        public decimal PrecioDolares { get; set; }
        public string fcIDPrestamo { get; set; }
        public int fiPlazo { get; set; }
        public int fiCuotasPagadas { get; set; }
        public int fiCuotasAtrasadas { get; set; }
        public int fiCuotasCursadas { get; set; }
        
    }


}


