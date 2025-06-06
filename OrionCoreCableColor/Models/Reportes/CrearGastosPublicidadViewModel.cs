using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Reportes
{
    public class CrearGastosPublicidadViewModel
    {
        public int fiIdGastosPublicidad { get; set; }
        public int fiAnio { get; set; }
        public int fiMes { get; set; }
        public int fiDia { get; set; }
        public int fiCodPais { get; set; }
        [DisplayName("Monto")]
        public decimal fnValorMonto { get; set; }
        [DisplayName("Fecha de Ingreso")]
        public System.DateTime fdFechaIngreso { get; set; }
        public int fiIdUsuarioCreador { get; set; }
        public System.DateTime fdFechaModificacion { get; set; }
        public int fiIDUsuarioModificacion { get; set; }
        [DisplayName("Comentario")]
        public string fcComentario { get; set; }
        public bool fbEditar { get; set; }
    }
}