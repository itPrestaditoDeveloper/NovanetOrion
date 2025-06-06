using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Mantenimiento
{
    public class CrearCatalogoTipoMovimientoViewModel
    {
        
        public int fiIDTipoMovimiento { get; set; }
        [DisplayName("Movimiento")]
        public string fcTipoMovimiento { get; set; }
        [DisplayName("Tipo de afectacion")]
        public int fiTipoAfectacion { get; set; }
        public bool fbEditar { get; set; }
    }
}