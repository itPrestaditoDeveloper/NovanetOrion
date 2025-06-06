using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Mantenimiento
{
    public class CrearCatalogoModeloViewModel
    {
        
        public int fiIDModelo { get; set; }
        [DisplayName("Marca")]
        [Required]
        public int fiIdMarca { get; set; }
        [DisplayName("Modelo")]
        [Required]
        public string fcModelo { get; set; }
        public bool fbEditar { get; set; }
    }
}