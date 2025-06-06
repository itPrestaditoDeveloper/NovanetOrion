using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Base
{
    public class MensajeRespuestaViewModel
    {
        public string Titulo { get; set; }
        public string Mensaje { get; set; }
        public bool Estado { get; set; }
        public int Id { get; set; }
        public string Correlativo { get; set; }
    }
}