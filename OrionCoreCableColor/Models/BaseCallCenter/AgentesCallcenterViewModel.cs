using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.BaseCallCenter
{
    public class AgentesCallcenterViewModel
    {
        [DisplayName("Agente")]
        public int fiIDUsuario { get; set; }
        [DisplayName("Nombre")]
        public string fcNombreCorto { get; set; }
    }
}