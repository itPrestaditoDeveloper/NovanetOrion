using OrionCoreCableColor.DbConnection.OrionDB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.Controllers
{
    public class ReferidosController : BaseController
    {
        // GET: Referidos
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult List()
        {
            var listado = new List<dynamic>();

            using (var connection = (new ORIONDBEntities()).Database.Connection)
            {
                int FiIDUsuario = GetUser().fiIdUsuario;

                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_ClientesReferidosCreditos_Listado {FiIDUsuario}";

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

                        var data = new
                        {
                            fiIDEquifaxClienteReferente = reader["fiIDEquifaxClienteReferente"],
                            fcNombreReferente = reader["fcNombreReferente"],
                            fcIdentidadReferente = reader["fcIdentidadReferente"],
                            fcClase = reader["fcClase"],
                            fcNombreReferido = reader["fcNombreReferido"],
                            fiIDEquifax = reader["fiIDEquifax"],
                            fcNumeroTelefono = reader["fcNumeroTelefono"],
                            fcDescripcion = reader["fcDescripcion"],
                            fbClienteInstalado = reader["fbClienteInstalado"],
                            fdFechaCreacion = ConvertToDateString(reader["fdFechaCreacion"]),
                            fdFechaVencimiento = ConvertToDateString(reader["fdFechaVencimiento"]),
                        };
                        listado.Add(data);
                    }
                }
            }
          
            return Json(listado, JsonRequestBehavior.AllowGet);
        }

    }
}