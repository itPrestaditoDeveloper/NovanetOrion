using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models.Solicitudes;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.Controllers
{
    public class DebitoAutomaticoController : BaseController
    {
        // GET: DebitoAutomatico
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult LsitaSolicitudes()
        {

            using (var conetion = new ORIONDBEntities())
            {
                
                var lista = EnviarListaJson(conetion.sp_Enrolamiento_listado().Where(x=>x.fiEstadoEnrolamiento == 1).ToList());
                //EnviarNotificacion("DESDE BASE SOLICITUDES");
                return lista;
            }

        }

        [HttpGet]
        public JsonResult LsitaSolicitudesPendiente()
        {

            using (var conetion = new ORIONDBEntities())
            {

                var lista = EnviarListaJson(conetion.sp_Enrolamiento_listado().Where(x => x.fiEstadoEnrolamiento == 0).ToList());
                //EnviarNotificacion("DESDE BASE SOLICITUDES");
                return lista;
            }

        }

        [HttpGet]
        public ActionResult ModalEnvioDebitoAutomaticoSMS(string Nombre, string Telefono, int IDCliente, string fcIdentidadCliente)
        {
            using (var conetion = new ORIONDBEntities())
            {
                //var datos = conetion.sp_OrionSolicitudes_InformacionDocumentacion(0, IDCliente, 1).FirstOrDefault();
                var infoDocumentos = new sp_OrionSolicitud_InformacionDocumentacion_ViewModel();

                using (var connection = (new ORIONDBEntities()).Database.Connection)
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = $"EXEC sp_Mensaje_DebitoAutomatico '{fcIdentidadCliente}' ";
                    using (var reader = command.ExecuteReader())
                    {
                        var db = ((IObjectContextAdapter)new ORIONDBEntities());
                        infoDocumentos = db.ObjectContext.Translate<sp_OrionSolicitud_InformacionDocumentacion_ViewModel>(reader).FirstOrDefault();
                    }
                    connection.Close();
                }

                ViewBag.ListaParentesco = conetion.sp_Catalogo_Parentescos_List().Select(x => new { id = x.fiIDParentesco.ToString(), text = x.fcDescripcionParentesco }).ToList();
                return PartialView(new SolicitudesViewModel { Nombre = Nombre, Telefono = Telefono, IdCliente = IDCliente, fcMensaje = infoDocumentos.fcMensaje });
            }

        }
    }
}