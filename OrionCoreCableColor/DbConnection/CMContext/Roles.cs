using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.DbConnection.CMContext
{
    public partial class Roles
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Roles()
        {
            PrivilegiosPorRol = new HashSet<PrivilegiosPorRol>();
            RolesPorUsuario = new HashSet<RolesPorUsuario>();
        }

        [Key]
        public int Pk_IdRol { get; set; }

        [Required]
        [StringLength(255)]
        public string Nombre { get; set; }

        public bool Activo { get; set; }

        [StringLength(255)]
        public string Observacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PrivilegiosPorRol> PrivilegiosPorRol { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RolesPorUsuario> RolesPorUsuario { get; set; }
    }
}