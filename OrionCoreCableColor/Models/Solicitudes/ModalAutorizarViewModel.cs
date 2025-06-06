
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace OrionCoreCableColor.Models.Solicitudes
{
    public  class ModalAutorizarViewModel
    {
        [Display(Name = "Cliente")]
        public string NombreCliente { get; set; }

        [Display(Name = "Artículos")]
        public string Articulos { get; set; }

        [Display(Name = "Comentario Vendedor")]
        public string ComentarioVendedor { get; set; }
    }
}