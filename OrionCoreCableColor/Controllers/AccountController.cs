using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.App_Services.EmailService;
using OrionCoreCableColor.DbConnection.CMContext;
using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models;
using OrionCoreCableColor.Models.Account;
using OrionCoreCableColor.Models.Base;
using OrionCoreCableColor.Models.EmailTemplateService;
using OrionCoreCableColor.Models.Usuario;

namespace OrionCoreCableColor.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        public ActionResult RedireccionarSaris()
        {
            var usuario = GetUser();
            //string ticketSystemUrl = "https://localhost:7291/Account/IngresarOrion?fiIdUsuario=" + usuario.fiIdUsuario;
            string ticketSystemUrl = "https://saris.novanetgroup.com/Account/IngresarOrion?fiIdUsuario=" + usuario.fiIdUsuario;
            return Redirect(ticketSystemUrl);
        }

        [AllowAnonymous]
        public ActionResult IngresarSaris(int fiIdUsuario)
        {

            return RedirectToAction("Index", "Home");
        }
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
      
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null) { ModelState.AddModelError("", "Usuario Invalido"); return View(model); } //usuario invalido

            using (var context = new OrionSecurityEntities())
            {
                var usuario = context.Usuarios.FirstOrDefault(x => x.AspNetUsers.UserName == model.Email);
                if (usuario == null || usuario.fiEstado == 0) { ModelState.AddModelError("", "Usuario sin acceso"); return View(model); } //usuario no activo 
                //role: coreseguridad_AccesoAlSistema


            }



            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);


            //FormsAuthentication.SetAuthCookie("CookieValue", false);
            switch (result)
            {

                case SignInStatus.Success:
                    EscribirEnLogJson(new NotificacionViewModel { fcOperacion = $"USUARIO {model.Email} HA INICIADO SESSION", fcClase = "info", fcTipoTransaccion = "Inicio de sesion" });
                    return RedirectToLocal(returnUrl);
                // return RedirectToAction("Index", "Home");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Error al Iniciar sesion. verificar correo y contraseña.");
                    return View(model);
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> LoginAPI(LoginViewModel model)
        {
            if (!ModelState.IsValid) return Json(new DatosUsuarioViewModel { Estado = false, Titulo = "Error", Mensaje = "Ingrese los datos" }, JsonRequestBehavior.AllowGet);
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null) { return Json(new DatosUsuarioViewModel { Estado = false, Titulo = "Error", Mensaje = "Usuario Invalido" }, JsonRequestBehavior.AllowGet); } //usuario invalido

            using (var context = new OrionSecurityEntities())
            {
                var usuario = context.Usuarios.FirstOrDefault(x => x.AspNetUsers.UserName == model.Email);
                if (usuario == null || usuario.fiEstado == 0) { return Json(new DatosUsuarioViewModel { Estado = false, Titulo = "Error", Mensaje = "Usuario Sin Acceso" }, JsonRequestBehavior.AllowGet); } //usuario no activo 
                //role: coreseguridad_AccesoAlSistema


            }



            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);


            switch (result)
            {

                case SignInStatus.Success:
                    using (var contexto = new ORIONDBEntities())
                    {
                        var modelDb = contexto.sp_ASPNETUSER_ObtenerInformacionUsuario(model.Email).FirstOrDefault();
                        var modelUsuario = new DatosUsuarioViewModel
                        {
                            Estado = true,
                            Mensaje = "Usuario Ingresado Exitosamente",
                            Titulo = "",
                            fiIDUsuario = modelDb.fiIDUsuario,
                            fcNombreCorto = modelDb.fcNombreCorto,
                            fcBuzonDeCorreo = modelDb.fcBuzondeCorreo,
                            fcEmpresa = modelDb.fcNombreComercial,
                            fiIDEmpresa = modelDb.fiIDEmpresa ?? 0,
                            fiIDRol = modelDb.Pk_IdRol,
                            Role = modelDb.Role,
                            Permisos = (modelDb.Permisos ?? "").Split(';', (char)StringSplitOptions.RemoveEmptyEntries).ToList(),
                            UsuarioCifrado = HttpUtility.UrlEncode(DataCrypt.Encriptar($"EMAIL={model.Email}&LOGUEADO=1&PASSWORD={model.Password}")),
                            fiIDUbicacion = modelDb.fiIDUbicacion ?? 0
                        };

                        EscribirEnLogJson(new NotificacionViewModel { fcOperacion = $"USUARIO {model.Email} HA INICIADO SESSION DESDE APLICACION", fcClase = "info", fcTipoTransaccion = "Inicio de sesion" });

                        return Json(modelUsuario, JsonRequestBehavior.AllowGet);
                    }
                // return RedirectToAction("Index", "Home");
                case SignInStatus.LockedOut:
                    return Json(new DatosUsuarioViewModel { Estado = false, Titulo = "Error", Mensaje = "Usuario Bloqueado" }, JsonRequestBehavior.AllowGet);

                case SignInStatus.RequiresVerification:
                    return Json(new DatosUsuarioViewModel { Estado = false, Titulo = "Error", Mensaje = "SendCode" }, JsonRequestBehavior.AllowGet);

                case SignInStatus.Failure:
                default:
                    return Json(new DatosUsuarioViewModel { Estado = false, Titulo = "Error", Mensaje = "Error al Iniciar sesion. verificar correo y contraseña." }, JsonRequestBehavior.AllowGet);


            }
        }
        //
        // GET: /Account/VerifyCode
        //[AllowAnonymous]
        //public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        //{
        //    // Requerir que el usuario haya iniciado sesión con nombre de usuario y contraseña o inicio de sesión externo
        //    if (!await SignInManager.HasBeenVerifiedAsync())
        //    {
        //        return View("Error");
        //    }
        //    return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/VerifyCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // El código siguiente protege de los ataques por fuerza bruta a los códigos de dos factores. 
        //    // Si un usuario introduce códigos incorrectos durante un intervalo especificado de tiempo, la cuenta del usuario 
        //    // se bloqueará durante un período de tiempo especificado. 
        //    // Puede configurar el bloqueo de la cuenta en IdentityConfig
        //    var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(model.ReturnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "Código no válido.");
        //            return View(model);
        //    }
        //}

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
                    // Para obtener más información sobre cómo habilitar la confirmación de cuentas y el restablecimiento de contraseña, visite https://go.microsoft.com/fwlink/?LinkID=320771
                    // Enviar correo electrónico con este vínculo
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirmar cuenta", "Para confirmar la cuenta, haga clic <a href=\"" + callbackUrl + "\">aquí</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(string nombreusuario)
        {
            try
            {
                using (var context = new OrionSecurityEntities())
                {
                    var usuario = context.Usuarios.Select(x => new ListaDeUsuariosViewModel
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
                        NombreRol = x.RolesPorUsuario.Any() ? x.RolesPorUsuario.FirstOrDefault().Roles.Nombre ?? "" : "",
                        fcNombreCorto = x.fcNombreCorto,
                        fcIdAspNetUser = x.fcIdAspNetUser
                    }).Where(a => a.fcNombreCorto == nombreusuario).FirstOrDefault();


                    if (usuario.fcBuzondeCorreo != "" || usuario.fcBuzondeCorreo != null)
                {
                    string randomString = GenerateRandomString(7);

                    var User = context.Usuarios.Find(usuario.fiIdUsuario);
                    string code = UserManager.GeneratePasswordResetToken(usuario.fcIdAspNetUser);

                    var result = UserManager.ResetPassword(usuario.fcIdAspNetUser, code, randomString);



                    MensajeriaApi.EnviarSMSPersonalizado(usuario.fcNombreCorto, usuario.fcTelefonoMovil, $"A solicitado una nueva contraseña, Su Contraseña nueva es *{randomString}* ");

                    var mode = new SendEmailViewModel();
                    mode.Subject = "Cambio de Contraseña";
                    //var _emailTemplateService = new EmailTemplateService();
                    //await _emailTemplateService.SendEmailPresonalizado(mode);
                    var _emailTemplateService = new EmailTemplateService();
                    await _emailTemplateService.SendEmailToPassword(new EmailTemplateNovanetModel
                    {
                        fcUsuario = usuario.fcNombreCorto,
                        fcCorreoElectronico = usuario.fcBuzondeCorreo,
                        fcNuevaContrasenia = randomString

                    });

                    return RedirectToAction("ForgotPasswordConfirmation", "Account");

                }
                else
                {
                    return EnviarResultado(false, "", "Usuario No Encontrado");
                }

                }

                

            }
            catch (Exception)
            {

                throw;
            }
            //if (ModelState.IsValid)
            //{
            //    var user = await UserManager.FindByNameAsync(model.Email);
            //    if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
            //    {
            //        // No revelar que el usuario no existe o que no está confirmado
            //        return View("ForgotPasswordConfirmation");
            //    }

                
            //}

        }

        [AllowAnonymous]
        public async Task<JsonResult> CambiarPasswordApi(string fcnombreusuario, string fcPasswordActual, string fcPasswordNueva)
        {
            try
            {
                using (var context = new OrionSecurityEntities())
                {
                    var usuario = context.Usuarios.Select(x => new ListaDeUsuariosViewModel
                    {
                        fiIdUsuario = x.fiIDUsuario,
                        fcPrimerNombre = x.fcPrimerNombre,
                        fcSegundoNombre = x.fcSegundoNombre,
                        fcSegundoApellido = x.fcSegundoApellido,
                        fcPrimerApellido = x.fcPrimerApellido,
                        fiEstado = x.fiEstado,
                        fcBuzondeCorreo = x.AspNetUsers.Email,
                        fcTelefonoMovil = x.fcTelefonoMovil,
                        UserName = x.AspNetUsers.UserName,
                        NombreRol = x.RolesPorUsuario.Any() ? x.RolesPorUsuario.FirstOrDefault().Roles.Nombre ?? "" : "",
                        fcNombreCorto = x.fcNombreCorto,
                        fcIdAspNetUser = x.fcIdAspNetUser,
                        fcPassword = x.fcPassword
                    }).Where(a => a.fcNombreCorto == fcnombreusuario).FirstOrDefault();

                    if (usuario == null)
                    {
                        return EnviarListaJson(new { fiCodeStatus = 404, fcMessageStatus = "Usuario no encontrado", fjValorDevuelto = "" });
                    }

                    // Usar ChangePasswordAsync para validar la contraseña actual y cambiarla
                    var user = await UserManager.FindByIdAsync(usuario.fcIdAspNetUser);
                    if (user == null)
                    {
                        return EnviarListaJson(new { fiCodeStatus = 404, fcMessageStatus = "Usuario no encontrado en Identity", fjValorDevuelto = "" });
                    }

                    var result = await UserManager.ChangePasswordAsync(usuario.fcIdAspNetUser, fcPasswordActual, fcPasswordNueva);
                    if (result.Succeeded)
                    {
                        return EnviarListaJson(new { fiCodeStatus = 200, fcMessageStatus = "Contraseña Cambiada Exitosamente", fjValorDevuelto = JsonConvert.SerializeObject(usuario) });
                    }
                    else
                    {
                        var errors = string.Join(", ", result.Errors);
                        return EnviarListaJson(new { fiCodeStatus = 400, fcMessageStatus = $"Error al cambiar la contraseña: {errors}", fjValorDevuelto = "" });
                    }
                }
            }
            catch (Exception ex)
            {
                return EnviarListaJson(new { fiCodeStatus = 500, fcMessageStatus = $"Error interno: {ex.Message}", fjValorDevuelto = "" });
            }
        }


        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // No revelar que el usuario no existe
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Solicitar redireccionamiento al proveedor de inicio de sesión externo
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generar el token y enviarlo
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        //[AllowAnonymous]
        //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    // Si el usuario ya tiene un inicio de sesión, iniciar sesión del usuario con este proveedor de inicio de sesión externo
        //    var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(returnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
        //        case SignInStatus.Failure:
        //        default:
        //            // Si el usuario no tiene ninguna cuenta, solicitar que cree una
        //            ViewBag.ReturnUrl = returnUrl;
        //            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        //            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
        //    }
        //}

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Obtener datos del usuario del proveedor de inicio de sesión externo
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        
        public ActionResult LogOff()
        {
            EscribirEnLogJson(new NotificacionViewModel { fcOperacion = $"USUARIO {User?.Identity?.Name ?? "sistembot"} SE HA DESLOGUEADO", fcClase = "info", fcTipoTransaccion = "Cierre de sesion" });
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if((User?.Identity?.Name ?? "") != "")
                {
                    EscribirEnLogJson(new NotificacionViewModel { fcOperacion = $"USUARIO {User?.Identity?.Name ?? "sistembot"} SE HA PERDIDO LA SESSION", fcClase = "info", fcTipoTransaccion = "Cierre de sesion" });
                }
                
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> ExtLogin(string id)
        {
            Session.RemoveAll(); // removes all session value specific to this user
            Session.Clear();
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            string returnUrl = null;
            //EMAIL=''&LOGUEADO=''&PASSWORD=''
            var data = DataCrypt.Desencriptar(id).Split('&');
            var parametros = new Dictionary<string, string>();
            foreach (var parameter in data)
            {
                var parametro = parameter.Split('=');
                parametros.Add(parametro[0], parametro[1]);
            }
            var model = new LoginViewModel();
            model.Email = parametros["EMAIL"];
            model.RememberMe = Convert.ToBoolean(Convert.ToInt32(parametros["LOGUEADO"]));
            model.Password = parametros["PASSWORD"];
            if (!ModelState.IsValid) return View(model);
            var user = await UserManager.FindByNameAsync(model.Email);

            if (user == null) { ModelState.AddModelError("", "Usuario Invalido"); return View(model); } //usuario invalido

            using (var context = new OrionSecurityEntities())
            {
                var usuario = context.Usuarios.FirstOrDefault(x => x.AspNetUsers.UserName == model.Email);

                if (usuario == null || usuario.fiEstado == 0) { ModelState.AddModelError("", "Usuario sin acceso"); return View(model); } //usuario no activo 
            }





            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);


            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                // return RedirectToAction("Index", "Home");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Error al Iniciar sesion. verificar correo y contraseña.");
                    return View(model);
            }
        }

        #region Aplicaciones auxiliares
        // Se usa para la protección XSRF al agregar inicios de sesión externos
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            //_ = User.Identity.Name;
            //var userLogueado = GetUser();
            //userLogueado.Dispositivos.Add(Request.UserHostAddress);
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            

            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}