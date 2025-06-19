using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Manto
{
    public class CrearBitacoraTokenViewModel
    {
        public class CampoAfectadoViewModel
        {
            public string fcNombreCampo { get; set; }
            public string fcValorAntes { get; set; }
            public string fcValorDespues { get; set; }
        }

        public class TokenBitacoraViewModel
        {
            public int fiIDSolicitud { get; set; }
            public int fiIDUsuario { get; set; }
            public string fcTokenAplicado { get; set; }
            public string fcCodigoToken { get; set; }
            public string fcComentario { get; set; }
            public List<CampoAfectadoViewModel> CamposAfectados { get; set; }
        }

    }
}