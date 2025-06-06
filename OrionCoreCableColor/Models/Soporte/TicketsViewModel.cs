using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Soporte
{
    public class TicketsViewModel
    {
        public int fiIdTicket { get; set; }
        public string fcTitulo { get; set; }
        public string fcDescripcion { get; set; }
        public DateTime fdFechaCreacion { get; set; }
        public int fiIdTipoSolicitante { get; set; }
        public int fiIdSolicitante { get; set; }
        public string fcSolicitante { get; set; } // Prefijo + ID
        public string fcNombreSolicitante { get; set; }
        public int fiIdAreaAsignada { get; set; }
        public string fcAreaAsignada { get; set; }
        public int fiIdUsuarioAsignado { get; set; }
        public string fcUsuarioAsignado { get; set; }
        public int fiIdEstado { get; set; }
        public string fcClaseColor { get; set; }
        public string fcEstado { get; set; }
        public bool fbEditable { get; set; }
        public bool fbEliminado { get; set; }


        // Permisos devueltos por sp_Ticket_Listar
        public bool fbPuedeVerlo { get; set; }
        public bool fbPuedeEliminar { get; set; }
        public bool fbPuedeEditar { get; set; }
        public bool fbPuedeAsignarArea { get; set; }
        public bool fbPuedeAsignarUsuario { get; set; }
        public bool fbPuedePonerEnAtencion { get; set; }
        public bool fbPuedePonerEnPausa { get; set; }
        public bool fbPuedeValidar { get; set; }
        public bool fbPuedeCerrar { get; set; }



        public int fiIdTipoIncidencia { get; set; }
        public string fcTipoIncidencia { get; set; }
        public int fiIdCategoria { get; set; }
        public string fcDescripcionCategoria { get; set; }
        public int fiIdPrioridad { get; set; }
        public string fcDescripcionPrioridad { get; set; }
        public int fiIdUsuarioCreacion { get; set; }
        public DateTime fdFechaCreacionRegistro { get; set; }
        public int fiIdUsuarioModificacion { get; set; }
        public DateTime fdFechaUltimaModificacion { get; set; }
        public int fiIdAreaRelacionada { get; set; }
        public string  fcAreaRelacionada { get; set; }
        public DateTime fdFechaCierre { get; set; }

        public string fcComentario { get; set; }
        public int fiPorcentajeResolucion { get; set; }
        public string fcUsuariosTrabajaron { get; set; }
    }
}