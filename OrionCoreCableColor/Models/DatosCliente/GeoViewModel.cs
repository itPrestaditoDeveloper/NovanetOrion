using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.DatosCliente
{
    public class GeoViewModel
    {
        public short fiCodPais { get; set; }
        public string fcPais { get; set; }
        public short fiCodDepartamento { get; set; }
        public string fcDepartamento { get; set; }
        public short fiCodMunicipio { get; set; }
        public string fcMunicipio { get; set; }
        public int fiCodBarrio { get; set; }
        public string fcBarrio { get; set; }
    }
}