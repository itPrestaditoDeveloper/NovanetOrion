using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OrionCoreCableColor.DbConnection.CMContext;
using OrionCoreCableColor.Models;
using OrionCoreCableColor.Models.Permiso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.Controllers
{
    [Authorize]
    public class PermisoController : BaseController
    {
        // GET: Permiso
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Listar()
        {
            using (var context = new OrionSecurityEntities())
            {
                var jsonResult = Json(context.AspNetRoles.Where(z => z.Activo == true).Select(x => new CrearPermisoViewModel { Id = x.Id, Nombre = x.Name, Estado = true }).ToList(), JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
        }

        [HttpGet]
        public ActionResult Crear()
        {
            return PartialView(new CrearPermisoViewModel());
        }


        [HttpPost]
        public ActionResult Crear(CrearPermisoViewModel model)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            if (roleManager.RoleExists(model.Nombre.Trim()))
                return EnviarResultado(false, "Crear Permiso", "El Permiso ya Existe");
            var result = roleManager.Create(new IdentityRole { Name = model.Nombre.Trim() });

            if (result.Succeeded)
            {
                using (var context = new OrionSecurityEntities())
                {
                    var rol = context.AspNetRoles.FirstOrDefault(z => z.Name == model.Nombre.Trim());
                    rol.Activo = true;
                    context.Entry(rol).State = System.Data.Entity.EntityState.Modified;
                    var result2 = context.SaveChanges() > 0;

                }
            }
            return EnviarResultado(result.Succeeded, "Crear Permiso", result.Succeeded ? "Se creo Satisfactoriamente" : "Error al crear el Permiso: " + result.Errors.FirstOrDefault());
        }


        [HttpGet]
        public ActionResult Editar(string Id)
        {
            using (var context = new OrionSecurityEntities())
            {
                var role = context.AspNetRoles.Find(Id);
                return PartialView("Crear", new CrearPermisoViewModel { Id = role.Id, Nombre = role.Name, EsEditar = true, Estado = true });
            }
        }

        [HttpPost]
        public ActionResult Editar(CrearPermisoViewModel model)
        {
            using (var context = new OrionSecurityEntities())
            {
                var rol = context.AspNetRoles.Find(model.Id);
                rol.Name = model.Nombre;
                context.Entry(rol).State = System.Data.Entity.EntityState.Modified;
                var result = context.SaveChanges() > 0;
                return EnviarResultado(result, "Editar Permiso", result ? "Se edito Satisfactoriamente" : "Error al editar ");
            }
        }


        public ActionResult EliminarPermiso(string id)
        {
            using (var context = new OrionSecurityEntities())
            {

                var model = context.AspNetRoles.Find(id);

                var listaPermisosPorRol = context.PrivilegiosPorRol.Where(x => x.Fk_IdPermiso == id);
                context.PrivilegiosPorRol.RemoveRange(listaPermisosPorRol);

                context.AspNetRoles.Remove(model);

                return EnviarResultado(context.SaveChanges() > 0, "Eliminar Permiso");
            }
        }

    }
}