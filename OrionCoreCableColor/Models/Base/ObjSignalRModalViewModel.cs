using OrionCoreCableColor.Models.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Base
{
    public class ObjSignalRModalViewModel
    {
        public string Url { get; set; }
        public object Obj { get; set; }
        public InfoUsuarioViewModel InfoUsuario { get; set; }
    }
}