using Newtonsoft.Json;
using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.App_Services.ContratoService;
using OrionCoreCableColor.App_Services.EmailService;
using OrionCoreCableColor.App_Services.ReportesService;
using OrionCoreCableColor.DbConnection.CoreFinancieroDB;
using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models.Base;
using OrionCoreCableColor.Models.DatosCliente;
using OrionCoreCableColor.Models.Manto;
using OrionCoreCableColor.Models.Reportes;
using OrionCoreCableColor.Models.Solicitudes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static OrionCoreCableColor.Models.Manto.CrearBitacoraTokenViewModel;

namespace OrionCoreCableColor.Controllers
{
    [Authorize]
    public class MantoController : BaseController
    {
        private readonly DbServiceConnection _connection;
        public SendEmailService ServicioCorreo;
        public static Dictionary<string, string> DictionaryList = new Dictionary<string, string>();
        public MantoController()
        {
            var ConnectionString = ConfigurationManager.ConnectionStrings["OrionConnections"].ConnectionString;
            _connection = new DbServiceConnection(ConnectionString);
            ServicioCorreo = new SendEmailService();
        }

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Solicitudes()
        {
            var RolesAdmin = GetConfiguracion<int>("Orion_Admin", ',');
            ViewBag.RolesAdmin = RolesAdmin.Contains(GetUser().IdRol);
            return View();
        }

        public ActionResult ViewInstalacionDocumentosContratista(int idsol, int idsolContra, int idclien)
        {
            ViewBag.IdSolicitud = idsol;
            ViewBag.IdSolicitudContratista = idsolContra;
            ViewBag.IdCliente = idclien;
            return View();
        }

        public JsonResult BuscarSolicitud(int IDSolicitud)
        {
            var listaDetalleCliente = new sp_OrionSolicitud_Detalle_ClienteListarViewModel();
            var listaEquifaxGarantia = new List<SolicitudesGarantiaViewModel>();

            try
            {
                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_OrionSolicitud_Detalle_ClienteListar {GetIdUser()} , {IDSolicitud}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        listaDetalleCliente = db.ObjectContext.Translate<sp_OrionSolicitud_Detalle_ClienteListarViewModel>(reader).FirstOrDefault();
                        reader.NextResult();
                        reader.NextResult();
                        listaEquifaxGarantia = db.ObjectContext.Translate<SolicitudesGarantiaViewModel>(reader).ToList();
                    }

                    var data = new
                    {
                        DetalleCliente = listaDetalleCliente,
                        DetalleProducto = listaEquifaxGarantia,
                    };

                    connection.Close();

                    return EnviarListaJson(data);
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public ActionResult ViewReiniciarGeoReferencia(int IdSolicitud, int idCliente)
        {
            ViewBag.IdSolicitud = IdSolicitud;
            ViewBag.IdCliente = idCliente;
            return PartialView();
        }

        public JsonResult CambiarGeoReferencia(int IdCliente, int GeoReinicio, string locaclizar)
        {
            try
            {
                var mensaje = _connection.OrionContext.sp_ReinicioGeoLocalizacion(IdCliente, GeoReinicio, locaclizar);
                return Json(mensaje);
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public JsonResult GuardarBitacorahistorial(int IdSolicitud, int IdCliente, string Observaciones, string Opcion)
        {
            try
            {                
                var mensaje = _connection.OrionContext.sp_BitacoraSolicitudHistorialMantenimiento(IdSolicitud, IdCliente, GetIdUser(), Observaciones,Opcion);
                return Json(mensaje);
            }
            catch (Exception e)
            {

                throw;
            }
        }


        [HttpGet]
        public JsonResult ObtenerCatalogosDocumentos(int IdContratistaSolicitud)
        {
            var catalogosDocumentos = new List<ListCatalogoFotosInstalacionViewModel>();

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = $"EXEC sp_Instalacion_documentosPorSolicitud_List @fiIDContratistaSolicitud = {IdContratistaSolicitud}";

                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    catalogosDocumentos = db.ObjectContext
                        .Translate<ListCatalogoFotosInstalacionViewModel>(reader)
                        .GroupBy(doc => new { doc.fiIDCatalogoFotoInstalacion, doc.fcNombreFoto })
                        .Select(grp => new ListCatalogoFotosInstalacionViewModel
                        {
                            fiIDCatalogoFotoInstalacion = grp.Key.fiIDCatalogoFotoInstalacion,
                            fcNombreFoto = grp.Key.fcNombreFoto
                        })
                        .ToList();
                }

                connection.Close();
            }

            return Json(catalogosDocumentos, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult ObtenerImagenesPorCatalogo(int IdCatalogo, int IdContratistaSolicitud)
        {
            var imagenesCatalogo = new List<ListCatalogoFotosInstalacionViewModel>();

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_Instalacion_documentosPorIdCatalogo_List {IdCatalogo} , {IdContratistaSolicitud}";

                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    imagenesCatalogo = db.ObjectContext
                        .Translate<ListCatalogoFotosInstalacionViewModel>(reader)
                        .ToList();
                }

                connection.Close();
            }

            return Json(imagenesCatalogo, JsonRequestBehavior.AllowGet);
        }



        public JsonResult DesactivarDocumentosInstalacion (int IdcontratistaSolicitud, string fcNombreDocumento)
        {
            try
            {
                var Mensaje = _connection.OrionContext.sp_Instalacion_EliminarDocumentos(IdcontratistaSolicitud, fcNombreDocumento);
                return Json(Mensaje);
            }
            catch (Exception e)
            {

                return EnviarException(e, "ERROR");
                throw;
            }
           
        }

        [HttpPost]
        public JsonResult GuardarFotoInstalacion(int fiIDSolicitud, int fiIDCatalogoFotoInstalacion, int fiIDContratistaSolicitud, string fcUrlDocumento, string fcNombreDocumento, string fcExtension, List<Base64ynombreViewModel> fotosbase)
        {
            try
            {
                if (fotosbase == null || fotosbase.Count == 0)
                {
                    return EnviarResultado(false, "Suba como mínimo una foto");
                }

                foreach (var item in fotosbase)
                {
                    var arch = ConvertirBase64AImagen(item.base64string, item.nombrearchivo);
                    // var documentoURL = @"\ImgProductos\imgInstalacion\";
                    var documentoURL = @"\Solicitudes\Solicitud_" + fiIDSolicitud + @"\SolicitudInstalacion_" + fiIDContratistaSolicitud;
                    var urlPdf = MemoryLoadManager.URL + documentoURL;
                    var ruta = documentoURL.Replace("*", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("|", "");
                    var pdfFile = Path.Combine(urlPdf, item.nombrearchivo);
                    var exists = System.IO.Directory.Exists(urlPdf);
                    using (var context = new ORIONDBEntities())
                    {
                        var connection = (SqlConnection)context.Database.Connection;
                        var command = new SqlCommand("sp_Instalacion_Solicitud_Contratista_Documentos_Insert", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@fiIDContratistaSolicitud", fiIDContratistaSolicitud);
                        command.Parameters.AddWithValue("@fiIDCatalogoFotoInstalacion", fiIDCatalogoFotoInstalacion);
                        command.Parameters.AddWithValue("@fcUrlDocumento", fcUrlDocumento);
                        command.Parameters.AddWithValue("@fcNombreDocumento", fcNombreDocumento);
                        command.Parameters.AddWithValue("@fcExtension", fcExtension);
                        var resultParam = new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        command.Parameters.Add(resultParam);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                        int ImagenGuardada = resultParam.Value != DBNull.Value ? Convert.ToInt32(resultParam.Value) : 0;
                        if (ImagenGuardada == -1)
                        {
                            return EnviarResultado(false, "Error al guardar la imagen en la base de datos.");
                        }
                    }
                    UploadFileServer148(@"\Solicitudes\Solicitud_" + fiIDSolicitud + @"\SolicitudInstalacion_" + fiIDContratistaSolicitud, arch);
                }

                return EnviarResultado(true, "Imágenes subidas exitosamente");
            }
            catch (Exception e)
            {
                return EnviarException(e, "ERROR");
            }
        }

        public JsonResult ListSolicitudesMantenimiento(int IdSolicitud)
        {
            var lista = EnviarListaJson(_connection.OrionContext.sp_BitacoraMantenimientoHistorialSolicitudLista(IdSolicitud).ToList());
            return lista;
        }

        public JsonResult ReiniciarContrato(int IdSolicitud)
        {
            var mensaje = _connection.OrionContext.sp_Reiniciodecontrato(IdSolicitud);

            return Json(mensaje);
        }

        public ActionResult ViewElimiardocumentos(int IdSolicitud, int idCliente)
        {
            ViewBag.IdSolicitud = IdSolicitud;
            ViewBag.IdCliente = idCliente;
            return PartialView();
        }

        public ActionResult ViewDocumentosSolicitud(int IdSolicitud, int idCliente)
        {
            return PartialView();
        }
        public JsonResult ListaDocumentosSolicitud(int fiIdSolicitud)
        {
            var DatosDocumentoListado = new List<DocumentosSolicitudViewModel>();
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_Documentos_Por_Solicitud {GetIdUser()} , {fiIdSolicitud}";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    DatosDocumentoListado = db.ObjectContext.Translate<DocumentosSolicitudViewModel>(reader).ToList();
                }
                connection.Close();
            }

            return EnviarListaJson(DatosDocumentoListado);
        }
       


        public ActionResult ViewEditarDatosCepheus(int IdSolicitud, int idCliente)
        {
            ViewBag.IdSolicitud = IdSolicitud;
            ViewBag.IdCliente = idCliente;
            var datosCliente = _connection.OrionContext.sp_OrionContratista_DetalleBySolicitud(1, IdSolicitud,0,0).FirstOrDefault();
            var clienteDetalleInstalacion = new SolicitudesViewModel
            {
                Nombre = datosCliente.fcNombre,
                IDSolicitud = IdSolicitud,
                CodigoCliente = datosCliente.fcCodigoCliente,
                NumeroOrdenCepheus = datosCliente.fcNumeroOrdenCfeus,
                NumeroOrdenTrabajoCepheus = datosCliente.fcNumeroOrdenTrabajoChepeus,
                IdCliente = idCliente
            };

            return PartialView(clienteDetalleInstalacion);
        }
        public ActionResult ViewEditarDatosVisitaInstalacion(int IdSolicitud, int idCliente)
        {
            var datosCliente = _connection.OrionContext.sp_OrionContratista_DetalleBySolicitud(1, IdSolicitud,0,0).FirstOrDefault();
            ViewBag.Listado1 = new List<SelectListItem>();
            ViewBag.List2 = new List<SelectListItem>();
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
            ViewBag.IdSolicitud = IdSolicitud;
            ViewBag.IdCliente = idCliente;
           
            var clienteDetalleInstalacion = new SolicitudesViewModel
            {
                Nombre = datosCliente.fcNombre,
                IDSolicitud = IdSolicitud,
                CodigoCliente = datosCliente.fcCodigoCliente,
                NumeroOrdenCepheus = datosCliente.fcNumeroOrdenCfeus,
                IdCliente = idCliente,
                fdFechaInstalacionAsignada = datosCliente.fdFechaInstalacionAsignada,
                fcComentarioOrdenIsntalacion = datosCliente.fcComentario,
                fiIDAgencia = datosCliente.fiIDAgenciaInstalacion,
                fcNombreAgencia = datosCliente.fcNombreAgencia,
                fiIDContratista = datosCliente.fiIDAgenciaContratista,
                fcNombreEmpresa = datosCliente.fcNombreEmpresa,
                fiIDEstadoInstalacion = datosCliente.fiIDEstadoInstalacion
            };

            return PartialView(clienteDetalleInstalacion);
        }
        public ActionResult ViewEditarDatosEstadoInstalacion(int IdSolicitud, int idCliente)
        {
            ViewBag.IdSolicitud = IdSolicitud;
            ViewBag.IdCliente = idCliente;
            ViewBag.ListaEstado = new List<SelectListItem>();
            var datosCliente = _connection.OrionContext.sp_OrionContratista_DetalleBySolicitud(1, IdSolicitud,0,0).FirstOrDefault();
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_OrionSolicitud_LlenarListas {GetIdUser()} ";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    reader.NextResult();
                    reader.NextResult();
                    reader.NextResult();
                    var ListaEstado = db.ObjectContext.Translate<SolicitudesViewModel>(reader).ToList();
                    ViewBag.ListaEstado = ListaEstado?.Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDEstadoInstalacion), Text = z.fcDescripcion })?.ToList() ?? new List<SelectListItem>();

                }

                connection.Close();

            }
           
            var clienteDetalleInstalacion = new SolicitudesViewModel
            {
                Nombre = datosCliente.fcNombre,
                IDSolicitud = IdSolicitud,
                CodigoCliente = datosCliente.fcCodigoCliente,
                NumeroOrdenCepheus = datosCliente.fcNumeroOrdenCfeus,
                IdCliente = idCliente,
                fdFechaInstalacionAsignada = datosCliente.fdFechaInstalacionAsignada,
                fcComentarioOrdenIsntalacion = datosCliente.fcComentario,
                fcDescripcion = datosCliente.fcDescripcionEstadoInstalacion,
                fiIDEstadoInstalacion = datosCliente.fiIDEstadoInstalacion,

            };

            return PartialView(clienteDetalleInstalacion);
        }
        public ActionResult ViewCambiarOfical(int IdSolicitud, int idCliente)
        {
            ViewBag.IdSolicitud = IdSolicitud;
            ViewBag.IdCliente = idCliente;
            ViewBag.ListaOficial = new List<SelectListItem>();
            var datosCliente = _connection.OrionContext.sp_OrionContratista_DetalleBySolicitud(1, IdSolicitud,0,0).FirstOrDefault();
            var datosClientes = _connection.OrionContext.sp_InfoClientePorSolicitud(IdSolicitud).FirstOrDefault();
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_OrionSolicitud_LlenarListas {GetIdUser()} ";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    reader.NextResult();
                    reader.NextResult();
                    reader.NextResult();
                    reader.NextResult();
                    var ListaOficial = db.ObjectContext.Translate<SolicitudesViewModel>(reader).ToList();
                    ViewBag.ListaOficial = ListaOficial?.Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDUsuario), Text = z.NombreOficial })?.ToList() ?? new List<SelectListItem>();

                }

                connection.Close();

            }

            var clienteDetalleInstalacion = new SolicitudesViewModel
            {
                Nombre = datosClientes.fcNombre,
                IDSolicitud = IdSolicitud,
                //CodigoCliente = datosCliente.fcCodigoCliente,
                //NumeroOrdenCepheus = datosCliente.fcNumeroOrdenCfeus,
                IdCliente = idCliente,
                //fdFechaInstalacionAsignada = datosCliente.fdFechaInstalacionAsignada,
                //fcComentarioOrdenIsntalacion = datosCliente.fcComentario,
                //fcDescripcion = datosCliente.fcDescripcionEstadoInstalacion,
               // fiIDEstadoInstalacion = datosCliente.fiIDEstadoInstalacion,
                fiIDUsuario = datosClientes?.fiIDUsuario?? 0,
                NombreOficial = datosClientes.fcNombreCorto

            };

            return PartialView(clienteDetalleInstalacion);
        }
        [HttpGet]
        public ActionResult ModalAgregarDatosCliente(string Nombre, string Identidad, int IDSolicitud, int idCliente , int fiPlazoSeleccionado)
        {
            ViewBag.ListaOficial = new List<SelectListItem>();
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_OrionSolicitud_LlenarListas {GetIdUser()} ";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    reader.NextResult();
                    reader.NextResult();
                    reader.NextResult();
                    reader.NextResult();
                    var ListaOficial = db.ObjectContext.Translate<SolicitudesViewModel>(reader).ToList();
                    ViewBag.ListaOficial = ListaOficial?.Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDUsuario), Text = z.NombreOficial })?.ToList() ?? new List<SelectListItem>();

                }

                connection.Close();

            }
            return PartialView(new SolicitudesViewModel { Nombre = Nombre, Identidad = Identidad, IDSolicitud = IDSolicitud, IdCliente = idCliente, fiIDUsuario = GetIdUser(), fiPlazoSeleccionado = fiPlazoSeleccionado });

        }
        public JsonResult DocumentosPorSolicitud(int IdSolicitud)
        {
            var DatosDocumentoListado = new List<sp_OrionSolicitud_DocumentoListado_ViewModel>();
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                ViewBag.IdSolicitud = IdSolicitud;
              
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_OrionSolicitud_Detalle_ClienteListar {GetIdUser()} , {IdSolicitud}";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    reader.NextResult();
                    reader.NextResult();
                    reader.NextResult();

                    DatosDocumentoListado = db.ObjectContext.Translate<sp_OrionSolicitud_DocumentoListado_ViewModel>(reader).ToList();
                    //ViewBag.IdSolicitudContratista = DatosDocumentoListado.ToList().FirstOrDefault().IdcontratistaSolicitud;
                }
                var lista = _connection.OrionContext.sp_Configuraciones().FirstOrDefault(a => a.NombreLlave == "DocumentosquenoVan").ValorLLave.Split(',').Select(a => Convert.ToInt32(a)).ToList();

                connection.Close();

                var documentos = _connection.OrionContext.sp_CatalogoSistemaDocumentos_Lista().Where(a => !lista.Any(b => b == a.fiIDDocumento)).ToList();

                var DocumentosEnLista = documentos.Select(a => new
                {
                    fiIDDocumento = a.fiIDDocumento,
                    fcDescripcion = a.fcDescripcion,
                    EstaenLista = DatosDocumentoListado.Any(b => b.fiIDTipoArchivo == a.fiIDDocumento)
                }).ToList();

                //return EnviarListaJson(DatosDocumentoListado.Where(a => !lista.Any(b => b == a.fiIDTipoArchivo)).ToList());
                return EnviarListaJson(DocumentosEnLista);
            }
        }
        
        

        public JsonResult DocumentosPorSolicitudInstalacion(int IdSolicitud)
        {
            var DatosDocumentoListado = new List<ListCatalogoFotosInstalacionViewModel>();

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = "EXEC sp_Instalacion_Catalogo_Fotos_List";

                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    DatosDocumentoListado = db.ObjectContext
                        .Translate<ListCatalogoFotosInstalacionViewModel>(reader)
                        .ToList();
                }

                connection.Close();
            }

            var DocumentosConEstado = DatosDocumentoListado.Select(doc => new
            {
                fiIDDocumento = doc.fiIDCatalogoFotoInstalacion,
                fcNombreFoto = doc.fcNombreFoto,
                EstaenLista = false
            }).ToList();

            return Json(DocumentosConEstado, JsonRequestBehavior.AllowGet);
        }

      

        public JsonResult DesactivarDocumentos(int IdSolicitud, int idDocumento)
        {
            var Mensaje = _connection.OrionContext.sp_EliminarDocumentos(IdSolicitud, idDocumento);
            return Json(Mensaje);
        }

        public JsonResult SubirPagare(int idSolicitud)
        {

            var comandList = $"sp_OrionSolicitudes_InformacionDocumentacion {idSolicitud}, 0, 1";
            var datosCliente = _connection.LoadListDataWithDbContext<Models.Reportes.sp_OrionSolicitudes_InformacionDocumentacion_ResultV2>(comandList).FirstOrDefault();

            var nombrePagare = $"Pagare_Solicitud{idSolicitud}_Identidad{datosCliente.fcIdentidad}.pdf";
            UploadFileServer148Async(@"Solicitudes\Solicitud_" + idSolicitud, GenerarPagare(idSolicitud, nombrePagare));
            return EnviarResultado(true, "", "Subido");
        }
        public HttpPostedFileBase GenerarPagare(int idSolicitud, string fileName)
        {
            var contratoService = new ContratoTemplateService();
            var pagare = new MemoryStream();
            var archivo = contratoService.GenerarPagare(idSolicitud, 0);
            archivo.CopyTo(pagare);
            return (HttpPostedFileBase)new MemoryPostedFile(pagare.ToArray(), fileName);
        }
        public ActionResult VistaReferencias(int id)
        {
            using (var conetion = new ORIONDBEntities())
            {

                ViewBag.ListaParentesco = conetion.sp_Catalogo_Parentescos_List().Select(x => new { id = x.fiIDParentesco.ToString(), text = x.fcDescripcionParentesco }).ToList();
                ViewBag.ListAceptoReferencia = _connection.OrionContext.sp_Configuraciones().FirstOrDefault(a => a.NombreLlave == "EstadoReferencias").ValorLLave.Split(',').Select(b => new { id = b.Split('-').ToList()[0], text = b.Split('-').ToList()[1] }).ToList();
            }
            //ViewBag.ListACeptarReferencia = 
            return PartialView(id);
        }
        [HttpGet]
        public ActionResult ConfrimacionNumero(string Nombre, int IDCliente, int IDSolicitud)
        {
            using (var conetion = new ORIONDBEntities())
            {
                //var datos = conetion.sp_OrionSolicitudes_InformacionDocumentacion(0, IDCliente, 1).FirstOrDefault();
                var infoDocumentos = new sp_OrionSolicitud_InformacionDocumentacion_ViewModel();
                var listaDetalleCliente = new sp_OrionSolicitud_Detalle_ClienteListarViewModel();
                var listaEquifaxGarantia = new List<SolicitudesGarantiaViewModel>();

                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_OrionSolicitudes_InformacionDocumentacion {0} ,{IDCliente},{1}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        infoDocumentos = db.ObjectContext.Translate<sp_OrionSolicitud_InformacionDocumentacion_ViewModel>(reader).FirstOrDefault();
                    }

                    connection.Close();
                }

                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_OrionSolicitud_Detalle_ClienteListar {GetIdUser()} , {IDSolicitud}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        listaDetalleCliente = db.ObjectContext.Translate<sp_OrionSolicitud_Detalle_ClienteListarViewModel>(reader).FirstOrDefault();
                        reader.NextResult();
                        //listaReferencias = db.ObjectContext.Translate<SolicitudesReferenciaViewModel>(reader).ToList();
                        reader.NextResult();
                        listaEquifaxGarantia = db.ObjectContext.Translate<SolicitudesGarantiaViewModel>(reader).ToList();
                        //reader.NextResult();
                        //DatosDocumentoListado = db.ObjectContext.Translate<sp_OrionSolicitud_DocumentoListado_ViewModel>(reader).ToList();

                    }
                    DictionaryList.Clear();
                    DictionaryList.Add("fcNombreCliente", "prueba");
                    connection.Close();
                }

                ViewBag.ListaParentesco = conetion.sp_Catalogo_Parentescos_List().Select(x => new { id = x.fiIDParentesco.ToString(), text = x.fcDescripcionParentesco }).ToList();
                return PartialView(new SolicitudesViewModel { Nombre = Nombre, IdCliente = IDCliente, DatosContrato = infoDocumentos, DatosCLientes = listaDetalleCliente, ListaGarantia = listaEquifaxGarantia });
            }

        }
        [HttpGet]
        public ActionResult ConfirmacionToken(string Nombre, int IDCliente, int IDSolicitud , string codigoToken , string Identidad, decimal ValorMonedaNacional)
        {
            using (var conetion = new ORIONDBEntities())
            {   
                return PartialView(new SolicitudesViewModel { Nombre = Nombre, IdCliente = IDCliente, IDSolicitud = IDSolicitud , CodigoSeccionToken = codigoToken, Identidad = Identidad, fnCuotaMensualMonedaNacional = ValorMonedaNacional });
            }

        }

        [HttpGet]
        public ActionResult ViewReinicioSolicitud(string Nombre, int IDSolicitud)
        {
            using (var conetion = new ORIONDBEntities())
            {
                //string texto = UEncoder.encode(Nombre, StandardCharsets.UTF_8.toString()); ;

                return PartialView(new SolicitudesViewModel { Nombre = Nombre, IDSolicitud = IDSolicitud });
            }

        }

        [HttpPost]
        public JsonResult ReiniciarSolicitud(int fiIDSolicitud, int Moneda)
        {
            try
            {
                using (var context = new ORIONDBEntities())
                {
                    var result = context.sp_SolicitudOrion_ReiniciarSolicitudDeclinada(fiIDSolicitud, Moneda);
                    var mensaje = context.sp_BitacoraSolicitudHistorialMantenimiento(fiIDSolicitud, fiIDSolicitud, GetIdUser(), "Se reinicia la Solicitud "+ fiIDSolicitud, "Reinicio de Solicitud");
                    return EnviarResultado(true, "Datos", "Datos Actualizados");
                }

                return EnviarResultado(true, "Se Reinicio la Solicitud");
            }
            catch (Exception e)
            {
                return EnviarException(e, "ERROR");
            }
        }




        [HttpPost]
        public JsonResult InsertarBitacoraToken(int fiIDSolicitud, string fcTokenAplicado, string fcCodigoToken)
        {
            string mensajeSalida = "";

            using (var context = new ORIONDBEntities())
            {
                var connection = (SqlConnection)context.Database.Connection;
                using (var command = new SqlCommand("sp_Solicitudes_Token_Maestro_Bitacora_Insertar", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@fiIDSolicitud", fiIDSolicitud);
                    command.Parameters.AddWithValue("@fiIDUsuario", GetIdUser());
                    command.Parameters.AddWithValue("@fcTokenAplicado", fcTokenAplicado);
                    command.Parameters.AddWithValue("@fcCodigoToken", fcCodigoToken);

                    var salida = new SqlParameter("@MensajeSalida", SqlDbType.NVarChar, 500)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(salida);

                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    command.ExecuteNonQuery();
                    mensajeSalida = salida.Value?.ToString();
                }
            }

            return Json(new { Estado = true, Mensaje = mensajeSalida }, JsonRequestBehavior.AllowGet);
        }




        [HttpGet]
        public JsonResult VerificarToken(string Token, string codigo)
        {
            var datos = new SolicitudesViewModel();
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                var ip = "0";
                //var codigo = "N001";
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_Token_Aplicar {GetIdUser()}, {ip}, {Token} , {codigo}";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    datos = db.ObjectContext.Translate<SolicitudesViewModel>(reader).FirstOrDefault();
                }
                connection.Close();
                //return EnviarResultado(datos);
                return EnviarResultado(true, "", datos.fcMensaje);
            }
        }

        public List<sp_OperacionesBanco_CuentasdeBanco_Result> ListadoCuentas()
        {
            using (var conetion = new CoreFinancieroEntities2())
            {
                var listaCuentaContable = conetion.sp_OperacionesBanco_CuentasdeBanco(1, 117, 1, 1).ToList();
                return listaCuentaContable;
            }
        }

        [HttpGet]
        public ActionResult ModalEnviarComprobantePago(string Nombre, string Identidad, int IDSolicitud, int idCliente, decimal ValorMonedaNacional)
        {
            ViewBag.ListaCuentaContable = ListadoCuentas().Select(z => new SelectListItem { Value = (z.fcCuentaContable), Text = z.fcDescripcionCuenta.ToString() }).ToList();
            using (var conetion = new ORIONDBEntities())
            {
                return PartialView(new SolicitudesViewModel { Nombre = Nombre, Identidad = Identidad, IDSolicitud = IDSolicitud, IdCliente = idCliente, fnCuotaMensualMonedaNacional = ValorMonedaNacional });
            }

        }
        public ActionResult ViewDetalleInstalacion(int IDSolicitud, int idEquifaxCliente)
        {
            using (var contexto = new ORIONDBEntities())
            {
                
                var solicitudInstalacion = contexto.sp_ContratistaSolicitudInstalacion_ObtenerPorSolicitud(IDSolicitud).ToList();
                if(solicitudInstalacion.Count() == 0)
                {
                    var cliente = contexto.sp_OrionSolicitud_Detalle_ClienteListar(1, IDSolicitud).FirstOrDefault();
                    contexto.sp_OrionSolicitud_ContratistaSolicitudInstalacio__Insertar(GetIdUser(), idEquifaxCliente, IDSolicitud, 1, 1, "", "", "", 1,"",null,0);
                }

                var model = contexto.sp_Contratista_SolicitudInstalacion_ListarProductosInstalados(IDSolicitud).Select((x, i) => new DetalleInstalacionViewModel
                {
                    index = i,
                    fiIDContratista_SolicitudInstalacion_Detalle = x.fiIDContratista_SolicitudInstalacion_Detalle,
                    fiIDContratista_SolicitudInstalacion = x.fiIDContratista_SolicitudInstalacion ?? solicitudInstalacion.FirstOrDefault().fiIDContratistaSolicitud,
                    fiIDProducto = x.fiIDProducto ?? 0,
                    fcProducto = x.fcProducto,
                    fiIDInventario = x.fiIDInventario ?? 0,
                    fcSerie = x.fcSerie,
                    fcMac = x.fcMac,
                    fnCantidad = x.fnCantidad ?? 0,
                    fbNuevo = x.fiIDContratista_SolicitudInstalacion_Detalle == null ? true : false
                }).ToList();
                ViewBag.fiIDContratistaSolicitud = solicitudInstalacion.FirstOrDefault().fiIDContratistaSolicitud;
                ViewBag.ListarProductos = GetListProductos().Select(x => new { id = x.Value, text = x.Text });
                return PartialView(model);
            }

        }
        [HttpPost]
        public JsonResult ActualizarProductosInstalacion(List<DetalleInstalacionViewModel> model, int IDSolicitud)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {

                    var modelDbDetalle = contexto.sp_Contratista_SolicitudInstalacion_ListarProductosInstalados(IDSolicitud).ToList();




                    foreach (var item in modelDbDetalle)
                    {
                        if (model.Any(x => x.fiIDContratista_SolicitudInstalacion_Detalle == item.fiIDContratista_SolicitudInstalacion_Detalle))
                        {
                            var itemModel = model.FirstOrDefault(x => x.fiIDContratista_SolicitudInstalacion_Detalle == item.fiIDContratista_SolicitudInstalacion_Detalle);

                            contexto.sp_Contratista_SolicitudInstalacion_Detalle_Editar(
                                itemModel.fiIDContratista_SolicitudInstalacion_Detalle,
                                itemModel.fiIDContratista_SolicitudInstalacion,
                                itemModel.fiIDProducto,
                                itemModel.fiIDInventario,
                                itemModel.fnCantidad,
                                itemModel.fcSerie,
                                itemModel.fcMac
                            );
                        }
                        else
                        {

                            contexto.sp_Contratista_SolicitudInstalacion_Detalle_Eliminar(item.fiIDContratista_SolicitudInstalacion_Detalle);
                        }
                    }



                    foreach (var item in model.Where(x => x.fbNuevo == true).ToList())
                    {
                        contexto.sp_Contratista_SolicitudInstalacion_Detalle_Crear(
                            item.fiIDContratista_SolicitudInstalacion,
                            item.fiIDProducto,
                            item.fiIDInventario,
                            item.fnCantidad,
                            item.fcSerie,
                            item.fcMac);
                    }



                    return EnviarResultado(true, "", $"INSTALACION MODIFICADA CON EXITO");

                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }


        [HttpPost]
        public JsonResult GuardarFinanciamiento(decimal cuotamensual, int plazo, int solicitud, int idequifax, string correo, string telefono)
        {
            try
            {
                var datosCliente = _connection.OrionContext.sp_OrionSolicitudes_InformacionDocumentacion(solicitud, idequifax, 1).FirstOrDefault();
                var guardarFinanciamiento = _connection.OrionContext.sp_MantenimientoSolicitud_RecalculoCuota(cuotamensual, plazo, solicitud, idequifax, correo, telefono).FirstOrDefault();

                if (guardarFinanciamiento != null && guardarFinanciamiento.fiMensaje == "1")
                {
                    CampoAfectadoViewModel Campo(string nombre, object antes, object despues) =>
                        new CampoAfectadoViewModel
                        {
                            fcNombreCampo = nombre,
                            fcValorAntes = antes?.ToString(),
                            fcValorDespues = despues?.ToString()
                        };

                    var camposAfectados = new List<CampoAfectadoViewModel>
                    {
                        Campo("fnCuotaMensual", datosCliente.fnCuotaMensual, cuotamensual.ToString("F2")),
                        Campo("fiPlazoSeleccionado", datosCliente.fiPlazoSeleccionado, plazo)
                    };

                    string jsonCampos = JsonConvert.SerializeObject(camposAfectados);
                    using (var context = new ORIONDBEntities())
                    {
                        var connection = (SqlConnection)context.Database.Connection;
                        using (var command = new SqlCommand("sp_Solicitudes_Token_Maestro_CamposAfectados_Insertar", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@fiIDSolicitud", solicitud);
                            command.Parameters.AddWithValue("@jsonCamposAfectados", jsonCampos);

                            var mensajeSalida = new SqlParameter("@MensajeSalida", SqlDbType.NVarChar, 500)
                            {
                                Direction = ParameterDirection.Output
                            };
                            command.Parameters.Add(mensajeSalida);

                            if (connection.State != ConnectionState.Open)
                                connection.Open();

                            command.ExecuteNonQuery();

                            string resultadoBitacora = mensajeSalida.Value?.ToString();
                           
                        }
                        if (context.Database.Connection.State == ConnectionState.Open)
                            context.Database.Connection.Close();
                    }

                    return EnviarResultado(true, "Guardado correctamente", "Guardada exitosamente");
                }
                else
                {
                    return EnviarResultado(false, "Error", "Ocurrió un error al guardar el financiamiento");
                }
            }
            catch (Exception ex)
            {
                return EnviarResultado(false, "Excepción", "Error inesperado: " + ex.Message);
            }
        }




        //public JsonResult GuardarFinanciamiento(decimal cuotamensual, int plazo, int solicitud, int idequifax,string correo, string telefono) 
        //{
        //    try
        //    {
        //        var guardarFinanciamiento = _connection.OrionContext.sp_MantenimientoSolicitud_RecalculoCuota(cuotamensual, plazo, solicitud, idequifax, correo, telefono).FirstOrDefault();

        //       //guardar campos antes  y despues 
        //        //  var guradarCamposAfectados = _connection.OrionContext.sp_Solicitudes_Token_Maestro_Bitacora_Insertar();
        //        if (guardarFinanciamiento.fiMensaje == "1" )
        //        {
        //            return EnviarResultado(true, "", "Guardada Exitosamente");
        //        }
        //        else
        //        {
        //            return EnviarResultado(true, "", "Ocurrio un Error al querer ");
        //        }
        //    }
        //    catch (Exception e )
        //    {

        //        throw;
        //    }
        //}


        [HttpPost]
        public JsonResult ActualizarDatosInstalacionCepheus(int IDSolicitud , string NumeroCepheus , string CodigoCliente, string NumeroTrabajoCepheus)
        {
            using (var context = new ORIONDBEntities())
            {
                    try
                    {
                        var result = context.sp_DatosInstalacionCepheus_Editar(IDSolicitud,NumeroCepheus,CodigoCliente,GetIdUser(), NumeroTrabajoCepheus) > 0;

                        return EnviarResultado(true, "Datos", "Datos Actualizados");

                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }


            }
        }
        [HttpPost]
        public JsonResult ActualizarDatosInstalacionReprogramarVisita(int IDSolicitud, DateTime FechaProgramadaInstalacion, string ComentarioInstalacion, int idAgencia, int idAgenciaContratista)
        {
            using (var context = new ORIONDBEntities())
            {
                try
                {
                    var result = context.sp_DatosInstalacion_ReprogramarDiaInstalacion(IDSolicitud , FechaProgramadaInstalacion, ComentarioInstalacion, GetIdUser(), idAgencia, idAgenciaContratista) > 0;

                    return EnviarResultado(true, "Datos", "Datos Actualizados");

                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }

            }
        }
        [HttpPost]
        public JsonResult ActualizarDatosInstalacionEstado(int IDSolicitud,  int IDEstadoInstalacion, int TipoSolicitud)
        {
            using (var context = new ORIONDBEntities())
            {
                try
                {
                  var result = context.sp_DatosInstalacion_CambiarEstado(IDSolicitud, IDEstadoInstalacion, TipoSolicitud, GetIdUser()) > 0;

                    return EnviarResultado(true, "Datos", "Datos Actualizados");

                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }

        [HttpPost]
        public JsonResult ActualizarDatosCambioOficial(int IDSolicitud, int IDUsuarioOficial)
        {
            using (var context = new ORIONDBEntities())
            {
                try
                {
                    var result = context.sp_DatosSolicitud_CambiarOficial(IDSolicitud, IDUsuarioOficial,GetIdUser()) > 0;

                    return EnviarResultado(true, "Datos", "Datos Actualizados");

                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }


            }
        }
        [HttpPost]
        public async Task<JsonResult> RegistrarPago(SolicitudesViewModel model)
        {
            using (var context = new ORIONDBEntities())
            {
                try
                {
                    var datosCliente = context.sp_OrionContratista_DetalleBySolicitud(GetIdUser(), model.IDSolicitud,0,0).FirstOrDefault();

                    var documentoURL = @"\Solicitudes\Solicitud_" + model.IDSolicitud;
                    var fcUrl = documentoURL + @"\" + model.DatosDocumento.ImgenProducto.FileName;
                    var url = MemoryLoadManager.URL + documentoURL;
                    var ruta = documentoURL + @"\" + model.DatosDocumento.ImgenProducto.FileName;
                    ruta = ruta.Replace("*", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("|", "");
                    var pdfFile = url + @"\" + model.DatosDocumento.ImgenProducto.FileName;
                    var fcNombreArchivo = model.DatosDocumento.ImgenProducto.FileName.Replace(Path.GetExtension(model.DatosDocumento.ImgenProducto.FileName), "");

                    var result = context.sp_Crear_DocumentosOrion(GetIdUser(), model.IDSolicitud, fcNombreArchivo, Path.GetExtension(model.DatosDocumento.ImgenProducto.FileName), url, fcUrl, 4, model.fcComentarioPagoInicial, 1);
                    context.sp_OrionSolicitud_CabiarEstadoPrimerPago(GetIdUser(), model.IDSolicitud, model.fcComentarioPagoInicial, model.CuotaEnLempiras , model.fcCuentaContable);
                    UploadFileServer148(@"Solicitudes\Solicitud_" + model.IDSolicitud, model.DatosDocumento.ImgenProducto);
                    await EnviarCorreo(model.IDSolicitud);
                    return EnviarResultado(true, "Solicitud", "Mensaje Enviado");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }

            }
        }
        public async Task<JsonResult> EnviarCorreo(int IDSolicitud)
        {

            using (var context = new ORIONDBEntities())
            {
                try
                {
                    var datosClientebyContratista = context.sp_OrionContratista_DetalleBySolicitud(GetIdUser(), IDSolicitud,0,0).FirstOrDefault();
                    var comandList = $"sp_OrionSolicitudes_InformacionDocumentacion {IDSolicitud}, 0, 1";
                    var datosClienteInformacionDocumentacion = _connection.LoadListDataWithDbContext<Models.Reportes.sp_OrionSolicitudes_InformacionDocumentacion_ResultV2>(comandList).FirstOrDefault();
                    //var datosClienteInformacionDocumentacion = context.sp_OrionSolicitudes_InformacionDocumentacion(IDSolicitud, 0, GetIdUser()).FirstOrDefault();
                    var datosClienteDetalle = context.sp_OrionSolicitud_Detalle_ClienteListar(GetIdUser(), IDSolicitud).FirstOrDefault();
                    var _emailTemplateService = new EmailTemplateService();


                    var Correo = "angel.bautista@miprestadito.com";
                    var modelCorreo = new SendEmailViewModel();
                    var CuerpoComentarioCorreo = "Se acaba de subir el comprobante de pago del cliente : " + datosClienteInformacionDocumentacion.fcNombre + " , por lo cual se solicita la generacion del contrato manual(Cepheus)," +
                        "<br/>" +
                        "Identidad: " + " " + datosClienteInformacionDocumentacion.fcIdentidad +
                        "<br/>" +
                        "direccion de cliente:" + datosClienteDetalle.fcDepartamento + " " + datosClienteDetalle.fcMunicipio + " " + datosClienteDetalle.fcBarrio + " " + datosClienteInformacionDocumentacion.fcDireccionDetallada +
                        "<br/>" + "Correo: " + datosClienteDetalle.fcCorreo + ", " +
                        "<br/>" +
                        "Telefono: " + datosClienteInformacionDocumentacion.fcTelefono +
                        "<br/>" +
                        "Paquete: " + datosClienteInformacionDocumentacion.fcArticulosdelContrato +
                       "<br/>" + "<br/>" + " Saludos Cordiales.!!:";
                    //modelCorreo.DestinationEmail = datosCliente.fcCorreoElectronico;
                    modelCorreo.DestinationEmail = Correo;
                    modelCorreo.Subject = "Generacion de contrato de Cepfeus";
                    modelCorreo.Body = CuerpoComentarioCorreo;
                    modelCorreo.EmailName = "Novanet";
                    // modelCorreo.List_CC.Add("eduardo.perez@miprestadito.com");
                    modelCorreo.List_CC.Add("Contacto@novanetgroup.com");
                    // modelCorreo.List_CC.Add("Jordi.Montanola@cablecolor.net");

                    await ServicioCorreo.SendEmailAsync(modelCorreo);
                    return EnviarResultado(true, "Solicitud", "Correo Enviado");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }

            }
        }
        [HttpGet]
        public HttpPostedFileBase MostrarDocumentoInstalacion(int fiIDSolicitud)
        {
            var reporteService = new ReportesTemplateService();
            var instalacion = new MemoryStream();
            var archivo = reporteService.GenerarReporteInstalacionCliente(fiIDSolicitud);
            archivo.CopyTo(instalacion);
            return (HttpPostedFileBase)new MemoryPostedFile(instalacion.ToArray(), "InstalacionDeSolicitud_" + fiIDSolicitud);
        }
        [HttpGet]
        public void GenerarDocumentoInstalacion(int fiIDSolicitud)
        {
            var reporteService = new ReportesTemplateService();
            var instalacion = new MemoryStream();
            var archivo = reporteService.GenerarReporteInstalacionCliente(fiIDSolicitud);
            archivo.CopyTo(instalacion);
            TempData["ReportePDF"] = instalacion;
        }
        [HttpGet]
        public ActionResult ViewDetalleGarantia(int IDSolicitud , int idEquifaxCliente)
        {
            ViewBag.ListarProductos = GetListProductos().Select(x => new { id = x.Value, text = x.Text });
            ViewBag.IDSolicitud = IDSolicitud;
            ViewBag.idEquifaxCliente = idEquifaxCliente;
            return PartialView();
        }


        [HttpGet]
        public JsonResult ListarProductosGarantia(int IDSolicitud, int idEquifaxCliente)
        {
            using(var contexto = new ORIONDBEntities())
            {
                var lista = contexto.sp_Equifax_Garantia_Listar(idEquifaxCliente, IDSolicitud).Select(x => new ListarEquifaxGarantiaViewModel { fbNuevo = false, fcToken = x.fcToken, fiIDProducto = x.fiIDProducto, fiIDEquifax = x.fiIDEquifax, fiIDSolicitud = x.fiIDSolicitud ?? 0 }).ToList();
                return EnviarListaJson(lista);
            }
        }



        [HttpPost]
        public JsonResult ActualizarProductosGarantia(List<ListarEquifaxGarantiaViewModel> model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var fiIDSolicitud = model.FirstOrDefault().fiIDSolicitud;
                    var fiIDEquifax = model.FirstOrDefault().fiIDEquifax;
                    var modelDbDetalle = contexto.sp_Equifax_Garantia_Listar(fiIDEquifax, fiIDSolicitud).ToList();
                    var productosAntes = string.Join(",", modelDbDetalle.Select(x => x.fcProducto.Trim()));

                    foreach (var item in modelDbDetalle)
                    {
                        if (model.Any(x => x.fcToken == item.fcToken))
                        {
                            var itemModel = model.FirstOrDefault(x => x.fcToken == item.fcToken);

                            contexto.sp_Equifax_Garantia_MantoEditar(
                                itemModel.fiIDSolicitud,
                                itemModel.fiIDEquifax,
                                itemModel.fcToken,
                                itemModel.fiIDProducto,
                                GetIdUser()
                            );
                        }
                        else
                        {
                            contexto.sp_Equifax_Garantia_MantoEliminar(item.fiIDSolicitud, item.fiIDEquifax, item.fcToken);
                        }
                    }

                    foreach (var item in model.Where(x => x.fbNuevo == true).ToList())
                    {
                        contexto.sp_Equifax_Garantia_MantoInsertar(
                            item.fiIDSolicitud,
                            item.fiIDEquifax,
                            item.fiIDProducto,
                            GetIdUser());
                    }
                    var productosDespuesList = contexto.sp_Equifax_Garantia_Listar(fiIDEquifax, fiIDSolicitud) .Select(x => x.fcProducto.Trim()).ToList();
                    var productosAntesList = productosAntes.Split(',').Select(p => p.Trim()).ToList();
                    var productosMarcados = productosDespuesList.Select(p => productosAntesList.Contains(p) ? p : p).ToList();
                    var fcValorAntes = string.Join(",", productosAntesList);
                    var fcValorDespues = string.Join(",", productosMarcados);

                    string jsonCampos = JsonConvert.SerializeObject(new List<CampoAfectadoViewModel>
                    {
                        new CampoAfectadoViewModel
                        {
                            fcNombreCampo = "fcProducto",
                            fcValorAntes = fcValorAntes,
                            fcValorDespues = fcValorDespues
                        }
                    });

                    var connection = (SqlConnection)contexto.Database.Connection;
                    using (var command = new SqlCommand("sp_Solicitudes_Token_Maestro_CamposAfectados_Insertar", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@fiIDSolicitud", fiIDSolicitud);
                        command.Parameters.AddWithValue("@jsonCamposAfectados", jsonCampos);

                        var mensajeSalida = new SqlParameter("@MensajeSalida", SqlDbType.NVarChar, 500)
                        {
                            Direction = ParameterDirection.Output
                        };

                        command.Parameters.Add(mensajeSalida);

                        if (connection.State != ConnectionState.Open)
                            connection.Open();

                        command.ExecuteNonQuery();
                        Console.WriteLine("Bitácora: " + mensajeSalida.Value?.ToString());
                    }

                    return EnviarResultado(true, "", "GARANTIA MODIFICADA CON EXITO");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }



        public ActionResult BandejaMantenimientoCliente(int idsolicitud)
        {
            try
            {
                using(var contexto = new ORIONDBEntities())
                {
                    
                    var infocliente = contexto.sp_InfoClientePorSolicitud(idsolicitud).FirstOrDefault();
                    var informacli = new datosclienteViewModel();
                    informacli.fcNombreCompleto = infocliente.fcNombre;
                    informacli.fiIDEquifax = (int)infocliente.fiIDEquifax;
                    informacli.fcIdentidad = infocliente.fcIdentidad;
                    informacli.fdFechaNacimiento = (DateTime)infocliente.fdFechaNacimiento;
                    informacli.fcTelefono = infocliente.fcTelefono;
                    informacli.fcCorreo = infocliente.fcCorreo;
                    informacli.fcLugarTrabajo = infocliente.fcLugarTrabajo;
                    informacli.fcDireccionDetallada = infocliente.fcDireccionDetallada;

                    informacli.fiCodDepartamento = (int)infocliente.fiCodDepartamento;
                    informacli.fiCodMunicipio = (int)infocliente.fiCodMunicipio;
                    informacli.fiCodColonia = (int)infocliente.fiCodColonia;

                    return View(informacli);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult InfoCliente(int idssolicitud)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var datosinfo = contexto.sp_InfoClientePorSolicitud(idssolicitud).FirstOrDefault();
                    var info = new datosclienteViewModel();
                    info.fiIDEquifax = (int)datosinfo.fiIDEquifax;
                    ViewBag.Idsolicitud = datosinfo.fiIDSolicitud;
                    info.fcNombreCompleto = datosinfo.fcNombre;//
                    info.fcIdentidad = datosinfo.fcIdentidad;//
                    info.fdFechaNacimiento = (DateTime)datosinfo.fdFechaNacimiento;//
                    info.fcTelefono = datosinfo.fcTelefono;//
                    info.fcCorreo = datosinfo.fcCorreo;//
                    info.fcLugarTrabajo = datosinfo.fcLugarTrabajo;//
                    info.fcDireccionDetallada = datosinfo.fcDireccionDetallada;//
                    info.fiCodDepartamento = (int)datosinfo.fiCodDepartamento;
                    info.fiCodMunicipio = (int)datosinfo.fiCodMunicipio;
                    info.fiCodColonia = (int)datosinfo.fiCodColonia;

                    ViewBag.ficodigodepartamento = datosinfo.fiCodDepartamento ?? 0 ;
                    ViewBag.fiCodMunicipio = datosinfo.fiCodMunicipio ?? 0;
                    ViewBag.fiCodColonia = datosinfo.fiCodColonia ?? 0;

                    return PartialView(info);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        [HttpPost]
        public JsonResult ActualizarDatosClientes(int fiIDEquifax, string fcNombreCompleto,string fcIdentidad,DateTime fdFechaNacimiento, string fcCorreo, string fcTelefono, string fcLugarTrabajo, string fcDireccionDetallada, int fiDepartamento,int fiCodMunicipio, int fiColonia ,int idsolicidut)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var info = contexto.sp_ActualizarInfoClientePorSolicitud(idsolicidut, fiIDEquifax, fcNombreCompleto, fcIdentidad, fdFechaNacimiento, fcTelefono, fcCorreo, fcLugarTrabajo, fcDireccionDetallada, fiDepartamento, fiCodMunicipio, fiColonia).FirstOrDefault();
                    return Json(info); 
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }


        [HttpPost]
        public ActionResult ViewMantoEditarPaquete(EditPaqueteViewModel model)
        {
            using(var contexto = new ORIONDBEntities())
            {
                //ViewBag.ListarPaquetes = contexto.sp_Paquetes_List().ToList().Select(x => new SelectListItem { Value = x.fiIDPaquete.ToString(), Text = $"{x.fcPaquete}, {(x.fiIDMoneda == 1 ? "L " : "$ ")} {x.fnValorMensual}", Disabled = x.fiestadoPaquete == 1 }).ToList();
                ViewBag.ListarPaquetes = contexto.sp_Paquetes_List().ToList().Select(x => new { id = x.fiIDPaquete, text = $"{x.fcPaquete}, {(x.fiIDMoneda == 1 ? "L " : "$ ")} {x.fnValorMensual}", disabled = !(x.fiestadoPaquete == 1) }).ToList();
                ViewBag.ListarProductos = GetListProductos().Select(x => new { id = x.Value, text = x.Text });
                return PartialView(model);
            }
        }


        [HttpGet]
        public JsonResult ListProductosInventario(int fiIDPaquete, int fiIDSolicitud)
        {
            using (var contexto = new ORIONDBEntities())
            {

                var resultado = contexto.sp_Paquetes_ListarProductosGarantia(fiIDPaquete, fiIDSolicitud).ToList();
                return EnviarListaJson(resultado);

            }
        }


        [HttpGet]
        public JsonResult ActualizarPaqueteSolicitud(int fiIDSolicitud, int fiIDPaquete)
        {
            using(var contexto = new ORIONDBEntities())
            {
                try {
                    var result = contexto.sp_Equifax_Garantia_CambiarPaquete(fiIDSolicitud, fiIDPaquete, GetIdUser()).FirstOrDefault();
                    if (result.fiMensaje == "1")
                    {
                        return EnviarResultado(true, "", "Paquete modificado exitosamente");
                    }
                    else
                    {
                        return EnviarResultado(false, "Error", "Error al cambiar el paquete");
                    }
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
                
            }
        }


        [HttpGet]
        public ActionResult GetDeshabilitar_Botones_Manto(int fiIDSolicitud)
        {
            try
            {
                using (var context = new ORIONDBEntities())
                {
                    var connection = (SqlConnection)context.Database.Connection;
                    var command = new SqlCommand("Orion.dbo.sp_Solicitudes_Manto_DeshabilitarBotones", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@fiIDSolicitud", fiIDSolicitud);
                    command.Parameters.AddWithValue("@fiIDUsuario", GetIdUser());

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var resultado = new
                            {
                                fiIDSolicitud = reader["fiIDSolicitud"],
                                fiIDEstadoInstalacion = reader["fiIDEstadoInstalacion"],
                                fbInstalada = Convert.ToBoolean(reader["fbInstalada"]),
                                fbEsAdmin = Convert.ToBoolean(reader["fbEsAdmin"])
                            };

                            connection.Close();
                            return Json(resultado, JsonRequestBehavior.AllowGet);
                        }
                    }

                    connection.Close();
                }
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return EnviarException(e, "Error");
            }
        }


        [HttpGet]
        public ActionResult ConfirmacionTokenHabilitarBtns(int IDSolicitud, string codigoToken)
        {
            using (var conetion = new ORIONDBEntities())
            {
                return PartialView(new SolicitudesViewModel { IDSolicitud = IDSolicitud, CodigoSeccionToken = codigoToken });
            }

        }


    }
}