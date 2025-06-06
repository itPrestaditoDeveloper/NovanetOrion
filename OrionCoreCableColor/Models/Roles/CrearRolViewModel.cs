using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Roles
{
    public class CrearRolViewModel
    {
        public int Pk_IdRol { get; set; }
        public string Nombre { get; set; }
        public string Observacion { get; set; }
        public bool Activo { get; set; }

        public bool EsEditar { get; set; }
    }
}