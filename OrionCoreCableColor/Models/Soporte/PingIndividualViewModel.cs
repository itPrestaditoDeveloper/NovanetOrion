using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Soporte
{
    public class PingIndividualViewModel
    {
        public string fcHost { get; set; }
        public int fiPacketSent { get; set; }
        public int fiPacketReceived { get; set; }
        public int fiPacketLost { get; set; }
        public decimal fnPorcentaje { get; set; }

        public long fiMinTime { get; set; }
        public long fiMaxTime { get; set; }
        public long fiMediaTime { get; set; }
        public long fiTotalTime { get; set; }
        public List<PingDataViewModel> Data { get; set; }
        public int fiIDUsuario { get; set; }
    }
}