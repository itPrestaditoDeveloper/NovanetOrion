using Newtonsoft.Json;
using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.App_Services.ContratoService;
using OrionCoreCableColor.App_Services.EmailService;
using OrionCoreCableColor.DbConnection.CMContext;
using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models;
using OrionCoreCableColor.Models.Base;
using OrionCoreCableColor.Models.EmailTemplateService;
using OrionCoreCableColor.Models.Precalificado;
using OrionCoreCableColor.Models.Reportes;
using OrionCoreCableColor.Models.Reportes.ReporteGeneral;
using OrionCoreCableColor.Models.Solicitudes;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static OrionCoreCableColor.Models.Reportes.ReporteGeneral.GraficoArpu;
using static OrionCoreCableColor.Models.Reportes.ReporteGeneral.GraficoMora;
using static OrionCoreCableColor.Models.Reportes.ReporteGeneral.GraficoVentas;


namespace OrionCoreCableColor.Controllers
{
    [System.Web.Mvc.Authorize]
    public class ReportesController : BaseController
    {

        public static Dictionary<string, string> DictionaryList = new Dictionary<string, string>();
        private DbServiceConnection _connection;
        // GET: Reportes

        public ReportesController()
        {
            var ConnectionString = ConfigurationManager.ConnectionStrings["OrionConnections"].ConnectionString; // DataCrypt.Desencriptar(ConfigurationManager.ConnectionStrings["ConexionEncriptada"].ConnectionString);
            _connection = new DbServiceConnection(ConnectionString);
        }

        public ActionResult CREACIONCLIENTECEPHEUS()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ListarClientesApiCepheus()
        {
            var listaDetalleCliente = new List<ConsultaAPIViewModel>();
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {

                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_OrionJob_CrearClienteCepheus_Lista";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    listaDetalleCliente = db.ObjectContext.Translate<ConsultaAPIViewModel>(reader).ToList();
                }

                connection.Close();

            }
            return EnviarListaJson(listaDetalleCliente);

        }

        [HttpGet]
        public JsonResult ListarClientesApiCepheusProcesados()
        {
            var listaDetalleCliente = new List<ConsultaAPIViewModel>();
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {

                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_OrionJob_CrearClienteCepheus_Lista";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    reader.NextResult();
                    listaDetalleCliente = db.ObjectContext.Translate<ConsultaAPIViewModel>(reader).ToList();
                }

                connection.Close();

            }
            return EnviarListaJson(listaDetalleCliente);

        }

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult ReportesClientes()
        {
            return View();
        }

        public ActionResult ReportesClientesFinalesInstalados()
        {
            return View();
        }

        public ActionResult ReportesSolicitudesPorAsesor()
        {
            return View();
        }

        public ActionResult ReportesClientesMesGratis()
        {
            return View();
        }
        public ActionResult ReporteClientesCortados()
        {
            return View();
        }
        public ActionResult ReporteClientesDescargadoAplicacion()
        {
            return View();
        }


        public ActionResult ReportesClientesFinalesInstaladosGlobalPDF()
        {
            using (var _connection = new ORIONDBEntities())
            {
                var datosCliente = GenerateDictionaryOfVariablesContratoGlobal().FirstOrDefault();

            }
            return View();
        }
        public ActionResult BandejaIngresoGastoPublicidad()
        {
            return View();
        }
        public ActionResult BandejaClientesConectadosPorRobchat()
        {
            return View();
        }
        public ActionResult BandejaClientesPorTerminarContrato()
        {
            return View();
        }

        public ActionResult BandejaClientesCancelados()
        {

            return View();
        }

        public JsonResult CargarDatosClientesCancelados(string fechaMinima = null, string fechaMaxima = null)
        {
            var listaDetalleDatos = new List<ReporteClientesCancelados>();

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

                command.CommandText = $"EXEC sp_Reporte_ClientesCancelados {fechaInicioParam}, {fechaFinParam}";

                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());

                    listaDetalleDatos = db.ObjectContext.Translate<ReporteClientesCancelados>(reader).ToList();
                }

                connection.Close();
            }

            return Json(listaDetalleDatos, JsonRequestBehavior.AllowGet);
        }






        public JsonResult CargarListaGastoPorPublicidad()
        {
            using (var context = new ORIONDBEntities())
            {
                return EnviarListaJson(context.sp_OrionGastosPublicidad_Listar(GetIdUser()).ToList());
            }
        }
        public JsonResult CargarListaClientesAdquiridosPorRobchat()
        {
            using (var context = new ORIONDBEntities())
            {
                return EnviarListaJson(context.sp_OrionVentas_ClientesAdquiridosPorRobchat(GetIdUser()).ToList());
            }
        }
        public JsonResult CargarDatosCLientesPorTerminarContrato()
        {
            using (var context = new ORIONDBEntities())
            {
                return EnviarListaJson(context.sp_Solicitudes_Bandeja_ClientesPorTerminarContrato(GetIdUser()).ToList());
            }
        }
        [AllowAnonymous]
        public ActionResult ReportesClientesFinalesInstaladosGlobalPDFExportart(int EnviarCorreo = 0)
        {
            ViewBag.EnviarCorreo = EnviarCorreo;
            return View();
        }

        //[HttpGet]
        //public JsonResult LsitaClientes()
        //{
        //    using (var conetion = new ORIONDBEntities())
        //    {
        //        return Json(conetion.sp_ReporteClientes().ToList(), JsonRequestBehavior.AllowGet);
        //    }

        //}

        //[HttpGet]
        //public JsonResult LsitaClientes_Sincontestar()
        //{
        //    using (var conetion = new ORIONDBEntities())
        //    {
        //        return Json(conetion.sp_ReporteClientes_SinContestar().ToList(), JsonRequestBehavior.AllowGet);
        //    }

        //}


        [HttpGet]
        public JsonResult LsitaClientes()
        {
            using (var db = new ORIONDBEntities())
            {
                var IDRol = GetUser().IdRol;
                var IDUser = GetIdUser();
                var fiIDDistribuidor = GetUser().fiIDDistribuidor;
                var rolesAsesorFreelancer = GetConfiguracion<int>("Orion_Ventas_Freelancer", ',');
                var rolesAsesor = GetConfiguracion<int>("Orion_Ventas_Asesor_Externo_Por_Distribuidor", ',');
                var rolesAdminDistribuidor = GetConfiguracion<int>("Orion_Admin_Ventas_Distribuidor", ',');
                var ReporteClientes = db.sp_ReporteClientes().AsQueryable();

                if (rolesAsesor.Contains(IDRol) || rolesAsesorFreelancer.Contains(IDRol))
                {
                    ReporteClientes = ReporteClientes.Where(x => x.fiIDUsuario == IDUser);
                }
                else if (rolesAdminDistribuidor.Contains(IDRol) )
                {
                    ReporteClientes = ReporteClientes.Where(x => x.fiIDDistribuidor == fiIDDistribuidor);
                }

                var resultado = ReporteClientes.ToList();

                if (!resultado.Any())
                    return Json(new { success = false, message = "Sin datos." }, JsonRequestBehavior.AllowGet);

                return Json(resultado, JsonRequestBehavior.AllowGet);

            }
        }



        [HttpGet]
        public JsonResult LsitaClientes_Sincontestar()
        {
            using (var db = new ORIONDBEntities())
             {
                var IDRol = GetUser().IdRol;
                var IDUser = GetIdUser();
                var fiIDDistribuidor = GetUser().fiIDDistribuidor;
                var rolesAsesorFreelancer = GetConfiguracion<int>("Orion_Ventas_Freelancer", ',');
                var rolesAsesor = GetConfiguracion<int>("Orion_Ventas_Asesor_Externo_Por_Distribuidor", ',');
                var rolesAdminDistribuidor = GetConfiguracion<int>("Orion_Admin_Ventas_Distribuidor", ',');
                var ReporteClientes_SinContestar = db.sp_ReporteClientes_SinContestar().AsQueryable();

                if (rolesAsesor.Contains(IDRol) || rolesAsesorFreelancer.Contains(IDRol))
                {
                    ReporteClientes_SinContestar = ReporteClientes_SinContestar.Where(x => x.fiIDUsuario == IDUser);
                }
                else if (rolesAdminDistribuidor.Contains(IDRol))
                {
                    ReporteClientes_SinContestar = ReporteClientes_SinContestar.Where(x => x.fiIDDistribuidor == fiIDDistribuidor);
                }

                var resultado = ReporteClientes_SinContestar.ToList();

                if (!resultado.Any())
                    return Json(new { success = false, message = "Sin datos." }, JsonRequestBehavior.AllowGet);

                return Json(resultado, JsonRequestBehavior.AllowGet);

            }
        }



        [HttpGet]
        public JsonResult ListaClientes_Solicitud_Cortes()
        {

            var usuarioLogueado = GetUser();
            using (var conetion = new ORIONDBEntities())
            {
                var lista = EnviarListaJson(conetion.sp_clientes_Solicitud_Corte_Administrativo_Listar(usuarioLogueado.fiIdUsuario, usuarioLogueado.IdRol).ToList());

                return lista;
            }

        }
        [HttpGet]
        public ActionResult CrearGastoPublicidad()
        {
            return PartialView(new CrearGastosPublicidadViewModel());
        }
        [HttpGet]
        public ActionResult EditarGastosDePublicidad(int fiIdGastosPublicidad)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Orion_GastoPublicidad_GetById(fiIdGastosPublicidad).FirstOrDefault();
                var fecha = new DateTime(modelDb.fiAnio, modelDb.fiMes, modelDb.fiDia);
                var model = new CrearGastosPublicidadViewModel
                {
                    fiIdGastosPublicidad = modelDb.fiIdGastosPublicidad,
                    fnValorMonto = modelDb.fnValorMonto,
                    fcComentario = modelDb.fcComentario,
                    fdFechaIngreso = fecha,
                    // fdFechaIngreso =

                    fbEditar = true
                };
                return PartialView("CrearGastoPublicidad", model);
            }
        }

        [HttpPost]
        public JsonResult EditarDatosGastosPublicidad(CrearGastosPublicidadViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var anio = model.fdFechaIngreso.Year;
                var Mes = model.fdFechaIngreso.Month;
                var Dia = model.fdFechaIngreso.Day;
                var result = contexto.sp_Orion_GastoPublicidad_Editar(model.fiIdGastosPublicidad, anio, Mes, model.fnValorMonto, model.fcComentario, GetIdUser(), Dia) > 0;
                if (result)
                {
                    return EnviarResultado(result, "", result ? "Registro Actualizado Exitosamente" : "Error de red");
                }
                else
                {
                    return EnviarResultado(false, "Error", "Error de red");
                }
            }
        }
        [HttpPost]
        public async Task<JsonResult> AprobarSolicitudCorteAdministrativo(int idsolicitud, string codClienteCfeus)
        {
            using (var context = new ORIONDBEntities())
            {
                try
                {
                    var apiService = new ApiServiceCfeus();
                    var apiResponse = await apiService.ConsultarServiciosActivosAsync(codClienteCfeus);

                    if (apiResponse.Codigo != "100")
                    {
                        return EnviarResultado(false, "El cliente debe estar activo en Cfeus para aprobar la solicitud de corte.");
                    }
                    var connection = context.Database.Connection;
                    if (connection.State != System.Data.ConnectionState.Open)
                        connection.Open();

                    int result = 0;

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "sp_clientes_Solicitud_Corte_Administrativo_Actualizar";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@fiIDsolicitud", idsolicitud));
                        command.Parameters.Add(new SqlParameter("@fiIDUsuarioAplicado", GetUser().fiIdUsuario));
                        command.Parameters.Add(new SqlParameter("@fdFechaAplicado", DateTime.Now));
                        var scalar = command.ExecuteScalar();
                        if (scalar != null && int.TryParse(scalar.ToString(), out int r))
                        {
                            result = r;
                        }
                    }

                    if (result > 0)
                    {
                        return EnviarResultado(true, "Solicitud", "La Solicitud Fue Aprobada e Ingresada");
                    }
                    else
                    {
                        return EnviarResultado(false, "Error de red", "No se pudo actualizar la solicitud.");
                    }
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }

        [HttpPost]
        public JsonResult EliminarSolicitudCorteAdministrativo(int idsolicitud)
        {
            using (var context = new ORIONDBEntities())
            {
                try
                {
                    var result = context.Database.SqlQuery<int>(
                        "EXEC sp_clientes_Solicitud_Corte_Administrativo_Eliminar @IdSolicitud",
                        new SqlParameter("@idsolicitud", idsolicitud)
                    ).FirstOrDefault();
                    if (result > 0)
                    {
                        return EnviarResultado(true, "Solicitud", "La solicitud fue eliminada Correctamente.");
                    }
                    else
                    {
                        return EnviarResultado(false, "Error de red", "No se pudo eliminar la solicitud.");
                    }
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }


        [HttpGet]
        public JsonResult ListarReferencias(int id)
        {


            using (var conetion = new ORIONDBEntities())
            {

                var clienteEquifax = conetion.sp_ReporteClientes().ToList().FirstOrDefault(x => x.fiIDEquifax == id);

                var list = conetion.sp_ObtenerReferencias_IDEquifax(id).ToList().Select(x => new ListReferenciasEquifax
                {
                    fcDescripcionParentesco = x.fcDescripcionParentesco,
                    fcNombreCompletoReferencia = x.fcNombreCompletoReferencia,
                    fbNuevo = false,
                    fiIDParentesco = (int)x.fiIDParentesco,
                    fcTelefonoReferencia = x.fcTelefonoReferencia,
                    fiIDEquifax = x.fiIDEquifax,
                    fiIDReferencia = x.fiIDReferencia,
                    NombreClienteEquifax = clienteEquifax.fcNombre,
                    fbAceptoSerReferencia = (int)x.fbAceptoSerReferencia,
                    fiRespuestasAcertadas = (int)x.fiRespuestasAcertadas,
                    fiRespuestasNoAcertadas = (int)x.fiRespuestasNoAcertadas,
                    fiIDTipoSolicitud = (int)x.fiIDTipoSolicitud,
                    fiIDConteoReinicio = x.fiIDConteoReinicio

                });
                return Json(list, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult DetallesReferencias(int id)
        {
            using (var conetion = new ORIONDBEntities())
            {
                ViewBag.ListaParentesco = conetion.sp_Catalogo_Parentescos_List().Select(x => new { id = x.fiIDParentesco.ToString(), text = x.fcDescripcionParentesco }).ToList();
                return PartialView(id);
            }

        }
        [HttpGet]
        public JsonResult ListaSolicitudesInstaladas()
        {

            using (var conetion = new ORIONDBEntities())
            {
                var lista = EnviarListaJson(conetion.sp_Solicitudes_Bandeja_Instalados(GetIdUser()).ToList());

                return lista;
            }

        }

        [HttpGet]
        public async Task<JsonResult> ListaSolicitudesPorAsesor(int? piAnio, int? piMes, int? piSemana, int? piIDOficial)
        {
            try
            {
                using (var context = new ORIONDBEntities())
                using (var connection = context.Database.Connection)
                {
                    await connection.OpenAsync();
                    var command = connection.CreateCommand();

                    command.CommandText = "EXEC sp_ResumenSolicitudesPorAsesor @piAnio, @piMes, @piSemana, @piIDOficial";
                    command.Parameters.Add(new SqlParameter("@piAnio", (object)piAnio ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@piMes", (object)piMes ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@piSemana", (object)piSemana ?? DBNull.Value));
                    command.Parameters.Add(new SqlParameter("@piIDOficial", (object)piIDOficial ?? DBNull.Value));

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var resultados = new List<ListSolicitudesPorAsesorViewModel>();

                        while (await reader.ReadAsync())
                        {
                            resultados.Add(new ListSolicitudesPorAsesorViewModel
                            {
                                IDOficial = reader["IDOficial"] != DBNull.Value ? (int)reader["IDOficial"] : 0,
                                NombreOficial = reader["NombreOficial"] != DBNull.Value ? (string)reader["NombreOficial"] : string.Empty,
                                DepartamentoVenta = reader["DepartamentoVenta"] != DBNull.Value ? (string)reader["DepartamentoVenta"] : string.Empty,
                                TotalIngresadas = reader["TotalIngresadas"] != DBNull.Value ? (int)reader["TotalIngresadas"] : 0,
                                TotalPendientes = reader["TotalPendientes"] != DBNull.Value ? (int)reader["TotalPendientes"] : 0,
                                TotalInstaladas = reader["TotalInstaladas"] != DBNull.Value ? (int)reader["TotalInstaladas"] : 0,
                                TotalEnProceso = reader["TotalEnProceso"] != DBNull.Value ? (int)reader["TotalEnProceso"] : 0,
                                InstaladasVsPendientes = reader["InstaladasVsPendientes"] != DBNull.Value ? (double)reader["InstaladasVsPendientes"] : 0.0,
                                EnProcesoVsPendientes = reader["EnProcesoVsPendientes"] != DBNull.Value ? (double)reader["EnProcesoVsPendientes"] : 0.0,
                                TotalIngresadasSoloServicio = reader["TotalIngresadasSoloServicio"] != DBNull.Value ? (int)reader["TotalIngresadasSoloServicio"] : 0,
                                TotalIngresadasConArticulo = reader["TotalIngresadasConArticulo"] != DBNull.Value ? (int)reader["TotalIngresadasConArticulo"] : 0,
                                TotalIngresadasSoloServicioVsTotal = reader["TotalIngresadasSoloServicioVsTotal"] != DBNull.Value ? (double)reader["TotalIngresadasSoloServicioVsTotal"] : 0.0,
                                TotalIngresadasConArticuloVsTotal = reader["TotalIngresadasConArticuloVsTotal"] != DBNull.Value ? (double)reader["TotalIngresadasConArticuloVsTotal"] : 0.0,
                                ArpuPromedioTotalIngresadas = reader["ArpuPromedioTotalIngresadas"] != DBNull.Value ? (double)reader["ArpuPromedioTotalIngresadas"] : 0.0,
                            });
                        }

                        if (resultados.Count == 0)
                        {
                            return Json(new { message = "No se encontraron solicitudes para los criterios seleccionados." }, JsonRequestBehavior.AllowGet);
                        }

                        return EnviarListaJson(resultados);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult ObtenerDetalleSolicitudesPorAsesor(int? anio, int? mes, int? semana, int idOficial, string tipo)
        {
            using (var conetion = new ORIONDBEntities())
            {
                var lista = EnviarListaJson(conetion.sp_DetalleSolicitudesPorAsesor(anio, mes, semana, idOficial, tipo).ToList());
                return lista;
            }

        }



        [HttpGet]
        public JsonResult ListaSolicitudesMEsGratis()
        {

            using (var conetion = new ORIONDBEntities())
            {
                var lista = EnviarListaJson(conetion.sp_Solicitudes_Bandeja_ClienteMesGratis(GetIdUser()).ToList());

                return lista;
            }

        }
        [HttpPost]
        public JsonResult ListaSolicitudesClientesIniciadoAplicacion(int id)
        {

            using (var conetion = new ORIONDBEntities())
            {
                var lista = EnviarListaJson(conetion.sp_Solicitudes_ClientesIniciadoAplicacion(id, GetIdUser()).ToList());

                return lista;
            }

        }

        [HttpPost]
        public JsonResult GuardaryEnviarMensaje(ListReferenciasEquifax model)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        model.NombreClienteEquifax = context.sp_ReporteClientes().ToList().FirstOrDefault(x => x.fiIDEquifax == model.fiIDEquifax).fcNombre;
                        var usuario = security.Usuarios.FirstOrDefault(x => x.fcNombreCorto == User.Identity.Name);
                        var result = Convert.ToInt32(context.sp_Cliente_Referencia_Insertar(model.fiIDEquifax, model.fcNombreCompletoReferencia, model.fcTelefonoReferencia, model.fiIDParentesco, usuario.fiIDUsuario, "").FirstOrDefault());

                        if (result > 0)
                        {
                            MensajeDeTexto.EnviarNumeroReferencias(result, model.fcNombreCompletoReferencia, "", model.fiIDEquifax, model.fcTelefonoReferencia, "");
                            return EnviarResultado(true, "Referencia", "Referencia enviada y Guardado con exito ");
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

        [HttpPost]
        public JsonResult EditarTelRef(ListReferenciasEquifax model)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {

                        var result = context.sp_Cliente_Referencia_EditarNumeroReferencia(model.fiIDReferencia, model.fcTelefonoReferencia, model.fcNombreCompletoReferencia, model.fiIDParentesco);
                        if (result > 0)
                        {
                            return EnviarResultado(true, "Referencia", "Se edito con Exito el telefono");
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

        [HttpPost]
        public JsonResult ReenviarMensaje(ListReferenciasEquifax model)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        model.NombreClienteEquifax = context.sp_ReporteClientes().ToList().FirstOrDefault(x => x.fiIDEquifax == model.fiIDEquifax).fcNombre;
                        var usuario = security.Usuarios.FirstOrDefault(x => x.fcNombreCorto == User.Identity.Name);
                        var result = context.sp_ObtenerReferencias_IDEquifax(model.fiIDEquifax).ToList().FirstOrDefault(x => x.fcTelefonoReferencia == model.fcTelefonoReferencia).fiIDReferencia;
                        if (result > 0)
                        {
                            MensajeDeTexto.EnviarNumeroReferencias(result, model.fcNombreCompletoReferencia, "", model.fiIDEquifax, model.fcTelefonoReferencia, "");
                            return EnviarResultado(true, "Referencia", "Mensaje Reenviada");
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

        [HttpPost]
        public JsonResult ReenviarMensaje_Principal(int IDEquifax, string Nombre, string Telefono)
        {
            try
            {
                MensajeriaApi.EnviarNumero(Nombre, IDEquifax, Telefono);
                //MensajeDeTexto.EnviarNumero(Nombre, IDEquifax, Telefono);
                return EnviarResultado(true, "Cliente", "Mensaje Reenviado");
            }
            catch (Exception ex)
            {
                return EnviarException(ex, "Error");
            }

        }

        [HttpPost]
        public JsonResult ProcesarSolicitud(int id, int Estatus)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var result = Convert.ToInt32(context.sp_ActualizarEstatusSolicitudProducto(id, Estatus).FirstOrDefault());
                        if (result > 0)
                        {
                            if (result == 1)
                            {
                                result = Convert.ToInt32(context.sp_OrionSolicitud_Mestro_Insertar(id, GetIdUser()).FirstOrDefault());

                                if (result > 0)
                                {
                                    return EnviarResultado(true, "Solicitud", "La Solicitud Fue Aprobada Correctamente.");
                                }
                                else
                                {
                                    return EnviarResultado(false, "Solicitud", "Error.");
                                }
                            }
                            else
                                return EnviarResultado(true, "Solicitud", "La Solicitud Fue Rechazada.");
                        }
                        else
                        {
                            return EnviarResultado(false, "Error de red", "");
                        }
                        ;
                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }
        }

        [HttpPost]
        public JsonResult CrearDatosGastosPublicidad(CrearGastosPublicidadViewModel model)
        {

            using (var contexto = new ORIONDBEntities())
            {
                try
                {

                    //var result = true;
                    var anio = model.fdFechaIngreso.Year;
                    var Mes = model.fdFechaIngreso.Month;
                    var Dia = model.fdFechaIngreso.Day;
                    var result = contexto.sp_OrionVentas_GastosPublicidad_Guardar(anio, Mes, model.fnValorMonto, model.fcComentario, GetIdUser(), Dia) > 0;
                    return EnviarResultado(result, "", result ? "Datos registrada exitosamente" : "Error de red");

                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "error");
                }

            }

        }

        [HttpGet]
        public JsonResult EditarNumero(string Telefono, int IDEquifax)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {

                        var result = context.sp_EditarTelefonoCliente(IDEquifax, Telefono);
                        if (result > 0)
                        {
                            return EnviarResultado(true, "Cliente", "Se edito con Exito el telefono");
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


        [AllowAnonymous]
        public HttpPostedFileBase GenerarContrato(int idSolicitud, string fileName, string Procedimiento = "sp_OrionSolicitudes_InformacionDocumentacion")
        {
            var contratoService = new ContratoTemplateService();
            var contrato = new MemoryStream();
            var archivo = contratoService.GenerarContrato(idSolicitud, 0, Procedimiento);
            archivo.CopyTo(contrato);
            return (HttpPostedFileBase)new MemoryPostedFile(contrato.ToArray(), fileName);
        }

        [AllowAnonymous]
        public HttpPostedFileBase GenerarPagare(int idSolicitud, string fileName)
        {
            var contratoService = new ContratoTemplateService();
            var pagare = new MemoryStream();
            var archivo = contratoService.GenerarPagare(idSolicitud, 0);
            archivo.CopyTo(pagare);
            return (HttpPostedFileBase)new MemoryPostedFile(pagare.ToArray(), fileName);
        }


        [HttpGet]
        public void GenerarContratoSolicitud(int idSolicitud)
        {
            var contratoService = new ContratoTemplateService();
            var contrato = new MemoryStream();
            var archivo = contratoService.GenerarContrato(idSolicitud, 0);
            archivo.CopyTo(contrato);
            TempData["ReportePDF"] = contrato;
            //return (HttpPostedFileBase)new MemoryPostedFile(contrato.ToArray(), fileName);
        }


        [HttpGet]
        public void GenerarPagareSolicitud(int idSolicitud)
        {
            var contratoService = new ContratoTemplateService();
            var pagare = new MemoryStream();
            var archivo = contratoService.GenerarPagare(idSolicitud, 0);
            archivo.CopyTo(pagare);
            TempData["ReportePDF"] = pagare;
            //return (HttpPostedFileBase)new MemoryPostedFile(contrato.ToArray(), fileName);
        }

        [HttpPost]
        [AllowAnonymous]
        //[EnableCors(origins: "", methods: "", headers: "*")]
        public async Task<JsonResult> GuardarArchivoExterno(DocumentosViewModel model)
        {

            try
            {

                using (var context = new ORIONDBEntities())
                {



                    var comandList = $"sp_OrionSolicitudes_InformacionDocumentacion {model.IDSolicitud}, 0, 1";

                    using (var log = new EventLog("Application"))
                    {
                        log.Source = "Application";
                        log.WriteEntry(comandList, EventLogEntryType.Warning, 101, 1);
                    }

                    var datosCliente = _connection.LoadListDataWithDbContext<Models.Reportes.sp_OrionSolicitudes_InformacionDocumentacion_ResultV2>(comandList).FirstOrDefault();



                    var nombreContrato = $"Contrato_Solicitud{model.IDSolicitud}_Identidad{datosCliente.fcIdentidad}.pdf";
                    var nombrePagare = $"Pagare_Solicitud{model.IDSolicitud}_Identidad{datosCliente.fcIdentidad}.pdf";

                    var documentoURL = @"\Solicitudes\Solicitud_" + model.IDSolicitud;
                    var urlPdf = MemoryLoadManager.URL + documentoURL;
                    var ruta = documentoURL + @"\";
                    ruta = ruta.Replace("*", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("|", "");

                    var pdfFileContrato = urlPdf + @"\" + nombreContrato;
                    var pdfFilePagare = urlPdf + @"\" + nombrePagare;

                    var exists = System.IO.Directory.Exists(urlPdf);
                    context.sp_Crear_DocumentosOrion(GetIdUser(), model.IDSolicitud, Path.GetFileNameWithoutExtension(nombreContrato), Path.GetExtension(nombreContrato), urlPdf, pdfFileContrato, 1, model.fcComentario, model.IDFirma);
                    context.sp_Crear_DocumentosOrion(GetIdUser(), model.IDSolicitud, Path.GetFileNameWithoutExtension(nombrePagare), Path.GetExtension(nombrePagare), urlPdf, pdfFilePagare, 2, model.fcComentario, model.IDFirma);
                    await UploadFileServer148Async(@"Solicitudes\Solicitud_" + model.IDSolicitud, GenerarContrato(model.IDSolicitud, nombreContrato));
                    await UploadFileServer148Async(@"Solicitudes\Solicitud_" + model.IDSolicitud, GenerarPagare(model.IDSolicitud, nombrePagare));
                }


                EnviarNotificacion((model.NombreCliente ?? "") + " ha firmado su contrato de convenio y servicios.");


                return EnviarResultado(true, "", "Subido");
            }
            catch (Exception ex)
            {
                return EnviarException(ex, "Error");
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> GuardarArchivoRenovacion(DocumentosViewModel model)
        {

            try
            {

                using (var context = new ORIONDBEntities())
                {



                    var comandList = $"sp_OrionSolicitudes_InformacionDocumentacion_Renovacion {model.fiIDRenovacion}, 0, 1";

                    using (var log = new EventLog("Application"))
                    {
                        log.Source = "Application";
                        log.WriteEntry(comandList, EventLogEntryType.Warning, 101, 1);
                    }

                    var datosCliente = _connection.LoadListDataWithDbContext<Models.Reportes.sp_OrionSolicitudes_InformacionDocumentacion_ResultV2>(comandList).FirstOrDefault();



                    var nombreContrato = $"ContratoRenovacion_Solicitud{model.IDSolicitud}_{model.fiIDRenovacion}_Identidad{datosCliente.fcIdentidad}.pdf";
                    //var nombrePagare = $"Pagare_Solicitud{model.IDSolicitud}_Identidad{datosCliente.fcIdentidad}.pdf";

                    var documentoURL = @"\Renovaciones\Solicitud_" + model.IDSolicitud;
                    var urlPdf = MemoryLoadManager.URL + documentoURL;
                    var ruta = documentoURL + @"\";
                    ruta = ruta.Replace("*", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("|", "");

                    var pdfFileContrato = urlPdf + @"\" + nombreContrato;
                    //var pdfFilePagare = urlPdf + @"\" + nombrePagare;

                    var exists = System.IO.Directory.Exists(urlPdf);
                    context.sp_Crear_DocumentosOrion_Renovacion(GetIdUser(), model.fiIDRenovacion, Path.GetFileNameWithoutExtension(nombreContrato), Path.GetExtension(nombreContrato), urlPdf, pdfFileContrato, 1, model.fcComentario, model.IDFirma);
                    //context.sp_Crear_DocumentosOrion(GetIdUser(), model.IDSolicitud, Path.GetFileNameWithoutExtension(nombrePagare), Path.GetExtension(nombrePagare), urlPdf, pdfFilePagare, 2, model.fcComentario, model.IDFirma);
                    await UploadFileServer148Async(@"Renovaciones\SolicitudRenovacion_" + model.IDSolicitud, GenerarContrato(model.fiIDRenovacion, nombreContrato, "sp_OrionSolicitudes_InformacionDocumentacion_Renovacion"));
                    //await UploadFileServer148Async(@"Solicitudes\Solicitud_" + model.IDSolicitud, GenerarPagare(model.IDSolicitud, nombrePagare));
                }


                EnviarNotificacion((model.NombreCliente ?? "") + " ha firmado su contrato de convenio y servicios.");


                return EnviarResultado(true, "", "Subido");
            }
            catch (Exception ex)
            {
                return EnviarException(ex, "Error");
            }

        }

        [HttpGet]
        public ActionResult EditarNumeroCliente(string Nombre, string Telefono, int IDEquifax)
        {

            return PartialView(new SolicitudesViewModel { Nombre = Nombre, Telefono = Telefono, IdCliente = IDEquifax });

        }

        [HttpGet]
        //[AllowAnonymous]
        public ActionResult VerificarAnexo(int idSolicitud)
        {

            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {

                        var result = Convert.ToInt32(context.sp_VerificarEstadoAnexo(idSolicitud).FirstOrDefault());
                        if (result == 4)
                        {
                            return EnviarResultado(true, "Anexo", "Ya se firmo el Anexo");
                        }
                        else
                        {
                            return EnviarResultado(false, "Anexo", "Realizar la firma para finalizar la instalacion");
                        }
                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }

        }

        [HttpGet]
        public ActionResult VerificarAnexoOrdenTrabajo(int idSolicitudContratista)
        {

            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {

                        var result = Convert.ToInt32(context.sp_VerificarEstadoAnexoOrdenTrabajo(idSolicitudContratista).FirstOrDefault());
                        if (result == 9)
                        {
                            return EnviarResultado(true, "Anexo", "Ya se firmo el Anexo");
                        }
                        else
                        {
                            return EnviarResultado(false, "Anexo", "Realizar la Firma para Finalizar Orden de Trabajo");
                        }
                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }

        }

        private Dictionary<string, string> GenerateDictionaryOfVariables(int idSolicitud)
        {
            using (var _connection = new ORIONDBEntities())
            {

                var datosCliente = _connection.sp_OrionContratista_DetalleBySolicitud(1, idSolicitud, 0, 0).FirstOrDefault();
                DictionaryList.Clear();
                DictionaryList.Add("DateNow", DateTime.Now.ToString("MMM dd, yyyy"));
                DictionaryList.Add("fcNombreEmpresa", datosCliente.fcNombreEmpresa);
                DictionaryList.Add("fcNombreCliente", datosCliente.fcNombre);
                DictionaryList.Add("fcNumeroORden", "001");
                DictionaryList.Add("fcTelefonoCliente", datosCliente.fcTelefonoCliente);
                DictionaryList.Add("fcEmailCliente", datosCliente.fcNombreEmpresa);
                DictionaryList.Add("fcEquipoAdquirido", datosCliente.fcArticulosdelContrato);
                DictionaryList.Add("fcDireccionDomicilioCliente", datosCliente.fcDepartamento + " " + datosCliente.fcMunicipio + "  " + datosCliente.fcBarrio + " " + datosCliente.fcDireccionDetallada);
                DictionaryList.Add("fcObservaciones", datosCliente.fcComentario);
                DictionaryList.Add("fcProductosAdicionados", datosCliente.fcProductosAdicionados);
                DictionaryList.Add("NombreTecnico", "NLA");
                DictionaryList.Add("HoraProgramada", "NLA");
                DictionaryList.Add("fcHoraInicio", "NLA");
                DictionaryList.Add("fcHoraFinal", "NLA");
                DictionaryList.Add("fcFirma", datosCliente.fcFirma);

            }


            return DictionaryList;
        }


        #region Firma y Creacion Documento de orden de trabajo

        public ActionResult AnexosOrdenDeTrabajo(int solicitudContratista)
        {
            using (var _connection = new ORIONDBEntities())
            {
                //var datosCliente = GenerateDictionaryOfVariablesPorIdContratista(solicitudContratista).FirstOrDefault();//aqui se genera el Direccionari que es el paso #1 
                //var datosClientePrueba = _connection.sp_SolicitudesAsignadas_By_Tecnico(1, 1).Where(a => a.fiIDSolicitudInstalacion == solicitudContratista).FirstOrDefault(); //(1, IdSolicitudContratista, 0, 0).FirstOrDefault();

                //var modeloaenviar = new EmailEnviarDocumentosViewModel();
                //var dictionary = ModificarPlantillasDocumentos(datosClientePrueba, modeloaenviar, "Emailenviar","EmailconCopia","NombreMostrarArchivo","NombreBuscarArchivo","Ruta Guardar Archivo");

                return View(solicitudContratista);
            }
        }

        private Dictionary<string, string> GenerateDictionaryOfVariablesPorIdContratista(int IdSolicitudContratista)
        {
            using (var _connection = new ORIONDBEntities())
            {
                //cambiar esta parte de 1,1 que son el usuario 1 con el rol 1 que son como el administrador y con el rol de administrador para que pueda verlo todo, que siento que no deberia de ser asi pero es una solucion rapida 
                var datosCliente = _connection.sp_SolicitudesAsignadas_By_Tecnico(1, 1).Where(a => a.fiIDSolicitudInstalacion == IdSolicitudContratista).FirstOrDefault(); //(1, IdSolicitudContratista, 0, 0).FirstOrDefault();
                DictionaryList.Clear();
                DictionaryList.Add("DateNow", DateTime.Now.ToString("MMM dd, yyyy"));
                DictionaryList.Add("fcNombreEmpresa", datosCliente.fcNombreEmpresa);
                DictionaryList.Add("fcNombreCliente", datosCliente.fcNombreCliente);
                DictionaryList.Add("fcNumeroORden", "001");
                DictionaryList.Add("fcTelefonoCliente", datosCliente.fcTelefonoCliente);
                DictionaryList.Add("fcEmailCliente", datosCliente.fcNombreEmpresa);
                DictionaryList.Add("fcEquipoAdquirido", datosCliente.fcArticulosdelContrato);
                //DictionaryList.Add("fcDireccionDomicilioCliente", datosCliente.fcDepartamento + " " + datosCliente.fcMunicipio + "  " + datosCliente.fcBarrio + " " + datosCliente.fcDireccionDetallada);
                DictionaryList.Add("fcDireccionDomicilioCliente", datosCliente.fcDepartamento + " " + datosCliente.fcDireccionDetallada);
                DictionaryList.Add("fcObservaciones", datosCliente.fcComentario);
                DictionaryList.Add("NombreTecnico", "NLA");
                DictionaryList.Add("HoraProgramada", "NLA");
                DictionaryList.Add("fcHoraInicio", "NLA");
                DictionaryList.Add("fcHoraFinal", "NLA");
                //DictionaryList.Add("fcFirma", datosCliente.fcFirma);
            }

            return DictionaryList;
        }


        [HttpPost]
        public async Task<JsonResult> EnviarAnexoOrdenTrabajoAlCliente(int IdSolicitudContratista, string firma)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var _emailTemplateService = new EmailTemplateService();
                        //var datosCliente = context.sp_OrionContratista_DetalleBySolicitud(1, idSolicitud,0,0).FirstOrDefault();
                        var datosCliente = context.sp_SolicitudesAsignadas_By_Tecnico(1, 1).Where(a => a.fiIDSolicitudInstalacion == IdSolicitudContratista).FirstOrDefault(); //(1, IdSolicitudContratista, 0, 0).FirstOrDefault();

                        //var modelCorreo = new SendEmailViewModel();
                        //modelCorreo.DestinationEmail = "edgardoprestadito@gmail.com";//datosCliente.fcCorreo;
                        //var correoContratista = "edgardoprestadito@gmail.com"; //datosCliente.fcCorreo;

                        //
                        var emailmodel = new EmailEnviarDocumentosViewModel
                        {
                            fiIdEmailTemplate = 21,
                            fcEmailDestino = "edgardoprestadito@gmail.com",
                            fcNombreMostrarArchivo = "PruebadeArchivo",
                            fcNombreARchivoAEditar = "PlantillaOrdenTrabajoIncidente.docx",
                            fcRutaGuardarARchivo = $"~/Solicitudes/Solicitud_{datosCliente.fiIDSolicitud}",
                            fcEmailcopiacorreo = new List<string> { MemoryLoadManager.EmailSystemAdministrator + "; " + "edgardoprestadito@gmail.com" },
                            fiIdSolicitud = (int)datosCliente.fiIDSolicitud,
                            fiIdCliente = datosCliente.fiIDEquifax ?? 0,
                            fiIdContratistaSolicitud = IdSolicitudContratista

                        };

                        //buscar una manera de que en datosclientes ponerle la firma para que se muestre 
                        await ModificarPlantillasDocumentos(datosCliente, emailmodel);

                        //await ServicioCorreo.SendEmailAsync(modelCorreo);
                        //await _emailTemplateService.SendEmailToCustomerFirmadoOrdenTrabajo(new EmailTemplateServiceModel // aquienviamos el documento al cliente que en teoria es el paso #2
                        //{
                        //    IdEmailTemplate = 21,
                        //    CustomerEmail = modelCorreo.DestinationEmail,
                        //    //CustomerEmail = "kevinsanme@gmail.com",
                        //    //IdCustomer = model.fcIDCustomer,
                        //    //IdLoan = model.fcIDLoan,
                        //    Comment = "Anexo.",
                        //    IDSolicitud = (int)datosCliente.fiIDSolicitud,
                        //    IdCliente = datosCliente.fiIDEquifax ?? 0,
                        //    List_CC = new List<string> { MemoryLoadManager.EmailSystemAdministrator + "; " + correoContratista },
                        //    firma = firma
                        //});

                        //await _emailTemplateService.SendEmailToContratoPagare(new EmailTemplateServiceModel
                        //{
                        //    CustomerEmail = modelCorreo.DestinationEmail,
                        //    IDSolicitud = (int)datosCliente.fiIDSolicitud,
                        //    List_CC = new List<string> { MemoryLoadManager.EmailSystemAdministrator },
                        //    Comment = "Documentos",
                        //    IdCliente = datosCliente.fiIDEquifax ?? 0,
                        //});

                        return EnviarResultado(true, "Solicitud", "Mensaje Enviado");

                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                        ex.Message.ToString();
                    }

                }

            }
        }

        #endregion


        private Dictionary<string, string> GenerateDictionaryOfVariablesContratoGlobal()
        {
            var datos = _connection.OrionContext.sp_DashboardInformativo(1).FirstOrDefault();

            DictionaryList.Clear();
            // DictionaryList.Add("DateNow", DateTime.Now.ToString("MMM dd, yyyy"));
            // DictionaryList.Add("fcNombreEmpresa", datosCliente.fcNombreEmpresa);
            //DictionaryList.Add("fcNombreCliente", datosCliente.fcNombre);
            // DictionaryList.Add("fiClientesActivo", datos.fiClientesActivos.ToString());
            DictionaryList.Add("fcArpuGlobal", "0");
            DictionaryList.Add("fcMora", "0");
            DictionaryList.Add("fcArpusMes", "0");
            DictionaryList.Add("fcCCancelados", "2");
            DictionaryList.Add("fiAutorizados", "10");
            DictionaryList.Add("fcSerProduc", "NLA");
            DictionaryList.Add("fcSerSinProduc", "NLA");


            return DictionaryList;
        }

        public ActionResult Anexo(int solicitud)
        {
            using (var _connection = new ORIONDBEntities())
            {
                var datosCliente = GenerateDictionaryOfVariables(solicitud).FirstOrDefault();

                return View(solicitud);
            }
        }

        public ActionResult Anexo_ComparativaInstalado(int solicitud)
        {
            var result = new ComparativaInstaladoViewModel
            {
                jsonArticulosEntregados = new List<ProductosComparativa>(),
                jsonArticulosPaquete = new List<ProductosComparativa>()
            };

            using (var context = new ORIONDBEntities())
            {
                var connection = context.Database.Connection;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"EXEC sp_Contratista_SolicitudInstalacion_Comparativa_Instalado {solicitud}";

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result.fiIDContratistaSolicitud = Convert.ToInt32(reader["fiIDContratistaSolicitud"]);
                            result.fcNombre = reader["fcNombre"].ToString();
                            result.fcIdentidad = reader["fcIdentidad"].ToString();

                            string jsonEntregados = reader["jsonArticulosEntregados"]?.ToString();
                            if (!string.IsNullOrWhiteSpace(jsonEntregados))
                                result.jsonArticulosEntregados = JsonConvert.DeserializeObject<List<ProductosComparativa>>(jsonEntregados);

                            string jsonPaquete = reader["jsonArticulosPaquete"]?.ToString();
                            if (!string.IsNullOrWhiteSpace(jsonPaquete))
                                result.jsonArticulosPaquete = JsonConvert.DeserializeObject<List<ProductosComparativa>>(jsonPaquete);
                        }
                    }

                    connection.Close();
                }
            }

            return PartialView(result);
        }

        [HttpPost]
        public JsonResult InsertarConfirmacionCliente(ConfirmacionClienteRequest request)
        {
            using (var context = new ORIONDBEntities())
            {
                try
                {
                    foreach (var producto in request.productos)
                    {
                        var resultado = context.Database.SqlQuery<RequestStatus>(
                            "EXEC sp_Cliente_SolicitudInstalacion_Confirmacion_Insertar @p0, @p1, @p2",
                            request.solicitudId,
                            producto.fiProducto,
                            producto.fnCantidad
                        ).FirstOrDefault();


                        // Si alguno falla, retornamos error inmediatamente
                        if (resultado == null || resultado.CodeStatus != 1)
                        {
                            return EnviarResultado(false, "Error", resultado?.MessageStatus ?? "Error desconocido.");
                        }
                    }

                    return EnviarResultado(true, "Éxito", "Todos los productos fueron confirmados correctamente.");
                }
                catch (Exception ex)
                {
                    return EnviarResultado(false, "Error", "Excepción: " + ex.Message);
                }
            }
        }



        [AllowAnonymous]
        public ActionResult Adendum(string solicitud)
        {
            if (string.IsNullOrEmpty(solicitud))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                string urlDesencriptada = Encoding.UTF8.GetString(Convert.FromBase64String(solicitud));
                Uri uri = new Uri(urlDesencriptada);
                var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                string idSolicitudStr = queryParams["solicitud"];
                if (string.IsNullOrEmpty(idSolicitudStr) || !int.TryParse(idSolicitudStr, out int idSolicitud))
                {
                    return RedirectToAction("Login", "Account");
                }

                using (var _connection = new ORIONDBEntities())
                {
                    var datosCliente = GenerateDictionaryOfVariables(idSolicitud).FirstOrDefault();
                    return View(idSolicitud);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Login", "Account");
            }
        }


        [HttpPost]
        public async Task<JsonResult> EnviarAnexoCliente(int idSolicitud, string firma)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var _emailTemplateService = new EmailTemplateService();
                        var datosCliente = context.sp_OrionContratista_DetalleBySolicitud(1, idSolicitud, 0, 0).FirstOrDefault();
                        var modelCorreo = new SendEmailViewModel();
                        modelCorreo.DestinationEmail = datosCliente.fcCorreo;
                        var correoContratista = datosCliente.fcCorreoElectronico;
                        //await ServicioCorreo.SendEmailAsync(modelCorreo);
                        await _emailTemplateService.SendEmailToCustomerFirmado(new EmailTemplateServiceModel
                        {
                            IdEmailTemplate = 20,
                            CustomerEmail = modelCorreo.DestinationEmail,
                            //CustomerEmail = "kevinsanme@gmail.com",
                            //IdCustomer = model.fcIDCustomer,
                            //IdLoan = model.fcIDLoan,
                            Comment = "Anexo.",
                            IDSolicitud = idSolicitud,
                            IdCliente = datosCliente.fiIDEquifax ?? 0,
                            List_CC = new List<string> { MemoryLoadManager.EmailSystemAdministrator + "; " + correoContratista },
                            firma = firma
                        });

                        await _emailTemplateService.SendEmailToContratoPagare(new EmailTemplateServiceModel
                        {
                            CustomerEmail = modelCorreo.DestinationEmail,
                            IDSolicitud = idSolicitud,
                            List_CC = new List<string> { MemoryLoadManager.EmailSystemAdministrator },
                            Comment = "Documentos",
                            IdCliente = datosCliente.fiIDEquifax ?? 0,
                        });

                        return EnviarResultado(true, "Solicitud", "Mensaje Enviado");

                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                        ex.Message.ToString();
                    }

                }

            }
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult VerificarAdendum(int idSolicitud)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var resultQuery = context.sp_VerificarEstadoAdendum(idSolicitud).FirstOrDefault();
                        int result = resultQuery != null ? Convert.ToInt32(resultQuery) : -1;

                        if (result == 4)
                        {
                            return EnviarResultado(true, "Anexo", "Ya se firmó el Adendum");
                        }
                        else if (result == -1)
                        {
                            return EnviarResultado(false, "Anexo", "No se encontró información del Adendum");
                        }
                        else
                        {
                            return EnviarResultado(false, "Anexo", "Realizar la firma para adición de equipo");
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
        [AllowAnonymous]
        public async Task<JsonResult> EnviarAdendumCliente(int idSolicitud, string firma)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var _emailTemplateService = new EmailTemplateService();
                        var datosCliente = context.sp_OrionContratista_DetalleBySolicitud(1, idSolicitud, 0, 0).FirstOrDefault();
                        var modelCorreo = new SendEmailViewModel();
                        modelCorreo.DestinationEmail = datosCliente.fcCorreo;
                        var correoContratista = datosCliente.fcCorreoElectronico;
                        await _emailTemplateService.SendEmailToCustomerAdendumFirmado(new EmailTemplateServiceModel
                        {
                            IdEmailTemplate = 24,
                            CustomerEmail = modelCorreo.DestinationEmail,
                            Comment = "Anexo.",
                            IDSolicitud = idSolicitud,
                            IdCliente = datosCliente.fiIDEquifax ?? 0,
                            List_CC = new List<string> { MemoryLoadManager.EmailSystemAdministrator + "; " + correoContratista },
                            firma = firma
                        });



                        return EnviarResultado(true, "Solicitud", "Mensaje Enviado");

                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                        ex.Message.ToString();
                    }

                }

            }
        }




        #region vendedor

        [AllowAnonymous]
        public ActionResult FirmaAcuerdoVendedor(int id)
        {
            using (var _connection = new ORIONDBEntities())
            {
                ViewBag.fiIDVendedorRepartidor = id;
                return View(id);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> EnviarDocumentoAcuerdoVendedor(int fiIDVendedorRepartidor, string firma)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var _emailTemplateService = new EmailTemplateService();
                        var datosVendedor = _connection.OrionContext.sp_OrionSolicitud_Repartidor_Listar(1).Where(x => x.fiIDVendedorRepartidor == fiIDVendedorRepartidor).FirstOrDefault();
                        var modelCorreo = new SendEmailViewModel();
                        modelCorreo.DestinationEmail = datosVendedor.fcCorreo;


                        await _emailTemplateService.SendEmailToVendedorDocumentoFirmado(new EmailTemplateServiceModel
                        {
                            IdEmailTemplate = 26,
                            CustomerEmail = modelCorreo.DestinationEmail,
                            Comment = "Acuerdo de colaboración.",
                            IdCustomer = datosVendedor.fcIdentidadVendedor,
                            fiIDVendedorRepartidor = datosVendedor.fiIDVendedorRepartidor,
                            List_CC = new List<string> { MemoryLoadManager.EmailSystemGustavo },
                            firma = firma
                        });



                        return EnviarResultado(true, "Acuerdo de colaboración", "Enviado correctamente");

                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                        ex.Message.ToString();
                    }

                }

            }
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult VerificarDocumentoColaboracionAgenteExterno(int fiIDVendedorRepartidor)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var resultQuery = context.sp_Ventas_VerificarDocumentoColaboracionAgenteExterno(fiIDVendedorRepartidor).FirstOrDefault() > 0;

                        if (resultQuery)
                            return EnviarResultado(true, "Documento Colaboración", "Ya se firmó este documento");

                        else
                            return EnviarResultado(false, "Documento Colaboración", "Realizar la firma para acuerdo de colaboración");

                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }
                }
            }
        }

        #endregion


        #region Distribuidor

        [AllowAnonymous]
        public ActionResult FirmaAcuerdoDistribuidor(int id)
        {
            using (var _connection = new ORIONDBEntities())
            {
                ViewBag.fiIDDistribuidor = id;
                return View(id);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> EnviarDocumentoAcuerdoDistribuidor(int fiIDDistribuidor, string firma)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var _emailTemplateService = new EmailTemplateService();
                        var data = _connection.OrionContext.Database.SqlQuery<ListDistribuidorViewModel>("Exec sp_Orion_Ventas_Distribuidor_Listado").Where(x => x.fiIDDistribuidor == fiIDDistribuidor).FirstOrDefault();
                        var modelCorreo = new SendEmailViewModel();
                        modelCorreo.DestinationEmail = data.fcCorreoElectronico;


                        await _emailTemplateService.SendEmailToDistribuidorDocumentoFirmado(new EmailTemplateServiceModel
                        {
                            IdEmailTemplate = 27,
                            CustomerEmail = modelCorreo.DestinationEmail,
                            Comment = "Acuerdo de colaboración.",
                            IdCustomer = data.fcIdentidadRepresentante,
                            fiIDDistribuidor = data.fiIDDistribuidor,
                            // List_CC = new List<string> { MemoryLoadManager.EmailSystemGustavo },
                            firma = firma
                        });



                        return EnviarResultado(true, "Acuerdo de colaboración", "Enviado correctamente");

                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                        //ex.Message.ToString();
                    }

                }

            }
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult VerificarDocumentoColaboracionDistribuidor(int fiIDDistribuidor)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var resultQuery = context.Database.SqlQuery<ListDistribuidorViewModel>("EXEC sp_Orion_Ventas_VerificarDocumentoColaboracionDistribuidor @p0", fiIDDistribuidor).FirstOrDefault();
                        if (resultQuery != null)
                            return EnviarResultado(true, "Documento Colaboración", "Ya se firmó este documento");

                        else
                            return EnviarResultado(false, "Documento Colaboración", "Realizar la firma para acuerdo de colaboración");

                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }
                }
            }
        }

        #endregion


        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> EnviarReporteDebitoAutomarico()
        {
            var Listado = new List<ReporteDebitoAutomatico>();
            var _emailTemplateService = new EmailTemplateService();

            using (var conetion = new ORIONDBEntities())
            {
                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_ReporteDebitoAutomatico";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        Listado = db.ObjectContext.Translate<ReporteDebitoAutomatico>(reader).ToList();
                    }
                    connection.Close();
                }
                if (Listado.Any())
                {
                    await _emailTemplateService.SendEmailReporteDebito(Listado);
                    return EnviarResultado(true, "Reporte", "Reporte enviado");
                }

                return EnviarResultado(false, "Reporte", "Reporte no enviado");
            }
        }



        public static string GeneratePDFAttachment(string pathToOpen, string pathToSave)
        {
            Document doc = new Document();
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
            doc.Close();

            return pathToSave;
        }
        public static string GeneratePDFAttachmentDoc(string pathToOpen, string pathToSave)
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
                //string pathToOpen = Path.Combine(HttpContext.Server.MapPath("~/Documento/Recursos/Plantilla/"), NombreArchivo + ".docx");
                string pathToOpen = MemoryLoadManager.URL + @"\Documento\Recursos\Plantilla\" + NombreArchivo + ".docx";
                var nuevoNombreArchivo = NombreArchivo + "_" + "No." + IdSolicitud + ".pdf";
                var urlToSave = "/Documento/Recursos/Solicitud/" + nuevoNombreArchivo;
                //string pathToSave = HttpContext.Server.MapPath("~" + urlToSave);
                string pathToSave = MemoryLoadManager.URL + urlToSave;
                GeneratePDFAttachment(pathToOpen, pathToSave);
                resultado = urlToSave;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return resultado;

        }
        [HttpPost]
        public string ImprimirDOCPDF(string NombreArchivo)
        {
            var resultado = "";
            try
            {
                string pathToOpen = Path.Combine(HttpContext.Server.MapPath("~/Documento/Recursos/Plantilla/"), NombreArchivo + ".docx");

                var nuevoNombreArchivo = NombreArchivo + "_" + "No." + 0 + ".pdf";
                var urlToSave = "/Documento/Recursos/Solicitud/" + nuevoNombreArchivo;
                string pathToSave = HttpContext.Server.MapPath("~" + urlToSave);
                GeneratePDFAttachmentDoc(pathToOpen, pathToSave);
                resultado = urlToSave;
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return resultado;

        }


        [AllowAnonymous]
        public JsonResult DashBoardInformativo()
        {

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                var listaDetalleInformacionGeneral = new InformacionGeneralViewModel();
                var listaVenta = new List<VentasMesViewModel>();
                var listaMora = new List<MoraMesViewModel>();
                var DatosClientesActivos = new List<VentasClientesActivosViewModel>();
                var DataArpusVentas = new List<ArpusViewModel>();
                var DataIngresos = new List<DatosIngresosViewModel>();

                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_InformacionGerencial {GetIdUser()}";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    listaDetalleInformacionGeneral = db.ObjectContext.Translate<InformacionGeneralViewModel>(reader).FirstOrDefault();
                    reader.NextResult();
                    listaVenta = db.ObjectContext.Translate<VentasMesViewModel>(reader).ToList();
                    reader.NextResult();
                    listaMora = db.ObjectContext.Translate<MoraMesViewModel>(reader).ToList();
                    reader.NextResult();
                    DatosClientesActivos = db.ObjectContext.Translate<VentasClientesActivosViewModel>(reader).ToList();
                    reader.NextResult();
                    DataArpusVentas = db.ObjectContext.Translate<ArpusViewModel>(reader).ToList();
                    reader.NextResult();
                    DataIngresos = db.ObjectContext.Translate<DatosIngresosViewModel>(reader).ToList();
                }
                listaDetalleInformacionGeneral.DataVentas = listaVenta;
                listaDetalleInformacionGeneral.DataMora = listaMora;
                listaDetalleInformacionGeneral.DataClientesActivos = DatosClientesActivos;
                listaDetalleInformacionGeneral.DataArpusVentas = DataArpusVentas;
                listaDetalleInformacionGeneral.DataIngresos = DataIngresos;

                connection.Close();
                return EnviarListaJson(listaDetalleInformacionGeneral);
            }

        }


        [AllowAnonymous]
        public JsonResult GraficaCancelaciones()
        {
            using (var context = new ORIONDBEntities())
            using (var connection = context.Database.Connection)
            {
                List<object> dataGrafica = new List<object>(); // Lista de objetos anónimos

                try
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "EXEC sp_InformacionGerencial_Cancelaciones";
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var row = new
                                    {
                                        fiOrden = reader["fiOrden"],
                                        fcFechaDato = reader["fcFechaDato"],
                                        fiCancelacionTotal = reader["fiCancelacionTotal"],
                                        fiCancelacionServicios = reader["fiCancelacionServicios"],
                                        fiCancelacionProductos = reader["fiCancelacionProductos"],
                                        fiCalculoChurnRate = reader["fiCalculoChurnRate"]
                                    };
                                    dataGrafica.Add(row);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    connection.Close();
                }

                return EnviarListaJson(dataGrafica);
            }
        }

        [AllowAnonymous]
        public JsonResult GraficaValorDeVidaUtil()
        {
            using (var context = new ORIONDBEntities())
            using (var connection = context.Database.Connection)
            {
                List<object> dataGrafica = new List<object>(); // Lista de objetos anónimos

                try
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "EXEC sp_InformacionGerencial_formulaValorVidaUtil";
                        command.CommandType = CommandType.Text;

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    var row = new
                                    {
                                        fiOrden = reader["fiOrden"],
                                        fcFechaDato = reader["fcFechaDato"],
                                        fiCancelacionTotal = reader["fiCancelacionTotal"],
                                        fiCancelacionServicios = reader["fiCancelacionServicios"],
                                        fiCancelacionProductos = reader["fiCancelacionProductos"],
                                        fiCalculoChurnRate = reader["fiCalculoChurnRate"],
                                        fiArpu = reader["fiArpu"],
                                        fiCalculoLTV = reader["fiCalculoLTV"],
                                    };
                                    dataGrafica.Add(row);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
                finally
                {
                    connection.Close();
                }

                return EnviarListaJson(dataGrafica);
            }
        }

        public JsonResult DashBoardInformativoEncabezado()
        {
            var prueba = _connection.OrionContext.sp_OrionSolicitud_Detalle_ClienteListar(GetIdUser(), 37).ToList();

            var etiqu = _connection.OrionContext.sp_DashboardInformativo(1).ToList();

            return EnviarListaJson(etiqu);
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> EnviarCorreoPDFGerencia(HttpPostedFileBase Archivo, int EnviarCorreo)
        {
            try
            {
                if (EnviarCorreo == 1)
                {
                    //SendEmailToGerencia
                    var _emailTemplateService = new EmailTemplateService();
                    var modelCorreo = new EmailTemplateServiceModel();
                    modelCorreo.ListCustomerEmail = GetConfiguracion<string>("Correo_Gerencia", ',');
                    modelCorreo.Archivo = Archivo;
                    await _emailTemplateService.SendEmailToGerencia(modelCorreo);
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return EnviarResultado(true, "");

        }

        [HttpGet]
        [AllowAnonymous]
        public FileContentResult GenerarContratoSolicitudApi(int idSolicitud)
        {
            var contratoService = new ContratoTemplateService();
            var contrato = new MemoryStream();
            var archivo = contratoService.GenerarContrato(idSolicitud, 0);
            archivo.CopyTo(contrato);
            return new FileContentResult(contrato.ToArray(), "application/pdf");

        }
        [HttpGet]
        public ActionResult ModalGenerarOrdenTrabajoContratista(string Nombre, int IDSolicitud, int idCliente)
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


            return PartialView(new SolicitudesViewModel { Nombre = Nombre, IDSolicitud = IDSolicitud, IdCliente = idCliente });
        }
        [HttpPost]
        public async Task<JsonResult> EnviarSolicitudTrabajoContratista(int idSolicitud, string Comentario, int idAgencia, int idAgenciaContratista)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var TipoSolicitudContratista = 2;
                        var _emailTemplateService = new EmailTemplateService();

                        //var result = context.sp_OrionSolicitud_ContratistaSolicitudInstalacio__Insertar(GetIdUser(), model.IdCliente, model.IDSolicitud, idAgencia, idAgenciaContratista, Comentario, "", "",2,"", null);

                        var datos = context.sp_OrionContratista_DetalleBySolicitud(GetIdUser(), idSolicitud, 0, 0).FirstOrDefault();
                        var result = context.sp_OrionSolicitud_ContratistaSolicitudInstalacio__Insertar(GetIdUser(), datos.fiIDEquifax, idSolicitud, idAgencia, idAgenciaContratista, Comentario, datos.fcCodigoCliente, datos.fcNumeroOrdenCfeus, 3, "", null,0);
                        var datosCliente = context.sp_OrionContratista_DetalleBySolicitud(GetIdUser(), idSolicitud, 0, 0).FirstOrDefault();
                        // var Correo = "aebautista63@gmail.com";
                        var modelCorreo = new SendEmailViewModel();
                        var CuerpoComentarioCorreo = "Estimado : " + datosCliente.fcNombreEmpresa + " , se le notifica la solicitud de trabajo(SOP) por reportes para el cliente: " + datos.fcNombre + "<br/>" + " comentario:" + Comentario;
                        modelCorreo.DestinationEmail = datosCliente.fcCorreoElectronico;

                        var idCliente = Convert.ToInt16(datos.fiIDEquifax);
                        await _emailTemplateService.SendEmailToCustomer(new EmailTemplateServiceModel
                        {
                            IdEmailTemplate = 21,
                            CustomerEmail = modelCorreo.DestinationEmail,
                            //CustomerEmail = Correo,
                            Comment = "Solicitud de contratista(SOP).",
                            IDSolicitud = idSolicitud,
                            IdCliente = idCliente,
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
        [HttpPost]
        public ActionResult RealizarCorteAdministrativo(string fcIdPrestamo)
        {
            try
            {
                using (var context = new ORIONDBEntities())
                {
                    var connection = (SqlConnection)context.Database.Connection;
                    var command = new SqlCommand("Orion.dbo.sp_Servicios_CambiodeEstadoConexion", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@piIDUsuario", GetIdUser());
                    command.Parameters.AddWithValue("@pcIDPrestamo", fcIdPrestamo);
                    command.Parameters.AddWithValue("@piIDEstadoServicio", "3");

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

        public JsonResult ReiniciarReferencia(int fiIdReferencia)
        {
            try
            {
                using (var context = new ORIONDBEntities())
                {
                    var datos = context.sp_Reiniciar_Referencia(fiIdReferencia, GetIdUser()).FirstOrDefault();
                    return EnviarListaJson(datos);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        #region Graficos Reporte General

        [AllowAnonymous]
        public JsonResult GraficaVentas()
        {
            var ventasAnuales = new List<VentaAnual>();

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_InformacionGerencial_GraficaVentas {GetIdUser()}";

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Asumiendo que la columna se llama "VentasPorAnio"
                        string jsonData = reader["VentasPorAnio"].ToString();

                        // Deserializamos el JSON a la lista de modelos
                        ventasAnuales = System.Text.Json.JsonSerializer.Deserialize<List<VentaAnual>>(jsonData, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                }

                connection.Close();
            }

            return Json(ventasAnuales, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        public JsonResult GraficaArpu()
        {
            var arpusAnuales = new List<ArpuResponse>();

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_InformacionGerencial_GraficaArpu {GetIdUser()}";

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Asumiendo que la columna se llama "DataArpusVentas"
                        string jsonData = reader["DataArpusVentas"].ToString();

                        // Deserializamos el JSON a la lista de modelos
                        arpusAnuales = System.Text.Json.JsonSerializer.Deserialize<List<ArpuResponse>>(jsonData, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                }

                connection.Close();
            }

            return Json(arpusAnuales, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GraficaMora()
        {
            var dataMora = new List<DataMoraResponse>();

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_InformacionGerencial_GraficaMora {GetIdUser()}";

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Asumiendo que la columna se llama "DataMora"
                        string jsonData = reader["DataMora"].ToString();

                        // Deserializamos el JSON a la lista de modelos
                        dataMora = System.Text.Json.JsonSerializer.Deserialize<List<DataMoraResponse>>(jsonData, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                }

                connection.Close();
            }

            return Json(dataMora, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult GraficaClientesActivos(int piMeses = 12)
        {
            var dataClientes = new List<VentasClientesActivosViewModel>();

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_InformacionGerencial_ClientesActivos {piMeses},{GetIdUser()}";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dataClientes.Add(new VentasClientesActivosViewModel
                        {
                            fiAno = reader.GetInt32(reader.GetOrdinal("fiAno")),
                            fiMes = reader.GetInt32(reader.GetOrdinal("fiMes")),
                            fcMes = reader.GetString(reader.GetOrdinal("fcMes")),
                            fiConteoVentas = reader.GetInt32(reader.GetOrdinal("fiConteoVentas")),
                            fnMontoVentas = reader.GetDecimal(reader.GetOrdinal("fnMontoVentas")),
                            fiClientesCancelados = reader.GetInt32(reader.GetOrdinal("fiClientesCancelados"))
                        });
                    }
                }

                connection.Close();
            }

            return Json(dataClientes, JsonRequestBehavior.AllowGet);
        }



        [AllowAnonymous]
        public JsonResult GraficaComparativaPaquetes(int piMeses = 12)
        {
            var dataClientes = new List<GraficoComparativaPaquetes>();

            using (var context = new ORIONDBEntities())
            {
                var connection = context.Database.Connection;
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_InformacionGerencial_PaquetesServicioyProducto {piMeses},{GetIdUser()}";

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var jsonData = reader["DataComparativa"].ToString();
                        dataClientes = System.Text.Json.JsonSerializer.Deserialize<List<GraficoComparativaPaquetes>>(jsonData, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                }

                connection.Close();
            }

            return Json(dataClientes, JsonRequestBehavior.AllowGet);
        }


        #endregion


    }
}