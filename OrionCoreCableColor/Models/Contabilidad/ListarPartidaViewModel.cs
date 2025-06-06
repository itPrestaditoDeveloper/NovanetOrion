using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Contabilidad
{
    public class ListarPartidaViewModel : CrearPartidaViewModel
    {
        public decimal TotalDebe { get; set; }
        public decimal TotalHaber { get; set; }
        public decimal TotalDiferencia { get; set; }
    }
}