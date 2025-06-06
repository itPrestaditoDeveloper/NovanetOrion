using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Usuario
{
    public class ListaDeUsuariosViewModel : CrearUsuarioViewModel
    {
        //public List<InfoUsuarioViewModel> InfoUsuario { get; set; }
        public double fnLongitud { get; set; }
        public double fnLatitud { get; set; }
    }

    public class InfoUsuarioViewModel
    {
        public string IP { get; set; }
        public string Navegador { get; set; }
        public string UserName { get; set; }
    }
}