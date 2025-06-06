
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNet.SignalR;
using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models.Base;
using OrionCoreCableColor.Models.Mantenimiento;
using OrionCoreCableColor.Models.Productos;
using OrionCoreCableColor.Models.Usuario;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Mvc;
using ZXing;
using System.Text.RegularExpressions;
using OrionCoreCableColor.App_Services.ContratoService;
using OrionCoreCableColor.DbConnection.CoreFinancieroDB;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using OrionCoreCableColor.Models.Contabilidad;
using OrionCoreCableColor.DbConnection.CoreAdministrativoDB;
using OrionCoreCableColor.App_Services.EmailService;
using OrionCoreCableColor.Models.EmailTemplateService;
using System.Data.Entity.Core.Objects;
using OrionCoreCableColor.App_Services.ReportesService;
using System.Diagnostics;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using OrionCoreCableColor.Models.Prestamo;

namespace OrionCoreCableColor.Controllers
{
    [System.Web.Mvc.Authorize]
    public class ProductosController : BaseController
    {



        #region Productos
        public ActionResult IndexProductoMaestro()
        {
            return View();
        }

        public JsonResult CargarListaProductos()
        {
            using (var contexto = new ORIONDBEntities())
            {

                var lista = contexto.sp_Producto_Maestro_Listar().ToList();
                foreach (var item in lista)
                {
                    if (!string.IsNullOrEmpty(item.fcImagenProducto))
                    {
                        var nombre = System.IO.Path.GetFileName(item.fcImagenProducto);
                        item.fcImagenProducto = GetContentDocument("img", "ImgProductos", nombre);
                    }
                }
                return EnviarListaJson(lista);
            }
        }

        [HttpPost]
        public ActionResult ViewHistoricoPrecio(int idproducto)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var modelo = contexto.sp_Producto_Precios_Detalle_ObtenerPorProducto(idproducto).ToList();
                return PartialView(modelo);
            }
        }


        public ActionResult CrearProducto()
        {

            ViewBag.TasaCambio = MemoryLoadManager.ObtenerTasaCambio();//tasaCambio.Value;
            ViewBag.ListaTipoProducto = GetListTipoProducto();
            ViewBag.ListaModelos = GetListModelos();
            return PartialView(new CrearProductoViewModel());

        }
        [HttpPost]
        public JsonResult CrearProducto(CrearProductoViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {

                try
                {
                    var listProductos = contexto.sp_Producto_Maestro_Listar().ToList();

                    if (listProductos.Any(x => x.fcProducto == model.fcProducto && x.fiIDTipoProducto == model.fiIdTipoProducto && x.fiIDModelo == model.fiIdModelo))
                    {
                        return EnviarResultado(false, "", "Producto ya esta registrado");
                    }

                    model.fcImagenProducto = "";
                    if (model.ImgenProducto != null)
                    {
                        model.fcImagenProducto = MemoryLoadManager.URL + @"\ImgProductos\" + model.ImgenProducto.FileName;
                        //model.ImgenProducto.SaveAs(model.fcImagenProducto);

                        UploadFileServer148("ImgProductos", model.ImgenProducto);
                    }


                    var result = contexto.sp_Producto_Maestro_Crear(model.fcCodigodeBarras,
                        model.fcProducto,
                        model.fcImagenProducto,
                        model.fiIdModelo,
                        model.fiIdTipoProducto,
                        model.fnValorProductoMN,
                        model.fnValorProductoME,
                        model.fnPorcentajeImpuesto1,
                        model.fnPorcentajeImpuesto2,
                        model.fiProductoInventariable,
                        GetIdUser(),
                        model.fnValorCuotaMensual,
                        0,
                        model.fnValorCuotaMensual12Cliente,
                        model.fnValorCuotaMensual24Cliente,
                        model.fnValorCuotaMensual12Nuevo,
                        model.fnValorCuotaMensual24Nuevo,
                        0,
                        model.fbProductoSeleccionablePorCliente,
                        model.fbProductoGenerico,
                        model.fbProductoDeInstalacion) > 0;


                    if (result)
                    {
                        return EnviarResultado(result, "", "Producto registrado exitosamente");
                    }
                    else
                    {
                        return EnviarResultado(result, "", "Error");
                    }
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }





            }
        }

        [HttpGet]
        public ActionResult EditarProducto(int fiIDProducto)
        {
            ViewBag.TasaCambio = MemoryLoadManager.ObtenerTasaCambio();
            ViewBag.ListaTipoProducto = GetListTipoProducto();
            ViewBag.ListaModelos = GetListModelos();
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Producto_Maestro_GetById(fiIDProducto).FirstOrDefault();
                var model = new CrearProductoViewModel
                {
                    fbEditar = true,
                    fiIDProducto = modelDb.fiIDProducto,
                    fcCodigodeBarras = modelDb.fcCodigodeBarras,
                    fiIdTipoProducto = modelDb.fiIDTipoProducto,
                    fiIdModelo = modelDb.fiIDModelo,
                    fcImagenProducto = modelDb.fcImagenProducto,
                    fcProducto = modelDb.fcProducto,
                    fiProductoInventariable = modelDb.fiProductoInventariable,
                    fnPorcentajeImpuesto1 = modelDb.fnPorcentajeImpuesto1,
                    fnPorcentajeImpuesto2 = modelDb.fnPorcentajeImpuesto2,
                    fnValorProductoME = modelDb.fnValorProductoME,
                    fnValorProductoMN = modelDb.fnValorProductoMN,
                    fnValorCuotaMensual = modelDb.fnValorCuotaMensual,
                    fiProductoEnPaquete = Convert.ToInt32(modelDb.fiProductoEnPaquete),
                    fnValorCuotaMensual12Cliente = modelDb.fnValorCuotaMensual12Cliente ?? 0,
                    fnValorCuotaMensual24Cliente = modelDb.fnValorCuotaMensual24Cliente ?? 0,
                    fnValorCuotaMensual12Nuevo = modelDb.fnValorCuotaMensual12Nuevo ?? 0,
                    fnValorCuotaMensual24Nuevo = modelDb.fnValorCuotaMensual24Nuevo ?? 0,
                    fbProductoDeInstalacion = modelDb.fbProductoDeInstalacion ?? false,
                    fbProductoGenerico = modelDb.fbProductoGenerico ?? false,
                    fbProductoSeleccionablePorCliente = modelDb.fbProductoSeleccionablePorCliente ?? false
                };
                return PartialView("CrearProducto", model);
            }
        }

        [HttpGet]
        public ActionResult ActualizarPrecio(int fiIDProducto)
        {
            ViewBag.TasaCambio = MemoryLoadManager.ObtenerTasaCambio();
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Producto_Maestro_GetById(fiIDProducto).FirstOrDefault();
                var model = new CrearProductoViewModel
                {
                    fbEditar = true,
                    fiIDProducto = modelDb.fiIDProducto,
                    fcCodigodeBarras = modelDb.fcCodigodeBarras,
                    fiIdTipoProducto = modelDb.fiIDTipoProducto,
                    fiIdModelo = modelDb.fiIDModelo,
                    fcImagenProducto = modelDb.fcImagenProducto,
                    fcProducto = modelDb.fcProducto,
                    fiProductoInventariable = modelDb.fiProductoInventariable,
                    fnPorcentajeImpuesto1 = modelDb.fnPorcentajeImpuesto1,
                    fnPorcentajeImpuesto2 = modelDb.fnPorcentajeImpuesto2,
                    fnValorProductoME = modelDb.fnValorProductoME,
                    fnValorProductoMN = modelDb.fnValorProductoMN,
                    fnValorCuotaMensual = modelDb.fnValorCuotaMensual,
                    fiProductoEnPaquete = Convert.ToInt32(modelDb.fiProductoEnPaquete),
                    fnValorCuotaMensual12Cliente = modelDb.fnValorCuotaMensual12Cliente ?? 0,
                    fnValorCuotaMensual24Cliente = modelDb.fnValorCuotaMensual24Cliente ?? 0,
                    fnValorCuotaMensual12Nuevo = modelDb.fnValorCuotaMensual12Nuevo ?? 0,
                    fnValorCuotaMensual24Nuevo = modelDb.fnValorCuotaMensual24Nuevo ?? 0
                };
                return PartialView(model);
            }

        }

        [HttpPost]
        public JsonResult ActualizarelPrecio(CrearProductoViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {

                var produc = contexto.sp_Producto_Precios_Detalle_ObtenerPorProducto(model.fiIDProducto).Where(a => a.fbEstadoPrecio == true).OrderByDescending(b => b.fiIdProductoPreciosDetalle).ToList();
                if (produc.Count > 0)
                {
                    var hoy = DateTime.Now;
                    var actualizarprecio = contexto.sp_Producto_Precios_Detalle_Editar(produc[0].fiIdProductoPreciosDetalle,
                        produc[0].fiIDProducto,
                        produc[0].fnValorProductoMN,
                        produc[0].fnValorProductoME,
                        produc[0].fnPorcentajeImpuesto1,
                        produc[0].fnPorcentajeImpuesto2,
                        produc[0].fnValorCuotaMensual,
                        0,
                        0,
                        0,
                        GetIdUser(),
                        false,
                        true,
                        produc[0].fnValorCuotaMensual12Cliente,
                        produc[0].fnValorCuotaMensual24Cliente,
                        produc[0].fnValorCuotaMensual12Nuevo,
                        produc[0].fnValorCuotaMensual24Nuevo);
                }
                var ingresar = contexto.sp_Producto_Precios_Detalle_Crear(model.fiIDProducto,
                    model.fnValorProductoMN,
                    model.fnValorProductoME,
                    model.fnPorcentajeImpuesto1,
                    model.fnPorcentajeImpuesto2,
                    model.fnValorCuotaMensual,
                    0,
                    0,
                    0,
                    DateTime.Now,
                    GetIdUser(),
                    model.fnValorCuotaMensual12Cliente,
                    model.fnValorCuotaMensual24Cliente,
                    model.fnValorCuotaMensual12Nuevo,
                    model.fnValorCuotaMensual24Nuevo).FirstOrDefault();
                return EnviarResultado(Convert.ToBoolean(Convert.ToInt32(ingresar.fiMensaje)), "", ingresar.fcMensaje);
            }
        }

        [HttpPost]
        //[EnableCors(origins: "*", methods: "*", headers: "*")]
        public ActionResult EditarProducto(CrearProductoViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var listProductos = contexto.sp_Producto_Maestro_Listar().Where(x => x.fiIDProducto != model.fiIDProducto).ToList();

                    if (listProductos.Any(x => x.fcProducto == model.fcProducto && x.fiIDTipoProducto == model.fiIdTipoProducto && x.fiIDModelo == model.fiIdModelo))
                    {
                        return EnviarResultado(false, "", "Producto ya esta registrado");
                    }

                    var modelDb = contexto.sp_Producto_Maestro_GetById(model.fiIDProducto).FirstOrDefault();
                    if (model.ImgenProducto != null)
                    {
                        model.fcImagenProducto = MemoryLoadManager.URL + @"\ImgProductos\" + model.ImgenProducto.FileName;
                        // model.ImgenProducto.SaveAs(model.fcImagenProducto);
                        UploadFileServer148(@"ImgProductos", model.ImgenProducto);
                    }
                    else
                    {
                        model.fcImagenProducto = modelDb.fcImagenProducto;
                    }

                    var result = contexto.sp_Producto_Maestro_Editar(model.fiIDProducto,
                        model.fcCodigodeBarras,
                        model.fcProducto,
                        model.fcImagenProducto,
                        model.fiIdModelo,
                        model.fiIdTipoProducto,
                        model.fnValorProductoMN,
                        model.fnValorProductoME,
                        model.fnPorcentajeImpuesto1,
                        model.fnPorcentajeImpuesto2,
                        1,
                        model.fiProductoInventariable, GetIdUser(),
                        model.fnValorCuotaMensual,
                        model.fiProductoEnPaquete,
                        model.fnValorCuotaMensual12Cliente,
                        model.fnValorCuotaMensual24Cliente,
                        model.fnValorCuotaMensual12Nuevo,
                        model.fnValorCuotaMensual24Nuevo,
                        model.fbProductoSeleccionablePorCliente,
                        model.fbProductoGenerico,
                        model.fbProductoDeInstalacion
                        ).FirstOrDefault();

                    if (Convert.ToInt32(result.fiMensaje) > 0)
                    {
                        return EnviarResultado(true, "", "Producto registrado exitosamente");
                    }
                    else
                    {
                        return EnviarResultado(false, "", "Error");
                    }
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }


        public JsonResult ValidarNumeroDeSerie(string fcCodigoDeBarra)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var listProductos = contexto.sp_Producto_Maestro_Listar().ToList();
                var result = listProductos.Any(x => x.fcCodigodeBarras == fcCodigoDeBarra);
                return EnviarResultado(result, "", result ? "Producto ya esta registrado" : "");
            }
        }


        public JsonResult CambiarStatusProducto(int fiIdProducto)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Producto_Maestro_GetById(fiIdProducto).FirstOrDefault();
                var result = Convert.ToInt32(contexto.sp_Producto_Maestro_Editar(modelDb.fiIDProducto,
                    modelDb.fcCodigodeBarras,
                    modelDb.fcProducto,
                    modelDb.fcImagenProducto,
                    modelDb.fiIDModelo,
                    modelDb.fiIDTipoProducto,
                    modelDb.fnValorProductoMN,
                    modelDb.fnValorProductoME,
                    modelDb.fnPorcentajeImpuesto1,
                    modelDb.fnPorcentajeImpuesto2,
                    modelDb.fiEstadoProducto == 1 ? 0 : 1,
                    modelDb.fiProductoInventariable,
                    GetIdUser(),
                    modelDb.fnValorCuotaMensual,
                    modelDb.fiProductoEnPaquete,
                    modelDb.fnValorCuotaMensual12Cliente,
                    modelDb.fnValorCuotaMensual24Cliente,
                    modelDb.fnValorCuotaMensual12Nuevo,
                    modelDb.fnValorCuotaMensual24Nuevo,
                    modelDb.fbProductoSeleccionablePorCliente,
                    modelDb.fbProductoGenerico,
                    modelDb.fbProductoDeInstalacion).FirstOrDefault().fiMensaje) > 0;

                return EnviarResultado(result, "", modelDb.fiEstadoProducto == 1 ? "Producto Deshabilitado" : "Producto Habilitado");
            }
        }
        #endregion


        #region EntradaDeInventario
        public ActionResult IndexInventario()
        {
            return View();
        }


        public JsonResult CargarListaInventarios()
        {
            using (var contexto = new ORIONDBEntities())
            {
                var lista = contexto.sp_Producto_Movimiento_Maestro_Listar().ToList();
                foreach (var item in lista)
                {
                    item.fcDocumentoFactura = GetContentDocument("img", @"inventario\facturas", System.IO.Path.GetFileName(item.fcDocumentoFactura));
                }
                return EnviarListaJson(lista);
            }
        }




        public ActionResult EscanearProducto()
        {
            return PartialView();
        }

        [HttpGet]
        public JsonResult GetInformacionProducto(string fcCodeBar)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var productosDb = contexto.sp_Producto_Maestro_Listar().ToList();
                var productoDb = productosDb.FirstOrDefault(x => (x.fcCodigodeBarras?.ToUpper() ?? "") == fcCodeBar.ToUpper());
                if (productoDb == null)
                {
                    return EnviarResultado(false, "", "No existe el producto registrado");
                }
                var producto = new sp_Producto_Maestro_Listar_ViewModel
                {
                    fiIDProducto = productoDb.fiIDProducto,
                    fcCodigodeBarras = productoDb.fcCodigodeBarras,
                    fcProducto = productoDb.fcProducto,
                    fcImagenProducto = GetContentDocument("img", "ImgProductos", System.IO.Path.GetFileName(productoDb.fcImagenProducto)),
                    fiIDMarca = productoDb.fiIDMarca,
                    fcMarca = productoDb.fcMarca,
                    fiIDModelo = productoDb.fiIDModelo,
                    fcModelo = productoDb.fcModelo,
                    fiIDTipoProducto = productoDb.fiIDTipoProducto,
                    fcTipoProducto = productoDb.fcTipoProducto,
                    fnValorProductoMN = productoDb.fnValorCuotaMensual,
                    fnValorProductoME = productoDb.fnValorProductoME,
                    fnPorcentajeImpuesto1 = productoDb.fnPorcentajeImpuesto1,
                    fnPorcentajeImpuesto2 = productoDb.fnPorcentajeImpuesto2,
                    fiEstadoProducto = productoDb.fiEstadoProducto,
                    fiProductoInventariable = productoDb.fiProductoInventariable,
                    fdFechaCreacion = productoDb.fdFechaCreacion,
                    fiIDUsuarioCreacion = productoDb.fiIDUsuarioCreacion,
                    fdFechaUltimaModificacion = productoDb.fdFechaUltimaModificacion,
                    fiIDUsuarioUltimaModificacion = productoDb.fiIDUsuarioUltimaModificacion,
                    fcToken = productoDb.fcToken,
                    fnValorCuotaMensual = productoDb.fnValorCuotaMensual,
                    fnValorCuotaCapital = productoDb.fnValorCuotaCapital,
                    fnValorCuotaInteres = productoDb.fnValorCuotaInteres,
                    fnTasaAnualPlana = productoDb.fnTasaAnualPlana,
                    fiProductoEnPaquete = productoDb.fiProductoEnPaquete,
                    fdFechaUltimaActualizacion = productoDb.fdFechaUltimaActualizacion,
                    fiCantidad = productoDb.fiCantidad,
                    fiEstadoPrecioActual = productoDb.fiEstadoPrecioActual,
                    fiIdProductoPreciosDetalleActual = productoDb.fiIdProductoPreciosDetalleActual,
                    ListPrecios = new List<SelectListItem>()
                };


                var precios = productoDb.fcPrecios.Split(';').ToList();
                foreach (var item in precios)
                {
                    var precio = item.Split(',');
                    if (precio.Count() > 0)
                    {
                        producto.ListPrecios.Add(new SelectListItem
                        {
                            Value = precio[0],
                            Text = precio[1]
                        });
                    }

                }

                return Json(new { Estado = true, producto }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult FotografiarProducto()
        {
            return PartialView();
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
                    return EnviarResultado(false, "", "No se logro leer informacion");
                }
                else
                {
                    //var data = new JavaScriptSerializer();
                    //var objeto = data.Deserialize(result.Text, typeof(ListPesadasAbiertasPuertoViewModel));

                    using (var contexto = new ORIONDBEntities())
                    {
                        var producto = contexto.sp_Producto_Maestro_Listar().ToList().FirstOrDefault(x => x.fcCodigodeBarras == result.Text);
                        if (producto == null)
                        {
                            return EnviarResultado(false, "", "No existe el producto registrado");
                        }
                        producto.fcImagenProducto = GetContentDocument("img", "ImgProductos", System.IO.Path.GetFileName(producto.fcImagenProducto));

                        return Json(new { Estado = true, producto }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                return EnviarException(ex, "Error");
            }
        }


        [HttpGet]
        public JsonResult GetInformacionBarCodeQr(string codigo)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var productoDb = contexto.sp_Producto_Maestro_Listar().ToList().FirstOrDefault(x => x.fcCodigodeBarras.ToUpper() == codigo.ToUpper());

                if (productoDb == null)
                {
                    return EnviarResultado(false, "", "No existe el producto registrado");
                }
                var producto = new sp_Producto_Maestro_Listar_ViewModel
                {
                    fiIDProducto = productoDb.fiIDProducto,
                    fcCodigodeBarras = productoDb.fcCodigodeBarras,
                    fcProducto = productoDb.fcProducto,
                    fcImagenProducto = GetContentDocument("img", "ImgProductos", System.IO.Path.GetFileName(productoDb.fcImagenProducto)),
                    fiIDMarca = productoDb.fiIDMarca,
                    fcMarca = productoDb.fcMarca,
                    fiIDModelo = productoDb.fiIDModelo,
                    fcModelo = productoDb.fcModelo,
                    fiIDTipoProducto = productoDb.fiIDTipoProducto,
                    fcTipoProducto = productoDb.fcTipoProducto,
                    fnValorProductoMN = productoDb.fnValorCuotaMensual,
                    fnValorProductoME = productoDb.fnValorProductoME,
                    fnPorcentajeImpuesto1 = productoDb.fnPorcentajeImpuesto1,
                    fnPorcentajeImpuesto2 = productoDb.fnPorcentajeImpuesto2,
                    fiEstadoProducto = productoDb.fiEstadoProducto,
                    fiProductoInventariable = productoDb.fiProductoInventariable,
                    fdFechaCreacion = productoDb.fdFechaCreacion,
                    fiIDUsuarioCreacion = productoDb.fiIDUsuarioCreacion,
                    fdFechaUltimaModificacion = productoDb.fdFechaUltimaModificacion,
                    fiIDUsuarioUltimaModificacion = productoDb.fiIDUsuarioUltimaModificacion,
                    fcToken = productoDb.fcToken,
                    fnValorCuotaMensual = productoDb.fnValorCuotaMensual,
                    fnValorCuotaCapital = productoDb.fnValorCuotaCapital,
                    fnValorCuotaInteres = productoDb.fnValorCuotaInteres,
                    fnTasaAnualPlana = productoDb.fnTasaAnualPlana,
                    fiProductoEnPaquete = productoDb.fiProductoEnPaquete,
                    fdFechaUltimaActualizacion = productoDb.fdFechaUltimaActualizacion,
                    fiCantidad = productoDb.fiCantidad,
                    fiEstadoPrecioActual = productoDb.fiEstadoPrecioActual,
                    fiIdProductoPreciosDetalleActual = productoDb.fiIdProductoPreciosDetalleActual,
                    ListPrecios = new List<SelectListItem>()
                };


                var precios = productoDb.fcPrecios.Split(';').ToList();
                foreach (var item in precios)
                {
                    var precio = item.Split(',');
                    if (precio.Count() > 0)
                    {
                        producto.ListPrecios.Add(new SelectListItem
                        {
                            Value = precio[0],
                            Text = precio[1]
                        });
                    }

                }

                return Json(new { Estado = true, producto }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult DigitarProducto()
        {
            return PartialView();
        }




        [HttpGet]
        public ActionResult CrearIngresoInventario()
        {
            var listaTipoMovimiento = GetListTiposMovimientos();
            ViewBag.ListaTipoMovimiento = listaTipoMovimiento;
            ViewBag.ListaTipoMovientoTable = listaTipoMovimiento.Select(x => new { id = x.Value, text = x.Text });
            ViewBag.ListaUbicaciones = GetListUbicaciones().Select(x => new { id = x.Value, text = x.Text });
            ViewBag.ListaUbicacionesHeader = GetListUbicaciones();
            ViewBag.ListaProductos = GetListProductos().Select(x => new { id = x.Value, text = x.Text });
            ViewBag.ListaMonedas = GetListMonedas();
            ViewBag.ListaProveedores = GetListProveedores();
            ViewBag.ListaPrecios = GetListaPrecios();
            return View("GuardarMovimientoInventario", new GuardarMovimientoInventarioViewModel { fiIDMoneda = 2, fdFechaFactura = DateTime.Now, fcSignoMoneda = "$" });
        }

        [HttpPost]
        public async Task<JsonResult> CrearIngresoInventario(GuardarMovimientoInventarioViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {

                    if (contexto.sp_Producto_Movimiento_Maestro_ExisteFactura(model.fcNumeroFactura).FirstOrDefault() ?? false)
                    {
                        return EnviarResultado(false, "", "Factura ya fue registrada");
                    }
                    var repetidosSerie1 = new List<sp_Producto_Movimiento_Detalle_BuscarRepetidos_Result>();
                    var repetidosSerie2 = new List<sp_Producto_Movimiento_Detalle_BuscarRepetidos_Result>();
                    if (model.DetalleMovimiento != null)
                    {
                        if (model.DetalleMovimiento.Count() > 0)
                        {
                            repetidosSerie1 = contexto.sp_Producto_Movimiento_Detalle_BuscarRepetidos(String.Join(",", model.DetalleMovimiento.Where(x => !String.IsNullOrWhiteSpace(x.fcCodigoSerie1)).Select(x => x.fcCodigoSerie1))).ToList();
                            repetidosSerie2 = contexto.sp_Producto_Movimiento_Detalle_BuscarRepetidos(String.Join(",", model.DetalleMovimiento.Where(x => !String.IsNullOrWhiteSpace(x.fcCodigoSerie2)).Select(x => x.fcCodigoSerie2))).ToList();
                        }
                    }

                    if (repetidosSerie1.Where(x => !string.IsNullOrWhiteSpace(x.fcCodigoSerie1)).Count() > 0)
                    {
                        return EnviarResultado(false, "", $"Los siguientes series1: {String.Join(",", repetidosSerie1.Select(x => x.fcCodigoSerie1).ToList())} ya fueron ingresados anteriormente");
                    }

                    if (repetidosSerie2.Where(x => !string.IsNullOrWhiteSpace(x.fcCodigoSerie2)).Count() > 0)
                    {
                        return EnviarResultado(false, "", $"Los siguientes series2/mac: {String.Join(",", repetidosSerie1.Select(x => x.fcCodigoSerie2).ToList())} ya fueron ingresados anteriormente");
                    }

                    if (model.DocumentoFactura != null)
                    {
                        model.fcDocumentoFactura = MemoryLoadManager.URL + @"\inventario\facturas\" + model.DocumentoFactura.FileName;
                        model.fcDocumentoExtension = System.IO.Path.GetExtension(model.DocumentoFactura.FileName);
                        //model.ImgenProducto.SaveAs(model.fcImagenProducto);

                        UploadFileServer148(@"inventario\facturas", model.DocumentoFactura);
                    }

                    var liIDMovimientoMaestro = contexto.sp_Producto_Movimiento_Maestro_Crear(
                        model.fcNumeroFactura,
                        model.fdFechaFactura,
                        model.fcReferenciaFactura,
                        model.fcDescripcionFactura,
                        model.fcCAI,
                        1,
                        model.fnImporteGravado,
                        model.fnImporteExento,
                        model.fnImporteExonerado,
                        model.fnSaldo,
                        model.fnISV,
                        model.fnTotalFactura,
                        GetIdUser(),
                        model.fcDocumentoFactura ?? "",
                        model.fcDocumentoExtension ?? "",
                        model.fnValorCuotaMensual,
                        model.fiIDMoneda, model.fiIDProveedor, true, model.fiIDUbicacion).FirstOrDefault() ?? 0m;

                    if (liIDMovimientoMaestro > 0)
                    {

                        if (model.DetalleMovimiento != null)
                        {
                            if (model.DetalleMovimiento.Count() > 0)
                            {
                                foreach (var prod in model.DetalleMovimiento)
                                {
                                    contexto.sp_Producto_Movimiento_Detalle_Crear(
                                        prod.fiIDProducto,
                                        prod.fiIDUbicacion,
                                        1,
                                        prod.fcCodigoSerie1 ?? "",
                                        prod.fcCodigoSerie2 ?? "",
                                        prod.fcReferenciaMovimiento ?? "",
                                        GetIdUser(),
                                        Convert.ToInt32(liIDMovimientoMaestro),
                                        prod.fnCantidad,
                                        prod.fiIdProductoPreciosDetalleActual);
                                }


                                _ = await EnviarIngresoInventario(Convert.ToInt32(liIDMovimientoMaestro));
                            }

                        }

                    }
                    else
                    {
                        return EnviarResultado(false, "Error", "Error de red");
                    }

                    return EnviarResultado(true, "", "Factura ingresada exitosamente");

                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }


        [HttpGet]
        public ActionResult EditarIngresoInventario(int fiIDMovimientoMaestro)
        {

            var listaTipoMovimiento = GetListTiposMovimientos();
            ViewBag.ListaTipoMovimiento = listaTipoMovimiento;
            ViewBag.ListaTipoMovientoTable = listaTipoMovimiento.Select(x => new { id = x.Value, text = x.Text });
            ViewBag.ListaUbicaciones = GetListUbicaciones().Select(x => new { id = x.Value, text = x.Text });
            ViewBag.ListaUbicacionesHeader = GetListUbicaciones();
            ViewBag.ListaProductos = GetListProductos().Select(x => new { id = x.Value, text = x.Text });
            ViewBag.ListaMonedas = GetListMonedas();
            ViewBag.ListaProveedores = GetListProveedores();
            ViewBag.ListaPrecios = GetListaPrecios();
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Producto_Movimiento_Maestro_GetById(fiIDMovimientoMaestro).FirstOrDefault();
                var productosDb = contexto.sp_Producto_Movimiento_Detalle_GetByIDMovimientoMaestro(fiIDMovimientoMaestro).ToList();

                var model = new GuardarMovimientoInventarioViewModel();
                model.fiIDMovimientoMaestro = modelDb.fiIDMovimientoMaestro;
                model.fiIDMoneda = modelDb.fiIDMoneda ?? 1;
                model.fcNumeroFactura = modelDb.fcNumeroFactura;
                model.fdFechaFactura = modelDb.fdFechaFactura;
                model.fcReferenciaFactura = modelDb.fcReferenciaFactura;
                model.fcDescripcionFactura = modelDb.fcDescripcionFactura;
                model.fcCAI = modelDb.fcCAI;
                model.fiIDTipoMovimiento = modelDb.fiIDTipoMovimiento;
                model.fiTipoAfectacion = modelDb.fiTipoAfectacion ?? 0;
                model.fnImporteGravado = modelDb.fnImporteGravado;
                model.fnImporteExento = modelDb.fnImporteExento;
                model.fnSaldo = modelDb.fnSaldo;
                model.fnISV = modelDb.fnISV;
                model.fnTotalFactura = modelDb.fnTotalFactura;
                model.fnValorCuotaMensual = modelDb.fnValorCuotaMensual;
                model.fiIDProveedor = modelDb.fiIDProveedor ?? 0;
                model.fnImporteExonerado = modelDb.fnImporteExonerado;
                model.fiIDUbicacion = modelDb.fiIDUbicacion ?? 0;
                model.fbEditar = true;
                model.TotalCantidad = productosDb.Sum(x => x.fnCantidad);
                model.TotalPrecioME = productosDb.Sum(x => (x.fnValorProductoME ?? 0) * x.fnCantidad);
                model.TotalPrecioMN = productosDb.Sum(x => (x.fnValorProductoMN ?? 0) * x.fnCantidad);
                model.fnTotalFactura = modelDb.fiIDMoneda == 1 ? model.TotalPrecioMN : model.TotalPrecioME;
                model.fcSignoMoneda = modelDb.fiIDMoneda == 1 ? "L" : "$";
                if (productosDb.Count > 0)
                {
                    model.DetalleMovimiento = new List<ListMovimientoDetalleViewModel>();
                    foreach (var pDb in productosDb)
                    {
                        var p = new ListMovimientoDetalleViewModel();
                        p.fiIDMovimiento = pDb.fiIDMovimiento;
                        p.fiIDMovimientoMaestro = pDb.fiIDMovimientoMaestro ?? 0;
                        p.fiIDProducto = pDb.fiIDProducto;
                        p.fiIDUbicacion = pDb.fiIDUbicacion;
                        p.fcCodigoSerie1 = pDb.fcCodigoSerie1;
                        p.fcCodigoSerie2 = pDb.fcCodigoSerie2;
                        p.fcReferenciaMovimiento = pDb.fcReferenciaMovimiento;
                        p.fcToken = pDb.fcToken;
                        p.fbEditar = true;
                        p.fnCantidad = pDb.fnCantidad;
                        p.fiIdProductoPreciosDetalleActual = pDb.fiIdProductoPreciosDetalle ?? 0;
                        p.fcProducto = pDb.fcProducto;
                        p.fnValorProductoMN = pDb.fnValorProductoMN ?? 0;
                        p.fnValorProductoME = pDb.fnValorProductoME ?? 0;
                        model.DetalleMovimiento.Add(p);
                    }

                }

                return View("GuardarMovimientoInventario", model);
            }
        }

        [HttpPost]
        public async Task<JsonResult> EditarIngresoInventario(GuardarMovimientoInventarioViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {

                    var modelDbInicio = contexto.sp_Producto_Movimiento_Maestro_GetById(model.fiIDMovimientoMaestro).FirstOrDefault();
                    var modelProductosInicio = contexto.sp_Producto_Movimiento_Detalle_GetByIDMovimientoMaestro(model.fiIDMovimientoMaestro).ToList();

                    var modelDb = contexto.sp_Producto_Movimiento_Maestro_GetById(model.fiIDMovimientoMaestro).FirstOrDefault();
                    var productosDb = contexto.sp_Producto_Movimiento_Detalle_GetByIDMovimientoMaestro(model.fiIDMovimientoMaestro).ToList();
                    if (modelDb.fcNumeroFactura != model.fcNumeroFactura)
                    {
                        if (contexto.sp_Producto_Movimiento_Maestro_ExisteFactura(model.fcNumeroFactura).FirstOrDefault() ?? false)
                        {
                            return EnviarResultado(false, "", "Factura ya fue registrada");
                        }
                    }

                    var repetidosSerie1 = contexto.sp_Producto_Movimiento_Detalle_BuscarRepetidos(String.Join(",", model.DetalleMovimiento.Where(x => !String.IsNullOrWhiteSpace(x.fcCodigoSerie1)).Select(x => x.fcCodigoSerie1))).ToList();
                    var repetidosSerie2 = contexto.sp_Producto_Movimiento_Detalle_BuscarRepetidos(String.Join(",", model.DetalleMovimiento.Where(x => !String.IsNullOrWhiteSpace(x.fcCodigoSerie2)).Select(x => x.fcCodigoSerie2))).ToList();


                    if (repetidosSerie1.Where(x => x.fiIDMovimientoMaestro != model.fiIDMovimientoMaestro && !string.IsNullOrWhiteSpace(x.fcCodigoSerie1)).Count() > 0)
                    {
                        return EnviarResultado(false, "", $"Los siguientes series1: {String.Join(",", repetidosSerie1.Select(x => x.fcCodigoSerie1).ToList())} ya fueron ingresados anteriormente");
                    }

                    if (repetidosSerie2.Where(x => x.fiIDMovimientoMaestro != model.fiIDMovimientoMaestro && !string.IsNullOrWhiteSpace(x.fcCodigoSerie2)).Count() > 0)
                    {
                        return EnviarResultado(false, "", $"Los siguientes series2/mac: {String.Join(",", repetidosSerie1.Select(x => x.fcCodigoSerie2).ToList())} ya fueron ingresados anteriormente");
                    }

                    if (model.DocumentoFactura != null)
                    {
                        model.fcDocumentoFactura = MemoryLoadManager.URL + @"\inventario\facturas\" + model.DocumentoFactura.FileName;
                        model.fcDocumentoExtension = System.IO.Path.GetExtension(model.DocumentoFactura.FileName);


                        UploadFileServer148(@"inventario\facturas", model.DocumentoFactura);
                    }

                    var result = contexto.sp_Producto_Movimiento_Maestro_Editar(
                        model.fiIDMovimientoMaestro,
                        model.fcNumeroFactura,
                        model.fdFechaFactura,
                        model.fcReferenciaFactura,
                        model.fcDescripcionFactura,
                        "_",
                        1,
                        model.fnImporteGravado,
                        model.fnImporteExento,
                        model.fnImporteExonerado,
                        model.fnSaldo,
                        model.fnISV,
                        model.fnTotalFactura,
                        GetIdUser(),
                        model.fcDocumentoFactura ?? modelDb.fcDocumentoFactura,
                        model.fcDocumentoExtension ?? modelDb.fcDocumentoExtension,
                        1,
                        model.fnValorCuotaMensual,
                        model.fiIDMoneda,
                        model.fiIDProveedor,
                        true, model.fiIDUbicacion) > 0;

                    if (result)
                    {
                        foreach (var item in productosDb)
                        {
                            if (model.DetalleMovimiento?.Any(x => x.fcToken == item.fcToken) ?? false)
                            {
                                var itemModel = model.DetalleMovimiento.FirstOrDefault(x => x.fcToken == item.fcToken);
                                model.DetalleMovimiento.FirstOrDefault(x => x.fcToken == item.fcToken).fbEditado = true;
                                contexto.sp_Producto_Movimiento_Detalle_EditarEnFactura(
                                    itemModel.fiIDMovimiento,
                                    itemModel.fiIDProducto,
                                    itemModel.fiIDUbicacion,
                                    1,
                                    itemModel.fcCodigoSerie1 ?? "",
                                    itemModel.fcCodigoSerie2 ?? "",
                                    itemModel.fcReferenciaMovimiento ?? "",
                                    itemModel.fnCantidad,
                                    itemModel.fiIdProductoPreciosDetalleActual);
                            }
                            else
                            {
                                contexto.sp_Producto_Movimiento_Detallle_Eliminar(item.fiIDMovimiento, item.fcToken);
                            }
                        }

                        if (model.DetalleMovimiento != null)
                        {
                            foreach (var prod in model.DetalleMovimiento.Where(x => x.fbEditado == false).ToList())
                            {
                                contexto.sp_Producto_Movimiento_Detalle_Crear(
                                   prod.fiIDProducto,
                                   prod.fiIDUbicacion,
                                   1,
                                   prod.fcCodigoSerie1 ?? "",
                                   prod.fcCodigoSerie2 ?? "",
                                   prod.fcReferenciaMovimiento ?? "",
                                   GetIdUser(),
                                   Convert.ToInt32(model.fiIDMovimientoMaestro),
                                   prod.fnCantidad,
                                   prod.fiIdProductoPreciosDetalleActual);
                            }
                        }



                    }
                    _ = await EnviarIngresoInventario(model.fiIDMovimientoMaestro, model);
                    return EnviarResultado(result, "", result ? "Ingreso de Inventario editada exitosamente" : "Error de red");

                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }

        //public ActionResult ListaDeUsuarios()
        //{
        //    return PartialView(GetUsuariosLogueados());
        //}


        [AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> EnviarIngresoInventario(int fiIDMovimientoMaestro, GuardarMovimientoInventarioViewModel model = null)
        {
            
            var email = new EmailTemplateService();
            var resultado = await email.SendEmailToIngresoInventario(new EmailTemplateServiceModel { IdTransaccion = fiIDMovimientoMaestro }, model);
            return EnviarResultado(resultado, "", "Salida exitosa");
        }


        public ActionResult ObtenerSerieCamaraWeb()
        {
            return PartialView();
        }


        [HttpPost]
        public JsonResult GetInformacionBarCodeQrString(HttpPostedFileBase file)
        {
            try
            {
                var qrRedear = new BarcodeReader();
                var imagen = new Bitmap(file.InputStream);
                var result = qrRedear.Decode(imagen);
                if (result == null)
                {
                    return EnviarResultado(false, "", "No se logro leer informacion");
                }
                else
                {

                    return EnviarResultado(true, "", result.Text);

                }
            }
            catch (Exception ex)
            {
                return EnviarException(ex, "Error");
            }
        }


        [HttpPost]
        public JsonResult GetOcrImage(HttpPostedFileBase file)
        {
            try
            {
                byte[] thePictureAsBytes = new byte[file.ContentLength];
                using (BinaryReader theReader = new BinaryReader(file.InputStream))
                {
                    thePictureAsBytes = theReader.ReadBytes(file.ContentLength);
                }
                //var api = new AsposeOcr();
                //var image = new OcrInput(Aspose.OCR.InputType.SingleImage);
                string thePictureDataAsString = Convert.ToBase64String(thePictureAsBytes);
                //image.AddBase64(thePictureDataAsString);
                //var result = api.Recognize(image);
                return EnviarResultado(true, "", "");
            }
            catch (Exception ex)
            {
                return EnviarException(ex, "Error");
            }
        }


        [HttpGet]
        public JsonResult GetPreciosPorProducto(int fiIDProducto)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var precios = GetListPreciosPorProducto(fiIDProducto);
                if (precios.Count == 0)
                {
                    return EnviarResultado(false, "", "Agregue Precios al producto");
                }
                var respuesta = new
                {
                    listPrecios = GetListPreciosPorProducto(fiIDProducto).Select(x => new { id = x.Value, text = x.Text }).ToList(),
                    fiIdProductoPreciosDetalle = contexto.sp_Producto_Precios_Detalle_ObtenerPorProducto(fiIDProducto).FirstOrDefault(x => x.fbEstadoPrecioActual == true).fiIdProductoPreciosDetalle
                };
                return EnviarListaJson(respuesta);
            }

        }


        [HttpGet]
        public ActionResult ViewGenerarPartidaInventario(int fiIDMovimientoMaestro)
        {
            using (var contabilidad = new CoreAdministrativoEntities())
            {
                using (var contexto = new ORIONDBEntities())
                {
                    var InventarioDb = contexto.sp_Producto_Movimiento_Maestro_GetById(fiIDMovimientoMaestro).FirstOrDefault();
                    var ProveedorDb = contexto.sp_Proveedores_ObtenerId(InventarioDb.fiIDProveedor).FirstOrDefault();
                    var InventarioDetalleDb = contexto.sp_Producto_Movimiento_Detalle_GetByIDMovimientoMaestro(fiIDMovimientoMaestro).ToList();

                    var detalleEncabezado = $"Empresa: {ProveedorDb.fcNombre}, \nSe ingresaron los siguientes productos ";

                    foreach (var item in InventarioDetalleDb)
                    {
                        detalleEncabezado += $"{item.fcProducto}, ";
                    }

                    var model = new CrearPartidaViewModel
                    {
                        fiIDMovimientoMaestro = fiIDMovimientoMaestro,
                        fdFechaPartida = InventarioDb.fdFechaFactura,
                        fnValorPartida = InventarioDb.fnTotalFactura,
                        fcTipoPartida = "INV",
                        fcOrigen = "INV",
                        fcCreador = User.Identity.Name,
                        fiIDUsuarioCreador = GetIdUser(),
                        fiIDEmpresa = 1,
                        fcEstadoPartida = "A",
                        fiMoneda = InventarioDb.fiIDMoneda ?? 2,
                        fcReferencia = "Partida de Inventario de la factura" + ProveedorDb.fcNombre,
                        fcSinopsis = detalleEncabezado,
                        ListaDetalle = new List<CrearDetallePartidaViewModel>()
                    };

                    int countRow = 1;

                    var confCuentasHaber = contexto.sp_Configuraciones().FirstOrDefault(x => x.NombreLlave == (model.fiMoneda == 1 ? "Inventario_CuentasContableHaber_Lempiras" : "Inventario_CuentasContableHaber_Dolar")).ValorLLave.Split(',').ToList();
                    var confCuetasDebe = contexto.sp_Configuraciones().FirstOrDefault(x => x.NombreLlave == (model.fiMoneda == 1 ? "Inventario_CuentasContableDebe_Lempiras" : "Inventario_CuentasContableDebe_Dolar")).ValorLLave.Split(',').ToList();
                    var confCuentasImpuesto = contexto.sp_Configuraciones().FirstOrDefault(x => x.NombreLlave == "Inventario_CuentasContableImpuesto").ValorLLave.Split(',').ToList();

                    var CuentasHaber = confCuentasHaber.Select(x => contabilidad.sp_Catalogo_Contable_ObtenerPorCuentaContable(1, x).FirstOrDefault()).ToList();
                    var cuentasDebe = confCuetasDebe.Select(x => contabilidad.sp_Catalogo_Contable_ObtenerPorCuentaContable(1, x).FirstOrDefault()).ToList();

                    foreach (var cuentaContable in cuentasDebe)
                    {
                        var modelDebe = new CrearDetallePartidaViewModel
                        {
                            fiIDFila = countRow,
                            fcCuentaContable = cuentaContable.fcCuentaContable,// CuentasContablesEnum.CuentaPorCobrar_Seguros,
                            DescripcionCuenta = cuentaContable.fcDescripcionCuenta,//  "Cuenta por Cobrar Seguros",
                            fcReferenciaDetalle = "Contabilizacion de inventario",
                            fcDebitooCredito = "D",
                            fnDebito = (confCuentasImpuesto.Any(x => x == cuentaContable.fcCuentaContable) ? InventarioDb.fnISV : InventarioDb.fnSaldo),
                            fcCentrodeCosto = "0100",
                            fcDocumento = InventarioDb.fcNumeroFactura,
                            fcAuxiliar = "INV",
                            fcAuxiliarCodigo = InventarioDb.fcNumeroFactura,
                            TieneAuxiliar = false,
                            fnTasadeCambio = 1,
                            fcFondo = "0001",
                            fcPrograma = "0001",
                            fcOrigen = "SEG",
                            IdMoneda = InventarioDb.fiIDMoneda ?? 0,
                        };
                        model.ListaDetalle.Add(modelDebe);
                        countRow++;
                    }






                    var modelHaber = new CrearDetallePartidaViewModel
                    {
                        fiIDFila = countRow,
                        fcCuentaContable = CuentasHaber.FirstOrDefault().fcCuentaContable,
                        DescripcionCuenta = CuentasHaber.FirstOrDefault().fcDescripcionCuenta,
                        fcReferenciaDetalle = "Contabilizacion de inventario",
                        fcDebitooCredito = "C",
                        fnCredito = InventarioDb.fnTotalFactura, //200
                        fcCentrodeCosto = "0100",
                        fcDocumento = InventarioDb.fcNumeroFactura,
                        fcAuxiliar = "INV",
                        fcAuxiliarCodigo = InventarioDb.fcNumeroFactura,
                        TieneAuxiliar = false,
                        fnTasadeCambio = 1,
                        fcFondo = "0001",
                        fcPrograma = "0001",
                        fcOrigen = "INV",
                        IdMoneda = InventarioDb.fiIDMoneda ?? 0,
                    };
                    model.ListaDetalle.Add(modelHaber);




                    return PartialView(model);
                }
            }

        }


        [HttpGet]
        public JsonResult ContabilizarInventario(int fiIDMovimientoMaestro)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    contexto.Database.CommandTimeout = 0;
                    var result = contexto.sp_Productos_Movimiento_Maestro_Contabilizar(fiIDMovimientoMaestro, GetIdUser());
                    if (result > 0)
                    {
                        return EnviarResultado(true, "Inventario", "Inventario enviado a contabilidad satisfactoriamente");
                    }
                    else
                    {
                        return EnviarResultado(false, "", "Error de red");
                    }
                }
                catch (Exception ex)
                {

                    return EnviarException(ex, "Error");
                }

            }
        }

        [HttpGet]
        public ActionResult ObtenerUsuariosParaFoto()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult ObtenerUsuariosParaSerie()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult EnviarModalParaFoto(InfoUsuarioViewModel usuario, string usuarioPeticion)
        {
            try
            {
                var modeloR = new ObjSignalRModalViewModel
                {
                    InfoUsuario = usuario,
                    Url = Url.Action("CapturaDeFoto", "Productos"),
                    Obj = new { usuarioPeticion }
                };
                var algo = EnviarFormularioSignalR<object>(modeloR);
                return EnviarResultado(true, "", algo);
            }
            catch (Exception ex)
            {
                return EnviarException(ex, "Error");
            }
        }

        [HttpGet]
        public ActionResult CapturaDeFoto(string usuarioPeticion)
        {
            var model = new CamaraExternaViewModel { UsuarioPeticion = usuarioPeticion };
            return PartialView(model);
        }


        [HttpPost]
        public void RetornarFotografia(string usuarioPeticion, HttpPostedFileBase foto)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();

            byte[] thePictureAsBytes = new byte[foto.ContentLength];
            using (BinaryReader theReader = new BinaryReader(foto.InputStream))
            {
                thePictureAsBytes = theReader.ReadBytes(foto.ContentLength);
            }
            string thePictureDataAsString = Convert.ToBase64String(thePictureAsBytes);
            using (var log = new EventLog("Application"))
            {
                log.Source = "Application";
                log.WriteEntry(foto.FileName, EventLogEntryType.Error, 101, 1);

            }
            hubContext.Clients.All.retornarFotografia(new { UsuarioPeticion = usuarioPeticion, Fotografia = thePictureDataAsString, Nombre = foto.FileName });
        }


        [HttpGet]
        public ActionResult ViewListaProductosMovimiento(int fiIDMovimientoMaestro)
        {
            using (var contexto = new ORIONDBEntities())
            {
                //var lista = contexto.sp_Producto_Movimiento_Detalle_GetByIDMovimientoMaestro(fiIDMovimientoMaestro).ToList();
                return PartialView(fiIDMovimientoMaestro);
            }
        }

        [HttpGet]
        public JsonResult CargarListaInventarioDetalle(int fiIDMovimientoMaestro)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var lista = contexto.sp_Producto_Movimiento_Detalle_GetByIDMovimientoMaestro(fiIDMovimientoMaestro).ToList();
                return EnviarListaJson(lista);
            }
        }


        [HttpGet]
        public ActionResult ViewEditarFacturaPadre(int fiIDMovimiento)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Producto_Movimiento_Detalle_GetById(fiIDMovimiento).FirstOrDefault();
                var model = new ListProductoMovimientoDetalleViewModel
                {
                    fiIDMovimiento = modelDb.fiIDMovimiento,
                    fiIDMovimientoMaestro = modelDb.fiIDMovimientoMaestro ?? 0
                };
                ViewBag.ListarFacturas = GetListFacturasInventario();
                return PartialView(model);
            }
        }

        [HttpPost]
        public JsonResult ActualizarNumeroFacturaProductoDetalle(ListProductoMovimientoDetalleViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var result = contexto.sp_Producto_Movimiento_Detalle_EditarMaestro(model.fiIDMovimiento, model.fiIDMovimientoMaestro) > 0;
                    return EnviarResultado(result, "Ingreso Inventario", result ? "Cambio realizado exitosamente" : "Error de red");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }

        [HttpGet]
        public ActionResult ViewModificarFactura(int fiIDMovimientoMaestro)
        {
            using (var contexto = new ORIONDBEntities())
            {


                ViewBag.ListaMonedas = GetListMonedas();
                ViewBag.ListaProveedores = GetListProveedores();


                var modelDb = contexto.sp_Producto_Movimiento_Maestro_GetById(fiIDMovimientoMaestro).FirstOrDefault();


                var model = new GuardarMovimientoInventarioViewModel();
                model.fiIDMovimientoMaestro = modelDb.fiIDMovimientoMaestro;
                model.fiIDMoneda = modelDb.fiIDMoneda ?? 1;
                model.fcNumeroFactura = modelDb.fcNumeroFactura;
                model.fdFechaFactura = modelDb.fdFechaFactura;
                model.fcReferenciaFactura = modelDb.fcReferenciaFactura;
                model.fcDescripcionFactura = modelDb.fcDescripcionFactura;
                model.fcCAI = modelDb.fcCAI;
                model.fiIDTipoMovimiento = modelDb.fiIDTipoMovimiento;
                model.fiTipoAfectacion = modelDb.fiTipoAfectacion ?? 0;
                model.fnImporteGravado = modelDb.fnImporteGravado;
                model.fnImporteExento = modelDb.fnImporteExento;
                model.fnImporteExonerado = modelDb.fnImporteExonerado;
                model.fnSaldo = modelDb.fnSaldo;
                model.fnISV = modelDb.fnISV;
                model.fnTotalFactura = modelDb.fnTotalFactura;
                model.fnValorCuotaMensual = modelDb.fnValorCuotaMensual;
                model.fiIDProveedor = modelDb.fiIDProveedor ?? 0;


                return PartialView(model);


            }
        }


        [HttpPost]
        public ActionResult EditarDatosFactura(GuardarMovimientoInventarioViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var modelDb = contexto.sp_Producto_Movimiento_Maestro_GetById(model.fiIDMovimientoMaestro).FirstOrDefault();

                    if (modelDb.fcNumeroFactura != model.fcNumeroFactura)
                    {
                        if (contexto.sp_Producto_Movimiento_Maestro_ExisteFactura(model.fcNumeroFactura).FirstOrDefault() ?? false)
                        {
                            return EnviarResultado(false, "", "Factura ya fue registrada");
                        }
                    }

                    var result = contexto.sp_Producto_Movimiento_Maestro_Editar(
                           model.fiIDMovimientoMaestro,
                           model.fcNumeroFactura,
                           model.fdFechaFactura,
                           model.fcReferenciaFactura,
                           model.fcDescripcionFactura,
                           "_",
                           1,
                           model.fnImporteGravado,
                           model.fnImporteExento,
                           model.fnImporteExonerado,
                           model.fnSaldo,
                           model.fnISV,
                           model.fnTotalFactura,
                           GetIdUser(),
                           model.fcDocumentoFactura ?? modelDb.fcDocumentoFactura,
                           model.fcDocumentoExtension ?? modelDb.fcDocumentoExtension,
                           1,
                           model.fnValorCuotaMensual,
                           model.fiIDMoneda,
                           model.fiIDProveedor,
                           true, modelDb.fiIDUbicacion) > 0;

                    return EnviarResultado(result, "", result ? "Datos de factura actualizados" : "Error de red");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }

            }
        }


        [HttpGet]
        public ActionResult SubidoMasivo()
        {
            using (var contexto = new ORIONDBEntities())
            {
                ViewBag.listProductos = contexto.sp_Producto_Maestro_Listar().ToList();
                return PartialView();
            }

        }
        #endregion


        #region SalidaDeInventario
        public ActionResult IndexSalidaInventario()
        {
            return View();
        }

        public JsonResult CargarListaSalidasInventario()
        {
            using (var contexto = new ORIONDBEntities())
            {
                var lista = contexto.sp_Inventario_Movimiento_Maestro_Listar().Where(x => x.fiIDTipoMovimiento == 2).ToList();
                return EnviarListaJson(lista);
            }
        }


        public List<SelectListItem> GetUsuarios()
        {
            using (var contexto = new ORIONDBEntities())
            {
                var lista = contexto.sp_Usuarios_Maestros_ListarUsuarios().ToList();
                return lista.Select(x => new SelectListItem { Value = x.fiIDUsuario.ToString(), Text = $"{x.fcNombreCorto} - {x.fcNombreComercial}" }).ToList();
            }
        }

        public ActionResult CrearSalidaInventario()
        {
            var ubicaciones = ListarUbicacionesPorTipo().Where(x => x.fcTipoUbicacion != "CLIENTE").Select(x => new SelectListItem { Value = x.fiIDUbicacion.ToString(), Text = x.fcUbicacion }).ToList();
            ViewBag.ListUbicaciones = ubicaciones;
            ViewBag.ListTecnicosContratistas = ubicaciones;
            ViewBag.ListaProductos = GetListProductos().Select(x => new { id = x.Value, text = x.Text });
            using (var contexto = new ORIONDBEntities())
            {
                ViewBag.ProductosSinSerie = GetListProductosConsumibles();
            }
            return View(new CrearSalidaInventarioViewModel());

        }

        public JsonResult GetInformacionInventario(string fcCodigoSerie)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var list = contexto.sp_Producto_Movimiento_Detalle_ByCodigoSerie(fcCodigoSerie).ToList();
                var listaInventarioDetalle = new List<ListInventarioMovimientoDetalleViewModel>();
                foreach (var item in list)
                {
                    listaInventarioDetalle.Add(new ListInventarioMovimientoDetalleViewModel
                    {
                        fiIDMovimiento = item.fiIDMovimiento,
                        fiIDUbicacionInicial = item.fiIDUbicacionInicial,
                        fcUbicacionInicial = item.fcUbicacion,
                        fiIdProducto = item.fiIDProducto,
                        fcProducto = item.fcProducto,
                        fcCodigoSerie1 = item.fcCodigoSerie1,
                        fcCodigoSerie2 = item.fcCodigoSerie2,
                        fnCantidad = item.fnCantidad,
                        fbPorCodigo = item.fbPorCodigo ?? false,
                        fcDescripcion = ""
                    });
                }

                return Json(new { Estado = true, listaInventarioDetalle }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult CodigoInventarioPorCamara()
        {
            return PartialView();
        }

        public ActionResult CodigoInventarioDigitado()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult CrearSalidaInventario(CrearSalidaInventarioViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var productosSinSerie = contexto.sp_Configuraciones().FirstOrDefault(x => x.NombreLlave == "Productos_SinCodigoBarra");

                    var listaProductos = GetListProductosConsumibles();

                    var result = contexto.sp_Inventario_Movimiento_Maestro_Crear(
                        model.fiIdUbicacion,
                        model.fiIDUsuarioDestino,
                        GetIdUser(),
                        2, model.fiIDUbicacionDestino)?.FirstOrDefault() ?? 0;

                    if (result > 0)
                    {
                        foreach (var item in model.ListaInventarioDetalle)
                        {
                            if (item.fbPorCodigo == false)
                            {
                                item.fiIDMovimiento = contexto.sp_Productos_Maestro_GetIdMovimiento(item.fiIdProducto, item.fiIDUbicacionInicial)?.FirstOrDefault()?.fiIDMovimiento ?? 0;
                            }
                            var piIDInventarioMovimientoDetalle = new ObjectParameter("piIDInventarioMovimientoDetalle", typeof(int));
                            contexto.sp_Inventario_Movimiento_Detalle_Crear(
                                Convert.ToInt32(result),
                                2,
                                item.fiIDMovimiento,
                                item.fiIDUbicacionInicial == 0 ? model.fiIdUbicacion : item.fiIDUbicacionInicial,
                                item.fiIDUbicacionDestino,
                                item.fcCodigoSerie1 ?? "",
                                item.fcCodigoSerie2 ?? "",
                                item.fnCantidad,
                                item.fcDescripcion ?? "",
                                GetIdUser(), piIDInventarioMovimientoDetalle);
                        }
                    }



                    return EnviarResultado(true, "", "Salida Registrada exitosamente");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }

            }
        }


        [HttpGet]
        public ActionResult EditarSalidaInventario(int fiIDInventarioMovimientoMaestro)
        {

            ViewBag.ListUbicaciones = GetListUbicaciones();
            ViewBag.ListTecnicosContratistas = GetUsuarios();
            ViewBag.ListaProductos = GetListProductos().Select(x => new { id = x.Value, text = x.Text });
            using (var contexto = new ORIONDBEntities())
            {
                ViewBag.ProductosSinSerie = GetListProductosConsumibles();
            }
            using (var contexto = new ORIONDBEntities())
            {
                var model = new CrearSalidaInventarioViewModel();
                var modelDb = contexto.sp_Inventario_Movimiento_Maestro_GetById(fiIDInventarioMovimientoMaestro).FirstOrDefault();
                var modelDbDetalle = contexto.sp_Inventario_Movimiento_Detalle_GetIDInventarioMovimientoMaestro(fiIDInventarioMovimientoMaestro).ToList();

                model.fiIDInventarioMovimientoMaestro = modelDb.fiIDInventarioMovimientoMaestro;
                model.fiIdUbicacion = modelDb.fiIDUbicacion;
                model.fiIDUsuarioDestino = modelDb.fiIDUsuarioDestino;
                model.fiIDUbicacionDestino = modelDb.fiIDUbicacionDestino ?? 0;
                model.fiIDTipoMomvimento = modelDb.fiIDTipoMovimiento;
                model.fbEditar = true;

                if (modelDbDetalle.Count > 0)
                {
                    model.ListaInventarioDetalle = new List<ListInventarioMovimientoDetalleViewModel>();
                    foreach (var itemDb in modelDbDetalle)
                    {
                        var item = new ListInventarioMovimientoDetalleViewModel
                        {
                            fiIDInventarioMovimientoDetalle = itemDb.fiIDInventarioMovimientoDetalle,
                            fiIDInventarioMovimientoMaestro = itemDb.fiIDInventarioMovimientoMaestro,
                            fiIDTipoMovimiento = itemDb.fiIDTipoMovimiento,
                            fiIDMovimiento = itemDb.fiIDMovimiento,
                            fiIDUbicacionInicial = itemDb.fiIDUbicacionInicial,
                            fiIDUbicacionDestino = itemDb.fiIDUbicacionDestino,
                            fiIdProducto = itemDb.fiIDProducto ?? 0,
                            fcProducto = itemDb.fcProducto,
                            fcCodigoSerie1 = itemDb.fcCodigoSerie1,
                            fcCodigoSerie2 = itemDb.fcCodigoSerie2,
                            fnCantidad = itemDb.fnCantidad,
                            fcDescripcion = itemDb.fcDescripcion,
                            fbPorCodigo = itemDb.fcCodigoSerie1.Trim() == "" ? false : true,
                            fcToken = itemDb.fcToken
                        };
                        model.ListaInventarioDetalle.Add(item);
                    }
                }

                return View("CrearSalidaInventario", model);

            }
        }

        [HttpPost]
        public ActionResult EditarSalidaInventario(CrearSalidaInventarioViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var modelDb = contexto.sp_Inventario_Movimiento_Maestro_GetById(model.fiIDInventarioMovimientoMaestro).FirstOrDefault();
                    var modelDbDetalle = contexto.sp_Inventario_Movimiento_Detalle_GetIDInventarioMovimientoMaestro(model.fiIDInventarioMovimientoMaestro).ToList();

                    var result = contexto.sp_Inventario_Movimiento_Maestro_Editar(
                        model.fiIDInventarioMovimientoMaestro,
                        model.fiIdUbicacion,
                        model.fiIDUsuarioDestino,
                        modelDb.fiIDTipoMovimiento,
                        2,
                        1, model.fiIDUbicacionDestino) > 0;

                    if (result)
                    {
                        foreach (var item in modelDbDetalle)
                        {
                            if (model.ListaInventarioDetalle?.Any(x => x.fcToken == item.fcToken) ?? false)
                            {
                                var itemModel = model.ListaInventarioDetalle.FirstOrDefault(x => x.fcToken == item.fcToken);
                                if (itemModel.fiIDMovimiento == 0)
                                {
                                    itemModel.fiIDMovimiento = contexto.sp_Productos_Maestro_GetIdMovimiento(itemModel.fiIdProducto, item.fiIDUbicacionInicial)?.FirstOrDefault()?.fiIDMovimiento ?? 0;
                                }
                                model.ListaInventarioDetalle.FirstOrDefault(x => x.fcToken == item.fcToken).fbEditado = true;
                                contexto.sp_Inventario_Movimiento_Detalle_Editar(
                                    itemModel.fiIDInventarioMovimientoDetalle,
                                    itemModel.fiIDInventarioMovimientoMaestro,
                                    2,
                                    itemModel.fiIDMovimiento,
                                    itemModel.fiIDUbicacionInicial == 0 ? model.fiIdUbicacion : itemModel.fiIDUbicacionInicial,
                                    itemModel.fiIDUbicacionDestino,
                                    itemModel.fcCodigoSerie1 ?? "",
                                    itemModel.fcCodigoSerie2 ?? "",
                                    itemModel.fnCantidad,
                                    itemModel.fcDescripcion ?? "",
                                    GetIdUser()
                                );
                            }
                            else
                            {
                                contexto.sp_Inventario_Movimiento_Detalle_Eliminar(item.fiIDInventarioMovimientoDetalle);
                            }
                        }

                        if (model.ListaInventarioDetalle != null)
                        {

                            foreach (var inv in model.ListaInventarioDetalle.Where(x => x.fbEditado == false).ToList())
                            {
                                if (inv.fbPorCodigo == false)
                                {
                                    inv.fiIDMovimiento = contexto.sp_Productos_Maestro_GetIdMovimiento(inv.fiIdProducto, model.fiIdUbicacion)?.FirstOrDefault()?.fiIDMovimiento ?? 0;
                                }
                                var piIDInventarioMovimientoDetalle = new ObjectParameter("piIDInventarioMovimientoDetalle", typeof(int));
                                contexto.sp_Inventario_Movimiento_Detalle_Crear(
                                model.fiIDInventarioMovimientoMaestro,
                                2,
                                inv.fiIDMovimiento,
                                inv.fiIDUbicacionInicial == 0 ? model.fiIdUbicacion : inv.fiIDUbicacionInicial,
                                inv.fiIDUbicacionDestino,
                                inv.fcCodigoSerie1 ?? "",
                                inv.fcCodigoSerie2 ?? "",
                                inv.fnCantidad,
                                inv.fcDescripcion ?? "",
                                GetIdUser(),
                                piIDInventarioMovimientoDetalle);
                            }
                        }
                    }

                    return EnviarResultado(result, "", result ? "Salida de Inventario editada exitosamente" : "Error de red");

                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }

        public ActionResult RptInventarioSalida(int fiIDInventarioMovimientoMaestro)
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

        [HttpPost]
        public bool GetPDFSalidaInventario(CrearSalidaInventarioViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var ms = new MemoryStream();
                var pw = new PdfWriter(ms);
                var pdfDocument = new PdfDocument(pw);

                string pathImageNovanetLogo = System.IO.Path.Combine(MemoryLoadManager.URL, @"Content\img\LogoPrestadito.png");
                var prestaditoLogo = new iText.Layout.Element.Image(ImageDataFactory.Create(pathImageNovanetLogo));

                string pathImageBackGround = System.IO.Path.Combine(MemoryLoadManager.URL, @"Content\img\HOJAS_NOVANET.png");
                var backGround = new iText.Layout.Element.Image(ImageDataFactory.Create(pathImageBackGround));

                pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, new BackgroundImageHandler(backGround));
                var doc = new Document(pdfDocument, PageSize.LETTER);

                doc.SetMargins(20, 20, 5, 20);

                doc.SetProperty(Property.LEADING, new Leading(Leading.MULTIPLIED, 1f));
                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                var imagen = new MemoryStream();
                var ubicaciones = ListarUbicacionesPorTipo();
                if (model.Firma != null)
                {
                    model.Firma.InputStream.CopyTo(imagen);
                    model.Firma.InputStream.Position = imagen.Position = 0;
                }
                else
                {
                    var client = new WebClient();
                    var uri = GetContentDocument("img", @"inventario\firmas", "FirmaSalidaInventario-" + model.fiIDInventarioMovimientoMaestro + ".png");
                    var content = client.DownloadData(uri);
                    imagen = new MemoryStream(content);
                }

                var modelDb = contexto.sp_Inventario_Movimiento_Maestro_GetById(model.fiIDInventarioMovimientoMaestro).FirstOrDefault();
                var ubicacionInicial = ubicaciones.FirstOrDefault(x => x.fiIDUbicacion == modelDb.fiIDUbicacion);
                var ubicacionDestino = ubicaciones.FirstOrDefault(x => x.fiIDUbicacion == modelDb.fiIDUbicacionDestino);


                var stringEntrada = "";
                if (ubicacionInicial.fcTipoUbicacion == "BODEGA") stringEntrada = $"{ubicacionInicial.fcUbicacion.ToUpper()}";
                if (ubicacionInicial.fcTipoUbicacion == "USUARIO") stringEntrada = $"USUARIO {ubicacionInicial.fcNombreCorto.ToUpper()} - {ubicacionInicial.fcNombreComercial.ToUpper()}";
                if (ubicacionInicial.fcTipoUbicacion == "CLIENTE") stringEntrada = $"CLIENTE {ubicacionInicial.fcNombreCorto.ToUpper()}";

                var stringSalida = "";
                if (ubicacionDestino.fcTipoUbicacion == "BODEGA") stringSalida = $"{ubicacionDestino.fcUbicacion}";
                if (ubicacionDestino.fcTipoUbicacion == "USUARIO") stringSalida = $"USUARIO {ubicacionDestino.fcNombreCorto.ToUpper()} - {ubicacionDestino.fcNombreComercial.ToUpper()}";
                if (ubicacionDestino.fcTipoUbicacion == "CLIENTE") stringSalida = $"CLIENTE {ubicacionDestino.fcNombreCorto.ToUpper()}";

                var stringFirma = "";
                if (ubicacionDestino.fcTipoUbicacion == "BODEGA") stringFirma = $"FIRMA ENCARGADO BODEGA";
                if (ubicacionDestino.fcTipoUbicacion == "USUARIO") stringFirma = $"FIRMA TECNICO";
                if (ubicacionDestino.fcTipoUbicacion == "CLIENTE") stringFirma = $"FIRMA CLIENTE";

                var modelDbDetalle = contexto.sp_Inventario_Movimiento_Detalle_GetIDInventarioMovimientoMaestro(model.fiIDInventarioMovimientoMaestro).ToList();
                var tipoMovimiento = contexto.sp_Catalogo_TipoMovimiento_GetById(modelDb.fiIDTipoMovimiento).FirstOrDefault();

                var tablaEncabezado = new Table(UnitValue.CreatePercentArray(new float[] { 25f, 50f, 25f })).SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                var cellLogo = new Cell(3, 1).Add(prestaditoLogo.SetAutoScale(true).SetBorder(Border.NO_BORDER)).SetBorder(Border.NO_BORDER);
                tablaEncabezado.AddCell(cellLogo);

                var cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text($"PRESTADITO S.A de C.V.").SetBold()).SetTextAlignment(TextAlignment.CENTER).SetFontSize(13f)).SetBorder(Border.NO_BORDER);
                tablaEncabezado.AddCell(cellInfoEmpresa);

                cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text($"Fecha de Posteo: {modelDb.fdFechaCreacion:dd/MM/yyyy}")).SetTextAlignment(TextAlignment.RIGHT).SetFontSize(9f)).SetBorder(Border.NO_BORDER);
                tablaEncabezado.AddCell(cellInfoEmpresa);

                cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text($"NOVANET").SetBold()).SetTextAlignment(TextAlignment.CENTER).SetFontSize(13f)).SetBorder(Border.NO_BORDER);
                tablaEncabezado.AddCell(cellInfoEmpresa);

                cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text($"Fecha de impresion: {DateTime.Now:dd/MM/yyyy}")).SetTextAlignment(TextAlignment.RIGHT).SetFontSize(9f)).SetBorder(Border.NO_BORDER);
                tablaEncabezado.AddCell(cellInfoEmpresa);

                cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text($"{tipoMovimiento.fcTipoMovimiento}").SetBold()).SetTextAlignment(TextAlignment.CENTER).SetFontSize(11f)).SetBorder(Border.NO_BORDER);
                tablaEncabezado.AddCell(cellInfoEmpresa);
                tablaEncabezado.AddCell(new Cell(1, 1).Add(new Paragraph("\n")).SetBorder(Border.NO_BORDER));

                doc.Add(tablaEncabezado);

                doc.Add(new Paragraph());
                doc.Add(new Paragraph());
                doc.Add(new Paragraph());
                doc.Add(new Paragraph());

                tablaEncabezado = new Table(UnitValue.CreatePercentArray(new float[] { 25f, 50f, 25f })).SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text($"CREADO POR: ").SetBold()).SetTextAlignment(TextAlignment.RIGHT).SetFontSize(11f)).SetBorder(Border.NO_BORDER);
                tablaEncabezado.AddCell(cellInfoEmpresa);

                cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text($"{modelDb.fcUsuarioCreacion.ToUpper()}").SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(11f)).SetBorder(Border.NO_BORDER);
                tablaEncabezado.AddCell(cellInfoEmpresa).AddCell(new Cell(1, 1).Add(new Paragraph("")).SetBorder(Border.NO_BORDER));


                cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text($"ORIGEN: ").SetBold()).SetTextAlignment(TextAlignment.RIGHT).SetFontSize(11f)).SetBorder(Border.NO_BORDER);
                tablaEncabezado.AddCell(cellInfoEmpresa);
                cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text($"{stringEntrada}").SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(11f)).SetBorder(Border.NO_BORDER);
                tablaEncabezado.AddCell(cellInfoEmpresa).AddCell(new Cell(1, 1).Add(new Paragraph("")).SetBorder(Border.NO_BORDER));

                cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text($"DESTINO: ").SetBold()).SetTextAlignment(TextAlignment.RIGHT).SetFontSize(11f)).SetBorder(Border.NO_BORDER);
                tablaEncabezado.AddCell(cellInfoEmpresa);
                cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text($"{stringSalida}").SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(11f)).SetBorder(Border.NO_BORDER);
                tablaEncabezado.AddCell(cellInfoEmpresa).AddCell(new Cell(1, 1).Add(new Paragraph("")).SetBorder(Border.NO_BORDER));
                doc.Add(tablaEncabezado);

                doc.Add(new Paragraph());
                doc.Add(new Paragraph());
                doc.Add(new Paragraph());
                doc.Add(new Paragraph());

                var tablaProductos = new Table(5, false).SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.CENTER);

                var index = new Cell(1, 1).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("#").SetBold()).SetBorder(Border.NO_BORDER);
                var producto = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph("Producto").SetBold()).SetBorder(Border.NO_BORDER);
                var serie = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph("Serie").SetBold()).SetBorder(Border.NO_BORDER);
                var mac = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph("Mac / Serie2").SetBold()).SetBorder(Border.NO_BORDER);
                var cantidad = new Cell(1, 1).SetTextAlignment(TextAlignment.RIGHT).Add(new Paragraph("Cantidad").SetBold()).SetBorder(Border.NO_BORDER).SetPaddingRight(30f);

                tablaProductos.AddHeaderCell(index);
                tablaProductos.AddHeaderCell(producto);
                tablaProductos.AddHeaderCell(cantidad);
                tablaProductos.AddHeaderCell(serie);
                tablaProductos.AddHeaderCell(mac);

                var i = 0;
                foreach (var item in modelDbDetalle)
                {

                    i++;
                    var dataIndex = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).SetFontSize(9f).Add(new Paragraph(i.ToString())).SetBorder(Border.NO_BORDER);
                    var dataProducto = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).SetFontSize(9f).Add(new Paragraph((item.fcProducto ?? "").ToUpper())).SetBorder(Border.NO_BORDER);
                    var dataSerie = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).SetFontSize(9f).Add(new Paragraph(item.fcCodigoSerie1.ToUpper())).SetBorder(Border.NO_BORDER);
                    var dataMac = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).SetFontSize(9f).Add(new Paragraph(item.fcCodigoSerie2.ToUpper())).SetBorder(Border.NO_BORDER);
                    var dataCantidad = new Cell(1, 1).SetTextAlignment(TextAlignment.RIGHT).SetFontSize(9f).Add(new Paragraph(item.fnCantidad.ToString("0.##"))).SetBorder(Border.NO_BORDER).SetPaddingRight(30f);


                    tablaProductos.AddCell(dataIndex)
                        .AddCell(dataProducto)
                        .AddCell(dataCantidad)
                        .AddCell(dataSerie)
                        .AddCell(dataMac)
                        ;

                }
                doc.Add(tablaProductos);
                doc.Add(new Paragraph("\n\n\n"));


                if (modelDb.fiFirma == 1)
                {
                    try {
                        var data = ImageDataFactory.Create(imagen.ToArray());

                        var image = new iText.Layout.Element.Image(data);
                        doc.Add(image);
                    }
                    catch(Exception)
                    {

                    }
                    
                }

                doc.Add(new Paragraph("\n"));
                var lineaFirma = new Table(1, false).SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.CENTER);

                var linea = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph()).SetBorder(null).SetBorderBottom(new SolidBorder(1f));
                lineaFirma.AddCell(linea);
                doc.Add(lineaFirma);
                doc.Add(new Paragraph($"{stringFirma}").SetTextAlignment(TextAlignment.CENTER));

                doc.Close();
                var bytesStream = ms.ToArray();
                ms = new MemoryStream();
                ms.Write(bytesStream, 0, bytesStream.Length);
                ms.Position = 0;
                TempData["ReportePDF"] = ms;
                TempData["NombreArchivo"] = $"SalidaDeInventario-{model.fiIDInventarioMovimientoMaestro}.pdf";
                return true;
            }
        }



        [HttpPost]
        public bool GetPDFSalidaInventarioViejo(CrearSalidaInventarioViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {

                var imagen = new MemoryStream();
                var ubicaciones = ListarUbicacionesPorTipo();
                if (model.Firma != null)
                {
                    model.Firma.InputStream.CopyTo(imagen);
                    model.Firma.InputStream.Position = imagen.Position = 0;
                }
                else
                {
                    var client = new WebClient();
                    var uri = GetContentDocument("img", @"inventario\firmas", "FirmaSalidaInventario-" + model.fiIDInventarioMovimientoMaestro + ".png");
                    var content = client.DownloadData(uri);
                    imagen = new MemoryStream(content);
                }

                var modelDb = contexto.sp_Inventario_Movimiento_Maestro_GetById(model.fiIDInventarioMovimientoMaestro).FirstOrDefault();
                var ubicacionInicial = ubicaciones.FirstOrDefault(x => x.fiIDUbicacion == modelDb.fiIDUbicacion);
                var ubicacionDestino = ubicaciones.FirstOrDefault(x => x.fiIDUbicacion == modelDb.fiIDUbicacionDestino);

                var stringSalida = "";
                if (ubicacionDestino.fcTipoUbicacion == "BODEGA") stringSalida = $"a la bodega {ubicacionDestino.fcUbicacion}";
                if (ubicacionDestino.fcTipoUbicacion == "USUARIO") stringSalida = $"al usuario {ubicacionDestino.fcNombreCorto} de la empresa {ubicacionDestino.fcNombreComercial}";
                if (ubicacionDestino.fcTipoUbicacion == "CLIENTE") stringSalida = $"al cliente {ubicacionDestino.fcNombreCorto}";

                var modelDbDetalle = contexto.sp_Inventario_Movimiento_Detalle_GetIDInventarioMovimientoMaestro(model.fiIDInventarioMovimientoMaestro).ToList();
                var ms = new MemoryStream();

                var pw = new PdfWriter(ms);

                var pdfDocument = new PdfDocument(pw);
                var doc = new Document(pdfDocument, PageSize.LETTER);


                doc.SetMargins(75, 35, 70, 35);

                doc.Add(new Paragraph($"Entrega de Inventario {stringSalida} desde {ubicacionInicial.fcUbicacion} el dia {modelDb.fdFechaCreacion:dd/MM/yyyy}").SetTextAlignment(TextAlignment.JUSTIFIED));

                var table = new Table(5, false).SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.CENTER);

                var index = new Cell(1, 1).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("#").SetBold()).SetBorder(Border.NO_BORDER);
                var producto = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph("Producto").SetBold()).SetBorder(Border.NO_BORDER);
                var serie = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph("Serie").SetBold()).SetBorder(Border.NO_BORDER);
                var mac = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph("Mac / Serie2").SetBold()).SetBorder(Border.NO_BORDER);
                var cantidad = new Cell(1, 1).SetTextAlignment(TextAlignment.RIGHT).Add(new Paragraph("Cantidad").SetBold()).SetBorder(Border.NO_BORDER);

                table.AddHeaderCell(index);
                table.AddHeaderCell(producto);
                table.AddHeaderCell(serie);
                table.AddHeaderCell(mac);
                table.AddHeaderCell(cantidad);

                var i = 0;

                foreach (var item in modelDbDetalle)
                {
                    i++;
                    var dataIndex = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph(i.ToString())).SetBorder(Border.NO_BORDER);
                    var dataProducto = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph(item.fcProducto ?? "")).SetBorder(Border.NO_BORDER);
                    var dataSerie = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph(item.fcCodigoSerie1)).SetBorder(Border.NO_BORDER);
                    var dataMac = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph(item.fcCodigoSerie2)).SetBorder(Border.NO_BORDER);
                    var dataCantidad = new Cell(1, 1).SetTextAlignment(TextAlignment.RIGHT).Add(new Paragraph(item.fnCantidad.ToString("0.##"))).SetBorder(Border.NO_BORDER);


                    table.AddCell(dataIndex)
                        .AddCell(dataProducto)
                        .AddCell(dataSerie)
                        .AddCell(dataMac)
                        .AddCell(dataCantidad);


                }
                doc.Add(table);
                doc.Add(new Paragraph("\n\n\n"));

                //byte[] thePictureAsBytes = new byte[imagen.Length];
                //using (BinaryReader theReader = new BinaryReader(imagen))
                //{
                //    thePictureAsBytes = imagen.ToArray(); //theReader.ReadBytes(imagen.);
                //}

                if (modelDb.fiFirma == 1)
                {
                    var data = ImageDataFactory.Create(imagen.ToArray());

                    var image = new iText.Layout.Element.Image(data);
                    doc.Add(image);
                }

                doc.Add(new Paragraph("\n"));
                var lineaFirma = new Table(1, false).SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.CENTER);

                var linea = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph()).SetBorder(null).SetBorderBottom(new SolidBorder(1f));
                lineaFirma.AddCell(linea);
                doc.Add(lineaFirma);
                doc.Add(new Paragraph("Firma del Tecnico").SetTextAlignment(TextAlignment.CENTER));
                doc.Close();

                var bytesStream = ms.ToArray();
                ms = new MemoryStream();
                ms.Write(bytesStream, 0, bytesStream.Length);
                ms.Position = 0;
                TempData["ReportePDF"] = ms;
                TempData["NombreArchivo"] = $"SalidaDeInventario-{model.fiIDInventarioMovimientoMaestro}.pdf";
                return true;
            }
        }


        [HttpPost]
        public JsonResult EnviarModalFirmaSalidaInventario(InfoUsuarioViewModel usuario, int fiIDInventarioMovimientoMaestro, string usuarioPeticion)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {

                    var modelDb = contexto.sp_Inventario_Movimiento_Maestro_GetById(fiIDInventarioMovimientoMaestro).FirstOrDefault();
                    var modeloR = new ObjSignalRModalViewModel
                    {
                        InfoUsuario = usuario,
                        Url = Url.Action("FirmaInventarioSalida", "Productos"),
                        Obj = new { fiIDInventarioMovimientoMaestro, usuarioPeticion, fiIDTipoMomvimento = modelDb.fiIDTipoMovimiento }
                    };
                    var algo = EnviarFormularioSignalR<object>(modeloR);
                    return EnviarResultado(true, "", algo);
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }

        }





        [HttpGet]
        public ActionResult FirmaInventarioSalida(int fiIDInventarioMovimientoMaestro, string usuarioPeticion)
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
                model.UsuarioPeticion = usuarioPeticion;
                model.fiIDTipoMomvimento = modelDb.fiIDTipoMovimiento;
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






        public async Task<JsonResult> ConfirmarSalidaInventario(CrearSalidaInventarioViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var archivo = model.Firma;
                    var modelDb = contexto.sp_Inventario_Movimiento_Maestro_GetById(model.fiIDInventarioMovimientoMaestro).FirstOrDefault();
                    var result = false;
                    if (modelDb.fiFirma == 0)
                    {
                        result = contexto.sp_Inventario_Movimiento_Maestro_ConfirmarSalidaInventario(model.fiIDInventarioMovimientoMaestro, model.Firma.FileName, GetIdUser()) > 0;
                    }
                    else
                    {
                        return EnviarResultado(false, "Error", "Inventario ya fue firmado");
                    }


                    if (result)
                    {
                        if (model.fiIDTipoMomvimento == 0)
                        {
                            model.fdFechaCreacion = modelDb.fdFechaCreacion;
                            model.fiIDTipoMomvimento = modelDb.fiIDTipoMovimiento;
                        }


                        var algo = GetPDFSalidaInventario(model);
                        UploadFileServer148(@"inventario\firmas", archivo);
                        var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
                        hubContext.Clients.All.actualizarSalidaInventario(model.UsuarioPeticion);

                        var ubicaciones = ListarUbicacionesPorTipo();
                        var ubicacionInicial = ubicaciones.FirstOrDefault(x => x.fiIDUbicacion == modelDb.fiIDUbicacion);
                        var ubicacionDestino = ubicaciones.FirstOrDefault(x => x.fiIDUbicacion == modelDb.fiIDUbicacionDestino);
                        var stringSalida = "";
                        if (ubicacionDestino.fcTipoUbicacion == "BODEGA") stringSalida = $"a la bodega {ubicacionDestino.fcUbicacion}";
                        if (ubicacionDestino.fcTipoUbicacion == "USUARIO") stringSalida = $"al usuario {ubicacionDestino.fcNombreCorto} de la empresa {ubicacionDestino.fcNombreComercial}";
                        if (ubicacionDestino.fcTipoUbicacion == "CLIENTE") stringSalida = $"al cliente {ubicacionDestino.fcNombreCorto}";

                        EnviarNotificacion($"Salida de inventario N#{model.fiIDInventarioMovimientoMaestro} desde {ubicacionInicial.fcUbicacion} {stringSalida}");
                        await MensajesWhatsappSalidasInventario(model.fiIDInventarioMovimientoMaestro);
                        return EnviarResultado(result, "", "Salida exitosa");
                    }
                    return EnviarResultado(result, "", "Error de red");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult EnviarModalCamaraExterna(InfoUsuarioViewModel usuario, string usuarioPeticion, string titulo)
        {
            try
            {
                var modeloR = new ObjSignalRModalViewModel
                {
                    InfoUsuario = usuario,
                    Url = Url.Action("CamaraExterna", "Productos"),
                    Obj = new { usuarioPeticion, titulo }
                };
                var algo = EnviarFormularioSignalR<object>(modeloR);
                return EnviarResultado(true, "", algo);
            }
            catch (Exception ex)
            {
                return EnviarException(ex, "Error");
            }
        }

        [HttpGet]
        public ActionResult CamaraExterna(string titulo, string usuarioPeticion)
        {
            var model = new CamaraExternaViewModel { Titulo = titulo, UsuarioPeticion = usuarioPeticion };
            return PartialView(model);
        }


        [HttpPost]

        public void RetornarResultado(CamaraExternaViewModel model)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
            hubContext.Clients.All.retornarResultado(model);
        }



        [HttpGet]
        public ActionResult ViewListaSalidaInventario(int fiIDInventarioMovimientoMaestro)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var lista = contexto.sp_Inventario_Movimiento_Detalle_GetIDInventarioMovimientoMaestro(fiIDInventarioMovimientoMaestro).ToList();
                var listModel = new List<ListInventarioMovimientoDetalleViewModel>();

                foreach (var item in lista)
                {
                    listModel.Add(new ListInventarioMovimientoDetalleViewModel
                    {
                        fcProducto = item.fcProducto,
                        fcCodigoSerie1 = item.fcCodigoSerie1,
                        fcCodigoSerie2 = item.fcCodigoSerie2,
                        fcDescripcion = item.fcDescripcion,
                        fcUbicacionInicial = item.fcUbicacionInicial,
                        fcUbicacionDestino = item.fcUbicacionDestino,
                        fnCantidad = item.fnCantidad,
                        fcPrecios = item.fcPrecios,
                        fiIDInventarioMovimientoMaestro = item.fiIDInventarioMovimientoMaestro
                    });
                }

                return PartialView(listModel);
            }
        }

        [HttpGet]
        public JsonResult CambiarStatusSalidaInventario(int fiIDInventarioMovimientoMaestro)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var model = contexto.sp_Inventario_Movimiento_Maestro_GetById(fiIDInventarioMovimientoMaestro).FirstOrDefault();
                    var result = contexto.sp_Inventario_Movimiento_Maestro_CambiarEstado(fiIDInventarioMovimientoMaestro, model.fiEstadoInventairoMovimientoMaestro == 0 ? 1 : 0, GetIdUser()) > 0;
                    if (result)
                    {
                        return EnviarResultado(true, "Salida de inventario", model.fiEstadoInventairoMovimientoMaestro == 0 ? "Salida Habilitada Exitosamente" : "Salida Deshabilitada Exitosamente");
                    }
                    else
                    {
                        return EnviarResultado(false, "Error", "Error de red");
                    }
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }


        [HttpGet]
        public async Task<JsonResult> RechazoSalidaInventario(int IDInventarioMovimiento)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var model = contexto.sp_Inventario_Movimiento_Maestro_GetById(IDInventarioMovimiento).FirstOrDefault();

                    var result = contexto.sp_Inventario_Movimiento_Maestro_CambiarEstado(IDInventarioMovimiento, 0, GetIdUser()) > 0;
                    


                    if (result)
                    {
                        var listUsuarios = new List<int>();
                        var bodegas = ListarUbicacionesPorTipo();

                        var bodegaInicial = bodegas.FirstOrDefault(x => x.fiIDUbicacion == model.fiIDUbicacion);
                        var bodegaFinal = bodegas.FirstOrDefault(x => x.fiIDUbicacion == model.fiIDUbicacionDestino);


                        if (contexto.sp_UbicacionesPorUsuarios_ListarPorUbicacion(model.fiIDUbicacion).Count() > 0)
                        {
                            listUsuarios.AddRange(contexto.sp_UbicacionesPorUsuarios_ListarPorUbicacion(model.fiIDUbicacion).Where(x => x.fbUsuarioPrincipal == true).Select(x => x.fiIDUsuario ?? 0));
                        }

                        if (contexto.sp_UbicacionesPorUsuarios_ListarPorUbicacion(model.fiIDUbicacionDestino).Count() > 0)
                        {
                            listUsuarios.AddRange(contexto.sp_UbicacionesPorUsuarios_ListarPorUbicacion(model.fiIDUbicacionDestino).Where(x => x.fbUsuarioPrincipal == true).Select(x => x.fiIDUsuario ?? 0));
                        }

                        if (contexto.sp_Usuarios_Maestros_ObtenerPorIdUbicacion(model.fiIDUbicacion).Count()>0 )
                        {
                            listUsuarios.Add(contexto.sp_Usuarios_Maestros_ObtenerPorIdUbicacion(model.fiIDUbicacion).FirstOrDefault().fiIDUsuario);
                        }

                        if (contexto.sp_Usuarios_Maestros_ObtenerPorIdUbicacion(model.fiIDUbicacionDestino).Count() > 0)
                        {
                            listUsuarios.Add(contexto.sp_Usuarios_Maestros_ObtenerPorIdUbicacion(model.fiIDUbicacionDestino).FirstOrDefault().fiIDUsuario);
                        }

                        listUsuarios = listUsuarios.Distinct().ToList();
                        var usuarios = MemoryLoadManager.ListaUsuarios.Where(x => listUsuarios.Any(y => y == x.fiIdUsuario)).Select(x => new { x.fcBuzondeCorreo, x.fcTelefonoMovil }).ToList();
                        var mensaje = $"Se rechazo el movimiento de inventario {IDInventarioMovimiento} que procedia {(bodegaInicial.fcTipoUbicacion == "BODEGA" ? "de la bodega" : "del usuario")} {(bodegaInicial.fcTipoUbicacion == "BODEGA" ? bodegaInicial.fcUbicacion : bodegaInicial.fcNombreCorto)} a {(bodegaFinal.fcTipoUbicacion == "BODEGA" ? "la bodega" : "el usuario")} {(bodegaFinal.fcTipoUbicacion == "BODEGA" ? bodegaFinal.fcUbicacion : bodegaFinal.fcNombreCorto)}";
                        var _emailTemplateService = new EmailTemplateService();
                        await _emailTemplateService.SendEmailPresonalizadoAVarios(new EmailTemplateServiceModel
                        {
                            ListCustomerEmail = usuarios.Select(x => x.fcBuzondeCorreo).ToList(),
                            HtmlBody = mensaje,
                            Comment = "Rechazo de Inventario",
                            List_CC = GetConfiguracion<string>("Correos_CC_Inventario", ',')
                        });

                        foreach(var item in usuarios.Select(x => x.fcTelefonoMovil).ToList())
                        {
                            if (!String.IsNullOrEmpty(item))
                            {
                                await MensajeriaApi.MensajeWhatsapp(item,mensaje);
                            }
                        }
                        

                        return EnviarResultado(true, "Salida de inventario", "Salida Deshabilitada Exitosamente");
                    }
                    else
                    {
                        return EnviarResultado(false, "Error", "Error de red");
                    }
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }


        [HttpGet]
        public async Task<JsonResult> EnviarCorreoSalidaDeInventario(int fiIDInventarioMovimientoMaestro)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Inventario_Movimiento_Maestro_GetById(fiIDInventarioMovimientoMaestro).FirstOrDefault();
                var usuario = contexto.sp_Usuarios_Maestros_ObtenerPorId(modelDb.fiIDUsuarioDestino).FirstOrDefault();
                var model = new EmailTemplateServiceModel();
                model.IdTransaccion = fiIDInventarioMovimientoMaestro;
                model.CustomerEmail = usuario.fcBuzondeCorreo;
                model.List_CC.AddRange(GetConfiguracion<string>("CorreosParaCC", ','));
                var _emailTemplateService = new EmailTemplateService();
                await _emailTemplateService.SendEmailToSalidaInventario(model);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        [AllowAnonymous]
        public HttpPostedFileBase GenerarDocumentoMovimientoInventario(int fiIDInventarioMovimientoMaestro, string fileName)
        {
            var DocumentoInventarioService = new ReportesTemplateService();
            var DocumentoInventario = new MemoryStream();
            var archivo = DocumentoInventarioService.GenerarSalidaInventario(fiIDInventarioMovimientoMaestro);
            archivo.CopyTo(DocumentoInventario);
            return new MemoryPostedFile(DocumentoInventario.ToArray(), fileName);
        }

        [HttpGet]
        public async Task<JsonResult> MensajesWhatsappSalidasInventario(int fiIDInventarioMovimientoMaestro)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {

                    var ubicaciones = ListarUbicacionesPorTipo();
                    var modelDb = contexto.sp_Inventario_Movimiento_Maestro_GetById(fiIDInventarioMovimientoMaestro).FirstOrDefault();
                    var ubicacionInicial = ubicaciones.FirstOrDefault(x => x.fiIDUbicacion == modelDb.fiIDUbicacion);
                    var ubicacionDestino = ubicaciones.FirstOrDefault(x => x.fiIDUbicacion == modelDb.fiIDUbicacionDestino);

                    var stringSalida = "";
                    if (ubicacionDestino.fcTipoUbicacion == "BODEGA") stringSalida = $"a la bodega {ubicacionDestino.fcUbicacion}";
                    if (ubicacionDestino.fcTipoUbicacion == "USUARIO") stringSalida = $"al usuario {ubicacionDestino.fcNombreCorto} de la empresa {ubicacionDestino.fcNombreComercial}";
                    if (ubicacionDestino.fcTipoUbicacion == "CLIENTE") stringSalida = $"al cliente {ubicacionDestino.fcNombreCorto}";


                    var nombreArchivo = modelDb.fiFirma == 1 ? $"MovimientoInventario_{fiIDInventarioMovimientoMaestro}_firmado.pdf" : $"MovimientoInventario_{fiIDInventarioMovimientoMaestro}.pdf";
                    var archivo = GenerarDocumentoMovimientoInventario(fiIDInventarioMovimientoMaestro, nombreArchivo);
                    var nombre = "https://orion.novanetgroup.com/Documento/wa/" + archivo.FileName;

                    if (ubicacionInicial.fcTipoUbicacion == "BODEGA")
                    {
                        await UploadFileServer148Async(@"Documento\WA", archivo);
                        var telefonos = GetConfiguracion<string>("Numeros_SalidaInventarioBodega", ',');
                        if (ubicacionDestino.fcTipoUbicacion == "USUARIO")
                        {
                            var idUsuario = contexto.sp_Usuarios_Maestros_ObtenerIdUsuario(ubicacionDestino.fcNombreCorto).FirstOrDefault();
                            var usuario = contexto.sp_Usuarios_Maestros_ObtenerPorId(idUsuario).FirstOrDefault();
                            if (usuario != null)
                            {
                                telefonos.Add(usuario.fcTelefonoMovil);
                            }

                        }
                        foreach (var telefono in telefonos)
                        {
                            //Convert.ToInt32(contexto.sp_MensajeriaMasiva_Detalle(GetIdUser(), telefono, $"Salida de Inventario de la bodega {ubicacionInicial.fcUbicacion} {stringSalida}", nombre));
                            await MensajeriaApi.MensajeWhatsapp(telefono, $"Salida de Inventario de la bodega {ubicacionInicial.fcUbicacion} {stringSalida} PDF en el siguiente link {nombre}");
                        }
                    }

                    return EnviarResultado(true, "Whatsapp", "Mensaje Enviado");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }



        public JsonResult ExistenciaProductosPorUbicacion(int fiIDUbicacion)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var productos = contexto.sp_Producto_Maestro_ListarPorUbicacion(fiIDUbicacion).ToList();
                return EnviarListaJson(productos);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> EnviarSalidaInventario(int fiIDInventarioMovimientoMaestro)
        {

            var email = new EmailTemplateService();
            var resultado = await email.SendEmailToSalidaInventarioPartida(new EmailTemplateServiceModel { IdTransaccion = fiIDInventarioMovimientoMaestro });
            return EnviarResultado(resultado, "", "Salida exitosa");
        }

        #endregion


        #region Devolucion
        public JsonResult BodegaPorUsuario(int fiIDUsuario)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var bodega = contexto.sp_Catalogo_Ubicaciones_ListarTipoUbicacion().FirstOrDefault(x => x.TipoUbicacion == "USUARIO" && x.fiIDUsuario == fiIDUsuario) ?? new sp_Catalogo_Ubicaciones_ListarTipoUbicacion_Result();
                return Json(bodega, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult IndexDevolucion()
        {
            return View();
        }


        public JsonResult CargarListaDevolucion()
        {
            using (var contexto = new ORIONDBEntities())
            {
                var lista = contexto.sp_Inventario_Movimiento_Maestro_Listar().Where(x => x.fiIDTipoMovimiento == 3).ToList();
                return EnviarListaJson(lista);
            }

        }

        [HttpGet]
        public ActionResult CrearDevolucionInventario()
        {
            ViewBag.ListUbicacionesPrestadito = GetListUbicaciones().Where(x => GetConfiguracion<string>("Catalogo_Ubicaciones_BodegasNovanet", ',').Any(y => y == x.Value)).ToList();
            ViewBag.ListTecnicosContratistas = GetUsuarios();
            ViewBag.ListaProductos = GetListProductos().Select(x => new { id = x.Value, text = x.Text });

            ViewBag.ListUbicaciones = GetListUbicaciones();
            ViewBag.ProductosSinSerie = GetListProductosConsumibles();
            ViewBag.idBodegaDeUsuario = 0;
            return View(new CrearSalidaInventarioViewModel());
        }

        [HttpPost]
        public JsonResult CrearDevolucionInventario(CrearSalidaInventarioViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var productosSinSerie = GetListProductosConsumibles();
                    var result = contexto.sp_Inventario_Movimiento_Maestro_Crear(
                        model.fiIdUbicacion,
                        model.fiIDUsuarioDestino,
                        GetIdUser(),
                        3, model.fiIDUbicacionDestino)?.FirstOrDefault() ?? 0;


                    if (result > 0)
                    {

                        var resultDevolucion = contexto.sp_DevolucionMaestro_Crear(model.fiIdUbicacion, model.fiIDUbicacionDestino, model.fcMotivoDevolucion, GetIdUser(), Convert.ToInt32(result)).FirstOrDefault();

                        foreach (var item in model.ListaInventarioDetalle)
                        {
                            if (item.fbPorCodigo == false)
                            {
                                item.fiIDMovimiento = contexto.sp_Productos_Maestro_GetIdMovimiento(item.fiIdProducto, item.fiIDUbicacionInicial)?.FirstOrDefault()?.fiIDMovimiento ?? 0;
                            }
                            var idInventarioMovimientoDetalle = new ObjectParameter("piIDInventarioMovimientoDetalle", typeof(int));
                            contexto.sp_Inventario_Movimiento_Detalle_Crear(
                                Convert.ToInt32(result),
                                3,
                                item.fiIDMovimiento,
                                item.fiIDUbicacionInicial == 0 ? model.fiIdUbicacion : item.fiIDUbicacionInicial,
                                item.fiIDUbicacionDestino,
                                item.fcCodigoSerie1 ?? "",
                                item.fcCodigoSerie2 ?? "",
                                item.fnCantidad,
                                item.fcDescripcion ?? "",
                                GetIdUser(), idInventarioMovimientoDetalle);

                            contexto.sp_DevolucionDetalle_Crear(Convert.ToInt32(resultDevolucion), item.fcCodigoSerie1, item.fcCodigoSerie2, item.fnCantidad, item.fcDescripcion, item.fiIdProducto, GetIdUser(), Convert.ToInt32(idInventarioMovimientoDetalle.Value));
                        }



                    }
                    return EnviarResultado(true, "", "Devolucion de Inventario Registrado exitosamente");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }

        }

        [HttpGet]
        public ActionResult EditarDevolucionInventario(int fiIDInventarioMovimientoMaestro)
        {
            ViewBag.ListUbicacionesPrestadito = GetListUbicaciones().Where(x => GetConfiguracion<string>("Catalogo_Ubicaciones_BodegasNovanet", ',').Any(y => y == x.Value)).ToList();
            ViewBag.ListTecnicosContratistas = GetUsuarios();
            ViewBag.ListUbicaciones = GetListUbicaciones();
            ViewBag.ListaProductos = GetListProductos().Select(x => new { id = x.Value, text = x.Text });
            ViewBag.ProductosSinSerie = GetListProductosConsumibles();

            using (var contexto = new ORIONDBEntities())
            {
                var model = new CrearSalidaInventarioViewModel();

                var modelDbSalidaInventario = contexto.sp_Inventario_Movimiento_Maestro_GetById(fiIDInventarioMovimientoMaestro).FirstOrDefault();
                var modelDbDevolucionMaestro = contexto.sp_DevolucionMaestro_ObtenerPorMovimientoMaestro(fiIDInventarioMovimientoMaestro).FirstOrDefault();

                var modelDbDetalle = contexto.sp_Inventario_Movimiento_Detalle_GetIDInventarioMovimientoMaestro(fiIDInventarioMovimientoMaestro).ToList();
                var modelDbDevolucionDetalle = contexto.sp_DevolucionDetalle_Listar(modelDbDevolucionMaestro.fiIDDevolucionMaestro).ToList();

                model.fiIDInventarioMovimientoMaestro = modelDbSalidaInventario.fiIDInventarioMovimientoMaestro;
                model.fiIdUbicacion = modelDbSalidaInventario.fiIDUsuarioDestino;
                model.fiIDUsuarioDestino = modelDbDevolucionMaestro.fiIDUbicacionDestino;
                model.fiIDUbicacionDestino = modelDbDevolucionMaestro.fiIDUbicacionDestino;
                model.fiIDTipoMomvimento = modelDbSalidaInventario.fiIDTipoMovimiento;
                model.fcMotivoDevolucion = modelDbDevolucionMaestro.fcMotivoDevolucion;
                model.fbEditar = true;
                ViewBag.idBodegaDeUsuario = modelDbDevolucionMaestro.fiIDUbicacionInicial;
                if (modelDbDetalle.Count > 0)
                {
                    model.ListaInventarioDetalle = new List<ListInventarioMovimientoDetalleViewModel>();
                    foreach (var itemDb in modelDbDetalle)
                    {
                        var item = new ListInventarioMovimientoDetalleViewModel
                        {
                            fiIDInventarioMovimientoDetalle = itemDb.fiIDInventarioMovimientoDetalle,
                            fiIDInventarioMovimientoMaestro = itemDb.fiIDInventarioMovimientoMaestro,
                            fiIDTipoMovimiento = itemDb.fiIDTipoMovimiento,
                            fiIDMovimiento = itemDb.fiIDMovimiento,
                            fiIDUbicacionInicial = itemDb.fiIDUbicacionInicial,
                            fiIDUbicacionDestino = itemDb.fiIDUbicacionDestino,
                            fiIdProducto = itemDb.fiIDProducto ?? 0,
                            fcProducto = itemDb.fcProducto,
                            fcCodigoSerie1 = itemDb.fcCodigoSerie1,
                            fcCodigoSerie2 = itemDb.fcCodigoSerie2,
                            fnCantidad = itemDb.fnCantidad,
                            fcDescripcion = itemDb.fcDescripcion,
                            fbPorCodigo = itemDb.fcCodigoSerie1.Trim() == "" ? false : true,
                            fcToken = itemDb.fcToken
                        };
                        model.ListaInventarioDetalle.Add(item);
                    }
                }
                return View("CrearDevolucionInventario", model);
            }



        }



        [HttpPost]
        public JsonResult EditarDevolucionInventario(CrearSalidaInventarioViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var modelDb = contexto.sp_Inventario_Movimiento_Maestro_GetById(model.fiIDInventarioMovimientoMaestro).FirstOrDefault();
                    var modelDbDetalle = contexto.sp_Inventario_Movimiento_Detalle_GetIDInventarioMovimientoMaestro(model.fiIDInventarioMovimientoMaestro).ToList();
                    var modelDbDevolucion = contexto.sp_DevolucionMaestro_ObtenerPorMovimientoMaestro(model.fiIDInventarioMovimientoMaestro).FirstOrDefault();

                    var result = contexto.sp_Inventario_Movimiento_Maestro_Editar(
                        model.fiIDInventarioMovimientoMaestro,
                        model.fiIdUbicacion,
                        model.fiIDUsuarioDestino,
                        modelDb.fiIDTipoMovimiento,
                        3,
                        1, model.fiIDUbicacionDestino) > 0;

                    contexto.sp_DevolucionMaestro_Editar(model.fiIdUbicacion, model.fiIDUbicacionDestino, model.fcMotivoDevolucion, GetIdUser(), model.fiIDInventarioMovimientoMaestro);

                    if (result)
                    {
                        foreach (var item in modelDbDetalle)
                        {
                            if (model.ListaInventarioDetalle?.Any(x => x.fcToken == item.fcToken) ?? false)
                            {
                                var itemModel = model.ListaInventarioDetalle.FirstOrDefault(x => x.fcToken == item.fcToken);
                                if (itemModel.fiIDMovimiento == 0)
                                {
                                    itemModel.fiIDMovimiento = contexto.sp_Productos_Maestro_GetIdMovimiento(itemModel.fiIdProducto, item.fiIDUbicacionInicial)?.FirstOrDefault()?.fiIDMovimiento ?? 0;
                                }
                                model.ListaInventarioDetalle.FirstOrDefault(x => x.fcToken == item.fcToken).fbEditado = true;
                                contexto.sp_Inventario_Movimiento_Detalle_Editar(
                                    itemModel.fiIDInventarioMovimientoDetalle,
                                    itemModel.fiIDInventarioMovimientoMaestro,
                                    3,
                                    itemModel.fiIDMovimiento,
                                    itemModel.fiIDUbicacionInicial == 0 ? model.fiIdUbicacion : itemModel.fiIDUbicacionInicial,
                                    itemModel.fiIDUbicacionDestino,
                                    itemModel.fcCodigoSerie1 ?? "",
                                    itemModel.fcCodigoSerie2 ?? "",
                                    itemModel.fnCantidad,
                                    itemModel.fcDescripcion ?? "",
                                    GetIdUser()
                                );
                                contexto.sp_DevolucionDetalle_Editar(itemModel.fiIDInventarioMovimientoDetalle, 0, itemModel.fcCodigoSerie1, itemModel.fcCodigoSerie2, itemModel.fnCantidad, itemModel.fcDescripcion, itemModel.fiIdProducto, GetIdUser());
                            }
                            else
                            {
                                contexto.sp_Inventario_Movimiento_Detalle_Eliminar(item.fiIDInventarioMovimientoDetalle);
                            }
                        }

                        if (model.ListaInventarioDetalle != null)
                        {

                            foreach (var inv in model.ListaInventarioDetalle.Where(x => x.fbEditado == false).ToList())
                            {
                                if (inv.fbPorCodigo == false)
                                {
                                    inv.fiIDMovimiento = contexto.sp_Productos_Maestro_GetIdMovimiento(inv.fiIdProducto, model.fiIdUbicacion)?.FirstOrDefault()?.fiIDMovimiento ?? 0;
                                }
                                var piIDInventarioMovimientoDetalle = new ObjectParameter("piIDInventarioMovimientoDetalle", typeof(int));
                                contexto.sp_Inventario_Movimiento_Detalle_Crear(
                                model.fiIDInventarioMovimientoMaestro,
                                3,
                                inv.fiIDMovimiento,
                                inv.fiIDUbicacionInicial == 0 ? model.fiIdUbicacion : inv.fiIDUbicacionInicial,
                                inv.fiIDUbicacionDestino,
                                inv.fcCodigoSerie1 ?? "",
                                inv.fcCodigoSerie2 ?? "",
                                inv.fnCantidad,
                                inv.fcDescripcion ?? "",
                                GetIdUser(),
                                piIDInventarioMovimientoDetalle);

                                contexto.sp_DevolucionDetalle_Crear(modelDbDevolucion.fiIDDevolucionMaestro, inv.fcCodigoSerie1, inv.fcCodigoSerie2, inv.fnCantidad, inv.fcDescripcion, inv.fiIdProducto, GetIdUser(), Convert.ToInt32(piIDInventarioMovimientoDetalle.Value));

                            }
                        }
                    }

                    return EnviarResultado(result, "", result ? "Salida de Inventario editada exitosamente" : "Error de red");

                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }

        [HttpGet]
        public JsonResult EditarUbicacionProducto(string codigo, int idUbicacionInicial)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var model = contexto.sp_Producto_Movimiento_Detalle_ByCodigoSerie(codigo).FirstOrDefault();
                contexto.sp_Producto_Movimiento_Detalle_Editar(model.fiIDMovimiento, model.fiIDProducto, idUbicacionInicial, 1, model.fcCodigoSerie1, model.fcCodigoSerie2, model.fcReferenciaMovimiento, model.fnCantidad, model.fiIdProductoPreciosDetalle, null);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        #region Instalacion
        public ActionResult IndexInventarioInstalacion()
        {
            return View();
        }

        public JsonResult CargarListaInstalacion()
        {
            using (var contexto = new ORIONDBEntities())
            {
                var ubicaciones = ListarUbicacionesPorTipo();
                var lista = contexto.sp_Inventario_Movimiento_Maestro_Listar().Where(x => x.fiIDTipoMovimiento == 4).ToList();
                lista.ForEach(x =>
                {
                    var ubicacionInicial = ubicaciones.FirstOrDefault(y => y.fiIDUbicacion == x.fiIDUbicacion);
                    var ubicacionDestino = ubicaciones.FirstOrDefault(y => y.fiIDUbicacion == x.fiIDUbicacionDestino);

                    x.fcUbicacion = ubicacionInicial?.fcNombreCorto ?? "";
                    x.fcDestinatario = (ubicacionDestino?.fcUbicacion + " | " ?? "") + (ubicacionDestino?.fcNombreCorto ?? "");

                });
                return EnviarListaJson(lista);
            }
        }

        #endregion

        #region Reportes
        public ActionResult IndexResumen()
        {
            using (var contexto = new ORIONDBEntities())
            {
                var resultados = contexto.sp_Productos_Maestro_ExistenciaInventario().ToList();
                var encabezados = new List<string>();

                encabezados.AddRange(resultados.GroupBy(x => x.TipoUbicacion).Select(x => x.Key).ToList());

                return View(encabezados);
            }

        }

        public JsonResult ListaResumenInventario()
        {
            using (var contexto = new ORIONDBEntities())
            {

                return EnviarListaJson(contexto.sp_Productos_Maestro_ExistenciaInventario().ToList());
            }
        }



        public ActionResult IndexInventarioPorUbicacion()
        {
            ViewBag.ListUbicaciones = GetListUbicaciones();
            return View();
        }

        public JsonResult ListaInventarioPorUbicacionDetallado(int fiIdUbicacion)
        {
            using (var contexto = new ORIONDBEntities())
            {
                return EnviarListaJson(contexto.sp_InventarioPorUbicacionDetallado(fiIdUbicacion).ToList());
            }
        }

        [AllowAnonymous]
        public JsonResult ListaResumenInventarioParaStock()
        {
            using (var contexto = new ORIONDBEntities())
            {
                var listaProductosStockEspeciales = new Dictionary<int, decimal>();
                var stockMinimo = GetConfiguracion<string>("Inventario_StockMinimoEspeciales_ID:CANTIDAD", ',');
                var listaStockMinimo = stockMinimo.Select(x => x.Split(':')).ToList();
                var listInventarioBodegas = contexto.sp_Productos_Maestro_ExistenciaInventarioBodegas().Where(x => x.fnCantidad > 0 || x.fbProductoDeInstalacion == true).ToList();

                foreach (var item in listaStockMinimo)
                {
                    listaProductosStockEspeciales.Add(Convert.ToInt32(item[0]), Convert.ToDecimal(item[1]));
                }

                var listModel = listInventarioBodegas.Select(x =>
                {
                    var producto = new ListProductosBodegaViewModel
                    {
                        fiIDProducto = x.fiIDProducto,
                        fcProducto = x.fcProducto,
                        fiIDUbicacion = x.fiIDUbicacion,
                        fcUbicacion = x.TipoUbicacion,
                        fnCantidad = x.fnCantidad ?? 0,
                        fnStockMinimo = listaProductosStockEspeciales?.FirstOrDefault(y => y.Key == x.fiIDProducto).Value > 0 ? listaProductosStockEspeciales.FirstOrDefault(y => y.Key == x.fiIDProducto).Value : GetConfiguracion<decimal>("Inventario_StockMinimoProductos"),
                    };

                    producto.fbStockMinimo = producto.fnCantidad <= producto.fnStockMinimo;
                    return producto;
                }).OrderBy(x => x.fiIDUbicacion).ToList();

                return EnviarListaJson(listModel);
            }
        }

        public ActionResult IndexCantidadProductosIngresados()
        {
            return View();
        }


        public JsonResult ListarTotalProductos()
        {
            using(var contexto = new ORIONDBEntities())
            {
                return EnviarListaJson(contexto.sp_Productos_Maestro_IngresoTotal().ToList());
            }
        }


        public ActionResult ViewProductosEnClientes(List<ListProductosPorClientes> model)
        {
            return PartialView(model);
        }

        public JsonResult ListarTotalProductoPorClientes(List<ListProductosPorClientes> model)
        {
            using (var contexto = new ORIONDBEntities())
            {

                var connection = contexto.Database.Connection;
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_OrionSolicitud_CantidadProductosInventarioPorCliente '{String.Join(",", model.Select(x => x.fiIDProducto))}'";

                var list = new List<Dictionary<string, object>>();
                var listDinamico = model.Select(x => x.fcProducto.Replace(" ", "_")).ToList();


                connection.Open();




                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var dictionary = new Dictionary<string, object>();

                        dictionary.Add("fiIDCliente", Convert.ToInt32(reader["fiIDCliente"]));
                        dictionary.Add("fiIDSolicitud", Convert.ToInt32(reader["fiIDSolicitud"]));
                        dictionary.Add("fcNombre", reader["fcNombre"]);
                        foreach(var item in listDinamico)
                        {
                            if (reader.GetOrdinal(item) >= 0 && !reader.IsDBNull(reader.GetOrdinal(item)))
                            {
                                dictionary.Add(item, Convert.ToInt32(reader[item]));
                            }
                            else
                            {
                                // Manejo de errores o valor predeterminado en caso de que la columna no exista o sea nula
                                dictionary.Add(item, 0); // Por ejemplo, puedes añadir 0 si la columna no existe
                            }
                        }
                        list.Add(dictionary);
                    }
                }
                connection.Close();
                return EnviarListaJson(list);
            }
        }

        #endregion


        #region Facturacion
        public ActionResult IndexFacturacion()
        {
            return View();
        }


        public ActionResult CargarListaFacturacionInventario()
        {
            using (var contexto = new ORIONDBEntities())
            {
                var lista = contexto.sp_Inventario_Movimiento_Maestro_ListarFacturacion().ToList();
                return EnviarListaJson(lista);
            }
        }

        [HttpGet]
        public ActionResult EditarFacturacionInventario(int fiIDInventarioMovimientoMaestro)
        {
            ViewBag.ListUbicaciones = GetListUbicaciones();
            ViewBag.ListTecnicosContratistas = GetUsuarios();
            ViewBag.ListaProductos = GetListProductos().Select(x => new { id = x.Value, text = x.Text });
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Inventario_Movimiento_Maestro_FacturacionBuscarPorId(fiIDInventarioMovimientoMaestro).FirstOrDefault();
                var detalleFacturaDb = contexto.sp_Facturacion_Detalle_ListarPorIDFacturacionMaestro(modelDb.fiIDFacturacionMaestro).ToList();
                var modelDbDetalle = contexto.sp_Inventario_Movimiento_Detalle_GetIDInventarioMovimientoMaestro(fiIDInventarioMovimientoMaestro).ToList();
                ViewBag.ProductosSinSerie = GetListProductosConsumibles();
                var model = new CrearFacturacionInventarioViewModel();
                model.fbEditar = true;
                model.fcFactura = modelDb.fcReferenciaDocumento;
                model.fcIdentidad = modelDb.fcIdentidad;
                model.fcNombreCliente = modelDb.fcNombreCliente;
                model.fcTelefonoCliente = modelDb.fcTelefonoCliente;
                model.fcCorreoCliente = modelDb.fcCorreoCliente;
                model.fiIDUbicacionDestino = modelDb.fiIDUbicacionDestino ?? 0;
                model.fiIDUbicacionOrigen = modelDb.fiIDUbicacion;
                model.DetalleFactura = new List<ListFacturacionDetalleViewModel>();
                model.fiIDInventarioMovimientoMaestro = modelDb.fiIDInventarioMovimientoMaestro;

                foreach (var item in detalleFacturaDb)
                {
                    model.DetalleFactura.Add(new ListFacturacionDetalleViewModel
                    {
                        fbAplicaImpuesto = item.fbAplicaImpuesto ?? false,
                        fbSeleccionado = true,
                        fcProducto = item.fcProducto,
                        fcToken = item.fcToken,
                        fiIDFacturacionDetalle = item.fiIDFacturacionDetalle,
                        fiIDFacturacionMaestro = item.fiIDFacturacionMaestro ?? 0,
                        fiIDProducto = item.fiIDProducto ?? 0,
                        fnCantidad = item.fnCantidad ?? 0,
                        fnExistencia = 0,
                        fnPorcentajeImpuesto1 = (item.fnPorcentajeImpuesto1 ?? 0) / 100,
                        fnPorcentajeImpuesto2 = item.fnPorcentajeImpuesto2 ?? 0,
                        fnSubTotal = (item.fnValorVentaDeContado ?? 0) * (item.fnCantidad ?? 0),
                        fnTotal = item.fnTotal ?? 0,
                        fnValorImpuesto = item.fnValorImpuesto ?? 0,
                        fnValorProductoME = item.fnValorProductoME ?? 0,
                        fnValorVentaDeContado = item.fnValorVentaDeContado ?? 0,
                        Foto = ""
                    });
                }

                if (modelDbDetalle.Count > 0)
                {
                    model.ListaInventarioDetalle = new List<ListInventarioMovimientoDetalleViewModel>();
                    foreach (var itemDb in modelDbDetalle)
                    {
                        var item = new ListInventarioMovimientoDetalleViewModel
                        {
                            fiIDInventarioMovimientoDetalle = itemDb.fiIDInventarioMovimientoDetalle,
                            fiIDInventarioMovimientoMaestro = itemDb.fiIDInventarioMovimientoMaestro,
                            fiIDTipoMovimiento = itemDb.fiIDTipoMovimiento,
                            fiIDMovimiento = itemDb.fiIDMovimiento,
                            fiIDUbicacionInicial = itemDb.fiIDUbicacionInicial,
                            fiIDUbicacionDestino = itemDb.fiIDUbicacionDestino,
                            fiIdProducto = itemDb.fiIDProducto ?? 0,
                            fcProducto = itemDb.fcProducto,
                            fcCodigoSerie1 = itemDb.fcCodigoSerie1,
                            fcCodigoSerie2 = itemDb.fcCodigoSerie2,
                            fnCantidad = itemDb.fnCantidad,
                            fcDescripcion = itemDb.fcDescripcion,
                            fbPorCodigo = itemDb.fcCodigoSerie1.Trim() == "" ? false : true,
                            fcToken = itemDb.fcToken
                        };
                        model.ListaInventarioDetalle.Add(item);
                    }
                }


                return View(model);
            }
        }

        [HttpPost]
        public ActionResult EditarFacturacionInventario(CrearFacturacionInventarioViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {

                    var modelDb = contexto.sp_Inventario_Movimiento_Maestro_GetById(model.fiIDInventarioMovimientoMaestro).FirstOrDefault();
                    var modelDbDetalle = contexto.sp_Inventario_Movimiento_Detalle_GetIDInventarioMovimientoMaestro(model.fiIDInventarioMovimientoMaestro).ToList();


                    var facturaSalida = model.ListaInventarioDetalle.GroupBy(x => x.fiIdProducto).Select(x => new ListFacturacionDetalleViewModel
                    {
                        fiIDProducto = x.Key,
                        fnCantidad = x.Sum(y => y.fnCantidad)
                    }).ToList();
                    foreach (var item in model.DetalleFactura)
                    {
                        var itemSalida = facturaSalida.FirstOrDefault(x => x.fiIDProducto == item.fiIDProducto);
                        if (itemSalida == null)
                        {
                            return EnviarResultado(false, "", $"Falto colocar este producto: {item.fcProducto}");
                        }


                        if (itemSalida.fnCantidad < item.fnCantidad)
                        {
                            var cantidad = item.fnCantidad - itemSalida.fnCantidad;
                            return EnviarResultado(false, "", $"Faltaron {cantidad:0.##} unidades de este producto {item.fcProducto}");
                        }

                        if (itemSalida.fnCantidad > item.fnCantidad)
                        {
                            var cantidad = itemSalida.fnCantidad - item.fnCantidad;
                            return EnviarResultado(false, "", $"Salida con {cantidad:0.##} unidades de mas de este producto {item.fcProducto}");
                        }
                    }


                    var result = contexto.sp_Inventario_Movimiento_Maestro_Editar(
                        model.fiIDInventarioMovimientoMaestro,
                        model.fiIDUbicacionOrigen,
                        20,
                        modelDb.fiIDTipoMovimiento,
                        5,
                        1, model.fiIDUbicacionDestino) > 0;

                    if (result)
                    {
                        foreach (var item in modelDbDetalle)
                        {
                            if (model.ListaInventarioDetalle?.Any(x => x.fcToken == item.fcToken) ?? false)
                            {
                                var itemModel = model.ListaInventarioDetalle.FirstOrDefault(x => x.fcToken == item.fcToken);
                                if (itemModel.fiIDMovimiento == 0)
                                {
                                    itemModel.fiIDMovimiento = contexto.sp_Productos_Maestro_GetIdMovimiento(itemModel.fiIdProducto, item.fiIDUbicacionInicial)?.FirstOrDefault()?.fiIDMovimiento ?? 0;
                                }
                                model.ListaInventarioDetalle.FirstOrDefault(x => x.fcToken == item.fcToken).fbEditado = true;
                                contexto.sp_Inventario_Movimiento_Detalle_Editar(
                                    itemModel.fiIDInventarioMovimientoDetalle,
                                    itemModel.fiIDInventarioMovimientoMaestro,
                                    5,
                                    itemModel.fiIDMovimiento,
                                    itemModel.fiIDUbicacionInicial == 0 ? model.fiIDUbicacionOrigen : itemModel.fiIDUbicacionInicial,
                                    itemModel.fiIDUbicacionDestino,
                                    itemModel.fcCodigoSerie1 ?? "",
                                    itemModel.fcCodigoSerie2 ?? "",
                                    itemModel.fnCantidad,
                                    itemModel.fcDescripcion ?? "",
                                    GetIdUser()
                                );
                            }
                            else
                            {
                                contexto.sp_Inventario_Movimiento_Detalle_Eliminar(item.fiIDInventarioMovimientoDetalle);
                            }
                        }

                        if (model.ListaInventarioDetalle != null)
                        {

                            foreach (var inv in model.ListaInventarioDetalle.Where(x => x.fbEditado == false).ToList())
                            {
                                if (inv.fbPorCodigo == false)
                                {
                                    inv.fiIDMovimiento = contexto.sp_Productos_Maestro_GetIdMovimiento(inv.fiIdProducto, model.fiIDUbicacionOrigen)?.FirstOrDefault()?.fiIDMovimiento ?? 0;
                                }
                                var piIDInventarioMovimientoDetalle = new ObjectParameter("piIDInventarioMovimientoDetalle", typeof(int));
                                contexto.sp_Inventario_Movimiento_Detalle_Crear(
                                model.fiIDInventarioMovimientoMaestro,
                                5,
                                inv.fiIDMovimiento,
                                inv.fiIDUbicacionInicial == 0 ? model.fiIDUbicacionOrigen : inv.fiIDUbicacionInicial,
                                inv.fiIDUbicacionDestino,
                                inv.fcCodigoSerie1 ?? "",
                                inv.fcCodigoSerie2 ?? "",
                                inv.fnCantidad,
                                inv.fcDescripcion ?? "",
                                GetIdUser(),
                                piIDInventarioMovimientoDetalle);



                            }
                        }
                    }

                    return EnviarResultado(result, "", result ? "Salida de Inventario editada exitosamente" : "Error de red");


                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }

        [HttpPost]
        public JsonResult ConfirmarFacturacionInventario(CrearSalidaInventarioViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var archivo = model.Firma;
                    var modelDb = contexto.sp_Inventario_Movimiento_Maestro_GetById(model.fiIDInventarioMovimientoMaestro).FirstOrDefault();
                    var pcIDFactura = new ObjectParameter("pcIDFactura", typeof(string));

                    var result = false;
                    if (modelDb.fiFirma == 0)
                    {
                        result = contexto.sp_Inventario_Movimiento_Maestro_ConfirmarSalidaInventario(model.fiIDInventarioMovimientoMaestro, model.Firma.FileName, GetIdUser()) > 0;

                        contexto.sp_Inventario_ActualizarFacturacion(model.fiIDInventarioMovimientoMaestro, pcIDFactura);
                        contexto.sp_Facturacion_ActualizarSinopsis(model.fiIDInventarioMovimientoMaestro);
                    }
                    else
                    {
                        return EnviarResultado(false, "Error", "Inventario ya fue firmado");
                    }


                    if (result)
                    {
                        if (model.fiIDTipoMomvimento == 0)
                        {
                            model.fdFechaCreacion = modelDb.fdFechaCreacion;
                            model.fiIDTipoMomvimento = modelDb.fiIDTipoMovimiento;
                        }


                        var algo = GetPDFSalidaInventario(model);
                        UploadFileServer148(@"inventario\firmas", archivo);
                        var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
                        hubContext.Clients.All.actualizarSalidaInventario(model.UsuarioPeticion);

                        var ubicaciones = ListarUbicacionesPorTipo();
                        var ubicacionInicial = ubicaciones.FirstOrDefault(x => x.fiIDUbicacion == modelDb.fiIDUbicacion);
                        var ubicacionDestino = ubicaciones.FirstOrDefault(x => x.fiIDUbicacion == modelDb.fiIDUbicacionDestino);
                        var stringSalida = "";
                        if (ubicacionDestino.fcTipoUbicacion == "BODEGA") stringSalida = $"a la bodega {ubicacionDestino.fcUbicacion}";
                        if (ubicacionDestino.fcTipoUbicacion == "USUARIO") stringSalida = $"al usuario {ubicacionDestino.fcNombreCorto} de la empresa {ubicacionDestino.fcNombreComercial}";
                        if (ubicacionDestino.fcTipoUbicacion == "CLIENTE") stringSalida = $"al cliente {ubicacionDestino.fcNombreCorto}";
                        EnviarNotificacion($"Salida de inventario N#{model.fiIDInventarioMovimientoMaestro} desde {ubicacionInicial.fcUbicacion} {stringSalida} | PRESTADITO");
                        MensajesWhatsappFacturacionInventario(Convert.ToInt32(pcIDFactura.Value));
                        return EnviarResultado(result, "", "Salida exitosa");
                    }
                    return EnviarResultado(result, "", "Error de red");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }

        [AllowAnonymous]
        public HttpPostedFileBase GenerarFacturacionVenta(int fiIDFacturacionMaestro, string fileName)
        {
            var DocumentoInventarioService = new ReportesTemplateService();
            var DocumentoInventario = new MemoryStream();
            var archivo = DocumentoInventarioService.GenerarFacturaDeVenta(fiIDFacturacionMaestro);
            archivo.CopyTo(DocumentoInventario);
            return new MemoryPostedFile(DocumentoInventario.ToArray(), fileName);
        }

        [HttpGet]
        public JsonResult MensajesWhatsappFacturacionInventario(int fiIDFacturacionMaestro)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var modelDb = contexto.sp_Facturacion_Maestro_ObtenerPorIdFacturacion(fiIDFacturacionMaestro).FirstOrDefault();
                    var nombreArchivo = $"Factura_{modelDb.fcReferenciaDocumento}.pdf";
                    var archivo = GenerarFacturacionVenta(fiIDFacturacionMaestro, nombreArchivo);
                    var nombre = "https://orion.novanetgroup.com/Documento/wa/" + archivo.FileName;
                    UploadFileServer148(@"Documento\WA", archivo);

                    if (modelDb.fcTelefonoCliente.Length >= 8)
                    {
                        MensajeriaApi.MensajesDigitales(modelDb.fcTelefonoCliente, $"Factura #{modelDb.fcNombreCliente} del cliente {modelDb.fcNombreCliente} con identidad {modelDb.fcIdentidad}", $@"https://orion.novanetgroup.com/Documento/WA/{nombreArchivo}");
                    }
                    return EnviarResultado(true, "Whatsapp", "Mensaje Enviado");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }

        #endregion

        #region Prestadito
        public ActionResult IndexFacturacionPrestadito()
        {
            return View();
        }


        public ActionResult CargarListaFacturacionInventarioPrestadito()
        {
            using (var contexto = new ORIONDBEntities())
            {
                var lista = contexto.sp_Inventario_Movimiento_Maestro_ListarFacturacion_Prestadito_Articulo().ToList();
                return EnviarListaJson(lista);
            }
        }

        [HttpGet]
        public ActionResult EditarFacturacionInventarioPrestadito(int fiIDInventarioMovimientoMaestro)
        {
            ViewBag.ListUbicaciones = GetListUbicaciones();
            ViewBag.ListTecnicosContratistas = GetUsuarios();
            ViewBag.ListaProductos = GetListProductos().Select(x => new { id = x.Value, text = x.Text });
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_InventarioP_Movimiento_Maestro_Prestadito_Articulo_PorId(fiIDInventarioMovimientoMaestro).FirstOrDefault();
                var detalleFacturaDb = contexto.sp_Producto_Maestro_BuscarPorCodigo(modelDb.fcModelo).ToList();
                var modelDbDetalle = contexto.sp_Inventario_Movimiento_Detalle_GetIDInventarioMovimientoMaestro(fiIDInventarioMovimientoMaestro).ToList();
                ViewBag.ProductosSinSerie = GetListProductosConsumibles();
                var model = new CrearFacturacionInventarioViewModel();
                model.fbEditar = true;
                model.fcIDPrestamo = modelDb.fcIDPrestamo;
                model.fcIdentidad = modelDb.fcIdentidadCliente;
                model.fcNombreCliente = modelDb.NombreCliente;
                model.fcTelefonoCliente = "";
                model.fcCorreoCliente = modelDb.fcCorreoElectronicoCliente;
                model.fiIDUbicacionDestino = modelDb.fiIDUbicacionDestino ?? 0;
                model.fiIDUbicacionOrigen = modelDb.fiIDUbicacion;
                model.DetalleFactura = new List<ListFacturacionDetalleViewModel>();
                model.fiIDInventarioMovimientoMaestro = modelDb.fiIDInventarioMovimientoMaestro;

                foreach (var item in detalleFacturaDb)
                {
                    model.DetalleFactura.Add(new ListFacturacionDetalleViewModel
                    {
                        //fbAplicaImpuesto = item.fbAplicaImpuesto ?? false,
                        fbSeleccionado = true,
                        fcProducto = item.fcProducto,
                        fcToken = item.fcToken,
                        fnValorVentaDeContado = item.fnValorVentaDeContado ?? 0,
                        Foto = "",
                        fnCantidad = 1,
                        fiIDProducto = item.fiIDProducto,
                    });
                }

                if (modelDbDetalle.Count > 0)
                {
                    model.ListaInventarioDetalle = new List<ListInventarioMovimientoDetalleViewModel>();
                    foreach (var itemDb in modelDbDetalle)
                    {
                        var item = new ListInventarioMovimientoDetalleViewModel
                        {
                            fiIDInventarioMovimientoDetalle = itemDb.fiIDInventarioMovimientoDetalle,
                            fiIDInventarioMovimientoMaestro = itemDb.fiIDInventarioMovimientoMaestro,
                            fiIDTipoMovimiento = itemDb.fiIDTipoMovimiento,
                            fiIDMovimiento = itemDb.fiIDMovimiento,
                            fiIDUbicacionInicial = itemDb.fiIDUbicacionInicial,
                            fiIDUbicacionDestino = itemDb.fiIDUbicacionDestino,
                            fiIdProducto = itemDb.fiIDProducto ?? 0,
                            fcProducto = itemDb.fcProducto,
                            fcCodigoSerie1 = itemDb.fcCodigoSerie1,
                            fcCodigoSerie2 = itemDb.fcCodigoSerie2,
                            fnCantidad = itemDb.fnCantidad,
                            fcDescripcion = itemDb.fcDescripcion,
                            fbPorCodigo = itemDb.fcCodigoSerie1.Trim() == "" ? false : true,
                            fcToken = itemDb.fcToken
                        };
                        model.ListaInventarioDetalle.Add(item);
                    }
                }


                return View(model);
            }
        }

        [HttpPost]
        public ActionResult EditarFacturacionInventarioPrestadito(CrearFacturacionInventarioViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {

                    var modelDb = contexto.sp_Inventario_Movimiento_Maestro_GetById(model.fiIDInventarioMovimientoMaestro).FirstOrDefault();
                    var modelDbDetalle = contexto.sp_Inventario_Movimiento_Detalle_GetIDInventarioMovimientoMaestro(model.fiIDInventarioMovimientoMaestro).ToList();


                    var facturaSalida = model.ListaInventarioDetalle.GroupBy(x => x.fiIdProducto).Select(x => new ListFacturacionDetalleViewModel
                    {
                        fiIDProducto = x.Key,
                        fnCantidad = x.Sum(y => y.fnCantidad)
                    }).ToList();
                    foreach (var item in model.DetalleFactura)
                    {
                        var itemSalida = facturaSalida.FirstOrDefault(x => x.fiIDProducto == item.fiIDProducto);
                        if (itemSalida == null)
                        {
                            return EnviarResultado(false, "", $"Falto colocar este producto: {item.fcProducto}");
                        }


                        if (itemSalida.fnCantidad < item.fnCantidad)
                        {
                            var cantidad = item.fnCantidad - itemSalida.fnCantidad;
                            return EnviarResultado(false, "", $"Faltaron {cantidad:0.##} unidades de este producto {item.fcProducto}");
                        }

                        if (itemSalida.fnCantidad > item.fnCantidad)
                        {
                            var cantidad = itemSalida.fnCantidad - item.fnCantidad;
                            return EnviarResultado(false, "", $"Salida con {cantidad:0.##} unidades de mas de este producto {item.fcProducto}");
                        }
                    }


                    var result = contexto.sp_Inventario_Movimiento_Maestro_Editar(
                        model.fiIDInventarioMovimientoMaestro,
                        model.fiIDUbicacionOrigen,
                        20,
                        modelDb.fiIDTipoMovimiento,
                        6,
                        1, model.fiIDUbicacionDestino) > 0;

                    if (result)
                    {
                        foreach (var item in modelDbDetalle)
                        {
                            if (model.ListaInventarioDetalle?.Any(x => x.fcToken == item.fcToken) ?? false)
                            {
                                var itemModel = model.ListaInventarioDetalle.FirstOrDefault(x => x.fcToken == item.fcToken);
                                if (itemModel.fiIDMovimiento == 0)
                                {
                                    itemModel.fiIDMovimiento = contexto.sp_Productos_Maestro_GetIdMovimiento(itemModel.fiIdProducto, item.fiIDUbicacionInicial)?.FirstOrDefault()?.fiIDMovimiento ?? 0;
                                }
                                model.ListaInventarioDetalle.FirstOrDefault(x => x.fcToken == item.fcToken).fbEditado = true;
                                contexto.sp_Inventario_Movimiento_Detalle_Editar(
                                    itemModel.fiIDInventarioMovimientoDetalle,
                                    itemModel.fiIDInventarioMovimientoMaestro,
                                    6,
                                    itemModel.fiIDMovimiento,
                                    itemModel.fiIDUbicacionInicial == 0 ? model.fiIDUbicacionOrigen : itemModel.fiIDUbicacionInicial,
                                    itemModel.fiIDUbicacionDestino,
                                    itemModel.fcCodigoSerie1 ?? "",
                                    itemModel.fcCodigoSerie2 ?? "",
                                    itemModel.fnCantidad,
                                    itemModel.fcDescripcion ?? "",
                                    GetIdUser()
                                );
                            }
                            else
                            {
                                contexto.sp_Inventario_Movimiento_Detalle_Eliminar(item.fiIDInventarioMovimientoDetalle);
                            }
                        }

                        if (model.ListaInventarioDetalle != null)
                        {

                            foreach (var inv in model.ListaInventarioDetalle.Where(x => x.fbEditado == false).ToList())
                            {
                                if (inv.fbPorCodigo == false)
                                {
                                    inv.fiIDMovimiento = contexto.sp_Productos_Maestro_GetIdMovimiento(inv.fiIdProducto, model.fiIDUbicacionOrigen)?.FirstOrDefault()?.fiIDMovimiento ?? 0;
                                }
                                var piIDInventarioMovimientoDetalle = new ObjectParameter("piIDInventarioMovimientoDetalle", typeof(int));
                                contexto.sp_Inventario_Movimiento_Detalle_Crear(
                                model.fiIDInventarioMovimientoMaestro,
                                6,
                                inv.fiIDMovimiento,
                                inv.fiIDUbicacionInicial == 0 ? model.fiIDUbicacionOrigen : inv.fiIDUbicacionInicial,
                                inv.fiIDUbicacionDestino,
                                inv.fcCodigoSerie1 ?? "",
                                inv.fcCodigoSerie2 ?? "",
                                inv.fnCantidad,
                                inv.fcDescripcion ?? "",
                                GetIdUser(),
                                piIDInventarioMovimientoDetalle);



                            }
                        }
                    }

                    return EnviarResultado(result, "", result ? "Salida de Inventario editada exitosamente" : "Error de red");


                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }
        [HttpPost]
        public JsonResult ConfirmarFacturacionInventarioPrestadito(CrearSalidaInventarioViewModel model)
        {
            using (var CoreFinanciero = new CoreFinancieroEntities2())
            {
                using (var contexto = new ORIONDBEntities())
                {
                    try
                    {
                        CoreFinanciero.Database.CommandTimeout = int.MaxValue;
                        contexto.Database.CommandTimeout = int.MaxValue;
                        var modelPrestadito = contexto.sp_FacturacionPrestadito_BuscarPorInventario(model.fiIDInventarioMovimientoMaestro).FirstOrDefault();

                        var archivo = model.Firma;
                        var modelDb = contexto.sp_Inventario_Movimiento_Maestro_GetById(model.fiIDInventarioMovimientoMaestro).FirstOrDefault();


                        var result = false;
                        if (modelDb.fiFirma == 0)
                        {
                            result = contexto.sp_Inventario_Movimiento_Maestro_ConfirmarSalidaInventario(model.fiIDInventarioMovimientoMaestro, model.Firma.FileName, GetIdUser()) > 0;
                            CoreFinanciero.sp_CREDGarantias_ActualizarSerieDeProducto(modelPrestadito.fiIDSolicitud, modelPrestadito.fcIDPrestamo, "");
                            contexto.sp_TrasladoNovanetPrestadito_Contabilizar(model.fiIDInventarioMovimientoMaestro, GetIdUser());
                        }
                        else
                        {
                            return EnviarResultado(false, "Error", "Inventario ya fue firmado");
                        }


                        if (result)
                        {
                            if (model.fiIDTipoMomvimento == 0)
                            {
                                model.fdFechaCreacion = modelDb.fdFechaCreacion;
                                model.fiIDTipoMomvimento = modelDb.fiIDTipoMovimiento;
                            }


                            var algo = GetPDFSalidaInventario(model);
                            UploadFileServer148(@"inventario\firmas", archivo);
                            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
                            hubContext.Clients.All.actualizarSalidaInventario(model.UsuarioPeticion);

                            var ubicaciones = ListarUbicacionesPorTipo();
                            var ubicacionInicial = ubicaciones.FirstOrDefault(x => x.fiIDUbicacion == modelDb.fiIDUbicacion);
                            var ubicacionDestino = ubicaciones.FirstOrDefault(x => x.fiIDUbicacion == modelDb.fiIDUbicacionDestino);
                            var stringSalida = "";
                            if (ubicacionDestino.fcTipoUbicacion == "BODEGA") stringSalida = $"a la bodega {ubicacionDestino.fcUbicacion}";
                            if (ubicacionDestino.fcTipoUbicacion == "USUARIO") stringSalida = $"al usuario {ubicacionDestino.fcNombreCorto} de la empresa {ubicacionDestino.fcNombreComercial}";
                            if (ubicacionDestino.fcTipoUbicacion == "CLIENTE") stringSalida = $"al cliente {ubicacionDestino.fcNombreCorto}";


                            EnviarNotificacion($"Salida de inventario N#{model.fiIDInventarioMovimientoMaestro} desde {ubicacionInicial.fcUbicacion} {stringSalida} | VENTA");
                            //MensajesWhatsappFacturacionInventario(Convert.ToInt32(pcIDFactura.Value));
                            return EnviarResultado(result, "", "Salida exitosa");
                        }
                        return EnviarResultado(result, "", "Error de red");
                    }
                    catch (Exception ex)
                    {
                        return EnviarException(ex, "Error");
                    }
                }
            }

        }

        #endregion


        #region AjusteDeInventario
        public ActionResult IndexProductosMovimientoDetalle()
        {
            return View();
        }

        public JsonResult CargarListaProductoMovimientoDetalle()
        {

            using (var contexto = new ORIONDBEntities())
            {

                var consumibles = GetListProductosConsumibles();
                var list = contexto.sp_Producto_Movimiento_Detalle_Listar().ToList();
                var listProductosConsumibles = contexto.sp_Producto_Maestro_ListarConsumibles().ToList();
                var listModel = new List<ListMovimientoDetalleViewModel>();
                var ubicaciones = ListarUbicacionesPorTipo();
                foreach (var item in list)
                {
                    var modelUbicacion = ubicaciones.FirstOrDefault(x => x.fiIDUbicacion == item.fiIDUbicacion);
                    var stringUbicacion = "";
                    if (modelUbicacion.fcTipoUbicacion == "BODEGA") stringUbicacion = modelUbicacion.fcUbicacion;
                    if (modelUbicacion.fcTipoUbicacion == "USUARIO") stringUbicacion = $"USUARIO: {modelUbicacion.fcNombreCorto} | {modelUbicacion.fcNombreComercial}";
                    if (modelUbicacion.fcTipoUbicacion == "CLIENTE") stringUbicacion = $"CLIENTE: {modelUbicacion.fcUbicacion} | {modelUbicacion.fcNombreCorto}";
                    listModel.Add(new ListMovimientoDetalleViewModel
                    {

                        fiIDMovimiento = item.fiIDMovimiento,
                        fiIDProducto = item.fiIDProducto,
                        fcProducto = item.fcProducto,
                        fiIDUbicacion = item.fiIDUbicacion,
                        fcUbicacion = stringUbicacion,
                        fcCodigoSerie1 = item.fcCodigoSerie1,
                        fcCodigoSerie2 = item.fcCodigoSerie2,
                        fcReferenciaMovimiento = item.fcReferenciaMovimiento,
                        fdFechaMovimiento = item.fdFechaMovimiento,
                        fiIDMovimientoMaestro = item.fiIDMovimientoMaestro ?? 0,
                        fnCantidad = item.fnCantidad,
                        fcTipoProducto = consumibles.Any(x => x == item.fiIDProducto) ? "Consumible" : "Activo",
                        fbConsumible = consumibles.Any(x => x == item.fiIDProducto),
                        fbFechaMovimiento = true,
                        fcNumeroFactura = item.fcNumeroFactura
                    });
                }

                listModel = listModel.Where(x => x.fcTipoProducto == "Activo").ToList();

                foreach (var item in listProductosConsumibles)
                {
                    listModel.Add(new ListMovimientoDetalleViewModel
                    {
                        fiIDProducto = item.fiIDProducto,
                        fcProducto = item.fcProducto,
                        fcCodigoSerie1 = item.fcCodigodeBarras,
                        fcTipoProducto = consumibles.Any(x => x == item.fiIDProducto) ? "Consumible" : "Activo",
                        fbConsumible = consumibles.Any(x => x == item.fiIDProducto),
                        fbFechaMovimiento = false
                    });
                }

                return EnviarListaJson(listModel);
            }


        }

        [HttpGet]
        public ActionResult EditarProductoMovimientoDetalle(int fiIDMovimiento)
        {

            ViewBag.Ubicaciones = GetListUbicaciones().ToList();
            
            using (var contexto = new ORIONDBEntities())
            {
                var consumibles = GetListProductosConsumibles();
                var modelDb = contexto.sp_Producto_Movimiento_Detalle_GetById(fiIDMovimiento).FirstOrDefault();
                var model = new ListMovimientoDetalleViewModel
                {
                    fiIDMovimiento = modelDb.fiIDMovimiento,
                    fiIDProducto = modelDb.fiIDProducto,
                    fcProducto = modelDb.fcProducto,
                    fiIDUbicacion = modelDb.fiIDUbicacion,
                    fcUbicacion = modelDb.fcUbicacion,
                    fcCodigoSerie1 = modelDb.fcCodigoSerie1,
                    fcCodigoSerie2 = modelDb.fcCodigoSerie2,
                    fcReferenciaMovimiento = modelDb.fcReferenciaMovimiento,
                    fdFechaMovimiento = modelDb.fdFechaMovimiento,
                    fiIDMovimientoMaestro = modelDb.fiIDMovimientoMaestro ?? 0,
                    fnCantidad = modelDb.fnCantidad,
                    fcTipoProducto = consumibles.Any(x => x == modelDb.fiIDProducto) ? "Consumible" : "Activo",
                    fbConsumible = consumibles.Any(x => x == modelDb.fiIDProducto),
                    fiIdProductoPreciosDetalleActual = modelDb.fiIdProductoPreciosDetalle ?? 0,
                    fiIDSolicitud = modelDb.fiIDSolicitud
                };
                ViewBag.Solicitudes = GetSolicitudesPorUbicacionParaSelect(modelDb.fiIDUbicacion);
                return PartialView(model);
            }
        }

        

        [HttpGet]
        public JsonResult GetSolicitudesPorUbicacion(int fiIDUbicacion)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var lista = contexto.sp_SolicitudesPorUbicacionCliente(fiIDUbicacion).Select(x=> new { id = x.fiIDSolicitud, text = x.fiIDSolicitud }).ToList();
                return EnviarListaJson(lista);
            }
        }


        [HttpGet]
        public ActionResult EditarConsumible(int fiIDProducto)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var consumibles = GetListProductosConsumibles();
                var modelDb = contexto.sp_Producto_Maestro_GetById(fiIDProducto).FirstOrDefault();
                ViewBag.Ubicaciones = GetListUbicaciones().ToList();
                
                var model = new ListMovimientoDetalleViewModel
                {
                    fiIDProducto = modelDb.fiIDProducto,
                    fcProducto = modelDb.fcProducto,
                    fcTipoProducto = consumibles.Any(x => x == modelDb.fiIDProducto) ? "Consumible" : "Activo",
                    fbConsumible = consumibles.Any(x => x == modelDb.fiIDProducto),
                };
                return PartialView(model);
            }
        }


        public JsonResult GetCantidadConsumible(int fiIDProducto, int fiIDUbicacion, int fiIDSolicitud)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Productos_Inventario_BuscarPorIdProductoYUbicacion(fiIDProducto, fiIDUbicacion, fiIDSolicitud).FirstOrDefault();
                return Json(modelDb?.fnCantidad ?? 0, JsonRequestBehavior.AllowGet);
            }
        }




        [HttpPost]
        public JsonResult EditarProductoMovimientoDetalle(ListMovimientoDetalleViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var modelOriginal = contexto.sp_Producto_Movimiento_Detalle_GetById(model.fiIDMovimiento).FirstOrDefault();
                    var result = contexto.sp_Producto_Movimiento_Detalle_Editar(model.fiIDMovimiento, model.fiIDProducto, model.fiIDUbicacion, 1, model.fcCodigoSerie1, model.fcCodigoSerie2, model.fcReferenciaMovimiento, model.fnCantidad, model.fiIdProductoPreciosDetalleActual, model.fiIDSolicitud);
                    var modelDb = contexto.sp_Producto_Movimiento_Detalle_GetById(model.fiIDMovimiento).FirstOrDefault();
                    /////////////////////
                    try 
                    {
                        var idMaestro = Convert.ToInt32(contexto.sp_Inventario_Movimiento_Maestro_Crear(modelOriginal.fiIDUbicacion, 0, GetIdUser(), 7, model.fiIDUbicacion)?.FirstOrDefault() ?? 0);
                        var piIDInventarioMovimientoDetalle = new ObjectParameter("piIDInventarioMovimientoDetalle", typeof(int));
                        contexto.sp_Inventario_Movimiento_Detalle_Crear(idMaestro, 7, model.fiIDMovimiento, modelOriginal.fiIDUbicacion, model.fiIDUbicacion, model.fcCodigoSerie1, model.fcCodigoSerie2, model.fnCantidad, "MOVIMIENTO EN AJUSTE", GetIdUser(), piIDInventarioMovimientoDetalle);
                    }
                    catch(Exception ex)
                    {

                    }
                    
                    ////////////////////////

                    var consumibles = GetListProductosConsumibles();
                    var ubicaciones = ListarUbicacionesPorTipo();
                    var modelUbicacion = ubicaciones.FirstOrDefault(x => x.fiIDUbicacion == model.fiIDUbicacion);
                    var modelUbicacionInicial = ubicaciones.FirstOrDefault(x => x.fiIDUbicacion == modelOriginal.fiIDUbicacion);
                    var stringUbicacionInicial = "";
                    if (modelUbicacionInicial.fcTipoUbicacion == "BODEGA") stringUbicacionInicial = modelUbicacionInicial.fcUbicacion;
                    if (modelUbicacionInicial.fcTipoUbicacion == "USUARIO") stringUbicacionInicial = $"USUARIO: { modelUbicacionInicial.fcNombreCorto } { modelUbicacionInicial.fcNombreComercial }";
                    if (modelUbicacionInicial.fcTipoUbicacion == "CLIENTE") stringUbicacionInicial = $"CLIENTE: { modelUbicacionInicial.fcUbicacion } | { modelUbicacionInicial.fcNombreCorto }";

                    var stringUbicacion = "";
                    if (modelUbicacion.fcTipoUbicacion == "BODEGA") stringUbicacion = modelUbicacion.fcUbicacion;
                    if (modelUbicacion.fcTipoUbicacion == "USUARIO") stringUbicacion = $"USUARIO: { modelUbicacion.fcNombreCorto } { modelUbicacion.fcNombreComercial }";
                    if (modelUbicacion.fcTipoUbicacion == "CLIENTE") stringUbicacion = $"CLIENTE: { modelUbicacion.fcUbicacion } | { modelUbicacion.fcNombreCorto }";

                    var modelActualizado = new ListMovimientoDetalleViewModel
                    {
                        fiIDMovimiento = modelDb.fiIDMovimiento,
                        fiIDProducto = modelDb.fiIDProducto,
                        fcProducto = modelDb.fcProducto,
                        fiIDUbicacion = modelDb.fiIDUbicacion,
                        fcUbicacion = stringUbicacion,
                        fcCodigoSerie1 = modelDb.fcCodigoSerie1,
                        fcCodigoSerie2 = modelDb.fcCodigoSerie2,
                        fcReferenciaMovimiento = modelDb.fcReferenciaMovimiento,
                        fdFechaMovimiento = modelDb.fdFechaMovimiento,
                        fiIDMovimientoMaestro = modelDb.fiIDMovimientoMaestro ?? 0,
                        fnCantidad = modelDb.fnCantidad,
                        fcTipoProducto = consumibles.Any(x => x == modelDb.fiIDProducto) ? "Consumible" : "Activo",
                        fbConsumible = consumibles.Any(x => x == modelDb.fiIDProducto),
                        fiIdProductoPreciosDetalleActual = modelDb.fiIdProductoPreciosDetalle ?? 0
                    };

                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificacionesHub>();
                    hubContext.Clients.All.actualizarRowAjusteProducto(modelActualizado);

                    EscribirEnLog($"SE MOVIO EL PRODUCTO { model.fcProducto } CON SERIE1: { model.fcCodigoSerie1 } {(string.IsNullOrEmpty(model.fcCodigoSerie2) ? "" : "Y/O SERIE2/MAC: " + model.fcCodigoSerie2)} DE LA BODEGA { stringUbicacionInicial }  A LA BODEGA { stringUbicacion }");

                    return EnviarResultado(result > 0, "", result > 0 ? "Invenario Editado Exitosamente" : "Error al editar");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }

            }
        }

        [HttpPost]
        public JsonResult EditarConsumible(AjusteDeConsumibleViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var result = contexto.sp_Productos_Inventario_AjusteConsumible(model.fiIDProducto, model.fiIDUbicacionIncial, model.fiIDUbicacionDestino, model.fnCantidad, GetIdUser(), model.fiIDSolicitudInicial, model.fiIDSolicitudDestino) > 0;

                    var ubicaciones = ListarUbicacionesPorTipo();
                    var modelUbicacion = ubicaciones.FirstOrDefault(x => x.fiIDUbicacion == model.fiIDUbicacionDestino);
                    var modelUbicacionInicial = ubicaciones.FirstOrDefault(x => x.fiIDUbicacion == model.fiIDUbicacionIncial);


                    var producto = contexto.sp_Producto_Maestro_GetById(model.fiIDProducto).FirstOrDefault();

                    var stringUbicacionInicial = "";
                    if (modelUbicacionInicial.fcTipoUbicacion == "BODEGA") stringUbicacionInicial = modelUbicacionInicial.fcUbicacion;
                    if (modelUbicacionInicial.fcTipoUbicacion == "USUARIO") stringUbicacionInicial = $"USUARIO: { modelUbicacionInicial.fcNombreCorto } { modelUbicacionInicial.fcNombreComercial }";
                    if (modelUbicacionInicial.fcTipoUbicacion == "CLIENTE") stringUbicacionInicial = $"CLIENTE: { modelUbicacionInicial.fcUbicacion } | { modelUbicacionInicial.fcNombreCorto }";

                    var stringUbicacion = "";
                    if (modelUbicacion.fcTipoUbicacion == "BODEGA") stringUbicacion = modelUbicacion.fcUbicacion;
                    if (modelUbicacion.fcTipoUbicacion == "USUARIO") stringUbicacion = $"USUARIO: { modelUbicacion.fcNombreCorto } { modelUbicacion.fcNombreComercial }";
                    if (modelUbicacion.fcTipoUbicacion == "CLIENTE") stringUbicacion = $"CLIENTE: { modelUbicacion.fcUbicacion } | { modelUbicacion.fcNombreCorto }";

                    EscribirEnLog($"SE MOVIO EL PRODUCTO { producto.fcProducto } LA CANTIDAD DE { model.fnCantidad } DE LA BODEGA { stringUbicacionInicial }  A LA BODEGA { stringUbicacion }");
                    return EnviarResultado(result, "Consumible editado");
                }
                catch (Exception ex)
                {
                    return EnviarException(ex, "Error");
                }
            }
        }


        [HttpGet]
        public ActionResult HistorialProducto(int fiIDProducto, string fcCodigoSerie1)
        {
            ViewBag.Data = new { fiIDProducto, fcCodigoSerie1 };
            return PartialView();

        }

        [HttpGet]
        public ActionResult ListarHistorial(int fiIDProducto, string fcCodigoSerie1 = "", int fiIDUbicacion = 0)
        {
            using(var contexto = new ORIONDBEntities())
            {
                var result = contexto.sp_Inventario_HistorialDeMovimiento(fiIDUbicacion, fiIDProducto, fcCodigoSerie1).ToList();
                return EnviarListaJson(result);
            }
        }


        [HttpGet]
        public ActionResult HistorialProductoConsumible(int fiIDProducto)
        {

            ViewBag.ListarBodegasConStock = ListarUbicacionesPorTipo().Where(x => x.fcTipoUbicacion == "BODEGA" || x.fcTipoUbicacion == "USUARIO").Select(x => new SelectListItem { Value = x.fiIDUbicacion.ToString(), Text = $"{(x.fcTipoUbicacion == "BODEGA" ? x.fcUbicacion : "USUARIO: " + x.fcNombreCorto + " " + x.fcNombreComercial)}" }).ToList();
            ViewBag.Data = new { fiIDProducto, fcCodigoSerie1 = "", fiIDUbicacion = 1 };
            //USUARIO: {modelUbicacion.fcNombreCorto} | {modelUbicacion.fcNombreComercial}
            return PartialView();
        }
        #endregion


        #region ListaDeProductosParaSeleccionar

        public ActionResult ViewListaProductosPorBodega(int fiIDUbicacion)
        {
            return PartialView(fiIDUbicacion);
        }

        [HttpPost]
        public JsonResult ListProductosPorUbicacion(int fiIDUbicacion, List<ListInventarioMovimientoDetalleViewModel> seleccionados, int fiIDTipoMovimiento)
        {
            using(var contexto = new ORIONDBEntities())
            {

                if (seleccionados == null) 
                {
                    seleccionados = new List<ListInventarioMovimientoDetalleViewModel>();
                }
                var lista = contexto.sp_InventarioPorUbicacionDetallado(fiIDUbicacion).Where(x => x.fcCodigoSerie1 != "" && x.fcCodigoSerie1 != null).ToList();
                var listaProductos = new List<ListInventarioMovimientoDetalleViewModel>();


                foreach(var item in lista)
                {
                    var productoExistente = seleccionados.FirstOrDefault(x => x.fiIDMovimiento == item.fiIDMovimiento);
                    listaProductos.Add(new ListInventarioMovimientoDetalleViewModel
                    {
                        fiIDInventarioMovimientoDetalle = productoExistente?.fiIDInventarioMovimientoDetalle ?? 0,
                        fiIDInventarioMovimientoMaestro = productoExistente?.fiIDInventarioMovimientoMaestro ?? 0,
                        fiIDTipoMovimiento = fiIDTipoMovimiento,
                        fiIDMovimiento = item.fiIDMovimiento ?? 0,
                        fiIDUbicacionInicial = item.fiIDUbicacion,
                        fiIDUbicacionDestino = productoExistente?.fiIDUbicacionDestino ?? 0,
                        fcProducto = item.fcProducto,
                        fcCodigoSerie1 = item.fcCodigoSerie1,
                        fnCantidad = item.fnCantidad,
                        fiIdProducto = item.fiIDProducto ?? 0,
                        fcDescripcion = productoExistente?.fcDescripcion ?? "",
                        fbPorCodigo = productoExistente?.fbPorCodigo ?? true,
                        fbEditado = productoExistente?.fbEditado ?? false,
                        fcCodigoSerie2 = item.fcCodigoSerie2,
                        fcPrecios = productoExistente?.fcPrecios ?? "",
                        fcToken = productoExistente?.fcToken ?? "",
                        fcUbicacionDestino = productoExistente?.fcUbicacionDestino ?? "",
                        fcUbicacionInicial = productoExistente?.fcUbicacionInicial ?? "",
                        index = productoExistente?.index ?? 0,
                        fbCheckeado = productoExistente != null
                    });
                }

                return EnviarListaJson(listaProductos);
            }
        }

        #endregion


        #region AdministracionPropia
        [HttpPost]
        public ActionResult CrearSalidaInventarioAdministracion(CrearSalidaInventarioViewModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                try
                {
                    var productosSinSerie = contexto.sp_Configuraciones().FirstOrDefault(x => x.NombreLlave == "Productos_SinCodigoBarra");

                    var listaProductos = GetListProductosConsumibles();

                    var result = contexto.sp_Inventario_Movimiento_Maestro_Crear(
                        model.fiIdUbicacion,
                        model.fiIDUsuarioDestino,
                        GetIdUser(),
                        2, model.fiIDUbicacionDestino)?.FirstOrDefault() ?? 0;

                    if (result > 0)
                    {
                        foreach (var item in model.ListaInventarioDetalle)
                        {
                            if (item.fbPorCodigo == false)
                            {
                                item.fiIDMovimiento = contexto.sp_Productos_Maestro_GetIdMovimiento(item.fiIdProducto, item.fiIDUbicacionInicial)?.FirstOrDefault()?.fiIDMovimiento ?? 0;
                            }
                            var piIDInventarioMovimientoDetalle = new ObjectParameter("piIDInventarioMovimientoDetalle", typeof(int));
                            contexto.sp_Inventario_Movimiento_Detalle_Crear(
                                Convert.ToInt32(result),
                                2,
                                item.fiIDMovimiento,
                                item.fiIDUbicacionInicial == 0 ? model.fiIdUbicacion : item.fiIDUbicacionInicial,
                                item.fiIDUbicacionDestino,
                                item.fcCodigoSerie1 ?? "",
                                item.fcCodigoSerie2 ?? "",
                                item.fnCantidad,
                                item.fcDescripcion ?? "",
                                GetIdUser(), piIDInventarioMovimientoDetalle);
                        }
                    }

                    model.fiIDInventarioMovimientoMaestro = Convert.ToInt32(result);

                    var result2 = contexto.sp_Inventario_Movimiento_Maestro_ConfirmarSalidaInventario(model.fiIDInventarioMovimientoMaestro, "", GetIdUser()) > 0;

                    return EnviarResultado(true, "", "Salida Registrada exitosamente");
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