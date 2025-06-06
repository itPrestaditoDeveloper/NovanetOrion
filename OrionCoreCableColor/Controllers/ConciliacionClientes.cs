
using ClosedXML.Excel;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser; 
using Microsoft.AspNetCore.Http;
using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models.ConciliacionClientes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OrionCoreCableColor.Controllers
{
    public class ConciliacionClientesController : BaseController
    {
        private readonly DbServiceConnection _connection;

        public ConciliacionClientesController()
        {
            var ConnectionString = ConfigurationManager.ConnectionStrings["OrionConnections"].ConnectionString;
            _connection = new DbServiceConnection(ConnectionString);

        }
        // GET: FacturacionClientes
        public ActionResult Index()
        {
            return View();
        }



        [HttpPost]
        public ActionResult UploadBillingFile(HttpPostedFileBase billingFile)
        {
            if (billingFile == null || billingFile.ContentLength == 0)
            {
                return Json(new { success = false, message = "Por favor suba un archivo válido." });
            }
            var extension = Path.GetExtension(billingFile.FileName).ToLower();
            List<ListConciliacionClientesViewModel> clientesCableColor = new List<ListConciliacionClientesViewModel>();

            if (extension == ".xlsx" || extension == ".xls")
            {
                using (var stream = billingFile.InputStream)
                {
                    try
                    {
                        clientesCableColor = LeerArchivoExcelDesdeStream(stream);
                        if (clientesCableColor == null || !clientesCableColor.Any())
                        {
                            return Json(new { success = false, message = "El archivo Excel no tiene el formato esperado o está vacío." });
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { success = false, message = "Error al procesar el archivo Excel. Verifique que el formato sea correcto." });

                    }
                }
            }
            else
            {
                return Json(new { success = false, message = "Formato de archivo no soportado." });
            }

            int user = GetIdUser();
            var usuarioLogueado = GetUser();
            var clientesActivosNovanet = _connection.OrionContext.sp_clientes_Activos_Novanet(usuarioLogueado.fiIdUsuario, usuarioLogueado.IdRol).ToList();

            //var clientesCoincidentes = clientesCableColor.Where(c => clientesActivosNovanet.Any(n =>
            //    (n.fcCodigoCliente?.Replace(" ", "").Trim() ?? "") == (c.CodigoCliente?.Replace(" ", "").Trim() ?? "")))
            //.ToList();

            //var clientesNoCoincidentes = clientesCableColor.Where(c =>
            //    !clientesActivosNovanet.Any(n =>
            //    (n.fcCodigoCliente?.Replace(" ", "").Trim() ?? "") == (c.CodigoCliente?.Replace(" ", "").Trim() ?? "")))
            //.ToList();
            var clientesCoincidentes = clientesCableColor.Where(c => clientesActivosNovanet.Any(n =>
            (n.fcNumeroOrdenCfeus?.Replace(" ", "").Trim() ?? "") == (c.NumeroOrdenCepheus?.Replace(" ", "").Trim() ?? "") ||
            CleanClientCode(n.fcNumeroOrdenCfeus) == CleanClientCode(c.NumeroOrdenCepheus)))
            .ToList();

            var clientesNoCoincidentes = clientesCableColor.Where(c => !clientesActivosNovanet.Any(n =>
                (n.fcNumeroOrdenCfeus?.Replace(" ", "").Trim() ?? "") == (c.NumeroOrdenCepheus?.Replace(" ", "").Trim() ?? "") ||
                CleanClientCode(n.fcNumeroOrdenCfeus) == CleanClientCode(c.NumeroOrdenCepheus)))
                .ToList();


            return Json(new { success = true, coincidentes = clientesCoincidentes, noCoincidentes = clientesNoCoincidentes });
        }

        private string CleanClientCode(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return "";
            }

            var match = System.Text.RegularExpressions.Regex.Match(codigo, @"\d+$");
            if (match.Success)
            {
                return match.Value.Trim();
            }      
            return codigo.Trim();
        }

        private List<ListConciliacionClientesViewModel> LeerArchivoExcelDesdeStream(Stream stream)
        {
            var clientesCableColor = new List<ListConciliacionClientesViewModel>();

            using (var workbook = new XLWorkbook(stream)) 
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed();

                var columnasEsperadas = new List<string> { "Cliente", "Servicio", "MesCreado", "Ciudad", "MontoTotal" };

                if (!worksheet.FirstRow().CellsUsed().Any() || worksheet.FirstRow().CellCount() < columnasEsperadas.Count)
                {
                    throw new Exception("Formato de archivo no válido.");
                }

                for (int i = 0; i < columnasEsperadas.Count; i++)
                {
                    var nombreColumna = worksheet.FirstRow().Cell(i + 1).GetString();
                    if (nombreColumna != columnasEsperadas[i])
                    {
                        throw new Exception($"El formato del archivo es incorrecto. Se esperaba la columna '{columnasEsperadas[i]}', pero se encontró '{nombreColumna}'.");
                    }
                }

                bool isFirstRow = true;

                foreach (var row in rows)
                {
                    if (isFirstRow)
                    {
                        isFirstRow = false;
                        continue;
                    }

                    var clienteConCodigo = row.Cell(1).GetString();
                    var codigoCliente = "";
                    var nombreCliente = clienteConCodigo;

                    if (clienteConCodigo.Contains("(") && clienteConCodigo.Contains(")"))
                    {
                        var startIdx = clienteConCodigo.IndexOf("(") + 1;
                        var endIdx = clienteConCodigo.IndexOf(")");
                        codigoCliente = clienteConCodigo.Substring(startIdx, endIdx - startIdx);
                        nombreCliente = clienteConCodigo.Substring(endIdx + 1).Trim();
                    }

                    var servicio = row.Cell(2).GetString();
                    var fcNumeroOrdenCfeus = "";

                    if (servicio.Contains("-"))
                    {
                        var partesServicio = servicio.Split('-');
                        if (partesServicio.Length > 1)
                        {
                            fcNumeroOrdenCfeus = partesServicio[1].Split(' ')[0]; 
                        }
                    }

                    string mesCreado;

                    var mesCreadoCell = row.Cell(3);
                    if (mesCreadoCell.DataType == XLDataType.DateTime)
                    {
                        mesCreado = mesCreadoCell.GetDateTime().ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        mesCreado = mesCreadoCell.GetString();
                    }

                    var ciudad = row.Cell(4).GetString();
                    var montoTotal = row.Cell(5).GetDouble().ToString("N2");

                    var clienteObj = new ListConciliacionClientesViewModel
                    {
                        CodigoCliente = codigoCliente,
                        NumeroOrdenCepheus = fcNumeroOrdenCfeus,
                        Nombre = nombreCliente,
                        Servicio = servicio,
                        MesCreado = mesCreado,
                        Ciudad = ciudad,
                        MontoTotal = montoTotal
                    };

                    clientesCableColor.Add(clienteObj); 
                }
            }

            return clientesCableColor;
        }


    }
}