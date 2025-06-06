using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.DbConnection.CMContext
{
    [Table("PrivilegiosPorRol")]
    public partial class PrivilegiosPorRol
    {
        [Key]
        public int Pk_IdPrivilegiosPorRol { get; set; }

        public int Fk_IdRol { get; set; }

        [Required]
        [StringLength(128)]
        public string Fk_IdPermiso { get; set; }

        public virtual AspNetRoles AspNetRoles { get; set; }

        public virtual Roles Roles { get; set; }
    }
}