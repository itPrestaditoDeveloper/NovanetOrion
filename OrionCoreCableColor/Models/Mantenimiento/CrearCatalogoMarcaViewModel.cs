using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Mantenimiento
{
    public class CrearCatalogoMarcaViewModel
    {
        public int fiIdMarca { get; set; }
        [DisplayName("Marca")]
        [Required]
        public string fcMarca { get; set; }
        public bool fbEditar { get; set; }
    }
}