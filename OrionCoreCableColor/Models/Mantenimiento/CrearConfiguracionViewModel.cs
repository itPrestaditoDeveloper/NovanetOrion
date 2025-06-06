using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Mantenimiento
{
    public class CrearConfiguracionViewModel
    {
        public int fiIdConfiguraciones { get; set; }
        [DisplayName("Llave")]
        [Required]
        public string fcNombreLlave { get; set; }
        [DisplayName("Valor")]
        [Required]
        public string fcValorLlave { get; set; }

        public bool fbEditar { get; set; }
    }
}