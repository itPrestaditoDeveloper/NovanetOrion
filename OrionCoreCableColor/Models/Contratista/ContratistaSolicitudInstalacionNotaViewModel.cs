using System;
using System.ComponentModel.DataAnnotations;

namespace OrionCoreCableColor.Models.Contratista
{
    public class ContratistaSolicitudInstalacionNota
    {
        public int FiIDSolicitudInstalacionNota { get; set; }

        public int FiIDContratistaSolicitud { get; set; }

        [Display(Name = "Comentario")]
        [Required]
        public string FcComentarioNota { get; set; }

        public int FiIDUsuarioCreacion { get; set; }

        public string FcNombreCortoCreacion { get; set; }

        public DateTime? FdFechaCreacion { get; set; }

        public int? FiIDUsuarioUltimaModificacion { get; set; }

        public string FcNombreCortoUltimaEdicion { get; set; }

        public DateTime? FdUltimaFechaModificacion { get; set; }

        public bool fbEditar { get; set; }
    }

}