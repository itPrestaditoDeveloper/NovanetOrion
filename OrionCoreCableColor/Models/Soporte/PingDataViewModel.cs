using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Soporte
{
    public class PingDataViewModel
    {
        public long fiTiempo { get; set; }
        public int fiTtl { get; set; }
        public int fiBytes { get; set; }
        public bool fbSuccess { get; set; }
    }
}