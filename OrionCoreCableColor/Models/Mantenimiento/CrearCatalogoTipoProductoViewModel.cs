using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Mantenimiento
{
    public class CrearCatalogoTipoProductoViewModel
    {
        public int fiIDTipoProducto { get; set; }
        [DisplayName("Tipo Producto")]
        [Required]
        public string fcTipoProducto { get; set; }
        public bool fbEditar { get; set; }
    }
}