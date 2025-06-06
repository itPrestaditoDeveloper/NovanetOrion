using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.App_Services.EmailService;
using OrionCoreCableColor.DbConnection.CMContext;
using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models;
using OrionCoreCableColor.Models.DatosCliente;
using OrionCoreCableColor.Models.EmailTemplateService;
using OrionCoreCableColor.Models.Precalificado;
using OrionCoreCableColor.Models.Usuario;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.Controllers
{
    [System.Web.Mvc.Authorize(Roles = "coreseguridad_AccesoAlSistema, coreseguridad_AccesoSolicitudesTecnicos")]
    public class UsuarioController : BaseController
    {

        private ApplicationUserManager _userManager;
        public SendEmailService ServicioCorreo;
        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }
        public UsuarioController()
        {
            ServicioCorreo = new SendEmailService();
        }
        // GET: Usuario
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexContratista()
        {
            return View(GetIdUser());
        }
        [AllowAnonymous]
        public JsonResult CargarListaUsuarios()
        {
            try
            {
                using (var context = new OrionSecurityEntities())
                {
                    var jsonResult = Json(context.Usuarios.Select(x => new ListaDeUsuariosViewModel
                    {
                        fiIdUsuario = x.fiIDUsuario,
                        fcPrimerNombre = x.fcPrimerNombre,
                        fcSegundoNombre = x.fcSegundoNombre,
                        fcSegundoApellido = x.fcSegundoApellido,
                        fcPrimerApellido = x.fcPrimerApellido,
                        //FechaNacimiento = x.FechaNacimiento,
                        fiEstado = x.fiEstado,
                        fcBuzondeCorreo = x.AspNetUsers.Email,
                        fcTelefonoMovil = x.fcTelefonoMovil,
                        UserName = x.AspNetUsers.UserName,
                        NombreRol = x.RolesPorUsuario.Any() ? x.RolesPorUsuario.FirstOrDefault().Roles.Nombre ?? "" : ""
                    }).ToList(), JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = Int32.MaxValue;
                    return jsonResult;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }


        public JsonResult CargarListaTecnicos(int id)
        {
            try
            {
                using (var context = new OrionSecurityEntities())
                {
                    using (var contextOrionDb = new ORIONDBEntities())
                    {
                        var empresa = contextOrionDb.sp_ObtenerEmpresa_ByIdUsuario(id).FirstOrDefault();
                        var listaDeContratistas = contextOrionDb.sp_TecnicosPorContratista_GetByIdContratista(empresa).Select(x => x.fiIDUsuarioTecnico).ToList();
                        listaDeContratistas.Add(id);
                        var jsonResult = Json(context.Usuarios.Where(x => listaDeContratistas.Any(y => y == x.fiIDUsuario)).Select(x => new ListaDeUsuariosViewModel
                        {
                            fiIdUsuario = x.fiIDUsuario,
                            fcPrimerNombre = x.fcPrimerNombre,
                            fcSegundoNombre = x.fcSegundoNombre,
                            fcSegundoApellido = x.fcSegundoApellido,
                            fcPrimerApellido = x.fcPrimerApellido,
                            //FechaNacimiento = x.FechaNacimiento,
                            fiEstado = x.fiEstado,
                            fcBuzondeCorreo = x.AspNetUsers.Email,
                            fcTelefonoMovil = x.fcTelefonoMovil,
                            UserName = x.AspNetUsers.UserName,
                            NombreRol = x.RolesPorUsuario.Any() ? x.RolesPorUsuario.FirstOrDefault().Roles.Nombre ?? "" : ""
                        }).ToList(), JsonRequestBehavior.AllowGet);
                        jsonResult.MaxJsonLength = Int32.MaxValue;
                        return jsonResult;
                    }
                }


            }
            catch (Exception e)
            {
                return null;
            }
        }


        [HttpGet]
        public ActionResult CrearUsuario()
        {
            using (var contextOrion = new ORIONDBEntities())
            {


                using (var context = new OrionSecurityEntities())
                {
                    var usuarioLogueado = GetUser();
                    var rol = GetRol(usuarioLogueado.IdRol);
                    var permisos = GetPermisos(usuarioLogueado.IdRol);

                    if (rol == "Orion_Admin")
                    {
                        ViewBag.ListaEmpresas = contextOrion.sp_Empresas_Maestro_Listar().ToList().Select(x => new SelectListItem { Value = x.fiIDEmpresa.ToString(), Text = x.fcNombreComercial }).ToList();
                    }
                    else
                    {
                        ViewBag.ListaEmpresas = contextOrion.sp_Empresas_Maestro_Listar().Where(a => a.fiIDEmpresa == usuarioLogueado.fiIDEmpresa).ToList().Select(x => new SelectListItem { Value = x.fiIDEmpresa.ToString(), Text = x.fcNombreComercial }).ToList();
                    }
                    
                    ViewBag.ListaRoles = context.Roles.Where(x => x.Activo).Select(x => new SelectListItem { Value = x.Pk_IdRol.ToString(), Text = x.Nombre }).ToList();
                    ViewBag.ListaDistribuidores = contextOrion.sp_Orion_Ventas_Distribuidor_Listado().Where(x => !string.IsNullOrEmpty(x.fcNombreComercial)).Select(x => new SelectListItem { Value = x.fiIDDistribuidor.ToString(), Text = x.fcNombreComercial }).ToList();
                    ViewBag.Rol = rol;
                    if (rol == "Orion_Contratista")
                    {
                        ViewBag.ListaRoles = context.Roles.Where(x => x.Activo && x.Pk_IdRol == 3).Select(x => new SelectListItem { Value = x.Pk_IdRol.ToString(), Text = x.Nombre }).ToList();
                    }

                    return View(new CrearUsuarioViewModel { fdFechaAlta = DateTime.Now });
                }
            }
        }

        [HttpGet]
        public ActionResult CrearUsuarioAgente(int fiIDVendedorRepartidor)
        {
            using (var contextOrion = new ORIONDBEntities())
            using (var context = new OrionSecurityEntities())
            {


                var fiIDTipoVendedor = GetConfigurationJson<ConfigDistribuidorVendedor>("Orion_Ventas_Vendedor_Distribuidor");
                var rolesAsesor = GetConfiguracion<int>("Orion_Ventas_Asesor_Externo_Por_Distribuidor", ',');
                var rolesAsesorFreelancer = GetConfiguracion<int>("Orion_Ventas_Freelancer", ',');
                var informacionAgente = contextOrion.sp_OrionSolicitud_Repartidor_Listar(GetIdUser()).FirstOrDefault(x => x.fiIDVendedorRepartidor == fiIDVendedorRepartidor);

                foreach (var item in fiIDTipoVendedor)
                {
                    if (informacionAgente != null && informacionAgente.fiIDTipoVendedor == item.fiIDTipoVendedor)
                        ViewBag.ListaRoles = context.Roles.Where(x => x.Activo && rolesAsesor.Contains(x.Pk_IdRol)).Select(x => new SelectListItem { Value = x.Pk_IdRol.ToString(), Text = x.Nombre }).ToList();
                    else
                        ViewBag.ListaRoles = context.Roles.Where(x => x.Activo && rolesAsesorFreelancer.Contains(x.Pk_IdRol)).Select(x => new SelectListItem { Value = x.Pk_IdRol.ToString(), Text = x.Nombre }).ToList();
                }
                      
                var usuarioLogueado = GetUser();
                var rol = GetRol(usuarioLogueado.IdRol);
                var permisos = GetPermisos(usuarioLogueado.IdRol);
                var defaultPassword = GetConfiguracion<string>("PasswordDefectoDistribuidor");
                ViewBag.ListaEmpresas = contextOrion.sp_Empresas_Maestro_Listar().Select(x => new SelectListItem { Value = x.fiIDEmpresa.ToString(), Text = x.fcNombreComercial }).ToList();
               
                ViewBag.ListaDistribuidores = contextOrion.sp_Orion_Ventas_Distribuidor_Listado().Where(x => !string.IsNullOrEmpty(x.fcNombreComercial)).Select(x => new SelectListItem { Value = x.fiIDDistribuidor.ToString(), Text = x.fcNombreComercial }).ToList();
                var modelo = new CrearUsuarioViewModel
                {
                    fdFechaAlta = DateTime.Now,
                    fcDocumentoIdentificacion = informacionAgente.fcIdentidadVendedor,
                    fcTelefonoMovil = informacionAgente.fcNumeroVendedor,
                    fcBuzondeCorreo = informacionAgente.fcCorreo,
                    fcPassword = defaultPassword,
                    ConfirmPassword = defaultPassword,
                    fcPrimerNombre= informacionAgente.fcPrimerNombre,
                    fcSegundoNombre = informacionAgente.fcSegundoNombre,
                    fcPrimerApellido = informacionAgente.fcPrimerApellido,
                    fcSegundoApellido = informacionAgente.fcSegundoApellido,
                    UserName = informacionAgente.fcPrimerNombre + "." + informacionAgente.fcPrimerApellido,
                };
                return View("CrearUsuario", modelo);
            }
        }


        [HttpGet]
        public ActionResult CrearUsuarioDistribuidor(int fiIDDistribuidor)
        {
            using (var contextOrion = new ORIONDBEntities())
            using (var context = new OrionSecurityEntities())
            {
                var rolesAsesor = GetConfiguracion<int>("Orion_Admin_Ventas_Distribuidor", ',');
                var modelDb = context.Database.SqlQuery<ListDistribuidorViewModel>("EXEC sp_Orion_Ventas_Distribuidor_ListadobyId @p0", fiIDDistribuidor).FirstOrDefault();



                var usuarioLogueado = GetUser();
                var rol = GetRol(usuarioLogueado.IdRol);
                var permisos = GetPermisos(usuarioLogueado.IdRol);
                var defaultPassword = GetConfiguracion<string>("PasswordDefectoDistribuidor"); 
                ViewBag.ListaEmpresas = contextOrion.sp_Empresas_Maestro_Listar().Select(x => new SelectListItem { Value = x.fiIDEmpresa.ToString(), Text = x.fcNombreComercial }).ToList();
                ViewBag.ListaRoles = context.Roles.Where(x => x.Activo && rolesAsesor.Contains(x.Pk_IdRol)).Select(x => new SelectListItem { Value = x.Pk_IdRol.ToString(), Text = x.Nombre }).ToList();
                ViewBag.ListaDistribuidores = contextOrion.sp_Orion_Ventas_Distribuidor_Listado().Select(x => new SelectListItem { Value = x.fiIDDistribuidor.ToString(), Text = x.fcNombreComercial }).ToList();
                var modelo = new CrearUsuarioViewModel
                {
                    fcDocumentoIdentificacion = modelDb.fcIdentidadRepresentante,
                    fcTelefonoMovil = modelDb.fcTelefonoMovil,
                    fcBuzondeCorreo = modelDb.fcCorreoElectronico,
                    fcPassword = defaultPassword,
                    ConfirmPassword = defaultPassword,
                    fcPrimerNombre = modelDb.fcPrimerNombre,
                    fcSegundoNombre = modelDb.fcSegundoNombre,
                    fcPrimerApellido = modelDb.fcPrimerApellido,
                    fcSegundoApellido = modelDb.fcSegundoApellido,
                    UserName = modelDb.fcPrimerNombre + "." + modelDb.fcPrimerApellido,
                };
                return View("CrearUsuario", modelo);



            }
        }
        
        
        [HttpGet]
        [AllowAnonymous]
        public ActionResult AgregarUsuarioTecnico(int id)
        {
            using (var contextOrion = new ORIONDBEntities())
            {
                using (var context = new OrionSecurityEntities())
                {
                    var usuarioLogueado = GetUser();
                    var rol = GetRol(usuarioLogueado.IdRol);
                    var permisos = GetPermisos(usuarioLogueado.IdRol);
                    if (rol == "Orion_Admin")
                    {
                        ViewBag.ListaEmpresas = contextOrion.sp_Empresas_Maestro_Listar().ToList().Select(x => new SelectListItem { Value = x.fiIDEmpresa.ToString(), Text = x.fcNombreComercial }).ToList();
                    }
                    else
                    {
                        ViewBag.ListaEmpresas = contextOrion.sp_Empresas_Maestro_Listar().Where(a => a.fiIDEmpresa == usuarioLogueado.fiIDEmpresa).ToList().Select(x => new SelectListItem { Value = x.fiIDEmpresa.ToString(), Text = x.fcNombreComercial }).ToList();
                    }

                    ViewBag.ListaRoles = context.Roles.Where(x => x.Activo && x.Pk_IdRol == 3).Select(x => new SelectListItem { Value = x.Pk_IdRol.ToString(), Text = x.Nombre }).ToList();
                    return View("CrearUsuario", new CrearUsuarioViewModel { fdFechaAlta = DateTime.Now, fbTecnico = true, fiIdUsuario = id, });
                }
            }

        }


        public async Task<ActionResult> EnviarCorreodeAccesoOrionAgenteExterno(string destinationEmail, string userName, string passwordHash, string nombreAgente)
        {
            try
            {
                var modelCorreo = new SendEmailViewModel
                {
                    DestinationEmail = destinationEmail,
                    Subject = "Bienvenido(a) a la plataforma Orion - Credenciales de acceso",
                    Body = $"Estimado(a) {nombreAgente},<br /><br />" +
                           $"A continuación, le enviamos sus credenciales para acceder a la plataforma Orion:<br /><br />" +
                           $"<strong>Usuario:</strong> {userName}<br />" +
                           $"<strong>Contraseña:</strong> {passwordHash}<br /><br />" +
                           $"<strong>Enlace a la plataforma:</strong> <a href='{MemoryLoadManager.orionUrl}'>{MemoryLoadManager.orionUrl}</a><br />" +
                           $"Por favor, cambie su contraseña al iniciar sesión por primera vez.<br /><br />" +
                           $"Atentamente,<br />Equipo de Novanet",
                    EmailName = "Novanet"
                };

                await ServicioCorreo.SendEmailAsync(modelCorreo);

                return Json(new { success = true, message = "Correo enviado correctamente" });
            }
            catch (Exception e)
            {
                return EnviarException(e, "Error al enviar el correo");
            }
        }

        [HttpPost]
        public async Task<ActionResult> CrearUsuario(CrearUsuarioViewModel model)
        {
            try
            {
                using (var context = new OrionSecurityEntities())
                {
                    var user = new ApplicationUser { UserName = model.UserName.Trim(), Email = model.fcBuzondeCorreo.Trim() };
                    var result = await UserManager.CreateAsync(user, model.fcPassword.Trim());
                    var usuario = context.AspNetUsers.FirstOrDefault(x => x.UserName == model.UserName);
                
                    if (result.Succeeded)
                    {

                        var nuevoUsuario = context.Usuarios.Add(new Usuarios_Maestro
                        {
                            fcPrimerNombre = model.fcPrimerNombre.Trim(),
                            fcSegundoNombre = model.fcSegundoNombre.Trim(),
                            fcPrimerApellido = model.fcPrimerApellido.Trim(),
                            fcSegundoApellido = model.fcSegundoApellido.Trim(),
                            //FechaNacimiento = model.FechaNacimiento,
                            fcNombreCorto = model.UserName,
                            fcCentrodeCosto = "0100",
                            fiIDPuesto = 1,
                            fiTipoUsuario = 1,
                            fiIDDepartamento = 1,
                            fiIDJefeInmediato = 1,
                            fcUsuarioDominio = model.UserName,
                            fiIDDominioRed = 1,
                            fcPassword = usuario.PasswordHash,
                            fcPasswordToken = user.PasswordHash,
                            fdFechaUltimoCambiodePassword = DateTime.Now,
                            fcBuzondeCorreo = model.fcBuzondeCorreo.Trim(),
                            fiIDDominioCorreo = 1,
                            fcDireccionFisica = "direccion",
                            fcDocumentoIdentificacion = model.fcDocumentoIdentificacion,
                            fdFechaAlta = DateTime.Now,
                            fiIDUsuarioSolicitante = 1,
                            fiIDUsuarioCreador = 1,
                            fiEstado = 1,
                            fdFechaBaja = null,
                            fcTelefonoMovil = model.fcTelefonoMovil,
                            fcToken = Guid.NewGuid().ToString(),
                            fcIdAspNetUser = usuario.Id,
                            fiIDEmpresa = model.fiIDEmpresa,
                            fiIDDistribuidor = model.fiIDDistribuidor
                        });

                        nuevoUsuario.RolesPorUsuario.Add(new RolesPorUsuario
                        {
                            Fk_IdRol = model.IdRol
                        });

                        var ListaPermisos = context.PrivilegiosPorRol.Where(x => x.Fk_IdRol == model.IdRol).Select(z => z.AspNetRoles.Name).ToList();
                        foreach (var permiso in ListaPermisos)
                        {
                            await UserManager.AddToRoleAsync(user.Id, permiso);
                        }

                        var resultado = context.SaveChanges() > 0;
                        using (var contexto = new ORIONDBEntities())
                        {
                            contexto.sp_Catalogo_Ubicaciones_CrearUbicacionPorUsuario(nuevoUsuario.fiIDUsuario, GetIdUser());
                        }
                        
                        await EnviarCorreodeAccesoOrionAgenteExterno(model.fcBuzondeCorreo, model.UserName, model.fcPassword.Trim(), (model.fcPrimerNombre.Trim() + " " + model.fcPrimerApellido.Trim()) );
                        MemoryLoadManager.ActualizarListaUsuarios();
                        return EnviarResultado(resultado, "Crear Usuario");
                    }
                    else
                    {
                        return EnviarResultado(result.Succeeded, result.Errors.FirstOrDefault());

                    }
                }
            }
            catch (Exception e)
            {
                return EnviarResultado(false, e.Message);
            }
        }

  


        [AllowAnonymous]
        public async Task<JsonResult> CrearUsuarioApi(CrearUsuarioViewModel model)
        {
            try
            {
                using (var context = new OrionSecurityEntities())
                {
                    var user = new ApplicationUser
                    {
                        UserName = model.UserName.Trim(),
                        Email = model.fcBuzondeCorreo.Trim()
                    };

                    var result = await UserManager.CreateAsync(user, model.fcPassword.Trim());
                    var usuarioIdentity = context.AspNetUsers.FirstOrDefault(x => x.UserName == model.UserName);

                    if (!result.Succeeded || usuarioIdentity == null)
                    {
                        var errores = string.Join(", ", result.Errors);
                        return EnviarListaJson(new
                        {
                            fiCodeStatus = 400,
                            fcMessageStatus = $"Error al crear el usuario: {errores}",
                            fjValorDevuelto = ""
                        });
                    }

                    var nuevoUsuario = context.Usuarios.Add(new Usuarios_Maestro
                    {
                        fcPrimerNombre = model.fcPrimerNombre.Trim(),
                        fcSegundoNombre = model.fcSegundoNombre.Trim(),
                        fcPrimerApellido = model.fcPrimerApellido.Trim(),
                        fcSegundoApellido = model.fcSegundoApellido.Trim(),
                        fcNombreCorto = model.UserName,
                        fcCentrodeCosto = "0100",
                        fiIDPuesto = 1,
                        fiTipoUsuario = 1,
                        fiIDDepartamento = 1,
                        fiIDJefeInmediato = 1,
                        fcUsuarioDominio = model.UserName,
                        fiIDDominioRed = 1,
                        fcPassword = usuarioIdentity.PasswordHash,
                        fcPasswordToken = user.PasswordHash,
                        fdFechaUltimoCambiodePassword = DateTime.Now,
                        fcBuzondeCorreo = model.fcBuzondeCorreo.Trim(),
                        fiIDDominioCorreo = 1,
                        fcDireccionFisica = "direccion",
                        fcDocumentoIdentificacion = model.fcDocumentoIdentificacion,
                        fdFechaAlta = DateTime.Now,
                        fiIDUsuarioSolicitante = 1,
                        fiIDUsuarioCreador = 1,
                        fiEstado = 1,
                        fdFechaBaja = null,
                        fcTelefonoMovil = model.fcTelefonoMovil,
                        fcToken = Guid.NewGuid().ToString(),
                        fcIdAspNetUser = usuarioIdentity.Id,
                        fiIDEmpresa = (model.fiIDEmpresa == 0 || model.fiIDEmpresa == null) ? 7 : model.fiIDEmpresa
                    });

                    nuevoUsuario.RolesPorUsuario.Add(new RolesPorUsuario
                    {
                        Fk_IdRol = model.IdRol
                    });

                    var listaPermisos = context.PrivilegiosPorRol
                        .Where(x => x.Fk_IdRol == model.IdRol)
                        .Select(z => z.AspNetRoles.Name)
                        .ToList();

                    foreach (var permiso in listaPermisos)
                    {
                        await UserManager.AddToRoleAsync(user.Id, permiso);
                    }

                    var resultado = context.SaveChanges() > 0;

                    if (resultado)
                    {
                        using (var contexto = new ORIONDBEntities())
                        {
                            contexto.sp_Catalogo_Ubicaciones_CrearUbicacionPorUsuario(nuevoUsuario.fiIDUsuario, GetIdUser());
                        }

                        var usuarioFinal = new
                        {
                            nuevoUsuario.fiIDUsuario,
                            nuevoUsuario.fcPrimerNombre,
                            nuevoUsuario.fcSegundoNombre,
                            nuevoUsuario.fcPrimerApellido,
                            nuevoUsuario.fcSegundoApellido,
                            nuevoUsuario.fcNombreCorto,
                            nuevoUsuario.fcBuzondeCorreo,
                            nuevoUsuario.fcTelefonoMovil,
                            model.IdRol,
                            model.UserName,
                            model.fiIDEmpresa
                        };
                        MemoryLoadManager.ActualizarListaUsuarios();
                        return EnviarListaJson(new
                        {
                            fiCodeStatus = 200,
                            fcMessageStatus = "Usuario creado exitosamente",
                            fjValorDevuelto = JsonConvert.SerializeObject(usuarioFinal)
                        });
                    }
                    else
                    {
                        return EnviarListaJson(new
                        {
                            fiCodeStatus = 500,
                            fcMessageStatus = "Error al guardar en la base de datos",
                            fjValorDevuelto = ""
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return EnviarListaJson(new
                {
                    fiCodeStatus = 500,
                    fcMessageStatus = $"Error interno: {ex.Message}",
                    fjValorDevuelto = ""
                });
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> CrearTecnico(CrearUsuarioViewModel model)
        {
            try
            {
                using (var contextOrion = new ORIONDBEntities())
                {

                    model.fiIDEmpresa = contextOrion.sp_ObtenerEmpresa_ByIdUsuario(GetIdUser()).FirstOrDefault();
                    using (var context = new OrionSecurityEntities())
                    {
                        var user = new ApplicationUser { UserName = model.UserName.Trim(), Email = model.fcBuzondeCorreo.Trim() };
                        var result = await UserManager.CreateAsync(user, model.fcPassword.Trim());
                        var usuario = context.AspNetUsers.FirstOrDefault(x => x.UserName == model.UserName);
                        if (result.Succeeded)
                        {

                            var nuevoUsuario = context.Usuarios.Add(new Usuarios_Maestro
                            {
                                fcPrimerNombre = model.fcPrimerNombre.Trim(),
                                fcSegundoNombre = model.fcSegundoNombre.Trim(),
                                fcPrimerApellido = model.fcPrimerApellido.Trim(),
                                fcSegundoApellido = model.fcSegundoApellido.Trim(),
                                //FechaNacimiento = model.FechaNacimiento,
                                fcNombreCorto = model.UserName,
                                fcCentrodeCosto = "0100",
                                fiIDPuesto = 1,
                                fiTipoUsuario = 1,
                                fiIDDepartamento = 1,
                                fiIDJefeInmediato = 1,
                                fcUsuarioDominio = model.UserName,
                                fiIDDominioRed = 1,
                                fcPassword = usuario.PasswordHash,
                                fcPasswordToken = user.PasswordHash,
                                fdFechaUltimoCambiodePassword = DateTime.Now,
                                fcBuzondeCorreo = model.fcBuzondeCorreo.Trim(),
                                fiIDDominioCorreo = 1,
                                fcDireccionFisica = "direccion",
                                fcDocumentoIdentificacion = model.fcDocumentoIdentificacion,
                                fdFechaAlta = DateTime.Now,
                                fiIDUsuarioSolicitante = 1,
                                fiIDUsuarioCreador = 1,
                                fiEstado = 1,
                                fdFechaBaja = null,
                                fcTelefonoMovil = model.fcTelefonoMovil,
                                fcToken = Guid.NewGuid().ToString(),
                                fcIdAspNetUser = usuario.Id,
                                fiIDEmpresa = model.fiIDEmpresa




                            });

                            nuevoUsuario.RolesPorUsuario.Add(new RolesPorUsuario
                            {
                                Fk_IdRol = model.IdRol
                            });

                            var ListaPermisos = context.PrivilegiosPorRol.Where(x => x.Fk_IdRol == model.IdRol).Select(z => z.AspNetRoles.Name).ToList();
                            foreach (var permiso in ListaPermisos)
                            {
                                await UserManager.AddToRoleAsync(user.Id, permiso);
                            }

                            var resultado = context.SaveChanges() > 0;

                            using (var contexto = new ORIONDBEntities())
                            {
                                contexto.sp_Catalogo_Ubicaciones_CrearUbicacionPorUsuario(nuevoUsuario.fiIDUsuario, GetIdUser());
                            }
                            using (var contexto = new ORIONDBEntities())
                            {
                                contexto.sp_TecnicosPorContratista_Crear(model.fiIdUsuario, nuevoUsuario.fiIDUsuario, GetIdUser());
                            }
                            MemoryLoadManager.ActualizarListaUsuarios();
                            return EnviarResultado(resultado, "Crear Usuario");
                        }
                        else
                        {
                            return EnviarResultado(result.Succeeded, result.Errors.FirstOrDefault());

                        }
                    }
                }
            }
            catch (Exception e)
            {
                return EnviarResultado(false, e.Message);
            }
        }




        [HttpGet]
        public ActionResult EditarInfoUsuario(int Id)
        {
            using (var context = new OrionSecurityEntities())
            {

                var usuario = context.Usuarios.Find(Id);
                return PartialView(new CrearUsuarioViewModel
                {
                    fiIdUsuario = usuario.fiIDUsuario,
                    fcPrimerNombre = usuario.fcPrimerNombre,
                    fcSegundoNombre = usuario.fcSegundoNombre,
                    fcPrimerApellido = usuario.fcPrimerApellido,
                    fcSegundoApellido = usuario.fcSegundoApellido,
                    //FechaNacimiento = usuario.FechaNacimiento,
                    fcTelefonoMovil = usuario.fcTelefonoMovil,
                });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult EditarInfoUsuario(CrearUsuarioViewModel model)
        {
            using (var context = new OrionSecurityEntities())
            {
                var usuario = context.Usuarios.Find(model.fiIdUsuario);
                usuario.fcPrimerNombre = model.fcPrimerNombre;
                usuario.fcSegundoNombre = model.fcSegundoNombre;
                usuario.fcPrimerApellido = model.fcPrimerApellido;
                usuario.fcSegundoApellido = model.fcSegundoApellido;
                usuario.fcTelefonoMovil = model.fcTelefonoMovil;
                context.Entry(usuario).State = EntityState.Modified;
                var result = context.SaveChanges() > 0;
                return EnviarResultado(result, "Editar Usuario", result ? "Se edito Satisfactoriamente" : "Error al editar el usuario");
            }
        }



        [HttpGet]
        public async Task<ActionResult> EditarCuentaUsuario(int Id)
        {
            using (var context = new OrionSecurityEntities())
            {
                ViewBag.ListaRoles = context.Roles.Where(x => x.Activo).Select(x => new SelectListItem { Value = x.Pk_IdRol.ToString(), Text = x.Nombre }).ToList();

                var usuario = context.Usuarios.Find(Id);
                var roles = await UserManager.GetRolesAsync(usuario.fcIdAspNetUser);
                return PartialView(new CrearUsuarioViewModel
                {
                    fiIdUsuario = usuario.fiIDUsuario,
                    UserName = usuario.AspNetUsers.UserName,
                    fcBuzondeCorreo = usuario.fcBuzondeCorreo,
                    fcIdAspNetUser = usuario.fcIdAspNetUser,
                    IdRol = usuario.RolesPorUsuario.FirstOrDefault()?.Fk_IdRol ?? 0,
                    fiEstado = usuario.fiEstado
                });

            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> EditarCuentaUsuario(CrearUsuarioViewModel model)
        {
            try
            {
                using (var context = new OrionSecurityEntities())
                {
                    var usuario = context.Usuarios.Find(model.fiIdUsuario);

                    if (context.AspNetUsers.Any(x => x.UserName == model.fcBuzondeCorreo && x.Id != usuario.fcIdAspNetUser))
                    {
                        return EnviarResultado(false, "Editar Usuario", "El nombre de usuario ya existe.");
                    }

                    usuario.AspNetUsers.UserName = model.UserName;
                    usuario.fcBuzondeCorreo = model.fcBuzondeCorreo;
                    usuario.AspNetUsers.Email = model.fcBuzondeCorreo;
                    usuario.fiEstado = model.fiEstado;
                    context.Entry(usuario).State = System.Data.Entity.EntityState.Modified;

                    //roles
                    var listaRolesPorEliminar = usuario.RolesPorUsuario.ToList();
                    context.RolesPorUsuario.RemoveRange(listaRolesPorEliminar);

                    usuario.RolesPorUsuario.Add(new RolesPorUsuario
                    {
                        Fk_IdRol = model.IdRol,
                    });

                    //aspnetroles
                    var roles = await UserManager.GetRolesAsync(usuario.AspNetUsers.Id);
                    await UserManager.RemoveFromRolesAsync(usuario.AspNetUsers.Id, roles.ToArray());

                    var ListaPermisos = context.PrivilegiosPorRol.Where(x => x.Fk_IdRol == model.IdRol).Select(z => z.AspNetRoles.Name).ToList();
                    foreach (var permiso in ListaPermisos)
                    {
                        await UserManager.AddToRoleAsync(usuario.AspNetUsers.Id, permiso);
                    }

                    var result = context.SaveChanges() > 0;
                    return EnviarResultado(result, "Editar Cuenta Usuario", result /*&& result2.Succeeded*/ ? "Se edito Satisfactoriamente" : "Error al editar el usuario");

                }
            }
            catch (Exception e)
            {
                return EnviarException(e, "Editar Usuario");

            }

        }


        [HttpGet]
        public ActionResult CambiarContrasenaUsuario(int Id)
        {
            using (var context = new OrionSecurityEntities())
            {
                return PartialView(new CambiarContraseñaUsuarioViewModel { Id = Id });
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> CambiarContrasenaUsuario(CambiarContraseñaUsuarioViewModel model)
        {
            try
            {
                using (var context = new OrionSecurityEntities())
                {
                    var User = context.Usuarios.Find(model.Id);
                    string code = await UserManager.GeneratePasswordResetTokenAsync(User.fcIdAspNetUser);
                    var result = await UserManager.ResetPasswordAsync(User.fcIdAspNetUser, code, model.NewPassword);
                    return EnviarResultado(result.Succeeded, "Cambiar Contrasena", result.Succeeded ? "Se cambio Satisfactoriamente" : "Error al cambiar la contrasena");
                }
            }
            catch (Exception e)
            {
                return EnviarException(e, "Cambiar Contraseña");
            }

        }



        [HttpPost]
        [AllowAnonymous]
        public ActionResult DeshabilitarUsuario(int Id)
        {
            using (var context = new OrionSecurityEntities())
            {

                var usuario = context.Usuarios.Find(Id);
                usuario.fiEstado = usuario.fiEstado == 1 ? 0 : 1;
                var result = context.SaveChanges() > 0;
                return EnviarResultado(result, usuario.fiEstado == 0 ? "Habilitar Usuario" : "Deshabilitar Usuario", result ? "Modificado exitosamente" : "Error al modificar el usuario");

            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult DeshabilitarUsuarios(int Id)
        {
            using (var context = new OrionSecurityEntities())
            {

                var usuario = context.Usuarios.Find(Id);
                usuario.fiEstado = usuario.fiEstado == 1 ? 0 : 1;
                var result = context.SaveChanges() > 0;
                return EnviarResultado(result, usuario.fiEstado == 0 ? "Habilitar Usuario" : "Deshabilitar Usuario", result ? "Modificado exitosamente" : "Error al modificar el usuario");

            }
        }

        [HttpGet]
        public void GetGeolocalizacionUsuario(int fiIDUsuario)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var usuario = contexto.sp_Usuarios_Maestros_ObtenerPorId(fiIDUsuario).FirstOrDefault();
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
                var model = new { usuarioPeticion = User.Identity.Name, usuarioGeolocalizado = usuario.fcNombreCorto };
                hubContext.Clients.All.getGeolocalizacionUsuario(model);
            }

        }

        [HttpGet]
        public JsonResult RetornoGeolocalizacionUsuario(string usuarioPeticion, string latitud, string longitud)
        {


            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            var model = new { latitud, longitud, usuarioPeticion };
            hubContext.Clients.All.mostrarGeolocalizacion(model);

            return Json(new { esUsuario = usuarioPeticion == User.Identity.Name, latitud, longitud }, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult MensajeDirecto(MensajeDirectoViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                model.fiIDUsuarioMensajero = GetIdUser();
                model.fcUsuarioReceptor = contexto.sp_Usuarios_Maestros_ObtenerPorId(model.fiIDUsuarioReceptor).FirstOrDefault().fcNombreCorto;
                return PartialView(model);
            }

        }

        [AllowAnonymous]
        [HttpPost]
        public void EnviarMensajeDirecto(MensajeDirectoViewModel model)
        {
            var mensaje = new MensajeDirectoViewModel();
            if (string.IsNullOrEmpty(model.fcMensajeEnviado))
            {
                mensaje = new MensajeDirectoViewModel
                {
                    fiIDUsuarioMensajero = model.fiIDUsuarioMensajero,
                    fcMensajeEnviado = model.fcMensaje,
                    fcUsuarioMensajero = model.fcUsuarioMensajero,
                    fiIDUsuarioReceptor = model.fiIDUsuarioReceptor,
                    fcUsuarioReceptor = model.fcUsuarioReceptor
                };
                EscribirEnLog($"Se envio mensaje [{ model.fcMensaje }] al usuario: {model.fcUsuarioReceptor}");
            }
            else
            {
                mensaje = new MensajeDirectoViewModel
                {
                    fiIDUsuarioMensajero = model.fiIDUsuarioReceptor,
                    fcMensajeEnviado = model.fcMensaje,
                    fcUsuarioMensajero = model.fcUsuarioReceptor,
                    fiIDUsuarioReceptor = model.fiIDUsuarioMensajero,
                    fcUsuarioReceptor = model.fcUsuarioMensajero
                };
                EscribirEnLog($"Se envio mensaje [{ model.fcMensaje }] al usuario: {model.fcUsuarioMensajero}");
            }

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.mostrarMensajeEnviado(mensaje);

        }


        public ActionResult PerfilUsuario()
        {
            return View();
        }

    }
}