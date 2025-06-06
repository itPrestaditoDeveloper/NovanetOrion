using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Precalificado
{
    public class CrearVendedorCalleViewModel
    {
        public int fiIDVendedorRepartidor { get; set; }
        [Display(Name = "Cliente")]
        public string fcNombreVendedor { get; set; }
        [Display(Name = "Número Vendedor")]
        public string fcNumeroVendedor { get; set; }
        [Display(Name = "Identidad")]
        public string fcIdentidadVendedor { get; set; }
        [Display(Name = "Código")]
        public string fcCodigoVendedor { get; set; }
        public Nullable<int> fiIDetalleCodigo { get; set; }
        public bool fbEditar { get; set; }

    }
}