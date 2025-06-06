using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Productos
{
    public class CrearSalidaInventarioViewModel
    {
        public int fiIDInventarioMovimientoMaestro { get; set; }
        [DisplayName("Ubicacion")]
        public int fiIdUbicacion { get; set; }
        public string fcUbicacion { get; set; }
        [DisplayName("Destinatario")]
        public int fiIDUsuarioDestino { get; set; }
        public string fcDestinatario { get; set; }
        public int fiIDTipoMomvimento { get; set; }
        public HttpPostedFileBase Firma { get; set; }
        public List<ListInventarioMovimientoDetalleViewModel> ListaInventarioDetalle { get; set; }
        public bool fbEditar { get; set; }
        public DateTime fdFechaCreacion { get; set; }
        public string UsuarioPeticion { get; set; }
        [DisplayName("Destino")]
        public int fiIDUbicacionDestino { get; set; }
        [DisplayName("Motivo de devolucion")]
        public string fcMotivoDevolucion { get; set; }
    }
}