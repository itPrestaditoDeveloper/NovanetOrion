using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models.DatosCliente;
using OrionCoreCableColor.Models.Mantenimiento;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace OrionCoreCableColor.Controllers
{
    [Authorize(Roles = "orion_AccesoMantenimientos")]
    public class MantenimientoController : BaseController
    {


        #region Marcas
        public ActionResult IndexCatalogoMarcas()
        {
            return View();
        }


        public JsonResult CargarListaCatalogoMarcas()
        {
            using (var context = new ORIONDBEntities())
            {
                return EnviarListaJson(context.sp_Catalogo_Marcas_Listar().ToList());
            }
        }
        [HttpGet]
        public ActionResult CrearCatalogoMarca()
        {
            return PartialView(new CrearCatalogoMarcaViewModel());
        }


        [HttpPost]
        public JsonResult CrearCatalogoMarca(CrearCatalogoMarcaViewModel model)
        {

            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var list = contexto.sp_Catalogo_Marcas_Listar().ToList();

                    if (list.Any(x => x.fcMarca == model.fcMarca))
                    {
                        return EnviarResultado(false, "", "La marca ya fue registrada");
                    }

                    var result = contexto.sp_Catalogo_Marcas_Crear(model.fcMarca, GetIdUser()) > 0;
                    return EnviarResultado(result, "", result ? "Marca registrada exitosamente" : "Error de red");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "error");
                }

            }

        }


        [HttpGet]
        public ActionResult EditarCatalogoMarca(int fiIdMarca)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Catalogo_Marcas_GetById(fiIdMarca).FirstOrDefault();
                var model = new CrearCatalogoMarcaViewModel
                {
                    fiIdMarca = modelDb.fiIDMarca,
                    fcMarca = modelDb.fcMarca,
                    fbEditar = true
                };

                return PartialView("CrearCatalogoMarca", model);
            }
        }

        [HttpPost]
        public JsonResult EditarCatalogoMarca(CrearCatalogoMarcaViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var list = contexto.sp_Catalogo_Marcas_Listar().Where(x => x.fiIDMarca != model.fiIdMarca).ToList();
                    if (list.Any(x => x.fcMarca == model.fcMarca))
                    {
                        return EnviarResultado(false, "", "La marca ya fue registrada");
                    }

                    var result = contexto.sp_Catalogo_Marcas_Editar(model.fiIdMarca, model.fcMarca, GetIdUser()) > 0;
                    return EnviarResultado(result, "", result ? "Marca Editada exitosamente" : "Error de red");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }
        #endregion


        #region TipoProducto

        [HttpGet]
        public ActionResult IndexCatalogoTipoProducto()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarListaCatalogoTipoProductos()
        {
            using (var context = new ORIONDBEntities())
            {
                return EnviarListaJson(context.sp_Catalogo_TipoProducto_Listar().ToList());
            }
        }

        [HttpGet]
        public ActionResult CrearCatalogoTipoProducto()
        {
            return PartialView(new CrearCatalogoTipoProductoViewModel());
        }

        [HttpPost]
        public JsonResult CrearCatalogoTipoProducto(CrearCatalogoTipoProductoViewModel model)
        {

            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var list = contexto.sp_Catalogo_TipoProducto_Listar().ToList();

                    if (list.Any(x => x.fcTipoProducto == model.fcTipoProducto))
                    {
                        return EnviarResultado(false, "", "La tipo producto ya fue registrado");
                    }

                    var result = contexto.sp_Catalogo_TipoProducto_Crear(model.fcTipoProducto, GetIdUser()) > 0;
                    return EnviarResultado(result, "", result ? "Tipo Producto registrado exitosamente" : "Error de red");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "error");
                }

            }

        }

        [HttpGet]
        public ActionResult EditarCatalogoTipoProducto(int fiIdTipoProducto)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Catalogo_TipoProducto_GetById(fiIdTipoProducto).FirstOrDefault();
                var model = new CrearCatalogoTipoProductoViewModel
                {
                    fiIDTipoProducto = modelDb.fiIDTipoProducto,
                    fcTipoProducto = modelDb.fcTipoProducto,
                    fbEditar = true
                };

                return PartialView("CrearCatalogoTipoProducto", model);
            }
        }


        [HttpPost]
        public JsonResult EditarCatalogoTipoProducto(CrearCatalogoTipoProductoViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var list = contexto.sp_Catalogo_TipoProducto_Listar().Where(x => x.fiIDTipoProducto != model.fiIDTipoProducto).ToList();
                    if (list.Any(x => x.fcTipoProducto == model.fcTipoProducto))
                    {
                        return EnviarResultado(false, "", "El tipo producto ya fue registrado");
                    }

                    var result = contexto.sp_Catalogo_TipoProducto_Editar(model.fiIDTipoProducto, model.fcTipoProducto, GetIdUser()) > 0;
                    return EnviarResultado(result, "", result ? "Tipo producto Editado exitosamente" : "Error de red");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }
        #endregion


        #region Ubicaciones
        public ActionResult IndexCatalogoUbicaciones()
        {
            return View();
        }


        [HttpGet]
        public JsonResult CargarListaCatalogoUbicacion()
        {
            using (var context = new ORIONDBEntities())
            {
                return EnviarListaJson(context.sp_Catalogo_Ubicaciones_Listar().ToList());
            }
        }

        [HttpGet]
        public ActionResult CrearCatalogoUbicacion()
        {
            return PartialView(new CrearCatalogoUbicacionesViewModel());
        }

        [HttpPost]
        public JsonResult CrearCatalogoUbicacion(CrearCatalogoUbicacionesViewModel model)
        {

            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var list = contexto.sp_Catalogo_Ubicaciones_Listar().ToList();

                    if (list.Any(x => x.fcUbicacion == model.fcUbicacion))
                    {
                        return EnviarResultado(false, "", "La ubicacion ya fue registrada");
                    }

                    //var piIDUbicacion = new SqlParameter("pnTasaDeCambio", SqlDbType.Int)
                    //{
                    //    Direction = System.Data.ParameterDirection.Output
                    //};

                    var piIDUbicacion = new ObjectParameter("piIDUbicacion", typeof(int));

                    var result = contexto.sp_Catalogo_Ubicaciones_Crear(model.fcUbicacion, model.fcUbicacionFisica, GetIdUser(), piIDUbicacion, 0) > 0;
                    return EnviarResultado(result, "", result ? "Ubicacion registrada exitosamente" : "Error de red");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "error");
                }

            }

        }

        [HttpGet]
        public ActionResult EditarCatalogoUbicacion(int fiIdUbicacion)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Catalogo_Ubicaciones_GetById(fiIdUbicacion).FirstOrDefault();
                var model = new CrearCatalogoUbicacionesViewModel
                {
                    fiIDUbicacion = modelDb.fiIDUbicacion,
                    fcUbicacion = modelDb.fcUbicacion,
                    fcUbicacionFisica = modelDb.fcUbicacionFisica,
                    fbEditar = true
                };

                return PartialView("CrearCatalogoUbicacion", model);
            }
        }

        [HttpPost]
        public JsonResult EditarCatalogoUbicacion(CrearCatalogoUbicacionesViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var list = contexto.sp_Catalogo_Ubicaciones_Listar().Where(x => x.fiIDUbicacion != model.fiIDUbicacion).ToList();
                    if (list.Any(x => x.fcUbicacion == model.fcUbicacion))
                    {
                        return EnviarResultado(false, "", "La ubicacion ya fue registrada");
                    }

                    var result = contexto.sp_Catalogo_Ubicaciones_Editar(model.fiIDUbicacion, model.fcUbicacion, model.fcUbicacionFisica, GetIdUser(), 1) > 0;
                    return EnviarResultado(result, "", result ? "Ubicacion editada exitosamente" : "Error de red");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }


        [HttpGet]
        public JsonResult CambiarStatusUbicacion(int fiIdUbicacion)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Catalogo_Ubicaciones_GetById(fiIdUbicacion).FirstOrDefault();
                var result = contexto.sp_Catalogo_Ubicaciones_Editar(modelDb.fiIDUbicacion, modelDb.fcUbicacion, modelDb.fcUbicacionFisica, GetIdUser(), modelDb.fiEstadoUbicacion == 1 ? 0 : 1) > 0;

                return EnviarResultado(result, "", result ? modelDb.fiEstadoUbicacion == 1 ? "Ubicacion deshabilitada exitosamente" : "Ubicacion habilitada exitosamente" : "Error de red");
            }
        }

        #endregion



        #region Modelos
        [HttpGet]
        public ActionResult IndexCatalogoModelos()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarListaCatalogoModelo()
        {
            using (var context = new ORIONDBEntities())
            {
                return EnviarListaJson(context.sp_Catalogo_Modelos_Listar().ToList());
            }
        }



        [HttpGet]
        public ActionResult CrearCatalogoModelo()
        {

            ViewBag.ListarMarcas = GetListMarcas();
            return PartialView(new CrearCatalogoModeloViewModel());

        }

        [HttpPost]
        public JsonResult CrearCatalogoModelo(CrearCatalogoModeloViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var list = contexto.sp_Catalogo_Modelos_Listar().ToList();

                    if (list.Any(x => x.fcModelo == model.fcModelo && x.fiIDMarca == model.fiIdMarca))
                    {
                        return EnviarResultado(false, "", "El modelo ya fue registrado");
                    }

                    var result = contexto.sp_Catalogo_Modelos_Crear(model.fiIdMarca, model.fcModelo, GetIdUser()) > 0;
                    return EnviarResultado(result, "", result ? "Modelo registrado exitosamente" : "Error de red");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "error");
                }
            }
        }


        [HttpGet]
        public ActionResult EditarCatalogoModelo(int fiIdModelo)
        {
            ViewBag.ListarMarcas = GetListMarcas();
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Catalogo_Modelos_GetById(fiIdModelo).FirstOrDefault();
                var model = new CrearCatalogoModeloViewModel
                {
                    fiIDModelo = modelDb.fiIDModelo,
                    fiIdMarca = modelDb.fiIDMarca,
                    fcModelo = modelDb.fcModelo,
                    fbEditar = true
                };
                return PartialView("CrearCatalogoModelo", model);
            }
        }

        [HttpPost]
        public JsonResult EditarCatalogoModelo(CrearCatalogoModeloViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var list = contexto.sp_Catalogo_Modelos_Listar().Where(x => x.fiIDModelo != model.fiIDModelo).ToList();
                    if (list.Any(x => x.fcModelo == model.fcModelo && x.fiIDMarca == model.fiIdMarca))
                    {
                        return EnviarResultado(false, "", "El modelo ya fue registrado");
                    }

                    var result = contexto.sp_Catalogo_Modelos_Editar(model.fiIDModelo, model.fiIdMarca, model.fcModelo, GetIdUser()) > 0;
                    return EnviarResultado(result, "", result ? "Modelo editado exitosamente" : "Error de red");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }
        #endregion


        #region TipoMovimientos
        [HttpGet]
        public ActionResult IndexCatalogoTipoMovimientos()
        {
            return View();
        }

        [HttpGet]
        public JsonResult CargarListaCatalogoTipoMovimientos()
        {
            using (var context = new ORIONDBEntities())
            {
                return EnviarListaJson(context.sp_Catalogo_TipoMovimiento_Listar().ToList());
            }
        }


        [HttpGet]
        public ActionResult CrearTipoMovimiento()
        {
            ViewBag.ListarTipoAfectacion = GetListTipoAfectacion();
            return PartialView(new CrearCatalogoTipoMovimientoViewModel());
        }

        [HttpPost]
        public ActionResult CrearTipoMovimiento(CrearCatalogoTipoMovimientoViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var list = contexto.sp_Catalogo_TipoMovimiento_Listar().ToList();
                    if (list.Any(x => x.fcTipoMovimiento == model.fcTipoMovimiento))
                    {
                        return EnviarResultado(false, "", "El tipo de movimiento ya fue registrado");
                    }

                    var result = contexto.sp_Catalogo_TipoMovimiento_Crear(model.fcTipoMovimiento, model.fiTipoAfectacion, GetIdUser()) > 0;
                    return EnviarResultado(result, "", result ? "Tipo de movimiento registrado exitosamente" : "Error de red");

                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "error");
                }
            }
        }

        [HttpGet]
        public ActionResult EditarTipoMovimiento(int fiIDTipoMovimiento)
        {
            using (var contexto = new ORIONDBEntities())
            {
                ViewBag.ListarTipoAfectacion = GetListTipoAfectacion();
                var modelDb = contexto.sp_Catalogo_TipoMovimiento_GetById(fiIDTipoMovimiento).FirstOrDefault();
                var model = new CrearCatalogoTipoMovimientoViewModel
                {
                    fiIDTipoMovimiento = modelDb.fiIDTipoMovimiento,
                    fcTipoMovimiento = modelDb.fcTipoMovimiento,
                    fiTipoAfectacion = modelDb.fiTipoAfectacion,
                    fbEditar = true
                };
                return PartialView("CrearTipoMovimiento", model);
            }
        }


        [HttpPost]
        public ActionResult EditarTipoMovimiento(CrearCatalogoTipoMovimientoViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var list = contexto.sp_Catalogo_TipoMovimiento_Listar().Where(x => x.fiIDTipoMovimiento != model.fiIDTipoMovimiento).ToList();
                    if (list.Any(x => x.fcTipoMovimiento == model.fcTipoMovimiento))
                    {
                        return EnviarResultado(false, "", "El tipo de movimiento ya fue registrado");
                    }
                    var result = contexto.sp_Catalogo_TipoMovimiento_Editar(model.fiIDTipoMovimiento, model.fcTipoMovimiento, model.fiTipoAfectacion, GetIdUser()) > 0;
                    return EnviarResultado(result, "", result ? "Tipo movimiento editado exitosamente" : "Error de red");

                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }


        #endregion


        #region Paquetes
        public ActionResult IndexPaquetes()
        {
            return View();
        }


        public JsonResult CargarListaPaquetes()
        {
            using (var context = new ORIONDBEntities())
            {
                var list = context.sp_Paquetes_List().ToList();
                return EnviarListaJson(list);
            }
        }




        public ActionResult ListProductos()
        {
            return PartialView();
        }


        [HttpPost]
        public JsonResult CargarListaProductos(List<ListPaqueteDetalleViewModel> productos)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var lista = contexto.sp_Producto_Maestro_Listar().ToList();
                var listproductos = new List<ListPaqueteDetalleViewModel>();
                foreach (var item in lista)
                {
                    var itemProducto = productos?.FirstOrDefault(x => x.fiIDProducto == item.fiIDProducto) ?? null;
                    listproductos.Add(new ListPaqueteDetalleViewModel
                    {
                        fiIDProducto = item.fiIDProducto,
                        fcProducto = item.fcProducto,
                        fbSeleccionado = itemProducto?.fbSeleccionado ?? false,
                        fiIdPaquete = itemProducto?.fiIdPaquete ?? 0,
                        fiCantidad = itemProducto?.fiCantidad ?? 0,
                        fcToken = itemProducto?.fcToken ?? ""
                    });
                }

                return EnviarListaJson(listproductos);
            }
        }


        public ActionResult CrearPaquete()
        {
            ViewBag.ListaMonedas = GetListMonedas();
            return PartialView(new CrearPaqueteViewModel { fiIDMoneda = 2, fbServicio = true });
        }

        [HttpPost]
        public JsonResult CrearPaquete(CrearPaqueteViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var result = contexto.sp_Paquetes_Maestro_Insertar(GetIdUser(), model.fcPaquete, model.fnValorMensual, model.fnTasaDeInteresAnual, model.fiIDMoneda, model.fbServicio).FirstOrDefault() ?? 0;
                    if (result > 0)
                    {
                        foreach (var producto in model.Productos)
                        {
                            contexto.sp_Paquetes_Detalle_Insertar(result, producto.fiIDProducto, producto.fiCantidad);
                        }
                    }
                    return EnviarResultado(result > 0, "Paquete", result > 0 ? "Paquete Creado" : "Error de red");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }




        public ActionResult EditarPaquete(int fiIDPaquete)
        {
            ViewBag.ListaMonedas = GetListMonedas();
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                var model = new CrearPaqueteViewModel();

                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_Paquetes_Listar {fiIDPaquete}";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    model = db.ObjectContext.Translate<CrearPaqueteViewModel>(reader).FirstOrDefault();
                    reader.NextResult();
                    model.Productos = db.ObjectContext.Translate<ListPaqueteDetalleViewModel>(reader).ToList();
                }
                connection.Close();
                model.Productos.ForEach(x => { x.fbSeleccionado = true; });
                model.fbEditar = true;
                model.fbServicio = model.fbSoloServicio;
                return PartialView("CrearPaquete", model);
            }
        }

        [HttpPost]
        public ActionResult EditarPaquete(CrearPaqueteViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var result = contexto.sp_Paquetes_Maestro_Actualizar(GetIdUser(), model.fiIdPaquete, model.fcPaquete, model.fnValorMensual, model.fnTasaDeInteresAnual, 1, model.fiIDMoneda, model.fbServicio) > 0;
                    if (result)
                    {
                        contexto.sp_Paquetes_Detalle_Eliminar(model.fiIdPaquete);
                        foreach (var producto in model.Productos)
                        {
                            contexto.sp_Paquetes_Detalle_Insertar(model.fiIdPaquete, producto.fiIDProducto, producto.fiCantidad);
                        }
                    }

                    return EnviarResultado(result, "Paquete", result ? "Paquete Editado" : "Error de red");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }


            }
        }


        public ActionResult AsignarUbicaciones(int piIDPaquete)
        {
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "EXEC sp_Listado_Geo_Municipio";
                var municipios = new List<Geo_Municipio>();

                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    municipios = db.ObjectContext.Translate<Geo_Municipio>(reader).ToList();
                }

                command = connection.CreateCommand();
                command.CommandText = "EXEC sp_PaquetesPorUbicacion_Listado_PorPaquete @piIDPaquete";
                command.Parameters.Add(new SqlParameter("@piIDPaquete", piIDPaquete));

                var municipiosAsociados = new List<Paquetes_Maestro_Ubicacion>();
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    municipiosAsociados = db.ObjectContext.Translate<Paquetes_Maestro_Ubicacion>(reader).ToList();
                }

                foreach (var municipio in municipios)
                {
                    municipio.fbSeleccion = municipiosAsociados.Any(ma => ma.fiCodMunicipio == municipio.fiCodMunicipio && ma.fiCodDepartamento == municipio.fiCodDepartamento && ma.fiCodPais == municipio.fiCodPais);
                }

                connection.Close();
                ViewBag.piIDPaquete = piIDPaquete;

                return PartialView("AsignarUbicaciones", municipios);
            }
        }

        [HttpPost]
        public ActionResult GuardarUbicaciones(int piIDPaquete, List<Geo_Municipio> municipiosSeleccionados)
        {
            try
            {
                if (municipiosSeleccionados == null || !municipiosSeleccionados.Any())
                {
                    return EnviarResultado(false, "", "No se seleccionaron municipios para guardar.");
                }

                using (var dbContext = new ORIONDBEntities())
                using (var connection = new SqlConnection(dbContext.Database.Connection.ConnectionString))
                {
                    connection.Open();

                    // 1. Obtener los municipios actualmente asociados al paquete
                    var municipiosAsociados = new List<Paquetes_Maestro_Ubicacion>();
                    using (var command = new SqlCommand("EXEC sp_PaquetesPorUbicacion_Listado_PorPaquete @piIDPaquete", connection))
                    {
                        command.Parameters.Add(new SqlParameter("@piIDPaquete", piIDPaquete));

                        using (var reader = command.ExecuteReader())
                        {
                            var db = ((IObjectContextAdapter)dbContext).ObjectContext;
                            municipiosAsociados = db.Translate<Paquetes_Maestro_Ubicacion>(reader).ToList();
                        }
                    }

                    // 2. Identificar municipios a deshabilitar
                    var municipiosADeshabilitar = municipiosAsociados
                        .Where(ma => !municipiosSeleccionados.Any(ms =>
                            ms.fiCodPais == ma.fiCodPais &&
                            ms.fiCodDepartamento == ma.fiCodDepartamento &&
                            ms.fiCodMunicipio == ma.fiCodMunicipio))
                        .ToList();

                    // Usuario autenticado
                    var usuario = GetUser();
                    if (usuario == null)
                    {
                        return EnviarResultado(false, "", "Usuario no autenticado.");
                    }

                    var resultadoMensajes = new List<string>();

                    // 3. Insertar o actualizar los municipios seleccionados
                    foreach (var municipio in municipiosSeleccionados)
                    {
                        using (var insertCommand = new SqlCommand("EXEC sp_ActualizarPaquetesPorUbicacion @fiIDPaquete, @fiCodPais, @fiCodDepartamento, @fiCodMunicipio, @fiIDUsuarioCreador, @fiIDUsuarioUltimaModificacion, @Deshabilitar, @Resultado OUTPUT", connection))
                        {
                            insertCommand.Parameters.AddWithValue("@fiIDPaquete", piIDPaquete);
                            insertCommand.Parameters.AddWithValue("@fiCodPais", municipio.fiCodPais);
                            insertCommand.Parameters.AddWithValue("@fiCodDepartamento", municipio.fiCodDepartamento);
                            insertCommand.Parameters.AddWithValue("@fiCodMunicipio", municipio.fiCodMunicipio);
                            insertCommand.Parameters.AddWithValue("@fiIDUsuarioCreador", usuario.fiIdUsuario);
                            insertCommand.Parameters.AddWithValue("@fiIDUsuarioUltimaModificacion", usuario.fiIdUsuario);
                            insertCommand.Parameters.AddWithValue("@Deshabilitar", 0);

                            var resultadoParam = new SqlParameter("@Resultado", SqlDbType.NVarChar, 1000) { Direction = ParameterDirection.Output };
                            insertCommand.Parameters.Add(resultadoParam);

                            insertCommand.ExecuteNonQuery();
                            resultadoMensajes.Add(resultadoParam.Value.ToString());
                        }
                    }

                    // 4. Deshabilitar los municipios que ya no están seleccionados
                    foreach (var municipio in municipiosADeshabilitar)
                    {
                        using (var disableCommand = new SqlCommand("EXEC sp_ActualizarPaquetesPorUbicacion @fiIDPaquete, @fiCodPais, @fiCodDepartamento, @fiCodMunicipio, @fiIDUsuarioCreador, @fiIDUsuarioUltimaModificacion, @Deshabilitar, @Resultado OUTPUT", connection))
                        {
                            disableCommand.Parameters.AddWithValue("@fiIDPaquete", piIDPaquete);
                            disableCommand.Parameters.AddWithValue("@fiCodPais", municipio.fiCodPais);
                            disableCommand.Parameters.AddWithValue("@fiCodDepartamento", municipio.fiCodDepartamento);
                            disableCommand.Parameters.AddWithValue("@fiCodMunicipio", municipio.fiCodMunicipio);
                            disableCommand.Parameters.AddWithValue("@fiIDUsuarioCreador", usuario.fiIdUsuario);
                            disableCommand.Parameters.AddWithValue("@fiIDUsuarioUltimaModificacion", usuario.fiIdUsuario);
                            disableCommand.Parameters.AddWithValue("@Deshabilitar", 1);

                            var resultadoParam = new SqlParameter("@Resultado", SqlDbType.NVarChar, 1000) { Direction = ParameterDirection.Output };
                            disableCommand.Parameters.Add(resultadoParam);

                            disableCommand.ExecuteNonQuery();
                            resultadoMensajes.Add(resultadoParam.Value.ToString());
                        }
                    }

                    connection.Close();

                    var resultadoFinal = string.Join(" ", resultadoMensajes).Trim();
                    bool exito = resultadoFinal.Contains("Registro creado exitosamente") || resultadoFinal.Contains("Registro actualizado exitosamente");

                    return EnviarResultado(exito, "", exito ? "Registro creado exitosamente" : "Ha ocurrido un error Inesperado");
                }
            }
            catch (Exception ex)
            {
                return EnviarResultado(false, "", $"Error al guardar las ubicaciones: {ex.Message}");
            }
        }


        public ActionResult ClonarPaquete(int fiIDPaquete)
        {
            ViewBag.ListaMonedas = GetListMonedas();
            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                var model = new CrearPaqueteViewModel();

                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_Paquetes_Listar {fiIDPaquete}";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    model = db.ObjectContext.Translate<CrearPaqueteViewModel>(reader).FirstOrDefault();
                    reader.NextResult();
                    model.Productos = db.ObjectContext.Translate<ListPaqueteDetalleViewModel>(reader).ToList();
                }
                connection.Close();
                model.Productos.ForEach(x => { x.fbSeleccionado = true; });
                model.fbEditar = false;
                model.fbClonar = true;
                return PartialView("CrearPaquete", model);
            }
        }

        public ActionResult ViewListaProductosPaquete(int fiIDPaquete)
        {
            using (var contexto = new ORIONDBEntities())
            {
                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {
                    var model = new CrearPaqueteViewModel();

                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_Paquetes_Listar {fiIDPaquete}";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        model = db.ObjectContext.Translate<CrearPaqueteViewModel>(reader).FirstOrDefault();
                        reader.NextResult();
                        model.Productos = db.ObjectContext.Translate<ListPaqueteDetalleViewModel>(reader).ToList();
                    }
                    connection.Close();
                    model.Productos.ForEach(x => { x.fbSeleccionado = true; });
                    model.fbEditar = false;
                    model.fbClonar = true;
                    return PartialView(model.Productos);
                }

            }
        }

        [HttpGet]
        public JsonResult CambiarStatusPaquete(int fiIDPaquete)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var model = new CrearPaqueteViewModel();

                contexto.Database.Connection.Open();
                var command = contexto.Database.Connection.CreateCommand();
                command.CommandText = $"EXEC sp_Paquetes_Listar {fiIDPaquete}";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    model = db.ObjectContext.Translate<CrearPaqueteViewModel>(reader).FirstOrDefault();
                }
                contexto.Database.Connection.Close();


                var result = contexto.sp_Paquetes_Maestro_Actualizar(GetIdUser(), fiIDPaquete, model.fcPaquete, model.fnValorMensual, model.fnTasaDeInteresAnual, (model.fiestadoPaquete == 1 ? 0 : 1), model.fiIDMoneda, model.fbSoloServicio) > 0;

                return EnviarResultado(result, "", result ? model.fiestadoPaquete == 1 ? "Paquete deshabilitado exitosamente" : "Paquete habilitado exitosamente" : "Error de red");
            }
        }

        #endregion

        #region OLT
        public ActionResult IndexListaOLT()
        {
            return View();
        }
        public JsonResult CargarListaCatalogoOLT()
        {
            using (var context = new ORIONDBEntities())
            {
                return EnviarListaJson(context.sp_OrionCatalogo_OLT(GetIdUser()).ToList());
            }
        }
        [HttpGet]
        public ActionResult CrearCatalogoOLT()
        {
            return PartialView(new CrearCatalogoOLTViewModel());
        }


        [HttpPost]
        public JsonResult CrearCatalogoOLT(CrearCatalogoOLTViewModel model)
        {

            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var list = contexto.sp_OrionCatalogo_OLT(GetIdUser()).ToList();
                    if (list.Any(x => x.fcDescripcionOLT == model.fcDescripcionOLT))
                    {
                        return EnviarResultado(false, "", "La OLT ya fue registrada");
                    }

                    var result = contexto.sp_OrionCatalogo_OLT_Guardar(model.fcDescripcionOLT, model.fcDireccion, model.fcLatitude, model.fcLongitud, model.fcIPOLT, GetIdUser()) > 0;
                    return EnviarResultado(result, "", result ? "OLT registrada exitosamente" : "Error de red");

                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "error");
                }

            }

        }

        [HttpGet]
        public ActionResult EditarDatosOLT(int fiIdOLT)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_OrionCatalogo_OLT_ObtenerPorID(fiIdOLT, 1).FirstOrDefault();
                var model = new CrearCatalogoOLTViewModel
                {
                    fiIDOlt = modelDb.fiIDOlt,
                    fcDescripcionOLT = modelDb.fcDescripcionOLT,
                    fcDireccion = modelDb.fcDireccion,
                    fcLatitude = modelDb.fcLatitude,
                    fcLongitud = modelDb.fcLongitud,
                    fcIPOLT = modelDb.fcIPOLT,
                    fbEditar = true
                };

                return PartialView("CrearCatalogoOLT", model);
            }
        }

        [HttpPost]
        public JsonResult EditarDatosOLT(CrearCatalogoOLTViewModel model)
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

                    var result = contexto.sp_OrionCatalogo_OLT_Editar(model.fiIDOlt, model.fcDescripcionOLT, model.fcDireccion, model.fcLatitude, model.fcLongitud, model.fcIPOLT, GetIdUser()) > 0;
                    return EnviarResultado(result, "", result ? "OLT Editada exitosamente" : "Error de red");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }

        [HttpPost]
        public JsonResult DeshabilitarOLT(int IdOlt)
        {
            using (var context = new ORIONDBEntities())
            {

                try
                {
                    var result = context.sp_OrionCatalogo_OLT_Desabilitar(IdOlt, GetIdUser()) > 0;
                    return EnviarResultado(true, "Datos", "Datos Actualizar");

                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }



            }
        }

        #endregion

        #region Configuraciones
        public ActionResult IndexConfiguraciones()
        {
            return View();
        }

        public JsonResult CargarListaConfiguracion()
        {
            using (var context = new ORIONDBEntities())
            {
                return EnviarListaJson(context.sp_Catalogo_Configuraciones_Listar().ToList());
            }
        }

        public ActionResult CrearCatalogoConfiguracion()
        {
            return PartialView(new CrearConfiguracionViewModel());
        }

        [HttpPost]
        public JsonResult CrearCatalogoConfiguracion(CrearConfiguracionViewModel model)
        {

            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var list = contexto.sp_Catalogo_Configuraciones_Listar().ToList();

                    if (list.Any(x => x.fcNombreLlave == model.fcNombreLlave))
                    {
                        return EnviarResultado(false, "", "La configuracion ya fue registrada");
                    }

                    var result = contexto.sp_Catalogo_Configuraciones_Crear(model.fcNombreLlave, model.fcValorLlave) > 0;
                    return EnviarResultado(result, "", result ? "Marca registrada exitosamente" : "Error de red");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "error");
                }

            }

        }

        [HttpGet]
        public ActionResult EditarCatalogoConfiguracion(int fiIdConfiguracion)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Catalogo_Configuraciones_ObtenerPorId(fiIdConfiguracion).FirstOrDefault();
                var model = new CrearConfiguracionViewModel
                {
                    fiIdConfiguraciones = modelDb.fiIdConfiguraciones,
                    fcNombreLlave = modelDb.fcNombreLlave,
                    fcValorLlave = modelDb.fcValorLlave,
                    fbEditar = true
                };

                return PartialView("CrearCatalogoConfiguracion", model);
            }
        }

        [HttpPost]
        public JsonResult EditarCatalogoConfiguracion(CrearConfiguracionViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var list = contexto.sp_Catalogo_Configuraciones_Listar().Where(x => x.fiIdConfiguraciones != model.fiIdConfiguraciones).ToList();
                    if (list.Any(x => x.fcNombreLlave == model.fcNombreLlave))
                    {
                        return EnviarResultado(false, "", "La configuracion ya fue registrada");
                    }

                    var result = contexto.sp_Catalogo_Configuraciones_Editar(model.fiIdConfiguraciones, model.fcValorLlave) > 0;
                    return EnviarResultado(result, "", result ? "Configuracion Editada exitosamente" : "Error de red");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }
        #endregion

        #region UbicacionesPorUsuarios
        public ActionResult IndexUbicacionesPorUsuarios()
        {
            return View();
        }


        public JsonResult CargarListaUbicacionPorUsuario()
        {
            using (var contexto = new ORIONDBEntities())
            {
                return EnviarListaJson(contexto.sp_UbicacionesPorUsuarios_Listar().ToList());
            }
        }

        [HttpGet]
        public ActionResult CrearUbicacionPorUsuario()
        {
            ViewBag.ListarUbicaciones = ListarUbicacionesPorTipo().Select(x => new SelectListItem
            {
                Value = x.fiIDUbicacion.ToString(),
                Text = x.fcTipoUbicacion == "CLIENTE" ? $"{x.fcTipoUbicacion}: {x.fcUbicacion} | {x.fcNombreCorto}" : $"{x.fcTipoUbicacion}: {x.fcUbicacion}"
            }).ToList();

            ViewBag.ListarEmpresas = GetListEmpresas();
            ViewBag.ListarUsuarios = GetListUsuarios();
            return PartialView(new CrearUbicacionPorUsuario());
        }

        [HttpPost]
        public JsonResult CrearUbicacionPorUsuario(CrearUbicacionPorUsuario model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var list = contexto.sp_UbicacionesPorUsuarios_Listar().ToList();

                    if (list.Any(x => x.fiIDUbicacion == model.fiIDUbicacion && x.fiIDUsuario == model.fiIDUsuario))
                    {
                        return EnviarResultado(false, "", "El usuario ya existe en esta bodega");
                    }

                    var result = contexto.sp_UbicacionesPorUsuarios_Insertar(model.fiIDEmpresa, model.fiIDUbicacion, model.fiIDUsuario, model.fbUsuarioPrincipal, GetIdUser()) > 0;

                    return EnviarResultado(result, "Usuario por Ubicacion", result ? "Creado" : "Error de red");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }

        [HttpGet]
        public ActionResult EditarUbicacionPorUsuario(int fiIDUbicacionPorUsuario)
        {
            ViewBag.ListarUbicaciones = ListarUbicacionesPorTipo().Select(x => new SelectListItem
            {
                Value = x.fiIDUbicacion.ToString(),
                Text = x.fcTipoUbicacion == "CLIENTE" ? $"{x.fcTipoUbicacion}: {x.fcUbicacion} | {x.fcNombreCorto}" : $"{x.fcTipoUbicacion}: {x.fcUbicacion}"
            }).ToList();

            ViewBag.ListarEmpresas = GetListEmpresas();
            ViewBag.ListarUsuarios = GetListUsuarios();
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_UbicacionesPorUsuarios_BusarPorId(fiIDUbicacionPorUsuario).FirstOrDefault();
                var model = new CrearUbicacionPorUsuario();
                model.fiIDUbicacionPorUsuario = modelDb.fiIDUbicacionPorUsuario;
                model.fiIDEmpresa = modelDb.fiIDEmpresa ?? 0;
                model.fiIDUbicacion = modelDb.fiIDUbicacion ?? 0;
                model.fiIDUsuario = modelDb.fiIDUsuario ?? 0;
                model.fbUsuarioPrincipal = modelDb.fbUsuarioPrincipal ?? false;
                model.fbEditar = true;

                return PartialView("CrearUbicacionPorUsuario", model);

            }
        }

        [HttpPost]
        public JsonResult EditarUbicacionPorUsuario(CrearUbicacionPorUsuario model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var list = contexto.sp_UbicacionesPorUsuarios_Listar().Where(x => x.fiIDUbicacionPorUsuario != model.fiIDUbicacionPorUsuario).ToList();
                    if (list.Any(x => x.fiIDUbicacion == model.fiIDUbicacion && x.fiIDUsuario == model.fiIDUsuario))
                    {
                        return EnviarResultado(false, "Error", "El usuario ya existe en esta bodega");
                    }

                    var result = contexto.sp_UbicacionesPorUsuarios_Editar(model.fiIDUbicacionPorUsuario, model.fiIDEmpresa, model.fiIDUbicacion, model.fiIDUsuario, model.fbUsuarioPrincipal, GetIdUser()) > 0;
                    return EnviarResultado(result, "Usuario por Ubicacion", result ? "Editado exitosamente" : "Error de red");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }

        [HttpGet]
        public JsonResult EliminarUbicacionPorUsuario(int fiIDUbicacionPorUsuario)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var result = contexto.sp_UbicacionesPorUsuarios_Eliminar(fiIDUbicacionPorUsuario) > 0;
                    return EnviarResultado(result, "Usuario por Ubicacion", result ? "Eliminado exitosamente" : "Error de red");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }
        #endregion

        #region Paquetes Plazos
        public async Task<ActionResult> ConfigurarPlazo(sp_Paquetes_List_Result model)
        {
            using(var contexto = new ORIONDBEntities())
            {
                var resultado = await contexto.Database.SqlQuery<ListCatalogoPlazos>("Exec sp_Catalogo_Plazos_Listar").ToListAsync();
                ViewBag.ListaPlazos = resultado.Where(x => x.fbEstadoPlazo == true).Select(x => new SelectListItem { Value = x.fiIDPlazo.ToString(), Text = x.fcDescripcionPlazo }).ToList();
                ViewBag.fiIDPaquete = model.fiIDPaquete;
                ViewBag.fcPaquete = model.fcPaquete;
                return PartialView();
            }
        }


        public async Task<JsonResult> CargarListaPlazos(int fiIDPaquete)
        {
            using(var contexto = new ORIONDBEntities())
            {
                var piIDPaquete = new SqlParameter("@piIDPaquete", fiIDPaquete);
                var resultado = await contexto.Database.SqlQuery<ListPlazoPorPaqueteViewModel>("Exec sp_Paquetes_Maestro_Plazo_ListarPorPaquete @piIDPaquete", piIDPaquete).ToListAsync();
                return Json(resultado.Where(x=>x.fiEstadoPlazo == 1).ToList(), JsonRequestBehavior.AllowGet);
            }
        }


        //await contexto.Database.ExecuteSqlCommandAsync("EXEC sp_OLT_Conexiones_Bitacora_Insertar @piIDOLT, @pbEstadoConexion ", piIDOLT, pbEstadoConexion);

        public async Task<JsonResult> GuardarConfiguracionPlazo(List<ListPlazoPorPaqueteViewModel> list)
        {

            using(var contexto = new ORIONDBEntities())
            {
                var piIDPaquete = new SqlParameter("@piIDPaquete", list.FirstOrDefault().fiIDPaquete);
                await contexto.Database.ExecuteSqlCommandAsync("EXEC sp_Paquetes_Maestro_Plazos_Deshabilitar @piIDPaquete", piIDPaquete);
                foreach (var item in list)
                {
                    var piIDPaqueteMaestroPlazos = new SqlParameter("@piIDPaqueteMaestroPlazos", SqlDbType.Int)
                    {
                        Value = (object)item.fiIDPaqueteMaestroPlazos ?? DBNull.Value
                    };
                    piIDPaquete = new SqlParameter("@piIDPaquete", item.fiIDPaquete);
                    var piIDPlazo = new SqlParameter("@piIDPlazo", item.fiIDPlazo);
                    var piEstadoPlazo = new SqlParameter("@piEstadoPlazo", item.fiEstadoPlazo);
                    var pnValorPlazo = new SqlParameter("@pnValorPlazo", item.fnValorPlazo);
                    var piIDUsuario = new SqlParameter("@piIDUsuario", GetIdUser());
                    
                    var resultado = await contexto.Database.ExecuteSqlCommandAsync(
                            "EXEC sp_Paquetes_Maestro_Plazos_RegistrarActualizar @piIDPaqueteMaestroPlazos, @piIDPaquete, @piIDPlazo, @piEstadoPlazo, @pnValorPlazo, @piIDUsuario",
                            piIDPaqueteMaestroPlazos, piIDPaquete, piIDPlazo, piEstadoPlazo, pnValorPlazo, piIDUsuario);
                }
                return EnviarResultado(true, "Plazos", "Plazos guardados exitosamente");
            }

            
        }
        #endregion


        #region Coloniaas
        public ActionResult IndexColonias()
        {
            return View();
        }

        public ContentResult CargarListaColonias(int idDepto = 5, int idMuni = 1)
        {
            using (var db = new ORIONDBEntities())
            {
                var colonias = db.Database.SqlQuery<ColoniaViewModel>("EXEC sp_Geo_ListaColonias @p0, @p1", idDepto, idMuni).ToList();
                var serializer = new JavaScriptSerializer();
                serializer.MaxJsonLength = Int32.MaxValue;

                var json = serializer.Serialize(colonias);
                return Content(json, "application/json");
            }
        }


        [HttpGet]
        public ActionResult CrearColonia()
        {
          
            return PartialView(new ColoniaViewModel());
        }


        [HttpPost]
        public JsonResult CrearColonia(ColoniaViewModel model)
        {

            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var list = contexto.Database.SqlQuery<ColoniaViewModel>("EXEC sp_Geo_ListaColonias @p0, @p1", model.fiCodDepartamento, model.fiCodMunicipio).ToList();
                    if (list.Any(x => x.fcBarrio== model.fcBarrio))
                        return EnviarResultado(false, "", "La colonia ya ha sido agregada.");
                    
                    using (var context = new ORIONDBEntities())
                    {
                        var connection = (SqlConnection)context.Database.Connection;
                        var command = new SqlCommand("Orion.dbo.sp_Geo_Barrio_Insertar", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@fiCodDepartamento", model.fiCodDepartamento);
                        command.Parameters.AddWithValue("@fiCodMunicipio", model.fiCodMunicipio);
                        command.Parameters.AddWithValue("@fcBarrio", model.fcBarrio);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    return EnviarResultado(true, "Colonia", "Registro guardado correctamente.");

                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "error");
                }

            }

        }


        [HttpGet]
        public ActionResult EditarColonia(string fiCodBarrio)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.Database.SqlQuery<ColoniaViewModel>("EXEC sp_Geo_ListaBarrioColoniabyId @p0",  fiCodBarrio).FirstOrDefault();
                var model = new ColoniaViewModel
                {
                    fiCodBarrio = modelDb.fiCodBarrio,
                    fiCodDepartamento = modelDb.fiCodDepartamento,
                    fiCodMunicipio = modelDb.fiCodMunicipio,
                    fcBarrio = modelDb.fcBarrio,
                    fbEditar = true

                };

                return PartialView("CrearColonia", model);
            }
        }

        [HttpPost]
        public JsonResult EditarColonia(ColoniaViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var list = contexto.Database.SqlQuery<ColoniaViewModel>("EXEC sp_Geo_ListaColonias @p0, @p1", model.fiCodDepartamento, model.fiCodMunicipio).ToList();
                    if (list.Any(x => x.fcBarrio == model.fcBarrio))
                        return EnviarResultado(false, "", "La colonia ya ha sido agregada.");

                    using (var context = new ORIONDBEntities())
                    {
                        var connection = (SqlConnection)context.Database.Connection;
                        var command = new SqlCommand("Orion.dbo.sp_Geo_Barrio_Actualizar", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@fiCodDepartamento", model.fiCodDepartamento);
                        command.Parameters.AddWithValue("@fiCodMunicipio", model.fiCodMunicipio);
                        command.Parameters.AddWithValue("@fiCodBarrio", model.fiCodBarrio);
                        command.Parameters.AddWithValue("@fcBarrio", model.fcBarrio);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                    return EnviarResultado(true, "Colonia", "Registro guardado correctamente.");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }
        #endregion
    }
}