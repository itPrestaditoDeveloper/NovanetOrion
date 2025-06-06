using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using NReco.PdfGenerator;
using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.App_Services.EmailService;
using OrionCoreCableColor.App_Services.ReportesService;
using OrionCoreCableColor.DbConnection.CMContext;
using OrionCoreCableColor.DbConnection.CoreAdministrativoDB;
using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models.Base;
using OrionCoreCableColor.Models.BaseCallCenter;
using OrionCoreCableColor.Models.Contabilidad;
using OrionCoreCableColor.Models.EmailTemplateService;
using OrionCoreCableColor.Models.Productos;
using OrionCoreCableColor.Models.Soporte;
using OrionCoreCableColor.Models.Usuario;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.Controllers
{
    [System.Web.Mvc.Authorize]
    //[ApiKeyAuthorize]
    public class BaseController : Controller
    {
        private DbServiceConnection _connection;
        public Dictionary<string, string> DictionaryList = new Dictionary<string, string>();
        private SendEmailService _emailService;

        public BaseController()
        {
            var ConnectionString = ConfigurationManager.ConnectionStrings["OrionConnections"].ConnectionString; // DataCrypt.Desencriptar(ConfigurationManager.ConnectionStrings["ConexionEncriptada"].ConnectionString);
            _connection = new DbServiceConnection(ConnectionString);
        }
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            

            //CultureInfo en = new CultureInfo("es-ES", true)
            //{
            //    NumberFormat = {
            //        NumberDecimalSeparator =".",
            //       CurrencyDecimalDigits=2,
            //       NumberDecimalDigits = 2,

            //       NumberGroupSeparator=",",
            //       CurrencyGroupSeparator=",",
            //       PerMilleSymbol=","

            //    },
            //};
            //Thread.CurrentThread.CurrentCulture = en;
            //Thread.CurrentThread.CurrentUICulture = en;



            return base.BeginExecuteCore(callback, state);
        }

        [AllowAnonymous]
        public void enviarMiUsuario(InfoUsuarioViewModel model)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.enviarMiUsuario(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult RegistrarUsuariosLogueados(InfoUsuarioViewModel model, string usuarioPeticion)
        {
            //var usuario = MemoryLoadManager.ListaUsuarios.FirstOrDefault(x => x.UserName == model.UserName);
            //if (model.UserName != User.Identity.Name)
            //{
            //    if (!usuario.InfoUsuario.Any(x => x.IP == model.IP && x.Navegador == model.Navegador && x.Navegador == model.Navegador))
            //    {
            //        usuario.InfoUsuario.Add(model);
            //    }
            //}

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.insertarUsuarios(model, usuarioPeticion);



            return Json(true, JsonRequestBehavior.AllowGet);
        }


        //[ApiKeyAuthorize]
        [AllowAnonymous]
        public void EnviarNotificacion(string mensajeNotificacion)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.enviarNotificacion(mensajeNotificacion);
            hubContext.Clients.All.recibirNotificacionLog($"Usuario: {GetIdUser()} | { (string.IsNullOrEmpty(User?.Identity?.Name ?? "") ? "sistembot" : User.Identity.Name) }, Operacion: {mensajeNotificacion}");
            hubContext.Clients.All.recibirNoficicacionJson(new NotificacionViewModel
            {
                fiIDUsuario = GetIdUser(),
                fdFechaTransaccion = DateTime.Now,
                fcUsuario = ((User?.Identity?.Name ?? "") == "") ? "sistembot" : User?.Identity?.Name ?? "sistembot",
                fcOperacion = mensajeNotificacion,
                fcClase = "success",
                fcTipoTransaccion = "Transaccion"
            });
        }

        [AllowAnonymous]
        public void EscribirEnLog(string mensaje)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.recibirNotificacionLog($"Usuario: {GetIdUser()} | {User?.Identity?.Name ?? "sistembot"}, Operacion: {mensaje}");
            hubContext.Clients.All.recibirNoficicacionJson(new NotificacionViewModel
            {
                fiIDUsuario = GetIdUser(),
                fdFechaTransaccion = DateTime.Now,
                fcUsuario = ((User?.Identity?.Name ?? "") == "") ? "sistembot" : User?.Identity?.Name ?? "sistembot",
                fcOperacion = mensaje,
                fcTipoTransaccion = "Transaccion",
                fcClase = "success"
            });
        }

        [AllowAnonymous]
        public void EscribirEnLogJson(string mensaje)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.recibirNoficicacionJson(new NotificacionViewModel
            {
                fiIDUsuario = GetIdUser(),
                fdFechaTransaccion = DateTime.Now,
                fcUsuario = ((User?.Identity?.Name ?? "") == "") ? "sistembot" : User?.Identity?.Name ?? "sistembot",
                fcOperacion = mensaje,
                fcTipoTransaccion = "Transaccion",
                fcClase = "success"
            });
        }

        [AllowAnonymous]
        public void EscribirEnLogJson(NotificacionViewModel model)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            model.fiIDUsuario = GetIdUser();
            model.fdFechaTransaccion = DateTime.Now;
            model.fcUsuario = ((User?.Identity?.Name ?? "") == "") ? "sistembot" : User?.Identity?.Name ?? "sistembot";
            hubContext.Clients.All.recibirNoficicacionJson(model);
        }

        [AllowAnonymous]
        public string EnviarFormularioSignalR<T>(ObjSignalRModalViewModel model) where T : class
        {
            try
            {
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
                hubContext.Clients.All.mostrarModal(new { model = (T)model.Obj, url = model.Url, user = model.InfoUsuario });
                return "Bueno";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public string EnviarFormularioSignalRStruct<T>(ObjSignalRModalViewModel model) where T : struct
        {
            try
            {
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
                hubContext.Clients.All.mostrarModal(new { model = model.Obj, url = model.Url, user = model.InfoUsuario });
                return "Bueno";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public JsonResult EnviarResultado(bool resultado, string Titulo)
        {

            EscribirEnLogJson(new NotificacionViewModel
            {
                fcOperacion = $"{Titulo}",
                fcClase = resultado ? "success" : "danger",
                fcTipoTransaccion = "Transaccion"
            });

            return Json(new MensajeRespuestaViewModel
            {
                Titulo = Titulo,
                Mensaje = resultado ? "Action Successful" : "Error",
                Estado = resultado

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult EnviarResultado(bool resultado, string Titulo, string Mensaje)
        {

            EscribirEnLogJson(new NotificacionViewModel
            {
                fcOperacion = $"{Titulo} | {Mensaje}",
                fcClase = resultado ? "success" : "danger",
                fcTipoTransaccion = "Transaccion"
            });

            return Json(new MensajeRespuestaViewModel
            {
                Titulo = Titulo,
                Mensaje = Mensaje,
                Estado = resultado,
            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult EnviarResultado(bool resultado, string Titulo, string Mensaje, int Id)
        {
            return Json(new MensajeRespuestaViewModel
            {
                Titulo = Titulo,
                Mensaje = Mensaje,
                Estado = resultado,
                Id = Id
            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult EnviarResultado(bool resultado, string Titulo, string Mensaje, string Correlativo)
        {
            return Json(new MensajeRespuestaViewModel
            {
                Titulo = Titulo,
                Mensaje = Mensaje,
                Estado = resultado,
                Correlativo = Correlativo
            }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult EnviarException(Exception e, string Titulo)
        {

            if (MemoryLoadManager.Produccion)
            {
                RegistrarError(e);
            }


            return Json(new MensajeRespuestaViewModel
            {
                Titulo = Titulo,
                Mensaje = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace,
                Estado = false,

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult EnviarListaJson(object e)
        {
            var jsonResult = Json(e, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;

        }

        public List<string> PreguntasAleatorias(List<string> lista, string opcionCorrecta, int tipoLista)
        {
            Random rand = new Random();
            //preguntas


            //si es 1 hacer randon de respuestas
            if (tipoLista == 1)
            {
                lista = lista.Where(x => x != opcionCorrecta).Select(x => x).ToList();
                var listaArreglo = lista.ToArray();
                int cantidaddeElementos = lista.Count;
                int numeroAlzar = rand.Next(1, cantidaddeElementos);
                int numeroAlzar2 = rand.Next(1, cantidaddeElementos);
                string opcion1AlAzar = listaArreglo[numeroAlzar];
                string opcion2AlAzar = listaArreglo[numeroAlzar2];
                while (numeroAlzar2 == numeroAlzar)
                {
                    numeroAlzar2 = rand.Next(1, cantidaddeElementos);
                    opcion2AlAzar = listaArreglo[numeroAlzar2];
                }

                var ListaRetornar = new List<string> { opcion1AlAzar, opcion2AlAzar, opcionCorrecta };

                ListaRetornar = ListaRetornar.OrderBy(x => rand.Next()).ToList();
                return ListaRetornar;
            }
            // sino solo retornar lista
            else
            {
                return lista;
            }


        }
        public string[] PreguntasATomar()
        {
            Random rand = new Random();
            List<int> preguntasAgregada = new List<int>();
            int numeroPregunta = rand.Next(1, 7);
            preguntasAgregada.Add(numeroPregunta);
            int numeroPregunta2 = rand.Next(1, 7);
            while (preguntasAgregada.Contains(numeroPregunta2))
            {
                numeroPregunta2 = rand.Next(1, 7);
            }
            preguntasAgregada.Add(numeroPregunta2);

            int numeroPregunta3 = rand.Next(1, 7);
            preguntasAgregada.Add(numeroPregunta3);
            while (preguntasAgregada.Contains(numeroPregunta3))
            {
                numeroPregunta3 = rand.Next(1, 7);
            }


            //while (numeroPregunta == numeroPregunta2)
            //{
            //    numeroPregunta2 = rand.Next(1, 6);
            //}
            //while (numeroPregunta2 == numeroPregunta3)
            //{
            //    numeroPregunta3 = rand.Next(1, 6);
            //}
            //while (numeroPregunta == numeroPregunta3)
            //{
            //    numeroPregunta3= rand.Next(1, 6);
            //}
            //while (numeroPregunta2 == numeroPregunta)
            //{
            //    numeroPregunta = rand.Next(1, 6);
            //}
            var ListadoSeleccionadoPreguntas = new List<string>() { "Vive", "Trabaja", "Parentesco", "Estado Civil", "Ciudad", "Tiene Hijos" }.ToArray();
            string pregunta1Seleccionada = ListadoSeleccionadoPreguntas[numeroPregunta - 1];
            string pregunta2Seleccionada = ListadoSeleccionadoPreguntas[numeroPregunta2 - 1];
            string pregunta3Seleccionada = ListadoSeleccionadoPreguntas[numeroPregunta3 - 1];

            return new List<string>() { pregunta1Seleccionada, pregunta2Seleccionada, pregunta3Seleccionada }.ToArray();



        }


        public List<int> ListaDias() //estoy seguro que Dennis hizo esto  XD
        {
            var lista = new List<int>();
            for (int i = 1; i <= 31; i++)
            {
                lista.Add(i);
            }
            return lista;
        }
        public List<int> ListaMeses() //estoy seguro que Dennis hizo esto  XD
        {
            var lista = new List<int>();
            for (int i = 1; i <= 12; i++)
            {
                lista.Add(i);
            }
            return lista;
        }
        public List<int> ListaAnio() //estoy seguro que Dennis hizo esto  XD
        {
            int anioActual = DateTime.Now.Year;
            int aniolimite = anioActual - 40;
            var lista = new List<int>();
            for (int i = anioActual; i >= aniolimite; i--)
            {
                lista.Add(i);
            }
            return lista;
        }


        public int GetIdUser()
        {
            using (var contexto = new ORIONDBEntities())
            {
                return contexto.sp_Usuarios_Maestros_ObtenerIdUsuario(User?.Identity?.Name ?? "").FirstOrDefault() ?? 20;
            }

        }

        public int GetIdUserForName(string nameUser)
        {
            using (var contexto = new ORIONDBEntities())
            {
                return contexto.sp_Usuarios_Maestros_ObtenerIdUsuario(nameUser ?? "").FirstOrDefault() ?? 20;
            }

        }

        public int GetIdContratista()
        {
            int contratista = 0;
            using (var context = new ORIONDBEntities())
            {
                contratista = context.sp_GetIdContratista_ById(GetIdUser()).FirstOrDefault() ?? 0;


            }
            return contratista;
        }
        public int GetIdTecnico()
        {
            int tecnico = 0;
            using (var context = new ORIONDBEntities())
            {
                tecnico = context.sp_GetIdTecnico_ById(GetIdUser()).FirstOrDefault() ?? 0;


            }
            return tecnico;
        }

        public ListaDeUsuariosViewModel GetUser()
        {
            return MemoryLoadManager.ListaUsuarios.FirstOrDefault(x => x.UserName == User.Identity.Name);

        }

        public async Task<ListaDeUsuariosViewModel> GetUserAsync()
        {
            return MemoryLoadManager.ListaUsuarios.FirstOrDefault(x => x.UserName == User.Identity.Name);

        }

        public string GetRol(int idRol)
        {
            using (var context = new OrionSecurityEntities())
            {
                return context.Roles.FirstOrDefault(x => x.Pk_IdRol == idRol).Nombre;
            }
        }

        public List<AspNetRoles> GetPermisos(int idRol)
        {
            using (var contexto = new OrionSecurityEntities())
            {
                return contexto.PrivilegiosPorRol.Where(x => x.Fk_IdRol == idRol).Select(x => x.AspNetRoles).ToList();
            }
        }


        public List<SelectListItem> GetListMarcas()
        {
            using (var contexto = new ORIONDBEntities())
            {
                return contexto.sp_Catalogo_Marcas_Listar().Select(x => new SelectListItem { Value = x.fiIDMarca.ToString(), Text = x.fcMarca }).ToList();
            }
        }

        public List<SelectListItem> GetListModelos()
        {
            using (var contexto = new ORIONDBEntities())
            {
                return contexto.sp_Catalogo_Modelos_Listar().Select(x => new SelectListItem { Value = x.fiIDModelo.ToString(), Text = x.fcModelo + " - " + x.fcMarca }).ToList();
            }
        }



        public List<SelectListItem> GetListUbicaciones()
        {
            using (var contexto = new ORIONDBEntities())
            {
                return contexto.sp_Catalogo_Ubicaciones_Listar().Where(x => x.fiEstadoUbicacion == 1).Select(x => new SelectListItem { Value = x.fiIDUbicacion.ToString(), Text = x.fcUbicacion }).ToList();
            }
        }

        public List<SelectListItem> GetListUbicacionesExternos(ListaDeUsuariosViewModel usuario)
        {
            using (var contexto = new ORIONDBEntities())
            {
                return contexto.sp_Catalogo_Ubicaciones_ListarExterno(usuario.fiIDEmpresa).Where(x => x.fiEstadoUbicacion == 1).Select(x => new SelectListItem { Value = x.fiIDUbicacion.ToString(), Text = x.fcUbicacion }).ToList();
            }
        }


        public List<SelectListItem> GetSolicitudesPorUbicacionParaSelect(int fiIDUbicacion)
        {
            using (var contexto = new ORIONDBEntities())
            {
                return contexto.sp_SolicitudesPorUbicacionCliente(fiIDUbicacion).Select(x => new SelectListItem { Value = x.fiIDSolicitud.ToString(), Text = x.fiIDSolicitud.ToString() }).ToList();
            }
        }


        public List<SelectListItem> GetListTipoProducto()
        {
            using (var contexto = new ORIONDBEntities())
            {
                return contexto.sp_Catalogo_TipoProducto_Listar().Select(x => new SelectListItem { Value = x.fiIDTipoProducto.ToString(), Text = x.fcTipoProducto }).ToList();
            }
        }

        public List<SelectListItem> GetListEmpresas()
        {
            using (var contexto = new ORIONDBEntities())
            {
                return contexto.sp_Empresas_Maestro_Listar().Select(x => new SelectListItem { Value = x.fiIDEmpresa.ToString(), Text = x.fcNombreComercial }).ToList();
            }
        }

        public List<SelectListItem> GetListUsuarios()
        {
            using (var contexto = new ORIONDBEntities())
            {
                return contexto.sp_Usuarios_Maestros_ListarUsuarios().Select(x => new SelectListItem { Value = x.fiIDUsuario.ToString(), Text = $"{x.fcNombreCorto} {x.fcNombreComercial}" }).ToList();
            }
        }



        [HttpPost]
        public JsonResult FormFileResponse()
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     para obtener los archivos guardados en el servidor
        /// </summary>
        /// <param name="tipo">img para imagenes y pdf para pdfs</param>
        /// <param name="carpeta"></param>
        /// <param name="nombreArchivo"></param>
        /// <returns></returns>
        public string GetContentDocument(string tipo, string carpeta, string nombreArchivo)
        {
            using (var client = new WebClient())
            {
                var uri = $@"{MemoryLoadManager.Helper}?type={tipo}&carpeta={carpeta}&documento={nombreArchivo}";
                //var content = client.DownloadData(uri);
                //return Convert.ToBase64String(content);
                return uri;
            }
        }

        /// <summary>
        ///     para subir documentos al server desde el local
        /// </summary>
        /// <param name="carpeta">primera y ultimo carecter nunca deben de ser \</param>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool UploadFileServer148(string carpeta, HttpPostedFileBase file)
        {

            using (var client = new HttpClient())
            {
                using (var content = new MultipartFormDataContent())
                {
                    try
                    {
                        content.Add(new StreamContent(file.InputStream), "documento", file.FileName);
                        var requestUri = $@"{MemoryLoadManager.Helper}?type=guardar&carpeta={carpeta}";
                        var result = client.PostAsync(requestUri, content).Result;

                        return true;
                    }
                    catch (Exception ex)
                    {

                        return false;
                    }
                }

            }


        }


        public bool _UploadFileServer148(string carpeta, HttpPostedFileBase file, string nombrePersonalizado)
        {
            using (var client = new HttpClient())
            {
                using (var content = new MultipartFormDataContent())
                {
                    try
                    {
                        content.Add(new StreamContent(file.InputStream), "documento", nombrePersonalizado);

                        var requestUri = $@"{MemoryLoadManager.Helper}?type=guardar&carpeta={carpeta}";
                        var result = client.PostAsync(requestUri, content).Result;

                        return result.IsSuccessStatusCode;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
        }


        public void ping()
        {
            //var algo = new Ping();
            var reply = new Ping().Send("190.6.197.90", 1000);
            //
        }

        public async Task<bool> UploadFileServer148Async(string carpeta, HttpPostedFileBase file)
        {

            using (var client = new HttpClient())
            {
                using (var content = new MultipartFormDataContent())
                {
                    try
                    {
                        content.Add(new StreamContent(file.InputStream), "documento", file.FileName);
                        var requestUri = $@"{MemoryLoadManager.Helper}?type=guardar&carpeta={carpeta}";
                        var result = await client.PostAsync(requestUri, content);


                        return true;
                    }
                    catch (Exception ex)
                    {

                        return false;
                    }
                }

            }


        }

        public void RegistrarError(Exception ex)
        {
            using (var log = new EventLog("Application"))
            {
                log.Source = "Application";
                log.WriteEntry((ex.InnerException?.Message ?? ex.Message) + ": " + ex.StackTrace, EventLogEntryType.Error, 101, 1);

                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
                hubContext.Clients.All.recibirNoficicacionJson(new NotificacionViewModel
                {
                    fiIDUsuario = GetIdUser(),
                    fdFechaTransaccion = DateTime.Now,
                    fcUsuario = ((User?.Identity?.Name ?? "") == "") ? "sistembot" : User?.Identity?.Name ?? "sistembot",
                    fcOperacion = (ex.InnerException?.Message ?? ex.Message) + ": " + ex.StackTrace,
                    fcClase = "danger",
                    fcTipoTransaccion = "ERROR"
                });


                var correo = new SendEmailService();
                correo.SendEmailExceptionWithOutAsync(ex, "Error de sistema");


            }

        }


        public List<SelectListItem> GetListTipoAfectacion()
        {
            return new List<SelectListItem> {
                new SelectListItem {
                    Value = "1",
                    Text = "AUMENTA INVENTARIO"
                },
                new SelectListItem {
                    Value = "-1",
                    Text = "REBAJA INVENTARIO"
                }
            };
        }


        public List<SelectListItem> GetListTiposMovimientos()
        {
            using (var contexto = new ORIONDBEntities())
            {
                return contexto.sp_Catalogo_TipoMovimiento_Listar().Select(x => new SelectListItem { Value = x.fiIDTipoMovimiento.ToString(), Text = x.fcTipoMovimiento }).ToList();
            }
        }

        public List<SelectListItem> GetListProductos()
        {
            using (var contexto = new ORIONDBEntities())
            {
                return contexto.sp_Producto_Maestro_Listar().Where(x => x.fiEstadoProducto == 1).Select(x => new SelectListItem { Value = x.fiIDProducto.ToString(), Text = x.fcProducto }).ToList();
            }
        }

        public List<SelectListItem> GetListMonedas()
        {
            return new List<SelectListItem> {
                new SelectListItem {
                    Value = "1",
                    Text = "Moneda Nacional (L)"
                },
                new SelectListItem {
                    Value = "2",
                    Text = "Moneda Extranjera ($)"
                }
            };
        }


        public List<SelectListItem> GetListProveedores()
        {
            using (var contexto = new ORIONDBEntities())
            {
                return contexto.sp_Proveedores_Listar().Where(x => x.fbEstadoProveedor == true).Select(x => new SelectListItem { Value = x.fiIDProveedor.ToString(), Text = x.fcNombre }).ToList();
            }
        }

        [AllowAnonymous]
        public void GetUsuariosConectados()
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.consultarUsuarios(User.Identity.Name);
        }



        public byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public ActionResult DownloadPdf()
        {
            try
            {
                var reportStream = TempData["ReportePDF"] as MemoryStream;
                //return new FileContentResult(reportStream.ToArray(), "application/pdf");
                var archivo = new FileContentResult(reportStream.ToArray(), "application/octet-stream");
                archivo.FileDownloadName = TempData["NombreArchivo"] as string;
                return archivo;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [AllowAnonymous]
        public ActionResult GetPdf()
        {
            try
            {
                var reportStream = TempData["ReportePDF"] as MemoryStream;
                //return new FileContentResult(reportStream.ToArray(), "application/pdf");
                var archivo = new FileContentResult(reportStream.ToArray(), "application/pdf");
                //archivo.FileDownloadName = TempData["NombreArchivo"] as string;

                return archivo;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public void EnviarWhatsapp(string telefono, string mensaje)
        {
            MensajeriaApi.MensajesDigitales(telefono, mensaje);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult MostrarReciboTransaccion(int fiIDTransaccion)
        {
            var reporteService = new ReportesTemplateService();
            var archivo = reporteService.GenerarRecibo(fiIDTransaccion);
            return new FileContentResult(((MemoryStream)archivo).ToArray(), "application/pdf"); ;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult MostrarFacturaTransaccion(int fiIDTransaccion)
        {
            var reporteService = new ReportesTemplateService();
            var archivo = reporteService.GenerarFactura(fiIDTransaccion);
            return new FileContentResult(((MemoryStream)archivo).ToArray(), "application/pdf"); ;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task EnviarRecibo(int fiIDTransaccion)
        {
            var email = new EmailTemplateService();
            var resultado = await email.SendEmailToRecibo(new EmailTemplateServiceModel { IdTransaccion = fiIDTransaccion });

        }


        [AllowAnonymous]
        [HttpGet]
        public async Task EnviarCorreoContratoPagare(int fiIDSolicitud)
        {
            var email = new EmailTemplateService();
            var resultado = await email.SendEmailToContratoPagare(new EmailTemplateServiceModel { IDSolicitud = fiIDSolicitud, CustomerEmail = "sistemas@miprestadito.com" });
        }


        public void eliminar(int idequifax)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.eliminarrow(idequifax);
        }

        public void agregar(int idequifax)
        {
            //var model = contexto.sp_Solicitudes_Bandeja_ObtenerPorIDSolicitud(fiIDSolicitud).FirstOrDefault();
            var model = _connection.OrionContext.sp_ReporteClientes_ObtenerPorID(idequifax).FirstOrDefault();


            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.agregarrow(model);

        }

        public List<SelectListItem> GetListPreciosPorProducto(int fiIDProducto)
        {
            using (var contexto = new ORIONDBEntities())
            {
                return contexto.sp_Producto_Precios_Detalle_ObtenerPorProducto(fiIDProducto)
                    .Select(x => new SelectListItem
                    {
                        Value = x.fiIdProductoPreciosDetalle.ToString(),
                        Text = $"L. {Convert.ToDecimal(x.fnValorProductoMN):n} / ${Convert.ToDecimal(x.fnValorProductoME):n}"
                    }).ToList();
            }
        }

        public List<ListPreciosProductosViewModel> GetListaPrecios()
        {
            using (var contexto = new ORIONDBEntities())
            {
                var PreciosPorProducto = contexto.sp_Producto_Precios_Detalle_Listar().GroupBy(x => x.fiIDProducto).ToList();
                var lista = new List<ListPreciosProductosViewModel>();
                foreach (var producto in PreciosPorProducto)
                {
                    var Precio = new ListPreciosProductosViewModel
                    {
                        fiIDProducto = producto.Key ?? 0,
                        Precios = new List<SelectListItem>(),
                        HistoricoPrecios = new List<ListHistoricoPreciosPorProductoViewModel>()
                    };

                    foreach (var precio in producto.ToList())
                    {
                        Precio.HistoricoPrecios.Add(new ListHistoricoPreciosPorProductoViewModel
                        {
                            fiIdProductoPreciosDetalle = precio.fiIdProductoPreciosDetalle,
                            fnValorProductoME = precio.fnValorProductoME ?? 0,
                            fnValorProductoMN = precio.fnValorProductoMN ?? 0
                        });
                        Precio.Precios.Add(new SelectListItem
                        {
                            Value = precio.fiIdProductoPreciosDetalle.ToString(),
                            Text = $"L. {Convert.ToDecimal(precio.fnValorProductoMN):n} / ${Convert.ToDecimal(precio.fnValorProductoME):n}"
                        });
                    }

                    lista.Add(Precio);
                }

                return lista;
            }
        }

        public ActionResult MensajeriaMasiva()
        {
            return View();
        }
        public ActionResult PromosMensajeriaMasiva()
        {
            return View();
        }


        [ValidateInput(false)]
        [HttpPost]
        public JsonResult smsMasivo(List<ExcelsmsMasivoViewModel> Excel, HttpPostedFileBase img)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var nombre = "";
                        if (img != null)
                        {
                            var documentoURL = @"\Documento\WA";
                            var urlPdf = MemoryLoadManager.URL + documentoURL;
                            var ruta = documentoURL + @"\";
                            ruta = ruta.Replace("*", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("|", "");
                            var pdfFile = urlPdf + @"\" + img.FileName;
                            var exists = System.IO.Directory.Exists(urlPdf);
                            UploadFileServer148(@"Documento\WA", img);
                            nombre = "https://orion.novanetgroup.com/Documento/wa/" + img.FileName;
                        }
                        var result = 0;
                        for (int i = 0; i < Excel.Count; i++)
                        {
                            result = Convert.ToInt32(context.sp_MensajeriaMasiva_Detalle(GetIdUser(), Excel[i].Telefono, Excel[i].Mensaje, nombre));
                        }


                        if (result > 0)
                        {
                            EscribirEnLogJson(new NotificacionViewModel { fcOperacion = $"Se envio mensajes a los siguiente numeros { string.Join(",", Excel.Select(x => x.Telefono).ToList()) }", fcClase = "success", fcTipoTransaccion = "Transaccion" });
                            return EnviarResultado(true, "Mensajes", "Guardados Exitosamente");

                        }
                        else
                        {
                            return EnviarResultado(false, "Error de red", "");
                        }
                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }

        }

        [ValidateInput(false)]
        [HttpPost]
        public JsonResult smsMasivoPromos(List<ExcelsmsMasivoViewModel> Excel, HttpPostedFileBase img)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var nombre = "";
                        if (img != null)
                        {
                            var documentoURL = @"\Documento\WA";
                            var urlPdf = MemoryLoadManager.URL + documentoURL;
                            var ruta = documentoURL + @"\";
                            ruta = ruta.Replace("*", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("|", "");
                            var pdfFile = urlPdf + @"\" + img.FileName;
                            var exists = System.IO.Directory.Exists(urlPdf);
                            UploadFileServer148(@"Documento\WA", img);
                            nombre = "https://orion.novanetgroup.com/Documento/wa/" + img.FileName;
                        }
                        var result = 0;
                        for (int i = 0; i < Excel.Count; i++)
                        {
                            result = Convert.ToInt32(context.sp_MensajeriaMasiva_ProcesarMensaje_Promos_AgregarRegistro(GetIdUser(), Excel[i].Telefono, Excel[i].Mensaje, nombre, Excel[i].IDTipoConversasion, Excel[i].pcVariable, Excel[i].pcGrupoConversasion));
                        }


                        if (result > 0)
                        {
                            EscribirEnLogJson(new NotificacionViewModel { fcOperacion = $"Se envio mensajes a los siguiente numeros {string.Join(",", Excel.Select(x => x.Telefono).ToList())}", fcClase = "success", fcTipoTransaccion = "Transaccion" });
                            return EnviarResultado(true, "Mensajes", "Guardados Exitosamente");

                        }
                        else
                        {
                            return EnviarResultado(false, "Error de red", "");
                        }
                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }

        }

        public ActionResult ReporteFinal()
        {
            var reporteService = new ReportesTemplateService();
            var archivo = reporteService.GenerarReporteDiaInfoClientesFinales();
            return new FileContentResult(((MemoryStream)archivo).ToArray(), "application/pdf");
        }

        public T GetConfiguracion<T>(string llave)
        {
            using (var contexto = new ORIONDBEntities())
            {
                return (T)Convert.ChangeType(contexto.sp_Configuraciones().FirstOrDefault(x => x.NombreLlave == llave).ValorLLave, typeof(T));
            }
        }


        public List<T> GetConfiguracion<T>(string llave, char Separador)
        {
            using (var contexto = new ORIONDBEntities())
            {
                return contexto.sp_Configuraciones().FirstOrDefault(x => x.NombreLlave == llave).ValorLLave.Split(Separador).Select(x => (T)Convert.ChangeType(x, typeof(T))).ToList();

            }
        }

        public List<T> GetConfiguracionJson<T>(string llave)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var valorConfig = contexto.sp_Configuraciones().FirstOrDefault(x => x.NombreLlave == llave)?.ValorLLave;

                if (string.IsNullOrEmpty(valorConfig))
                    return new List<T>();
                try
                {
                    return JsonConvert.DeserializeObject<List<T>>(valorConfig);
                }
                catch (JsonException)
                {
                    throw new Exception($"Error al deserializar la configuración '{llave}'. Verifica el formato JSON.");
                }
            }
        }


        public List<T> GetConfiguracionJson2<T>(string llave)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var valorConfig = contexto.sp_Configuraciones().FirstOrDefault(x => x.NombreLlave == llave)?.ValorLLave;

                if (string.IsNullOrEmpty(valorConfig))
                    return new List<T>();

                try
                {
                    return JsonConvert.DeserializeObject<List<T>>(valorConfig);
                }
                catch (JsonException)
                {
                    throw new Exception($"Error al deserializar la configuración '{llave}'. Verifica el formato JSON.");
                }
            }
        }


        public List<T> GetConfigurationJson<T>(string llave)
        {
            try
            {
                using (var contexto = new ORIONDBEntities())
                {
                    var valorConfig = contexto.sp_Configuraciones().Where(x => x.NombreLlave == llave).Select(x => x.ValorLLave).FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(valorConfig))
                        return new List<T>();

                    valorConfig = valorConfig.Trim();

                    if (valorConfig.StartsWith("["))
                    {
                        return JsonConvert.DeserializeObject<List<T>>(valorConfig);
                    }

                    var objeto = JsonConvert.DeserializeObject<T>(valorConfig);
                    return new List<T> { objeto };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al deserializar JSON para la llave '{llave}': {ex.Message}");
                return new List<T>();
            }
        }



        public static HttpPostedFileBase ConvertirBase64AImagen(string base64String, string nombreArchivo)
        {
            // Convertir la cadena Base64 en un array de bytes
            //byte[] bytes = Convert.FromBase64String(CleanBase64String(base64String));

            HttpPostedFileBase archivo = new ByteArrayHttpPostedFile(base64String, nombreArchivo);
            //GuardarImagen(bytes, archivo);
            return archivo;

        }


        public List<ListUbicacionesPorTipo> ListarUbicacionesPorTipo()
        {
            using (var contexto = new ORIONDBEntities())
            {
                var list = contexto.sp_Catalogo_Ubicaciones_ListarTipoUbicacion().ToList();
                var listModel = new List<ListUbicacionesPorTipo>();
                foreach (var item in list)
                {
                    listModel.Add(new ListUbicacionesPorTipo
                    {
                        fiIDUbicacion = item.fiIDUbicacion,
                        fcUbicacion = item.fcUbicacion,
                        fcTipoUbicacion = item.TipoUbicacion,
                        fcNombreComercial = item.fcNombreComercial,
                        fcNombreCorto = item.fcNombreCorto,
                        fbUbicacionExterna = item.fiUbicacionExterna == 1 ? true : false
                    });
                }

                return listModel;
            }
        }


        public ListUbicacionesPorTipo ObtenerInfoUbicacionPorTipo(int fiIDUbicacion)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Catalogo_Ubicaciones_ListarTipoUbicacion().FirstOrDefault(x => x.fiIDUbicacion == fiIDUbicacion);
                return new ListUbicacionesPorTipo
                {
                    fiIDUbicacion = modelDb.fiIDUbicacion,
                    fcNombreComercial = modelDb.fcNombreComercial,
                    fcNombreCorto = modelDb.fcNombreCorto,
                    fcTipoUbicacion = modelDb.TipoUbicacion,
                    fcUbicacion = modelDb.fcUbicacion
                };
            }
        }


        public List<SelectListItem> GetListFacturasInventario()
        {
            using (var contexto = new ORIONDBEntities())
            {
                return contexto.sp_Producto_Movimiento_Maestro_Listar().Where(x => x.fiEstadoMovimientoMaestro == 1).Select(x => new SelectListItem { Value = x.fiIDMovimientoMaestro.ToString(), Text = $"{x.fcNumeroFactura} ({x.fcSimboloMoneda} {x.fnTotalFactura}) {x.fcPartida}" }).ToList();
            }
        }

        [AllowAnonymous]
        public async Task<JsonResult> ObtenerIpsClientes()
        {


            using (var contexto = new ORIONDBEntities())
            {
                if (!MemoryLoadManager.ListaOLTsONUs.Any())
                {
                    MemoryLoadManager.ListaOLTsONUs = await GetListaOnusYOltsAsync();
                }
                

                var listaOLT = MemoryLoadManager.ListaOLTsONUs.Where(x => x.fcTipoArticulo == "OLT").ToList();
                var listaONU = MemoryLoadManager.ListaOLTsONUs.Where(x => x.fcTipoArticulo == "ONU").ToList();

                var listaModel = new List<ListOLTViewModel>();

                foreach (var OLT in listaOLT)
                {
                    var model = new ListOLTViewModel
                    {
                        fiIDOLT = OLT.fiIDDetalleCliente_Servicio,
                        fcIP = OLT.fcIP,
                        listaONUS = new List<ListONUViewModel>(),
                        fcNombre = OLT.fcNombre,
                        fbConectado = OLT.fbConectado ?? false,
                        fdUltimaActualizacion = OLT.fdFechaRegitro
                    };

                    var solicitudes = OLT?.fcSolicitudes?.Split(',')?.Select(x => Convert.ToInt32(x))?.ToList() ?? new List<int>();
                    model.listaONUS.AddRange(listaONU.Where(x => solicitudes.Any(y => y == x.fiIDOrionSolicitud)).Select(x => new ListONUViewModel
                    {
                        fiIDONU = x.fiIDDetalleCliente_Servicio,
                        fcIP = x.fcIP,
                        fcMac = x.fcMac,
                        fcNombre = x.fcNombre,
                        fcNombreWifi = x.fcNombreWifi,
                        fiIDEquifax = x.fiIDEquifax ?? 0,
                        fiIDOrionSolicitud = x.fiIDOrionSolicitud,
                        fbConectado = x.fbConectado ?? false,
                        fcIPOLT = OLT.fcIP,
                        fcPon = x.fcPom,
                        CodigoCepheus = x.fcCodigoCepheus,
                        fdUltimaActualizacion = x.fdFechaRegitro,
                        fcEmpresa = x.fcEmpresa,
                        fcContrasenia = x.fcContrasenia,
                        fcURLLogin = x.fcURLLogin,
                        fcUsuarioONU = x.fcUsuarioONU
                    }).ToList());


                    listaModel.Add(model);
                }


                return EnviarListaJson(listaModel);
            }

        }







        [AllowAnonymous]
        public JsonResult GetURLPath()
        {
            return Json(MemoryLoadManager.URL, JsonRequestBehavior.AllowGet);
        }


        public List<SelectListItem> GetClientesSRC()
        {
            using (var contexto = new ORIONDBEntities())
            {
                return contexto.sp_SRC_Clientes_Listar().Select(x => new SelectListItem { Value = x.fcIdentidad, Text = $" {(x.fiIDEquifax == null ? "" : x.fiIDEquifax.ToString() + " | ")} {x.fcNombreSAF}" }).ToList();
            }
        }

        public Dictionary<string, string> GeneradorDelDireccionarioCliente(int idSolicitud, int idCliente, int idOperacion, int idSolicitudInstalacion)
        {

            // var listVariables = _connection.autoLoanContext.EmailTemplate_Variable.Where(z => z.fbVariableActive).ToList();
            //foreach (var item in listVariables)
            //{
            //    dictionary.Add($"{{item.fcVariableName}}", DateTime.Now.ToString("dd/MM/yyyy"));
            //}

            var datosCliente = _connection.OrionContext.sp_OrionContratista_DetalleBySolicitud(1, idSolicitud, 0, 0).FirstOrDefault();
            var NumeroOrden = Convert.ToString(datosCliente.fiNoOrden);
            var DiaPago = Convert.ToString(datosCliente.fiDiadePago);
            DictionaryList.Clear();
            DictionaryList.Add("DateNow", DateTime.Now.ToString("MMM dd, yyyy"));
            DictionaryList.Add("fcNombreEmpresa", datosCliente.fcNombreEmpresa);
            DictionaryList.Add("fcNombreCliente", datosCliente.fcNombre);
            DictionaryList.Add("fcNumeroORden", "00" + NumeroOrden);
            DictionaryList.Add("fcNORdenCepehus", datosCliente.fcNumeroOrdenCfeus);
            DictionaryList.Add("fcTelefonoCliente", datosCliente.fcTelefonoCliente);
            DictionaryList.Add("fcEmailCliente", datosCliente.fcCorreo);
            DictionaryList.Add("fcEquipoAdquirido", datosCliente.fcArticulosdelContrato);
            DictionaryList.Add("fcDireccionDomicilioCliente", datosCliente.fcDepartamento + " " + datosCliente.fcMunicipio + "  " + datosCliente.fcBarrio + " " + datosCliente.fcDireccionDetallada);
            DictionaryList.Add("fcObservaciones", datosCliente.fcComentario);
            DictionaryList.Add("fcObservacionesTecnico", datosCliente.fcComentarioInstalacion);
            DictionaryList.Add("NombreTecnico", datosCliente.NombreTecnicoAsignado);
            DictionaryList.Add("HoraProgramada", datosCliente.fdFechaInstalacionInicio.ToString("dd/MM/yyyy HH:mm:ss"));
            DictionaryList.Add("fcHoraInicio", datosCliente.fdFechaInstalacionInicio.ToString("dd/MM/yyyy HH:mm:ss"));
            DictionaryList.Add("fcHoraFinal", datosCliente.fdFechaInstalacionFinal.ToString("dd/MM/yyyy HH:mm:ss"));
            DictionaryList.Add("FiDiaPago", DiaPago);



            var Lista = _connection.OrionContext.sp_ListadoMaterialesInstalados_BySolicitudInstalacion(datosCliente.fiIDContratistaSolicitud).ToList();
            var DatosMaterialesInstalacion = new List<sp_ListadoMaterialesInstalados_BySolicitudInstalacion_Result>();
            foreach (var item in Lista)
            {
                DatosMaterialesInstalacion.Add(new sp_ListadoMaterialesInstalados_BySolicitudInstalacion_Result
                {
                    fcProducto = item.fcProducto,
                    fnCantidad = item.fnCantidad,
                });
            }

            DictionaryList.Add("fcProducto", string.Join(",", DatosMaterialesInstalacion.Select(x => x.fcProducto).ToList()));
            DictionaryList.Add("fnCantidad", string.Join(",", DatosMaterialesInstalacion.Select(x => x.fnCantidad.ToString()).ToList()));

            var materiales = _connection.OrionContext.sp_ListadoMaterialesInstalados_Articulos(datosCliente.fiIDContratistaSolicitud).FirstOrDefault();
            DictionaryList.Add("fcProductoMateriales", materiales);

            return DictionaryList;
        }


        public List<int> GetListProductosConsumibles()
        {
            using (var contexto = new ORIONDBEntities())
            {
                return contexto.sp_Producto_Maestro_ListarConsumibles().Select(x => x.fiIDProducto).ToList();
            }
        }

        public string GetPropiedades(Object o)
        {
            Type t = o.GetType();
            PropertyInfo[] pis = t.GetProperties();
            string[] valores = pis.Select(p => p.Name + " : " + p.GetValue(o)).ToArray();
            string delimitados = string.Join(",", valores);
            return delimitados;
        }



        public List<SelectListItem> GetListaTipoPartida()
        {
            using (var coneccion = new CoreAdministrativoEntities())
            {
                var lista = coneccion.sp_Catalogo_TipoPartida_Listar().Where(z => z.fiIDEmpresa == 1).Select(z => new SelectListItem { Value = z.fcTipoPartida.Trim(), Text = z.fcTipoPartida + " - " + z.fcDescripcionTipoOperacion }).ToList();
                return lista;
            }
        }


        public string ObtenerCorrelativoPartida(int idEmpresa, string tipoPartida)
        {
            using (var conexion = new CoreAdministrativoEntities())
            {
                var TipoPartidaModel = conexion.sp_Catalogo_TipoPartida_Listar().FirstOrDefault(x => x.fiIDEmpresa == idEmpresa && x.fcTipoPartida == tipoPartida);
                var model = new CorrelativoPartidaViewModel { TipoPartida = tipoPartida, Correlativo = CorrelativeHelper.CrearCorrelativoPartida(TipoPartidaModel.fcTipoPartida, TipoPartidaModel.fiNumeroCorrelativo) };

                return CorrelativeHelper.CrearCorrelativoPartida(TipoPartidaModel.fcTipoPartida, TipoPartidaModel.fiNumeroCorrelativo);
            }

        }

        public List<SelectListItem> CargarListaCuentasContablesPorMoneda(int idEmpresa, int idMoneda, int idUsuario)
        {
            var list = new List<SelectListItem>();
            using (var contexto = new CoreAdministrativoEntities())
            {
                using (var conection = contexto.Database.Connection)
                {
                    conection.Open();
                    var command = conection.CreateCommand();
                    command.CommandText = $"EXEC sp_CuentasContables_Listar 113, {idUsuario}, 1, ${idMoneda}";

                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new CoreAdministrativoEntities());
                        list = db.ObjectContext.Translate<sp_CuentasContables_Listar_Model>(reader).ToList().Select(z => new SelectListItem { Value = z.fcCuentaContable.Trim(), Text = z.fcCuentaContable.Trim() + " - " + z.fcDescripcionCuenta, Disabled = z.fcAceptaPosteo == "N" || z.fcCuentaBloqueada == "S" }).ToList();

                    }
                    conection.Close();
                }
            }

            return list;
        }

        public Catalogo_Contable GetCuentaContable(string fcCuentaContable, int idMoneda)
        {
            using (var contexto = new CoreAdministrativoEntities())
            {
                return contexto.Catalogo_Contable.FirstOrDefault(x => x.fiMoneda == idMoneda && x.fiIDEmpresa == 1 && x.fcCuentaContable == fcCuentaContable);
            }
        }


        public List<SelectListItem> GetListaFondos()
        {

            using (var _connection = new CoreAdministrativoEntities())
            {
                var ListaFondos = _connection.sp_Fondos_Listar(113, GetIdUser(), 1).Select(z => new SelectListItem { Value = z.fcFondo.ToString(), Text = z.fcFondo.ToString() + " - " + z.fcDescripcionFondo }).ToList(); ;
                return ListaFondos;
            }


        }

        public List<SelectListItem> GetListaProgramas()
        {

            using (var _connection = new CoreAdministrativoEntities())
            {
                var ListaFondos = _connection.sp_Programas_Listar(113, GetIdUser(), 1).Select(z => new SelectListItem { Value = z.fcPrograma.ToString(), Text = z.fcPrograma.ToString() + " - " + z.fcDescripcionPrograma }).ToList();
                return ListaFondos;
            }


        }


        public List<SelectListItem> GetListaCentroCosto()
        {

            using (var _connection = new CoreAdministrativoEntities())
            {
                var ListaCentroCosto = _connection.sp_CentroCosto_Listar(113, GetIdUser(), 1).Select(z => new SelectListItem { Value = z.fcCentrodeCosto.ToString(), Text = z.fcCentrodeCosto.ToString() + " - " + z.fcDescripcionAgencia }).ToList();
                return ListaCentroCosto;
            }


        }

        public decimal GetTasaDeCambiosPorMes(DateTime fecha)
        {
            using (var contexto = new CoreAdministrativoEntities())
            {
                var conversion = contexto.sp_ObtenerTasaDeCambioMoneda(fecha.Month, fecha.Year, 1, 2).FirstOrDefault();
                return conversion?.fnTasadeCambio ?? 0;// Convert.ToDecimal(ConfigurationManager.AppSettings["TasaDeCambioActual"]);//  23.8815m;// 24.0463m;// 24.0715m;
            }

        }

        public List<SelectListItem> GetOperacionesContablesInventario()
        {
            using (var contexto = new ORIONDBEntities())
            {
                return contexto.sp_Catalogo_OperacionesContables_Inventario_Listar().Select(x => new SelectListItem { Value = x.fiIDOperacionesContablesInventario.ToString(), Text = x.fcNombreOperacion }).ToList();
            }
        }


        public async Task<List<sp_ONUsYOLTs_Listar_Result>> GetListaOnusYOltsAsync()
        {
            using (var context = new ORIONDBEntities())
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["OrionConnections"].ConnectionString))
                {
                    using (var command = new SqlCommand("sp_ONUsYOLTs_Listar", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        List<sp_ONUsYOLTs_Listar_Result> resultList = new List<sp_ONUsYOLTs_Listar_Result>();

                        try
                        {
                            if (connection.State != ConnectionState.Open)
                            {
                                await connection.OpenAsync();
                            }

                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                while (await reader.ReadAsync())
                                {
                                    var item = LlenarFormulario<sp_ONUsYOLTs_Listar_Result>(reader);
                                    resultList.Add(item);
                                }
                            }

                            return resultList;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error al obtener la lista de ONUs y OLTs", ex);
                        }
                    }
                }
            }
        }


        public T LlenarFormulario<T>(DbDataReader reader) where T : new()
        {
            var obj = new T();
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                int ordinal;
                try
                {
                    ordinal = reader.GetOrdinal(property.Name);
                }
                catch (IndexOutOfRangeException)
                {
                    continue; // La columna no existe en el resultado
                }

                if (!reader.HasRows || reader.IsDBNull(ordinal))
                {
                    // Si el valor es DBNull y la propiedad es nullable, asignar null
                    if (IsNullableType(property.PropertyType))
                    {
                        property.SetValue(obj, null);
                    }
                    continue;
                }

                object value = reader[property.Name];
                if (value != DBNull.Value)
                {
                    // Manejar la conversión según si el tipo es nullable o no
                    if (IsNullableType(property.PropertyType))
                    {
                        // Obtener el tipo subyacente del Nullable<T>
                        Type underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
                        property.SetValue(obj, Convert.ChangeType(value, underlyingType));
                    }
                    else
                    {
                        property.SetValue(obj, Convert.ChangeType(value, property.PropertyType));
                    }
                }
            }

            return obj;
        }

        // Método auxiliar para verificar si un tipo es nullable
        private bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }


        #region SignalR Clientes Potenciales

        public void AgregarClientePotencial(int idClientePotencial)
        {
            var ListaClientePotencial = new List<ClientesPotencialesViewModel>();
            try
            {
                var usuario = GetUser();
                var model = _connection.OrionContext.sp_Clientes_Potenciales_Listado_ById(idClientePotencial, usuario.fiIdUsuario, usuario.IdRol, 1).FirstOrDefault();

                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
                hubContext.Clients.All.AgregarClientePotencial(model);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void ActualizarClienteBandejaClientePotenciales(int idclientePotencial)
        {

            var usuario = GetUser();
            var model = _connection.OrionContext.sp_Clientes_Potenciales_Listado_ById(idclientePotencial, usuario.fiIdUsuario, usuario.IdRol, 1).FirstOrDefault();
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.ActualizarBandejaClientesPotenciales(model);

        }

        public void AgregarBitacoraClientesPotenciales(int idbitacora)
        {
            var Usuario = GetUser();
            var model = _connection.OrionContext.sp_clientes_Potenciales_Bitacoras_Listado_ById(idbitacora, Usuario.fiIdUsuario, Usuario.IdRol, 1).FirstOrDefault();
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.AgregarBitacoraClientePotenciales(model);
            //como es una vista parcial, al momento de consultarla en la vista debe de llamarse diferente
        }

        public void EliminarRowClientesPotenciales(int fiIdClientePotencial)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.EliminarRowClientesPotenciales(fiIdClientePotencial);
        }

        public void AgregarEstatusDropCrearClientePotencia(int fiIdEstatusCliente)
        {
            var Usuario = GetUser();
            var model = _connection.OrionContext.sp_Estatus_Clientes_Potenciales_Listado_ById(fiIdEstatusCliente, Usuario.fiIdUsuario, Usuario.IdRol, 1).FirstOrDefault();
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.AgregarDropEstatusClientesPotenciales(model);

        }

        public void AgregarRowEstatusClientePotenciales(int fiIdEstatusCliente)
        {
            var Usuario = GetUser();
            var model = _connection.OrionContext.sp_Estatus_Clientes_Potenciales_Listado_ById(fiIdEstatusCliente, Usuario.fiIdUsuario, Usuario.IdRol, 1).FirstOrDefault();
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.AgregarRowEstatusClientes(model);

        }

        public void EditarRowEstatusClientePotenciales(int fiIdEstatusCliente)
        {
            var Usuario = GetUser();
            var model = _connection.OrionContext.sp_Estatus_Clientes_Potenciales_Listado_ById(fiIdEstatusCliente, Usuario.fiIdUsuario, Usuario.IdRol, 1).FirstOrDefault();
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.EditarRowEstatusCliente(model);
        }
        public void AgregarRowFuenteClientePotenciales(int fiIdFuenteCliente)
        {
            var usuario = GetUser();
            var modelo = _connection.OrionContext.sp_Fuente_Clientes_Potenciales_Listado_ById(fiIdFuenteCliente, usuario.fiIdUsuario, usuario.IdRol, 1).FirstOrDefault();
            var hubContex = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContex.Clients.All.AgregarRowFuenteClientesPotenciales(modelo);

        }

        public void EditarRowFuenteClientePotenciales(int fiIdFuenteCliente)
        {
            var Usuario = GetUser();
            var modelo = _connection.OrionContext.sp_Fuente_Clientes_Potenciales_Listado_ById(fiIdFuenteCliente, Usuario.fiIdUsuario, Usuario.IdRol, 1).FirstOrDefault();
            var hubContex = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContex.Clients.All.EditarRowClientesPotenciales(modelo);

        }

        public void EliminarRowClientePotenciales(int fiIdFuenteCliente)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.EliminarRowFuenteClientesPotenciales(fiIdFuenteCliente);
        }


        public void AgregarFuenteDropClientePotenciales(int fiIdFuenteCliente)
        {
            var usuario = GetUser();
            var modelo = _connection.OrionContext.sp_Fuente_Clientes_Potenciales_Listado_ById(fiIdFuenteCliente, usuario.fiIdUsuario, usuario.IdRol, 1).FirstOrDefault();
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.AgregarDropFuenteClientesPotenciales(modelo);
        }


        #endregion

        #region Signal R de Cotratista y Tecnicos


        public void ActualizarFilaTecnico(int fiIdcontratistaSolicitud)
        {
            var Usuario = GetUser();
            var model = _connection.OrionContext.sp_SolicitudesAsignadas_By_Tecnico(Usuario.fiIdUsuario, Usuario.IdRol).FirstOrDefault(a => a.fiIDSolicitudInstalacion == fiIdcontratistaSolicitud);
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.ModificarFilaTecnico(model);

        }


        public void ActualizarFilaContratista(int fiIdcontratistaSolicitud)
        {
            var Usuario = GetUser();
            var model = _connection.OrionContext.sp_ContratistaSolicitudInstalacion_ById(Usuario.fiIdUsuario, Usuario.IdRol).FirstOrDefault(a => a.fiIDSolicitudInstalacion == fiIdcontratistaSolicitud);
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.ModificarFilacontratista(model);

        }

        public void ActualizarFilaOrdenTrabajo(int fiIdcontratistaSolicitud)
        {
            var Usuario = GetUser();
            var model = _connection.OrionContext.sp_SolicitudesAsignadas_By_Tecnico(Usuario.fiIdUsuario, Usuario.IdRol).FirstOrDefault(a => a.fiIDSolicitudInstalacion == fiIdcontratistaSolicitud);
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.ModificarFilaOrdenTrabajo(model);

        }


        #endregion

        #region Generar Documento y enviarlo tanto por email como por mensaje de whatsap

        public string pathToOpen { get; set; }
        public string pathToSave { get; set; }

        public async Task<bool> ModificarPlantillasDocumentos<T>(T model, EmailEnviarDocumentosViewModel emailmodel)
        {
            //Dictionary<string, string> DictionaryList = new Dictionary<string, string>();

            foreach (var property in typeof(T).GetProperties())
            {
                var value = property.GetValue(model, null)?.ToString() ?? "N/A";
                DictionaryList.Add(property.Name, value);
            }

            var emailTemplateModel = _connection.OrionContext.sp_OrionSolicitud_detallePlantillaCorreo(1, emailmodel.fiIdEmailTemplate).FirstOrDefault();

            Attachment fileAttachment = null;
            var CustomerAttachmentName = $"{emailmodel.fiIdSolicitud}_{emailmodel.fcNombreMostrarArchivo}.pdf";
            emailTemplateModel.fcEmailAttachmentName = emailTemplateModel.fcEmailAttachmentName;//se supone que es el archivo que busca para que lo modifique //"PlantillaOrdenTrabajoIncidente.docx";

            pathToOpen = Path.Combine(System.Web.HttpContext.Current.Server.MapPath(MemoryLoadManager.VirtualPathServerToEmailTemplates), emailTemplateModel.fcEmailAttachmentName);//  "PlantillaOrdenTrabajoIncidente.docx");
            pathToSave = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Solicitudes/Solicitud_" + emailmodel.fiIdSolicitud + "/"), CustomerAttachmentName);

            var pdfDoc = GeneradorPDFAttachmentFrimadoRevisado();
            fileAttachment = pdfDoc;

            var BodyEmailParsed = ParsearVariablesEnEmailTemplate(emailTemplateModel.fcEmailBody, DictionaryList);
            var subjectEmailParsed = ParsearVariablesEnEmailTemplate(emailTemplateModel.fcEmailSubject, DictionaryList);
            var HtmlPdfConverter = new HtmlToPdfConverter();
            var body = "Buenas Estimado: Cliente le adjuntamos el Anexo de la orden de trabajo ya firmado";
            var pdfBytes = HtmlPdfConverter.GeneratePdf(body);

            // no se para quese ocupara estos dos la verdad
            var fileStream = new StreamReader(new MemoryStream(pdfBytes), System.Text.Encoding.Default, false); //no se por que se ocupara esto 
            var file = new Attachment(fileStream.BaseStream, "ContratoExtensionResivido.pdf");// tampoco se por que se ocupara esto 

            var envio = new SendEmailViewModel // se supone que debe de ser un return esto para que se envie 
            {
                EmailName = emailTemplateModel.fcEmailTitle,
                Subject = subjectEmailParsed,
                Body = BodyEmailParsed,
                DestinationEmail = emailmodel.fcEmailDestino,// CustomerEmail,
                Attachment = fileAttachment,
                AttachmentName = CustomerAttachmentName,
                List_CC = emailmodel.fcEmailcopiacorreo //List_CC
            };

            var SendEmailResult = await _emailService.SendEmailAsync(envio);

            return SendEmailResult;
        }

        // creo que mejor seria pasar esto a public por cualquier cosa aun que de momento solo se esta usando para crear los documentos asi que no se si abra problemas despues 
        private Attachment GeneradorPDFAttachmentFrimadoRevisado()
        {
            using (Document doc = new Document())
            {
                doc.LoadFromFile(pathToOpen);
                string data = "";
                foreach (var item in DictionaryList)
                {
                    if (item.Key == "fcFirma")
                    {
                        data = item.Value;
                    }
                    else
                    {
                        doc.Replace("{" + item.Key + "}", item.Value ?? "", true, true);
                    }
                }

                var base64Data = Regex.Match(data, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                var binData = Convert.FromBase64String(base64Data);

                using (var stream = new MemoryStream(binData))
                {
                    var Images = new Bitmap(stream);

                    Image image = Image.FromHbitmap(Images.GetHbitmap());

                    foreach (Section section in doc.Sections)
                    {
                        foreach (Paragraph paragraph in section.Paragraphs)
                        {
                            foreach (DocumentObject docObj in paragraph.ChildObjects)
                            {
                                if (docObj.DocumentObjectType == DocumentObjectType.Picture)
                                {
                                    DocPicture picture = docObj as DocPicture;
                                    if (picture.Title == "Firma")
                                    {
                                        //Replace the image
                                        picture.LoadImage(image);
                                    }
                                }
                            }
                        }
                        //Loop through the child elements of paragraph

                    }


                }

                doc.SaveToFile(pathToSave, FileFormat.PDF);

                var attachmentFile = new Attachment(pathToSave, MediaTypeNames.Application.Octet);
                doc.Close();
                return attachmentFile;
            }
        }

        public string ParsearVariablesEnEmailTemplate(string emailTemplate, Dictionary<string, string> variableValues)
        {

            var codeStartDelimiter = "{";
            var codeEndDelimiter = "}";

            var escapeCharacters = new[] { ".", "$", "{", "}", "[", "]", "^", "(", ")", "|", "*", "+", "?", @"\" };

            var delStartReg = $"{codeStartDelimiter}";
            var delEndReg = $"{codeEndDelimiter}";

            if (escapeCharacters.Contains(delStartReg)) delStartReg = $"\\{delStartReg}";
            if (escapeCharacters.Contains(delEndReg)) delEndReg = $"\\{delEndReg}";

            var regexp = new Regex(delStartReg + "[^" + delStartReg + delEndReg + "]*" + delEndReg);

            var matches = regexp.Matches(emailTemplate);

            foreach (Match match in matches)
            {
                var code = match.Value.Replace(codeStartDelimiter, "");
                code = code.Replace(codeEndDelimiter, "");

                var codeResult = variableValues[code];

                //try
                //{
                //    codeResult = _evaluateExpression.EvaluateCodeSnippet<string>(code).Result;
                //}
                //catch (Exception e)
                //{
                //    throw _exceptionFactory.GetException(25084, code);
                //}

                emailTemplate = emailTemplate.Replace(match.Value, codeResult);

            }

            return emailTemplate;

        }


        #endregion

    }

    public class ByteArrayHttpPostedFile : HttpPostedFileBase
    { // a partir de un archivo en formato de bytes, se creo esta clase derivada de HttpPostedFileBase que envuelva los bytes del archivo
        private readonly byte[] _fileBytes;
        private readonly string _fileName;

        public ByteArrayHttpPostedFile(string base64String, string fileName)
        {
            _fileBytes = Convert.FromBase64String(ObtenerContenidoBase64(base64String));
            _fileName = fileName;
        }

        private string ObtenerContenidoBase64(string base64String)
        {
            int index = base64String.IndexOf(',');
            if (index >= 0)
            {
                return base64String.Substring(index + 1);
            }
            return base64String;
        }

        public override int ContentLength => _fileBytes.Length;

        public override string FileName => _fileName;

        public override Stream InputStream => new MemoryStream(_fileBytes);
    }




}
