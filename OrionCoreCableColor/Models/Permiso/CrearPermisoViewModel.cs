using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Permiso
{
    public class CrearPermisoViewModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "Nombre de Permiso")]
        public string Nombre { get; set; }

        public string Id { get; set; }
        public bool Estado { get; set; }
        public bool EsEditar { get; set; }
    }
}