using Newtonsoft.Json;
using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.App_Services.EmailService;
using OrionCoreCableColor.DbConnection.CMContext;
using OrionCoreCableColor.DbConnection.CoreFinancieroDB;
using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models.Base;
using OrionCoreCableColor.Models.DatosCliente;
using OrionCoreCableColor.Models.EmailTemplateService;
using OrionCoreCableColor.Models.Mantenimiento;
using OrionCoreCableColor.Models.Precalificado;
using OrionCoreCableColor.Models.Prestamo;
using OrionCoreCableColor.Models.Productos;
using OrionCoreCableColor.Models.Reportes;
using OrionCoreCableColor.Models.Roles;
using OrionCoreCableColor.Models.Solicitudes;
using OrionCoreCableColor.Models.Usuario;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.Controllers
{
    [Authorize]
    public class PrecalificaClienteController : BaseController
    {
        private readonly DbServiceConnection _connection;
        public PrecalificaClienteController()
        {
            var ConnectionString = ConfigurationManager.ConnectionStrings["OrionConnections"].ConnectionString;
            _connection = new DbServiceConnection(ConnectionString);
        }

        // GET: PrecalificaCliente
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult BuscarCliente()
        {
            var model = new datosclienteViewModel();
            return View(model);
        }
        public ActionResult VendedorExterno()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ClienteSolicitudServicio(int IDCliente)
        {
            var listaSolicitudes = new List<SolicitudesCliente_ViewModel>();
            var model = new SolicitudesViewModel();
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {

                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_OrionSolicitud_SolicitudesCliente_PorIDCliente {GetIdUser()} , {IDCliente}";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    listaSolicitudes = db.ObjectContext.Translate<SolicitudesCliente_ViewModel>(reader).ToList();

                }

            }
            model.ListaSolicitudCliente = listaSolicitudes;
            return PartialView(model);
        }

        public ActionResult BandejaComisionesVendedor()
        {
            using (var db = new ORIONDBEntities())
            {
                var datos = db.Database.SqlQuery<MesAnioVenta>("EXEC [dbo].[sp_ObtenerMesesAniosVentas]").ToList();
                bool visualizacionAcciones = false;

                var Acciones_ComisionesVendedores = GetConfiguracion<int>("Acciones_ComisionesVendedores", ',');
                var RolesAdmin = GetConfiguracion<int>("Orion_Admin", ',');

                if (Acciones_ComisionesVendedores.Contains(GetIdUser()) || RolesAdmin.Contains(GetUser().IdRol))
                {
                    visualizacionAcciones = true;
                }


                ViewBag.Anios = datos.Select(d => d.Anio).Distinct().OrderBy(a => a).ToList();

                ViewBag.MesesPorAnio = datos
                    .GroupBy(d => d.Anio)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(m => new { Mes = m.Mes, NombreMes = m.NombreMes }).ToList()
                    );

                ViewBag.VisualizacionAcciones = visualizacionAcciones;
            }

            return View();
        }

        public ActionResult Bandeja_ClienteFinales()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult ConsultarCliente(datosclienteViewModel model)
        {
            try
            {

                using (var context = new OrionSecurityEntities())
                {
                    var EstadoPrecalificado = new EstadoPrecalificadosViewModel();
                    var usuario = context.Usuarios.FirstOrDefault(x => x.fcNombreCorto == User.Identity.Name);

                    using (var connection = (new ORIONDBEntities()).Database.Connection)
                    {

                        connection.Open();
                        var command = connection.CreateCommand();
                        command.CommandText = $@"EXEC sp_precalificado_ValidarTiempoEstado '{model.fcIdentidad}'";
                        using (var reader = command.ExecuteReader())
                        {
                            var db = ((IObjectContextAdapter)new ORIONDBEntities());
                            EstadoPrecalificado = db.ObjectContext.Translate<EstadoPrecalificadosViewModel>(reader).FirstOrDefault();

                        }


                    }

                    var get = new HttpClientManager();
                    if (EstadoPrecalificado != null)
                    {
                        if (EstadoPrecalificado.fiDias > 60 || EstadoPrecalificado.fiEstadoActualPrecalificado == 0)
                        {
                            var Data = DataCrypt.Encriptar("usr=" + GetIdUser() + "&ID=" + model.fcIdentidad);
                            //var URL = "http://172.20.3.144/WSOrion/WSProcesarCliente.aspx?" + Data;
                            var URL = MemoryLoadManager.UrlEquifax + Data;
                            HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
                            request.Accept = "text/xml";
                            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                            WebHeaderCollection header = response.Headers;
                            var encoding = ASCIIEncoding.ASCII;

                            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                            {
                                string responseText = reader.ReadToEnd();
                            }
                            using (var connection = (new ORIONDBEntities()).Database.Connection)
                            {

                                connection.Open();
                                var command = connection.CreateCommand();
                                command.CommandText = $@"EXEC sp_MatrizNueva '{model.fcIdentidad}'";
                                using (var reader = command.ExecuteReader())
                                {

                                }

                            }

                        }
                    }
                    else
                    {
                        var Data = DataCrypt.Encriptar("usr=" + GetIdUser() + "&ID=" + model.fcIdentidad);
                        //var URL = "http://172.20.3.144/WSOrion/WSProcesarCliente.aspx?" + Data;
                        var URL = MemoryLoadManager.UrlEquifax + Data;
                        HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
                        request.Accept = "text/xml";
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        WebHeaderCollection header = response.Headers;
                        var encoding = ASCIIEncoding.ASCII;

                        using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                        {
                            string responseText = reader.ReadToEnd();
                        }
                        using (var connection = (new ORIONDBEntities()).Database.Connection)
                        {

                            connection.Open();
                            var command = connection.CreateCommand();
                            command.CommandText = $@"EXEC sp_MatrizNueva '{model.fcIdentidad}'";
                            using (var reader = command.ExecuteReader())
                            {

                            }

                        }
                    }


                    using (var Dbcontext = new ORIONDBEntities())
                    {
                        var result = Dbcontext.sp_info_ConsultaAnalistas(101, GetIdUser(), model.fcIdentidad).FirstOrDefault();

                        return EnviarListaJson(result);
                    }



                    // get.GetResult<>
                }

            }
            catch (Exception e)
            {
                return EnviarException(e, "Repo Request Send Exception");
            }

        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult ConsultarClientePorIdentidad(string fcIdentidad, int fiIDUsuario)
        {
            try
            {

                using (var context = new OrionSecurityEntities())
                {
                    var EstadoPrecalificado = new EstadoPrecalificadosViewModel();


                    using (var connection = (new ORIONDBEntities()).Database.Connection)
                    {

                        connection.Open();
                        var command = connection.CreateCommand();
                        command.CommandText = $@"EXEC sp_precalificado_ValidarTiempoEstado '{fcIdentidad}'";
                        using (var reader = command.ExecuteReader())
                        {
                            var db = ((IObjectContextAdapter)new ORIONDBEntities());
                            EstadoPrecalificado = db.ObjectContext.Translate<EstadoPrecalificadosViewModel>(reader).FirstOrDefault();

                        }


                    }

                    var get = new HttpClientManager();
                    if (EstadoPrecalificado != null)
                    {
                        if (EstadoPrecalificado.fiDias > 60 || EstadoPrecalificado.fiEstadoActualPrecalificado == 0)
                        {
                            var Data = DataCrypt.Encriptar("usr=" + fiIDUsuario + "&ID=" + fcIdentidad);
                            //var URL = "http://172.20.3.144/WSOrion/WSProcesarCliente.aspx?" + Data;
                            var URL = MemoryLoadManager.UrlEquifax + Data;
                            HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
                            request.Accept = "text/xml";
                            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                            WebHeaderCollection header = response.Headers;
                            var encoding = ASCIIEncoding.ASCII;

                            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                            {
                                string responseText = reader.ReadToEnd();
                            }
                            using (var connection = (new ORIONDBEntities()).Database.Connection)
                            {

                                connection.Open();
                                var command = connection.CreateCommand();
                                command.CommandText = $@"EXEC sp_MatrizNueva '{fcIdentidad}'";
                                using (var reader = command.ExecuteReader())
                                {

                                }

                            }

                        }
                    }
                    else
                    {
                        var Data = DataCrypt.Encriptar("usr=" + fiIDUsuario + "&ID=" + fcIdentidad);
                        //var URL = "http://172.20.3.144/WSOrion/WSProcesarCliente.aspx?" + Data;
                        var URL = MemoryLoadManager.UrlEquifax + Data;
                        HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
                        request.Accept = "text/xml";
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        WebHeaderCollection header = response.Headers;
                        var encoding = ASCIIEncoding.ASCII;

                        using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                        {
                            string responseText = reader.ReadToEnd();
                        }
                        using (var connection = (new ORIONDBEntities()).Database.Connection)
                        {

                            connection.Open();
                            var command = connection.CreateCommand();
                            command.CommandText = $@"EXEC sp_MatrizNueva '{fcIdentidad}'";
                            using (var reader = command.ExecuteReader())
                            {

                            }

                        }
                    }



                    using (var Dbcontext = new ORIONDBEntities())
                    {
                        var result = Dbcontext.sp_info_ConsultaAnalistas(101, fiIDUsuario, fcIdentidad).FirstOrDefault();

                        return EnviarListaJson(result);
                    }


                    // get.GetResult<>
                }

            }
            catch (Exception e)
            {
                return EnviarException(e, "Repo Request Send Exception");
            }

        }

        [HttpPost]
        public JsonResult ConsultarClientePrecalificado(datosclienteViewModel model)
        {
            try
            {
                using (var context = new OrionSecurityEntities())
                {
                    var usuario = context.Usuarios.FirstOrDefault(x => x.fcNombreCorto == User.Identity.Name);


                    using (var Dbcontext = new ORIONDBEntities())
                    {
                        var result = Dbcontext.sp_info_ConsultaAnalistasValidadoCredito(101, GetIdUser(), model.fcIdentidad).FirstOrDefault();

                        return EnviarListaJson(result);
                    }



                    // get.GetResult<>
                }

            }
            catch (Exception e)
            {
                return EnviarException(e, "Repo Request Send Exception");
            }

        }

        public JsonResult CargarListaProducto()
        {
            try
            {
                using (var Dbcontext = new ORIONDBEntities())

                {
                    return EnviarListaJson(Dbcontext.sp_Productos_Propuesta(1, "").ToList());
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        //datosclienteViewModel
        [HttpPost]
        public JsonResult EnviarLinkCliente(datosclienteViewModel model, int condicionMes, string fcPlazo, decimal ValorPlazo, int fiIDSolicitudDireccion, bool fbClienteExiste = false, int opcionselectdireccion = 0)
        {
            try
            {

                var NumeroTelefono = model.fcTelefonoReferencia1;
                NumeroTelefono = NumeroTelefono.Replace("-", "");

                using (var Dbcontext = new ORIONDBEntities())
                {
                    if (fbClienteExiste == true)
                    {
                        //var ResultadoDatos = 1;
                        var PlazoConntrato = model.DatosProductos.FirstOrDefault().fiIDPlazo;
                        var ResultadoDatos = Convert.ToInt32(Dbcontext.sp_OrionSolicitud_Mestro_NuevoServicio_Insertar(model.fiIDEquifax, model.fcDireccionDetallada, " ", model.fiDepartamento, model.fiMunicipio, 0, model.fiColonia, 0, 0, GetIdUser(), model.fcCorreo, opcionselectdireccion, condicionMes, fiIDSolicitudDireccion, PlazoConntrato).FirstOrDefault());
                        if (ResultadoDatos > 0)
                        {
                            foreach (var item in model.DatosProductos)
                            {
                                Dbcontext.sp_Equifax_GarantiaNuevoServicio_Guardar(model.fiIDEquifax, model.fcIdentidad, item.fiIDProducto, item.fnValorProductoMN, item.fnValorProductoME, item.fnPorcentajeImpuesto1, item.fnPorcentajeImpuesto2, GetIdUser(), item.fnValorCuotaMensual, item.fiIDPaquete, ResultadoDatos, condicionMes, item.fiIDPlazo, item.fnValorPaquete);
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in model.DatosProductos)
                        {
                            Dbcontext.sp_Equifax_Garantia_Guardar(model.fiIDEquifax, model.fcIdentidad, item.fiIDProducto, item.fnValorProductoMN, item.fnValorProductoME, item.fnPorcentajeImpuesto1, item.fnPorcentajeImpuesto2, GetIdUser(), item.fnValorCuotaMensual, item.fiIDPaquete, 0, condicionMes, item.fiIDPlazo, item.fnValorPaquete);
                        }
                        MensajeriaApi.EnviarNumero(model.fcPrimerNombre, model.fiIDEquifax, NumeroTelefono);
                        //MensajeDeTexto.EnviarNumero(model.fcPrimerNombre, model.fiIDEquifax, NumeroTelefono);
                    }
                    var result = Dbcontext.sp_Equifax_Cliente_Actualizar_Telefono(model.fiIDEquifax, NumeroTelefono) > 0;

                    // var result = true;
                    return EnviarResultado(result, "Actualizar", "Se envio mensaje");
                }

            }
            catch (Exception e)
            {
                return EnviarException(e, "Repo Request Send Exception");
            }
        }

        [HttpGet]
        public ActionResult ViewDataProducto()
        {

            return PartialView();

        }

        [HttpGet]
        public ActionResult ViewDataPrestamosCliente(string fcIdentidad)
        {

            return PartialView("ViewDataPrestamosCliente", fcIdentidad);

        }

        [HttpGet]
        public ActionResult ViewDataBitacoraClienteConsulta(string fcIdentidad)
        {

            return PartialView("ViewDataBitacoraClienteConsulta", fcIdentidad);

        }

        [HttpGet]
        public ActionResult BitacoraClientePorIdentidad(string fcIdentidad)
        {

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {

                connection.Open();
                var list = new List<BitacoraGestionCliente>();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_BasesCallCenter_Bitacoras_Identidad '{fcIdentidad}'";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new CoreFinancieroEntities2());
                    list = db.ObjectContext.Translate<BitacoraGestionCliente>(reader).ToList();
                }



                connection.Close();

                return Json(list, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public ActionResult PrestamosClientePorIdentidad(string fcIdentidad)
        {

            using (var connection = (new CoreFinancieroEntities2()).Database.Connection)
            {

                connection.Open();
                var list = new List<PrestamosIdentidadViewModel>();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_Prestamos_PorIdentidad '{fcIdentidad}'";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new CoreFinancieroEntities2());
                    list = db.ObjectContext.Translate<PrestamosIdentidadViewModel>(reader).ToList();
                }



                connection.Close();

                return Json(list, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpGet]
        public ActionResult ViewDataPaquete(string Identidad)
        {

            return PartialView("ViewDataPaquete", Identidad);

        }

        [HttpGet]
        public async Task<ActionResult> ViewDataPaqueteGeneral(string fcIdentidad)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var resultado = await contexto.Database.SqlQuery<ListCatalogoPlazos>("Exec sp_Catalogo_Plazos_Listar").ToListAsync();
                var Estatus = contexto.sp_Estatus_Precalificado(fcIdentidad).FirstOrDefault();

                var listTipoSolicitudes = await contexto.Database.SqlQuery<ListTipoSolicitudViewModel>("Exec sp_ListarTipoSolicitud").ToListAsync();

                ViewBag.listadoTipoSolicitudes = listTipoSolicitudes.Select(x => new { id = x.fiIDTipoSolicitud, text = x.fcTipoSolicitud }).ToList();
                ViewBag.idSolicitudesDireccionDiferente = GetConfiguracion<int>("TiposSolicitudNuevasDirecciones", ',');
                var valor = 0;

                if (Estatus == 1)
                {
                    valor = 6000;
                }
                if (Estatus == 2)
                {
                    valor = 5000;
                }
                if (Estatus == 3)
                {
                    valor = 4000;
                }
                if (Estatus == 4)
                {
                    valor = 3000;
                }
                if (Estatus == 5)
                {
                    valor = 2000;
                }
                if (Estatus == 6 || Estatus == 7)
                {
                    valor = 0;

                }

                ViewBag.ListaPlazos = resultado;
                ViewBag.Valor = valor;



                return PartialView();
            }



        }


        /// <summary>
        /// Obtener Departamentos para Listado de Vista Buscar Clientes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetDepartamentos()
        {
            try
            {
                using (var contexto = new ORIONDBEntities())
                {
                    var departamentos = await contexto.Database.SqlQuery<GeoViewModel>(
                        "Exec sp_geo_ListaDepartamentos"
                    ).ToListAsync();


                    return EnviarListaJson(departamentos);
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw;
            }

        }

        /// <summary>
        /// Obtener Municipios para Listado de Vista Buscar Clientes, filtrados por Departamento
        /// </summary>
        /// <param name="fiCodDepartamento">Código del Departamento</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetMunicipios(int fiCodDepartamento)
        {
            try
            {
                using (var contexto = new ORIONDBEntities())
                {
                    var municipios = await contexto.Database.SqlQuery<GeoViewModel>(
                        "Exec sp_Listado_Geo_Municipios").ToListAsync();

                    return EnviarListaJson(municipios.Where(x => x.fiCodDepartamento == fiCodDepartamento));
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw;
            }

        }

        /// <summary>
        /// Obtener Barrios para Listado de Vista Buscar Clientes, filtrados por Departamento y Municipio
        /// </summary>
        /// <param name="fiCodDepartamento">Código del Departamento</param>
        /// <param name="fiCodMunicipio">Código del Municipio</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetBarrio(int fiCodDepartamento, int fiCodMunicipio)
        {
            try
            {
                using (var contexto = new ORIONDBEntities())
                {
                    var barrios = await contexto.Database.SqlQuery<GeoViewModel>(
                        "Exec sp_Geo_Barrio_Listado").ToListAsync();

                    return EnviarListaJson(barrios.Where(x => x.fiCodDepartamento == fiCodDepartamento && x.fiCodMunicipio == fiCodMunicipio));
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                throw;
            }

        }



        [HttpPost]
        public ActionResult ModalConfirmarDatosProcesoSolicitud
        (
            datosclienteViewModel model,
            decimal ValorPlazo,
            int condicionMes,
            decimal ValorCondicion,
            string fcPlazo,
            string fcPaquete,
            //utilizar para buscar los id de geo para la nueva solicitud
            int fiIDSolicitudDireccion = 0
        )
        {

            ViewBag.ValorPlazo = ValorPlazo;
            ViewBag.condicionMes = condicionMes;
            ViewBag.ValorCondicion = ValorCondicion;
            ViewBag.fcPlazo = fcPlazo;
            ViewBag.fcPaquete = fcPaquete;
            ViewBag.fiIDSolicitudDireccion = fiIDSolicitudDireccion;

            return PartialView(model);
        }
        [HttpGet]
        public ActionResult ViewVerBuro(string IDIDentidad)
        {
            // usr = 1 & ID = 0801197809215 & IDApp = 107
            var parametros = $"usr=1&ID={IDIDentidad}&IDApp=107";
            var modelToSend = new SolicitudesViewModel
            {
                Identidad = IDIDentidad,
                ParametroEncriptado = DataCrypt.Encriptar(parametros)
            };
            return PartialView(modelToSend);
        }

        [HttpGet]
        public ActionResult ViewInfoPrecalificado(string IDIDentidad, string nombre)
        {

            var modelToSend = new SolicitudesViewModel
            {
                Identidad = IDIDentidad,
                Nombre = nombre
            };
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {

                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_RangoLaboral_lista";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    var List = db.ObjectContext.Translate<RangoLaboralViewModel>(reader).ToList();
                    ViewBag.ListaLaboral = List.Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDRangoLaboral), Text = z.fcRangoLaboral }).ToList();

                }

                connection.Close();

                using (var context = new ORIONDBEntities())
                {
                    //  idPaquete.ForEach(x => { x.Seleccionado = true; });
                    var UsuariosComayagua = GetConfiguracion<int>("Usuarios_Comayagua", ',');
                    var PaquetesComayagua = GetConfiguracion<int>("Paquetes_Comayagua", ',');

                    var listProductos = context.sp_Paquetes_List().Where(x => x.fiestadoPaquete == 1 && x.fbSoloServicio == false).ToList();
                    var listaPaquete = new List<DetallePaqueteViewModel>();

                    var UserID = GetIdUser();

                    if (!UsuariosComayagua.Any(x => x == UserID))
                    {
                        listProductos = listProductos.Where(x => !PaquetesComayagua.Any(y => y == x.fiIDPaquete)).ToList();
                    }


                    foreach (var item in listProductos)
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
                            fiTieneProducto = item.fiTieneProducto

                        });

                    }

                    ViewBag.ListaPaquetes = listaPaquete.Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDPaquete), Text = (z.fcPaquete + "----L." + z.fnValorMensual) }).ToList();

                }

            }
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {


                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_RangoSalarial_lista";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    var List = db.ObjectContext.Translate<RangoSalarioViewModel>(reader).ToList();
                    ViewBag.ListaSalario = List.Select(z => new SelectListItem { Value = Convert.ToString(z.fiRangoSalario), Text = z.fcRangoSalario }).ToList();
                }
                connection.Close();
            }

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {


                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_TipoVivienda_Lista";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    var List = db.ObjectContext.Translate<TipoViviendaViewModel>(reader).ToList();
                    ViewBag.ListaVivienda = List.Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDTipoVivienda), Text = z.fcDescripcionTipoVivienda }).ToList();
                }
                connection.Close();
            }

            return PartialView(modelToSend);

        }

        [HttpGet]
        public ActionResult ViewPrecalificadoValidadoCredito(string identidad)
        {
            var model = new datosclienteViewModel();
            model.fcIdentidad = identidad;
            return View(model);

        }

        [HttpPost]
        public JsonResult GuardarDatosPrecalificado(string Identidad, string Trabajo, string Puesto, int RangoLaboral, int RangoSalarial, int TipoVivienda, string DireccionTrabajo, string DireccionResidencia, int Paquete, string InfoAdicional)
        {
            try
            {
                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $@"EXEC sp_InfoPrecalifiacdo_Validacion_Insertar '{Identidad}','{Trabajo}','{Puesto}',{RangoLaboral},{RangoSalarial},'{DireccionResidencia}','{DireccionTrabajo}',{TipoVivienda},{Paquete},'{InfoAdicional}'";
                    using (var reader = command.ExecuteReader())
                    {
                        return EnviarResultado(true, "Datos Precalificado", "Se guardo Exitosamente");
                    }

                    return EnviarResultado(false, "Datos Precalificado", "Error en el Guardado");

                }

            }
            catch (Exception e)
            {
                return EnviarException(e, "Repo Request Send Exception");
            }
        }

        public JsonResult CargarProducto()
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

        public JsonResult CargarPaquete(string Identidad)
        {
            try
            {
                using (var context = new ORIONDBEntities())
                {
                    //  idPaquete.ForEach(x => { x.Seleccionado = true; });
                    var UsuariosComayagua = GetConfiguracion<int>("Usuarios_Comayagua", ',');
                    var PaquetesComayagua = GetConfiguracion<int>("Paquetes_Comayagua", ',');

                    var estatus = context.sp_Estatus_Precalificado(Identidad).FirstOrDefault();
                    var listServicios = context.sp_Paquetes_List().Where(x => x.fiestadoPaquete == 1 && x.fiTieneProducto == 0).ToList();
                    var listProductos = context.sp_Paquetes_List().Where(x => x.fiestadoPaquete == 1 && x.fiTieneProducto == 1).ToList();
                    var listaPaquete = new List<DetallePaqueteViewModel>();

                    var UserID = GetIdUser();

                    if (!UsuariosComayagua.Any(x => x == UserID))
                    {
                        listServicios = listServicios.Where(x => !PaquetesComayagua.Any(y => y == x.fiIDPaquete)).ToList();
                        listProductos = listProductos.Where(x => !PaquetesComayagua.Any(y => y == x.fiIDPaquete)).ToList();
                    }



                    if (estatus == 2)
                    {
                        listProductos = listProductos.Where(x => x.fnValorMensual <= 4000).ToList();
                    }
                    if (estatus == 3)
                    {
                        listProductos = listProductos.Where(x => x.fnValorMensual <= 3000).ToList();
                    }
                    if (estatus == 4)
                    {
                        listProductos = listProductos.Where(x => x.fnValorMensual <= 2500).ToList();
                    }
                    if (estatus == 5)
                    {
                        listProductos = listProductos.Where(x => x.fnValorMensual <= 1500).ToList();
                    }
                    if (estatus == 6 || estatus == 7)
                    {
                        listProductos = listProductos.Where(x => x.fnValorMensual <= 0).ToList();
                    }

                    foreach (var item in listProductos)
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
                            fiTieneProducto = item.fiTieneProducto

                        });

                    }

                    foreach (var item in listServicios)
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
                            fiTieneProducto = item.fiTieneProducto
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

        public async Task<JsonResult> CargarPaqueteGeneral()
        {
            try
            {
                using (var context = new ORIONDBEntities())
                {

                    var resultado = await context.Database.SqlQuery<string>("EXEC sp_PlazosPorPaquete_Listar").ToListAsync();

                    return Json(resultado, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception e)
            {
                return null;
            }
        }

        public JsonResult CargarDatosVendedorComisiones(string fechaInicio, string fechaFin)
        {
            var listaDetalleDatos = new List<BandejaComisionesVendedor>();
            try
            {

                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {
                    connection.Open();
                    var command = connection.CreateCommand();

                    var fechaInicioParam = string.IsNullOrEmpty(fechaInicio) ? DateTime.Now.ToString("yyyy-MM-01") : fechaInicio;
                    var fechaFinParam = string.IsNullOrEmpty(fechaFin) ? DateTime.Now.ToString("yyyy-MM-dd") : fechaFin;

                    command.CommandText = $"EXEC sp_BandejaComisionesNovanet_Calculos {GetIdUser()}, {GetUser().IdRol}, '{fechaInicioParam}', '{fechaFinParam}'";

                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        listaDetalleDatos = db.ObjectContext.Translate<BandejaComisionesVendedor>(reader).ToList();
                    }

                    connection.Close();
                }

                return EnviarListaJson(listaDetalleDatos);
            }
            catch (Exception e)
            {
                return EnviarListaJson(listaDetalleDatos);
            }
        }

        public JsonResult CargarDatosVendedorComisionesPorVendedor(string nameUser, string mes = null, string anio = null)
        {
            var idUsuario = GetIdUserForName(nameUser);
            var listaDetalleDatos = new List<CalculoComisionesNovanetModel>();

            DateTime? fechaInicio = null;
            DateTime? fechaFin = null;

            if (!string.IsNullOrEmpty(mes) && !string.IsNullOrEmpty(anio))
            {
                int mesInt = int.Parse(mes);
                int anioInt = int.Parse(anio);

                fechaInicio = new DateTime(anioInt, mesInt, 1);
                fechaFin = new DateTime(anioInt, mesInt, DateTime.DaysInMonth(anioInt, mesInt));
            }

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();
                var command = connection.CreateCommand();

                string fechaInicioParam = fechaInicio.HasValue ? $"'{fechaInicio.Value:yyyy-MM-dd}'" : "NULL";
                string fechaFinParam = fechaFin.HasValue ? $"'{fechaFin.Value:yyyy-MM-dd}'" : "NULL";

                command.CommandText = $"EXEC sp_CalculoComisionesNovanet_PorUsuario {idUsuario}, {GetUser().IdRol}, {fechaInicioParam}, {fechaFinParam}";

                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    listaDetalleDatos = db.ObjectContext.Translate<CalculoComisionesNovanetModel>(reader).ToList();
                }

                connection.Close();
            }

            return EnviarListaJson(listaDetalleDatos);
        }

        public JsonResult CargarEstadoCuentaVendedor(string fechaInicio, string fechaFin)
        {

            var listaDetalleEstadoCuenteUsuario = new List<EstadoCuentaUsuarioVendedor>();

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {

                connection.Open();
                var command = connection.CreateCommand();

                var fechaInicioParam = string.IsNullOrEmpty(fechaInicio) ? DateTime.Now.ToString("yyyy-MM-01") : fechaInicio;
                var fechaFinParam = string.IsNullOrEmpty(fechaFin) ? DateTime.Now.ToString("yyyy-MM-dd") : fechaFin;
                command.CommandText = $"EXEC sp_BandejaComisionesNovanet {GetIdUser()}, '{fechaInicioParam}', '{fechaFinParam}'";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    listaDetalleEstadoCuenteUsuario = db.ObjectContext.Translate<EstadoCuentaUsuarioVendedor>(reader).ToList();
                    reader.NextResult();
                    //listaReferencias = db.ObjectContext.Translate<SolicitudesReferenciaViewModel>(reader).ToList();

                }

                connection.Close();
                return EnviarListaJson(listaDetalleEstadoCuenteUsuario);
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

        public ActionResult PrecalificadosListado()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ListadoPrecalificadosBandeja()
        {

            using (var conetion = new ORIONDBEntities())
            {
                var lista = EnviarListaJson(conetion.sp_Precalificado_Bandeja_Agentes(GetIdUser()).ToList());
                //EnviarNotificacion("DESDE BASE SOLICITUDES");
                return lista;
            }

        }

        [AllowAnonymous]
        [HttpGet]
        public JsonResult ListadoPaquetesAplica(string fcIdentidad)
        {
            try
            {
                using (var context = new ORIONDBEntities())
                {
                    var UsuariosComayagua = GetConfiguracion<int>("Usuarios_Comayagua", ',');
                    var PaquetesComayagua = GetConfiguracion<int>("Paquetes_Comayagua", ',');

                    var estatus = context.sp_Estatus_Precalificado(fcIdentidad).FirstOrDefault();
                    var listServicios = context.sp_Paquetes_List().Where(x => x.fiestadoPaquete == 1 && x.fiTieneProducto == 0).ToList();
                    var listProductos = context.sp_Paquetes_List().Where(x => x.fiestadoPaquete == 1 && x.fiTieneProducto == 1).ToList();
                    var listaPaquete = new List<DetallePaqueteViewModel>();

                    var UserID = GetIdUser();

                    if (!UsuariosComayagua.Any(x => x == UserID))
                    {
                        listServicios = listServicios.Where(x => !PaquetesComayagua.Any(y => y == x.fiIDPaquete)).ToList();
                        listProductos = listProductos.Where(x => !PaquetesComayagua.Any(y => y == x.fiIDPaquete)).ToList();
                    }







                    if (estatus == 2)
                    {
                        listProductos = listProductos.Where(x => x.fnValorMensual <= 4000).ToList();
                    }
                    if (estatus == 3)
                    {
                        listProductos = listProductos.Where(x => x.fnValorMensual <= 3000).ToList();
                    }
                    if (estatus == 4)
                    {
                        listProductos = listProductos.Where(x => x.fnValorMensual <= 2500).ToList();
                    }
                    if (estatus == 5)
                    {
                        listProductos = listProductos.Where(x => x.fnValorMensual <= 1500).ToList();
                    }
                    if (estatus == 6 || estatus == 7)
                    {
                        listProductos = listProductos.Where(x => x.fnValorMensual <= 0).ToList();
                    }

                    foreach (var item in listProductos)
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
                            fiTieneProducto = item.fiTieneProducto

                        });

                    }

                    foreach (var item in listServicios)
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
                            fiTieneProducto = item.fiTieneProducto
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

        [HttpGet]
        public ActionResult ViewActualizarCondicion(int InfoPrecalificado)
        {
            ViewBag.fiIDinfoPrecalificado = InfoPrecalificado;
            return PartialView();

        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult EnviarLinkClienteMovil(datosclienteViewModel model, int condicionMes, string fcPlazo, decimal ValorPlazo, int fiIDSolicitudDireccion, int fiIDUser, bool fbClienteExiste = false, int opcionselectdireccion = 0)
        {
            try
            {
                var NumeroTelefono = model.fcTelefonoReferencia1;
                NumeroTelefono = NumeroTelefono.Replace("-", "");

                using (var Dbcontext = new ORIONDBEntities())
                {
                    if (fbClienteExiste == true)
                    {
                        var PlazoConntrato = model.DatosProductos.FirstOrDefault().fiIDPlazo;
                        var ResultadoDatos = Convert.ToInt32(Dbcontext.sp_OrionSolicitud_Mestro_NuevoServicio_Insertar(model.fiIDEquifax, model.fcDireccionDetallada, " ", model.fiDepartamento, model.fiMunicipio, 0, model.fiColonia, 0, 0, fiIDUser, model.fcCorreo, opcionselectdireccion, condicionMes, fiIDSolicitudDireccion, PlazoConntrato).FirstOrDefault());
                        if (ResultadoDatos > 0)
                        {
                            foreach (var item in model.DatosProductos)
                            {
                                Dbcontext.sp_Equifax_GarantiaNuevoServicio_Guardar(model.fiIDEquifax, model.fcIdentidad, item.fiIDProducto, item.fnValorProductoMN, item.fnValorProductoME, item.fnPorcentajeImpuesto1, item.fnPorcentajeImpuesto2, fiIDUser, item.fnValorCuotaMensual, item.fiIDPaquete, ResultadoDatos, condicionMes, item.fiIDPlazo, item.fnValorPaquete);
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in model.DatosProductos)
                        {
                            Dbcontext.sp_Equifax_Garantia_Guardar(model.fiIDEquifax, model.fcIdentidad, item.fiIDProducto, item.fnValorProductoMN, item.fnValorProductoME, item.fnPorcentajeImpuesto1, item.fnPorcentajeImpuesto2, fiIDUser, item.fnValorCuotaMensual, item.fiIDPaquete, 0, condicionMes, item.fiIDPlazo, item.fnValorPaquete);
                        }
                        MensajeriaApi.EnviarNumero(model.fcPrimerNombre, model.fiIDEquifax, NumeroTelefono);
                    }
                    var result = Dbcontext.sp_Equifax_Cliente_Actualizar_Telefono(model.fiIDEquifax, NumeroTelefono) > 0;

                    return EnviarResultado(result, "Actualizar", "Se envio mensaje");
                }
            }
            catch (Exception e)
            {
                return EnviarException(e, "Repo Request Send Exception");
            }
        }

        [HttpGet]
        public JsonResult DocumentosPorCondicion(int InfoPrecalificado)
        {
            try
            {
                using (var context = new ORIONDBEntities())
                {
                    var UsuariosComayagua = context.sp_CondicionesPRecalificados_listarPorIDInfoPrecalificado(InfoPrecalificado).ToList();
                    return EnviarListaJson(UsuariosComayagua);
                }
            }
            catch (Exception e)
            {
                return null;
            }

        }

        [HttpPost]
        public JsonResult GuardarFoto(int IDInfoPrecalificado, int IDCondicion, List<Base64ynombreViewModel> fotosbase)
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
                    var documentoURL = @"\Precalificado\Precalificado_" + IDInfoPrecalificado + @"\";
                    var urlPdf = MemoryLoadManager.URL + documentoURL;
                    var ruta = documentoURL.Replace("*", "").Replace("/", "").Replace("\\", "").Replace(":", "").Replace("?", "").Replace("<", "").Replace(">", "").Replace("|", "");
                    var pdfFile = Path.Combine(urlPdf, item.nombrearchivo);
                    var exists = System.IO.Directory.Exists(urlPdf);
                    using (var context = new ORIONDBEntities())
                    {
                        var connection = (SqlConnection)context.Database.Connection;
                        var command = new SqlCommand("sp_Documetos_Precalificados_Insert", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@piIDInfoPrecalificado", IDInfoPrecalificado);
                        command.Parameters.AddWithValue("@piIDCondicion", IDCondicion);
                        command.Parameters.AddWithValue("@piIDUsuario", GetIdUser());
                        command.Parameters.AddWithValue("@pcUrlDocumento", urlPdf);
                        command.Parameters.AddWithValue("@pcNombreDocumento", arch.FileName);
                        command.Parameters.AddWithValue("@pcRuta", ruta);
                        command.Parameters.AddWithValue("@pcExtencion", Path.GetExtension(arch.FileName));
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
                    UploadFileServer148(@"\Precalificado\Precalificado_" + IDInfoPrecalificado + @"\", arch);
                }

                return EnviarResultado(true, "Imágenes subidas exitosamente");
            }
            catch (Exception e)
            {
                return EnviarException(e, "ERROR");
            }
        }

        [HttpGet]
        public JsonResult gvDetallePrestamos_RowCommand(string pcIdentidad, string IDPrestamo)
        {

            // string IDSOL = HttpUtility.ParseQueryString(lURLDesencriptado.Query).Get("IDSOL");

            string parametrosEncriptados = HttpUtility.UrlEncode(DataCrypt.Encriptar("IDCliente=" + pcIdentidad.Trim() + "&Usr=" + 1.ToString() + "&IDPrestamo=" + IDPrestamo));
            return EnviarListaJson(parametrosEncriptados);

        }
        public List<sp_geo_ListaDepartamentos_Result> ListaDepartamentos()
        {
            var lista = _connection.OrionContext.sp_geo_ListaDepartamentos().ToList();

            return lista;
        }

        public List<sp_ListaOcupaciones_Select_Result> ListaOcupaciones()
        {
            var lista = _connection.OrionContext.sp_ListaOcupaciones_Select().ToList();

            return lista;
        }

        [AllowAnonymous]
        public ActionResult ViewFormuVendedoresExternos()
        {
            datosclienteViewModel model = new datosclienteViewModel();
            ViewBag.ListaDepartamentos = ListaDepartamentos().Select(z => new SelectListItem { Value = Convert.ToString(z.fiCodDepartamento), Text = z.fcDepartamento.ToString() }).ToList();
            ViewBag.ListaOcupaciones = ListaOcupaciones().Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDOcupacion), Text = z.fcDescripcion.ToString() }).ToList();


            return View(model);
        }


        #region ModeloMesesAño
        public class MesAnioVenta
        {
            public int Anio { get; set; }
            public int Mes { get; set; }
            public string NombreMes { get; set; }
        }
        #endregion

        #region Modulo Soporte Tecnico
        [HttpGet]
        public JsonResult ListaPrestamo()
        {
            using (var conetion = new ORIONDBEntities())
            {
                var lista = EnviarListaJson(conetion.sp_Prestamos_Lista_Novanet(1, 1, GetIdUser()).ToList());
                return lista;
            }

        }
        [HttpGet]
        public ActionResult ModalAgregarDatosClienteDetalleInstalacion(string Nombre, int IDSolicitud)
        {
            using (var conetion = new ORIONDBEntities())
            {

                var modelClientesDetalle = conetion.sp_ObtenerDatosServicioRed_PorOrionSolicitud(IDSolicitud).FirstOrDefault();
                ViewBag.ListaOLT = new List<SelectListItem>();
                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_OrionSolicitud_LlenarListas {1} ";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        reader.NextResult();
                        reader.NextResult();
                        var ListOLT = db.ObjectContext.Translate<SolicitudesViewModel>(reader).ToList();
                        ViewBag.ListaOLT = ListOLT?.Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDOLT), Text = z.fcDescripcionOLT })?.ToList() ?? new List<SelectListItem>();

                    }

                    connection.Close();

                }
                if (modelClientesDetalle == null) return PartialView(new SolicitudesViewModel { IDSolicitud = IDSolicitud, Nombre = Nombre, fcDescripcionOLT = " ", fcMac = "", fcIP = "", fcPom = "", fcNombreWifi = " ", fiIDOLT = 1 });
                var clienteDetalleServicioRed = new SolicitudesViewModel
                {
                    Nombre = Nombre,
                    IDSolicitud = IDSolicitud,
                    fiIDOrionSolicitud = modelClientesDetalle.fiIDOrionSolicitud,
                    fcDescripcionOLT = modelClientesDetalle.fcDescripcionOLT,
                    fcMac = modelClientesDetalle.fcMac,
                    fcIP = modelClientesDetalle.fcIP,
                    fcPom = modelClientesDetalle.fcPom,
                    fcNombreWifi = modelClientesDetalle.fcNombreWifi,
                    fiIDOLT = modelClientesDetalle.fiIDOLT,

                };




                return PartialView(clienteDetalleServicioRed);
            }

        }
        [HttpGet]
        public ActionResult ModalVerDatoServicioRed(string Nombre, int IDSolicitud)
        {
            try
            {

                using (var conetion = new ORIONDBEntities())
                {
                    var modelClientesDetalle = conetion.sp_ObtenerDatosServicioRed_PorOrionSolicitud(IDSolicitud).FirstOrDefault();
                    if (modelClientesDetalle == null) return PartialView(new SolicitudesViewModel { Nombre = Nombre, IDSolicitud = IDSolicitud });
                    var clienteDetalleServicioRed = new SolicitudesViewModel
                    {
                        fiIDOrionSolicitud = modelClientesDetalle.fiIDOrionSolicitud,
                        fcDescripcionOLT = modelClientesDetalle.fcDescripcionOLT,
                        fcMac = modelClientesDetalle.fcMac,
                        fcIP = modelClientesDetalle.fcIP,
                        fcPom = modelClientesDetalle.fcPom,
                        fcNombreWifi = modelClientesDetalle.fcNombreWifi,

                    };


                    return PartialView(clienteDetalleServicioRed);

                }
            }
            catch (Exception e)
            {
                return PartialView(new InformacionCreditoClienteViewModel());
            }


        }
        [HttpGet]
        public ActionResult ModalCrearBitacoras(string Nombre, int IDSolicitud)
        {
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {

                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_OrionSolicitud_LlenarListaGestion {1} ";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    var List = db.ObjectContext.Translate<SolicitudesViewModel>(reader).ToList();
                    ViewBag.ListadoGestion = List.Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDGestion), Text = z.fcGestion }).ToList();


                }
                connection.Close();
            }
            using (var conetion = new ORIONDBEntities())
            {
                return PartialView(new SolicitudesViewModel { Nombre = Nombre, IDSolicitud = IDSolicitud, fiIDGestionAnterior = 1 });
            }

        }

        [HttpGet]
        public ActionResult ModalGenerarOrdenTrabajoContratista(string Nombre, int IDSolicitud, int idCliente , int idGestion)
        {
            bool permiteCrearOrden;
            string mensajeValidacion;

            using (var context = new ORIONDBEntities())
            {
                var connection = context.Database.Connection;
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "sp_Contratista_SolicitudInstalacion_VerificacionCreacionSOP";
                    command.CommandType = CommandType.StoredProcedure;

                    var paramSolicitud = command.CreateParameter();
                    paramSolicitud.ParameterName = "@piIDSolicitud";
                    paramSolicitud.Value = IDSolicitud;
                    paramSolicitud.DbType = DbType.Int32;
                    command.Parameters.Add(paramSolicitud);

                    var paramCreacion = command.CreateParameter();
                    paramCreacion.ParameterName = "@pbCreacion";
                    paramCreacion.DbType = DbType.Boolean;
                    paramCreacion.Direction = ParameterDirection.Output;
                    command.Parameters.Add(paramCreacion);

                    var paramMensaje = command.CreateParameter();
                    paramMensaje.ParameterName = "@pcMensaje";
                    paramMensaje.DbType = DbType.String;
                    paramMensaje.Size = 150;
                    paramMensaje.Direction = ParameterDirection.Output;
                    command.Parameters.Add(paramMensaje);

                    command.ExecuteNonQuery();

                    permiteCrearOrden = (bool)paramCreacion.Value;
                    mensajeValidacion = paramMensaje.Value?.ToString(); 
                }

                if (!permiteCrearOrden)
                {
                    connection.Close();
                    return Json(new
                    {
                        exito = false,
                        tipo = "Advertencia",
                        mensaje = mensajeValidacion
                    }, JsonRequestBehavior.AllowGet);
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"EXEC sp_OrionSolicitud_LlenarListas {1} ";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)context);
                        var List = db.ObjectContext.Translate<SolicitudesViewModel>(reader).ToList();
                        ViewBag.Listado1 = List.Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDAgencia), Text = z.fcNombreAgencia }).ToList();

                        reader.NextResult();
                        var List2 = db.ObjectContext.Translate<SolicitudesViewModel>(reader).ToList();
                        ViewBag.Listado2 = List2.Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDContratista), Text = z.fcNombreEmpresa }).ToList();
                    }

                    command.CommandText = $"EXEC sp_OrionSolicitud_LlenarListaGestion_byId {1}, {idGestion}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        var Lista = db.ObjectContext.Translate<SolicitudesViewModel>(reader).ToList();
                        ViewBag.ListadoGestion = Lista.Select(z => new SelectListItem { Value = Convert.ToString(z.fiIDGestion), Text = z.fcGestion }).ToList();

                    }
                }

                connection.Close();
            }

            return PartialView(new SolicitudesViewModel
            {
                Nombre = Nombre,
                IDSolicitud = IDSolicitud,
                IdCliente = idCliente
            });
        }



        [HttpGet]
        public async Task<ActionResult> ModalGenerarSolicitudSpliter(string Nombre, int IDSolicitud, int idCliente)
        {
            try
            {
                var datosCliente = _connection.OrionContext.sp_OrionContratista_DetalleBySolicitud(1, IDSolicitud, 0, 0).FirstOrDefault();
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
                var listaActividades = await ListaActividades();
                listaActividades = JsonConvert.DeserializeObject<dynamic>(listaActividades);
                ViewBag.Actividades = new SelectList(listaActividades.resultado, "codigo", "nombre");

                var clienteDetalleInstalacion = new SolicitudesViewModel
                {
                    Nombre = datosCliente.fcNombre,
                    IDSolicitud = IDSolicitud,
                    CodigoCliente = datosCliente.fcCodigoCliente,
                    NumeroOrdenCepheus = datosCliente.fcNumeroOrdenCfeus,
                    NumeroOrdenTrabajoCepheus = datosCliente.fcNumeroOrdenTrabajoChepeus,
                    IdCliente = idCliente
                };
                // return PartialView(new SolicitudesViewModel { Nombre = Nombre, IDSolicitud = IDSolicitud, IdCliente = idCliente   });
                return PartialView(clienteDetalleInstalacion);
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public async Task<dynamic> ListaActividades()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://192.168.20.23:8984/API/actividades");
                client.Timeout = TimeSpan.FromMilliseconds(240000); // 240 segundos

                // Encabezados
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "Basic VVNSX1JFU0xMRVJJTlRFUjowbFhwOThAJQ==");

                // Cuerpo de la solicitud
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("usuario", "us.nova"),
                    new KeyValuePair<string, string>("clave", "24s/GKT29T")
                });

                try
                {
                    var response = await client.PostAsync("", content);
                    Console.WriteLine(response);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        return responseContent;
                    }
                    else
                    {
                        Console.WriteLine("Error: " + response.StatusCode);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Excepción: " + ex.Message);
                    return null;
                }
            }
        }

        [HttpGet]
        public ActionResult ModalVerBitacorasCliente(string Nombre, int IDSolicitud)
        {
            try
            {
                var DatosBitacorasListado = new List<sp_OrionListaBitacoras_Clientes_ViewModel>();

                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_OrionListaBitacoras_Clientes {IDSolicitud}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        DatosBitacorasListado = db.ObjectContext.Translate<sp_OrionListaBitacoras_Clientes_ViewModel>(reader).ToList();

                    }

                    connection.Close();
                }

                return PartialView(new SolicitudesViewModel { Nombre = Nombre, IDSolicitud = IDSolicitud, BitacorasClienteSoporte = DatosBitacorasListado });
            }
            catch (Exception e)
            {
                return PartialView(new SolicitudesViewModel { Nombre = Nombre, IDSolicitud = IDSolicitud });
            }


        }
        [HttpPost]
        public JsonResult GuardarDatosClienteServicioInstalacion(SolicitudesViewModel model, string Mac, string IP, string POM, int IdOLT, string NombreWifi)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var result = context.sp_OrionSolicitud_DetalleClienteServicioContratado_Guardar(model.IDSolicitud, IdOLT, Mac, IP, POM, NombreWifi, GetIdUser()) > 0;

                        return EnviarResultado(true, "Datos", "Datos Registrados");


                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }
        }
        [HttpPost]
        public JsonResult GuardarBitacoras(SolicitudesViewModel model, int IdGestion, string ObservacionesBitacoras)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var result = context.sp_Orion_ClienteBitacora_Registrar(GetIdUser(), IdGestion, ObservacionesBitacoras, model.IDSolicitud) > 0;

                        return EnviarResultado(true, "Datos", "Datos Registrados");


                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }
        }
        [HttpPost]
        public async Task<JsonResult> EnviarSolicitudTrabajoContratista(SolicitudesViewModel model, string Comentario, int idAgencia, int idAgenciaContratista , int idGestion)
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

                        var datos = context.sp_OrionContratista_DetalleBySolicitud(GetIdUser(), model.IDSolicitud, 0, 0).FirstOrDefault();
                        var result = context.sp_OrionSolicitud_ContratistaSolicitudInstalacio__Insertar(GetIdUser(), model.IdCliente, model.IDSolicitud, idAgencia, idAgenciaContratista, Comentario, datos.fcCodigoCliente, datos.fcNumeroOrdenCfeus, 2, "", null, idGestion);
                        var datosCliente = context.sp_OrionContratista_DetalleBySolicitud(GetIdUser(), model.IDSolicitud, 0, 0).FirstOrDefault();
                        //var Correo = "aebautista63@gmail.com";
                        var modelCorreo = new SendEmailViewModel();
                        var CuerpoComentarioCorreo = "Estimado : " + datosCliente.fcNombreEmpresa + " , se le notifica la solicitud de trabajo(SOP) por reportes para el cliente: " + model.Nombre + "<br/>" + " comentario:" + Comentario;
                        modelCorreo.DestinationEmail = datosCliente.fcCorreoElectronico;


                        //await _emailTemplateService.SendEmailToCustomer(new EmailTemplateServiceModel
                        //{
                        //    IdEmailTemplate = 21,
                        //    //CustomerEmail = modelCorreo.DestinationEmail,
                        //    CustomerEmail = Correo,
                        //    Comment = "Solicitud de contratista(SOP).",
                        //    IDSolicitud = model.IDSolicitud,
                        //    IdCliente = model.IdCliente,
                        //    List_CC = new List<string> { MemoryLoadManager.EmailSystemAdministrator }
                        //});


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
        public async Task<bool> EnviarSolicitudCepheus(SolicitudesViewModel model, string Comentario, string Actividad, string CodigoCliente, string NumeroOrdenCepheus, string NumeroOrdenTrabajoCepheus)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://192.168.20.23:8984/API/crearOrden");
                        client.Timeout = TimeSpan.FromMilliseconds(240000); // 240 segundos

                        // Encabezados
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Add("Authorization", "Basic VVNSX1JFU0xMRVJJTlRFUjowbFhwOThAJQ==");

                        // Cuerpo de la solicitud
                        var content = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("usuario", "us.nova"),
                            new KeyValuePair<string, string>("workorder", NumeroOrdenTrabajoCepheus),
                            new KeyValuePair<string, string>("actividad", Actividad),
                            new KeyValuePair<string, string>("comentario", Comentario)
                        });

                        try
                        {
                            var result = context.sp_Orion_ClienteBitacora_Registrar(GetIdUser(), 5, Comentario, model.IDSolicitud) > 0;
                            var response = await client.PostAsync("", content);

                            Console.WriteLine(response);
                            if (response.IsSuccessStatusCode)
                            {
                                var responseContent = await response.Content.ReadAsStringAsync();
                                responseContent = JsonConvert.DeserializeObject<dynamic>(responseContent);
                                return response.IsSuccessStatusCode;

                            }
                            else
                            {
                                Console.WriteLine("Error: " + response.StatusCode);
                                return false;
                            }

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Excepción: " + ex.Message);
                            return false;
                        }
                    }
                }

            }
        }
        [HttpGet]
        public ActionResult ModalEnvioSMS(string Nombre, string Telefono, int IDCliente)
        {
            using (var conetion = new ORIONDBEntities())
            {
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

        [AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> ObtenerInformacionMatrizSecundaria(string fcIdentidad, int fiMeses, decimal fnMonto)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var model = new MatrizSecundariaValidacionViewModel();

                    var conexion = contexto.Database.Connection;
                    await conexion.OpenAsync(); // Abre la conexión a la BD

                    using (var comando = conexion.CreateCommand())
                    {
                        comando.CommandText = "sp_MatrizSecundaria_Validacion";
                        comando.CommandType = CommandType.StoredProcedure;

                        // Agregar parámetros
                        comando.Parameters.Add(new SqlParameter("@pcIdentidad", fcIdentidad));
                        comando.Parameters.Add(new SqlParameter("@piPlazo", fiMeses));
                        comando.Parameters.Add(new SqlParameter("@pnCuotaMensual", fnMonto));

                        using (var reader = await comando.ExecuteReaderAsync())
                        {
                            // Leer el primer conjunto de resultados (CondicionViewModel)
                            if (await reader.ReadAsync())
                            {
                                model.Condicion = new CondicionViewModel
                                {
                                    fiMesesCondicion = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                    fnMontoCondicion = reader.IsDBNull(1) ? 0.0m : reader.GetDecimal(1)
                                };
                            }

                            // Mover al siguiente conjunto de resultados
                            if (await reader.NextResultAsync())
                            {
                                model.Direcciones = new List<DireccionesSolicitudesViewModel>();

                                // Leer el segundo conjunto de resultados (Lista de Direcciones)
                                while (await reader.ReadAsync())
                                {
                                    model.Direcciones.Add(new DireccionesSolicitudesViewModel
                                    {
                                        fiIDSolicitud = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                        fiCodColonia = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                                        fcBarrio = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                        fiCodDepartamento = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                                        fcDepartamento = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                        fiCodMunicipio = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                        fcMunicipio = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                        fcDireccionDetallada = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
                                    });
                                }
                            }
                        }
                    }
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }



            }
        }

        /// <summary>
        /// Resumen de Existencias para el paquete escogido al precalificar cliente
        /// </summary>
        /// <param name="piIDPaquete"></param>
        /// <returns></returns>
        public async Task<JsonResult> ObtenerInformacionExistenciasPaquete(int piIDPaquete)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var existencias = new List<ExistenciaInventarioViewModel>();

                    var conexion = contexto.Database.Connection;
                    await conexion.OpenAsync();

                    using (var comando = conexion.CreateCommand())
                    {
                        comando.CommandText = "sp_Productos_Maestro_ExistenciaInventarioParaPaquete";
                        comando.CommandType = CommandType.StoredProcedure;

                        comando.Parameters.Add(new SqlParameter("@piIDPaquete", piIDPaquete));

                        using (var reader = await comando.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var existencia = new ExistenciaInventarioViewModel
                                {
                                    FiIndex = reader.IsDBNull(reader.GetOrdinal("fiIndex")) ? 0 : (int)reader.GetInt64(reader.GetOrdinal("fiIndex")),
                                    FcProducto = reader.IsDBNull(reader.GetOrdinal("fcProducto")) ? string.Empty : reader.GetString(reader.GetOrdinal("fcProducto")),
                                    Ubicacion = reader.IsDBNull(reader.GetOrdinal("Ubicacion")) ? string.Empty : reader.GetString(reader.GetOrdinal("Ubicacion")),
                                    FnCantidad = reader.IsDBNull(reader.GetOrdinal("fnCantidad")) ? 0.0m : reader.GetDecimal(reader.GetOrdinal("fnCantidad"))
                                };

                                existencias.Add(existencia);
                            }
                        }
                    }

                    return Json(existencias, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion

        #region Ventas Repartidor
        public ActionResult Bandeja_VendedoresCalleRepartidores()
        {
            return View();
        }


        public JsonResult CargarListaVendedorRepartidor()
        {
            try
            {
                using (var context = new ORIONDBEntities())
                {
                    var IDRol = GetUser().IdRol;
                    var IDUser = GetIdUser();

                    var rolesAsesor = GetConfiguracion<int>("Orion_Ventas_Asesor_Externo_Por_Distribuidor", ',');
                    var rolesAdminDistribuidor = GetConfiguracion<int>("Orion_Admin_Ventas_Distribuidor", ',');

                    var listaVendedores = context.sp_OrionSolicitud_Repartidor_Listar(IDUser).AsQueryable();

                    if (rolesAdminDistribuidor.Contains(IDRol))
                    {
                        listaVendedores = listaVendedores.Where(x => x.fiUsuarioCreador == IDUser);
                    }
                   
                    return EnviarListaJson(listaVendedores.ToList());
                      
                    
                }
               
            }
            catch (Exception ex)
            {
                return EnviarException(ex, "ERROR");
            }
        }




        public ActionResult DetalleVendedor(int fiIDVendedorRepartidor)
        {
            using (var context = new ORIONDBEntities())
            {
                var datos = context.sp_OrionSolicitud_Repartidor_Listar(GetIdUser()).FirstOrDefault(x => x.fiIDVendedorRepartidor == fiIDVendedorRepartidor);

                var viewModel = new datosclienteViewModel();
                if (!string.IsNullOrEmpty(datos.fcDocumentosJSON))
                {
                    viewModel.DatosDocumentoListado = JsonConvert.DeserializeObject<List<VendedorDocumentosViewModel>>(datos.fcDocumentosJSON);
                    viewModel.fcNombreCompleto = datos.fcNombreVendedor;
                    viewModel.fcIdentidad = datos.fcIdentidadVendedor;
                    viewModel.fcTelefono = datos.fcNumeroVendedor;
                    viewModel.fcCorreo = datos.fcCorreo;
                    viewModel.fcDireccionDetallada = datos.fcDireccionExacta;
                    viewModel.fcOcupacion = datos.fcOcupacion;
                    viewModel.fcCiudad = datos.fcCiudad;
                }
                else
                {
                    viewModel.DatosDocumentoListado = new List<VendedorDocumentosViewModel>();
                }

                return PartialView(viewModel);
            }
           
        }

      

        [HttpGet]
        public ActionResult ModalAgregarDatosVendedorComision(string Nombre, int IDVendedor)
        {
            using (var conetion = new ORIONDBEntities())
            {

                var modelDetalle = conetion.sp_ObtenerDatosVendedor_PorId(IDVendedor).FirstOrDefault();

                if (modelDetalle == null) return PartialView(new SolicitudesViewModel { IDSolicitud = IDVendedor, Nombre = Nombre, fnMetaComision = 0, fnArpuMeta = 0 });
                var Datos = new SolicitudesViewModel
                {
                    Nombre = Nombre,
                    fiIDUsuario = IDVendedor,
                    fnMetaComision = modelDetalle.fnMetaComision,
                    fnArpuMeta = modelDetalle.fnArpuMeta,

                };


                return PartialView(Datos);
            }

        }

        [HttpGet]
        public ActionResult EditarVendedoresCalle(int fiIDVendedorRepartidor)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_OrionVentas_VendedorCalle_GetById(fiIDVendedorRepartidor).FirstOrDefault();

                var model = new CrearVendedorCalleViewModel
                {
                    fiIDVendedorRepartidor = modelDb.fiIDVendedorRepartidor,
                    fcNombreVendedor = modelDb.fcNombreVendedor,
                    fcNumeroVendedor = modelDb.fcNumeroVendedor,
                    fcIdentidadVendedor = modelDb.fcIdentidadVendedor,
                    fcCodigoVendedor = modelDb.fcCodigoVendedor,
                    fbEditar = true
                };
                return PartialView("CrearVendedoresCalle", model);
            }
        }



        [HttpPost]
        public JsonResult EditarVendedoresCalle(CrearVendedorCalleViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var result = contexto.sp_OrionVentas_VendedorCalle_Editar(model.fiIDVendedorRepartidor, model.fcNombreVendedor, model.fcNumeroVendedor, model.fcIdentidadVendedor, model.fcCodigoVendedor, GetIdUser()) > 0;
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

        [HttpGet]
        public ActionResult CrearVendedoresCalle()
        {
            return PartialView(new CrearVendedorCalleViewModel());
        }
        [HttpPost]
        public JsonResult CrearVendedorRepartidor(CrearVendedorCalleViewModel model)
        {

            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    //var list = contexto.sp_OrionCatalogo_OLT(GetIdUser()).ToList();
                    //if (list.Any(x => x.fcDescripcionOLT == model.fcDescripcionOLT))
                    //{
                    //    return EnviarResultado(false, "", "La OLT ya fue registrada");
                    //}
                    var result = contexto.sp_OrionVentas_VendedorCalle_Guardar(model.fcNombreVendedor, model.fcNumeroVendedor, model.fcIdentidadVendedor, model.fcCodigoVendedor, GetIdUser()) > 0;
                    return EnviarResultado(result, "", result ? "Vendedor registrada exitosamente" : "Error de red");

                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "error");
                }

            }

        }

        [HttpGet]
        public ActionResult ViewEnviarFormularioVendedor()
        {
            return PartialView(new datosclienteViewModel());
        }


        [HttpGet]
        public JsonResult EnviarFormularioVendedor(datosclienteViewModel model)
        {
            try
            {
                string urlFormulario = "";

                var tiposFormularioVendedores = GetConfiguracion<int>("Orion_Ventas_Vendedores_EnvioFormulario", ',');
                if (tiposFormularioVendedores.Contains(model.fiIDEnvioFormulario))
                    urlFormulario = $"{MemoryLoadManager.orionUrl}PrecalificaCliente/ViewFormuDistribuidores";                
                else
                    urlFormulario = $"{MemoryLoadManager.orionUrl}PrecalificaCliente/ViewFormuVendedoresExternos";
                
                urlFormulario += $"?tipo={model.fiIDEnvioFormulario}";
                string urlBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(urlFormulario));
                string redirectUrl = $"{MemoryLoadManager.orionUrl}PrecalificaCliente/formulario?id={Uri.EscapeDataString(urlBase64)}";
                string mensaje = "Estimado(a) aplicante, por favor complete el siguiente formulario haciendo clic en el enlace a continuación para continuar con el proceso de registro: " + redirectUrl;
                string mensajeCodificado = Uri.EscapeDataString(mensaje);
                List<string> telefonos = new List<string>();
                if (!string.IsNullOrEmpty(model.fcTelefono))
                {
                    if (model.fcTelefono.Contains(","))
                         telefonos = model.fcTelefono.Split(',', (char)StringSplitOptions.RemoveEmptyEntries).Select(t => t.Replace("-", "").Trim()).ToList(); //verificar si hay comas
                    else
                        telefonos = model.fcTelefono.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries) .Select(t => t.Replace("-", "").Trim()).ToList();  //saltos de línea
                }

                if (!telefonos.Any()) return EnviarResultado(false, "", "Error: No se proporcionaron números de teléfono válidos.");
                bool allSuccess = true;
                string errorMessage = "";

                using (var httpClient = new HttpClient())
                {
                    foreach (var telefono in telefonos)
                    {
                        if (!string.IsNullOrEmpty(telefono))
                        {
                            string url = $"https://orion.novanetgroup.com/base/EnviarWhatsapp?telefono={telefono}&mensaje={mensajeCodificado}";
                            var response = httpClient.GetAsync(url).Result;
                            var responseBody = response.Content.ReadAsStringAsync().Result;

                            if (!response.IsSuccessStatusCode)
                            {
                                allSuccess = false;
                                errorMessage += $"Error al enviar a {telefono}: {responseBody}; ";
                            }
                        }
                    }
                }
                if (allSuccess)
                    return EnviarResultado(true, "Formulario enviado", "Formulario enviado exitosamente a todos los números.", model.fiIDEnvioFormulario.ToString());
                else
                    return EnviarResultado(false, "Error parcial", $"Error en algunos envíos: {errorMessage}", model.fiIDEnvioFormulario.ToString());

            }
            catch (Exception ex)
            {
                return EnviarException(ex, "Error al enviar el formulario del vendedor.");
            }
        }

  




        [AllowAnonymous]
        [HttpGet]
        public ActionResult Formulario(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return HttpNotFound("Parámetro ID no proporcionado.");
                }
                byte[] decodedBytes = Convert.FromBase64String(id);
                string decodedUrl = Encoding.UTF8.GetString(decodedBytes);
                if (!decodedUrl.StartsWith(MemoryLoadManager.orionUrl))
                {
                    return HttpNotFound("URL inválida.");
                }
                return Redirect(decodedUrl);
            }
            catch (Exception ex)
            {
                return HttpNotFound("Error al procesar la solicitud: " + ex.Message);
            }
        }


        [AllowAnonymous]
        [HttpPost]
        public JsonResult CrearVendedorExterno(datosclienteViewModel model, List<Base64ynombreViewModel> fotosbase)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                  
                    var configuraciones = GetConfigurationJson<ConfigDistribuidorVendedor>("Orion_Ventas_Vendedor_Distribuidor");
                    var carpetaV = $@"DocumentosVendedores\Vendedor_{model.fcIdentidad.Trim()}";
                    var carpetaFisica = $@"C:\inetpub\novanetgroup.com_Orion\{carpetaV}";
                    string documentoURL = "";
                    string nombreArchivo = "";

                    foreach (var item in fotosbase)
                    {
                        var archivo = ConvertirBase64AImagen(item.base64string, item.nombrearchivo);
                        var guardado = UploadFileServer148(carpetaV, archivo);
                        if (!guardado)
                            return EnviarResultado(false, "", "Error al guardar la imagen.");

                        nombreArchivo = archivo.FileName;

                        if (string.IsNullOrEmpty(documentoURL))
                        {
                            documentoURL = $"{MemoryLoadManager.orionUrl}{carpetaV.Replace(@"\", "/")}/{archivo.FileName}";
                        }
                    }
                    int? idInsertado = null;

                    var connection = contexto.Database.Connection;
                    if (connection.State != System.Data.ConnectionState.Open)
                        connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "sp_OrionVentas_VendedorExterno_Guardar";
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@pcNombreVendedor", model.fcNombreCompleto));
                        command.Parameters.Add(new SqlParameter("@pcTelefono", model.fcTelefono));
                        command.Parameters.Add(new SqlParameter("@pcIdentidadVendedor", model.fcIdentidad));
                        command.Parameters.Add(new SqlParameter("@fiIDOcupacion", model.fiIDOcupacion));
                        command.Parameters.Add(new SqlParameter("@fiIDTipoVendedor", model.fiIDEnvioFormulario));
                        command.Parameters.Add(new SqlParameter("@pcCiudad", model.fcCiudad));
                        command.Parameters.Add(new SqlParameter("@pcDireccion", model.fcDireccion));
                        command.Parameters.Add(new SqlParameter("@pcLongitud", model.fcLongitud));
                        command.Parameters.Add(new SqlParameter("@pcfcLatitud", model.fcLatitud));
                        command.Parameters.Add(new SqlParameter("@PdFechaNacimiento", model.fdFechaNacimiento));
                        command.Parameters.Add(new SqlParameter("@pcCorreo", model.fcCorreo));
                        command.Parameters.Add(new SqlParameter("@pcPrimerNombre", model.fcPrimerNombre));
                        command.Parameters.Add(new SqlParameter("@pcSegundoNombre", model.fcSegundoNombre));
                        command.Parameters.Add(new SqlParameter("@pcPrimerApellido", model.fcPrimerApellido));
                        command.Parameters.Add(new SqlParameter("@pcSegundoApellido", model.fcSegundoApellido));
                        command.Parameters.Add(new SqlParameter("@fiIDUsuario", GetIdUser()));

                        var result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int id))
                        {
                            idInsertado = id;
                        }
                    }

                    if (idInsertado.HasValue && idInsertado.Value > 0)
                    {
                        foreach (var config in configuraciones)
                        {
                            contexto.sp_OrionVendedores_Documentos_Insertar(idInsertado, nombreArchivo.Replace(".jpg", ""), ".jpg", carpetaFisica, documentoURL, config.fiIDOrion_DocumentoIdentidadVendedor, "Foto Identidad", GetIdUser());
                        }


                       
                    }

                    return EnviarResultado(idInsertado > 0, "", idInsertado > 0 ? "Vendedor registrado exitosamente" : "Error al registrar");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }

        [HttpPost]
        public async Task<JsonResult> EnviarSolicitudFirmaVendedor(datosclienteViewModel model)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {

                        var _emailTemplateService = new EmailTemplateService();
                        var datosVendedor = _connection.OrionContext.sp_OrionSolicitud_Repartidor_Listar(1).Where(x=>x.fiIDVendedorRepartidor == model.fiIDVendedorRepartidor).FirstOrDefault();
                        await _emailTemplateService.SendEmailToAcuerdoColaboracionVendedor(new EmailTemplateServiceModel
                        {
                            CustomerEmail = datosVendedor.fcCorreo,
                            IdCustomer = datosVendedor.fcNombreVendedor,  
                            fiIDVendedorRepartidor = datosVendedor.fiIDVendedorRepartidor,
                            List_CC = new List<string> { MemoryLoadManager.EmailSystemGustavo }
                        });

                        context.Database.ExecuteSqlCommand("EXEC sp_Orion_Ventas_RepartidorMaestro_Actualizar @fiIDVendedorRepartidor",new SqlParameter("@fiIDVendedorRepartidor", model.fiIDVendedorRepartidor));
                        return EnviarResultado(true, "Firma", "Solicitud de firma enviado correctamente.");


                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }
        }

        [HttpPost]
        public JsonResult GuardarDatosMetasVendedor(SolicitudesViewModel model, decimal Meta, decimal Arpu)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {
                        var result = context.sp_OrionVentas_ComisionesUsuario_Guardar(Meta, Arpu, model.fiIDUsuario, GetIdUser()) > 0;

                        return EnviarResultado(true, "Datos", "Datos Registrados");


                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }
        }


        public ActionResult ViewDistribuidores()
        {
            return View();
        }

        public JsonResult CargarListaDistribuidores()
        {
            try
            {
                var IDRol =  GetUser().IdRol;
                var IDUser = GetIdUser();
                var fiIDDistribuidor = GetUser().fiIDDistribuidor;

                using (var contexto = new ORIONDBEntities())
                {
                    var rolesAdminDistribuidor = GetConfiguracion<int>("Orion_Admin_Ventas_Distribuidor", ',');
                    bool esAdminDistribuidor = rolesAdminDistribuidor.Contains(IDRol);
                    var distribuidores = contexto.Database .SqlQuery<ListDistribuidorViewModel>("Exec sp_Orion_Ventas_Distribuidor_Listado").ToList(); 

                    if (rolesAdminDistribuidor.Contains(IDRol))
                    {
                        distribuidores = distribuidores .Where(x => x.fiIDDistribuidor == fiIDDistribuidor).ToList();
                    }

                    if (!distribuidores.Any())
                        return Json(new { success = false, message = "Sin datos." }, JsonRequestBehavior.AllowGet);

                    return Json(distribuidores, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return EnviarException(ex, "Error");
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public ActionResult ViewFormuDistribuidores()
        {

            return View(new ListDistribuidorViewModel());
        }


        public ActionResult DetalleDistribuidor(int fiIDDistribuidor)
        {
            using (var context = new ORIONDBEntities())
            {
                var datos = context.Database.SqlQuery<ListDistribuidorViewModel>("Exec sp_Orion_Ventas_Distribuidor_Listado").FirstOrDefault(x => x.fiIDDistribuidor == fiIDDistribuidor);

                var viewModel = new ListDistribuidorViewModel();
                if (!string.IsNullOrEmpty(datos.fcDocumentosJSON))
                {
                    viewModel.DatosDocumentoListado = JsonConvert.DeserializeObject<List<VendedorDocumentosViewModel>>(datos.fcDocumentosJSON);
                    viewModel.NombreRepresentante = datos.NombreRepresentante;
                    viewModel.fcIdentidadRepresentante = datos.fcIdentidadRepresentante;
                    viewModel.fcTelefono = datos.fcTelefonoMovil;
                    viewModel.fcCorreoElectronico = datos.fcCorreoElectronico;

                    viewModel.fcNombreComercial = datos.fcNombreComercial;
                    viewModel.fcRTN = datos.fcRTN;  
                    viewModel.fcTelefono = datos.fcTelefono;
                    viewModel.fcDireccion = datos.fcDireccion;
                }
                else
                {
                    viewModel.DatosDocumentoListado = new List<VendedorDocumentosViewModel>();
                }

                return PartialView(viewModel);
            }

        }

        public ActionResult ModalActulizarDistribuidor(int fiIDDistribuidor = 0)
        {
            using (var contexto = new ORIONDBEntities())
            {
                if (fiIDDistribuidor == 0)
                    return PartialView(new ListDistribuidorViewModel());
                
                var modelDb = contexto.Database.SqlQuery<ListDistribuidorViewModel>("EXEC sp_Orion_Ventas_Distribuidor_ListadobyId @p0", fiIDDistribuidor).FirstOrDefault();
                var model = new ListDistribuidorViewModel
                {
                    fcNombreComercial = modelDb.fcNombreComercial,
                    fcRTN = modelDb.fcRTN,
                    fcTelefono = modelDb.fcTelefono,
                    fcDireccion = modelDb.fcDireccion,
                    fcIdentidadRepresentante = modelDb.fcIdentidadRepresentante,
                    fcPrimerNombre = modelDb.fcPrimerNombre,
                    fcSegundoNombre = modelDb.fcSegundoNombre,
                    fcPrimerApellido = modelDb.fcPrimerApellido,
                    fcSegundoApellido = modelDb.fcSegundoApellido,
                    fcTelefonoMovil = modelDb.fcTelefonoMovil,
                    fcCorreoElectronico = modelDb.fcCorreoElectronico,
                    EsEditar = true
                };


                return PartialView("ModalActulizarDistribuidor", model);
            }
          
        }


        [HttpPost]
        public ActionResult DeshabilitarDistribuidor(int fiIDDistribuidor)
        {
            try
            {
                using (var contexto = new ORIONDBEntities())
                {

                    var connection = (SqlConnection)contexto.Database.Connection;
                    var command = new SqlCommand("Orion.dbo.sp_Distribuidor_Deshabilitar", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@fiIDDistribuidor", fiIDDistribuidor);
                    command.Parameters.AddWithValue("@fiIDUsuario", GetIdUser());

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return EnviarResultado(true, "Distribuidor", "Registro deshabilitado correctamente.");
            }
            catch (Exception ex)
            {
                return EnviarException(ex, "error");
            }          

        }

        [HttpPost]
        public ActionResult ActualizarDistribuidor(ListDistribuidorViewModel model)
        {
            try
            {
                using (var contexto = new ORIONDBEntities())
                {

                    var connection = (SqlConnection)contexto.Database.Connection;
                    var command = new SqlCommand("Orion.dbo.sp_Orion_Ventas_Distribuidores_Actualizar", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@fiIDDistribuidor", model.fiIDDistribuidor);
                    command.Parameters.AddWithValue("@fcNombreComercial", model.fcNombreComercial);
                    command.Parameters.AddWithValue("@fcRTN", model.fcRTN);
                    command.Parameters.AddWithValue("@fcTelefono", model.fcTelefono);
                    command.Parameters.AddWithValue("@fcDireccion", model.fcDireccion);
                    command.Parameters.AddWithValue("@fcIdentidadRepresentante", model.fcIdentidadRepresentante);
                    command.Parameters.AddWithValue("@fcPrimerNombre", model.fcPrimerNombre);
                    command.Parameters.AddWithValue("@fcSegundoNombre", model.fcSegundoNombre);
                    command.Parameters.AddWithValue("@fcPrimerApellido", model.fcPrimerApellido);
                    command.Parameters.AddWithValue("@fcSegundoApellido", model.fcSegundoApellido);
                    command.Parameters.AddWithValue("@fcTelefonoMovil", model.fcTelefonoMovil);
                    command.Parameters.AddWithValue("@fcCorreoElectronico", model.fcCorreoElectronico);
                    command.Parameters.AddWithValue("@fiIDUsuario", GetIdUser());

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                return EnviarResultado(true, "Distribuidor", "Registro actualizado correctamente.");
            }
            catch (Exception ex)
            {
                return EnviarException(ex, "error");
            }

        }



        [HttpGet]
        public JsonResult VendedoresPorDistribuidor(int fiIDDistribuidor)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var lista = contexto.Database.SqlQuery<ListVendedoresViewModel>("EXEC SP_Orion_Ventas_VendedoresPorDistribuidor @fiIDDistribuidor", new SqlParameter("@fiIDDistribuidor", fiIDDistribuidor)).ToList();

                return Json(lista, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult CrearDistribuidor(ListDistribuidorViewModel model, List<Base64ynombreViewModel> fotosbase)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {

                    var configuraciones = GetConfigurationJson<ConfigDistribuidorVendedor>("Orion_Ventas_Vendedor_Distribuidor");
                    var carpetaV = $@"DocumentosDistribuidor\Distribuidor_{model.fcIdentidadRepresentante.Trim()}";
                    var carpetaFisica = $@"C:\inetpub\novanetgroup.com_Orion\{carpetaV}";
                    bool esNuevo = false;
                    int? idInsertado = null;
                    using (var context = new ORIONDBEntities())
                    {
                        var connection = (SqlConnection)context.Database.Connection;
                        var command = new SqlCommand("Orion.dbo.sp_Orion_Ventas_Distribuidores_Insertar", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@fiIDDistribuidor", model.fiIDDistribuidor);
                        command.Parameters.AddWithValue("@fcNombreComercial", model.fcNombreComercial);
                        command.Parameters.AddWithValue("@fcRTN", model.fcRTN);
                        command.Parameters.AddWithValue("@fcTelefono", model.fcTelefono);
                        command.Parameters.AddWithValue("@fcDireccion", model.fcDireccion);
                        command.Parameters.AddWithValue("@fcIdentidadRepresentante", model.fcIdentidadRepresentante);
                        command.Parameters.AddWithValue("@fcPrimerNombre", model.fcPrimerNombre);
                        command.Parameters.AddWithValue("@fcSegundoNombre", model.fcSegundoNombre);
                        command.Parameters.AddWithValue("@fcPrimerApellido", model.fcPrimerApellido);
                        command.Parameters.AddWithValue("@fcSegundoApellido", model.fcSegundoApellido);
                        command.Parameters.AddWithValue("@fcTelefonoMovil", model.fcTelefonoMovil);
                        command.Parameters.AddWithValue("@fcCorreoElectronico", model.fcCorreoElectronico);
                        command.Parameters.AddWithValue("@fiIDEstado", model.fiIDEstado);
                        command.Parameters.AddWithValue("@fiIDUsuario", GetIdUser());

                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idInsertado = reader.GetInt32(0);     
                                esNuevo = reader.GetInt32(1) == 1;   
                            }
                        }
                        connection.Close();
                    }

                    if (esNuevo && idInsertado.HasValue && idInsertado.Value > 0)
                    {
                        foreach (var item in fotosbase)
                        {
                            var archivo = ConvertirBase64AImagen(item.base64string, item.nombrearchivo);
                            var guardado = UploadFileServer148(carpetaV, archivo);
                            if (!guardado)
                                return EnviarResultado(false, "", "Error al guardar la imagen.");

                            var documentoURL = $"{MemoryLoadManager.orionUrl}{carpetaV.Replace("\\", "/")}/{archivo.FileName}";
                            foreach (var config in configuraciones)
                            {
                                contexto.sp_OrionDistribuidor_Documentos_Insertar(idInsertado, archivo.FileName.Replace(".jpg", ""), ".jpg", carpetaFisica, documentoURL, item.fiIDDocumento, item.fcComentario, GetIdUser());
                            }
                          
                        }
                    }
                    if(!esNuevo) return EnviarResultado(false, "Distribuidor", "Ya hay un registro con este distribuidor.");

                    return EnviarResultado(true, "Distribuidor", "Registro guardado correctamente.");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "error");
                }
            }
        }


        [HttpPost]
        public async Task<JsonResult> EnviarSolicitudFirmaDistribuidor(ListDistribuidorViewModel model)
        {
            using (var context = new ORIONDBEntities())
            {
                using (var security = new OrionSecurityEntities())
                {
                    try
                    {

                        var _emailTemplateService = new EmailTemplateService();
                        var data = context.Database.SqlQuery<ListDistribuidorViewModel>("Exec sp_Orion_Ventas_Distribuidor_Listado").Where(x => x.fiIDDistribuidor == model.fiIDDistribuidor).FirstOrDefault();
                        await _emailTemplateService.SendEmailToAcuerdoColaboracionDistribuidor(new EmailTemplateServiceModel
                        {
                            CustomerEmail = data.fcCorreoElectronico,
                            IdCustomer = data.NombreRepresentante,
                            fiIDDistribuidor = data.fiIDDistribuidor,
                            //List_CC = new List<string> { MemoryLoadManager.EmailSystemGustavo }
                        });

                        context.Database.ExecuteSqlCommand("EXEC sp_Orion_Ventas_Distribuidores_Actualizar_Estado @fiIDDistribuidor", new SqlParameter("@fiIDDistribuidor", model.fiIDDistribuidor));
                        return EnviarResultado(true, "Firma", "Solicitud de firma enviado correctamente.");


                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }

                }

            }
        }
        #endregion


        #region InventarioCliente
        [HttpGet]
        public JsonResult GetInfoInventarioCliente(int fiIdEquifax, int fiIDSolicitud)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var listPaquete = contexto.sp_OrionSolicitud_PaqueteCliente(fiIdEquifax).Where(x => x.fiIDSolicitud == fiIDSolicitud).ToList();
                var listInventario = contexto.sp_OrionSolicitud_InventarioPorCliente(fiIdEquifax).Where(x => x.fiIDSolicitud == fiIDSolicitud || x.fiIDSolicitud == 0 || x.fiIDSolicitud == null).ToList();
                var listInstalado = contexto.sp_OrionSolicitud_InstalacionPorCliente(fiIdEquifax).Where(x => x.fiIDSolicitud == fiIDSolicitud).ToList();
                //✓
                var obj = new { listPaquete, listInventario, listInstalado };

                return EnviarListaJson(obj);
            }
        }


        [HttpGet]
        public JsonResult GetInfoInventario()
        {
            using (var contexto = new ORIONDBEntities())
            {
                var listClientes = contexto.sp_Prestamos_Lista_Novanet(1, 1, GetIdUser()).ToList();
                var listPaquetes = contexto.sp_OrionSolicitud_ListarPaqueteCliente().ToList();
                var listInventario = contexto.sp_OrionSolicitud_ListarInventarioPorCliente().ToList();
                var listInstalado = contexto.sp_OrionSolicitud_ListarInstalacionPorCliente().ToList();


                var info = new List<ListInventarioPorCliente>();

                var groupCliente = listClientes.GroupBy(x => x.fiIDEquifax).ToList();

                foreach (var cliente in groupCliente)
                {
                    var item = new ListInventarioPorCliente
                    {
                        fiIDEquifax = cliente.Key,
                        fcNombre = cliente.FirstOrDefault().fcNombreCliente,
                        Productos = new List<ListProductosPorSolicitud>()
                    };

                    var listPaqueteCliente = listPaquetes.Where(x => x.fiIDCliente == cliente.Key).ToList();
                    var listInventarioCliente = listInventario.Where(x => x.fiIDEquifax == cliente.Key).ToList();
                    var listInstalacionCliente = listInstalado.Where(x => x.fiIDEquifax == cliente.Key).ToList();

                    var listInventarioSinSolicitud = listInventarioCliente.Where(x => x.fiIDSolicitud == 0).ToList();
                    var listPaqueteSolicitud = new List<sp_OrionSolicitud_ListarPaqueteCliente_Result>();
                    var productos = new List<string>();

                    foreach (var solicitud in cliente.ToList())
                    {

                        listPaqueteSolicitud = listPaqueteCliente.Where(x => x.fiIDSolicitud == solicitud.fiIDSolicitud).ToList();
                        var listInventarioSolicitud = listInventarioCliente.Where(x => x.fiIDSolicitud == solicitud.fiIDSolicitud).ToList();
                        var listInstalacionSolicitud = listInstalacionCliente.Where(x => x.fiIDSolicitud == solicitud.fiIDSolicitud).ToList();

                        productos = listPaqueteSolicitud.Select(x => x.fcProducto).ToList();
                        productos.AddRange(listInventarioSolicitud.Select(x => x.fcProducto).ToList());
                        productos.AddRange(listInstalacionSolicitud.Select(x => x.fcProducto).ToList());
                        productos.AddRange(listInventarioSinSolicitud.Select(x => x.fcProducto).ToList());

                        productos = productos.Distinct().ToList();

                        foreach (var producto in productos)
                        {


                            item.Productos.Add(new ListProductosPorSolicitud
                            {
                                fiIDSolicitud = solicitud.fiIDSolicitud,
                                fcProducto = producto,
                                fcEnPaquete = listPaqueteSolicitud?.Any(x => x.fcProducto == producto) ?? false ? "✓" : "X",
                                fnCantidadInstalado = listInstalacionSolicitud?.Where(x => x.fcProducto == producto)?.Sum(x => x.fnCantidad ?? 0) ?? 0,
                                fnCantidadInventario = listInventarioSolicitud?.Where(x => x.fcProducto == producto)?.Sum(x => x.fnCantidad) ?? 0
                            });
                        }
                    }


                    if (listInventarioSinSolicitud != null)
                    {
                        if (listInventarioSinSolicitud.Count() > 0)
                        {
                            foreach (var producto in productos)
                            {
                                item.Productos.Add(new ListProductosPorSolicitud
                                {
                                    fiIDSolicitud = 0,
                                    fcProducto = producto,
                                    fcEnPaquete = listPaqueteSolicitud?.Any(x => x.fcProducto == producto) ?? false ? "✓" : "X",
                                    fnCantidadInstalado = 0,
                                    fnCantidadInventario = listInventarioSinSolicitud?.Where(x => x.fcProducto == producto)?.Sum(x => x.fnCantidad) ?? 0
                                });
                            }
                        }
                    }
                    info.Add(item);
                }

                return EnviarListaJson(info);
            }
        }
        #endregion
    }
}