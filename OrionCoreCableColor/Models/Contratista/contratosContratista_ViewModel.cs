using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Contratista
{
    public class contratosContratista_ViewModel
    {
        public int fiIDSolicitudInstalacion { get; set; }

        [Display(Name ="Cliente")]
        public string fcNombreCliente { get; set; }

        public string fcUrlContratoSinFirma { get; set; }
        public string fcNombreArchivoSinFirma { get; set; }
        public string fcUrlContratoConFirma { get; set; }
        public string fcNombreArchivoConFirma { get; set; }
        public  int  fiIDEstadoInstalacion { get; set; }
    }
}