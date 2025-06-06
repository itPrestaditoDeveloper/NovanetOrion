using Newtonsoft.Json;
using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.App_Services.TecnicoService;
using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models.Base;
using OrionCoreCableColor.Models.Solicitudes;
using OrionCoreCableColor.Models.Tecnico;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Drawing.Imaging;
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
    [Authorize(Roles = "coreseguridad_AccesoAlSistema, coreseguridad_AccesoSolicitudesTecnicos")]
    public class TenicoController : BaseController
    {
        // GET: Tenico
        private readonly DbServiceConnection _connection;
        private readonly ApiTecnicosService _apitecnico;
        public TenicoController()
        {
            var ConnectionString = ConfigurationManager.ConnectionStrings["OrionConnections"].ConnectionString;
            _connection = new DbServiceConnection(ConnectionString);
            _apitecnico = new ApiTecnicosService();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult BandejaSolicitudesAsignadasTecnico()
        {
            ViewBag.IdInfoUser = GetUser();
            return View();
        }


        public ActionResult BandejaOrdenesTrabajoAsignadasTecnico()
        {

            ViewBag.IdInfoUser = GetUser();
            return View();
        }

        public async Task<JsonResult> ListaSolicitudes()
        {
            try
            {

                int user = GetIdUser();
                var usuaruiologueado = GetUser();
                var listadoTecnico = _connection.OrionContext.sp_SolicitudesAsignadas_By_Tecnico(usuaruiologueado.fiIdUsuario, usuaruiologueado.IdRol).ToList();
                //var listausuario = await _apitecnico.GetApiListadoTecnico(usuaruiologueado.IdRol, usuaruiologueado.fiIdUsuario);//llamado a la api asi que por ambiente de prueba se cambiara
                var jsonResult = new JsonResult();
                jsonResult = EnviarListaJson(listadoTecnico.Where(a => a.fiIDTipoSolicitud == 1).OrderBy(x => x.fiNoOrden));

                return jsonResult;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<JsonResult> ListaSolicitudesOrdenTrabajo()
        {
            try
            {

                int user = GetIdUser();
                var usuaruiologueado = GetUser();
                var listadoTecnico = _connection.OrionContext.sp_SolicitudesAsignadas_By_Tecnico(usuaruiologueado.fiIdUsuario, usuaruiologueado.IdRol).ToList();
                //var listausuario = await _apitecnico.GetApiListadoTecnico(usuaruiologueado.IdRol, usuaruiologueado.fiIdUsuario);//llamado a la api asi que por ambiente de prueba se cambiara
                var jsonResult = new JsonResult();
                jsonResult = EnviarListaJson(listadoTecnico.Where(a => a.fiIDTipoSolicitud == 2).OrderBy(x => x.fiNoOrden));

                return jsonResult;
            }
            catch (Exception e)
            {
                throw;
            }
        }


        [AllowAnonymous]
        public ActionResult ViewDetalleInstalacion(int idSolicitudInstalacion)
        {

            //var listadoSolicitud = await _apitecnico.GetApiInfoInstalacion(idSolicitudInstalacion); // hacer el llamado despues de la api, ya esta todo, solo seria cambiar lo que esta abajo con esto 
            var listadetalle = new InstalacionViewModel();

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_Instalacion_TecnicosExternos {idSolicitudInstalacion}";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    listadetalle = db.ObjectContext.Translate<InstalacionViewModel>(reader).FirstOrDefault();
                }

            }
            var empresa = _connection.OrionContext.sp_ObtenerEmpresa_ByIdUsuario(GetIdUser()).FirstOrDefault();
            ViewBag.CodigoBarra = _connection.OrionContext.sp_Productos_Inventario_SeriesPorTecnico().Where(a => a.fiIDEmpresa == empresa).ToList();

            return View(listadetalle);
        }

        public ActionResult TerminarInstalacion(TerminarInstalacionYOrdenTrabajo modelo)
        {
            try
            {
                //subir Imagenes que tomaron Tecnicos
                List<ImagenesaEnviarProcedimiento> Listaimagenes = new List<ImagenesaEnviarProcedimiento>();
                foreach (var item in modelo.flListadoFotosList)
                {
                    var arch = ConvertirBase64AImagen(item.base64, item.FcNombreFoto);

                    UploadFileServer148(@"\Solicitudes\Solicitud_" + modelo.FiIDSolicitud + @"\SolicitudInstalacion_" + modelo.FiIDContratistaSolicitud, arch);
                    Listaimagenes.Add(new ImagenesaEnviarProcedimiento
                    {
                        fiIDSolicitudContratista = modelo.FiIDContratistaSolicitud,
                        fiIDCatalogoFotoInstalacion = item.FiIDCatalogo_FotosInstalacion,
                        fcUrlDocumento = @"Solicitudes\\Solicitud_" + modelo.FiIDSolicitud + @"\\SolicitudInstalacion_" + modelo.FiIDContratistaSolicitud + @"\\" + item.FcNombreFoto,
                        fcNombreDocumento = item.FcNombreFoto,
                        fcExtension = Path.GetExtension(arch.FileName)
                    }); //siento que se puede mejorar esto pero de momento no logro saber como hacerlo....
                }
                string ListaimagenesJson = JsonConvert.SerializeObject(Listaimagenes, Formatting.Indented);
                var result = _connection.OrionContext.sp_Contratista_SolicitudIntalacion_TerminarInstalacion(modelo.FiIDContratistaSolicitud, modelo.fcComentarioInstalacion, 6, ListaimagenesJson, modelo.ListaMateriales, GetIdUser(), modelo.fcVinietaCliente);
                ActualizarFilaTecnico(modelo.FiIDContratistaSolicitud);
                //------------------------------------------------------------
                //Nota de Edgardito: Favor de arreglar esto despues por que todo iba bien hasta que llego este punto....
                var listadetalle = new InstalacionViewModel();

                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_Instalacion_TecnicosExternos {modelo.FiIDContratistaSolicitud}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        listadetalle = db.ObjectContext.Translate<InstalacionViewModel>(reader).FirstOrDefault();
                    }
                    connection.Close();
                }

                //enviar mensaje
                var bod = $"Estimado se mandado a Validar la Solicitud *{listadetalle.FiIDSolicitud}* del cliente Cliente: {listadetalle.FcNombre}";
                var numeroTecnicoJefe = _connection.OrionContext.sp_Configuraciones().FirstOrDefault(a => a.NombreLlave == "NumeroTecnicoJefe").ValorLLave;
                //mensaje al Tecnico Superior
                MensajeriaApi.MensajesDigitales(numeroTecnicoJefe, bod);


                EnviarNotificacion($"Se finalizo la instalacion del cliente: {listadetalle.FcNombre}");
                EnviarMensaje_QR_Vendedor_WhatsApp(listadetalle.FiIDSolicitud.ToString());

                return EnviarResultado(true, "Instalacion Terminada con Exito!");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult TerminarEntregaArticulos(TerminarInstalacionYOrdenTrabajo modelo)
        {
            try
            {
                //subir Imagenes que tomaron Tecnicos
                List<ImagenesaEnviarProcedimiento> Listaimagenes = new List<ImagenesaEnviarProcedimiento>();
                foreach (var item in modelo.flListadoFotosList)
                {
                    var arch = ConvertirBase64AImagen(item.base64, item.FcNombreFoto);

                    UploadFileServer148(@"\Solicitudes\Solicitud_" + modelo.FiIDSolicitud + @"\SolicitudInstalacion_" + modelo.FiIDContratistaSolicitud, arch);
                    Listaimagenes.Add(new ImagenesaEnviarProcedimiento
                    {
                        fiIDSolicitudContratista = modelo.FiIDContratistaSolicitud,
                        fiIDCatalogoFotoInstalacion = item.FiIDCatalogo_FotosInstalacion,
                        fcUrlDocumento = @"Solicitudes\\Solicitud_" + modelo.FiIDSolicitud + @"\\SolicitudInstalacion_" + modelo.FiIDContratistaSolicitud + @"\\" + item.FcNombreFoto,
                        fcNombreDocumento = item.FcNombreFoto,
                        fcExtension = Path.GetExtension(arch.FileName)
                    }); //siento que se puede mejorar esto pero de momento no logro saber como hacerlo....
                }
                string ListaimagenesJson = JsonConvert.SerializeObject(Listaimagenes, Formatting.Indented);
                var result = _connection.OrionContext.sp_Contratista_SolicitudIntalacion_TerminarInstalacion_ProductosExtra(modelo.FiIDContratistaSolicitud, modelo.fcComentarioInstalacion, 7, ListaimagenesJson, modelo.ListaMateriales, GetIdUser());

                ActualizarFilaTecnico(modelo.FiIDContratistaSolicitud);

                //------------------------------------------------------------
                //Nota de Edgardito: Favor de arreglar esto despues por que todo iba bien hasta que llego este punto....
                var listadetalle = new InstalacionViewModel();

                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_Instalacion_TecnicosExternos {modelo.FiIDContratistaSolicitud}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        listadetalle = db.ObjectContext.Translate<InstalacionViewModel>(reader).FirstOrDefault();
                    }
                    connection.Close();
                }
                EnviarNotificacion($"Se Entrego el Articulo Al Cliente: {listadetalle.FcNombre}");

                if (listadetalle.fbArticulosExtra)
                {
                    MensajeriaApi.EnviarmensajeEntregadoArticuloCliente(listadetalle.FcNombre, "31864531", listadetalle.FcArticulosdelContrato);
                    MensajeriaApi.EnviarmensajeEntregadoArticuloCliente(listadetalle.FcNombre, "31843783", listadetalle.FcArticulosdelContrato);
                    MensajeriaApi.EnviarmensajeEntregadoArticuloCliente(listadetalle.FcNombre, "33798826", listadetalle.FcArticulosdelContrato);
                    MensajeriaApi.EnviarmensajeEntregadoArticuloCliente(listadetalle.FcNombre, "31598536", listadetalle.FcArticulosdelContrato);
                    MensajeriaApi.EnviarmensajeEntregadoArticuloCliente(listadetalle.FcNombre, "87535712", listadetalle.FcArticulosdelContrato);
                }

                return EnviarResultado(true, "Entrega del Articulo Finalizo con Exito!");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult TerminarOrdenDeTrabajo(TerminarInstalacionYOrdenTrabajo modelo)
        {
            try
            {
                //subir Imagenes que tomaron Tecnicos
                List<ImagenesaEnviarProcedimiento> Listaimagenes = new List<ImagenesaEnviarProcedimiento>();
                foreach (var item in modelo.flListadoFotosList)
                {
                    var arch = ConvertirBase64AImagen(item.base64, item.FcNombreFoto);

                    UploadFileServer148(@"\Solicitudes\Solicitud_" + modelo.FiIDSolicitud + @"\SolicitudInstalacion_" + modelo.FiIDContratistaSolicitud, arch);
                    Listaimagenes.Add(new ImagenesaEnviarProcedimiento
                    {
                        fiIDSolicitudContratista = modelo.FiIDContratistaSolicitud,
                        fiIDCatalogoFotoInstalacion = item.FiIDCatalogo_FotosInstalacion,
                        fcUrlDocumento = @"Solicitudes\\Solicitud_" + modelo.FiIDSolicitud + @"\\SolicitudInstalacion_" + modelo.FiIDContratistaSolicitud + @"\\" + item.FcNombreFoto,
                        fcNombreDocumento = item.FcNombreFoto,
                        fcExtension = Path.GetExtension(arch.FileName)
                    }); //siento que se puede mejorar esto pero de momento no logro saber como hacerlo....
                }

                string ListaimagenesJson = JsonConvert.SerializeObject(Listaimagenes, Formatting.Indented);
                var result = _connection.OrionContext.sp_Contratista_SolicitudIntalacion_TerminarInstalacion(modelo.FiIDContratistaSolicitud, modelo.fcComentarioInstalacion, 8, ListaimagenesJson, modelo.ListaMateriales, GetIdUser(), modelo.fcVinietaCliente);




                return EnviarResultado(true, "Orden de trabajo Terminada con Exito!");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult BusquedaVineta()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult BusquedaVineta(string vineta)
        {
            List<VinetaData> clientList = new List<VinetaData>();

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_OrionSolicitud_Maestro_BuscarVineta {vineta}";

                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    clientList = db.ObjectContext.Translate<VinetaData>(reader).ToList();
                }
                connection.Close();
            }
            var data = EnviarListaJson(clientList);
            return data;
        }


        public ActionResult CrearCreditoCliente(int fiIdSolicitudinstalacion, int idsolicitud)
        {
            try
            {
                //aqui se crea lo que viene siendo el credito del cliente y tambien se cambia la solicitud del cliente a que esta pendiente de entregar el articulo y falta de entregar el asticulo
                var comentarioInsertar = _connection.OrionContext.sp_GenerarCreditoClienteConArticulo(fiIdSolicitudinstalacion, idsolicitud, GetIdUser()).FirstOrDefault();
                ActualizarFilaTecnico(fiIdSolicitudinstalacion);

                EnviarMensajeArticulosPendientes(fiIdSolicitudinstalacion);
                //var listadetalle = new InstalacionViewModel();

                //using (var connection = (new ORIONDBEntities()).Database.Connection)
                //{
                //    connection.Open();
                //    var command = connection.CreateCommand();
                //    command.CommandText = $"EXEC sp_Instalacion_TecnicosExternos {fiIdSolicitudinstalacion}";
                //    using (var reader = command.ExecuteReader())
                //    {
                //        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                //        listadetalle = db.ObjectContext.Translate<InstalacionViewModel>(reader).FirstOrDefault();
                //    }
                //    connection.Close();
                //}

                //if (listadetalle.fbArticulosExtra)
                //{
                //    var telefonos = GetConfiguracion<string>("Numeros_Personas_Caer_Mensaje_Solicitud_Con_Articulos", ',');
                //    foreach (var item in telefonos)
                //    {
                //        MensajeriaApi.EnviarmensajeAgenteArticulosExtra(listadetalle.FcNombre, Convert.ToString(item), listadetalle.FcArticulosdelContrato, listadetalle.DireccionDetalladaCliente, listadetalle.fcGeolocalizacion);
                //    }
                //}

                return EnviarResultado(true, "Se envió resultado al técnico");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public JsonResult EnviarMensajeArticulosPendientes(int fiIdSolicitudinstalacion)
        {
            try
            {
                var listadetalle = new InstalacionViewModel();

                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_Instalacion_TecnicosExternos {fiIdSolicitudinstalacion}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        listadetalle = db.ObjectContext.Translate<InstalacionViewModel>(reader).FirstOrDefault();
                    }
                    connection.Close();
                }

                if (listadetalle.fbArticulosExtra)
                {
                    var telefonos = GetConfiguracion<string>("Numeros_Personas_Caer_Mensaje_Solicitud_Con_Articulos", ',');
                    foreach (var item in telefonos)
                    {
                        MensajeriaApi.EnviarmensajeAgenteArticulosExtra(listadetalle.FcNombre, Convert.ToString(item), listadetalle.FcArticulosdelContrato, listadetalle.DireccionDetalladaCliente, listadetalle.fcGeolocalizacion);
                    }
                }

                return EnviarResultado(true, "Se envió resultado al técnico");
                
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public ActionResult ValidarOrdenTrabajo(int fiIdSolicitudinstalacion, int idsolicitud)
        {
            try
            {
                var listadetalle = new InstalacionViewModel();

                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_Instalacion_TecnicosExternos {fiIdSolicitudinstalacion}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        listadetalle = db.ObjectContext.Translate<InstalacionViewModel>(reader).FirstOrDefault();
                    }
                    connection.Close();
                }

                var usuario = GetUser();
                var guardar = _connection.OrionContext.sp_Validar_SOP(fiIdSolicitudinstalacion, usuario.fiIdUsuario).FirstOrDefault(); //_connection.OrionContext.sp_
                ActualizarFilaOrdenTrabajo(fiIdSolicitudinstalacion);

                //var conten = DataCrypt.Encriptar($"Reportes/AnexosOrdenDeTrabajo?solicitudContratista={fiIdSolicitudinstalacion}");
                var conten = $"Reportes/AnexosOrdenDeTrabajo?solicitudContratista={fiIdSolicitudinstalacion}";


                MensajeriaApi.EnviarMensajePersonalizado("87535712"/*listadetalle.FcTelefono*/, $"Estimado Cliente, se le adjunta el link de Finalizacion de trabajo: \n \n https://orion.novanetgroup.com/{conten}");


                return EnviarResultado(true, "Se envio la revicion");
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public ActionResult ViewDetalleInstalacionOrdenTrabajo(int idSolicitudInstalacion)
        {
            //var listadoSolicitud = await _apitecnico.GetApiInfoInstalacion(idSolicitudInstalacion); // hacer el llamado despues de la api, ya esta todo, solo seria cambiar lo que esta abajo con esto 
            var listadetalle = new InstalacionViewModel();

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_Instalacion_TecnicosExternos {idSolicitudInstalacion}";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    listadetalle = db.ObjectContext.Translate<InstalacionViewModel>(reader).FirstOrDefault();
                }
            }

            var empresa = _connection.OrionContext.sp_ObtenerEmpresa_ByIdUsuario(GetIdUser()).FirstOrDefault();
            ViewBag.CodigoBarra = _connection.OrionContext.sp_Productos_Inventario_SeriesPorTecnico().Where(a => a.fiIDEmpresa == empresa).ToList();

            return View(listadetalle);

            //var ListadoDeMaterialesDB = _connection.OrionContext.sp_MaterialesInstalacion_ListarByInstalacion(idSolicitudInstalacion).ToList();
            //var ListadoMateriales = new List<Listado_materiales_viewModel>();
            //var materialessincodigobarra = GetListProductosConsumibles();

            //var empresa = _connection.OrionContext.sp_ObtenerEmpresa_ByIdUsuario(GetIdUser()).FirstOrDefault();
            //var tecnico = GetIdTecnico();
            //ViewBag.CodigoBarra = _connection.OrionContext.sp_Productos_Inventario_SeriesPorTecnico().Where(a => a.fiIDEmpresa == empresa).ToList();
            //foreach (var item in ListadoDeMaterialesDB)
            //{
            //    ListadoMateriales.Add(new Listado_materiales_viewModel() { fiIDProducto = item.fiIDProducto, fcProducto = item.fcProducto.Replace("\\", "").Replace("\"", ""), fnCantidad = 1, fiIDMarca = item.fiIDMarca, fbCodigo = !materialessincodigobarra.Any(a => a == item.fiIDProducto) });
            //}
            //detallesolicitudInstalacion_ViewModel model = new detallesolicitudInstalacion_ViewModel();
            //model.fiIDSolicitudContratista = idSolicitudInstalacion;
            //var infoCliente = _connection.OrionContext.sp_InformacionInstalacion_Tecnico(idSolicitudInstalacion).FirstOrDefault();
            //model.fcArticulosAsignados = infoCliente.fcArticulosdelContrato;
            //model.fcCliente = infoCliente.fcNombre;

            //var listadoEstados = _connection.OrionContext.sp_ListaCatalogoInstalacion().ToList();
            //ViewBag.listadoEstados = listadoEstados.Select(x => new { Value = x.fiIDEstadoInstalacion, Text = x.fcDescripcion }).ToList();
            //var listaFotosDB = _connection.OrionContext.sp_catalogo_FotosInstalacion_lista().ToList();
            //List<lista_detallesolicitudInstalacion_ViewModel> listadoFotos = new List<lista_detallesolicitudInstalacion_ViewModel>();
            //foreach (var item in listaFotosDB)
            //{
            //    listadoFotos.Add(new lista_detallesolicitudInstalacion_ViewModel() { fiIDCatalogo_FotosInstalacion = item.fiIDCatalogo_FotosInstalacion, fcDescripcion = item.fcDescripcion, fcNombreFoto = item.fcNombreFoto });
            //}
            //model.listadoFotos = listadoFotos;
            //var EstadoSolicitud = _connection.OrionContext.sp_ObtenerEstado_SolicitudInstalacion(idSolicitudInstalacion).FirstOrDefault();
            //model.fiIDEstadoInstalacion = EstadoSolicitud.fiIDEstadoInstalacion ?? 0;
            //model.fcDescripcionEstado = EstadoSolicitud.fcDescripcion;
            //model.fcClase = EstadoSolicitud.fcClase;
            //model.listadoMateriales = ListadoMateriales;
            //return View(model);
        }

        public ActionResult ViewRevisarDetalleInstalacion(int idSolicitudInstalacion)
        {

            var ListadoDeMaterialesDB = _connection.OrionContext.sp_ListadoMaterialesInstalados_BySolicitudInstalacion(idSolicitudInstalacion).ToList();
            var ListadoMateriales = new List<Revisar_Listado_materiales_viewModel>();
            foreach (var item in ListadoDeMaterialesDB)
            {
                ListadoMateriales.Add(new Revisar_Listado_materiales_viewModel() { fcProducto = item.fcProducto.Replace("\\", "").Replace("\"", ""), fnCantidad = item.fnCantidad ?? 0, });
            }
            revisarinstalacionViewModel model = new revisarinstalacionViewModel();
            model.fiIDSolicitudContratista = idSolicitudInstalacion;
            var infoCliente = _connection.OrionContext.sp_InformacionInstalacion_Tecnico(idSolicitudInstalacion).FirstOrDefault();
            model.fcArticulosAsignados = infoCliente.fcArticulosdelContrato;
            model.fcCliente = infoCliente.fcNombre;

            var listadoEstados = _connection.OrionContext.sp_ListaCatalogoInstalacion().ToList();
            ViewBag.listadoEstados = listadoEstados.Select(x => new { Value = x.fiIDEstadoInstalacion, Text = x.fcDescripcion }).ToList();
            var listaFotosDB = _connection.OrionContext.sp_ListadoFotosSubidas_BySolicitudInstalacion(idSolicitudInstalacion).ToList();
            List<Revisar_lista_detallesolicitudInstalacion_ViewModel> listadoFotos = new List<Revisar_lista_detallesolicitudInstalacion_ViewModel>();
            foreach (var item in listaFotosDB)
            {
                listadoFotos.Add(new Revisar_lista_detallesolicitudInstalacion_ViewModel() { fiIDCatalogo_FotosInstalacion = item.fiIDCatalogoFotoInstalacion ?? 0, fcDescripcion = item.fcNombreDocumento, fcUrlDocumento = item.fcUrlDocumento });
            }
            model.listadoFotos = listadoFotos;
            var EstadoSolicitud = _connection.OrionContext.sp_ObtenerEstado_SolicitudInstalacion(idSolicitudInstalacion).FirstOrDefault();
            model.fiIDEstadoInstalacion = EstadoSolicitud.fiIDEstadoInstalacion ?? 0;
            model.fcDescripcionEstado = EstadoSolicitud.fcDescripcion;
            model.fcClase = EstadoSolicitud.fcClase;
            model.listadoMateriales = ListadoMateriales;
            return View(model);
        }

        // creo que ya no se usa por que se esta usando otro metodo que esta mejor estructurado... ver si ya no se esta usando para asi eliminarlo del todo
        public ActionResult EditarInstalacion(detallesolicitudInstalacion_ViewModel model)
        {
            try
            {
                //model.fiIDEstadoInstalacion = 6;//Estado validacion
                //var comentarioInsertar = _connection.OrionContext.sp_SolicitudInstalacion_ObtenerComentarioInstalacion(model.fiIDSolicitudContratista).FirstOrDefault()??"";
                var conte = _connection.OrionContext.sp_SolicitudesAsignadas_ByTecnico().Where(a => a.fiIDSolicitudInstalacion == model.fiIDSolicitudContratista).FirstOrDefault();
                if (!string.IsNullOrEmpty(model.fcVinietaCliente))
                {

                    //var guardarviñeta = _connection.OrionContext.sp_CambiarVinietaSolicitud_Temp(conte.fiIDSolicitud, model.fcVinietaCliente);
                    using (var connection = (new ORIONDBEntities()).Database.Connection)
                    {

                        connection.Open();
                        var command = connection.CreateCommand();
                        command.CommandText = $"EXEC sp_CambiarVinietaSolicitud_Temp {conte.fiIDSolicitud} , {model.fcVinietaCliente}";
                        using (var reader = command.ExecuteReader())
                        {

                        }
                        connection.Close();
                    }
                }

                var datosCliente = new sp_OrionSolicitud_Detalle_ClienteListarViewModel();
                var Actualizar = _connection.OrionContext.sp_ActualizarInstalacion_Tecnico(model.fiIDSolicitudContratista, model.fiIDEstadoInstalacion, model.fnMetrosFibra, model.fcComentarioInstalacion) > 0;
                var idsolicitud = 0;



                if (Actualizar)
                {
                    //model.fiIDSolicitudContratista // idsolicitud para verificar
                    foreach (var item in model.listadoMateriales)
                    {
                        var validarregistro = _connection.OrionContext.sp_ContratistaSolicitudInstalacionDetalle_ValidarIDContratista(model.fiIDSolicitudContratista, item.fiIDProducto).FirstOrDefault();
                        if (validarregistro.fiMensaje == "0")
                        {

                            var insertarDetalle = _connection.OrionContext.sp_ContratistaSolicitudInstalacionDetalle_Insertar(model.fiIDSolicitudContratista, item.fiIDProducto, 0, item.fnCantidad, "_", "_", GetIdUser()).FirstOrDefault();
                            idsolicitud = (int)insertarDetalle.IdActualizado;
                            if (item.fbCodigo && item.fnCantidad > 0)
                            {
                                foreach (var iten in item.CodigoBarra)
                                {
                                    var insercodigobarra = _connection.OrionContext.sp_Contratista_SolicitudInstalacion_Detalle_CodigoBarras_Insert(insertarDetalle.IdActualizado, iten.fiIDMovimiento).FirstOrDefault();
                                }
                            }
                            else
                            {

                            }

                        }

                    }
                    //foreach (var item in model.listadoFotos)
                    //{
                    //    var ImagenGuardada = _connection.OrionContext.sp_GuardarFotoInstalacion(model.fiIDSolicitudContratista, item.fiIDCatalogo_FotosInstalacion, @"ptdto.com\Orion\ImgProductos\imgInstalacion\" + item.Archivo.FileName, Path.GetFileNameWithoutExtension(item.Archivo.FileName), Path.GetExtension(item.Archivo.FileName)) > 0;

                    //    UploadFileServer148(@"ImgProductos\imgInstalacion", item.Archivo);
                    //}
                    //generar bitacora
                    var bitacora = _connection.OrionContext.sp_Solicitud_Instalacion_Bitacoras_Crear(model.fiIDSolicitudContratista, "Se actualizo la instalacion a Validacion", GetIdUser());


                    var listaDetalleCliente = new sp_OrionSolicitud_Detalle_ClienteListarViewModel();
                    var listaclienter = _connection.OrionContext.sp_OrionSolicitud_Detalle_ClienteListar(GetIdUser(), conte.fiIDSolicitud).FirstOrDefault();//new sp_OrionSolicitud_Detalle_ClienteListarViewModel();

                    using (var connection = (new ORIONDBEntities()).Database.Connection)
                    {

                        connection.Open();
                        var command = connection.CreateCommand();
                        command.CommandText = $"EXEC sp_OrionSolicitud_Detalle_ClienteListar {GetIdUser()} , {idsolicitud}";
                        using (var reader = command.ExecuteReader())
                        {
                            var db = ((IObjectContextAdapter)new ORIONDBEntities());
                            listaDetalleCliente = db.ObjectContext.Translate<sp_OrionSolicitud_Detalle_ClienteListarViewModel>(reader).FirstOrDefault();
                        }
                        connection.Close();
                    }

                    //enviar mensaje
                    var bod = $"Estimado se mandado a Validar la Solicitud *{conte.fiIDSolicitud}* del cliente Cliente: {conte.fcNombreCliente}";
                    var numeroTecnicoJefe = _connection.OrionContext.sp_Configuraciones().FirstOrDefault(a => a.NombreLlave == "NumeroTecnicoJefe").ValorLLave;
                    //mensaje al Tecnico Superior
                    MensajeriaApi.MensajesDigitales(numeroTecnicoJefe, bod);


                    EnviarNotificacion($"Se finalizo la instalacion del cliente: {conte.fcNombreCliente}");
                    EnviarMensaje_QR_Vendedor_WhatsApp(conte.fiIDSolicitud.ToString());

                    if ((bool)conte.fbArticulosExtra)
                    {
                        MensajeriaApi.EnviarmensajeAgenteArticulosExtra(conte.fcNombreCliente, "31864531", listaclienter.fcArticulosdelContrato, listaclienter.DireccionDetalladaCliente, listaclienter.fcGeolocalizacion);
                        MensajeriaApi.EnviarmensajeAgenteArticulosExtra(conte.fcNombreCliente, "31843783", listaclienter.fcArticulosdelContrato, listaclienter.DireccionDetalladaCliente, listaclienter.fcGeolocalizacion);
                        MensajeriaApi.EnviarmensajeAgenteArticulosExtra(conte.fcNombreCliente, "33798826", listaclienter.fcArticulosdelContrato, listaclienter.DireccionDetalladaCliente, listaclienter.fcGeolocalizacion);
                        MensajeriaApi.EnviarmensajeAgenteArticulosExtra(conte.fcNombreCliente, "87535712", listaclienter.fcArticulosdelContrato, listaclienter.DireccionDetalladaCliente, listaclienter.fcGeolocalizacion);
                    }

                    return EnviarResultado(true, "Se envio la instalacion para su revision");

                }
                return EnviarResultado(false, "Error:Not Working");


            }
            catch (Exception e)
            {

                throw;
            }
        }

        //metodo que se creo para una solucion rapida de si la solicitud lleva articulos o no // creo que ya no se usara mas este metodo por la nueva restructuracion 
        public ActionResult EditarInstalacionConArticulos(detallesolicitudInstalacion_ViewModel model)
        {
            try
            {
                //model.fiIDEstadoInstalacion = 6;//Estado validacion
                //var comentarioInsertar = _connection.OrionContext.sp_SolicitudInstalacion_ObtenerComentarioInstalacion(model.fiIDSolicitudContratista).FirstOrDefault()??"";
                var conte = _connection.OrionContext.sp_SolicitudesAsignadas_ByTecnico().Where(a => a.fiIDSolicitudInstalacion == model.fiIDSolicitudContratista).FirstOrDefault();
                if (!string.IsNullOrEmpty(model.fcVinietaCliente))
                {

                    //var guardarviñeta = _connection.OrionContext.sp_CambiarVinietaSolicitud_Temp(conte.fiIDSolicitud, model.fcVinietaCliente);
                    using (var connection = (new ORIONDBEntities()).Database.Connection)
                    {

                        connection.Open();
                        var command = connection.CreateCommand();
                        command.CommandText = $"EXEC sp_CambiarVinietaSolicitud_Temp {conte.fiIDSolicitud} , {model.fcVinietaCliente}";
                        using (var reader = command.ExecuteReader())
                        {

                        }
                        connection.Close();
                    }
                }

                var datosCliente = new sp_OrionSolicitud_Detalle_ClienteListarViewModel();
                var Actualizar = _connection.OrionContext.sp_ActualizarInstalacion_Tecnico(model.fiIDSolicitudContratista, model.fiIDEstadoInstalacion, model.fnMetrosFibra, model.fcComentarioInstalacion) > 0;
                var idsolicitud = 0;



                if (Actualizar)
                {
                    //model.fiIDSolicitudContratista // idsolicitud para verificar
                    foreach (var item in model.listadoMateriales)
                    {
                        var validarregistro = _connection.OrionContext.sp_ContratistaSolicitudInstalacionDetalle_ValidarIDContratista(model.fiIDSolicitudContratista, item.fiIDProducto).FirstOrDefault();
                        if (validarregistro.fiMensaje == "0")
                        {
                            var insertarDetalle = _connection.OrionContext.sp_ContratistaSolicitudInstalacionDetalle_Insertar(model.fiIDSolicitudContratista, item.fiIDProducto, 0, item.fnCantidad, "_", "_", GetIdUser()).FirstOrDefault();
                            idsolicitud = (int)insertarDetalle.IdActualizado;
                            if (item.fbCodigo && item.fnCantidad > 0)
                            {
                                foreach (var iten in item.CodigoBarra)
                                {
                                    var insercodigobarra = _connection.OrionContext.sp_Contratista_SolicitudInstalacion_Detalle_CodigoBarras_Insert(insertarDetalle.IdActualizado, iten.fiIDMovimiento).FirstOrDefault();
                                }
                            }
                            else
                            {

                            }

                        }

                    }
                    var bitacora = _connection.OrionContext.sp_Solicitud_Instalacion_Bitacoras_Crear(model.fiIDSolicitudContratista, "Se actualizo la instalacion a Validacion", GetIdUser());


                    var listaDetalleCliente = new sp_OrionSolicitud_Detalle_ClienteListarViewModel();

                    using (var connection = (new ORIONDBEntities()).Database.Connection)
                    {

                        connection.Open();
                        var command = connection.CreateCommand();
                        command.CommandText = $"EXEC sp_OrionSolicitud_Detalle_ClienteListar {GetIdUser()} , {idsolicitud}";
                        using (var reader = command.ExecuteReader())
                        {
                            var db = ((IObjectContextAdapter)new ORIONDBEntities());
                            listaDetalleCliente = db.ObjectContext.Translate<sp_OrionSolicitud_Detalle_ClienteListarViewModel>(reader).FirstOrDefault();
                        }
                        connection.Close();
                    }

                    //enviar mensaje
                    var bod = $"Estimado se mandado a Validar la Solicitud *{conte.fiIDSolicitud}* del cliente Cliente: {conte.fcNombreCliente}";
                    var numeroTecnicoJefe = _connection.OrionContext.sp_Configuraciones().FirstOrDefault(a => a.NombreLlave == "NumeroTecnicoJefe").ValorLLave;
                    //mensaje al Tecnico Superior
                    MensajeriaApi.MensajesDigitales(numeroTecnicoJefe, bod);

                    EnviarNotificacion($"Se finalizo la instalacion del cliente: {conte.fcNombreCliente}");
                    EnviarMensaje_QR_Vendedor_WhatsApp(conte.fiIDSolicitud.ToString());

                    return EnviarResultado(true, "Se envio la instalacion para su revision");

                }
                return EnviarResultado(false, "Error:Not Working");


            }
            catch (Exception e)
            {

                throw;
            }
        }


        public string EnviarMensaje_QR_Vendedor_WhatsApp(string piIDSolicitud)
        {
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_Orion_VentaSolicitudCliente_Promocion_Datos {piIDSolicitud}";

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var data = new
                        {
                            fcNumeroVendedor = reader["fcNumeroVendedor"].ToString(),
                            fiIDSolicitud = reader["fiIDSolicitud"].ToString(),
                            fcToken = reader["fcToken"].ToString(),
                        };

                        string url = "https://api.novanetgroup.com/api/Novanet/Mensajeria/EnviarMensaje_QR_Vendedor_WhatsApp";
                        string fullUrl = $"{url}?url=https://orion.novanetgroup.com/dAtosCliente/QRDescuento?UID={Uri.EscapeDataString(data.fcToken)}&telefono={data.fcNumeroVendedor}";

                        using (var httpClient = new HttpClient())
                        {
                            var response = httpClient.PostAsync(fullUrl, null).Result;
                            string jsonResponse = response.Content.ReadAsStringAsync().Result;
                            return jsonResponse;
                        }
                    }
                    else
                    {
                        return "No se encontraron datos para la solicitud proporcionada.";
                    }
                }
            }
        }


        static string CleanBase64String(string base64String) // aqui se limpia el base64 para poder convertirlo en un array de byte que si no estoy mal se usa para los archivos, ya sea word o pdf, menos las imagenes, para eso se usa otra funcion
        {
            // Eliminar cualquier caracter que no sea válido para Base64
            return Regex.Replace(base64String, @"[^a-zA-Z0-9\+/=]", "");
        }

        static HttpPostedFileBase ConvertirBase64AImagen(string base64String, string nombreArchivo)
        {
            // Convertir la cadena Base64 en un array de bytes
            byte[] bytes = Convert.FromBase64String(CleanBase64String(base64String));

            HttpPostedFileBase archivo = new ByteArrayHttpPostedFile(base64String, nombreArchivo);
            //GuardarImagen(bytes, archivo);
            return archivo;

        }

        static void GuardarImagen(byte[] bytes, HttpPostedFileBase formato) // funcion para guardar imagenes que por el momento me tiro mas errores que la relacion que tuve con mi ex
        {
            // Convertir los bytes en una imagen
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                var obtenerextencion = (ImageFormat)typeof(ImageFormat).GetProperty(Path.GetExtension(formato.FileName).Substring(1), typeof(ImageFormat)).GetValue(null);
                using (Image imagen = Image.FromStream(ms))
                {
                    // Guardar la imagen en el archivo especificando el formato

                    imagen.Save(@"ImgProductos\imgInstalacion", obtenerextencion);
                }
            }
        }


        public JsonResult GuardarFotoInstalacion(int fiIDCatalogo_FotosInstalacion, List<HttpPostedFileBase> Archivo, int fiIDSolicitudContratista, int numeroFoto, List<Base64ynombreViewModel> fotosbase)
        {
            try
            {
                if (fotosbase is null)
                {
                    return EnviarResultado(false, "suba como Minimo una foto");
                }
                else
                {
                    foreach (var item in fotosbase)
                    {
                        var arch = ConvertirBase64AImagen(item.base64string, item.nombrearchivo);

                        var ImagenGuardada = _connection.OrionContext.sp_GuardarFotoInstalacion(fiIDSolicitudContratista, item.numimagen, MemoryLoadManager.UrlWeb + @"\ImgProductos\imgInstalacion\" + arch.FileName, Path.GetFileNameWithoutExtension(arch.FileName), Path.GetExtension(arch.FileName)).ToList();

                        //arch.SaveAs(MemoryLoadManager.URL +@"\" + arch.FileName);
                        UploadFileServer148(@"ImgProductos\imgInstalacion", arch);

                    }
                    //return Json(new { contador = numeroFoto + 1 });
                    return EnviarResultado(true, "Imagenes subidas exitosamente");
                }

            }
            catch (Exception e)
            {
                return EnviarException(e, "ERROR");
                throw;
            }

        }
        public ActionResult ActualizarEstadoaProceso(int fiIDSolicitudInstalacion, string nombrecliente, string articulos)
        {
            try
            {
                //1   Pendiente warning 1
                //2   Asignado info    1
                //3   Proceso primary 1
                //4   Isntalado success 1
                //5   Cancelado danger  1
                //6   Validacion secondary   1

                var Actualizar = _connection.OrionContext.sp_ActualizarInstalacionaProceso(fiIDSolicitudInstalacion, 3) > 0;
                var cliente = nombrecliente;
                //enviar mensaje
                var bod = "Estimado se ha comenzado el proceso de instalación del cliente: " + cliente + " y lleva estos Articulos " + articulos;
                var numeroTecnicoJefe = _connection.OrionContext.sp_Configuraciones().FirstOrDefault(a => a.NombreLlave == "NumeroTecnicoJefe").ValorLLave;
                //mensaje al Tecnico Superior
                MensajeriaApi.MensajesDigitales(numeroTecnicoJefe, bod);

                if (Actualizar)
                {
                    //generar bitacora 
                    var bitacora = _connection.OrionContext.sp_Solicitud_Instalacion_Bitacoras_Crear(fiIDSolicitudInstalacion, "Se actualizo a Proceso", GetIdUser());
                    EnviarNotificacion($"Se empezo la instalacion del cliente: {nombrecliente}");
                    return EnviarResultado(true, "Se actualizo la instalacion a Proceso");

                }
                return EnviarResultado(false, "Error:Not Working");


            }
            catch (Exception)
            {

                throw;
            }
        }


        [HttpPost]
        public JsonResult GetInformacionBarCodeQr(HttpPostedFileBase file)

        {
            try
            {
                var qrRedear = new BarcodeReader();
                var imagen = new Bitmap(file.InputStream);
                var result = qrRedear.Decode(imagen);
                if (result == null)
                {
                    return EnviarResultado(false, "", "No se logro leer el QR, vuelva a intentarlo");
                }
                else
                {
                    //var data = new JavaScriptSerializer();
                    //var objeto = data.Deserialize(result.Text, typeof(ListPesadasAbiertasPuertoViewModel));

                    using (var contexto = new ORIONDBEntities())
                    {
                        return Json(new { Estado = true, producto = result.Text }, JsonRequestBehavior.AllowGet);




                    }

                }
            }
            catch (Exception ex)
            {
                return EnviarException(ex, "Error");
            }
        }

        public ActionResult ShowUrlAdendun(int fiIDSolicitudInstalacion, string url)
        {

            try
            {
                ViewBag.Linkurl = url;
                return PartialView();
            }
            catch (Exception)
            {

                throw;
            }

        }


        public ActionResult FotoQr(int fiIDSolicitudInstalacion)
        {
            ViewBag.IdInstalacion = fiIDSolicitudInstalacion;
            return PartialView();
        }

        public ActionResult ClientesConArticulos(int idSolicitudInstalacion, int idsolicitud, int idTipoSolicitud)
        {

            var listadetalle = new InstalacionViewModel();
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_Instalacion_TecnicosExternos_ArticulosExtra {idSolicitudInstalacion}";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    listadetalle = db.ObjectContext.Translate<InstalacionViewModel>(reader).FirstOrDefault();
                }
            }
            ViewBag.CodigoBarra = _connection.OrionContext.sp_Productos_Inventario_SeriesPorTecnico().Where(a => a.fiIDUsuario == GetIdUser()).ToList();
            ViewBag.IdTipoSolicitud = idTipoSolicitud;
            ViewBag.IdSolicitud = listadetalle.FiIDSolicitud;
            ViewBag.TieneFotos = false;
            //ViewBag.TieneFotos = listadetalle.flListadoFotosList.Any();
            //ViewBag.TieneFotos
            //var ListadoDeMaterialesDB = _connection.OrionContext.sp_ProductosExtras_Por_Solicitud(GetIdUser(), idsolicitud).ToList();
            //var ListadoMateriales = new List<Listado_materiales_viewModel>();

            //var empresa = _connection.OrionContext.sp_ObtenerEmpresa_ByIdUsuario(GetIdUser()).FirstOrDefault();
            //var tecnico = GetIdUser();

            //foreach (var item in ListadoDeMaterialesDB)
            //{
            //    ListadoMateriales.Add(new Listado_materiales_viewModel() { fiIDProducto = item.fiIDProducto, fcProducto = item.fcProducto.Replace("\\", "").Replace("\"", ""), fnCantidad = 1, fiIDMarca = item.fiIDMarca, fbCodigo = true });
            //}

            //var catalogoDocumentosInstalacion = _connection.OrionContext.sp_catalogo_FotosInstalacion_lista().ToList();

            //var listaFotosDB = _connection.OrionContext.sp_DocumentosInstalacion_SoporteTecnico(idSolicitudInstalacion).ToList();
            //List<lista_fotosInstalacion> listadoFotos = new List<lista_fotosInstalacion>();


            //detallesolicitudInstalacion_ViewModel model = new detallesolicitudInstalacion_ViewModel();
            //ViewBag.ListaFotos = listadoFotos;

            //model.fiIDSolicitudContratista = idSolicitudInstalacion;
            //var infoCliente = _connection.OrionContext.sp_InformacionInstalacion_Tecnico(idSolicitudInstalacion).FirstOrDefault();
            //model.fcArticulosAsignados = infoCliente.fcArticulosdelContrato;
            //model.fcCliente = infoCliente.fcNombre;

            //var listadoEstados = _connection.OrionContext.sp_ListaCatalogoInstalacion().ToList();
            //ViewBag.listadoEstados = listadoEstados.Select(x => new { Value = x.fiIDEstadoInstalacion, Text = x.fcDescripcion }).ToList();

            //var EstadoSolicitud = _connection.OrionContext.sp_ObtenerEstado_SolicitudInstalacion(idSolicitudInstalacion).FirstOrDefault();
            //model.fiIDEstadoInstalacion = EstadoSolicitud.fiIDEstadoInstalacion ?? 0;
            //model.fcDescripcionEstado = EstadoSolicitud.fcDescripcion;
            //model.fcClase = EstadoSolicitud.fcClase;
            //model.listadoMateriales = ListadoMateriales;


            return View(listadetalle);
        }

    }
}