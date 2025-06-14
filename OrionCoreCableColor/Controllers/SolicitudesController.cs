using Microsoft.AspNet.SignalR;
using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.App_Services.EmailService;
using OrionCoreCableColor.DbConnection.CMContext;
using OrionCoreCableColor.DbConnection.CoreFinancieroDB;
using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models.DatosCliente;
using OrionCoreCableColor.Models.EmailTemplateService;
using OrionCoreCableColor.Models.Productos;
using OrionCoreCableColor.Models.Solicitudes;
using OrionCoreCableColor.Models.Tecnico;
using Spire.Doc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OrionCoreCableColor.Controllers
{
    [System.Web.Mvc.Authorize]
    public class SolicitudesController : BaseController
    {
        public DbServiceConnection _connection;
        public SendEmailService ServicioCorreo;
        public static Dictionary<string, string> DictionaryList = new Dictionary<string, string>();
        // GET: Solicitudes
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Bandeja_Solicitudes_Productos()
        {
            var RolesAdmin = GetConfiguracion<int>("Orion_Admin", ',');
            ViewBag.RolesAdmin = RolesAdmin.Contains(GetUser().IdRol);
            return View();
        }


        public ActionResult ModalSolicitudesProductos()
        {
            using (var conetion = new ORIONDBEntities())
            {
                //ViewBag.NombreCliente = conetion.sp_OrionSolicitud_ServiciosPorClientes().Select(x => new { id = x.fiIDCliente.ToString(), text = x.fcNombre }).ToList();
                ViewBag.NombreCliente = conetion.sp_OrionSolicitud_ServiciosPorClientes().Select(x => new SelectListItem { Value = x.fiIDCliente.ToString(), Text = x.fcNombre }).ToList();
                return PartialView();
            }
        }

        public ActionResult ModalAutorizarSolicitud(int fiIDAdicionProduto, string fcNombreCliente, string fcArticulos, string fcComentarioVendedor)
        {
            var model = new ModalAutorizarViewModel
            {
                NombreCliente = fcNombreCliente,
                Articulos = fcArticulos,
                ComentarioVendedor = fcComentarioVendedor
            };
            ViewBag.fiIDAdicionProduto = fiIDAdicionProduto;
            return PartialView(model);
        }

        public SolicitudesController()
        {
            var ConnectionString = ConfigurationManager.ConnectionStrings["OrionConnections"].ConnectionString; // DataCrypt.Desencriptar(ConfigurationManager.ConnectionStrings["ConexionEncriptada"].ConnectionString);
            _connection = new DbServiceConnection(ConnectionString);
            ServicioCorreo = new SendEmailService();
        }
        public ActionResult Solicitudes_Bandeja()
        {

            return View();
        }
        //[HttpGet]
        //public JsonResult LsitaSolicitudes()
        //{

        //    using (var conetion = new ORIONDBEntities())
        //    {
        //        var IDRol = GetUser().IdRol;
        //        var IDUser = GetIdUser();
        //        var lista = EnviarListaJson(conetion.sp_Solicitudes_Bandeja(IDRol, IDUser).ToList());
        //        //EnviarNotificacion("DESDE BASE SOLICITUDES");
        //        return lista;
        //    }

        //}




        [HttpGet]
        public ActionResult LsitaSolicitudes()
        {
            using (var conetion = new ORIONDBEntities())
            {
                var IDRol = GetUser().IdRol;
                var IDUser = GetIdUser();
                var fiIDDistribuidor = GetUser().fiIDDistribuidor;

                var rolesAsesorFreelancer = GetConfiguracion<int>("Orion_Ventas_Freelancer", ',');
                var rolesAsesor = GetConfiguracion<int>("Orion_Ventas_Asesor_Externo_Por_Distribuidor", ',');
                var rolesAdminDistribuidor = GetConfiguracion<int>("Orion_Admin_Ventas_Distribuidor", ',');

                var solicitudes = conetion.sp_Solicitudes_Bandeja(IDRol, IDUser).AsQueryable();

                if (rolesAsesor.Contains(IDRol) || rolesAsesorFreelancer.Contains(IDRol))
                {
                    solicitudes = solicitudes.Where(x => x.fiIDUsuario == IDUser);
                }
                else if (rolesAdminDistribuidor.Contains(IDRol))
                {
                    solicitudes = solicitudes.Where(x => x.fiIDDistribuidor == fiIDDistribuidor);
                }

                var resultado = solicitudes.ToList();

                if (!resultado.Any())
                    return new LargeJsonResult(new { success = false, message = "Sin datos." });

                return new LargeJsonResult(resultado);
            }
        }


        [HttpGet]
        public JsonResult SolicitudesAdicionProductosList()
        {

            using (var conetion = new ORIONDBEntities())
            {
                var IDRol = GetUser().IdRol;
                var IDUser = GetIdUser();
                var lista = EnviarListaJson(conetion.sp_Solicitudes_AdicionProducto_List(IDRol, IDUser).ToList());
                //EnviarNotificacion("DESDE BASE SOLICITUDES");
                return lista;
            }

        }

        [HttpPost]
        public JsonResult GuardarSolicitudAdicionProducto(int IdSolicitud, int IdCliente, string Articulos, string comentario)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var result = context.sp_Solicitudes_AdicionProducto_Agregar(IdCliente, Articulos, GetIdUser(), comentario, IdSolicitud) > 0;

                        return EnviarResultado(true, "Solicirud Ingresada Correctamente.");


                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }
        }

        public JsonResult ListaSolicitudes(int fiIDEquifax = 0)
        {
            try
            {
                var lista = _connection.OrionContext.sp_Solicitudes_Clientes_By_Solicitud_Listado(fiIDEquifax).ToList();
                return Json(lista, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Estado = false, Mensaje = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ActualizarSolicitudAdicionProducto(int fiIDAdicionProduto, int fiDEstado, string comentario)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var result = context.sp_Solicitudes_AdicionProducto_Actualizar(fiIDAdicionProduto, fiDEstado, GetIdUser(), comentario) > 0;

                        return EnviarResultado(true, "Solicirud Ingresada Correctamente.");


                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }
        }

        [HttpPost]
        public JsonResult RenovacionSolicitud(int fiIDsolicitud, int fiIDInternet)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {

                        var result = context.OrionSolicitudRenovacion_Maestro_Insertar(fiIDsolicitud, GetIdUser(), fiIDInternet);

                        return EnviarResultado(true, "Solicirud Ingresada Correctamente.");


                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }
        }

        public ActionResult ListaSolicitudesDeclinadas()
        {
            return View();
        }
        [HttpGet]
        public JsonResult SolicitudesDeclinadasListado()
        {

            using (var conetion = new ORIONDBEntities())
            {
                var lista = EnviarListaJson(conetion.sp_Listado_SolicitudesDeclinadas().Where(x => x.fiIDEstatusResolucion != 0).ToList());
                //EnviarNotificacion("DESDE BASE SOLICITUDES");
                return lista;
            }

        }
        [HttpGet]
        public JsonResult SolicitudesDeclinadasListadoPendientes()
        {

            using (var conetion = new ORIONDBEntities())
            {
                var lista = EnviarListaJson(conetion.sp_Listado_SolicitudesDeclinadas().Where(x => x.fiIDEstatusResolucion == 0).ToList());
                //EnviarNotificacion("DESDE BASE SOLICITUDES");
                return lista;
            }

        }
        [HttpGet]
        public ActionResult ConfrimacionNumero(string Nombre, string Telefono, int IDFirma, int IDCliente, int IdSolicitudes = 0)
        {
            using (var conetion = new ORIONDBEntities())
            {
                //var datos = conetion.sp_OrionSolicitudes_InformacionDocumentacion(0, IDCliente, 1).FirstOrDefault();
                var infoDocumentos = new sp_OrionSolicitud_InformacionDocumentacion_ViewModel();

                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_OrionSolicitudes_InformacionDocumentacion {IdSolicitudes} ,{IDCliente},{1}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        infoDocumentos = db.ObjectContext.Translate<sp_OrionSolicitud_InformacionDocumentacion_ViewModel>(reader).FirstOrDefault();
                    }

                    connection.Close();
                }

                //ViewBag.IdSolicitud = IdSolicitudes;
                ViewBag.ListaParentesco = conetion.sp_Catalogo_Parentescos_List().Select(x => new { id = x.fiIDParentesco.ToString(), text = x.fcDescripcionParentesco }).ToList();
                return PartialView(new SolicitudesViewModel { IDSolicitud = IdSolicitudes, Nombre = Nombre, Telefono = Telefono, IDFirma = IDFirma, IdCliente = IDCliente, DatosContrato = infoDocumentos });
            }
        }

        [HttpGet]
        public ActionResult ModalRenovacion(string Nombre, string Telefono, int IDFirma, int IDCliente, int IdSolicitudes = 0)
        {


            using (var conetion = new ORIONDBEntities())
            {
                //var datos = conetion.sp_OrionSolicitudes_InformacionDocumentacion(0, IDCliente, 1).FirstOrDefault();
                var infoDocumentos = new sp_OrionSolicitud_InformacionDocumentacion_ViewModel();

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

                //ViewBag.IdSolicitud = IdSolicitudes;
                var ids = conetion.sp_Internet_Renovacion().ToList();
                ViewBag.ListaOficial = ids.Select(x => new SelectListItem { Value = x.fiIDProducto.ToString(), Text = x.fcProducto }).ToList();
                ViewBag.ListaParentesco = conetion.sp_Catalogo_Parentescos_List().Select(x => new { id = x.fiIDParentesco.ToString(), text = x.fcDescripcionParentesco }).ToList();
                return PartialView(new SolicitudesViewModel { IDSolicitud = IdSolicitudes, Nombre = Nombre, Telefono = Telefono, IDFirma = IDFirma, IdCliente = IDCliente, DatosContrato = infoDocumentos });
            }
        }

        [HttpGet]
        public ActionResult ModalResolicionDeclinacion(string Nombre, int Solicitud, string Razon)
        {
            return PartialView(new SolicitudesViewModel { Nombre = Nombre, fcRazonDelcinar = Razon, IDSolicitud = Solicitud });

        }


        [HttpGet]
        public ActionResult ModalEnvioSMS(string Nombre, string Telefono, int IDCliente)
        {
            using (var conetion = new ORIONDBEntities())
            {
                //var datos = conetion.sp_OrionSolicitudes_InformacionDocumentacion(0, IDCliente, 1).FirstOrDefault();
                var infoDocumentos = new sp_OrionSolicitud_InformacionDocumentacion_ViewModel();

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

                ViewBag.ListaParentesco = conetion.sp_Catalogo_Parentescos_List().Select(x => new { id = x.fiIDParentesco.ToString(), text = x.fcDescripcionParentesco }).ToList();
                return PartialView(new SolicitudesViewModel { Nombre = Nombre, Telefono = Telefono, IdCliente = IDCliente, DatosContrato = infoDocumentos });
            }

        }

        [HttpGet]
        public ActionResult ModalEnvioCorreo(string Nombre, string Correo, int IDCliente)
        {
            using (var conetion = new ORIONDBEntities())
            {
                //var datos = conetion.sp_OrionSolicitudes_InformacionDocumentacion(0, IDCliente, 1).FirstOrDefault();
                var infoDocumentos = new sp_OrionSolicitud_InformacionDocumentacion_ViewModel();

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

                ViewBag.ListaParentesco = conetion.sp_Catalogo_Parentescos_List().Select(x => new { id = x.fiIDParentesco.ToString(), text = x.fcDescripcionParentesco }).ToList();
                return PartialView(new SolicitudesViewModel { Nombre = Nombre, IdCliente = IDCliente, fcCorreo = Correo });
            }

        }

        [HttpGet]
        public ActionResult ConfirmacionPrestamo(int IdCliente, string Identidad, int IDSolicitud)
        {
            using (var conetion = new ORIONDBEntities())
            {
                return PartialView(new SolicitudesViewModel { IdCliente = IdCliente, Identidad = Identidad, IDSolicitud = IDSolicitud });
            }

        }

        [HttpGet]
        public ActionResult ModalDetalleCliente(string Nombre, string Telefono, int IDSolicitud)
        {
            var listaDetalleCliente = new sp_OrionSolicitud_Detalle_ClienteListarViewModel();
            var listaReferencias = new List<SolicitudesReferenciaViewModel>();
            var listaEquifaxGarantia = new List<SolicitudesGarantiaViewModel>();
            var DatosDocumentoListado = new List<sp_OrionSolicitud_DocumentoListado_ViewModel>();
            var ListaInventarioPorUbicacion = new List<SolicitudesInventarioPorUbicacionViewModel>();
            var telefonosecundario = "";

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
                    listaReferencias = db.ObjectContext.Translate<SolicitudesReferenciaViewModel>(reader).ToList();
                    reader.NextResult();
                    listaEquifaxGarantia = db.ObjectContext.Translate<SolicitudesGarantiaViewModel>(reader).ToList();
                    reader.NextResult();
                    DatosDocumentoListado = db.ObjectContext.Translate<sp_OrionSolicitud_DocumentoListado_ViewModel>(reader).ToList();
                    //reader.NextResult();
                    //listaEquifaxGarantia = db.ObjectContext.Translate<SolicitudesGarantiaViewModel>(reader).ToList();

                }
                telefonosecundario = listaDetalleCliente.fcTelefonoSecundario;

                command.CommandText = $"EXEC sp_OrionSolicitud_InventarioUbicacionPorSolicitud {IDSolicitud}";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    ListaInventarioPorUbicacion = db.ObjectContext.Translate<SolicitudesInventarioPorUbicacionViewModel>(reader).ToList();

                }
                DictionaryList.Clear();
                DictionaryList.Add("fcNombreCliente", "prueba");
                connection.Close();
            }

            return PartialView(new SolicitudesViewModel { Nombre = Nombre, Telefono = Telefono, TelefonoSecundario = telefonosecundario, IDSolicitud = IDSolicitud, DatosCLientes = listaDetalleCliente, ListaGarantia = listaEquifaxGarantia, ListaReferencia = listaReferencias, DatosDocumentoListado = DatosDocumentoListado, ListaInventarioPorUbicacion = ListaInventarioPorUbicacion });


        }

        [HttpGet]
        public ActionResult ModalEnviarLinkPago(string Nombre, string Telefono, int IDFirma, string Correo, string Identidad, decimal CuotaMensual, int IdCondicionCuota, int IdSolicitud)
        {

            using (var conetion = new ORIONDBEntities())
            {
                return PartialView(new SolicitudesViewModel { Nombre = Nombre, Telefono = Telefono, IDFirma = IDFirma, fcCorreo = Correo, Identidad = Identidad, CuotaEnLempiras = CuotaMensual, fiMesesCondicion = IdCondicionCuota, IDSolicitud = IdSolicitud });
            }

        }

        [HttpGet]
        public ActionResult ModalDeclinarSolicitd(string Nombre, int Solicitud)
        {

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {

                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_ListarCatalogo_RazonDeclinar";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    var List = db.ObjectContext.Translate<SolicitudesViewModel>(reader).ToList();
                    ViewBag.ListadoRazon = List.Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDRazon), Text = z.fcRazonDelcinar }).ToList();


                }
                connection.Close();
            }
            using (var conetion = new ORIONDBEntities())
            {
                return PartialView(new SolicitudesViewModel { IDSolicitud = Solicitud, Nombre = Nombre, fiIDGestionAnterior = 1 });
            }

        }
        [HttpGet]
        public ActionResult ModalEstadosSolicitud(string Nombre, int Solicitud)
        {
            var ListaProcesoSolicitud = new List<sp_OrionSolicitud_Tiempo_SolicitudViewModel>();

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {

                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_OrionSolicitud_SolicitudEstadoProcesamiento  {Solicitud} , {GetIdUser()}";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    ListaProcesoSolicitud = db.ObjectContext.Translate<sp_OrionSolicitud_Tiempo_SolicitudViewModel>(reader).ToList();

                }

                connection.Close();
            }
            return PartialView(new SolicitudesViewModel { Nombre = Nombre, IDSolicitud = Solicitud, ListaProcesoSolicitud = ListaProcesoSolicitud });
        }

        [HttpGet]
        public ActionResult ModalGenerarSolicitudContratista(string Nombre, string Telefono, int IDSolicitud, int idCliente)
        {
            var CodigoCliente = "";
            var NumeroOrden = "";
            var OrdenTrabajo = "";


            using (var context = new ORIONDBEntities())
            {
                var datosClienteDetalle = context.sp_OrionSolicitud_Detalle_ClienteListar(GetIdUser(), IDSolicitud).FirstOrDefault();
                CodigoCliente = datosClienteDetalle.fcCodigoCliente;
                NumeroOrden = datosClienteDetalle.fcNumeroOrdenCfeus;
                OrdenTrabajo = datosClienteDetalle.fcNumeroOrdenTrabajoChepeus;


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
            }


            return PartialView(new SolicitudesViewModel { Nombre = Nombre, Telefono = Telefono, IDSolicitud = IDSolicitud, IdCliente = idCliente, CodigoCliente = CodigoCliente, NumeroOrdenCepheus = NumeroOrden, NumeroOrdenTrabajoCepheus = OrdenTrabajo });
        }

        [HttpGet]
        public ActionResult ModalGenerarSolicitudContratistaServicio(string Nombre, string Telefono, int IDSolicitud, int idCliente)
        {
            var CodigoCliente = "";
            var NumeroOrden = "";
            var OrdenTrabajo = "";
            var SolicitudAnterior = 0;
            var PaqueteAnterior = "";
            var PaqueteActual = "";

            using (var context = new ORIONDBEntities())
            {
                var datosClienteDetalle = context.sp_OrionSolicitud_Detalle_ClienteListar(GetIdUser(), IDSolicitud).FirstOrDefault();
                CodigoCliente = datosClienteDetalle.fcCodigoCliente;
                NumeroOrden = datosClienteDetalle.fcNumeroOrdenCfeus;
                OrdenTrabajo = datosClienteDetalle.fcNumeroOrdenTrabajoChepeus;
                SolicitudAnterior = datosClienteDetalle.fiSolicitudAnterior;
                PaqueteAnterior = datosClienteDetalle.fcArticulosdelContratoAnterior;
                PaqueteActual = datosClienteDetalle.fcArticulosdelContrato;



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
                        ViewBag.Listado2 = List2.Where(x => x.fiIDContratista == 3).Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDContratista), Text = z.fcNombreEmpresa }).ToList();

                    }
                    connection.Close();
                }
            }


            return PartialView(new SolicitudesViewModel { Nombre = Nombre, Telefono = Telefono, IDSolicitud = IDSolicitud, IdCliente = idCliente, CodigoCliente = CodigoCliente, NumeroOrdenCepheus = NumeroOrden, NumeroOrdenTrabajoCepheus = OrdenTrabajo, fiSolicitudAnterior = SolicitudAnterior, fcArticulosdelContratoAnterior = PaqueteAnterior, fcArticulosdelContrato = PaqueteActual });
        }

        [HttpGet]
        public ActionResult ModalGenerarSolicitudCepheus(string Nombre, string Telefono, int IDSolicitud, int idCliente)
        {
            return PartialView(new SolicitudesViewModel { Nombre = Nombre, Telefono = Telefono, IDSolicitud = IDSolicitud, IdCliente = idCliente });
        }

        [HttpGet]
        public ActionResult ModalEnviarComprobantePago(string Nombre, string Identidad, int IDSolicitud, int idCliente, decimal ValorMonedaNacional, int fiMesesCondicion)
        {
            var ValorCondicionPagar = ValorMonedaNacional * fiMesesCondicion;
            ViewBag.ListaCuentaContable = ListadoCuentas().Select(z => new SelectListItem { Value = (z.fcCuentaContable), Text = z.fcDescripcionCuenta.ToString() }).ToList();
            using (var conetion = new ORIONDBEntities())
            {
                return PartialView(new SolicitudesViewModel { Nombre = Nombre, Identidad = Identidad, IDSolicitud = IDSolicitud, IdCliente = idCliente, fnCuotaMensualMonedaNacional = ValorMonedaNacional, fiMesesCondicion = fiMesesCondicion, fnCuotaMensualMonedaNacionalCondicion = ValorCondicionPagar });
            }

        }
        [HttpGet]
        public ActionResult ShowGarantiaCliente(int id, string Nombre, string Identidad, int IDCliente)
        {


            var listaDetalleCliente = new sp_OrionSolicitud_Detalle_ClienteListarViewModel();
            var listaReferencias = new List<SolicitudesReferenciaViewModel>();
            var listaEquifaxGarantia = new List<SolicitudesGarantiaViewModel>();
            var DatosDocumentoListado = new List<sp_OrionSolicitud_DocumentoListado_ViewModel>();

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {

                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_OrionSolicitud_Detalle_ClienteListar {GetIdUser()} , {id}";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    listaDetalleCliente = db.ObjectContext.Translate<sp_OrionSolicitud_Detalle_ClienteListarViewModel>(reader).FirstOrDefault();
                    reader.NextResult();
                    listaReferencias = db.ObjectContext.Translate<SolicitudesReferenciaViewModel>(reader).ToList();
                    reader.NextResult();
                    listaEquifaxGarantia = db.ObjectContext.Translate<SolicitudesGarantiaViewModel>(reader).ToList();
                    reader.NextResult();
                    DatosDocumentoListado = db.ObjectContext.Translate<sp_OrionSolicitud_DocumentoListado_ViewModel>(reader).ToList();

                }

                connection.Close();
            }

            return PartialView(new SolicitudesViewModel { Identidad = Identidad, Nombre = Nombre, IDSolicitud = id, IdCliente = IDCliente, DatosCLientes = listaDetalleCliente, ListaGarantia = listaEquifaxGarantia, ListaReferencia = listaReferencias, DatosDocumentoListado = DatosDocumentoListado });
        }
        public ActionResult ListaProductos()
        {
            return PartialView();
        }
        public ActionResult ModalAgregarDatosCliente(string Nombre, string Identidad, int IDSolicitud, int idCliente, int fiPlazoSeleccionado)
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
        [HttpGet]
        public ActionResult ModalVerDocumentosInstalacionDatos(int IDSolicitud)
        {
            using (var conetion = new ORIONDBEntities())
            {
                return PartialView(new SolicitudesViewModel { IDSolicitud = IDSolicitud });
            }

        }

        #region Servicio

        public List<sp_geo_ListaDepartamentos_Result> ListaDepartamentos()
        {
            var lista = _connection.OrionContext.sp_geo_ListaDepartamentos().ToList();

            return lista;
        }

        public List<sp_OperacionesBanco_CuentasdeBanco_Result> ListadoCuentas()
        {
            using (var conetion = new CoreFinancieroEntities2())
            {
                var listaCuentaContable = conetion.sp_OperacionesBanco_CuentasdeBanco(1, 117, 1, 1).ToList();
                return listaCuentaContable;
            }
        }

        public ActionResult ViewDataProductoParaServicio()
        {
            return PartialView();

        }
        [HttpGet]
        public ActionResult ModalGenerarSolicitudNuevoServicio(string Nombre, string Telefono, int IDSolicitud, int idCliente)
        {
            var listaDepartamentos = ListaDepartamentos().ToList();

            var itemSeleccion = new SelectListItem { Value = "", Text = "Seleccione un departamento", Disabled = true, Selected = true };

            var listaSelectListItems = listaDepartamentos
                .Select(z => new SelectListItem { Value = Convert.ToString(z.fiCodDepartamento), Text = z.fcDepartamento.ToString() })
                .ToList();

            listaSelectListItems.Insert(0, itemSeleccion);

            ViewBag.ListaDepartamentos = listaSelectListItems;

            return PartialView(new SolicitudesViewModel { Nombre = Nombre, Telefono = Telefono, IDSolicitud = IDSolicitud, IdCliente = idCliente });
        }

        [HttpGet]
        public ActionResult ViewDataPaqueteParaServicio()
        {
            return PartialView();

        }
        public JsonResult CargarProductoServicio()
        {
            try
            {
                using (var Dbcontext = new ORIONDBEntities())
                {
                    return EnviarListaJson(Dbcontext.sp_Producto_Maestro_Cotizador().Select(x => new ProductoListarViewModel
                    {
                        //return EnviarListaJson(Dbcontext.sp_Producto_Maestro_Listar().Select(x => new ProductoListarViewModel
                        //{
                        fcImagenProducto = x.fcImagenProducto,
                        fiIDProducto = x.fiIDProducto,
                        fcProducto = x.fcProducto,
                        fnValorProducto = x.fnValorProductoME,
                        fnValorProductoME = x.fnValorProductoME,
                        fcTipoProducto = x.fcTipoProducto,
                        fnValorProductoMN = x.fnValorProductoMN,
                        fnPorcentajeImpuesto1 = x.fnPorcentajeImpuesto1,
                        fnPorcentajeImpuesto2 = x.fnPorcentajeImpuesto2,
                        fnValorCuotaMensual = x.fnValorCuotaMensual,
                        fnValorMensual = x.fnValorCuotaMensual,
                        Seleccionado = false,
                        fiIDPaquete = 0


                    }).ToList());
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public JsonResult CargarPaqueteServicio()
        {
            try
            {
                using (var context = new ORIONDBEntities())
                {
                    //  idPaquete.ForEach(x => { x.Seleccionado = true; });

                    var list = context.sp_Paquetes_List().Where(x => x.fiestadoPaquete == 1).ToList();
                    var listaPaquete = new List<DetallePaqueteViewModel>();
                    foreach (var item in list)
                    {
                        // var itemPaquete = idPaquete?.FirstOrDefault(x => x.fiIDPaquete == item.fiIDPaquete) ?? null;
                        listaPaquete.Add(new DetallePaqueteViewModel
                        {
                            fiIDPaquete = item.fiIDPaquete,
                            fcPaquete = item.fcPaquete,
                            fnValorMensual = item.fnValorMensual,
                            fnTasadeInteresAnual = item.fnTasadeInteresAnual,
                            fiIDUsuarioCreador = item.fiIDUsuarioCreador,
                            fdFechaCreacion = item.fdFechaCreacion,
                            fcProductos = item.fcProductos,
                            Seleccionado = false,

                        });

                    }

                    return EnviarListaJson(listaPaquete);
                }


            }
            catch (Exception e)
            {
                return null;
            }
        }
        public JsonResult CargarDetallePaquete(int idPaquete)
        {
            var Paquete = new DetallePaqueteViewModel();
            var DetallePaquete = new List<ListaDetallePaqueteViewModel>();
            try
            {
                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_Paquetes_Listar {idPaquete} ";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        //listaDetalleCliente = db.ObjectContext.Translate<sp_OrionSolicitud_Detalle_ClienteListarViewModel>(reader).FirstOrDefault();
                        Paquete = db.ObjectContext.Translate<DetallePaqueteViewModel>(reader).FirstOrDefault();
                        reader.NextResult();
                        DetallePaquete = db.ObjectContext.Translate<ListaDetallePaqueteViewModel>(reader).ToList();

                    }
                    connection.Close();
                    return EnviarListaJson(DetallePaquete);
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        [HttpPost]
        public JsonResult CrearSolicitudExtraCliente(SolicitudesViewModel model, sp_OrionSolicitud_Detalle_ClienteListarViewModel DatosClientesDetalle)
        {
            try
            {
                using (var Dbcontext = new ORIONDBEntities())
                {
                    var resultado = true;
                    // var Resultado = Convert.ToInt32(Dbcontext.sp_OrionSolicitud_Mestro_NuevoServicio_Insertar(model.IdCliente, DatosClientesDetalle.fcDireccionDetallada, " ", DatosClientesDetalle.fiDepartamento, DatosClientesDetalle.fiMunicipio, 0, DatosClientesDetalle.fiColonia, 0, 0, GetIdUser()).FirstOrDefault());
                    var Resultado = Convert.ToInt32(Dbcontext.sp_OrionSolicitud_Mestro_NuevoServicio_Insertar(model.IdCliente, DatosClientesDetalle.fcDireccionDetallada, " ", DatosClientesDetalle.fiDepartamento, DatosClientesDetalle.fiMunicipio, 0, DatosClientesDetalle.fiColonia, 0, 0, GetIdUser(), DatosClientesDetalle.fcCorreo, 0, 0, 0, 0).FirstOrDefault());
                    if (Resultado > 0)
                    {
                        foreach (var item in model.DatosProductos)
                        {
                            Dbcontext.sp_Equifax_GarantiaNuevoServicio_Guardar(model.IdCliente, model.Identidad, item.fiIDProducto, item.fnValorProductoMN, item.fnValorProductoME, item.fnPorcentajeImpuesto1, item.fnPorcentajeImpuesto2, GetIdUser(), item.fnValorCuotaMensual, item.fiIDPaquete, Resultado, 0, 0, 0);
                        }
                    }


                    return EnviarResultado(true, "Actualizar", "Se envio mensaje");
                }

            }
            catch (Exception e)
            {
                return EnviarException(e, "Repo Request Send Exception");
            }
        }


        #endregion
        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> MensajeToken(SolicitudesViewModel model, string Token)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var result = context.sp_CrearTokenFirma(model.IDFirma, Token) > 0;

                        MensajeriaApi.EnviarNumeroToken(model.Nombre, model.IDFirma, model.Telefono, Token);
                        //MensajeDeTexto.EnviarNumeroToken(model.Nombre, model.IDFirma, model.Telefono, Token);

                        var _emailTemplateService = new EmailTemplateService();
                        await _emailTemplateService.SendEmailToSolicitud(new EmailTemplateServiceModel
                        {
                            CustomerEmail = model.DatosContrato.fcCorreo,
                            //CustomerEmail = "kevinsanme@gmail.com",
                            IDSolicitud = model.IDSolicitud,
                            IdCliente = model.IdCliente,
                            IDFirma = model.IDFirma
                            //List_CC = new List<string> { MemoryLoadManager.EmailSystemAdministrator }
                        });
                        //MensajeDeTexto.EnviarLinkGeoLocation(model.Nombre, model.IdCliente, model.Telefono, "");
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
        public JsonResult DeclinarSolicitud(int solicitud, string comentario, int razon)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var result = context.sp_Crear_SolicitudDeclinada(solicitud, GetIdUser(), comentario, razon) > 0;

                        return EnviarResultado(true, "Solicitud", "La solicitud " + solicitud + " Fue declinada");


                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }
        }

        [HttpPost]
        public JsonResult ResolucionFinalDeclinacion(int solicitud, string comentario, int estado)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var result = context.sp_SolicitudesDeclinadas_ResolicionDelcinar(solicitud, GetIdUser(), estado, comentario) > 0;

                        if (estado == 2)
                        {
                            return EnviarResultado(true, "Solicitud", "La solicitud " + solicitud + " Fue totalmente declinada");
                        }
                        else
                        {
                            return EnviarResultado(true, "Solicitud", "La solicitud " + solicitud + " se renovo la solicitud");
                        }



                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }
        }


        [HttpPost]
        public JsonResult MensajeSMS(SolicitudesViewModel model, string Mensaje)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        MensajeriaApi.EnviarSMSPersonalizado(model.Nombre, model.Telefono, Mensaje);
                        //MensajeDeTexto.EnviarSMSPersonalizado(model.Nombre, model.Telefono, Mensaje);
                        return EnviarResultado(true, "SMS", "Mensaje Enviado");
                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }
        }

        [HttpPost]
        public async Task<JsonResult> MensajeCorreo(SolicitudesViewModel model, string Mensaje, string Asunto)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var _emailTemplateService = new EmailTemplateService();
                        await _emailTemplateService.SendEmailPresonalizado(new EmailTemplateServiceModel
                        {
                            CustomerEmail = model.fcCorreo,
                            //CustomerEmail = "kevinsanme@gmail.com",
                            HtmlBody = Mensaje,
                            Comment = Asunto,
                            //List_CC = new List<string> { MemoryLoadManager.EmailSystemAdministrator }
                        });
                        return EnviarResultado(true, "Correo", "Mensaje Enviado");
                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }
        }

        [HttpPost]
        public JsonResult CrearPrestamo(SolicitudesViewModel model)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var result = context.sp_OrionSolicitud_Prestamo_Crear(model.IDSolicitud);
                        return EnviarResultado(true, "Prestamo", "Mensaje Enviado");

                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<JsonResult> EnviarLinkPago(SolicitudesViewModel model, string LinkPago)
        {
            var datos = new SolicitudesViewModel();
            var LinkPagoGlobal = "https://ppostc.novanetgroup.com/Cobro?id=";
            var telefono = "88136509";

            using (var context = new ORIONDBEntities().Database.Connection)
            {
                try
                {
                    var ip = "0";

                    decimal CuotaCliente = 0;
                    if (model.fiMesesCondicion > 1)
                    {
                        CuotaCliente = model.CuotaEnLempiras * model.fiMesesCondicion;
                    }
                    else
                    {
                        CuotaCliente = model.CuotaEnLempiras;
                    }
                    //var codigo = "N002";
                    context.Open();
                    var command = context.CreateCommand();
                    command.CommandText = $@"EXEC sp_OperacionesPOS_CrearLink  '{"PG1R"}', '{model.Nombre}', '{model.Telefono}' , '{model.Identidad}' , {CuotaCliente} , '{model.fcCorreo}' , '{"Cobro pago inicial"}' , {GetIdUser()}, '{"172.20.3.132"}',  {model.IDSolicitud}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        datos = db.ObjectContext.Translate<SolicitudesViewModel>(reader).FirstOrDefault();
                    }
                    context.Close();
                }
                catch (Exception ex)
                {

                    throw;
                }


                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var modelCorreo = new SendEmailViewModel();
                        modelCorreo.DestinationEmail = model.fcCorreo;
                        // modelCorreo.DestinationEmail = "aebautista63@gmail.com";
                        modelCorreo.Subject = "Link de Pago";
                        modelCorreo.Body = LinkPagoGlobal + datos.fcCodigoUnico;
                        modelCorreo.EmailName = "Novanet";
                        var linkCobro = LinkPagoGlobal + datos.fcCodigoUnico;
                        MensajeriaApi.EnviarLinkPago(model.Nombre, model.IdCliente, model.Telefono, LinkPagoGlobal + datos.fcCodigoUnico, model.fcCorreo, linkCobro);
                        //MensajeriaApi.EnviarLinkPago(model.Nombre, model.IdCliente, telefono, LinkPagoGlobal + datos.fcCodigoUnico, model.fcCorreo, linkCobro);
                        await ServicioCorreo.SendEmailAsync(modelCorreo);
                        return EnviarResultado(true, "Link", "Mensaje Enviado");

                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }
        }
        [HttpPost]
        public async Task<JsonResult> EnviarSolicitudContratista(SolicitudesViewModel model, string Comentario, int idAgencia, int idAgenciaContratista, string CodigoCliente, string NumeroOrden, string NumeroOrdenTrabajo, DateTime FechaInstalacionAsignada, int tipoSolicitudInstalacion)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var _emailTemplateService = new EmailTemplateService();
                        var CuerpoCorreo = "";
                        if (tipoSolicitudInstalacion == 4)
                        {
                            var result = context.sp_OrionSolicitud_SolicitudInstalacion_CambioServicio__Insertar(GetIdUser(), model.IdCliente, model.IDSolicitud, idAgencia, idAgenciaContratista, Comentario, CodigoCliente, NumeroOrden, tipoSolicitudInstalacion, NumeroOrdenTrabajo, FechaInstalacionAsignada);
                        }
                        else
                        {
                            var result = context.sp_OrionSolicitud_ContratistaSolicitudInstalacio__Insertar(GetIdUser(), model.IdCliente, model.IDSolicitud, idAgencia, idAgenciaContratista, Comentario, CodigoCliente, NumeroOrden, tipoSolicitudInstalacion, NumeroOrdenTrabajo, FechaInstalacionAsignada);

                        }
                        //var result = context.sp_OrionSolicitud_ContratistaSolicitudInstalacio__Insertar(GetIdUser(), model.IdCliente, model.IDSolicitud, idAgencia, idAgenciaContratista, Comentario, CodigoCliente, NumeroOrden, 1, NumeroOrdenTrabajo, FechaInstalacionAsignada);


                        var datosCliente = context.sp_OrionContratista_DetalleBySolicitud(GetIdUser(), model.IDSolicitud, 0, 0).FirstOrDefault();

                        if (tipoSolicitudInstalacion == 4)
                        {
                            EnviarMensajeArticulosPendientes(model.IDSolicitud);
                            CuerpoCorreo = "Estimado : " + datosCliente.fcNombreEmpresa + " , se le notifica que esta solicitud es para cambio de servicio: " + model.Nombre + "<br/>" + " comentario:" + Comentario;
                        }
                        else
                        {
                            CuerpoCorreo = "Estimado : " + datosCliente.fcNombreEmpresa + " , se le notifica la solicitud de instalacion para el cliente: " + model.Nombre + "<br/>" + " comentario:" + Comentario;
                        }
                        //var Correo = "aebautista63@gmail.com";
                        var modelCorreo = new SendEmailViewModel();
                        var CuerpoComentarioCorreo = CuerpoCorreo;
                        modelCorreo.DestinationEmail = datosCliente.fcCorreoElectronico;
                        // modelCorreo.DestinationEmail = Correo;
                        modelCorreo.Subject = "Solicitud de contratista";
                        modelCorreo.Body = CuerpoComentarioCorreo;
                        modelCorreo.EmailName = "Novanet";


                        await _emailTemplateService.SendEmailToCustomer(new EmailTemplateServiceModel
                        {
                            IdEmailTemplate = 20,
                            CustomerEmail = modelCorreo.DestinationEmail,
                            //CustomerEmail = Correo,
                            //IdCustomer = model.fcIDCustomer,
                            //IdLoan = model.fcIDLoan,
                            Comment = "Solicitud de contratista.",
                            IDSolicitud = model.IDSolicitud,
                            IdCliente = model.IdCliente,
                            IdTransaccion = 0,
                            IDSolicitudContratrista = 0,
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

        public JsonResult EnviarMensajeArticulosPendientes(int fiIdSolicitudinstalacion)
        {
            try
            {
                var listadetalle = new InstalacionViewModel();

                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_Instalacion_TecnicosExternos_CambioServicio {fiIdSolicitudinstalacion}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        listadetalle = db.ObjectContext.Translate<InstalacionViewModel>(reader).FirstOrDefault();
                    }
                    connection.Close();
                }

                if (listadetalle.fbArticulosExtra)
                {
                    MensajeriaApi.EnviarMensajeInstalacionaClienteCambioServicio(listadetalle.FcNombre, listadetalle.FiIDSolicitud, listadetalle.FcTelefono);

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
        [HttpPost]
        public JsonResult CrearSolicitudCepheus(SolicitudesViewModel model, string Comentario)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var result = context.sp_OrionJob_CrearClienteCepheus(GetIdUser(), model.IDSolicitud, Comentario);
                        return EnviarResultado(true, "Prestamo", "Mensaje Enviado");
                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

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
                    var datosCliente = context.sp_OrionContratista_DetalleBySolicitud(GetIdUser(), model.IDSolicitud, 0, 0).FirstOrDefault();
                    var datosClienteDetalle = context.sp_OrionSolicitud_Detalle_ClienteListar(GetIdUser(), model.IDSolicitud).FirstOrDefault();
                    var documentoURL = @"\Solicitudes\Solicitud_" + model.IDSolicitud;
                    var fcUrl = documentoURL + @"\" + model.DatosDocumento.ImgenProducto.FileName;
                    var url = MemoryLoadManager.URL + documentoURL;
                    var ruta = documentoURL + @"\" + model.DatosDocumento.ImgenProducto.FileName;
                    ruta = ruta.Replace("*", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("|", "");
                    var pdfFile = url + @"\" + model.DatosDocumento.ImgenProducto.FileName;
                    var fcNombreArchivo = model.DatosDocumento.ImgenProducto.FileName.Replace(Path.GetExtension(model.DatosDocumento.ImgenProducto.FileName), "");
                    var existeParam = new ObjectParameter("piExiste", typeof(int));
                    context.sp_DocumentosOrion_Verificar_Existente(model.IDSolicitud, fcNombreArchivo, existeParam);
                    if ((int)existeParam.Value == 1) { return EnviarResultado(false, "Archivo Duplicado", "El comprobante de pago ya esta subido."); }

                    var result = context.sp_Crear_DocumentosOrion(GetIdUser(), model.IDSolicitud, fcNombreArchivo, Path.GetExtension(model.DatosDocumento.ImgenProducto.FileName), url, fcUrl, 4, model.fcComentarioPagoInicial, 1);
                    context.sp_OrionSolicitud_CabiarEstadoPrimerPago(GetIdUser(), model.IDSolicitud, model.fcComentarioPagoInicial, model.CuotaEnLempiras, model.fcCuentaContable);
                    UploadFileServer148(@"Solicitudes\Solicitud_" + model.IDSolicitud, model.DatosDocumento.ImgenProducto);
                    await EnviarCorreo(model.IDSolicitud);
                    if (datosClienteDetalle.fiPlazoSeleccionado == 24)
                    {
                        MensajeriaApi.EnviarSMSPersonalizadoAvisoCobrosArticulo(datosClienteDetalle.fcNombre, "31864531", datosClienteDetalle.fcArticulosdelContrato, datosClienteDetalle.DireccionDetalladaCliente, datosClienteDetalle.fcGeolocalizacion);
                        MensajeriaApi.EnviarSMSPersonalizadoAvisoCobrosArticulo(datosClienteDetalle.fcNombre, "31843783", datosClienteDetalle.fcArticulosdelContrato, datosClienteDetalle.DireccionDetalladaCliente, datosClienteDetalle.fcGeolocalizacion);
                        MensajeriaApi.EnviarSMSPersonalizadoAvisoCobrosArticulo(datosClienteDetalle.fcNombre, "33260693", datosClienteDetalle.fcArticulosdelContrato, datosClienteDetalle.DireccionDetalladaCliente, datosClienteDetalle.fcGeolocalizacion);
                    }

                    return EnviarResultado(true, "Solicitud", "Mensaje Enviado");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }

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
        [HttpPost]
        public string ImprimirDOC(string NombreArchivo, string IdSolicitud)
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
        public async Task<JsonResult> EnviarCorreo(int IDSolicitud)
        {

            using (var context = new ORIONDBEntities())
            {
                try
                {
                    var datosClientebyContratista = context.sp_OrionContratista_DetalleBySolicitud(GetIdUser(), IDSolicitud, 0, 0).FirstOrDefault();
                    var comandList = $"sp_OrionSolicitudes_InformacionDocumentacion {IDSolicitud}, 0, 1";
                    var datosClienteInformacionDocumentacion = _connection.LoadListDataWithDbContext<Models.Reportes.sp_OrionSolicitudes_InformacionDocumentacion_Result>(comandList).FirstOrDefault();
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
        [HttpPost]
        public JsonResult CargarListaProductos(List<ProductoListarViewModel> productos)
        {
            using (var contexto = new ORIONDBEntities())
            {
                // Inicializa la lista si es null
                if (productos == null)
                {
                    productos = new List<ProductoListarViewModel>();
                }

                if (productos.Count > 0)
                {
                    productos.ForEach(x => { x.Seleccionado = true; });
                }

                var lista = contexto.sp_Producto_Maestro_Listar().ToList();
                var listproductos = new List<ProductoListarViewModel>();

                foreach (var item in lista)
                {
                    var itemProducto = productos?.FirstOrDefault(x => x.fiIDProducto == item.fiIDProducto) ?? null;
                    listproductos.Add(new ProductoListarViewModel
                    {
                        fiIDProducto = item.fiIDProducto,
                        fcProducto = item.fcProducto,
                        Seleccionado = itemProducto?.Seleccionado ?? false,
                        fnValorProductoME = item.fnValorProductoME,
                        fnValorProductoMN = item.fnValorProductoMN,
                        fcTipoProducto = item.fcTipoProducto,
                        fnPorcentajeImpuesto1 = item.fnPorcentajeImpuesto1,
                        fnPorcentajeImpuesto2 = item.fnPorcentajeImpuesto2,
                    });
                }

                return EnviarListaJson(listproductos);
            }
        }

        [HttpPost]
        public ActionResult EditarPaquete(SolicitudesViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    contexto.sp_Equifax_Garantia_Eliminar(model.IDSolicitud);
                    foreach (var producto in model.ListaGarantia)
                    {
                        contexto.sp_Equifax_Garantia_Editar(model.IdCliente, model.Identidad, producto.fiIDProducto, producto.fnValorProductoMN, producto.fnValorProductoME, producto.fnPorcentajeImpuesto1, producto.fnPorcentajeImpuesto2, GetIdUser(), model.IDSolicitud);
                    }
                    ObtenerDataSolicitud(model.IDSolicitud);

                    return EnviarResultado(true, " ", "Se ha Editado con Exito el paquete");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }


            }

        }
        [HttpPost]
        public JsonResult GuardarDatosSolicitudCliente(SolicitudesViewModel model, string OrigenServicio, string CodigoVendedor, int IdTipoPersona, string NombreEmpresaTipoPersona, string RtnEmpresaTipoPersona, string ComentarioOferta, int IDMesPromocion, int IDCodigoPromocion, string codigoPromocion)
        {

            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var result = context.sp_OrionSolicitud_Maestro_ActualizarDatosServicio(GetIdUser(), model.IDSolicitud, 0, OrigenServicio, CodigoVendedor, IdTipoPersona, NombreEmpresaTipoPersona, RtnEmpresaTipoPersona, ComentarioOferta, null, IDMesPromocion, IDCodigoPromocion, codigoPromocion, 0) > 0;
                        //  var result = context.sp_OrionSolicitud_Maestro_ActualizarDatosServicio(GetIdUser(), model.IDSolicitud, 0, OrigenServicio, CodigoVendedor, IdTipoPersona, NombreEmpresaTipoPersona, RtnEmpresaTipoPersona, ComentarioOferta, MesPromocion, IDMesPromocion) > 0;
                        ObtenerDataSolicitud(model.IDSolicitud);
                        return EnviarResultado(true, "Datos", "Datos Registrados");
                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }
        }

        public void ObtenerDataSolicitud(int fiIDSolicitud)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var model = contexto.sp_Solicitudes_Bandeja_ObtenerPorIDSolicitud(fiIDSolicitud).FirstOrDefault();
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
                hubContext.Clients.All.actualizarBandeja(model);
            }

        }
        #region Token
        [HttpGet]
        public ActionResult ConfirmacionToken(string Nombre, int IDCliente, int IDSolicitud)
        {
            using (var conetion = new ORIONDBEntities())
            {
                return PartialView(new SolicitudesViewModel { Nombre = Nombre, IdCliente = IDCliente, IDSolicitud = IDSolicitud });
            }

        }
        [HttpGet]
        public JsonResult VerificarToken(string Token, string Codigo)
        {
            var datos = new SolicitudesViewModel();
            try
            {
                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {
                    var ip = "0";
                    //var codigo = "N002";
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_Token_Aplicar {GetIdUser()}, {ip}, {Token} , {Codigo}";
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
            catch (Exception ex)
            {

                return EnviarException(ex, "Error");
            }


        }

        #endregion

    }
}