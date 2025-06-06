using Newtonsoft.Json;
using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models.BaseCallCenter;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.Controllers
{
    [Authorize]
    public class BaseClientesController : BaseController
    {
        private readonly DbServiceConnection _connection;
        // GET: BaseClientes
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BaseClienteBandeja()
        {
            return View();
        }

        public JsonResult ListaBaseCliente()
        {
            try
            {
                using (var conetion = new ORIONDBEntities())
                {
                    var list = GetIdUser();
                    var lista = EnviarListaJson(conetion.sp_BasesCallCenter_Clientes(GetIdUser()).ToList());
                    return lista;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public ActionResult FiltroAgenteBaseCallCenter()
        {
            using (var contexto = new ORIONDBEntities())
            {
                //ViewBag.Agentes = contexto.sp_ListaAgentes_BaseCallCenter().Select(x => new SelectListItem { Value = x.fiIDUsuario.ToString(), Text = x.fcNombreCorto }).ToList();
            }

            return View();
        }

        public JsonResult ListaBaseClientePorAgente(int IDUsuario) //usar esta para el datatable del filtro
        {
            try
            {
                using (var conetion = new ORIONDBEntities())
                {
                    var lista = EnviarListaJson(conetion.sp_BasesCallCenter_Clientes(IDUsuario).ToList());
                    return lista;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public ActionResult AgregarBitacora(int cliente,string telefonocontacto)
        {
            ViewBag.idcliente = cliente;
            ViewBag.telefonocontacto = telefonocontacto;

            return PartialView();
        }
        public JsonResult SelectGestion()
        {
            using (var contexto = new ORIONDBEntities())
            {
                var SelecAreas = contexto.sp_BasesCallCenter_CatalogoGestiones(GetIdUser()).Select(x => new SelectListItem { Value = x.fiIDGestion.ToString(), Text = x.fcGestion }).ToList();
                return EnviarListaJson(SelecAreas);
            }
        }
        public JsonResult GuardarBitacora(int idcliente, string comentariogestion,int idgestion,string telefono)
        {
            try
            {
                using (var conection= new ORIONDBEntities())
                {
                    var save = conection.sp_BasesCallCenter_Bitacoras_Agregar(GetIdUser(), idcliente, comentariogestion, idgestion, telefono);
                    return EnviarListaJson(save);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public JsonResult GraficaGlobal()
        {

            using (var conetion = new ORIONDBEntities())
            {
                var lista = conetion.sp_BasesCallCenter_AvanceGlobal(GetIdUser()).ToList();

                return EnviarListaJson(lista);
            }

        }

        public ActionResult DashboardGestiones()
        {
            return View();
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
        public ActionResult DetalleCliente(int ID)
        {
            using (var conetion = new ORIONDBEntities())
            {
                var lista = conetion.sp_BasesCallCenter_Cliente_Informacion(ID).FirstOrDefault();
                ViewBag.idcliente = lista.fiIDCliente;
                return PartialView(lista);
            }
        }

        public JsonResult BitacoraCliente(int ID)
        {
            using (var conetion = new ORIONDBEntities())
            {
                var lista = EnviarListaJson(conetion.sp_BasesCallCenter_Bitacoras(GetIdUser(), ID).ToList());
                return lista;
            }
        }

        public JsonResult LlenarDatosBaseCallCenter()
        {
            try
            {
                using (var conetion = new ORIONDBEntities())
                {
                    List<BasesCallCenter_AvanceGlobalViewModel> result = new List<BasesCallCenter_AvanceGlobalViewModel>();
                    var idusuario = GetIdUser();
                    var lista = conetion.sp_BaseCallCenter_AvanceGeneral_Filtro(GetIdUser()).ToList();
                    foreach (var item in lista)
                    {
                        var list = JsonConvert.DeserializeObject<BasesCallCenter_AvanceGlobalViewModel>(item);
                        result.Add(list);
                    }
                    return EnviarListaJson(result.ToList());
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult ListaFiltroBaseClietes()
        {
            using (var contexto = new ORIONDBEntities())
            {
                ViewBag.Agentes = contexto.sp_ListaAgentes_BaseCallCenter().Select(x => new SelectListItem { Value = x.fiIDUsuario.ToString(), Text = x.fcNombreCorto }).ToList();
            }
            return View();
        }

        public JsonResult GraficaDeTiempoAgentes(string fechavisualizar,int idagente)
        {
            var List = new List<GestionTiempoBaseCallCenterViewModel>();//_connection.OrionContext.sp_BaseCallCenter_Estadistica(fechavisualizar,GetIdUser(), 51);
            var List2 = new List<GestionCallCenter>();

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_BaseCallCenter_Estadistica '{fechavisualizar}',{GetIdUser()},{idagente}";
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    List = db.ObjectContext.Translate<GestionTiempoBaseCallCenterViewModel>(reader).ToList();
                    reader.NextResult();
                    List2 = db.ObjectContext.Translate<GestionCallCenter>(reader).ToList();
                }
                connection.Close();
            }
            return Json(new { GestionDiaCallCenter = List, GestionDetalleCallCenter = List2 }, JsonRequestBehavior.AllowGet);
        }

        #region Clientes Potenciales

        public ActionResult BandejaClientesPotenciales()
        {
            return View();
        }

        public JsonResult ListadoClientesPotenciales()
        {
            try
            {
                using (var contexto = new ORIONDBEntities())
                {
                    var usuario = GetUser();
                    var listado = contexto.sp_Clientes_Potenciales_Listado(usuario.fiIdUsuario, usuario.IdRol, 1).ToList();
                    return EnviarListaJson(listado.Where(a => a.fbPuedeVerlo == true).ToList());
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult ClientePotencialFormulario()
        {
            return PartialView();
        }

        public JsonResult GuardarClientePotencial(ClientesPotencialesViewModel modelo)
        {
            try
            {
                using (var contexto = new ORIONDBEntities())
                {
                    var usuario = GetUser();
                    var GuardarClientePotencial = contexto.sp_Clientes_Potenciales_Guardar(modelo.fcNombreCliente,modelo.fcNumeroTelefono,modelo.fcDireccionCliente,modelo.fcRazonEscribir,modelo.fiIdFuenteCliente,modelo.fcUrlEnlace, usuario.fiIdUsuario, usuario.fiIdUsuario,modelo.fiEstatus).FirstOrDefault();
                    AgregarClientePotencial((int)GuardarClientePotencial.fiIdRegistroCreado);
                    return EnviarListaJson(GuardarClientePotencial);
                }   
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public ActionResult BitacoraClientePotencial(int idClientePotencial,int fiidestatus)
        {
            ViewBag.FiIdClientePotenciales = idClientePotencial;
            ViewBag.fiEstatus = fiidestatus;
            return PartialView();
        }

        public JsonResult ListadoBitacoraClientePotencial(int idClientePotencial)
        {
            try
            {
                using (var connection = new ORIONDBEntities())
                {
                    var Usuario = GetUser();
                    var listado = connection.sp_clientes_Potenciales_Bitacoras_Listado(idClientePotencial, Usuario.fiIdUsuario, Usuario.IdRol, 1).ToList();
                    return EnviarListaJson(listado);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public JsonResult GuardarBitacoraClientePotencias(ClientesPotencialesViewModel modelo)
        {
            try
            {
                using (var contexto = new ORIONDBEntities())
                {
                    var usuario = GetUser();
                    var GuardarClientePotencial = contexto.sp_clientes_Potenciales_Bitacoras_Guardar(modelo.fiIdClientesPotenciales, usuario.fiIdUsuario,modelo.fcomentario, modelo.fiEstatus).FirstOrDefault();
                    ActualizarClienteBandejaClientePotenciales(modelo.fiIdClientesPotenciales);
                    AgregarBitacoraClientesPotenciales((int)GuardarClientePotencial.FiRegistroCreado);
                    return EnviarListaJson(GuardarClientePotencial);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public JsonResult EliminarClientePotencial(int fiIdClientePotencial)
        {
            try
            {
                using (var contexto = new ORIONDBEntities())
                {
                    var usuario = GetUser();
                    var EliminarClientes = contexto.sp_Clientes_Potenciales_Eliminar(fiIdClientePotencial,usuario.fiIdUsuario,usuario.IdRol,1).FirstOrDefault();
                    EliminarRowClientesPotenciales(fiIdClientePotencial);
                    return EnviarListaJson(EliminarClientes);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ActionResult BandejaEstadoclientePotencial()
        {
            return View();
        }

        public JsonResult ListadoEstadusClientePotencial()
        {
            try
            {
                using (var connection = new ORIONDBEntities())
                {
                    var Usuario = GetUser();
                    var modelo = connection.sp_Estatus_Clientes_Potenciales_Listado(Usuario.fiIdUsuario, Usuario.IdRol,1).ToList();
                    return EnviarListaJson(modelo);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public ActionResult AgregarEstadoClientePotencial()
        {
            return PartialView();
        }
        public JsonResult GuardarEstadoClientePotencial(EstadosClientesPotencialesViewModel modelo)
        {
            try
            {
                using (var contexto = new ORIONDBEntities())
                {   
                    var usuario = GetUser();
                    var AgregarEstatusClientes = contexto.sp_Estatus_Clientes_Potenciales_Guardar(modelo.fcNombreEstatus,modelo.fcdescripcionEstatus, modelo.fcHexadecimal,usuario.fiIdUsuario,usuario.IdRol,1).FirstOrDefault();
                    AgregarEstatusDropCrearClientePotencia((int)AgregarEstatusClientes.FiRegistroCreado);
                    AgregarRowEstatusClientePotenciales((int)AgregarEstatusClientes.FiRegistroCreado);
                    return EnviarListaJson(AgregarEstatusClientes);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public ActionResult EditarEstatusClientePotenciales(int fiIdEstatusCliente)
        {
            using (var connection = new ORIONDBEntities())
            {
                var Usuario = GetUser();
                var modelo = connection.sp_Estatus_Clientes_Potenciales_Listado_ById(fiIdEstatusCliente,Usuario.fiIdUsuario,Usuario.IdRol,1).FirstOrDefault();
                ViewBag.fiIdEstadoCliente = fiIdEstatusCliente;
                return PartialView(modelo);
            }
        }
        public JsonResult ModificarEstadoClientePotencial(EstadosClientesPotencialesViewModel modelo)
        {
            try
            {
                using (var connection = new ORIONDBEntities())
                {
                    var Usuario = GetUser();
                    var model = connection.sp_Estatus_Clientes_Potenciales_Editar(modelo.fiIdEstatusCliente,modelo.fcNombreEstatus,modelo.fcdescripcionEstatus,modelo.fcHexadecimal,Usuario.fiIdUsuario,Usuario.IdRol,1).FirstOrDefault();
                    EditarRowEstatusClientePotenciales(modelo.fiIdEstatusCliente);
                    return EnviarListaJson(model);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        ////////////
        public ActionResult BandejaFuenteClientesPotenciales()
        {
            return View();
        }

        public JsonResult ListadoBandejaFuenteCliente()
        {
            try
            {
                using (var connection = new ORIONDBEntities())
                {
                    var Usuario = GetUser();
                    var listado = connection.sp_Fuente_Clientes_Potenciales_Listado(Usuario.fiIdUsuario, Usuario.IdRol,1).ToList();
                    return EnviarListaJson(listado);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        public ActionResult AgregarFuenteClientePotencial(int fiLiugarDondeCrea)
        {
            return PartialView();
        }

        public JsonResult GuardarFuenteClientePotencial(FuenteClientePotencialesViewModel modelo)
        {
            try
            {
                using (var contexto = new ORIONDBEntities())
                {
                    var Usuario = GetUser();
                    var AgregarFuenteCliente = contexto.sp_Fuente_Clientes_Potenciales_Guardar(modelo.fcNombrefuenteOrigen, modelo.fcDescripcionFuenteOrigen, Usuario.fiIdUsuario, 1).FirstOrDefault();

                    /*
                        1: es desde el modal
                        2: es desde la bandeja de fuente
                     */
                    switch (modelo.fiLiugarDondeCrea)
                    {
                        case 1:  break; 
                        case 2:   break;
                        case 3: break;
                        default: break;
                    }
                    AgregarFuenteDropClientePotenciales((int)AgregarFuenteCliente.FiRegistroCreado);
                    AgregarRowFuenteClientePotenciales((int)AgregarFuenteCliente.FiRegistroCreado);

                    return EnviarListaJson(AgregarFuenteCliente);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public ActionResult EditarFuenteClientePotencial(int fiIdClientePotencial)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var Usuario = GetUser();
                var model = contexto.sp_Fuente_Clientes_Potenciales_Listado_ById(fiIdClientePotencial, Usuario.fiIdUsuario,Usuario.IdRol,1).FirstOrDefault();
                ViewBag.fiIdFuenteCliente = fiIdClientePotencial;
                return PartialView(model);
            }
        }

        public JsonResult ModificarFuenteClientePotencial(FuenteClientePotencialesViewModel modelo)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var Usuario = GetUser();
                var model = contexto.sp_Fuente_Clientes_Potenciales_Modificar(modelo.fiIdFuenteCliente,modelo.fcNombrefuenteOrigen,modelo.fcDescripcionFuenteOrigen,Usuario.fiIdUsuario,1).FirstOrDefault();
                EditarRowFuenteClientePotenciales((int)model.FiIdRegistro);
                return EnviarListaJson(model);
            }
        }


        public JsonResult EliminarFuenteClientePotenciales(int fiIdFuenteClientePotenciales)
        {
            try
            {
                using (var contexto = new ORIONDBEntities())
                {
                    var Usuario = GetUser();
                    var modelo = contexto.sp_Fuente_Clientes_Potenciales_Eliminar(fiIdFuenteClientePotenciales,Usuario.fiIdUsuario,1).FirstOrDefault();
                    EliminarRowClientePotenciales(fiIdFuenteClientePotenciales);
                    return EnviarListaJson(modelo);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion

    }
}