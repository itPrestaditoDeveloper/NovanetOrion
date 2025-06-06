using OrionCoreCableColor.DbConnection.CoreFinancieroDB;
using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models.Proveedores;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.Controllers
{
    [Authorize]
    public class ProveedoresController : BaseController
    {
        private readonly DbServiceConnection _connection;


        [HttpGet]
        public ActionResult MostrarListaDepartamentos(int codPais)
        {
            using(var contexto = new ORIONDBEntities())
            {
                var listaDepto = contexto.sp_geo_ListaDepartamentos().Select(x => new SelectListItem { Value = x.fiCodDepartamento.ToString(), Text = x.fcDepartamento }).ToList();
                return Json(listaDepto, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult MostrarListaMunicipio(int codPais, int codDepto)
        {
            using(var contexto = new ORIONDBEntities())
            {
                var listaMunicipio = contexto.sp_Geo_ListaMunicipio_ByIdDepartamento(codDepto).Select(x => new SelectListItem { Value = x.fiCodMunicipio.ToString(), Text = x.fcMunicipio }).ToList();
                return Json(listaMunicipio, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult MostrarListaAldea(int codPais, int codDepto, int codMuni)
        {
            using(var contexto = new ORIONDBEntities())
            {
                var listaMunicipio = contexto.sp_Geo_ListaPoblado_ByIdMunicipio(codDepto, codMuni).Select(x => new SelectListItem { Value = x.fiCodPoblado.ToString(), Text = x.fcPoblado }).ToList();
                return Json(listaMunicipio, JsonRequestBehavior.AllowGet);
            }
            
        }

        public ProveedoresController()
        {
            var ConnectionString = ConfigurationManager.ConnectionStrings["OrionConnections"].ConnectionString;
            _connection = new DbServiceConnection(ConnectionString);
        }

        // GET: Proveedores
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult CargarListaProveedores()
        {
            var lista = _connection.OrionContext.sp_Proveedores_Listar().ToList();

            return EnviarListaJson(lista);
        }

        public ActionResult ViewCrearProveedor()
        {

            ViewBag.ListaPaises = _connection.OrionContext.sp_GeoPais_Lista().Select(a => new { Value = a.fiCodPais, Text = a.fcPais}).ToList();
            ViewBag.ListaDepartamentos = _connection.OrionContext.sp_geo_ListaDepartamentos().Select(x => new { Value = x.fiCodDepartamento, Text = x.fcDepartamento}).ToList();
            ViewBag.ListaMunicipio = _connection.OrionContext.sp_Geo_ListaMunicipio_ByIdDepartamento(0).Select(a => new { Value = a.fiCodMunicipio, Text = a.fcMunicipio }).ToList();//logicaProveedor.CargarListaMunicipioParaSelect(0, 0);
            ViewBag.ListaAldea = _connection.OrionContext.sp_Geo_ListaBarrioColonia(1, 1,1).Select(a => new { Value = a.fiCodBarrio, Text = a.fcBarrio }).ToList(); //logicaProveedor.CargarListaAldeaParaSelect(0, 0, 0);
            ViewBag.ListaTipoPersona = _connection.OrionContext.sp_Catalogo_TipoPersona_Listar().Select(x => new SelectListItem { Value = x.fiIDTipoPersona.ToString(), Text = x.fcDescripcion }).ToList();
            //return PartialView(new CrearProveedorViewModel { fcFechaLimiteEmisionFactura = DateTimeHelper.getToday(), fdFechaCreado = DateTimeHelper.getToday() });
            return PartialView(new CrearProveedorViewModel());
        }

        [HttpPost]
        public JsonResult CrearProveedor(CrearProveedorViewModel model)
        {


            var listaProveedores = _connection.OrionContext.sp_Proveedores_Listar().ToList();

            if(listaProveedores.Any(x => x.fcRTN == model.fcRTN))
            {
                return Json(new sp_Proveedores_Crear_Result { fiMensaje = "0", fcMensaje = "Proveedor ya fue ingresado", IdIngresado = 0 });
            }

            var result = _connection.OrionContext.sp_Proveedores_Crear(
                model.fcNombre,
                model.fiTipoPersona,
                model.fcRTN,
                model.fcContacto,
                (model.fiCodPais > 0 ? model.fiCodPais : (short?)null),
                (model.fiCodDepartamento > 0 ? model.fiCodDepartamento : (int?)null),
                model.fiCodCiudad,
                model.fiCodAldeaPoblado,
                model.fcDireccion1,
                model.fcDIreccion2,
                model.fcTelefono,
                model.fcTelefonoMovil,
                model.fcCorreoElectronico,
                0/*fiidcuentaporpagar*/,
                model.fcCAI,
                model.fcFechaLimiteEmisionFactura/*model.fiDiasLimiteCredito*/,
                model.fcPrefijoFactura,
                model.fcRangoInicialFactura,
                model.fcRangoFinalFactura,
                model.fcComentario,
                model.fiDiasLimiteCredito,
                GetIdUser()).FirstOrDefault();

            return EnviarListaJson(result);
        }



        [HttpGet]
        public ActionResult ViewEditarProveedor(int fiIDProveedor)
        {
            using(var contexto = new ORIONDBEntities())
            {
                ViewBag.ListaTipoPersona = contexto.sp_Catalogo_TipoPersona_Listar().Select(x => new SelectListItem { Value = x.fiIDTipoPersona.ToString(), Text = x.fcDescripcion }).ToList();
                var modelDb = contexto.sp_Proveedores_ObtenerId(fiIDProveedor).FirstOrDefault();
                var model = new CrearProveedorViewModel
                {
                    fiIDProveedor = modelDb.fiIDProveedor,
                    fcNombre = modelDb.fcNombre,
                    fcRTN = modelDb.fcRTN,
                    fiTipoPersona = (modelDb.fiTipoPersona ?? 0),
                    fiDiasLimiteCredito = modelDb.fiDiasLimiteCredito,
                    fcCAI = modelDb.fcCAI,
                    fcFechaLimiteEmisionFactura = modelDb.fdFechaLimiteEmisionFactura,
                    fcRangoInicialFactura = modelDb.fiRangoInicialFactura,
                    fcRangoFinalFactura = modelDb.fcRangoFinalFactura,
                    fcPrefijoFactura = modelDb.fcPrefijoFactura,
                    fcTelefono = modelDb.fcTelefono,
                    fcTelefonoMovil = modelDb.fcTelefonoMovil,
                    fcDireccion1 = modelDb.fcDireccion1,
                    fcDIreccion2 = modelDb.fcDireccion2,
                    fcCorreoElectronico = modelDb.fcCorreoElectronico,
                    fcContacto = modelDb.fcContacto,
                    EsEditar = true
                    
                };
                return PartialView("ViewCrearProveedor", model);
            }
        }


        [HttpPost]
        public JsonResult EditarProveedor(CrearProveedorViewModel model) 
        {
            var listaProveedores = _connection.OrionContext.sp_Proveedores_Listar().ToList();

            if (listaProveedores.Where(x=>x.fiIDProveedor != model.fiIDProveedor).Any(x => x.fcRTN == model.fcRTN))
            {
                return Json(new sp_Proveedores_Crear_Result { fiMensaje = "0", fcMensaje = "Proveedor ya fue ingresado", IdIngresado = 0 });
            }

            var result = _connection.OrionContext.sp_Proveedores_Editar(
                model.fiIDProveedor,
                model.fcNombre,
                model.fiTipoPersona,
                model.fcRTN,
                model.fcContacto,
                (model.fiCodPais > 0 ? model.fiCodPais : (short?)null),
                (model.fiCodDepartamento > 0 ? model.fiCodDepartamento : (int?)null),
                model.fiCodCiudad,
                model.fiCodAldeaPoblado,
                model.fcDireccion1,
                model.fcDIreccion2,
                model.fcTelefono,
                model.fcTelefonoMovil,
                model.fcCorreoElectronico,
                0/*fiidcuentaporpagar*/,
                model.fcCAI,
                model.fcFechaLimiteEmisionFactura/*model.fiDiasLimiteCredito*/,
                model.fcPrefijoFactura,
                model.fcRangoInicialFactura,
                model.fcRangoFinalFactura,
                model.fcComentario,
                model.fiDiasLimiteCredito,
                GetIdUser()).FirstOrDefault();

            return EnviarListaJson(result);

        }




    }
}