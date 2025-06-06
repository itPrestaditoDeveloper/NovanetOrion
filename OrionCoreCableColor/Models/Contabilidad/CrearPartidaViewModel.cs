using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Contabilidad
{
    public class CrearPartidaViewModel
    {
        public int fiIDEmpresa { get; set; }

        [Required]
        [Display(Name = "Tipo Partida")]
        public string fcTipoPartida { get; set; }

        [Required]
        [Display(Name = "Partida")]
        public string fcPartida { get; set; }

        [Required]
        [Display(Name = "Fecha")]
        public System.DateTime fdFechaPartida { get; set; }

        [Required]
        [Display(Name = "Lote")]
        public string fcLote { get; set; }

        [Required]
        [Display(Name = "Referencia Documento")]
        public string fcReferencia { get; set; }

        public string fcEstadoPartida { get; set; }

        public int fiMoneda { get; set; }
        public string Moneda { get; set; }
        public string SimboloMoneda { get; set; }

        public string fcAutoriza { get; set; }
        public System.DateTime fdFechaAutorizacion { get; set; }
        public string fcOrigen { get; set; }
        public System.DateTime fdFechaMigracion { get; set; }

        [Display(Name = "Partida Origen")]
        public string fcPartidaOrigen { get; set; }

        public System.DateTime fdFechaOrigen { get; set; }

        public string fcCreador { get; set; }
        public System.DateTime fdFechaCreacion { get; set; }
        public string fcPartidaAnterior { get; set; }
        public string fcCierreSAF { get; set; }
        public string fcEmpresaSAF { get; set; }
        public string fdUltimaActualizacion { get; set; }
        public string fcToken { get; set; }

        [Required]
        [Display(Name = "Sinopsis")]
        public string fcSinopsis { get; set; }

        public string fcAutGuid { get; set; }
        public string fcUsuarioMigrador { get; set; }

        [Required]
        [Display(Name = "Monto Total Partida")]

        public decimal fnValorPartida { get; set; }
        public int fiIDUsuarioCreador { get; set; }

        public byte fiPartidaMayorizada { get; set; }
        public System.DateTime fdFechadeMayorizacion { get; set; }
        public int fiIDUsuarioMayorizacion { get; set; }
        public System.Guid IDGUID { get; set; }

        [Display(Name = "Partida Seguimiento")]
        public string fcPartidaSeguimiento { get; set; }

        public bool EsEditar { get; set; }
        public bool EsReclasificacion { get; set; }

        public List<CrearDetallePartidaViewModel> ListaDetalle { get; set; }

        //helper
        public bool EsClonacion { get; set; }

        public int fiIDMovimientoMaestro { get; set; }
        public bool Exento { get; set; }
        public bool EsCuentaPorCobrar { get; set; }
        
        public int fiIDOperacionesContablesInventario { get; set; }
        public bool fbTrasladoPrestadito { get; set; }
        public decimal TotalDebe { get; set; }
        public decimal TotalHaber { get; set; }
    }
}