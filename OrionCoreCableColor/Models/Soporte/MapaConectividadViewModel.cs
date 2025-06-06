using OrionCoreCableColor.DbConnection.OrionDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Soporte
{
    public class MapaConectividadViewModel : sp_MapaConInfoOnuyOlt_Result
    {
        public bool fbEstadoServicio { get; set; }
        public int fiIDOLT { get; set; }
        public bool fbOLTConectado { get; set; }
        public bool fbONUConectado { get; set; }
        public decimal fnCapitalAtrasado { get; set; }
        public int fiDiasAtraso { get; set; }
        public string fcCodigoCepheus { get; set; }
        public string fcEmpresa { get; set; }
        public DateTime fdUltimaActualizacion { get; set; }
        public string fcEstadoPing { get; set; }
    }
}