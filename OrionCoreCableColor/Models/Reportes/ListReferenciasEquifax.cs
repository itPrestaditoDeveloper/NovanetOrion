using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Reportes
{
    public class ListReferenciasEquifax
    {
        public int fiIDReferencia { get; set; }
        public int fiIDEquifax { get; set; }
        public string fcNombreCompletoReferencia { get; set; }
        public string fcTelefonoReferencia { get; set; }
        public string fcDescripcionParentesco { get; set; }

        public bool fbNuevo { get; set; }
        public bool fbEnviado { get; set; }
        public int fiIDParentesco { get; set; }
        public string NombreClienteEquifax { get; set; }
        public int? fbAceptoSerReferencia { get; set; }
        public int? fiRespuestasAcertadas { get; set; }
        public int? fiRespuestasNoAcertadas { get; set; }
        public int? fiIDTipoSolicitud { get; set; }
        public int? fiIDConteoReinicio { get; set; }
    }
}