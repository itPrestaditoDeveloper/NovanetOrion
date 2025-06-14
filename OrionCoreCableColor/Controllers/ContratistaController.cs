using Newtonsoft.Json;
using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.App_Services.EmailService;
using OrionCoreCableColor.DbConnection.CMContext;
using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models.Base;
using OrionCoreCableColor.Models.Contratista;
using OrionCoreCableColor.Models.EmailTemplateService;
using OrionCoreCableColor.Models.Solicitudes;
using OrionCoreCableColor.Models.Tecnico;
using Spire.Doc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.Controllers
{
    [System.Web.Mvc.Authorize]
    public class ContratistaController : BaseController
    {
        private readonly DbServiceConnection _connection;
        public SendEmailService ServicioCorreo;
        public EmailTemplateService _EmailTemplate;

        public static Dictionary<string, string> DictionaryList = new Dictionary<string, string>();

        //public IHubContext<NotificacionesHub> _notificacion;
        // private static IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
        public ContratistaController()
        {
            var ConnectionString = ConfigurationManager.ConnectionStrings["OrionConnections"].ConnectionString;
            _connection = new DbServiceConnection(ConnectionString);
            ServicioCorreo = new SendEmailService();

            // HubContext =  IHubContext<NotificacionesHub>;
            //_notificacion = new IHubContext<NotificacionesHub>;

        }
        enum Roles
        {
            Admin,
            OtroUsuario,

        };
        // GET: Contratista
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ContratistaBandeja()

        {
            try
            {

                // var NotificacionesHub = new NotificacionesHub();
                //NotificacionesHub.AgregarSesion();



                //hubContext.Clients.All.enviarNotificacion("mensaje desde controller ");

                //HubContext.Clients.All.EnviarNotificacion("mensaje desde controller ");
                // notificacion.EnviarNotificacion("MENSAJE EDUARDO");
                var RolesAdmin = GetConfiguracion<int>("Orion_Admin", ',');
                ViewBag.RolesAdmin = RolesAdmin.Contains(GetUser().IdRol);
                ViewBag.IdInfoUser = GetUser();

                return View();
            }
            catch (Exception e)
            {

                throw;
            }
            //var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();

        }

        public ActionResult SolicitudInstalacionNotas(int fiIDContratistaSolicitud, bool fbEditar = false)
        {
            ContratistaSolicitudInstalacionNota model = new ContratistaSolicitudInstalacionNota();
            model.FiIDContratistaSolicitud = fiIDContratistaSolicitud;
            model.fbEditar = fbEditar;

            using (var context = new ORIONDBEntities())
            {
                var connection = (SqlConnection)context.Database.Connection;
                var command = new SqlCommand("sp_Contratista_SolicitudInstalacion_Notas_ListarPorID", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@piIDContratistaSolicitud", fiIDContratistaSolicitud);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var objectContext = ((IObjectContextAdapter)context).ObjectContext;
                    var notasList = objectContext.Translate<ContratistaSolicitudInstalacionNota>(reader).ToList();

                    foreach (var nota in notasList)
                    {
                        nota.fbEditar = fbEditar;
                    }

                    ViewBag.Notas = notasList;
                }
                connection.Close();
            }

            return PartialView(model);
        }


        [HttpPost]
        public JsonResult CrearNota()
        {
            try
            {
                // Deserializar el objeto notaModel enviado desde el frontend
                var notaModelJson = Request.Form["notaModel"];
                if (string.IsNullOrEmpty(notaModelJson))
                    return EnviarResultado(false, "Validación", "El modelo de nota no puede estar vacío.");

                var notaModel = JsonConvert.DeserializeObject<ContratistaSolicitudInstalacionNota>(notaModelJson);

                using (var context = new ORIONDBEntities())
                {
                    var connection = (SqlConnection)context.Database.Connection;
                    var command = new SqlCommand("sp_Contratista_SolicitudInstalacion_Notas_InsertarNota", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    // Validar parámetros
                    if (string.IsNullOrWhiteSpace(notaModel.FcComentarioNota))
                        return EnviarResultado(false, "Validación", "El comentario no puede estar vacío.");
                    if (notaModel.FiIDContratistaSolicitud <= 0)
                        return EnviarResultado(false, "Validación", "El ID de la solicitud del contratista no es válido.");



                    // Agregar parámetros al comando
                    command.Parameters.AddWithValue("@fiIDContratistaSolicitud", notaModel.FiIDContratistaSolicitud);
                    command.Parameters.AddWithValue("@fcComentarioNota", notaModel.FcComentarioNota);
                    command.Parameters.AddWithValue("@fiIDUsuarioCreacion", GetIdUser());

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int codeStatus = reader.GetInt32(reader.GetOrdinal("CodeStatus"));
                            string messageStatus = reader.GetString(reader.GetOrdinal("MessageStatus"));

                            connection.Close();

                            if (codeStatus == 200)
                            {
                                return EnviarResultado(true, "", messageStatus);
                            }
                            else
                            {
                                return EnviarResultado(false, "Error", messageStatus);
                            }
                        }
                        else
                        {
                            connection.Close();
                            return EnviarResultado(false, "Error", "No se recibió respuesta del procedimiento.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return EnviarException(e, "Error al crear la nota");
            }
        }


        [HttpPost]
        public JsonResult EditarNota()
        {
            try
            {
                // Deserializar el objeto notaModel enviado desde el frontend
                var notaModelJson = Request.Form["notaModel"];
                if (string.IsNullOrEmpty(notaModelJson))
                    return EnviarResultado(false, "Validación", "El modelo de nota no puede estar vacío.");

                var notaModel = JsonConvert.DeserializeObject<ContratistaSolicitudInstalacionNota>(notaModelJson);

                using (var context = new ORIONDBEntities())
                {
                    var connection = (SqlConnection)context.Database.Connection;
                    var command = new SqlCommand("sp_Contratista_SolicitudInstalacion_Notas_EditarNota", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    // Validar parámetros
                    if (string.IsNullOrWhiteSpace(notaModel.FcComentarioNota))
                        return EnviarResultado(false, "Validación", "El comentario no puede estar vacío.");


                    // Agregar parámetros al comando
                    command.Parameters.AddWithValue("@fiIDSolicitudInstalacionNota", notaModel.FiIDSolicitudInstalacionNota);
                    command.Parameters.AddWithValue("@fcComentarioNota", notaModel.FcComentarioNota);
                    command.Parameters.AddWithValue("@fiIDUsuarioUltimaModificacion", GetIdUser());

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int codeStatus = reader.GetInt32(reader.GetOrdinal("CodeStatus"));
                            string messageStatus = reader.GetString(reader.GetOrdinal("MessageStatus"));

                            connection.Close();

                            if (codeStatus == 200)
                            {
                                return EnviarResultado(true, "", messageStatus);
                            }
                            else
                            {
                                return EnviarResultado(false, "Error", messageStatus);
                            }
                        }
                        else
                        {
                            connection.Close();
                            return EnviarResultado(false, "Error", "No se recibió respuesta del procedimiento.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return EnviarException(e, "Error al actualizar la nota");
            }
        }


        public JsonResult ListaSolicitudes()
        {
            try
            {
                var jsonResult = new JsonResult();
                var Roles_Contratista_Bandeja = GetConfiguracion<string>("Roles_Contratista_Bandeja", ',').Select(x => _connection.OrionContext.sp_Roles_ObtenerPorRole(x).FirstOrDefault()).ToList();

                var usuaruiologueado = GetUser();
                var listadoTecnico = _connection.OrionContext.sp_SolicitudesAsignadas_By_Tecnico(usuaruiologueado.fiIdUsuario, usuaruiologueado.IdRol).Where(a => a.fiIDTipoSolicitud == 1 || a.fiIDTipoSolicitud == 4).ToList();

                jsonResult = EnviarListaJson(listadoTecnico);

                return jsonResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public JsonResult ListaSolicitudesPendientes()
        {
            try
            {
                //var jsonResult = new JsonResult();
                //var Roles_Contratista_Bandeja = GetConfiguracion<string>("Roles_Contratista_Bandeja", ',').Select(x => _connection.OrionContext.sp_Roles_ObtenerPorRole(x).FirstOrDefault()).ToList();
                //var contratistaLogeado = GetIdContratista();
                //var empresa = _connection.OrionContext.sp_ObtenerEmpresa_ByIdUsuario(GetIdUser()).FirstOrDefault();
                //var lista = _connection.OrionContext.sp_SolicitudesInstalacion_Lista().Where(x=>x.fcDescripcion == "Pendiente" && x.fiIDTipoSolicitud == 1).ToList();
                //foreach (var item in lista)
                //{
                //    item.fcPaquete = _connection.OrionContext.sp_InformacionInstalacion_Tecnico(item.fiIDSolicitudInstalacion).FirstOrDefault().fcArticulosdelContrato ?? "";
                //}
                //Si es Admin
                //if (Roles_Contratista_Bandeja.Any(x => x.Pk_IdRol == GetUser().IdRol))
                //{
                //    jsonResult = EnviarListaJson(lista);
                //}
                //else
                //{
                //    jsonResult = EnviarListaJson(lista.Where(x => x.fiIdEmpresa == empresa).ToList());
                //    // jsonResult = EnviarListaJson(lista.Where(x=>x.fiIDContratista==contratistaLogeado).ToList());
                //}

                var Usuario = GetUser();
                var listado = _connection.OrionContext.sp_ContratistaSolicitudInstalacion_ById(Usuario.fiIdUsuario, Usuario.IdRol).Where(a => a.fiIDEstadoInstalacion == 1 && a.fiIDTipoSolicitud == 1).ToList();


                return EnviarListaJson(listado);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public JsonResult ListaSolicitudesCanceladas()
        {
            try
            {
                //var jsonResult = new JsonResult();
                //var Roles_Contratista_Bandeja = GetConfiguracion<string>("Roles_Contratista_Bandeja", ',').Select(x => _connection.OrionContext.sp_Roles_ObtenerPorRole(x).FirstOrDefault()).ToList();
                //var contratistaLogeado = GetIdContratista();
                //var empresa = _connection.OrionContext.sp_ObtenerEmpresa_ByIdUsuario(GetIdUser()).FirstOrDefault();
                //var lista = _connection.OrionContext.sp_SolicitudesInstalacion_Lista().Where(x=>x.fcDescripcion == "Pendiente" && x.fiIDTipoSolicitud == 1).ToList();
                //foreach (var item in lista)
                //{
                //    item.fcPaquete = _connection.OrionContext.sp_InformacionInstalacion_Tecnico(item.fiIDSolicitudInstalacion).FirstOrDefault().fcArticulosdelContrato ?? "";
                //}
                //Si es Admin
                //if (Roles_Contratista_Bandeja.Any(x => x.Pk_IdRol == GetUser().IdRol))
                //{
                //    jsonResult = EnviarListaJson(lista);
                //}
                //else
                //{
                //    jsonResult = EnviarListaJson(lista.Where(x => x.fiIdEmpresa == empresa).ToList());
                //    // jsonResult = EnviarListaJson(lista.Where(x=>x.fiIDContratista==contratistaLogeado).ToList());
                //}

                var Usuario = GetUser();
                var listado = _connection.OrionContext.sp_ContratistaSolicitudInstalacion_ById(Usuario.fiIdUsuario, Usuario.IdRol).Where(a => a.fiIDEstadoInstalacion == 5 && a.fiIDTipoSolicitud == 1).ToList();


                return EnviarListaJson(listado);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public JsonResult ListaIncidenciasSolicitudes()
        {
            try
            {
                List<IncidenciasViewModel> result = new List<IncidenciasViewModel>();
                var lista = _connection.OrionContext.sp_SolicitudesIncidencias_Lista().ToList();
                foreach (var item in lista)
                {
                    var list = JsonConvert.DeserializeObject<IncidenciasViewModel>(item);
                    result.Add(list);
                }

                //using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["OrionConnections"].ToString()))
                //{
                //    await connection.OpenAsync();

                //    using (SqlCommand command = new SqlCommand("sp_SolicitudesIncidencias_Lista", connection))
                //    {
                //        //command.Parameters.AddWithValue("IdInsidencia", incidencia);
                //        //command.CommandType = CommandType.StoredProcedure;
                //        command.CommandTimeout = 0;

                //        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                //        {
                //            while (await reader.ReadAsync())
                //            {
                //                string json = reader.GetString(0);
                //                IncidenciasViewModel item = JsonConvert.DeserializeObject<IncidenciasViewModel>(json);
                //                result.Add(item);
                //            }
                //        }
                //    }
                //}

                return EnviarListaJson(result);//Json(result, JsonRequestBehavior.AllowGet);
                //IncidenciasViewModel item = JsonConvert
            }
            catch (Exception)
            {

                throw;
            }

        }

        public ActionResult ShowAsignacionContratista(int fiIDSolicitudInstalacion, int fiIDContratista, string Cliente, string ubicacion)
        {

            try
            {
                var empresa = _connection.OrionContext.sp_ObtenerEmpresa_ByIdUsuario(GetIdUser()).FirstOrDefault();
                ViewBag.ListadoTecnico = _connection.OrionContext.sp_TecnicosPorContratista_GetByIdContratista(empresa).ToList().Select(x => new { Value = x.fiIDUsuarioTecnico, Text = x.fcTecnico }).ToList();
                return PartialView(new contratistaAsignacionViewModel() { fiIDSolicitudInstalacion = fiIDSolicitudInstalacion, cliente = Cliente, ubicacion = ubicacion });
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet]
        public ActionResult ShowModalDenegarArticulos(int fiIDSolicitud, int fiIDSolicitudInstalacion, string fcNombreCliente, string fcArticulosdelContrato, string fcDescripcion, string fcClase)
        {
            try
            {


                return PartialView(new DetalleInstalacionViewModel()
                {
                    fiIDSolicitud = fiIDSolicitud,
                    fiIDContratistaSolicitud = fiIDSolicitudInstalacion,
                    fcNombre = fcNombreCliente,
                    fcArticulosdelContrato = fcArticulosdelContrato,
                    fcDescripcion = fcDescripcion,
                    fcClase = fcClase
                });

            }
            catch (Exception)
            {
                throw;
            }
        }


        [ValidateInput(false)]
        [AllowAnonymous]
        public async Task<ActionResult> DenegarArticulos(int fiIDSolicitud, int fiIDContratistaSolicitud, string nombreCliente, string fcComentario, string fcArticulosdelContrato)
        {
            try
            {
                using (var dbContext = new ORIONDBEntities())
                using (var connection = dbContext.Database.Connection)
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "EXEC sp_Contratista_ArticulosCancelacion_Actualizar @fiIDSolicitud"; command.Parameters.Add(new SqlParameter("@fiIDSolicitud", fiIDSolicitud)); command.ExecuteNonQuery();
                    }
                    var InformacionSoporte = _connection.OrionContext.sp_Configuraciones().Where(a => a.NombreLlave == "CorreoTecnicoJefe" || a.NombreLlave == "CorreoJefeVentas" || a.NombreLlave == "CorreoSistemasGeneral").ToList();

                    var modelCorreo = new SendEmailViewModel
                    {
                        DestinationEmail = "brayan.sierra@miprestadito.com",
                        Subject = "Cancelación de Entrega de Artículos",
                        Body = $"Se ha cancelado la entrega de los siguientes artículos en la solicitud {fiIDSolicitud}.<br /><br />" +
                               $"<strong>Cliente:</strong> {nombreCliente}<br />" +
                               $"<strong>Artículos:</strong> {fcArticulosdelContrato}<br /><br />" +
                               $"<strong>Motivo de cancelación:</strong> {fcComentario}<br /><br />" +
                               $"<strong>Se ha cambiado el estado:</strong> Validado por Soporte",
                        EmailName = "Novanet"
                    };

                    modelCorreo.List_CC.AddRange(InformacionSoporte.Select(s => s.ValorLLave));
                    await ServicioCorreo.SendEmailAsync(modelCorreo);
                    _connection.OrionContext.sp_Solicitud_Instalacion_Bitacoras_Crear(fiIDContratistaSolicitud, $"Entrega de artículos cancelada: {fcComentario}", GetIdUser());
                }

                return Json(new { success = true, message = "Comentario guardado y notificación enviada" });
            }
            catch (Exception e)
            {
                return EnviarException(e, "Error al guardar el comentario y enviar la notificación");
            }
        }





        public ActionResult ShowAsignacionContratistaOrden(int fiIDSolicitudInstalacion, int fiIDContratista, string Cliente, string ubicacion)
        {

            try
            {
                var empresa = _connection.OrionContext.sp_ObtenerEmpresa_ByIdUsuario(GetIdUser()).FirstOrDefault();
                ViewBag.ListadoTecnico = _connection.OrionContext.sp_TecnicosPorContratista_GetByIdContratista(empresa).ToList().Select(x => new { Value = x.fiIDUsuarioTecnico, Text = x.fcTecnico }).ToList();
                return PartialView(new contratistaAsignacionViewModel() { fiIDSolicitudInstalacion = fiIDSolicitudInstalacion, cliente = Cliente, ubicacion = ubicacion });
            }
            catch (Exception)
            {

                throw;
            }

        }
        public ActionResult ShowAsignacionContratistaSOP(int fiIDSolicitudInstalacion, string cliente, int fiSolicitud)
        {
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {

                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_OrionSolicitud_LlenarListas {1} ";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    var List = db.ObjectContext.Translate<SolicitudesViewModel>(reader).ToList();
                    ViewBag.Listado1 = List.Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDAgencia), Text = z.fcNombreAgencia }).ToList();
                    reader.NextResult();
                    var List2 = db.ObjectContext.Translate<SolicitudesViewModel>(reader).ToList();
                    ViewBag.Listado2 = List2.Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDContratista), Text = z.fcNombreEmpresa }).ToList();

                }
                connection.Close();
            }


            return PartialView(new SolicitudesViewModel { Nombre = cliente, IDSolicitud = fiSolicitud, fiIDSolicitudInstalacion = fiIDSolicitudInstalacion });

        }

        [ValidateInput(false)]
        [AllowAnonymous]
        public async Task<ActionResult> AsignarSolicitudContratista(int fiIDSolicitudInstalacion, int fiIDTecnico, string cliente, string ubicacion)
        {
            try
            {
                // ubicacion = ubicacion.Replace('A', ',');
                var actualizaraAsignado = _connection.OrionContext.sp_ActualizarInstalacion_Tecnico(fiIDSolicitudInstalacion, 2, 0, "_");
                var InformacionPaquete = _connection.OrionContext.sp_InformacionInstalacion_Tecnico(fiIDSolicitudInstalacion).FirstOrDefault();
                var Asignar = _connection.OrionContext.sp_AsignarSolicitud_Contratista(fiIDSolicitudInstalacion, fiIDTecnico) > 0;
                string informacion = $"Servicio a instalar: {InformacionPaquete.fcArticulosdelContrato} ";
                var InformacionTecnico = _connection.OrionContext.sp_InformacionTecnico(fiIDTecnico).FirstOrDefault();
                var InfoCliente = _connection.OrionContext.sp_InformacionCliente_BySolicitudInstalacion(fiIDSolicitudInstalacion).FirstOrDefault();
                if (Asignar)
                {
                    string lat = ubicacion.Split(',')[0];
                    string longi = ubicacion.Split(',')[1];
                    string urlMaps = $"https://www.google.com/maps?z=12&t=k&q={lat},{longi}";
                    //email a tecnico
                    var modelCorreo = new SendEmailViewModel();
                    modelCorreo.DestinationEmail = InformacionTecnico.fcCorreoTecnico;
                    // modelCorreo.DestinationEmail = "denis.saavedra@miprestadito.com";    
                    modelCorreo.Subject = "Instalacion de servicio";
                    var bod = "Estimado " + InformacionTecnico.fcNombreTecnico + " se le informa que se le ha asignado una instalacion " + ",Cliente: " + cliente + "" + informacion + "Ubicacion:" + urlMaps;
                    modelCorreo.Body = "Estimado " + InformacionTecnico.fcNombreTecnico + " se le informa que se le ha asignado una instalacion " + ",<br>Cliente: <b>" + cliente + "</b><br>" + informacion + "<br>Ubicacion:" + urlMaps; ;
                    modelCorreo.EmailName = "Novanet";
                    var correoTecnicoJefe = _connection.OrionContext.sp_Configuraciones().FirstOrDefault(a => a.NombreLlave == "CorreoTecnicoJefe").ValorLLave;
                    modelCorreo.List_CC.Add("brayan.sierra@miprestadito.com");
                    await ServicioCorreo.SendEmailAsync(modelCorreo);
                    //telegram a tecnico

                    var numeroTecnicoJefe = _connection.OrionContext.sp_Configuraciones().FirstOrDefault(a => a.NombreLlave == "NumeroTecnicoJefe").ValorLLave;

                    //sms a cliente
                    MensajeriaApi.EnviarMensajeInstalacionaCliente(cliente, InformacionPaquete.fiIDSolicitud ?? 0, InfoCliente.fcTelefono, InformacionTecnico.fcNombreTecnico.Replace('.', ' '), informacion, InformacionTecnico.fcTelefonoMovil, InformacionTecnico.fcIdentidadTecnico);

                    //mensaje al Tecnico Superior
                    MensajeriaApi.MensajesDigitales(numeroTecnicoJefe, bod);
                    //mensaje a los tecnicos 
                    MensajeriaApi.MensajesDigitales(InformacionTecnico.fcTelefonoMovil, bod);


                    //MensajeDeTexto.EnviarMensajeInstalacionaCliente(cliente, InfoCliente.fiIDEquifax ?? 0, InfoCliente.fcTelefono, InformacionTecnico.fcNombreTecnico.Replace('.', ' '), informacion, InformacionTecnico.fcTelefonoMovil,InformacionTecnico.fcIdentidadTecnico);
                    //MensajeDeTexto.EnviarMensajeInstalacionaCliente(cliente, InfoCliente.fiIDEquifax ?? 0, "99521461", InformacionTecnico.fcNombreTecnico.Replace('.', ' '), informacion, InformacionTecnico.fcTelefonoMovil,InformacionTecnico.fcIdentidadTecnico);


                    //email a cliente
                    var modelCorreoCliente = new SendEmailViewModel();

                    modelCorreoCliente.DestinationEmail = InfoCliente.fcCorreo;
                    //modelCorreoCliente.DestinationEmail = "denis.saavedra@miprestadito.com";
                    modelCorreoCliente.Subject = "Instalacion de servicio";
                    modelCorreoCliente.Body = "Estimado  " + cliente + " se le notifica que la instalacion de su servicio sera realizada por el tecnico: " + "<br>" + InformacionTecnico.fcNombreTecnico.Replace('.', ' ') + "</b>,Con identidad " + InformacionTecnico.fcIdentidadTecnico + "<br>Se contactara con usted del telefono: " + InformacionTecnico.fcTelefonoMovil + "<br>Se le adjunta el codigo QR de su instalacion:" + MemoryLoadManager.UrlWeb + "/DatosCliente/ViewFormQR/" + InformacionPaquete.fiIDSolicitud;
                    modelCorreoCliente.EmailName = "Novanet";
                    await ServicioCorreo.SendEmailAsync(modelCorreoCliente);
                    //telegram a cliente


                    //Generar bitacora
                    var bitacora = _connection.OrionContext.sp_Solicitud_Instalacion_Bitacoras_Crear(fiIDSolicitudInstalacion, "Se asigno al tecnico " + InformacionTecnico.fcNombreTecnico, GetIdUser());
                    var tecnicoaMostrarNotif = _connection.OrionContext.sp_GetUsuario_Tecnico(fiIDTecnico).FirstOrDefault();

                    //EnviarNotificacion($"Se te asigno una instalacion, revisa tu bandeja {  InformacionTecnico.fcNombreTecnico }");
                }
                return EnviarResultado(Asignar, "Se asigno correctamente la instalación");

            }
            catch (Exception e)
            {

                return EnviarException(e, "Error");
            }
        }

        [ValidateInput(false)]
        public async Task<ActionResult> AsignarSolicitudContratistaSinQR(int fiIDSolicitudInstalacion, int fiIDTecnico, string cliente, string ubicacion)
        {
            try
            {
                // ubicacion = ubicacion.Replace('A', ',');
                var actualizaraAsignado = _connection.OrionContext.sp_ActualizarInstalacion_Tecnico(fiIDSolicitudInstalacion, 2, 0, "_");
                var InformacionPaquete = _connection.OrionContext.sp_InformacionInstalacion_Tecnico(fiIDSolicitudInstalacion).FirstOrDefault();
                var Asignar = _connection.OrionContext.sp_AsignarSolicitud_Contratista(fiIDSolicitudInstalacion, fiIDTecnico) > 0;
                string informacion = $"Servicio a instalar: {InformacionPaquete.fcArticulosdelContrato} ";
                var InformacionTecnico = _connection.OrionContext.sp_InformacionTecnico(fiIDTecnico).FirstOrDefault();
                var InfoCliente = _connection.OrionContext.sp_InformacionCliente_BySolicitudInstalacion(fiIDSolicitudInstalacion).FirstOrDefault();
                if (Asignar)
                {
                    string lat = ubicacion.Split(',')[0];
                    string longi = ubicacion.Split(',')[1];
                    string urlMaps = $"https://www.google.com/maps?z=12&t=k&q={lat},{longi}";
                    //email a tecnico
                    var bod = "Estimado " + InformacionTecnico.fcNombreTecnico + " se le informa que se le ha asignado una instalacion " + ",Cliente: " + cliente + "" + informacion + "Ubicacion:" + urlMaps;

                    var modelCorreo = new SendEmailViewModel();
                    modelCorreo.DestinationEmail = InformacionTecnico.fcCorreoTecnico;
                    // modelCorreo.DestinationEmail = "denis.saavedra@miprestadito.com";    
                    modelCorreo.Subject = "Instalacion de servicio";
                    modelCorreo.Body = "Estimado " + InformacionTecnico.fcNombreTecnico + " se le informa que se le ha asignado una instalacion " + ",<br>Cliente: <b>" + cliente + "</b><br>" + informacion + "<br>Ubicacion:" + urlMaps;
                    modelCorreo.EmailName = "Novanet";
                    modelCorreo.List_CC.Add("brayan.sierra@miprestadito.com");
                    await ServicioCorreo.SendEmailAsync(modelCorreo);
                    //telegram a tecnico


                    //sms a cliente
                    MensajeriaApi.EnviarMensajeInstalacionaCliente(cliente, InformacionPaquete.fiIDSolicitud ?? 0, InfoCliente.fcTelefono, InformacionTecnico.fcNombreTecnico.Replace('.', ' '), informacion, InformacionTecnico.fcTelefonoMovil, InformacionTecnico.fcIdentidadTecnico);
                    //MensajeDeTexto.EnviarMensajeInstalacionaCliente(cliente, InfoCliente.fiIDEquifax ?? 0, InfoCliente.fcTelefono, InformacionTecnico.fcNombreTecnico.Replace('.', ' '), informacion, InformacionTecnico.fcTelefonoMovil,InformacionTecnico.fcIdentidadTecnico);
                    //MensajeDeTexto.EnviarMensajeInstalacionaCliente(cliente, InfoCliente.fiIDEquifax ?? 0, "99521461", InformacionTecnico.fcNombreTecnico.Replace('.', ' '), informacion, InformacionTecnico.fcTelefonoMovil,InformacionTecnico.fcIdentidadTecnico);


                    //email a cliente
                    var modelCorreoCliente = new SendEmailViewModel();

                    modelCorreoCliente.DestinationEmail = InfoCliente.fcCorreo;
                    //modelCorreoCliente.DestinationEmail = "denis.saavedra@miprestadito.com";
                    modelCorreoCliente.Subject = "Instalacion de servicio";
                    modelCorreoCliente.Body = "Estimado  " + cliente + " se le notifica que la instalacion de su servicio sera realizada por el tecnico: " + "<br>" + InformacionTecnico.fcNombreTecnico.Replace('.', ' ') + "</b>,Con identidad " + InformacionTecnico.fcIdentidadTecnico + "<br>Se contactara con usted del telefono: " + InformacionTecnico.fcTelefonoMovil;
                    modelCorreoCliente.EmailName = "Novanet";
                    await ServicioCorreo.SendEmailAsync(modelCorreoCliente);
                    MensajeriaApi.MensajesDigitales(InformacionTecnico.fcTelefonoMovil, bod);

                    //telegram a cliente


                    //Generar bitacora
                    var bitacora = _connection.OrionContext.sp_Solicitud_Instalacion_Bitacoras_Crear(fiIDSolicitudInstalacion, "Se asigno al tecnico " + InformacionTecnico.fcNombreTecnico, GetIdUser());
                    var tecnicoaMostrarNotif = _connection.OrionContext.sp_GetUsuario_Tecnico(fiIDTecnico).FirstOrDefault();
                    EnviarNotificacion($"Se te asigno una instalacion, revisa tu bandeja {InformacionTecnico.fcNombreTecnico}");
                }
                return EnviarResultado(Asignar, "Se asigno correctamente la instalación");

            }
            catch (Exception e)
            {

                return EnviarException(e, "Error");
            }
        }

        [ValidateInput(false)]
        public async Task<ActionResult> AsignarSolicitudContratistaEmpresa(int fiIDSolicitudInstalacion, int fiIDTecnico, string cliente, int idSolicitud)
        {
            try
            {

                var InformacionPaquete = _connection.OrionContext.sp_InformacionInstalacion_Tecnico(fiIDSolicitudInstalacion).FirstOrDefault();
                var datosInstalacionSop = _connection.OrionContext.sp_InformacionInstalacionSop(fiIDSolicitudInstalacion).FirstOrDefault();
                string informacion = $"Servicio a instalar: {InformacionPaquete.fcArticulosdelContrato} ";
                var result = _connection.OrionContext.sp_DatosInstalacion_AsignarEmpresaContratista(idSolicitud, datosInstalacionSop.fcComentario, GetIdUser(), fiIDTecnico, fiIDSolicitudInstalacion);

                var TipoSolicitudContratista = 2;
                var _emailTemplateService = new EmailTemplateService();

                var datosCliente = _connection.OrionContext.sp_OrionContratista_DetalleBySolicitud(GetIdUser(), idSolicitud, 0, 0).FirstOrDefault();
                var Correo = "aebautista63@gmail.com";
                var modelCorreo = new SendEmailViewModel();
                var CuerpoComentarioCorreo = "Estimado : " + datosCliente.fcNombreEmpresa + " , se le notifica la solicitud de trabajo(SOP) por reportes para el cliente: " + cliente + "<br/>" + " comentario:" + datosInstalacionSop.fcComentario;
                modelCorreo.DestinationEmail = datosCliente.fcCorreoElectronico;

                await _emailTemplateService.SendEmailToCustomer(new EmailTemplateServiceModel
                {
                    IdEmailTemplate = 21,
                    CustomerEmail = modelCorreo.DestinationEmail,
                    //CustomerEmail = Correo,
                    Comment = "Solicitud de contratista(SOP).",
                    IDSolicitud = idSolicitud,
                    IdCliente = datosCliente?.fiIDEquifax ?? 0,
                    IdTransaccion = 2,
                    IDSolicitudContratrista = fiIDSolicitudInstalacion,
                    List_CC = new List<string> { MemoryLoadManager.EmailSystemAdministrator }
                });


                return EnviarResultado(true, "Se asigno correctamente la instalación");

            }
            catch (Exception e)
            {

                return EnviarException(e, "Error");
            }
        }

        public ActionResult ViewDetalleInstalacion(int idSolicitudInstalacion, int idSolicitud, int idTipoSolicitud)
        {
            detallesolicitudInstalacioViewModel model = new detallesolicitudInstalacioViewModel();
            model.fiIDSolicitudContratista = idSolicitudInstalacion;
            var infoCliente = _connection.OrionContext.sp_InformacionInstalacion_Tecnico(idSolicitudInstalacion).FirstOrDefault();
            model.fcArticulosAsignados = infoCliente.fcArticulosdelContrato;
            model.fcCliente = infoCliente.fcNombre;
            var catalogoDocumentosInstalacion = _connection.OrionContext.sp_catalogo_FotosInstalacion_lista().ToList();
            ViewBag.IdSolicitud = idSolicitud;
            ViewBag.IdTipoSolicitud = idTipoSolicitud;
            var listaFotosDB = _connection.OrionContext.sp_DocumentosInstalacion_SoporteTecnico(idSolicitudInstalacion).ToList();
            List<lista_fotosInstalacion> listadoFotos = new List<lista_fotosInstalacion>();
            foreach (var item in listaFotosDB)
            {
                listadoFotos.Add(new lista_fotosInstalacion() { fiIDContratistaSolicitud = item.fiIDContratistaSolicitud, fcNombreDocumento = item.fcNombreDocumento, fcExtension = item.fcExtension, fcUrlDocumento = item.fcUrlDocumento, fiIDCatalogoFotoInstalacion = item.fiIDCatalogoFotoInstalacion, fiIDSolicitudInstalacion = item.fiIDSolicitudInstalacion, DescripcionDocumento = catalogoDocumentosInstalacion.Where(x => x.fiIDCatalogo_FotosInstalacion == item.fiIDCatalogoFotoInstalacion).Select(x => x.fcDescripcion).FirstOrDefault() });
            }

            int contador = 1;
            var lifoto = listadoFotos.Select(a => new
            {
                fiIDSolicitudInstalacion = a.fiIDSolicitudInstalacion,
                fiIDContratistaSolicitud = a.fiIDContratistaSolicitud,
                fiIDCatalogoFotoInstalacion = a.fiIDCatalogoFotoInstalacion,
                fcUrlDocumento = a.fcUrlDocumento,
                fcNombreDocumento = a.fcNombreDocumento,
                fcExtension = a.fcExtension,
                DescripcionDocumento = a.DescripcionDocumento,
                posicion = contador++
            }).ToList();

            model.listadoFotos = listadoFotos;
            var EstadoSolicitud = _connection.OrionContext.sp_ObtenerEstado_SolicitudInstalacion(idSolicitudInstalacion).FirstOrDefault();
            model.fiIDEstadoInstalacion = EstadoSolicitud.fiIDEstadoInstalacion ?? 0;
            model.fcDescripcionEstado = EstadoSolicitud.fcDescripcion;
            model.fcClase = EstadoSolicitud.fcClase;
            model.AccesoQR = _connection.OrionContext.sp_ValidarQR_SolicitudInstalacion(idSolicitudInstalacion).FirstOrDefault() ?? 0;
            model.fnMetrosFibra = _connection.OrionContext.sp_ObtenerMetrosFibra_Solicitud(idSolicitudInstalacion).FirstOrDefault() ?? 0;
            model.fcComentarioInstalacion = _connection.OrionContext.sp_SolicitudInstalacion_ObtenerComentarioInstalacion(idSolicitudInstalacion).FirstOrDefault() ?? "";
            return View(model);
        }


        public ActionResult ViewDetalleInstalacionVista(int idSolicitudInstalacion, int idSolicitud)
        {
            detallesolicitudInstalacioViewModel model = new detallesolicitudInstalacioViewModel();
            model.fiIDSolicitudContratista = idSolicitudInstalacion;
            var infoCliente = _connection.OrionContext.sp_InformacionInstalacion_Tecnico(idSolicitudInstalacion).FirstOrDefault();
            model.fcArticulosAsignados = infoCliente.fcArticulosdelContrato;
            model.fcCliente = infoCliente.fcNombre;
            var catalogoDocumentosInstalacion = _connection.OrionContext.sp_catalogo_FotosInstalacion_lista().ToList();
            ViewBag.IdSolicitud = idSolicitud;
            var listaFotosDB = _connection.OrionContext.sp_DocumentosInstalacion_SoporteTecnico(idSolicitudInstalacion).ToList();
            List<lista_fotosInstalacion> listadoFotos = new List<lista_fotosInstalacion>();
            foreach (var item in listaFotosDB)
            {
                listadoFotos.Add(new lista_fotosInstalacion() { fiIDContratistaSolicitud = item.fiIDContratistaSolicitud, fcNombreDocumento = item.fcNombreDocumento, fcExtension = item.fcExtension, fcUrlDocumento = item.fcUrlDocumento, fiIDCatalogoFotoInstalacion = item.fiIDCatalogoFotoInstalacion, fiIDSolicitudInstalacion = item.fiIDSolicitudInstalacion, DescripcionDocumento = catalogoDocumentosInstalacion.Where(x => x.fiIDCatalogo_FotosInstalacion == item.fiIDCatalogoFotoInstalacion).Select(x => x.fcDescripcion).FirstOrDefault() });
            }

            int contador = 1;
            var lifoto = listadoFotos.Select(a => new
            {
                fiIDSolicitudInstalacion = a.fiIDSolicitudInstalacion,
                fiIDContratistaSolicitud = a.fiIDContratistaSolicitud,
                fiIDCatalogoFotoInstalacion = a.fiIDCatalogoFotoInstalacion,
                fcUrlDocumento = a.fcUrlDocumento,
                fcNombreDocumento = a.fcNombreDocumento,
                fcExtension = a.fcExtension,
                DescripcionDocumento = a.DescripcionDocumento,
                posicion = contador++
            }).ToList();

            model.listadoFotos = listadoFotos;
            var EstadoSolicitud = _connection.OrionContext.sp_ObtenerEstado_SolicitudInstalacion(idSolicitudInstalacion).FirstOrDefault();
            model.fiIDEstadoInstalacion = EstadoSolicitud.fiIDEstadoInstalacion ?? 0;
            model.fcDescripcionEstado = EstadoSolicitud.fcDescripcion;
            model.fcClase = EstadoSolicitud.fcClase;
            model.AccesoQR = _connection.OrionContext.sp_ValidarQR_SolicitudInstalacion(idSolicitudInstalacion).FirstOrDefault() ?? 0;
            model.fnMetrosFibra = _connection.OrionContext.sp_ObtenerMetrosFibra_Solicitud(idSolicitudInstalacion).FirstOrDefault() ?? 0;
            model.fcComentarioInstalacion = _connection.OrionContext.sp_SolicitudInstalacion_ObtenerComentarioInstalacion(idSolicitudInstalacion).FirstOrDefault() ?? "";
            return View(model);
        }


        // Función para obtener la representación ordinal (First, Second, Third, etc.)
        static string GetOrdinal(int number)
        {
            if (number <= 0)
                return number.ToString();

            switch (number % 100)
            {
                case 11:
                case 12:
                case 13:
                    return number + "th";
            }

            switch (number % 10)
            {
                case 1:
                    return number + "st";
                case 2:
                    return number + "nd";
                case 3:
                    return number + "rd";
                default:
                    return number + "th";
            }
        }

        public ActionResult ViewBitacora(int idSolicitudInstalacion)
        {
            try
            {
                var listaBitacorasDB = _connection.OrionContext.sp_Solicitud_Instalacion_Bitacoras_GetByfiIDSolicitudInstalacion(idSolicitudInstalacion).ToList();
                var model = new BitacorasInstalacion();
                var lista = new List<listaBitacora>();
                foreach (var item in listaBitacorasDB)
                {
                    lista.Add(new listaBitacora() { usuario = _connection.OrionContext.sp_ObtenerNombreUsuario(item.fiIdUsuarioCreacion).FirstOrDefault(), fechaCreacion = item.fdFechaCreacion ?? DateTime.Now, fiIDSolicitudInstalacion = item.fiIDSolicitudInstalacion ?? 0, comentario = item.fcComentario });
                }
                model.ListadoBitacoras = lista;
                model.fiIDSolicitudInstalacion = idSolicitudInstalacion;
                model.nombreCliente = _connection.OrionContext.sp_InformacionInstalacion_Tecnico(idSolicitudInstalacion).FirstOrDefault().fcNombre;
                return View(model);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        public ActionResult ActualizarMaterial(int idDetalle, string fcProducto, int fnCantidad)
        {
            try
            {
                using (var context = new ORIONDBEntities())
                {
                    var connection = (SqlConnection)context.Database.Connection;
                    var command = new SqlCommand("sp_Instalacion_Materiales_Actualizar", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@fiIDContratista_SolicitudInstalacion_Detalle", idDetalle);
                    command.Parameters.AddWithValue("@fcProducto", fcProducto);
                    command.Parameters.AddWithValue("@fnCantidad", fnCantidad);
                    command.Parameters.AddWithValue("@piIDUsuario", GetIdUser());

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return EnviarResultado(true, "", "Material actualizado correctamente.");

            }
            catch (Exception e)
            {
                return EnviarException(e, "Error");

            }
        }

        public ActionResult ModalDetalleInstalacion(int idSolicitudInstalacion)
        {
            try
            {
                var listadetalle = new DetalleInstalacionViewModel();
                var confi = GetConfiguracion<int>("Orion_Admin", ',');
                var IDEstadoInstalacion = GetConfiguracion<int>("ID_Contratista_EstadoInstalacion", ',');
                var PermisoValidSoporte = GetConfiguracion<int>("ID_Roles_Contratista_Bandeja", ',');
                ViewBag.RolesContratista = confi.Contains(GetUser().IdRol);
                ViewBag.PermisoValidarSoporte = PermisoValidSoporte.Contains(GetUser().IdRol);

                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_Instalacion_RevisionInstalacion {idSolicitudInstalacion}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        listadetalle = db.ObjectContext.Translate<DetalleInstalacionViewModel>(reader).FirstOrDefault();
                    }
                    ViewBag.fiIDSolicitud = listadetalle.fiIDSolicitud;
                    ViewBag.IdCliente = listadetalle.fiIDEquifax;
                    ViewBag.NombreCliente = listadetalle.fcNombre;
                    var material = listadetalle.fjListaMateriales.FirstOrDefault(x => x.fiIDProducto == 24);
                    ViewBag.fnCantidad = material?.fnCantidad ?? 0;

                    ViewBag.fiIDEstadoInstalacion = IDEstadoInstalacion.Any(id => id == (int)listadetalle.fiIDEstadoInstalacion); //(listadetalle.fiIDEstadoInstalacion == IDEstadoInstalacion.FirstOrDefault()) ? true : false;

                    ViewBag.fiIDTipoSolicitud = listadetalle.fiIDTipoSolicitud;
                    ViewBag.fbArticulosExtra = listadetalle.fbArticulosExtra;
                    connection.Close();
                }
                return PartialView(listadetalle);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [ValidateInput(false)]
        [AllowAnonymous]
        public async Task<ActionResult> EnviarCorreoActualizarMateriales(ListaMaterialesEnviarCorreoCorreo model)
        {
            try
            {
                using (var dbContext = new ORIONDBEntities())
                using (var connection = dbContext.Database.Connection)
                {
                    connection.Open();

                    var usuaruiologueado = GetUser();

                    var InformacionSoporte = _connection.OrionContext.sp_Configuraciones().Where(a => a.NombreLlave == "CorreoSistemasGeneral").ToList();


                    var tableHtml = $@"
                     <h3>Cambios en los materiales:</h3>
                     <table style='border-collapse: collapse; width: 100%;'>
                         <thead>
                             <tr>
                                 <th style='border: 1px solid black; padding: 8px; text-align: left;'>ID Detalle</th>
                                 <th style='border: 1px solid black; padding: 8px; text-align: left;'>Producto</th>
                                 <th style='border: 1px solid black; padding: 8px; text-align: left;'>Cantidad (Antes)</th>
                                 <th style='border: 1px solid black; padding: 8px; text-align: left;'>Cantidad (Después)</th>
                                 <th style='border: 1px solid black; padding: 8px; text-align: left;'>Usuario Modifica</th>
                             </tr>
                         </thead>
                         <tbody>
                             <tr>
                                 <td style='border: 1px solid black; padding: 8px;'>{model.fiIDDetalle}</td>
                                 <td style='border: 1px solid black; padding: 8px;'>{model.fcProductoAntes}</td>
                                 <td style='border: 1px solid black; padding: 8px;'>{model.fnCantidadAntes}</td>
                                 <td style='border: 1px solid black; padding: 8px;'>{model.fnCantidadDespues}</td>
                                 <td style='border: 1px solid black; padding: 8px;'>{usuaruiologueado.UserName}</td>

                               </tr>
                         </tbody>
                     </table>
                 ";

                    var modelCorreo = new SendEmailViewModel
                    {
                        DestinationEmail = "bodegaprincipal@novanetgroup.com",
                        Subject = "Actualización de Materiales",
                        Body = $@"
                            <p>Estimado(a),</p>
                            <p>Se ha realizado una actualización en los materiales de la solicitud <strong>{model.fiIDSolicitud}</strong>.</p>
                            <p><strong>Cliente:</strong> {model.NombreCliente}</p>
                            <p><strong>Motivo de la actualización:</strong> {model.Observaciones}</p>
                            {tableHtml}
                            <p>Saludos,</p>
                            <p>Equipo Novanet</p>
                        ",
                        EmailName = "Novanet"
                    };

                    modelCorreo.List_CC.AddRange(InformacionSoporte.Select(s => s.ValorLLave));
                    await ServicioCorreo.SendEmailAsync(modelCorreo);
                    _connection.OrionContext.sp_BitacoraSolicitudHistorialMantenimiento(model.fiIDSolicitud, model.IdCliente, GetIdUser(), model.Observaciones, model.Opcion);
                }

                return Json(new { success = true, message = "Correo enviado exitosamente." });
            }
            catch (Exception e)
            {
                return EnviarException(e, "Error al guardar y enviar Correo");
            }
        }


        public ActionResult ViewOrdenDeTrabajo(int idSolicitudInstalacion, int idOperacion)
        {
            try
            {
                contratosContratista_ViewModel model = new contratosContratista_ViewModel();
                model.fiIDSolicitudInstalacion = idSolicitudInstalacion;
                var infoCliente = _connection.OrionContext.sp_InformacionInstalacion_Tecnico(idSolicitudInstalacion).FirstOrDefault();
                model.fcNombreCliente = infoCliente.fcNombre;
                var DatosDocumentoListado = new List<sp_OrionSolicitud_DocumentoListado_ViewModel>();
                var solicitud = _connection.OrionContext.sp_ObtenerSolicitud_ByIdInstalacion(idSolicitudInstalacion).FirstOrDefault();
                var EstadoSolicitud = _connection.OrionContext.sp_ObtenerEstado_SolicitudInstalacion(idSolicitudInstalacion).FirstOrDefault();

                model.fiIDEstadoInstalacion = EstadoSolicitud.fiIDEstadoInstalacion ?? 0;

                ViewBag.idSolicitud = solicitud;
                // var datosCliente = _connection.OrionContext.sp_OrionContratista_DetalleBySolicitud(1, solicitud, 0, 0).FirstOrDefault();
                var datosCliente = _connection.OrionContext.sp_OrionContratista_DetalleBySolicitud(1, solicitud, idOperacion, idSolicitudInstalacion).FirstOrDefault();
                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_OrionSolicitud_Detalle_ClienteListar {GetIdUser()} , {solicitud}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        for (int i = 0; i < 3; i++)
                        {
                            reader.NextResult();
                        }

                        //reader.NextResult();
                        DatosDocumentoListado = db.ObjectContext.Translate<sp_OrionSolicitud_DocumentoListado_ViewModel>(reader).ToList();
                        model.fcNombreArchivoSinFirma = DatosDocumentoListado.Where(x => x.fcDescripcion == "Anexos").Select(x => x.fcNombreArchivo).FirstOrDefault();
                        model.fcUrlContratoSinFirma = DatosDocumentoListado.Where(x => x.fcDescripcion == "Anexos").Select(x => x.fcRuta).FirstOrDefault();

                        model.fcNombreArchivoConFirma = DatosDocumentoListado.Where(x => x.fcDescripcion == "Anexo Firmado por el cliente o Tercero").Select(x => x.fcNombreArchivo).FirstOrDefault();
                        model.fcUrlContratoConFirma = DatosDocumentoListado.Where(x => x.fcDescripcion == "Anexo Firmado por el cliente o Tercero").Select(x => x.fcRuta).FirstOrDefault();

                    }
                    //DictionaryList.Clear();
                    //DictionaryList.Add("fcNombreCliente", "prueba");
                    connection.Close();
                }
                //var TipoOperacion = datosCliente.
                DictionaryList = GeneradorDelDireccionarioCliente((int)solicitud, 0, 0, idSolicitudInstalacion);
                return View(model);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static string GeneratePDFAttachment(string pathToOpen, string pathToSave)
        {
            Document doc = new Document();
            doc.LoadFromFile(pathToOpen);
            foreach (var item in DictionaryList)
            {
                doc.Replace("{" + item.Key + "}", item.Value ?? "", true, true);
            }
            doc.SaveToFile(pathToSave, FileFormat.PDF);
            doc.Close();

            return pathToSave;
        }

        public string VistaPreviaOrdenTrabajo(string NombreArchivo, string IdSolicitud)
        {
            var resultado = "";
            try
            {
                string pathToOpen = Path.Combine(HttpContext.Server.MapPath("~/Documento/Recursos/Plantilla/"), NombreArchivo + ".docx");
                // string pathToOpen = MemoryLoadManager.URL + @"\Documento\Recursos\Plantilla\" + NombreArchivo + ".docx";
                var nuevoNombreArchivo = NombreArchivo + "_" + "No." + IdSolicitud + ".pdf";
                var urlToSave = "/Documento/Recursos/Solicitud/" + nuevoNombreArchivo;
                string pathToSave = HttpContext.Server.MapPath("~" + urlToSave);
                GeneratePDFAttachment(pathToOpen, pathToSave);
                resultado = urlToSave;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return resultado;

        }


        public ActionResult VistaCambiarContratistaInstalacionYOrdenTrabajo(int fiIdConstratistaSolicitud)
        {
            ViewBag.fiSolicitudContratista = fiIdConstratistaSolicitud;
            return PartialView();
        }

        public ActionResult ActualizarEstadoQR(int fiIDSolicitudInstalacion, int idSolicitud, decimal fnMetrosFibra)
        {
            try
            {
                var Usuario = GetUser();
                var ValidarInstalacion = _connection.OrionContext.sp_Validar_Instalacion_Solicitud(fiIDSolicitudInstalacion, idSolicitud, Usuario.fiIdUsuario).FirstOrDefault();
                ActualizarFilaContratista(fiIDSolicitudInstalacion);//signal R para la bandeja de contratista y tambien en la bandeja de Tecnicos

                var listadetalle = new InstalacionViewModel();

                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_Instalacion_TecnicosExternos {fiIDSolicitudInstalacion}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        listadetalle = db.ObjectContext.Translate<InstalacionViewModel>(reader).FirstOrDefault();
                    }
                    connection.Close();

                    //enviar mensaje
                    var bod = "Estimado  " + listadetalle.FcNombre + " se le notifica que Ya Finalizo la Instalacion de su servicio " + "\n <br>Se le adjunta el codigo QR de su instalacion:" + MemoryLoadManager.UrlWeb + "/DatosCliente/ViewFormQR/" + listadetalle.FiIDSolicitud;
                    //mensaje al Tecnico Superior
                    MensajeriaApi.MensajesDigitales(listadetalle.FcTelefono, bod);
                    ////---- antes no estaba esto de arriba cuando se mandaba a validar la instalacion

                    //_connection.OrionContext.sp_Solicitud_Instalacion_Bitacoras_Crear(fiIDSolicitudInstalacion, "Se ha validado la instalación por parte de soporte técnico", GetIdUser());
                    return EnviarResultado(true, "Se envió resultado al técnico");

                }



                //bool isSuccessful = false;

                //using (var connection = (new ORIONDBEntities()).Database.Connection)
                //{
                //    connection.Open();
                //    using (var command = connection.CreateCommand())
                //    {
                //        command.CommandText = "EXEC sp_Instalacion_Tecnico_Actualizar @fiIDSolicitudInstalacion, @fnMetrosFibra";
                //        command.Parameters.Add(new SqlParameter("@fiIDSolicitudInstalacion", fiIDSolicitudInstalacion));
                //        command.Parameters.Add(new SqlParameter("@fnMetrosFibra", fnMetrosFibra));
                //        using (var reader = command.ExecuteReader())
                //        {
                //            isSuccessful = reader.RecordsAffected > 0;
                //        }
                //    }
                //    if (isSuccessful)
                //    {
                //        using (var command = connection.CreateCommand())
                //        {
                //            command.CommandText = "EXEC sp_OrionSolicitud_PasoFinal @piIDSolicitud";
                //            command.Parameters.Add(new SqlParameter("@piIDSolicitud", idSolicitud));
                //            using (var reader = command.ExecuteReader())
                //            {
                //                isSuccessful = reader.RecordsAffected > 0;
                //            }
                //        }
                //        if (isSuccessful)
                //        {


                //        }


                //    }
                //    connection.Close();
                //}

                //return EnviarResultado(false, "¡Algo falló!");
            }
            catch (Exception e)
            {
                return EnviarException(e, "Error");
            }
        }

        public ActionResult ActualizarEstadoSinQR(int fiIDSolicitudInstalacion, int idSolicitud)
        {
            try
            {
                // ubicacion = ubicacion.Replace('A', ',');
                var comentarioInsertar = _connection.OrionContext.sp_SolicitudInstalacion_ObtenerComentarioInstalacion(fiIDSolicitudInstalacion).FirstOrDefault() ?? "";
                var actualizarEstadoInstalacion = _connection.OrionContext.sp_ActualizarInstalacion_Tecnico(fiIDSolicitudInstalacion, 9, 0, comentarioInsertar);//7 es estado validado por supervisor
                                                                                                                                                                //var qr = _connection.OrionContext.sp_PermitirEscanearQR(fiIDSolicitudInstalacion) > 0;


                //if (qr)
                //{
                //EnviarNotificacion("Se ")
                //generar bitacora
                var bitacora = _connection.OrionContext.sp_Solicitud_Instalacion_Bitacoras_Crear(fiIDSolicitudInstalacion, "Se ha validado la instalacion por parte de soporte tecnico", GetIdUser());

                return EnviarResultado(true, "Se envio resultado al tecnico");
                //}

                //return EnviarResultado(false, "Fallo algo!");
            }
            catch (Exception e)
            {

                return EnviarException(e, "Error");
            }
        }


        public ActionResult OrdendeTrabajo()
        {
            return View();
        }

        public ActionResult ViewEditarDatosEstadoInstalacion(int piIDContratistaSolicitud)
        {
            ViewBag.IdSolicitud = piIDContratistaSolicitud;
            ViewBag.ListaEstado = new List<SelectListItem>();

            SolicitudesViewModel clienteDetalleInstalacion = null;
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();

                // Retrieve client details
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "EXEC sp_OrionContratista_DetalleOrdenTrabajoByContratistaSolicitud @piIDContratistaSolicitud";
                    var paramSolicitud = command.CreateParameter();
                    paramSolicitud.ParameterName = "@piIDContratistaSolicitud";
                    paramSolicitud.Value = piIDContratistaSolicitud;
                    paramSolicitud.DbType = DbType.Int32;
                    command.Parameters.Add(paramSolicitud);

                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        var datosCliente = db.ObjectContext.Translate<sp_OrionContratista_DetalleBySolicitud_Result>(reader).FirstOrDefault();
                        if (datosCliente != null)
                        {
                            clienteDetalleInstalacion = new SolicitudesViewModel
                            {
                                Nombre = datosCliente.fcNombre,
                                IDSolicitud = (int)datosCliente.fiIDSolicitud,
                                CodigoCliente = datosCliente.fcCodigoCliente,
                                fiIDSolicitudInstalacion = piIDContratistaSolicitud,
                                NumeroOrdenCepheus = datosCliente.fcNumeroOrdenCfeus,
                                fdFechaInstalacionAsignada = datosCliente.fdFechaInstalacionAsignada,
                                fcComentarioOrdenIsntalacion = datosCliente.fcComentario,
                                fcDescripcion = datosCliente.fcDescripcionEstadoInstalacion,
                                fiIDEstadoInstalacion = datosCliente.fiIDEstadoInstalacion,

                            };
                        }
                    }
                }

                // Retrieve state list
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "EXEC sp_OrionSolicitud_LlenarListas @UserId";
                    var paramUser = command.CreateParameter();
                    paramUser.ParameterName = "@UserId";
                    paramUser.Value = GetIdUser();
                    paramUser.DbType = DbType.Int32;
                    command.Parameters.Add(paramUser);

                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        reader.NextResult();
                        reader.NextResult();
                        reader.NextResult();

                        var ListaEstado = db.ObjectContext.Translate<SolicitudesViewModel>(reader).ToList();
                        ViewBag.ListaEstado = ListaEstado?.Select(z => new SelectListItem
                        {
                            Value = Convert.ToString(z.fiIDEstadoInstalacion),
                            Text = z.fcDescripcion
                        }).ToList() ?? new List<SelectListItem>();
                    }
                }

                connection.Close();
            }

            // Return the partial view with the model
            return PartialView(clienteDetalleInstalacion ?? new SolicitudesViewModel { IDSolicitud = piIDContratistaSolicitud });
        }



        [HttpPost]
        public JsonResult ActualizarDatosInstalacionEstado(int piIDContratistaSolicitud, int piIDEstadoInstalacion, string pcObservaciones)
        {
            using (var context = new ORIONDBEntities())
            {
                try
                {
                    using (var connection = context.Database.Connection)
                    {
                        connection.Open();
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "EXEC sp_DatosInstalacion_CambiarEstadoOrdenTrabajo @piIDContratistaSolicitud, @piIDEstadoInstalacion, @piIDUsuario";

                            var paramContratistaSolicitud = command.CreateParameter();
                            paramContratistaSolicitud.ParameterName = "@piIDContratistaSolicitud";
                            paramContratistaSolicitud.Value = piIDContratistaSolicitud;
                            paramContratistaSolicitud.DbType = DbType.Int32;
                            command.Parameters.Add(paramContratistaSolicitud);

                            var paramEstadoInstalacion = command.CreateParameter();
                            paramEstadoInstalacion.ParameterName = "@piIDEstadoInstalacion";
                            paramEstadoInstalacion.Value = piIDEstadoInstalacion;
                            paramEstadoInstalacion.DbType = DbType.Int32;
                            command.Parameters.Add(paramEstadoInstalacion);

                            var paramUsuario = command.CreateParameter();
                            paramUsuario.ParameterName = "@piIDUsuario";
                            paramUsuario.Value = GetIdUser();
                            paramUsuario.DbType = DbType.Int32;
                            command.Parameters.Add(paramUsuario);

                            var paramObservaciones = command.CreateParameter();
                            paramObservaciones.ParameterName = "@pcObservaciones";
                            paramObservaciones.Value = pcObservaciones;
                            paramObservaciones.DbType = DbType.String;
                            paramObservaciones.Size = 300;
                            command.Parameters.Add(paramObservaciones);

                            command.ExecuteNonQuery();
                            connection.Close();

                            return EnviarResultado(true, "Datos", "Datos Actualizados");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }


        public ActionResult ListarClientesConfirmarIsntalacion()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarConfirmacionCliente(int fiIDSolicitud)
        {

            try
            {
                var lista = new List<SolicitudInstalacion_Confirmacion_Listar>();


                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_Cliente_SolicitudInstalacion_Confirmacion_Listar {fiIDSolicitud}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        lista = db.ObjectContext.Translate<SolicitudInstalacion_Confirmacion_Listar>(reader).ToList();
                    }

                    connection.Close();
                }
                return EnviarListaJson(lista);

            }
            catch (Exception ex)
            {

                throw;
            }


        }

        public JsonResult ListaOrdenesdeTrabajo()
        {
            try
            {
                var jsonResult = new JsonResult();
                var Roles_Contratista_Bandeja = GetConfiguracion<string>("Roles_Contratista_Bandeja", ',').Select(x => _connection.OrionContext.sp_Roles_ObtenerPorRole(x).FirstOrDefault()).ToList();
                var contratistaLogeado = GetIdContratista();

                var empresa = _connection.OrionContext.sp_ObtenerEmpresa_ByIdUsuario(GetIdUser()).FirstOrDefault();
                var lista = _connection.OrionContext.sp_SolicitudesInstalacion_Lista().Where(x => x.fcDescripcion != "Pendiente" && x.fiIDTipoSolicitud == 2).ToList();
                foreach (var item in lista)
                {
                    item.fcPaquete = _connection.OrionContext.sp_InformacionInstalacion_Tecnico(item.fiIDSolicitudInstalacion).FirstOrDefault().fcArticulosdelContrato ?? "";
                }
                //Si es Admin
                if (Roles_Contratista_Bandeja.Any(x => x.Pk_IdRol == GetUser().IdRol))
                {
                    jsonResult = EnviarListaJson(lista);
                }
                else
                {
                    jsonResult = EnviarListaJson(lista.Where(a => a.fiIDContratista == empresa));

                }

                return jsonResult;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public JsonResult ListaOrdenesdeTrabajoPendiente()
        {
            try
            {
                //var jsonResult = new JsonResult();
                //var Roles_Contratista_Bandeja = GetConfiguracion<string>("Roles_Contratista_Bandeja", ',').Select(x => _connection.OrionContext.sp_Roles_ObtenerPorRole(x).FirstOrDefault()).ToList();
                //var contratistaLogeado = GetIdContratista();

                //var empresa = _connection.OrionContext.sp_ObtenerEmpresa_ByIdUsuario(GetIdUser()).FirstOrDefault();
                //var lista = _connection.OrionContext.sp_SolicitudesInstalacion_Lista().Where(x => x.fcDescripcion == "Pendiente" && x.fiIDTipoSolicitud == 2).ToList();
                //foreach (var item in lista)
                //{
                //    item.fcPaquete = _connection.OrionContext.sp_InformacionInstalacion_Tecnico(item.fiIDSolicitudInstalacion).FirstOrDefault().fcArticulosdelContrato ?? "";
                //}


                //Si es Admin
                var Usuario = GetUser();
                var listado = _connection.OrionContext.sp_ContratistaSolicitudInstalacion_ById(Usuario.fiIdUsuario, Usuario.IdRol).Where(a => a.fiIDEstadoInstalacion == 1 && a.fiIDTipoSolicitud == 2).ToList();

                //;

                //if (Roles_Contratista_Bandeja.Any(x => x.Pk_IdRol == GetUser().IdRol))
                //{
                //    jsonResult = EnviarListaJson(listado);
                //}
                //else
                //{
                //    jsonResult = EnviarListaJson(lista.Where(a => a.fiIDContratista == empresa));
                //    // jsonResult = EnviarListaJson(lista.Where(x=>x.fiIDContratista==contratistaLogeado).ToList());
                //}




                //await Task.Delay(100);


                // EnviarNotificacion("DESDE BASE CONTROLLER");

                return EnviarListaJson(listado);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public JsonResult ListaOrdenesdeTrabajoCanceladas()
        {
            try
            {
                //var jsonResult = new JsonResult();
                //var Roles_Contratista_Bandeja = GetConfiguracion<string>("Roles_Contratista_Bandeja", ',').Select(x => _connection.OrionContext.sp_Roles_ObtenerPorRole(x).FirstOrDefault()).ToList();
                //var contratistaLogeado = GetIdContratista();

                //var empresa = _connection.OrionContext.sp_ObtenerEmpresa_ByIdUsuario(GetIdUser()).FirstOrDefault();
                //var lista = _connection.OrionContext.sp_SolicitudesInstalacion_Lista().Where(x => x.fcDescripcion == "Pendiente" && x.fiIDTipoSolicitud == 2).ToList();
                //foreach (var item in lista)
                //{
                //    item.fcPaquete = _connection.OrionContext.sp_InformacionInstalacion_Tecnico(item.fiIDSolicitudInstalacion).FirstOrDefault().fcArticulosdelContrato ?? "";
                //}


                //Si es Admin
                var Usuario = GetUser();
                var listado = _connection.OrionContext.sp_ContratistaSolicitudInstalacion_ById(Usuario.fiIdUsuario, Usuario.IdRol).Where(a => a.fiIDEstadoInstalacion == 5 && a.fiIDTipoSolicitud == 2).ToList();

                //;

                //if (Roles_Contratista_Bandeja.Any(x => x.Pk_IdRol == GetUser().IdRol))
                //{
                //    jsonResult = EnviarListaJson(listado);
                //}
                //else
                //{
                //    jsonResult = EnviarListaJson(lista.Where(a => a.fiIDContratista == empresa));
                //    // jsonResult = EnviarListaJson(lista.Where(x=>x.fiIDContratista==contratistaLogeado).ToList());
                //}




                //await Task.Delay(100);


                // EnviarNotificacion("DESDE BASE CONTROLLER");

                return EnviarListaJson(listado);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<JsonResult> EnviarSolicitudContratista(SolicitudesViewModel model, string Comentario)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var _emailTemplateService = new EmailTemplateService();
                        var tipoSolicitudInstalacion = 1;
                        //var result = context.sp_OrionSolicitud_ContratistaSolicitudInstalacio__Insertar(GetIdUser(), model.IdCliente, model.IDSolicitud, idAgencia, idAgenciaContratista, Comentario, CodigoCliente, NumeroOrden, 1);

                        var datosCliente = context.sp_OrionContratista_DetalleBySolicitud(GetIdUser(), model.IDSolicitud, 0, 0).FirstOrDefault();
                        //var Correo = "aebautista63@gmail.com";
                        var modelCorreo = new SendEmailViewModel();
                        var CuerpoComentarioCorreo = "Estimado : " + datosCliente.fcNombreEmpresa + " , se le notifica la Orde de trabajo para el cliente: " + model.Nombre + "<br/>" + " comentario:" + Comentario;
                        modelCorreo.DestinationEmail = datosCliente.fcCorreoElectronico;
                        // modelCorreo.DestinationEmail = Correo;
                        modelCorreo.Subject = "Solicitud de contratista";
                        modelCorreo.Body = CuerpoComentarioCorreo;
                        modelCorreo.EmailName = "Novanet";

                        //await ServicioCorreo.SendEmailAsync(modelCorreo);
                        await _emailTemplateService.SendEmailToCustomer(new EmailTemplateServiceModel
                        {
                            IdEmailTemplate = 21,
                            CustomerEmail = modelCorreo.DestinationEmail,
                            //CustomerEmail = Correo,
                            //IdCustomer = model.fcIDCustomer,
                            //IdLoan = model.fcIDLoan,
                            Comment = "Solicitud de contratista.",
                            IDSolicitud = model.IDSolicitud,
                            IdCliente = model.IdCliente,
                            List_CC = new List<string> { MemoryLoadManager.EmailSystemAdministrator }
                        });


                        return EnviarResultado(true, "Solicitud", "Mensaje Enviado");

                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }
                }
            }
        }

        public ActionResult ClientesConArticulos(int idSolicitudInstalacion, int idsolicitud, int idTipoSolicitud)
        {
            var ListadoDeMaterialesDB = _connection.OrionContext.sp_ProductosExtras_Por_Solicitud(GetIdUser(), idsolicitud).ToList();
            var ListadoMateriales = new List<Listado_materiales_viewModel>();

            var empresa = _connection.OrionContext.sp_ObtenerEmpresa_ByIdUsuario(GetIdUser()).FirstOrDefault();
            var tecnico = GetIdUser();
            ViewBag.CodigoBarra = _connection.OrionContext.sp_Productos_Inventario_SeriesPorTecnico().Where(a => a.fiIDUsuario == GetIdUser()).ToList();
            ViewBag.IdTipoSolicitud = idTipoSolicitud;
            ViewBag.IdSolicitud = idsolicitud;
            foreach (var item in ListadoDeMaterialesDB)
            {
                ListadoMateriales.Add(new Listado_materiales_viewModel() { fiIDProducto = item.fiIDProducto, fcProducto = item.fcProducto.Replace("\\", "").Replace("\"", ""), fnCantidad = 1, fiIDMarca = item.fiIDMarca, fbCodigo = true });
            }
            var catalogoDocumentosInstalacion = _connection.OrionContext.sp_catalogo_FotosInstalacion_lista().ToList();

            var listaFotosDB = _connection.OrionContext.sp_DocumentosInstalacion_SoporteTecnico(idSolicitudInstalacion).ToList();
            List<lista_fotosInstalacion> listadoFotos = new List<lista_fotosInstalacion>();
            foreach (var item in listaFotosDB)
            {
                listadoFotos.Add(new lista_fotosInstalacion() { fiIDContratistaSolicitud = item.fiIDContratistaSolicitud, fcNombreDocumento = item.fcNombreDocumento, fcExtension = item.fcExtension, fcUrlDocumento = item.fcUrlDocumento, fiIDCatalogoFotoInstalacion = item.fiIDCatalogoFotoInstalacion, fiIDSolicitudInstalacion = item.fiIDSolicitudInstalacion, DescripcionDocumento = catalogoDocumentosInstalacion.Where(x => x.fiIDCatalogo_FotosInstalacion == item.fiIDCatalogoFotoInstalacion).Select(x => x.fcDescripcion).FirstOrDefault() });
            }

            detallesolicitudInstalacion_ViewModel model = new detallesolicitudInstalacion_ViewModel();
            ViewBag.ListaFotos = listadoFotos;

            model.fiIDSolicitudContratista = idSolicitudInstalacion;
            var infoCliente = _connection.OrionContext.sp_InformacionInstalacion_Tecnico(idSolicitudInstalacion).FirstOrDefault();
            model.fcArticulosAsignados = infoCliente.fcArticulosdelContrato;
            model.fcCliente = infoCliente.fcNombre;

            var listadoEstados = _connection.OrionContext.sp_ListaCatalogoInstalacion().ToList();
            ViewBag.listadoEstados = listadoEstados.Select(x => new { Value = x.fiIDEstadoInstalacion, Text = x.fcDescripcion }).ToList();

            var EstadoSolicitud = _connection.OrionContext.sp_ObtenerEstado_SolicitudInstalacion(idSolicitudInstalacion).FirstOrDefault();
            model.fiIDEstadoInstalacion = EstadoSolicitud.fiIDEstadoInstalacion ?? 0;
            model.fcDescripcionEstado = EstadoSolicitud.fcDescripcion;
            model.fcClase = EstadoSolicitud.fcClase;
            model.listadoMateriales = ListadoMateriales;

            EnviarNotificacion($"Se comenzo la instalacion del cliente: {model.fcCliente}");
            return View(model);
        }

        public ActionResult AsignarTecnicosUsuario()
        {
            return View();
        }

        public JsonResult ListaTecnicosPorContratista()
        {
            try
            {
                var lista = _connection.OrionContext.sp_TecnicosPorContratista_List().ToList();
                return EnviarListaJson(lista);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public ActionResult AsignarUsuarioContratista()
        {
            return PartialView();
        }

        public JsonResult GuardarTecnicoaContratista(int idcontratista, int idtecnico)
        {
            try
            {
                var lista = _connection.OrionContext.sp_TecnicosPorContratista_Crear(idcontratista, idtecnico, GetIdUser()) > 0;
                return EnviarListaJson(lista);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        [HttpGet]
        public ActionResult ViewListaProductosNovanet(int fiIDSolicitud)
        {
            return PartialView(fiIDSolicitud);
        }

        [HttpGet]
        public JsonResult CargarPaqueteSolicitud(int fiIDSolicitud)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var cliente = contexto.sp_InfoClientePorSolicitud(fiIDSolicitud).FirstOrDefault();
                var datos = contexto.sp_OrionSolicitud_PaqueteCliente(cliente.fiIDEquifax).Where(x => x.fiIDSolicitud == fiIDSolicitud).ToList();
                return EnviarListaJson(contexto.sp_OrionSolicitud_PaqueteCliente(cliente.fiIDEquifax).Where(x => x.fiIDSolicitud == fiIDSolicitud).ToList());
            }
        }

        [HttpGet]
        public JsonResult CargarInstalacionSolicitud(int fiIDSolicitud)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var cliente = contexto.sp_InfoClientePorSolicitud(fiIDSolicitud).FirstOrDefault();
                var datos = contexto.sp_OrionSolicitud_InstalacionPorCliente(cliente.fiIDEquifax).Where(x => x.fiIDSolicitud == fiIDSolicitud).ToList();
                return EnviarListaJson(contexto.sp_OrionSolicitud_InstalacionPorCliente(cliente.fiIDEquifax).Where(x => x.fiIDSolicitud == fiIDSolicitud).ToList());
            }
        }

        [HttpGet]
        public JsonResult CargarInventarioSolicitud(int fiIDSolicitud)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var cliente = contexto.sp_InfoClientePorSolicitud(fiIDSolicitud).FirstOrDefault();
                return EnviarListaJson(contexto.sp_OrionSolicitud_InventarioPorCliente(cliente.fiIDEquifax).Where(x => x.fiIDSolicitud == fiIDSolicitud || x.fiIDSolicitud == 0 || x.fiIDSolicitud == null).ToList());
            }
        }


        public JsonResult CambiarContratistaSolicitud(int fiIdSolicitud, int fiIdcontratista, string fccomentario)
        {
            try
            {
                using (var connection = new ORIONDBEntities())
                {
                    var Usuario = GetUser();
                    var CambioContratista = connection.sp_CambiarContratistaSolicitud(fiIdSolicitud, fiIdcontratista, fccomentario, Usuario.fiIdUsuario, Usuario.IdRol, 1).FirstOrDefault();
                    ActualizarFilaContratista(fiIdSolicitud);
                    return EnviarListaJson(CambioContratista);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #region Adicion Equipo
        [HttpGet]
        public ActionResult ConsultarCliente(string identidad)
        {
            var clientes = new Dictionary<int, AdicionEquipoViewModel>();

            try
            {
                using (var conetion = new ORIONDBEntities())
                {
                    using (var connection = conetion.Database.Connection)
                    {
                        connection.Open();
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "EXEC sp_OrionSolicitud_Maestro_ConsultarClienteByIdentidad @identidad";

                            var param = command.CreateParameter();
                            param.ParameterName = "@identidad";
                            param.Value = identidad;
                            command.Parameters.Add(param);

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int fiIDEquifax = Convert.IsDBNull(reader["fiIDEquifax"]) ? 0 : Convert.ToInt32(reader["fiIDEquifax"]);

                                    if (!clientes.ContainsKey(fiIDEquifax))
                                    {
                                        clientes[fiIDEquifax] = new AdicionEquipoViewModel
                                        {

                                            Identidad = Convert.IsDBNull(reader["fcIdentidad"]) ? string.Empty : reader["fcIdentidad"].ToString(),
                                            fcIDPrestamo = Convert.IsDBNull(reader["fcIDPrestamo"]) ? string.Empty : reader["fcIDPrestamo"].ToString(),
                                            Nombre = Convert.IsDBNull(reader["fcNombre"]) ? string.Empty : reader["fcNombre"].ToString(),
                                            Telefono = Convert.IsDBNull(reader["fcTelefono"]) ? string.Empty : reader["fcTelefono"].ToString(),
                                            fiIDEquifax = fiIDEquifax,
                                            CuotaMensualMonedaNacional = Convert.IsDBNull(reader["fnCuotaMensualMonedaNacional"]) ? 0 : Convert.ToDecimal(reader["fnCuotaMensualMonedaNacional"]),
                                            fiIDSolicitud = Convert.IsDBNull(reader["fiIDSolicitud"]) ? 0 : Convert.ToInt32(reader["fiIDSolicitud"]),
                                            fiIDFirma = Convert.IsDBNull(reader["fiIDFirma"]) ? 0 : Convert.ToInt32(reader["fiIDFirma"]),
                                            equiposCliente = new List<EquipoViewModel>()
                                        };
                                    }
                                    clientes[fiIDEquifax].equiposCliente.Add(new EquipoViewModel
                                    {
                                        fiIDSolicitud = Convert.IsDBNull(reader["fiIDSolicitud"]) ? 0 : Convert.ToInt32(reader["fiIDSolicitud"]),
                                        Articulos = Convert.IsDBNull(reader["Articulos"]) ? string.Empty : reader["Articulos"].ToString(),
                                        fiCantidad = 1,
                                        CuotaMensualMonedaE = Convert.IsDBNull(reader["fnCuotaMensualMonedaE"]) ? 0 : Convert.ToDecimal(reader["fnCuotaMensualMonedaE"]),
                                        CuotaMensualMonedaNacional = Convert.IsDBNull(reader["fnCuotaMensualMonedaNacional"]) ? 0 : Convert.ToDecimal(reader["fnCuotaMensualMonedaNacional"]),

                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al consultar cliente", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(clientes.Values, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ValidarCuotasPrestamo(string fcIDPrestamo)
        {
            var resultado = new Dictionary<string, EquipoViewModel>();

            try
            {
                using (var conetion = new ORIONDBEntities())
                {
                    using (var connection = conetion.Database.Connection)
                    {
                        connection.Open();
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "EXEC sp_OrionSolicitud_Maestro_ValidarCuotasPrestamo @fcIDPrestamo";

                            var param = command.CreateParameter();
                            param.ParameterName = "@fcIDPrestamo";
                            param.Value = fcIDPrestamo;
                            command.Parameters.Add(param);

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string prestamoId = reader["fcIDPrestamo"].ToString();

                                    if (!resultado.ContainsKey(prestamoId))
                                    {
                                        resultado[prestamoId] = new EquipoViewModel
                                        {
                                            fcIDPrestamo = prestamoId,
                                            fiPlazo = Convert.IsDBNull(reader["fiPlazo"]) ? 0 : Convert.ToInt32(reader["fiPlazo"]),
                                            fiCuotasPagadas = Convert.IsDBNull(reader["fiCuotasPagadas"]) ? 0 : Convert.ToInt32(reader["fiCuotasPagadas"]),
                                            fiCuotasAtrasadas = Convert.IsDBNull(reader["fiCuotasAtrasadas"]) ? 0 : Convert.ToInt32(reader["fiCuotasAtrasadas"]),
                                            fiCuotasCursadas = Convert.IsDBNull(reader["fiCuotasCursadas"]) ? 0 : Convert.ToInt32(reader["fiCuotasCursadas"])
                                        };
                                    }
                                }
                            }
                        }
                    }
                }
                if (!resultado.Any())
                {
                    return Json(new { success = false, message = "No se encontraron datos para el préstamo." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al consultar el préstamo", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(resultado.Values, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AdicionDeEquipo()
        {
            // var ArticulosRed = GetConfiguracion<int>("Id_Productos_que_Sirven_Para_Red", ',').ToList();
            var ArticulosRed = GetConfiguracion<int>("Id_Productos_que_Sirven_Para_Red", ',').Except(new[] { 10, 11, 20 }).ToList();

            var configuraciones = GetConfiguracionJson<Dictionary<string, int>>("OrionlimiteCantidadProductosPorEquipo");
            var preciosArrendqamientos = GetConfiguracionJson2<dynamic>("preciosArrendamientoAdicionEquipo");

            var limitesEquipos = configuraciones.ToDictionary(c => c["ID"].ToString(), c => c["Maximo"]);
            var productos = _connection.OrionContext.sp_Producto_Maestro_Listar().Where(a => ArticulosRed.Contains(a.fiIDProducto)).Select(a => new EquipoViewModel { fiIDProducto = a.fiIDProducto, Articulos = a.fcProducto, CuotaMensualMonedaNacional = a.fnValorProductoMN }).ToList();



            ViewBag.ListadoProductos = productos;
            ViewBag.LimitesEquipos = limitesEquipos;
            ViewBag.PreciosArrendamiento = ViewData["PreciosArrendamiento"] = JsonConvert.SerializeObject(preciosArrendqamientos);

            return View();
        }


        [HttpPost]

        public async Task<JsonResult> CorreoSolicitudFirmaAdendumAdicionEquipo(SolicitudesViewModel model)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var _emailTemplateService = new EmailTemplateService();
                        var datosCliente = _connection.OrionContext.sp_OrionContratista_DetalleBySolicitud(1, model.IDSolicitud, 0, 0).FirstOrDefault();
                        await _emailTemplateService.SendEmailToSolicitudFirmaAdicionEquipos(new EmailTemplateServiceModel
                        {
                            CustomerEmail = datosCliente.fcCorreo,
                            IDSolicitud = (int)datosCliente.fiIDSolicitud,
                            IdCliente = (int)datosCliente.fiIDEquifax,
                            List_CC = new List<string> { MemoryLoadManager.EmailSystemAdministrator }
                        });
                        return EnviarResultado(true, "Token", "Mensaje Reenviada");


                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }
        }



        [HttpPost]
        public ActionResult GuardarAdicionEquipo(int IDCliente, int IdSolicitud, string jsondetalles, HttpPostedFileBase comprobantePago, string fcExtension, List<Base64ynombreViewModel> fotosbase)
        {
            try
            {
                var usuario = GetUser();
                var listaDetalles = JsonConvert.DeserializeObject<List<EquipoViewModel>>(jsondetalles);
                string comprobanteNombre = null;
                string comprobanteURL = null;
                string rutaFisica = null;
                using (var context = new ORIONDBEntities())
                {
                    string jsonDetallesStr = JsonConvert.SerializeObject(listaDetalles);

                    var fiIDAdicionProduto = context.Database.SqlQuery<int>(
                        "EXEC sp_SolicitudesAdicionProducto_Insertar_Orion @fiIDCliente, @fiIDSolicitud, @jsondetalles, @piIDApp, @fiIDUsuarioSolicitante",
                        new SqlParameter("@fiIDCliente", IDCliente),
                        new SqlParameter("@fiIDSolicitud", IdSolicitud),
                        new SqlParameter("@jsondetalles", jsonDetallesStr),
                        new SqlParameter("@piIDApp", 2),
                        new SqlParameter("@fiIDUsuarioSolicitante", usuario.fiIdUsuario)
                    ).FirstOrDefault();

                    if (fiIDAdicionProduto <= 0)
                    {
                        return EnviarResultado(false, "Error al guardar la adición de equipo.");
                    }
                    rutaFisica = $@"C:\inetpub\novanetgroup.com_Orion\Solicitudes\Solicitud_{IdSolicitud}\Solicitud_AdicionEquipo";
                    var documentoURL = $@"\Solicitudes\Solicitud_{IdSolicitud}\Solicitud_AdicionEquipo";

                    if (comprobantePago != null && comprobantePago.ContentLength > 0)
                    {
                        string nombreOriginal = Path.GetFileNameWithoutExtension(comprobantePago.FileName);
                        string extension = Path.GetExtension(comprobantePago.FileName);
                        comprobanteNombre = $"Comprobante_pago_IDEquifax_{IDCliente}_IDAdicionProduto_{fiIDAdicionProduto}_{nombreOriginal}{extension}";
                        string tempFilePath = Path.Combine(Path.GetTempPath(), comprobanteNombre);
                        comprobantePago.SaveAs(tempFilePath);
                        _UploadFileServer148(documentoURL, comprobantePago, comprobanteNombre);
                        comprobanteURL = $"https://orion.novanetgroup.com{documentoURL}/{comprobanteNombre}".Replace("\\", "/");
                        using (var command = context.Database.Connection.CreateCommand())
                        {
                            context.Database.Connection.Open();
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = "sp_OrionSolicitud_Documentos_Crear";
                            command.Parameters.Add(new SqlParameter("@piIDUSuario", usuario.fiIdUsuario));
                            command.Parameters.Add(new SqlParameter("@piIDSolicitud", IdSolicitud));
                            command.Parameters.Add(new SqlParameter("@pcNombreArchivo", comprobanteNombre));
                            command.Parameters.Add(new SqlParameter("@pcExtencion", fcExtension));
                            command.Parameters.Add(new SqlParameter("@pcRutaArchivo", rutaFisica));
                            command.Parameters.Add(new SqlParameter("@pcURL", comprobanteURL));
                            command.ExecuteNonQuery();
                            context.Database.Connection.Close();
                        }

                        System.IO.File.Delete(tempFilePath);
                    }
                }

                return Json(new { Estado = true, Titulo = "Adición de Equipo", Mensaje = "Solicitud creada exitosamente y comprobante guardado." });
            }
            catch (Exception ex)
            {
                return EnviarException(ex, "Error al guardar la adición de equipo y comprobante.");
            }
        }


        #endregion

    }
}