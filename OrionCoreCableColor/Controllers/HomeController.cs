using Newtonsoft.Json;
using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.App_Services.FileKMZService;
using OrionCoreCableColor.DbConnection.CMContext;
using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models.Home;
using OrionCoreCableColor.Models.Productos;
using OrionCoreCableColor.Models.Reportes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OrionCoreCableColor.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly DbServiceConnection _connection;

        public HomeController()
        {
            var ConnectionString = ConfigurationManager.ConnectionStrings["OrionConnections"].ConnectionString;
            _connection = new DbServiceConnection(ConnectionString);
        }

        [Authorize]
        public ActionResult Index()
        {
            ViewBag.urlimagen = MemoryLoadManager.Helper;
            var usuar = GetUser();
            //var usuara = GetRol();
            return View();
        }

        public ActionResult ProcesarLink(string encryptedPath)
        {
            // Procesar la ruta encriptada
            return Content("Ruta procesada");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize(Roles = ("Orion_Acceso_Dashboard"))]
        public JsonResult DashBoardInformativo()
        {
            var prueba = _connection.OrionContext.sp_OrionSolicitud_Detalle_ClienteListar(GetIdUser(), 37).ToList();

            var etiqu = _connection.OrionContext.sp_DashboardInformativo(1).ToList();

            return EnviarListaJson(etiqu);
        }

        public ActionResult Cotizador()
        {
            var productosacotizar = _connection.OrionContext.sp_Producto_Maestro_Listar().GroupBy(a => a.fiIDTipoProducto).ToList();

            return View(productosacotizar);
        }

        public JsonResult CotizadordeProductos()
        {
            try
            {
                var cotizadorverdadero = _connection.OrionContext.sp_Cotizador_ProductosyServicios().GroupBy(a => a.fcTipoProducto).ToList();
                //ViewBag.idproductosbasicos = GetConfiguracion<int>("IdProductosPaqueteBasico", ',');
                //var idtipoproductos = GetConfiguracion<int>("IdTipoProductosSinCuotaMensual", ',');
                //var productosacotizar = _connection.OrionContext.sp_Producto_Maestro_Listar().Where(a => !idtipoproductos.Any(b => b == a.fiIDTipoProducto)).GroupBy(a => a.fiIDTipoProducto).ToList();
                return EnviarListaJson(cotizadorverdadero);
            }
            catch (Exception e)
            {

                throw;
            }
        }
        [AllowAnonymous]
        public JsonResult PaqueteServicio()
        {
            try
            {
                List<ComparativoPaqueteServicios> result = new List<ComparativoPaqueteServicios>();
                var lista = _connection.OrionContext.sp_DashBoard_Comparativo_ServiciosyPaquetes().ToList();
                foreach (var item in lista)
                {
                    var list = JsonConvert.DeserializeObject<ComparativoPaqueteServicios>(item);
                    result.Add(list);
                }
                return EnviarListaJson(result.ToList());
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public JsonResult FacturacionMensual()
        {
            try
            {
                List<FacturacionMensualViewModel> facturacionmensual = new List<FacturacionMensualViewModel>();
                List<VentasMensualesViewModel> ventasmensuales = new List<VentasMensualesViewModel>();
                List<ArpusViewModel> arpumes = new List<ArpusViewModel>();
                List<ProyeccionyActualViewModel> proyeccionmes = new List<ProyeccionyActualViewModel>();
                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {
                    //var lista = _connection.OrionContext.sp_DashBoard_FacturacionMensual(GetIdUser()).ToList();
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_DashBoard_FacturacionMensual {GetIdUser()}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        var facmensual = db.ObjectContext.Translate<string>(reader).ToList();

                        foreach (var item in facmensual)
                        {
                            var list = JsonConvert.DeserializeObject<FacturacionMensualViewModel>(item);
                            facturacionmensual.Add(list);
                        }
                        reader.NextResult();
                        var ventmensua = db.ObjectContext.Translate<string>(reader).ToList();
                        foreach (var item in ventmensua)
                        {
                            var list = JsonConvert.DeserializeObject<VentasMensualesViewModel>(item);
                            ventasmensuales.Add(list);
                        }
                        reader.NextResult();
                        var arpu = db.ObjectContext.Translate<string>(reader).ToList();
                        foreach (var item in ventmensua)
                        {
                            var list = JsonConvert.DeserializeObject<ArpusViewModel>(item);
                            arpumes.Add(list);
                        }
                        reader.NextResult();
                        var proyeccion = db.ObjectContext.Translate<string>(reader).ToList();
                        foreach (var item in proyeccion)
                        {
                            var list = JsonConvert.DeserializeObject<ProyeccionyActualViewModel>(item);
                            proyeccionmes.Add(list);
                        }
                    }
                }

                return EnviarListaJson(new ListaDashBoardViewModel { facturacionmensual = facturacionmensual.ToList(), ventasMensuales = ventasmensuales.ToList(), Arpumensuales = arpumes.ToList(), proyecciones = proyeccionmes.ToList() });
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> MapaCliente()
        {
            try
            {
                using (var conetion = new ORIONDBEntities())
                {
                    FileKMZService kmzService = new FileKMZService();
                    await kmzService.LoadKMZFromUrl(GetConfiguracion<string>("UrlKMZ"));

                    var tecnicos = MemoryLoadManager.ListaUsuarios.Where(x => x.fnLatitud != 0).ToList();
                    //await kmzService.LoadKMZFromPath();
                    var listaInfoOnus = conetion.sp_MapaConInfoOnuyOlt().ToList();

                    //foreach(var item in kmzService.Placemarks)
                    //{
                    //    listaInfoOnus.Add(new sp_MapaConInfoOnuyOlt_Result
                    //    {
                    //        fcNombreCliente = item.Name,
                    //        fcGeolocalizacion = item.Latitude + "," + item.Longitude,
                    //        fcDescripcion = "MUFA",
                    //        fcRGB = "128,128,0"
                    //    });
                    //}

                    listaInfoOnus.Add(new sp_MapaConInfoOnuyOlt_Result
                    {
                        fcNombreCliente = User?.Identity?.Name ?? "USUARIO",
                        fcDescripcion = "Mi Ubicacion",
                        fcRGB = "0,255,255"
                    });

                    foreach (var item in tecnicos)
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
                    //EnviarNotificacion("DESDE BASE SOLICITUDES");
                    return lista;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }



        public JsonResult GetInventarioPendientePorFirmar()
        {
            using (var contexto = new ORIONDBEntities())
            {
                var lista = contexto.sp_Inventario_Movimiento_Maestro_PendientesPorUsuario(GetIdUser()).ToList();

                return EnviarListaJson(lista);
            }
        }


        public ActionResult FirmaInventarioSalida(int fiIDInventarioMovimientoMaestro)
        {
            using (var contexto = new ORIONDBEntities())
            {

                var modelDb = contexto.sp_Inventario_Movimiento_Maestro_GetById(fiIDInventarioMovimientoMaestro).FirstOrDefault();
                var modelDbDetalle = contexto.sp_Inventario_Movimiento_Detalle_GetIDInventarioMovimientoMaestro(fiIDInventarioMovimientoMaestro).ToList();

                var model = new CrearSalidaInventarioViewModel();
                model.fiIDInventarioMovimientoMaestro = modelDb.fiIDInventarioMovimientoMaestro;
                model.fiIdUbicacion = modelDb.fiIDUbicacion;
                model.fcUbicacion = modelDb.fcUbicacion;
                model.fcDestinatario = modelDb.fcDestinatario;
                model.fdFechaCreacion = modelDb.fdFechaCreacion;
                model.ListaInventarioDetalle = new List<ListInventarioMovimientoDetalleViewModel>();
                model.UsuarioPeticion = User.Identity.Name;
                var i = 1;
                foreach (var item in modelDbDetalle)
                {
                    model.ListaInventarioDetalle.Add(new ListInventarioMovimientoDetalleViewModel
                    {
                        index = i++,
                        fcProducto = item?.fcProducto ?? "",
                        fcCodigoSerie1 = item?.fcCodigoSerie1 ?? "",
                        fcCodigoSerie2 = item?.fcCodigoSerie2 ?? "",
                        fnCantidad = item?.fnCantidad ?? 0
                    });
                }
                return PartialView(model);
            }
        }

        public ActionResult PaginasHacerPruebas()
        {
            return View();
        }


        public ActionResult ListaProductosPorUsuario()
        {
            using (var contexto = new ORIONDBEntities())
            {

                var lista = new List<ListProductosPorUbicacionViewModel>();
                var listUbicaciones = new List<ListUbicacionesPorUsuarioViewModel>();


                var ubicaciones = ListarUbicacionesPorTipo();
                var ubicacionUsuario = ubicaciones.Where(x => x.fcTipoUbicacion == "USUARIO").FirstOrDefault(x => x.fcUbicacion.Split('-')[0] == GetIdUser().ToString());
                var productosLocal = contexto.sp_InventarioPorUbicacionDetallado(ubicacionUsuario.fiIDUbicacion).ToList();
                var bodegas = contexto.sp_UbicacionesPorUsuarios_ListarPorUsuario(GetIdUser()).Where(x => x.fiIDUbicacion != ubicacionUsuario.fiIDUbicacion).ToList();


                if (productosLocal != null)
                {
                    if (productosLocal.Sum(x => x.fnCantidad) > 0)
                    {
                        foreach (var inv in productosLocal)
                        {
                            lista.Add(new ListProductosPorUbicacionViewModel
                            {
                                fiIDMovimiento = inv.fiIDMovimiento ?? 0,
                                fcCodigoSerie1 = inv.fcCodigoSerie1,
                                fcCodigoSerie2 = inv.fcCodigoSerie2,
                                fcProducto = inv.fcProducto,
                                fcReferenciaMovimiento = inv.fcReferenciaMovimiento,
                                fcUbicacion = "MIS PRODUCTOS ASIGNADOS",
                                fiIDProducto = inv.fiIDProducto ?? 0,
                                fiIDUbicacion = inv.fiIDUbicacion,
                                fnCantidad = inv.fnCantidad,
                                fbUsuarioPrincipal = true,
                                fbSeleccionado = false,
                                fbConsumible = inv.fbCodigoSerie ?? false
                            });
                        }
                    }
                }





                foreach (var item in bodegas)
                {
                    var inventario = contexto.sp_InventarioPorUbicacionDetallado(item.fiIDUbicacion).ToList();
                    var ubicacionItem = ubicaciones.FirstOrDefault(x => x.fiIDUbicacion == item.fiIDUbicacion);
                    var nombreUbicacion = "";

                    if (ubicacionItem.fcTipoUbicacion == "BODEGA") nombreUbicacion = $"BODEGA: {ubicacionItem.fcUbicacion.ToUpper()}";
                    if (ubicacionItem.fcTipoUbicacion == "USUARIO") nombreUbicacion = $"BODEGA DE USUARIO: {ubicacionItem.fcNombreCorto.ToUpper()} EMPRESA {ubicacionItem.fcNombreComercial.ToUpper()}";
                    if (ubicacionItem.fcTipoUbicacion == "CLIENTE") nombreUbicacion = $"CLIENTE: {ubicacionItem.fcUbicacion.ToUpper()} | {ubicacionItem.fcNombreCorto.ToUpper()}";

                    foreach (var inv in inventario)
                    {
                        lista.Add(new ListProductosPorUbicacionViewModel
                        {
                            fiIDMovimiento = inv.fiIDMovimiento ?? 0,
                            fcCodigoSerie1 = inv.fcCodigoSerie1,
                            fcCodigoSerie2 = inv.fcCodigoSerie2,
                            fcProducto = inv.fcProducto,
                            fcReferenciaMovimiento = inv.fcReferenciaMovimiento,
                            fcUbicacion = nombreUbicacion,
                            fiIDProducto = inv.fiIDProducto ?? 0,
                            fiIDUbicacion = inv.fiIDUbicacion,
                            fnCantidad = inv.fnCantidad,
                            fbUsuarioPrincipal = item.fbUsuarioPrincipal ?? false,
                            fbSeleccionado = false,
                            fbConsumible = inv.fbCodigoSerie ?? false
                        });
                    }


                }

                var grupo = lista.GroupBy(x => x.fiIDUbicacion).Select(x => new ListUbicacionesPorUsuarioViewModel
                {
                    fiIDUbicacion = x.Key,
                    fcUbicacion = x.FirstOrDefault().fcUbicacion,
                    Productos = x.ToList(),
                    fbUsuarioPrincipal = x.FirstOrDefault().fbUsuarioPrincipal
                }).ToList();
                //listUbicaciones.Add();


                return PartialView(grupo);
            }

        }

        [HttpPost]
        public ActionResult ViewMovimientoInventario(ListUbicacionesPorUsuarioViewModel model)
        {

            using (var context = new OrionSecurityEntities())
            {
                using (var contexto = new ORIONDBEntities())
                {
                    var SalidaInventario = new CrearSalidaInventarioViewModel
                    {
                        fbEditar = false,
                        fiIdUbicacion = model.fiIDUbicacion,
                        fcUbicacion = model.fcUbicacion,
                        fiIDTipoMomvimento = 2,
                        fdFechaCreacion = DateTime.Now,
                        fiIDUsuarioDestino = 0,
                        fiIDInventarioMovimientoMaestro = 0
                    };

                    SalidaInventario.ListaInventarioDetalle = new List<ListInventarioMovimientoDetalleViewModel>();
                    var i = 0;
                    foreach (var item in model.Productos)
                    {
                        var detalle = new ListInventarioMovimientoDetalleViewModel
                        {
                            fbPorCodigo = item.fbConsumible,
                            fbEditado = true,
                            fcCodigoSerie1 = item.fcCodigoSerie1,
                            fcCodigoSerie2 = item.fcCodigoSerie2,
                            fcDescripcion = item.fcReferenciaMovimiento,
                            fcPrecios = "",
                            fcProducto = item.fcProducto,
                            fcToken = "",
                            fcUbicacionDestino = "",
                            fcUbicacionInicial = model.fcUbicacion,
                            fiIDInventarioMovimientoDetalle = 0,
                            fiIDInventarioMovimientoMaestro = 0,
                            fiIDMovimiento = item.fiIDMovimiento,
                            fiIdProducto = item.fiIDProducto,
                            fiIDTipoMovimiento = 2,
                            fiIDUbicacionDestino = 0,
                            fiIDUbicacionInicial = model.fiIDUbicacion,
                            fnCantidad = item.fbConsumible ? item.fnCantidad : 0,
                            index = i
                        };
                        i++;
                        SalidaInventario.ListaInventarioDetalle.Add(detalle);
                    }

                    var usuario = GetUser();

                    var RolesAdmin = GetConfiguracion<int>("RolesPermisoMovInvent", ',');

                    ViewBag.BagPermisoTotal = RolesAdmin.Contains(GetUser().IdRol);

                    ViewBag.ListUbicaciones = GetListUbicaciones();
                    ViewBag.ListUbicacionesExternos = GetListUbicacionesExternos(usuario);
                    return PartialView(SalidaInventario);
                }
            }
        }


        [HttpGet]
        public ActionResult LogSistema()
        {
            return View();
        }


        public ActionResult RentabilidadPaquetes()
        {
            using (var contexto = new ORIONDBEntities())
            {
                var ArticulosExtra = GetConfiguracion<int>("Productos_Que_No_Lleva_Contratista", ',').ToList();
                var ArticulosRed = GetConfiguracion<int>("Id_Productos_que_Sirven_Para_Red", ',').ToList();

                ViewBag.ListaEquipos = _connection.OrionContext.sp_Producto_Maestro_Listar().Where(a => ArticulosRed.Any(b => b == a.fiIDProducto)).ToList().Select(a => new SelectListItem { Value = a.fnValorProductoMN.ToString(), Text = a.fcProducto });
                ViewBag.ListaAdicionales = _connection.OrionContext.sp_Producto_Maestro_Listar().Where(a => ArticulosExtra.Any(b => b == a.fiIDProducto)).ToList().Select(a => new SelectListItem { Value = a.fnValorProductoMN.ToString(), Text = a.fcProducto });
                return View();
            }
        }



        public JsonResult ListaSolicitudes()
        {
            try
            {
                var jsonResult = new JsonResult();
                var Roles_Contratista_Bandeja = GetConfiguracion<string>("Roles_Contratista_Bandeja", ',').Select(x => _connection.OrionContext.sp_Roles_ObtenerPorRole(x).FirstOrDefault()).ToList();

                var usuaruiologueado = GetUser();
                var listadoTecnico = _connection.OrionContext.sp_SolicitudesAsignadas_By_Tecnico(usuaruiologueado.fiIdUsuario, usuaruiologueado.IdRol).ToList();

                jsonResult = EnviarListaJson(listadoTecnico);

                return jsonResult;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}