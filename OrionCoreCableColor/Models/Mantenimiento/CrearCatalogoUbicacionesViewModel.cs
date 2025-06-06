using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Mantenimiento
{
    public class CrearCatalogoUbicacionesViewModel
    {
        public int fiIDUbicacion { get; set; }
        [DisplayName("Ubicacion")]
        [Required]
        public string fcUbicacion { get; set; }
        [DisplayName("Descripcion")]
        [Required]
        public string fcUbicacionFisica { get; set; }
        public int fiEstadoUbicacion { get; set; }
        public bool fbEditar { get; set; }
        [DisplayName("Ubicacion Externa")]
        public int fiUbicacionExterna { get; set; }
    }
}