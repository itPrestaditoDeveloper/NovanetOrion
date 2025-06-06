using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Notificaciones
{
    public class NotificacionesViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        [Display(Name = "Texto de Notificación")]
        public string Message { get; set; }
        public List<string> Users { get; set; }
        public int FiIDUsuario { get; set; }


    }

    public class BitacoraNotificacionesViewModel
    {
        public int fiIDNotificacion { get; set; }
        public string fcNotificacion { get; set; }
        public int fiIDEquifax { get; set; }
        public string fcNombre { get; set; }
        public DateTime fdFechaEnvio { get; set; }
        public int fiIDUsuario { get; set; }
        public string fcNombreCorto { get; set; }
    }
}