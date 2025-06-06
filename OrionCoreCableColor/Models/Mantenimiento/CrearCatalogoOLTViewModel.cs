using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Mantenimiento
{
    public class CrearCatalogoOLTViewModel
    {
        public int fiIDOlt { get; set; }
        [DisplayName("OLT")]
        [Required]
        public string fcDescripcionOLT { get; set; }
        [DisplayName("Direccion")]
        public string fcDireccion { get; set; }
        [DisplayName("Latitude")]
        public string fcLatitude { get; set; }
        [DisplayName("Longitud")]
        public string fcLongitud { get; set; }
        [DisplayName("IP")]
        public string fcIPOLT { get; set; }
        public System.DateTime fdFechaCreacion { get; set; }
        public int fiIDUsuarioCreacion { get; set; }
        public System.DateTime fdFechaModificacion { get; set; }
        public int fiIDUsuarioModificacion { get; set; }
        public string USuarionCreacion { get; set; }
        public bool fbEditar { get; set; }
    }
}