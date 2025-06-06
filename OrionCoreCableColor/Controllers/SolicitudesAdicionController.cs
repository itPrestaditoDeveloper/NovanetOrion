using Newtonsoft.Json;
using OrionCoreCableColor.DbConnection.OrionDB;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.Controllers
{
    public class SolicitudesAdicionController : Controller
    {
        // GET: SolicitudesAdicion
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
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "EXEC sp_Solicitudes_AdicionProducto_Listado";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string ConvertToDateString(object dateObj) => dateObj != DBNull.Value ? Convert.ToDateTime(dateObj).ToString("dd/MM/yyyy") : null;

                            var detallesProductos = DeserializeJson<List<DetalleSolicitudProducto>>(reader["fcJsonDetalles"]);
                            var detallesSolicitudes = DeserializeJson<List<DetalleOtraSolicitud>>(reader["fcJsonSolicitudes"]);

                            detallesSolicitudes?.ForEach(solicitud =>
                            {
                                solicitud.productos = DeserializeJson<List<Producto>>(solicitud.fcJsonProductos);
                            });

                            listado.Add(new
                            {
                                fiIDAdicionProduto = GetInt(reader, "fiIDAdicionProduto"),
                                fiIDCliente = GetInt(reader, "fiIDCliente"),
                                fdFechadeCreacion = ConvertToDateString(reader["fdFechadeCreacion"]),
                                fcIdentidad = GetString(reader, "fcIdentidad"),
                                fcNombre = GetString(reader, "fcNombre"),
                                fcTelefono = GetString(reader, "fcTelefono"),
                                fiIDSolicitudOrion = GetInt(reader, "fiIDSolicitudOrion"),
                                fiIDUsuarioSolicitante = GetString(reader, "fiIDUsuarioSolicitante"),
                                fcCorreo = GetString(reader, "fcCorreo"),
                                fiIDApp = GetInt(reader, "fiIDApp"),
                                fiDEstadoInstalacion = GetInt(reader, "fiDEstadoInstalacion"),
                                fdFechaInstalacion = ConvertToDateString(reader["fdFechaInstalacion"]),
                                fiIDPagoInicial = GetInt(reader, "fiIDPagoInicial"),
                                fdFechaPagoInicial = ConvertToDateString(reader["fdFechaPagoInicial"]),
                                fiDEstadoFirma = GetInt(reader, "fiDEstadoFirma"),
                                fdFechaFirma = ConvertToDateString(reader["fdFechaFirma"]),
                                fcComprobanteUrl = GetString(reader, "fcComprobanteUrl"),
                                DetallesProductos = JsonConvert.SerializeObject(detallesProductos),
                                DetallesSolicitudes = JsonConvert.SerializeObject(detallesSolicitudes),
                                fcJsonProductos = JsonConvert.SerializeObject(detallesSolicitudes?.SelectMany(s => s.productos ?? new List<Producto>()) ?? new List<Producto>())
                            });
                        }
                    }
                }
            }
            return Json(listado, JsonRequestBehavior.AllowGet);
        }

        // Métodos auxiliares para limpiar el código
        private static T DeserializeJson<T>(object json) where T : new()
        {
            try { return json != DBNull.Value && !string.IsNullOrEmpty(json.ToString()) ? JsonConvert.DeserializeObject<T>(json.ToString()) : new T(); }
            catch { return new T(); }
        }

        private static int GetInt(DbDataReader reader, string column) => reader[column] != DBNull.Value ? Convert.ToInt32(reader[column]) : 0;
        private static string GetString(DbDataReader reader, string column) => reader[column] != DBNull.Value ? reader[column].ToString() : string.Empty;





    }

    public class DetalleSolicitudProducto
    {
        public int fiIDSolicitud { get; set; }
        public int fiIDAdicionProdutoDetalle { get; set; }
        public int fiIDAdicionProduto { get; set; }
        public int fiCantidad { get; set; }
        public string fcProducto { get; set; }
        public string fcMarca { get; set; }
        public string fcModelo { get; set; }
        public string fcTipoProducto { get; set; }
    }

    public class DetalleOtraSolicitud
    {
        public int fiIDSolicitud { get; set; }
        public DateTime fdFechaCreacionSolicitud { get; set; }
        public decimal fnCuotaMensual { get; set; }
        public string fcDireccionDetallada { get; set; }
        public string fcDepartamento { get; set; }
        public string fcMunicipio { get; set; }
        public string fcBarrio { get; set; }
        public string fcJsonProductos { get; set; }
        public List<Producto> productos { get; set; }

    }
    public class Producto
    {
        public int fiIDProducto { get; set; }
        public string fcTipoProducto { get; set; }
        public string fcMarca { get; set; }
        public string fcModelo { get; set; }
        public string fcProducto { get; set; }
        public decimal fnCantidad { get; set; }
        public string fcNumerodeSerie1 { get; set; }
        public string fcNumerodeSerie2 { get; set; }
        public int fiIDSolicitud { get; set; }
        public int fiIDCliente { get; set; }
    }

}
