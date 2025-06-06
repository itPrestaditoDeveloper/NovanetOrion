using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.DatosCliente
{
    public class agendainstalacionViewModel
    {
       
        public int fiIDEquifax { get; set; }
     
        public string fcNombreCliente { get; set; }
        [Required]
        [Display(Name = "Hora")]
        public int fihora { get; set; }
        public DateTime fdfechaInstalacion  { get; set; }
        [Required]
        [Display(Name = "Dia")]
        public int fiDia { get; set; }
        [Required]
        [Display(Name = "Mes")]
        public int fiMes { get; set; }
        [Required]
        [Display(Name = "Indique la persona quien atendera la instalación")]
        public string fcNombreAtiende { get; set; }
    }
}