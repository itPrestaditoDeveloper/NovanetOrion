using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Reportes
{
    public class ReporteClientesCancelados
    {
        public int fiIDSolicitud { get; set; } // ID de la solicitud
        public int fiIDCliente { get; set; } // ID del cliente
        public string fcNombre { get; set; } // Nombre del cliente
        public string fcIdentidad { get; set; } // Identidad del cliente
        public int fiPlazoSeleccionado { get; set; } // Plazo seleccionado
        public decimal fnCuotaMensual { get; set; } // Cuota mensual
        public string fcIDPrestamo { get; set; } // ID del préstamo
        public int fiIDEstadoInstalacion { get; set; } // Estado de la instalación
        public string fcDescripcion { get; set; } // Descripción del estado de instalación
        public DateTime fdFechaInstalacionInicio { get; set; } // Fecha de inicio de la instalación
        public DateTime fdFechaInstalacionFinal { get; set; } // Fecha de finalización de la instalación
        public int fiIDTipoSolicitud { get; set; } // Tipo de solicitud
        public string fcIDPrestamoDuplicado { get; set; } // ID del préstamo duplicado
        public int fiIDTransaccion { get; set; } // ID de la transacción
        public DateTime fdFechaTransaccion { get; set; } // Fecha de la transacción
        public string fcOrigenOperacion { get; set; } // Origen de la operación
        public int fiIDUsuario { get; set; } // ID del usuario
        public string fcNombreCorto { get; set; } // Nombre corto del usuario
        public string fcReferencia { get; set; } // Referencia de la operación
    }

}