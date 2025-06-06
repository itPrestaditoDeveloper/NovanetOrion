using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.DbConnection.CMContext
{
    public partial class OrionSecurityEntities : DbContext
    {
        public OrionSecurityEntities() : base("name=OrionSecurityEntities")
        {

        }

        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<PrivilegiosPorRol> PrivilegiosPorRol { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<RolesPorUsuario> RolesPorUsuario { get; set; }
        public virtual DbSet<Usuarios_Maestro> Usuarios { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoles>()
                .HasMany(e => e.PrivilegiosPorRol)
                .WithRequired(e => e.AspNetRoles)
                .HasForeignKey(e => e.Fk_IdPermiso)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetRoles>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.Usuarios)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.fcIdAspNetUser)
                .WillCascadeOnDelete(false);

            

            modelBuilder.Entity<Roles>()
                .HasMany(e => e.PrivilegiosPorRol)
                .WithRequired(e => e.Roles)
                .HasForeignKey(e => e.Fk_IdRol)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Roles>()
                .HasMany(e => e.RolesPorUsuario)
                .WithRequired(e => e.Roles)
                .HasForeignKey(e => e.Fk_IdRol)
                .WillCascadeOnDelete(false);


          

            modelBuilder.Entity<Usuarios_Maestro>()
                .HasMany(e => e.RolesPorUsuario)
                .WithRequired(e => e.Usuarios)
                .HasForeignKey(e => e.Fk_IdUsuario)
                .WillCascadeOnDelete(false);

           
        }


    }
}