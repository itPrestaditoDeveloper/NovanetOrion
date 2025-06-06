using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.App_Services.EmailService;
using OrionCoreCableColor.App_Services.FileKMZService;
using OrionCoreCableColor.App_Services.PingService;
using OrionCoreCableColor.DbConnection.CMContext;
using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models.Base;
using OrionCoreCableColor.Models.Contratista;
using OrionCoreCableColor.Models.EmailTemplateService;
using OrionCoreCableColor.Models.Solicitudes;
using OrionCoreCableColor.Models.Soporte;
using OrionCoreCableColor.Models.Tecnico;

using Spire.Doc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ZXing;

namespace OrionCoreCableColor.Controllers
{
    [System.Web.Mvc.Authorize]
    public class SoporteController : BaseController
    {
        AdminDispositivosService Dispositivos = new AdminDispositivosService();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexEquiposCliente()
        {
            return View();
        }

        public ActionResult BandejaRespuestaApiCpheus()
        {

            return View();
        }

        public JsonResult CargarDatosRespuestaApi(string fechaMinima = null, string fechaMaxima = null)
        {
            var listaDetalleDatos = new List<RespuestaApiCpheusViewModel>();

            DateTime? fechaInicio = null;
            DateTime? fechaFin = null;

            if (!string.IsNullOrEmpty(fechaMinima) && !string.IsNullOrEmpty(fechaMaxima))
            {
                fechaInicio = DateTime.Parse(fechaMinima);
                fechaFin = DateTime.Parse(fechaMaxima);
            }
            else
            {
                fechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                fechaFin = DateTime.Now;
            }

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();
                var command = connection.CreateCommand();

                string fechaInicioParam = fechaInicio.HasValue ? $"'{fechaInicio.Value:yyyy-MM-dd}'" : "NULL";
                string fechaFinParam = fechaFin.HasValue ? $"'{fechaFin.Value:yyyy-MM-dd}'" : "NULL";

                command.CommandText = $"EXEC sp_Servicios_APIEstados ";

                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());

                    listaDetalleDatos = db.ObjectContext.Translate<RespuestaApiCpheusViewModel>(reader).ToList();
                }

                connection.Close();
            }

            return Json(listaDetalleDatos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> CargarListaOltOnus()
        {
            using (var contexto = new ORIONDBEntities())
            {
                var listaDispositivos = Dispositivos.LeerDispostivosObjeto();
                MemoryLoadManager.ListaOLTsONUs = await GetListaOnusYOltsAsync(); //contexto.sp_ONUsYOLTs_Listar().ToList();
                var listaOLT = MemoryLoadManager.ListaOLTsONUs.Where(x => x.fcTipoArticulo == "OLT").ToList();
                var listaONU = MemoryLoadManager.ListaOLTsONUs.Where(x => x.fcTipoArticulo == "ONU").ToList();


                var listaModel = new List<ListInfoOLTViewModel>();

                foreach (var OLT in listaOLT)
                {

                    //var oltDispositivo = listaDispositivos.FirstOrDefault(x => x.fcTipoDispositivo == "OLT" && x.fiIDOrionSolicitud == OLT.fiIDDetalleCliente_Servicio);
                    var model = new ListInfoOLTViewModel
                    {
                        fiIDOLT = OLT.fiIDDetalleCliente_Servicio,
                        fcIP = OLT.fcIP,
                        listaONUS = new List<ListONUViewModel>(),
                        ListInfoONU = new List<ListInfoONUViewModel>(),
                        fcNombre = OLT.fcNombre,
                        fbConectado = OLT.fbConectado ?? false,
                        fbEstado = OLT.fbConectado ?? null,
                        fdUltimaActualizacion = OLT.fdFechaRegitro,
                        
                    };





                    var solicitudes = OLT?.fcSolicitudes?.Split(',')?.Select(x => Convert.ToInt32(x))?.ToList() ?? new List<int>();

                    foreach (var ONU in listaONU.Where(x => solicitudes.Any(y => y == x.fiIDOrionSolicitud)).ToList())
                    {


                        model.ListInfoONU.Add(new ListInfoONUViewModel
                        {
                            fiIDONU = ONU.fiIDDetalleCliente_Servicio,
                            fcIP = ONU.fcIP,
                            fcMac = ONU.fcMac,
                            fcNombre = ONU.fcNombre,
                            fcNombreWifi = ONU.fcNombreWifi,
                            fiIDEquifax = ONU.fiIDEquifax ?? 0,
                            fiIDOrionSolicitud = ONU.fiIDOrionSolicitud,
                            fbEstado = ONU.fbConectado,
                            fbConectado = ONU.fbConectado ?? false,
                            fbCancelado = false,
                            fcGeolocalizacion = (ONU?.fcGeolocalizacion?.Replace(" ", "")?.Replace(",", "") ?? "") != "" ? "https://www.google.com/maps?z=12&t=k&q=" + ONU?.fcGeolocalizacion?.Replace(" ", "") ?? "" : "",
                            fcPrestamo = ONU?.fcIDPrestamo ?? "",
                            fcTelefono = ONU?.fcTelefono ?? "",
                            fcIPOLT = OLT.fcIP,
                            fcPon = ONU.fcPom,
                            fcCodigoCepheus = ONU.fcCodigoCepheus,
                            fdUltimaActualizacion = ONU.fdFechaRegitro,
                            fcEmpresa = ONU.fcEmpresa
                        });


                        //https://web.whatsapp.com/send/?phone=504{numero}&text&type=phone_number&app_absent=0
                    }



                    listaModel.Add(model);
                }
                listaModel = listaModel.Where(x => x.ListInfoONU.Any()).ToList();

                return EnviarListaJson(listaModel);
            }
            //return ObtenerIpsClientes();
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> ActualizarOLT(ListOLTViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var piIDOLT = new SqlParameter("@piIDOLT", model.fiIDOLT);
                var pbEstadoConexion = new SqlParameter("@pbEstadoConexion", model.fbConectado);
                await contexto.Database.ExecuteSqlCommandAsync("EXEC sp_OLT_Conexiones_Bitacora_Insertar @piIDOLT, @pbEstadoConexion ", piIDOLT, pbEstadoConexion);
                ///await ActualizarOLTApi(model); //DESCOMENTAR
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
                hubContext.Clients.All.actualizarOLT(model);
                ActualizarDispositivoOLT(model);
                return Json(true, JsonRequestBehavior.AllowGet);
            }

        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> ActualizarListaOLT(List<ListOLTViewModel> Data)
        { 
            
            using (var contexto = new ORIONDBEntities())
            {

                foreach (var model in Data)
                {
                    var piIDOLT = new SqlParameter("@piIDOLT", model.fiIDOLT);
                    var pbEstadoConexion = new SqlParameter("@pbEstadoConexion", model.fbConectado);
                    var pcEstadoPing = new SqlParameter("@pcEstadoPing", model.fcStatusPing);
                    await contexto.Database.ExecuteSqlCommandAsync("EXEC sp_OLT_Conexiones_Bitacora_Insertar @piIDOLT, @pbEstadoConexion, @pcEstadoPing ", piIDOLT, pbEstadoConexion, pcEstadoPing);

                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
                    hubContext.Clients.All.actualizarOLT(model);
                    ActualizarDispositivoOLT(model);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }






        public void ActualizarDispositivoOLT(ListOLTViewModel model)
        {


            var olt = new DispositivoViewModel();
            olt.fcTipoDispositivo = "OLT";
            olt.fcIPOLT = model.fcIP.Replace("-", ".");
            olt.fcIP = model.fcIP.Replace("-", ".");
            olt.fiIDOrionSolicitud = model.fiIDOLT;
            olt.fcPon = "";
            olt.fcWifi = "";
            olt.fcNombreDispositivo = model.fcNombre;
            olt.fbConectado = model.fbConectado;
            olt.fcMac = "";
            olt.fcCodigoCableColor = "";
            olt.fbEstadoServicio = true;
            var oltJson = JObject.FromObject(olt);

            Dispositivos.AgregarOEditarDispositivo("OLT", "fiIDOrionSolicitud", model.fiIDOLT.ToString(), oltJson);

        }


        public async Task ActualizarDispositivoONU(ListONUViewModel model)
        {
            var estadoConneccion = true;

            // Capturar los tres grupos
            //codigoCableColor = match.Groups[1].Value; // Contenido del primer paréntesis
            //nombreCliente = match.Groups[2].Value; // Texto central
            try
            {
                var cableColor = await Dispositivos.GetEstadoUsuarioCepheus(model.CodigoCepheus);
                estadoConneccion = cableColor.estadoServicio;
            }
            catch (Exception ex)
            {

            }

            ///pon = match.Groups[3].Value; // Contenido del segundo paréntesis


            var onu = new DispositivoViewModel();
            onu.fcTipoDispositivo = "ONU";
            onu.fcIPOLT = model.fcIPOLT;
            onu.fcIP = model.fcIP;
            onu.fiIDOrionSolicitud = model.fiIDOrionSolicitud;
            onu.fcPon = model.fcPon;
            onu.fcWifi = model.fcNombreWifi;
            onu.fcNombreDispositivo = model.fcNombre;
            onu.fbConectado = model.fbConectado;
            onu.fcMac = model.fcMac;
            onu.fcCodigoCableColor = model.CodigoCepheus;
            onu.fbEstadoServicio = estadoConneccion;
            var onuJson = JObject.FromObject(onu);

            Dispositivos.AgregarOEditarDispositivo("ONU", "fiIDOrionSolicitud", model.fiIDOrionSolicitud.ToString(), onuJson);
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> ActualizarONU(ListONUViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {

                var piIDDetalleCliente_Servicio = new SqlParameter("@piIDDetalleCliente_Servicio", model.fiIDONU);
                var pbEstadoConexion = new SqlParameter("@pbEstadoConexion", model.fbConectado);
                await contexto.Database.ExecuteSqlCommandAsync("EXEC sp_ONU_Conexiones_Bitacora_Insertar @piIDDetalleCliente_Servicio, @pbEstadoConexion ", piIDDetalleCliente_Servicio, pbEstadoConexion);
                var listaGeneral = MemoryLoadManager.ListaOLTsONUs; //contexto.sp_ONUsYOLTs_Listar().ToList();
                var OLTs = listaGeneral.Where(x => x.fcTipoArticulo == "OLT").ToList();
                var ONU = listaGeneral.FirstOrDefault(x => x.fcTipoArticulo == "ONU" && x.fiIDOrionSolicitud == model.fiIDOrionSolicitud);
                foreach (var olt in OLTs)
                {
                    var solicitudes = olt?.fcSolicitudes?.Split(',')?.Select(x => Convert.ToInt32(x))?.ToList() ?? new List<int>();
                    if (solicitudes.Any(x => x == model.fiIDOrionSolicitud))
                    {
                        model.fcIPOLT = olt.fcIP;
                        model.CodigoCepheus = ONU.fcCodigoCepheus;
                        model.fcPon = ONU.fcPom;
                        break;
                    }
                }




                //await ActualizarONUApi(model); //DESCOMENTAR
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
                hubContext.Clients.All.actualizarONU(model);



                await ActualizarDispositivoONU(model);



                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }



        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> ActualizarListONU(List<ListONUViewModel> Data)
        {
            using (var contexto = new ORIONDBEntities())
            {
                foreach(var model in Data)
                {
                    var piIDDetalleCliente_Servicio = new SqlParameter("@piIDDetalleCliente_Servicio", model.fiIDONU);
                    var pbEstadoConexion = new SqlParameter("@pbEstadoConexion", model.fbConectado);
                    var pcEstadoPing = new SqlParameter("@pcEstadoPing", model.fcStatusPing);
                    await contexto.Database.ExecuteSqlCommandAsync("EXEC sp_ONU_Conexiones_Bitacora_Insertar @piIDDetalleCliente_Servicio, @pbEstadoConexion,@pcEstadoPing ", piIDDetalleCliente_Servicio, pbEstadoConexion, pcEstadoPing);
                    var listaGeneral = MemoryLoadManager.ListaOLTsONUs; //contexto.sp_ONUsYOLTs_Listar().ToList();
                    var OLTs = listaGeneral.Where(x => x.fcTipoArticulo == "OLT").ToList();
                    var ONU = listaGeneral.FirstOrDefault(x => x.fcTipoArticulo == "ONU" && x.fiIDOrionSolicitud == model.fiIDOrionSolicitud);
                    foreach (var olt in OLTs)
                    {
                        var solicitudes = olt?.fcSolicitudes?.Split(',')?.Select(x => Convert.ToInt32(x))?.ToList() ?? new List<int>();
                        if (solicitudes.Any(x => x == model.fiIDOrionSolicitud))
                        {
                            model.fcIPOLT = olt.fcIP;
                            model.CodigoCepheus = ONU.fcCodigoCepheus;
                            model.fcPon = ONU.fcPom;
                            break;
                        }
                    }


                    //await ActualizarONUApi(model); //DESCOMENTAR


                    await ActualizarDispositivoONU(model);
                }

                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
                hubContext.Clients.All.actualizarONU(Data);

                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }


        [AllowAnonymous]
        [HttpGet]
        public JsonResult InformacionDispositivo(string fcTipoDispositivo, string propiedadUnica, string valor)
        {
            var listado = Dispositivos.GetIntervaloConexionDispositivo(fcTipoDispositivo, propiedadUnica, valor);
            return EnviarListaJson(listado);
        }


        public async Task ActualizarOLTApi(ListOLTViewModel model)
        {
            using (var client = new HttpClient())
            {
                //var listaSeteada = Dispositivos.LeerDispostivosObjeto();
                //model.fbConectado = listaSeteada.FirstOrDefault(x => x.fcTipoDispositivo == "OLT" && x.fiIDOrionSolicitud == model.fiIDOLT)?.fbConectado ?? false;
                var request = new HttpRequestMessage(HttpMethod.Post, "https://apitec.novanetgroup.com/api/Novanet/SignalR/ActulizarOLT");
                var content = new StringContent(JsonConvert.SerializeObject(model), null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task ActualizarONUApi(ListONUViewModel model)
        {
            using (var client = new HttpClient())
            {
                //var listaSeteada = Dispositivos.LeerDispostivosObjeto();
                //model.fbConectado = listaSeteada.FirstOrDefault(x => x.fcTipoDispositivo == "ONU" && x.fiIDOrionSolicitud == model.fiIDOrionSolicitud)?.fbConectado ?? false;
                var request = new HttpRequestMessage(HttpMethod.Post, "https://apitec.novanetgroup.com/api/Novanet/SignalR/ActulizarONU");
                var content = new StringContent(JsonConvert.SerializeObject(model), null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }


        [HttpGet]
        public ActionResult IndexAuditoria()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarListaClientesAuditoria()
        {
            using (var contexto = new ORIONDBEntities())
            {
                return EnviarListaJson(contexto.sp_Auditoria_ListarClientes().ToList());
            }
        }

        [HttpGet]
        public JsonResult CargarListaClientesAuditados()
        {
            using (var contexto = new ORIONDBEntities())
            {
                return EnviarListaJson(contexto.sp_Auditoria_ListaClientesAuditados().ToList());
            }
        }

        [HttpGet]
        public ActionResult ViewAgregarObservacion(int fiIDSolicitud, string fcNombre)
        {
            ViewBag.fiIDSolicitud = fiIDSolicitud;
            ViewBag.fcNombre = fcNombre;

            var model = new ObservacionAuditoriaViewModel { fiIDSolicitud = fiIDSolicitud, fcNombre = fcNombre };

            return PartialView(model);
        }

        [HttpPost]
        public JsonResult GuardarAuditoria(ObservacionAuditoriaViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var nombreArchivo = "";
                if (model.Documento != null)
                {
                    var algo = UploadFileServer148($@"Documento\Auditoria\Solicitud_{model.fiIDSolicitud}", model.Documento);
                    nombreArchivo = model.Documento.FileName;
                }

                if (model.Fotografia != null)
                {
                    var algo = UploadFileServer148($@"Documento\Auditoria\Solicitud_{model.fiIDSolicitud}", model.Fotografia);
                }
                var result = contexto.sp_AuditoriaContratistas_Insertar(model.fiIDSolicitud, 24, model.fnCantidad, model.fcObservacion, $@"C:\inetpub\novanetgroup.com_Orion\Documento\Auditoria\Solicitud_{model.fiIDSolicitud}\{model?.Documento?.FileName ?? ""}", GetIdUser(), $@"C:\inetpub\novanetgroup.com_Orion\Documento\Auditoria\Solicitud_{model.fiIDSolicitud}\{model?.Fotografia?.FileName ?? ""}", model.fcGeolocalizacionInicio, model.fcGeolocalizacionFinal) > 0;
                return EnviarResultado(result, "Auditoria Ingresada");
            }
        }


        [HttpGet]
        public ActionResult ViewHistorialConeccionONU(string fcTipoDispositivo, string propiedadUnica, string valor, string titulo)
        {
            ViewBag.Titulo = titulo;
            var listado = Dispositivos.GetIntervaloConexionDispositivo(fcTipoDispositivo, propiedadUnica, valor);
            return PartialView(listado);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult ViewHistorialDispositivos()
        {
            var fiCantidad = GetConfiguracion<int>("cantidad_historial");
            return EnviarListaJson(Dispositivos.GetIntervaloConexionesDispositivos(fiCantidad));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> ConsultarEstadoCliente(int fiIDSolicitud)
        {
            var cliente = Dispositivos.ObtenerDispositivoCliente(fiIDSolicitud);
            return Json(await Dispositivos.GetEstadoUsuarioCepheus(cliente.fcCodigoCableColor), JsonRequestBehavior.AllowGet);
        }



        


        #region Control de inventario

        public ActionResult IndexControlInvventario()
        {
            return View();
        }

        public ActionResult CrearConteoInventario()
        {
            ViewBag.ListUbicaciones = ListarUbicacionesPorTipo().Where(x => x.fcTipoUbicacion != "CLIENTE").Select(x => new SelectListItem { Value = x.fiIDUbicacion.ToString(), Text = x.fcUbicacion }).ToList();
            return View(new BitacoraInventarioViewModel());
        }


        [HttpGet]
        public JsonResult CargarListaBitacoraInventario()
        {
            using (var contexto = new ORIONDBEntities())
            {
                return EnviarListaJson(contexto.sp_Inventario_Bitacora_Maestro_Listar().ToList());
            }
        }

        [HttpGet]
        public JsonResult CargarConteoInventario(int IDUbicacion)
        {
            using (var contexto = new ORIONDBEntities())
            {

                var model = new BitacoraInventarioViewModel();
                model.fiIDUbicacion = IDUbicacion;
                model.Existencia = new List<ListInventarioViewModel>();

                var existencias = contexto.sp_Inventario_ExistenciaPorUbicacion(IDUbicacion).ToList();
                var inventario = contexto.sp_InventarioPorUbicacionDetallado(IDUbicacion).ToList();
                if (IDUbicacion > 0)
                {
                    foreach (var item in existencias)
                    {
                        var ModelExistencias = new ListInventarioViewModel();

                        ModelExistencias.fiIDProducto = item.fiIDProducto;
                        ModelExistencias.fcProducto = item.fcProducto;
                        ModelExistencias.fbGenerico = item.fbProductoGenerico ?? false;
                        ModelExistencias.fnCantidadSistema = item.fnCantidad;
                        ModelExistencias.fnDiferencia = item.fnCantidad;
                        ModelExistencias.Series = new List<ListSeriesViewModel>();
                        if (item.fbProductoGenerico == false)
                        {
                            ModelExistencias.Series = inventario.Where(x => x.fiIDProducto == item.fiIDProducto).Select(x => new ListSeriesViewModel { fiIDMovimiento = x.fiIDMovimiento ?? 0, fcCodigoSerie1 = x.fcCodigoSerie1, fcCodigoSerie2 = x.fcCodigoSerie2, fbFaltante = false, fiIDProducto = item.fiIDProducto }).ToList();
                        }

                        model.Existencia.Add(ModelExistencias);
                    }
                }


                return EnviarListaJson(model);
            }
        }

        [HttpGet]
        public ActionResult ViewListaDetalleBitacora(int fiIDInventarioBitacoraMaestro)
        {
            return PartialView(fiIDInventarioBitacoraMaestro);
        }

        [HttpGet]
        public JsonResult CargarListaDetalleBitacora(int fiIDInventarioBitacoraMaestro)
        {
            using (var contexto = new ORIONDBEntities())
            {
                return EnviarListaJson(contexto.sp_Inventario_Bitacora_Detalle_FiltrarPorIDInventarioBitacoraMaestro(fiIDInventarioBitacoraMaestro).ToList());
            }
        }


        [HttpPost]
        public JsonResult InsertarControlInventario(BitacoraInventarioViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var result = contexto.sp_Inventario_Bitacora_Maestro_Insertar(model.fiIDUbicacion, GetIdUser(), model.fcObservacion).FirstOrDefault() ?? 0;

                    if (result > 0)
                    {
                        foreach (var item in model.Existencia)
                        {
                            var resultado = contexto.sp_Inventario_Bitacora_Detalle_Insertar(result, item.fiIDProducto, item.fnCantidadSistema, item.fnCantidadFisico, item.fnDiferencia, item.fcComentario, GetIdUser());
                            if (item.Series != null)
                            {
                                foreach (var Serie in item.Series.Where(x => x.fbFaltante).ToList())
                                {
                                    var ResultadoFaltante = contexto.sp_Inventario_ActualizarFaltante(Serie.fiIDMovimiento);
                                }
                            }

                        }

                    }
                    return EnviarResultado(result > 0, result > 0 ? "Control Inventario Ingresado" : "Error");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }

            }

        }


        [HttpGet]
        [AllowAnonymous]
        public async Task ActualizarEstadoServicio(int fiIDSolicitud, int fiEstadoServicio)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var piIDSolicitud = new SqlParameter("@piIDSolicitud", fiIDSolicitud);
                var piEstadoServicio = new SqlParameter("@piEstadoServicio", fiEstadoServicio);
                await contexto.Database.ExecuteSqlCommandAsync("EXEC sp_OrionSolicitud_Maestro_ActualizarEstadoServicio @piIDSolicitud, @piEstadoServicio", piIDSolicitud, piEstadoServicio);
            }
        }


        #endregion

        #region Mapas

        public ActionResult MapaCobertura()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> CargarCoberturas()
        {
            FileKMZService kmzService = new FileKMZService();
            //await kmzService.LoadKMZFromUrl(MemoryLoadManager.UrlWeb + GetConfiguracion<string>("Coberturas"));
            await kmzService.LoadKMZFromPath(GetConfiguracion<string>("Ruta_Coberturas"));

            var jsonResult = Json(kmzService.KmlContentArray, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;
        }

            
        [HttpGet]
        public async Task<JsonResult> CargarMufasKMZ()
        {
            FileKMZService kmzService = new FileKMZService();
            await kmzService.LoadKMZFromPath(GetConfiguracion<string>("Ruta_Mufas"));
            var jsonResult = Json(kmzService.KmlContentArray, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = Int32.MaxValue;
            return jsonResult;
        }


        [HttpGet]
        [AllowAnonymous]
        public string CreateURLMap(int fiIDUsuario = 0, double fiLongitud = 0, double fiLatitud = 0)
        {
            return HttpUtility.UrlEncode(DataCrypt.Encriptar($"idUsuario={fiIDUsuario}&longitud={fiLongitud}&latitud={fiLatitud}"));
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult MapaMufas(string url)
        {
            var desencriptado = DataCrypt.Desencriptar(url).Split('&').Select(part => part.Split('=')).ToDictionary(split => split[0], split => split[1]);

            var idUsuario = Convert.ToInt32(desencriptado["idUsuario"]);
            ViewBag.fiLongitud = Convert.ToDouble(desencriptado["longitud"]);
            ViewBag.fiLatitud = Convert.ToDouble(desencriptado["latitud"]);
            ViewBag.fiIDUsuario = idUsuario;

            var usuario = MemoryLoadManager.ListaUsuarios.FirstOrDefault(x => x.fiIdUsuario == idUsuario);
            if (usuario != null)
            {
                usuario.fnLongitud = Convert.ToDouble(desencriptado["longitud"]);
                usuario.fnLatitud = Convert.ToDouble(desencriptado["latitud"]);

                EscribirEnLogJson($"{usuario.fcNombreCorto}, lat: {usuario.fnLatitud}, long: {usuario.fnLongitud}");
            }




            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public JsonResult ActualizarUbicacion(int fiIDUsuario, double lat, double lon)
        {
            var usuario = MemoryLoadManager.ListaUsuarios.FirstOrDefault(x => x.fiIdUsuario == fiIDUsuario);
            if (usuario != null)
            {
                usuario.fnLongitud = Convert.ToDouble(lon);
                usuario.fnLatitud = Convert.ToDouble(lat);

                EscribirEnLogJson($"{usuario.fcNombreCorto}, lat: {usuario.fnLatitud}, long: {usuario.fnLongitud}");
            }
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> CargarMufas(double fiLongitud = 0, double fiLatitud = 0, int fiIDUsuario = 0)
        {
            try
            {
                using (var conetion = new ORIONDBEntities())
                {
                    FileKMZService kmzService = new FileKMZService();
                    // await kmzService.LoadKMZFromUrl(GetConfiguracion<string>("UrlKMZ"));


                    await kmzService.LoadKMZFromPath(GetConfiguracion<string>("Ruta_Mufas"));
                    var listaInfoOnus = new List<sp_MapaConInfoOnuyOlt_Result>();

                    foreach (var item in kmzService.Placemarks)
                    {
                        listaInfoOnus.Add(new sp_MapaConInfoOnuyOlt_Result
                        {
                            fcNombreCliente = item.Name,
                            fcGeolocalizacion = item.Latitude + "," + item.Longitude,
                            fcDescripcion = "MUFA",
                            fcRGB = "128,128,0"
                        });
                    }

                    listaInfoOnus.Add(new sp_MapaConInfoOnuyOlt_Result
                    {
                        fcNombreCliente = User?.Identity?.Name ?? "USUARIO",
                        fcDescripcion = "Mi Ubicacion",
                        fcRGB = "0,255,255",
                        fcGeolocalizacion = $"{fiLatitud},{fiLongitud}"
                    });

                    var lista = EnviarListaJson(listaInfoOnus);
                    return lista;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        [HttpGet]
        public ActionResult MapaSoporteTecnicos()
        {
            var prueba = DataCrypt.Encriptar("Hola, mundo!");
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> CargarPuntosSoporte()
        {
            try
            {
                FileKMZService kmzService = new FileKMZService();
                // await kmzService.LoadKMZFromUrl(GetConfiguracion<string>("UrlKMZ"));


                await kmzService.LoadKMZFromPath(GetConfiguracion<string>("Ruta_Mufas"));
                var listaInfoOnus = new List<sp_MapaConInfoOnuyOlt_Result>();

                foreach (var item in kmzService.Placemarks)
                {
                    listaInfoOnus.Add(new sp_MapaConInfoOnuyOlt_Result
                    {
                        fcNombreCliente = item.Name,
                        fcGeolocalizacion = item.Latitude + "," + item.Longitude,
                        fcDescripcion = "MUFA",
                        fcRGB = "128,128,0"
                    });
                }

                var listaUsuarios = MemoryLoadManager.ListaUsuarios.Where(x => x.fnLongitud != 0).ToList();
                foreach (var item in listaUsuarios)
                {
                    listaInfoOnus.Add(new sp_MapaConInfoOnuyOlt_Result
                    {
                        fcNombreCliente = item.UserName,
                        fcDescripcion = "Tecnico",
                        fcRGB = "0,255,255",
                        fcGeolocalizacion = $"{item.fnLatitud},{item.fnLongitud}"
                    });
                }



                var lista = EnviarListaJson(listaInfoOnus);
                return lista;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpGet]
        public ActionResult MapaConectividad()
        {
            ViewBag.idUsuario = GetIdUser();
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> CargarPuntosConectividad()
        {
            using (var contexto = new ORIONDBEntities())
            {

                //FileKMZService kmzService = new FileKMZService();
                //await kmzService.LoadKMZFromPath(GetConfiguracion<string>("Ruta_Mufas"));

                var listaInfoOnus = new List<MapaConectividadViewModel>();
                var listaDispositivos = Dispositivos.LeerDispostivosObjeto();
                MemoryLoadManager.ListaOLTsONUs = await GetListaOnusYOltsAsync();//contexto.sp_ONUsYOLTs_Listar().ToList();
                var listaOLT = MemoryLoadManager.ListaOLTsONUs.Where(x => x.fcTipoArticulo == "OLT").ToList();
                var listaONU = MemoryLoadManager.ListaOLTsONUs.Where(x => x.fcTipoArticulo == "ONU").ToList();
                var listaMapas = contexto.sp_MapaConInfoOnuyOlt().ToList();

                var listaModel = new List<ListInfoOLTViewModel>();

                foreach (var OLT in listaOLT)
                {


                    //var model = new ListInfoOLTViewModel
                    //{
                    //    fiIDOLT = OLT.fiIDDetalleCliente_Servicio,
                    //    fcIP = OLT.fcIP,
                    //    listaONUS = new List<ListONUViewModel>(),
                    //    ListInfoONU = new List<ListInfoONUViewModel>(),
                    //    fcNombre = OLT.fcNombre,
                    //    fbConectado = OLT.fbConectado ?? false,
                    //    fbEstado = OLT.fbConectado ?? null,
                    //    fdUltimaActualizacion = OLT.fdFechaRegitro
                    //};



                    var solicitudes = OLT?.fcSolicitudes?.Split(',')?.Select(x => Convert.ToInt32(x))?.ToList() ?? new List<int>();

                    foreach (var ONU in listaONU.Where(x => solicitudes.Any(y => y == x.fiIDOrionSolicitud)).ToList())
                    {

                        //var onuDispositivo = listaDispositivos.FirstOrDefault(y => y.fcTipoDispositivo == "ONU" && y.fiIDOrionSolicitud == ONU.fiIDOrionSolicitud);

                        var clienteMap = listaMapas.FirstOrDefault(y => y.fiIDSolicitud == ONU.fiIDOrionSolicitud);
                        listaInfoOnus.Add(new MapaConectividadViewModel
                        {
                            fcDescripcionOLT = OLT.fcNombre,
                            fcDescripcion = ONU.fiDiasAtraso > 0 ? "Desconectado" : ONU.fbConectado == null ? "Pendiente" : ONU.fbConectado == true ? "Conectado" : "Sin Servicio",
                            fcGeolocalizacion = (ONU?.fcGeolocalizacion?.Replace(" ", "")?.Replace(",", "") ?? "") != "" ? ONU?.fcGeolocalizacion?.Replace(" ", "") ?? "" : "",
                            fcNombreCliente = ONU.fcNombre,
                            fcTelefono = ONU?.fcTelefono ?? "",
                            fcIPOlt = OLT.fcIP,
                            fcIpOnu = ONU.fcIP,
                            fiIDSolicitud = ONU.fiIDOrionSolicitud,
                            fcPomOnu = ONU?.fcPom ?? "",
                            fiCodDepartamento = clienteMap?.fiCodDepartamento ?? 0,
                            fcDepartamento = clienteMap?.fcDepartamento ?? "",
                            fiCodMunicipio = clienteMap?.fiCodMunicipio ?? 0,
                            fcMunicipio = clienteMap?.fcMunicipio ?? "",
                            fiCodColonia = clienteMap?.fiCodColonia ?? 0,
                            fcBarrio = clienteMap?.fcBarrio ?? "",
                            fbEstadoServicio = ONU.fiEstadoServicio == 1,
                            fiIDOLT = OLT.fiIDDetalleCliente_Servicio,
                            fbOLTConectado = OLT.fbConectado ?? false,
                            fiDiasAtraso = ONU.fiDiasAtraso,
                            fnCapitalAtrasado = ONU.fnCapitalAtrasado,
                            fcCodigoCepheus = ONU.fcCodigoCepheus,
                            fbONUConectado = ONU.fbConectado ?? false,
                            fcEmpresa = ONU.fcEmpresa,
                            fdUltimaActualizacion = ONU.fdFechaRegitro,
                            fcEstadoPing = ONU.fcEstadoPing
                        });


                    }





                }

                return EnviarListaJson(listaInfoOnus);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult UbicarClienteMapa(int fiIDUsuario, int fiIDSolicitud)
        {
            var model = new { fiIDUsuario, fiIDSolicitud };
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.ubicarClienteMapa(model);


            return Json("ok", JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult DoPing(string fcIP, string fcNombre, string fcDispositivo)
        {
            ViewBag.fiIDUsuario = GetIdUser();
            ViewBag.fcIP = fcIP;
            ViewBag.fcTitulo = fcDispositivo == "ONU" ? $"Ping a la IP {fcIP} del cliente {fcNombre}" : $"Ping a la IP {fcIP} de la OLT {fcNombre}";
            return PartialView();
        }


        [HttpGet]
        public async Task EnviarPingApi(string fcIP, string fiIDUser)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://apitec.novanetgroup.com/api/Novanet/SignalR/RecibirPingOrion?fcIp={fcIP}&fiIDUsuario={fiIDUser}");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }

        


        [HttpPost]
        [AllowAnonymous]
        public JsonResult RecibirPing(PingIndividualViewModel model)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.recibirPing(model);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]

        public ActionResult DataOnu(string fcIP)
        {
            var onu = MemoryLoadManager.ListaOLTsONUs.FirstOrDefault(x => x.fcIP == fcIP);
            ViewBag.fiIDUsuario = GetIdUser();
            ViewBag.fcIP = fcIP;
            ViewBag.fcNombre = onu.fcNombre;
            return PartialView();
        }


        [HttpGet]
        public async Task ConsultarDataOnu(string fcIP, string fiIDUser)
        {
            using (var client = new HttpClient())
            {
                var onu = MemoryLoadManager.ListaOLTsONUs.FirstOrDefault(x => x.fcIP == fcIP);
                //fcIP, fcUser, fcPassword, fcLogin, fiIDUsuario
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://apitec.novanetgroup.com/api/Novanet/SignalR/RecibirInfoONUOrion?fcIp={fcIP}&fcUser={onu.fcUsuarioONU}&fcPassword={onu.fcContrasenia}&fcLogin={onu.fcURLLogin}&fiIDUsuario={fiIDUser}");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult RecibirDataOnu(EstadisticasONU1ViewModel model)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.recibirDataOnu(model);
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult MapasKMZ()
        {
            var kmzService = new FileKMZService();
            var archivos = kmzService.GetInfoArchivos(GetConfiguracion<string>("Ruta_Coberturas"));
            archivos.AddRange(kmzService.GetInfoArchivos(GetConfiguracion<string>("Ruta_Mufas")));

            archivos = archivos.GroupBy(r => Path.GetFileName(r.fcNombreArchivo).ToLower()).Select(grp => grp.First()).ToList();

            ViewBag.ListaArchivos = archivos.Select(x => new SelectListItem { Value = x.fcNombreArchivo, Text = $"{Path.GetFileName(x.fcNombreArchivo)} {x.fdFechaCreacion:dd/MM/yyyy}" }).ToList();

            return View();
        }

        [HttpGet]
        public async Task<JsonResult> CargarMapaMKZ(string archivo)
        {
            var kmzService = new FileKMZService();
            var data = await kmzService.DescargarKMZ(archivo);
            return EnviarListaJson(data);
        }

        #endregion
    }
}

