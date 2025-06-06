using Microsoft.AspNet.Identity.Owin;
using OrionCoreCableColor.DbConnection.CMContext;
using OrionCoreCableColor.Models.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.Controllers
{
    [Authorize]
    public class RoleController : BaseController
    {
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }
        // GET: Role
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Listar()
        {
            using (var context = new OrionSecurityEntities())
            {
                var jsonResult = Json(context.Roles.Where(z => z.Activo).Select(x => new CrearRolViewModel { Pk_IdRol = x.Pk_IdRol, Nombre = x.Nombre, Observacion = x.Observacion, Activo = x.Activo }).ToList(), JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
        }


        [HttpGet]
        public ActionResult Crear()
        {
            return PartialView(new CrearRolViewModel());
        }

        [HttpPost]
        public ActionResult Crear(CrearRolViewModel model)
        {

            using (var context = new OrionSecurityEntities())
            {
                var newModel = context.Roles.Add(new Roles
                {
                    Nombre = model.Nombre,
                    Activo = true,
                    Observacion = model.Observacion
                });

                return EnviarResultado(context.SaveChanges() > 0, "Crear Rol");
            }

        }

        [HttpGet]
        public ActionResult Editar(int id)
        {
            using (var context = new OrionSecurityEntities())
            {
                var role = context.Roles.Find(id);
                return PartialView("Crear", new CrearRolViewModel { Pk_IdRol = role.Pk_IdRol, Nombre = role.Nombre, Observacion = role.Observacion, EsEditar = true, Activo = true });
            }
        }

        [HttpPost]
        public ActionResult Editar(CrearRolViewModel model)
        {
            using (var context = new OrionSecurityEntities())
            {
                var rol = context.Roles.Find(model.Pk_IdRol);
                rol.Nombre = model.Nombre;
                rol.Observacion = model.Observacion;
                context.Entry(rol).State = System.Data.Entity.EntityState.Modified;
                var result = context.SaveChanges() > 0;
                return EnviarResultado(result, "Editar Rol", result ? "Se edito Satisfactoriamente" : "Error al editar ");
            }
        }


        public ActionResult EliminarRol(int id)
        {
            using (var context = new OrionSecurityEntities())
            {
                var model = context.Roles.Find(id);
                model.Activo = false;

                return EnviarResultado(context.SaveChanges() > 0, "Eliminar Rol");
            }
        }


        public ActionResult AsignarPermisos(int id, string Nombre)
        {
            ViewBag.Id = id;
            ViewBag.Nombre = Nombre;
            return PartialView();
        }


        [HttpGet]
        public JsonResult GetListaSeleccionPermisosPorRol(int id)
        {
            using (var context = new OrionSecurityEntities())
            {
                var ListaProveedor = context.AspNetRoles.Where(z => z.Activo).Select(x => new ListaPermisosPorRolViewModel
                {
                    Nombre = x.Name,
                    Id = x.Id,
                    Seleccionado = x.PrivilegiosPorRol.Any(z => z.Fk_IdRol == id)
                }).ToList();
                var jsonResult = Json(ListaProveedor, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = Int32.MaxValue;
                return jsonResult;
            }
        }


        [HttpPost]
        public async Task<ActionResult> GuardarSeleccionPermisosPorRol(int Id, List<ListaPermisosPorRolViewModel> ListaPermisos)
        {
            using (var context = new OrionSecurityEntities())
            {
                var ListaPermisosPorRol = context.PrivilegiosPorRol.Where(x => x.Fk_IdRol == Id).ToList();
                if (ListaPermisosPorRol.Any()) context.PrivilegiosPorRol.RemoveRange(ListaPermisosPorRol);

                if (ListaPermisos.Any(x => x.Seleccionado))
                    foreach (var detalle in ListaPermisos.Where(x => x.Seleccionado))
                    {
                        context.PrivilegiosPorRol.Add(new PrivilegiosPorRol
                        {
                            Fk_IdRol = Id,
                            Fk_IdPermiso = detalle.Id,
                        });

                    }

                await context.SaveChangesAsync();

                var listUsersWithRoleToEdit = context.Usuarios.Where(z => z.RolesPorUsuario.Any(x => x.Fk_IdRol == Id) && z.fiEstado == 1).ToList();
                var ListaPermisosAAgregar = context.PrivilegiosPorRol.Where(x => x.Fk_IdRol == Id).Select(z => z.AspNetRoles.Name).ToList();
                foreach (var usuario in listUsersWithRoleToEdit)
                {
                    //roles
                    var listaRolesPorEliminar = usuario.RolesPorUsuario.ToList();
                    context.RolesPorUsuario.RemoveRange(listaRolesPorEliminar);

                    usuario.RolesPorUsuario.Add(new RolesPorUsuario
                    {
                        Fk_IdRol = Id,
                    });

                    //aspnetroles
                    var roles = await UserManager.GetRolesAsync(usuario.AspNetUsers.Id);
                    await UserManager.RemoveFromRolesAsync(usuario.AspNetUsers.Id, roles.ToArray());


                    foreach (var nombrePermiso in ListaPermisosAAgregar)
                    {
                        await UserManager.AddToRoleAsync(usuario.AspNetUsers.Id, nombrePermiso);
                    }
                }


                await context.SaveChangesAsync();

                return EnviarResultado(true, "Seleccion de Permisos para Rol");
            }
        }

    }
}