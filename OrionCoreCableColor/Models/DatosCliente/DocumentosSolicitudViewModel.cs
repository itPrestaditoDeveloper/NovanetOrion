using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.DatosCliente
{
    public class DocumentosSolicitudViewModel
    {
        public int fiIDDocumento { get; set; }
        public int fiIDOrion_Documento { get; set; }
        public string fcDocumento { get; set; }
        public bool fbDocumentoExiste { get; set; }
        public string fcNombreArchivo { get; set; }
        public string fcExtension { get; set; }
        public string fcRutaArchivo { get; set; }
        public string fcTipoDocumento { get; set; }
        public string fcRuta { get; set; }
        public string fcURL { get; set; }
        public string fcDireccion { get; set; }

    }
}