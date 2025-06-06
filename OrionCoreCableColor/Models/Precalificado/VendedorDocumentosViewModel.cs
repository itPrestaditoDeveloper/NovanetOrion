using OrionCoreCableColor.Models.Reportes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Precalificado
{
    public class VendedorDocumentosViewModel
    {
        public int fiIDOrion_Documento { get; set; }
        public string fcNombreArchivo { get; set; }
        public string fcRuta { get; set; }
        public string fcTipoDocumento { get; set; }
        public string fcComentario { get; set; }
        public int fiEstadoFirma { get; set; }
        public string fcDescripcionDocumento { get; set; }
        public string fcDescripcion => fcDescripcionDocumento ?? "Documento";

       
    }

}