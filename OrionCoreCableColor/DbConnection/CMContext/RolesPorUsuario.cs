using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.DbConnection.CMContext
{
    [Table("RolesPorUsuario")]
    public partial class RolesPorUsuario
    {
        [Key]
        public int Pk_IdRolesPorUsuario { get; set; }

        public int Fk_IdUsuario { get; set; }

        public int Fk_IdRol { get; set; }

        public virtual Roles Roles { get; set; }

        public virtual Usuarios_Maestro Usuarios { get; set; }
    }
}