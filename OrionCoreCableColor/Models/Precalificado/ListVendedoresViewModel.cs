using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Precalificado
{
    public class ListVendedoresViewModel
    {

        public int fiIDVendedorRepartidor { get; set; }
        public string fcNombreVendedor { get; set; }
        public string fcNumeroVendedor { get; set; }
        public string fcIdentidadVendedor { get; set; }
        public string fcPrimerNombre { get; set; }
        public string fcSegundoNombre { get; set; }
        public string fcPrimerApellido { get; set; }
        public string fcSegundoApellido { get; set; }
        public string fcCodigoVendedor { get; set; }
        public Nullable<int> fiIDetalleCodigo { get; set; }
        public Nullable<int> fiUsuarioCreador { get; set; }
        public Nullable<System.DateTime> fdFechaCreacion { get; set; }
        public Nullable<int> fiUsuarioModificacion { get; set; }
        public Nullable<System.DateTime> fdFechaModificacion { get; set; }
        public Nullable<int> fiIDTipoVendedor { get; set; }
        public Nullable<int> fIIDOcupacion { get; set; }
        public string fcOcupacion { get; set; }
        public string fcCiudad { get; set; }
        public string fcDireccionExacta { get; set; }
        public string fcLongitud { get; set; }
        public string fcLatitud { get; set; }
        public Nullable<System.DateTime> fdFechaNacimiento { get; set; }
        public string fcCorreo { get; set; }


    }
}