using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Account
{
    public class DatosUsuarioViewModel
    {
        public bool Estado { get; set; }
        public string Mensaje { get; set; }
        public string Titulo { get; set; }
        public int fiIDUsuario { get; set; }
        public string fcNombreCorto { get; set; }
        public string fcBuzonDeCorreo { get; set; }
        public int fiIDEmpresa { get; set; }
        public string fcEmpresa { get; set; }
        public int fiIDRol { get; set; }
        public string Role { get; set; }
        public List<string> Permisos { get; set; }
        public string UsuarioCifrado { get; set; }
        public int fiIDUbicacion { get; set; }
    }
}