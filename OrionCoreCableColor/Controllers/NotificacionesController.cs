using OrionCoreCableColor.DbConnection.OrionDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using OrionCoreCableColor.Models.Notificaciones;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace OrionCoreCableColor.Controllers
{
    //[Authorize]
    public class NotificacionesController : BaseController
    {
        // GET: Notificaciones
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Listar()
        {
            var listaUsuariosClientes = new List<dynamic>();

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_ClientesMovil_Listado";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string ConvertToDateString(object dateObj)
                        {
                            if (dateObj == DBNull.Value) return null;
                            var dateValue = Convert.ToDateTime(dateObj);
                            return dateValue.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }

                        var cliente = new
                        {
                            fiIDUnico = reader["fiIDUnico"],
                            fiIDCuentaFamiliar = reader["fiIDCuentaFamiliar"],
                            fiIDCliente = reader["fiIDCliente"],
                            fcUsuarioAcceso = reader["fcUsuarioAcceso"],
                            fcPassword = reader["fcPassword"],
                            fcNombreUsuario = reader["fcNombreUsuario"],
                            fcNmbreCorto = reader["fcNmbreCorto"],
                            fcTelefono = reader["fcTelefono"],
                            fcURLFotoPersonalizda = reader["fcURLFotoPersonalizda"],
                            fiTipodeUsuario = reader["fiTipodeUsuario"],
                            fdFechaCreacion = ConvertToDateString(reader["fdFechaCreacion"]),
                            fdFechaPrimerAcceso = ConvertToDateString(reader["fdFechaPrimerAcceso"]),
                            fdFechaUltimoAcceso = ConvertToDateString(reader["fdFechaUltimoAcceso"]),
                            fcToken = reader["fcToken"],
                        };
                        listaUsuariosClientes.Add(cliente);
                    }
                }
            }

            return Json(listaUsuariosClientes, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult EnviarNotificacion(List<string> users)
        {
            if (users == null || users.Count == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "No se seleccionaron usuarios.");
            }

            var viewModel = new NotificacionesViewModel { Users = users, Message = "", FiIDUsuario = GetIdUser() };
            return PartialView("_EnviarNotificacion", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> EnviarNotificacionApp(NotificacionesViewModel model)
        {
            try
            {
                var response = await SendEndPointPush(model);

                if (response.Contains("exitosamente"))
                {
                    return EnviarResultado(true, "Enviar Notificación por App Novanet", response);
                }
                return EnviarResultado(false, "Enviar Notificación por App Novanet", response);
            }
            catch (Exception ex)
            {
                return EnviarResultado(true, "Enviar Notificación por App Novanet", ex.Message);

            }
        }


        public async Task<string> SendEndPointPush(NotificacionesViewModel model)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var jsonContent = JsonConvert.SerializeObject(model);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("https://api.novanetgroup.com/api/Novanet/SignalR/EnviarNotificacion", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var resultContent = await response.Content.ReadAsStringAsync();

                        var responseObject = JsonConvert.DeserializeObject<dynamic>(resultContent);

                        string message = responseObject?.message;

                        return message ?? "No se pudo obtener el mensaje";
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return $"Error: {errorContent}";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }


        //------------------------------------------------------------------------------------------------------------------------------------------------------


        [HttpPost]
        public ActionResult BitacoraNotificaciones(int fiIDEquifax)
        {
            var listaNotificaciones = new List<BitacoraNotificacionesViewModel>();

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_ClientesMovil_Notificaciones_By_Cliente {fiIDEquifax}";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var notificacion = new BitacoraNotificacionesViewModel
                        {
                            fiIDNotificacion = reader.GetInt32(reader.GetOrdinal("fiIDNotificacion")),
                            fcNotificacion = reader.GetString(reader.GetOrdinal("fcNotificacion")),
                            fiIDEquifax = reader.GetInt32(reader.GetOrdinal("fiIDEquifax")),
                            fcNombre = reader.GetString(reader.GetOrdinal("fcNombre")),
                            fdFechaEnvio = reader.GetDateTime(reader.GetOrdinal("fdFechaEnvio")),
                            fiIDUsuario = reader.GetInt32(reader.GetOrdinal("fiIDUsuario")),
                            fcNombreCorto = reader.GetString(reader.GetOrdinal("fcNombreCorto"))
                        };
                        listaNotificaciones.Add(notificacion);
                    }
                }
            }

            return PartialView("_BitacoraNotificaciones", listaNotificaciones);
        }


    }
}