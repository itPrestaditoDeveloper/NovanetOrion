using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.BaseCallCenter
{
    public class GestionTiempoBaseCallCenterViewModel
    {
        public int fiHora { get; set; }
        public string fcRangodeFecha { get; set; }
        public int fiLlamadasRealizadas { get; set; }
    }
    public class GestionCallCenter
    {
        public string fcGestion { get; set; }
        public int fiTotalBitacoras { get; set; }   
    }

}