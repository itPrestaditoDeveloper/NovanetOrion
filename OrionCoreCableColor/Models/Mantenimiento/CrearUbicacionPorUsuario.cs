using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Mantenimiento
{
    public class CrearUbicacionPorUsuario
    {
        public int fiIDUbicacionPorUsuario { get; set; }
        [DisplayName("Empresa")]
        public int fiIDEmpresa { get; set; }
        [DisplayName("Ubicacion")]
        public int fiIDUbicacion { get; set; }
        [DisplayName("Usuario")]
        public int fiIDUsuario { get; set; }
        [DisplayName("Usuario Principal")]
        public bool fbUsuarioPrincipal { get; set; }
        public bool fbEditar { get; set; }
    }
}