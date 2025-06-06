using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.DbConnection.CMContext
{
    public partial class Usuarios_Maestro
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Usuarios_Maestro()
        {
            RolesPorUsuario = new HashSet<RolesPorUsuario>();
        }

        [Key]
        public int fiIDUsuario { get; set; }

        [Required]
        [StringLength(20)]
        public string fcPrimerNombre { get; set; }

        [Required]
        [StringLength(20)]
        public string fcSegundoNombre { get; set; }

        [Required]
        [StringLength(20)]
        public string fcPrimerApellido { get; set; }

        [Required]
        [StringLength(20)]
        public string fcSegundoApellido { get; set; }

        [Required]
        [StringLength(30)]
        public string fcNombreCorto { get; set; }
        [Required]
        [StringLength(4)]
        public string fcCentrodeCosto { get; set; }

        [Required]
        public Int16 fiIDPuesto { get; set; }

        [Required]
        public byte fiTipoUsuario { get; set; }

        [Required]
        public byte fiIDDepartamento { get; set; }

        [Required]
        public int fiIDJefeInmediato { get; set; }

        [Required]
        [StringLength(30)]
        public string fcUsuarioDominio { get; set; }

        [Required]
        public byte fiIDDominioRed { get; set; }

        [Required]
        [StringLength(256)]
        public string fcPassword { get; set; }

        [Required]
        [StringLength(256)]
        public string fcPasswordToken { get; set; }

        public DateTime? fdFechaUltimoCambiodePassword { get; set; }

        [Required]
        [StringLength(100)]
        public string fcBuzondeCorreo { get; set; }

        [Required]
        public byte fiIDDominioCorreo { get; set; }
        
       


        [Required]
        [StringLength(50)]
        public string fcDireccionFisica { get; set; }


        [Required]
        [StringLength(20)]
        public string fcDocumentoIdentificacion { get; set; }

        [Required]
        public DateTime fdFechaAlta { get; set; }

        [Required]
        public int fiIDUsuarioSolicitante { get; set; }

        [Required]
        public int fiIDUsuarioCreador { get; set; }

        [Required]
        public int fiEstado { get; set; }

        public DateTime? fdFechaBaja { get; set; }

        [Required]
        [StringLength(10)]
        public string fcTelefonoMovil { get; set; }

        [Required]
        [StringLength(512)]
        public string fcToken { get; set; }

        [Required]
        [StringLength(128)]
        public string fcIdAspNetUser { get; set; }


        public string fcUrlImage { get; set; }

        //empresa
        public int? fiIDEmpresa { get; set; }
        public int? fiIDDistribuidor { get; set; }
        public virtual AspNetUsers AspNetUsers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RolesPorUsuario> RolesPorUsuario { get; set; }
    }
}