using OrionCoreCableColor.DbConnection.OrionDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.Controllers
{
    [Authorize]
    public class LlenarCamposController : BaseController
    {
        private readonly DbServiceConnection _connection;

        // GET: LlenarCampos
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult SelectUsuarios()
        {

            using (var conetion = new ORIONDBEntities())
            {
                var SelecAreas = conetion.sp_Usuarios_Maestros_ListarUsuarios().Select(x => new SelectListItem { Value = x.fiIDUsuario.ToString(), Text = x.fcNombreCorto }).ToList();
                return EnviarListaJson(SelecAreas);
            }
        }

        public JsonResult SelectUsuarioTecnicosPorEmpresa(int idContratista, int idrol)
        {
            using (var conetion = new ORIONDBEntities())
            {
                var SelecAreas = conetion.sp_UsuariosPorEmpresaContratistaYRol(idContratista,3).Select(x => new SelectListItem { Value = x.fiIDUsuario.ToString(), Text = x.fcNombreCorto }).ToList();
                return EnviarListaJson(SelecAreas);
            }
        }

        public JsonResult ListaDepartamentos()
        {
            using (var coneccion = new ORIONDBEntities())
            {
                var lista = coneccion.sp_geo_ListaDepartamentos().ToList();
                return EnviarListaJson(lista);
            }
        }

        public JsonResult ListaMunicipio(int iddepartamento)
        {
            using (var coneccion = new ORIONDBEntities())
            {
                var lista = coneccion.sp_Geo_ListaMunicipio_ByIdDepartamento(iddepartamento).ToList();
                return EnviarListaJson(lista);
            }
        }
        
        public JsonResult ListaColonia(int codDepartamento, int codmunicipio)
        {
            using (var coneccion = new ORIONDBEntities())
            {
                var lista = coneccion.sp_Geo_ListaBarrioColonia(codDepartamento, codmunicipio, 1).ToList();
                return EnviarListaJson(lista);
            }
        }

        public JsonResult ListaEstatusClientesPotenciales()
        {
            using (var connection = new ORIONDBEntities())
            {
                var usuario = GetUser();
                var lista = connection.sp_Estatus_Clientes_Potenciales_Listado(usuario.fiIdUsuario, usuario.IdRol,1).Select(a => new SelectListItem { Value = a.fiIdEstatusCliente.ToString(), Text = a.fcNombreEstatus.ToString() }).ToList();
                return EnviarListaJson(lista);
            }
        }

        public JsonResult ListaFuentesClientesPotenciales()
        {
            using (var connection = new ORIONDBEntities())
            {
                var Usuario = GetUser();
                var listado = connection.sp_Fuente_Clientes_Potenciales_Listado(Usuario.fiIdUsuario, Usuario.IdRol, 1).Select(a => new SelectListItem { Value = a.fiIdFuenteCliente.ToString(), Text = a.fcNombrefuenteOrigen.ToString()}).ToList();
                return EnviarListaJson(listado);
            }
        }

        public JsonResult Listadocodigobarra(int fiIdProducto)
        {
            using (var connection = new ORIONDBEntities())
            {
                var Usuario = GetUser();
                var Listado = connection.sp_Productos_Maestro_ListadoSeries(Usuario.fiIdUsuario, Usuario.fiIdUsuario, fiIdProducto).ToList();
                return EnviarListaJson(Listado);
            }
        }

        public JsonResult ListadoAgenciasContratista()
        {
            try
            {
                using(var connection = new ORIONDBEntities())
                {
                    var Usuario = GetUser();
                    var listado = connection.sp_ListadoContratistaAgencia(Usuario.fiIdUsuario, Usuario.IdRol, 1).ToList();

                    return EnviarListaJson(listado);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public JsonResult ListadoTecnicosPorAgencia(int fiEmpreza)
        {
            try
            {
                using (var connection = new ORIONDBEntities())
                {
                    var Usuario = GetUser();
                    var listado = connection.sp_TecnicosPorContratista_GetByIdContratista(fiEmpreza).ToList().Select(x => new SelectListItem { Value = x.fiIDUsuarioTecnico.ToString(), Text = x.fcTecnico }).ToList();

                    return EnviarListaJson(listado);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }



    }
}