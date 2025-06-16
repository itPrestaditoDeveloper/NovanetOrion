using Newtonsoft.Json;
using NReco.PdfGenerator;
using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.App_Services.ContratoService;
using OrionCoreCableColor.App_Services.ReportesService;
using OrionCoreCableColor.Controllers;
using OrionCoreCableColor.DbConnection.CoreAdministrativoDB;
using OrionCoreCableColor.DbConnection.CoreFinancieroDB;
using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models.Base;
using OrionCoreCableColor.Models.EmailTemplateService;
using OrionCoreCableColor.Models.Precalificado;
using OrionCoreCableColor.Models.Productos;
using OrionCoreCableColor.Models.Reportes;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;


namespace OrionCoreCableColor.App_Services.EmailService
{
    public class EmailTemplateService 
    {
        private DbServiceConnection _connection;
        private SendEmailService _emailService;
        private readonly BaseController _baseController;
        public EmailTemplateService()
        {
            var ConnectionString = ConfigurationManager.ConnectionStrings["OrionConnections"].ConnectionString; // DataCrypt.Desencriptar(ConfigurationManager.ConnectionStrings["ConexionEncriptada"].ConnectionString);
            _connection = new DbServiceConnection(ConnectionString);

            _emailService = new SendEmailService();
            _baseController = new BaseController();

        }

        public string pathToOpen { get; set; }
        public string pathToSave { get; set; }

        public Dictionary<string, string> DictionaryList = new Dictionary<string, string>();

        private Attachment GeneratePDFAttachment()
        {
            using (Document doc = new Document()) {
                doc.LoadFromFile(pathToOpen);

                foreach (var item in DictionaryList)
                {
                    doc.Replace("{" + item.Key + "}", item.Value, true, true);
                }

                doc.SaveToFile(pathToSave, FileFormat.PDF);

                var attachmentFile = new Attachment(pathToSave, MediaTypeNames.Application.Octet);
                doc.Close();
                return attachmentFile;
            }

                
            
        }

        private Attachment GeneratePDFAttachmentFrimadoRevisado()
        {
            using (Document doc = new Document())
            {
                doc.LoadFromFile(pathToOpen);
                string data = "";
                foreach (var item in DictionaryList)
                {
                    if (item.Key == "fcFirma")
                    {
                        data = item.Value;
                    }
                    else
                    {
                        doc.Replace("{" + item.Key + "}", item.Value ?? "", true, true);
                    }
                }

                var base64Data = Regex.Match(data, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                var binData = Convert.FromBase64String(base64Data);

                using (var stream = new MemoryStream(binData))
                {
                    var Images = new Bitmap(stream);

                    Image image = Image.FromHbitmap(Images.GetHbitmap());

                    foreach (Section section in doc.Sections)
                    {
                        foreach (Paragraph paragraph in section.Paragraphs)
                        {
                            foreach (DocumentObject docObj in paragraph.ChildObjects)
                            {
                                if (docObj.DocumentObjectType == DocumentObjectType.Picture)
                                {
                                    DocPicture picture = docObj as DocPicture;
                                    if (picture.Title == "Firma")
                                    {
                                        //Replace the image
                                        picture.LoadImage(image);
                                    }
                                }
                            }
                        }
                        //Loop through the child elements of paragraph

                    }


                }

                doc.SaveToFile(pathToSave, FileFormat.PDF);

                var attachmentFile = new Attachment(pathToSave, MediaTypeNames.Application.Octet);
                doc.Close();
                return attachmentFile;
            }
            
        }

        private Attachment GeneratePDFAttachmentAdendumFirmado()
        {
            using (Document doc = new Document())
            {
                doc.LoadFromFile(pathToOpen);
                string dataFirma1 = "";
                string dataFirma2 = "";

                // Extraer valores del diccionario
                foreach (var item in DictionaryList)
                {
                    if (item.Key == "fcFirma")
                    {
                        dataFirma1 = item.Value;
                    }
                    else if (item.Key == "fcFirmaErick")
                    {
                        dataFirma2 = item.Value;
                    }
                    else
                    {
                        doc.Replace("{" + item.Key + "}", item.Value ?? "", true, true);
                    }
                }

                // Convertir base64 a imagen
                Image firma1 = ConvertirBase64AImagen(dataFirma1);
                Image firma2 = ConvertirBase64AImagen(dataFirma2);

                // Insertar ambas firmas en el documento
                InsertarFirmaEnDocumento(doc, firma1, "Firma");
                InsertarFirmaEnDocumento(doc, firma2, "Firma2");

                doc.SaveToFile(pathToSave, FileFormat.PDF);

                var attachmentFile = new Attachment(pathToSave, MediaTypeNames.Application.Octet);
                doc.Close();
                return attachmentFile;
            }
        }

        ///// <summary>
        ///// Convierte una cadena base64 en una imagen.
        ///// </summary>
        private Image ConvertirBase64AImagen(string base64Data)
        {
            if (string.IsNullOrEmpty(base64Data))
            {
                Console.WriteLine("Base64 vacío o nulo.");
                return null;
            }

            try
            {
                var match = Regex.Match(base64Data, @"data:image/(?<type>.+?);base64,(?<data>.+)");
                if (!match.Success)
                {
                    Console.WriteLine("Formato de Base64 incorrecto.");
                    return null;
                }

                var binData = Convert.FromBase64String(match.Groups["data"].Value);
                using (var stream = new MemoryStream(binData))
                {
                    return Image.FromStream(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al convertir base64 a imagen: {ex.Message}");
                return null;
            }
        }

        ///// <summary>
        ///// Inserta una firma en un documento buscando el marcador de imagen con el título dado.
        ///// </summary>
        private void InsertarFirmaEnDocumento(Document doc, Image firma, string tituloFirma)
        {
            if (firma == null)
            {
                Console.WriteLine($"Firma '{tituloFirma}' es nula, no se insertará.");
                return;
            }

            foreach (Section section in doc.Sections)
            {
                foreach (Paragraph paragraph in section.Paragraphs)
                {
                    foreach (DocumentObject docObj in paragraph.ChildObjects)
                    {
                        if (docObj.DocumentObjectType == DocumentObjectType.Picture)
                        {
                            DocPicture picture = docObj as DocPicture;

                            if (picture.Title == tituloFirma)
                            {
                                picture.LoadImage(firma);
                                picture.Width = (int)(1.65 * 37.8); // 1.65 cm ≈ 62.37 px
                                picture.Height = (int)(3.45 * 37.8); // 3.45 cm ≈ 130.41 px
                                Console.WriteLine($"Firma '{tituloFirma}' insertada correctamente.");
                            }
                        }
                    }
                }
            }
        }


        private Attachment GeneratePDFAttachmentFrimado()
        {
            using (Document doc = new Document())
            {
                doc.LoadFromFile(pathToOpen);
                string data = "";
                foreach (var item in DictionaryList)
                {
                    if (item.Key == "fcFirma")
                    {
                        data = item.Value;
                    }
                    else
                    {
                        doc.Replace("{" + item.Key + "}", item.Value ?? "", true, true);
                    }
                }

                var base64Data = Regex.Match(data, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                var binData = Convert.FromBase64String(base64Data);

                using (var stream = new MemoryStream(binData))
                {
                    var Images = new Bitmap(stream);

                    Image image = Image.FromHbitmap(Images.GetHbitmap());

                    foreach (Section section in doc.Sections)
                    {
                        foreach (Paragraph paragraph in section.Paragraphs)
                        {
                            foreach (DocumentObject docObj in paragraph.ChildObjects)
                            {
                                if (docObj.DocumentObjectType == DocumentObjectType.Picture)
                                {
                                    DocPicture picture = docObj as DocPicture;
                                    if (picture.Title == "Firma")
                                    {
                                        //Replace the image
                                        picture.LoadImage(image);
                                    }
                                }
                            }
                        }
                        //Loop through the child elements of paragraph

                    }


                }

                doc.SaveToFile(pathToSave, FileFormat.PDF);

                var attachmentFile = new Attachment(pathToSave, MediaTypeNames.Application.Octet);
                doc.Close();
                return attachmentFile;
            }

        }

        private Dictionary<string, string> GenerateDictionaryOfVariablesVendedores(int fiIDVendedorRepartidor)
        {

            var datosCliente = _connection.OrionContext.sp_OrionSolicitud_Repartidor_Listar(1).FirstOrDefault(x=>x.fiIDVendedorRepartidor == fiIDVendedorRepartidor);
            DictionaryList.Clear();
            DictionaryList.Add("DateNow", DateTime.Now.ToString("MMM dd, yyyy"));
            DictionaryList.Add("fcNombreVendedor", datosCliente.fcNombreVendedor.ToUpper());
            DictionaryList.Add("fcIdentidadVendedor", datosCliente.fcIdentidadVendedor);
            return DictionaryList;
        }

        private Dictionary<string, string> GenerateDictionaryOfVariablesDistribuidor(int fiIDDistribuidor)
        {

            var data = _connection.OrionContext.Database.SqlQuery<ListDistribuidorViewModel>("Exec sp_Orion_Ventas_Distribuidor_Listado").FirstOrDefault(x => x.fiIDDistribuidor == fiIDDistribuidor);
            DictionaryList.Clear();
            DictionaryList.Add("DateNow", DateTime.Now.ToString("MMM dd, yyyy"));
            DictionaryList.Add("NombreRepresentante", data.NombreRepresentante.ToUpper());
            DictionaryList.Add("fcIdentidadRepresentante", data.fcIdentidadRepresentante);
            DictionaryList.Add("fcNombreComercial", data.fcNombreComercial);
            DictionaryList.Add("fcRTN", data.fcRTN);
            return DictionaryList;
        }


        private Dictionary<string, string> GenerateDictionaryOfVariables(int idSolicitud, int idCliente, int idOperacion, int fiIDSOlicitudInstalacion)
        {

            var datosCliente = _connection.OrionContext.sp_OrionContratista_DetalleBySolicitud(1, idSolicitud, idOperacion, fiIDSOlicitudInstalacion).FirstOrDefault();
            var NumeroOrden = Convert.ToString(datosCliente.fiNoOrden);
            DateTime fechaActual = DateTime.Now;
            string fechaFormateada = $"{fechaActual.Day} días del mes de {fechaActual.ToString("MMMM", new System.Globalization.CultureInfo("es-ES"))} del año {fechaActual.Year}";
            DictionaryList.Clear();
            DictionaryList.Add("DateNow", DateTime.Now.ToString("MMM dd, yyyy"));
            DictionaryList.Add("fcNombreEmpresa", datosCliente.fcNombreEmpresa);
            DictionaryList.Add("fcNombreCliente", datosCliente.fcNombre);
            DictionaryList.Add("fcIdentidad", datosCliente.fcIdentidad);
            DictionaryList.Add("fcNumeroORden", "00" + NumeroOrden);
            DictionaryList.Add("fcNORdenCepehus", datosCliente.fcNumeroOrdenCfeus);
            DictionaryList.Add("fcTelefonoCliente", datosCliente.fcTelefonoCliente);
            DictionaryList.Add("fcEmailCliente", datosCliente.fcCorreo);
            DictionaryList.Add("fcEquipoAdquirido", datosCliente.fcArticulosdelContrato);
            DictionaryList.Add("fcDireccionDomicilioCliente", datosCliente.fcDepartamento + " " + datosCliente.fcMunicipio + "  " + datosCliente.fcBarrio + " " + datosCliente.fcDireccionDetallada);
            DictionaryList.Add("fcObservaciones", datosCliente.fcComentario);
            DictionaryList.Add("fcObservacionesTecnico", datosCliente.fcComentarioInstalacion);
            DictionaryList.Add("NombreTecnico", datosCliente.NombreTecnicoAsignado);
            DictionaryList.Add("HoraProgramada", datosCliente.fdFechaInstalacionInicio.ToString("dd/MM/yyyy HH:mm:ss"));
            DictionaryList.Add("fcHoraInicio", datosCliente.fdFechaInstalacionInicio.ToString("dd/MM/yyyy HH:mm:ss"));
            DictionaryList.Add("fcHoraFinal", datosCliente.fdFechaInstalacionFinal.ToString("dd/MM/yyyy HH:mm:ss"));
            DictionaryList.Add("TotalcuotaMensual", datosCliente.TotalcuotaMensual);
            DictionaryList.Add("fnCuotaMensualMonedaNacional", datosCliente.fnCuotaMensualMonedaNacional);


            string listaProductos = "No hay productos adicionados.";
            if (!string.IsNullOrWhiteSpace(datosCliente?.fcProductosAdicionados))
            {
                try
                {
                    var productos = JsonConvert.DeserializeObject<ProductosAdicionadosResponse>(datosCliente.fcProductosAdicionados);
                    if (productos?.ProductosAdicionados != null && productos.ProductosAdicionados.Any())
                    {
                        listaProductos = string.Join(Environment.NewLine, productos.ProductosAdicionados
                            .Select(p => $"Producto: {p.Producto}, Marca: {p.Marca}, Modelo: {p.Modelo}, Tipo Producto: {p.TipoProducto}, cantidad: {p.Cantidad}, Precio: {p.Precio}"));
                    }
                }
                catch (JsonException ex)
                {
                    listaProductos = $"Error al procesar los productos adicionados: {ex.Message}";
                }
            }

            DictionaryList.Add("fcProductosAdicionados", listaProductos);
            DictionaryList.Add("fcFechaFirmaContratoVigente", datosCliente.fcFechaFirmaContratoVigente.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-ES")));
            DictionaryList.Add("FechaAhora", DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-ES")));
            DictionaryList.Add("FechaAhoraFormateada", fechaFormateada);
            /*
            var Lista = _connection.OrionContext.sp_ListadoMaterialesInstalados_BySolicitudInstalacion(datosCliente.fiIDContratistaSolicitud).ToList();
            var DatosMaterialesInstalacion = new List<sp_ListadoMaterialesInstalados_BySolicitudInstalacion_Result>();            
            foreach (var item in Lista)
            {
                DatosMaterialesInstalacion.Add(new sp_ListadoMaterialesInstalados_BySolicitudInstalacion_Result
                {
                    fcProducto = item.fcProducto,
                    fnCantidad = item.fnCantidad,  
                });
            }

            DictionaryList.Add("fcProducto", string.Join(",", DatosMaterialesInstalacion.Select(x => x.fcProducto).ToList()));
            DictionaryList.Add("fnCantidad", string.Join(",", DatosMaterialesInstalacion.Select(x => x.fnCantidad.ToString()).ToList()));*/
            var materiales = _connection.OrionContext.sp_ListadoMaterialesInstalados_Articulos(datosCliente.fiIDContratistaSolicitud).FirstOrDefault();
            DictionaryList.Add("fcProductoMateriales", materiales);
           

            return DictionaryList;
        }

        public string ParseVariablesInEmailTemplate(string emailTemplate, Dictionary<string, string> variableValues)
        {

            var codeStartDelimiter = "{";
            var codeEndDelimiter = "}";

            var escapeCharacters = new[] { ".", "$", "{", "}", "[", "]", "^", "(", ")", "|", "*", "+", "?", @"\" };

            var delStartReg = $"{codeStartDelimiter}";
            var delEndReg = $"{codeEndDelimiter}";

            if (escapeCharacters.Contains(delStartReg)) delStartReg = $"\\{delStartReg}";
            if (escapeCharacters.Contains(delEndReg)) delEndReg = $"\\{delEndReg}";

            var regexp = new Regex(delStartReg + "[^" + delStartReg + delEndReg + "]*" + delEndReg);

            var matches = regexp.Matches(emailTemplate);

            foreach (Match match in matches)
            {
                var code = match.Value.Replace(codeStartDelimiter, "");
                code = code.Replace(codeEndDelimiter, "");

                var codeResult = variableValues[code];

                //try
                //{
                //    codeResult = _evaluateExpression.EvaluateCodeSnippet<string>(code).Result;
                //}
                //catch (Exception e)
                //{
                //    throw _exceptionFactory.GetException(25084, code);
                //}

                emailTemplate = emailTemplate.Replace(match.Value, codeResult);

            }

            return emailTemplate;

        }


        public SendEmailViewModel GenerateCustomerEmailToSend(EmailTemplateServiceModel model)
        {
            var emailTemplateModel = _connection.OrionContext.sp_OrionSolicitud_detallePlantillaCorreo(1, model.IdEmailTemplate).FirstOrDefault();

            Attachment fileAttachment = null;
            var CustomerAttachmentName = $"{model.IDSolicitud}_{emailTemplateModel.fcEmailAttachmentName}.pdf";
            if (!string.IsNullOrEmpty(emailTemplateModel.fcEmailAttachmentName))
            {
                pathToOpen = Path.Combine(HttpContext.Current.Server.MapPath(MemoryLoadManager.VirtualPathServerToEmailTemplates), emailTemplateModel.fcEmailAttachmentName);
                pathToSave = Path.Combine(HttpContext.Current.Server.MapPath(MemoryLoadManager.VirtualPathServerToCustomerAttachment), CustomerAttachmentName);

                var pdfDoc = GeneratePDFAttachment();
                fileAttachment = pdfDoc;

            }


            var BodyEmailParsed = ParseVariablesInEmailTemplate(emailTemplateModel.fcEmailBody, DictionaryList);
            var subjectEmailParsed = ParseVariablesInEmailTemplate(emailTemplateModel.fcEmailSubject, DictionaryList);
            //ConvertHtmlToPDF
            // var pdfBytes = new HtmlToPdfConverter().GeneratePdf(testBody);
            var HtmlPdfConverter = new HtmlToPdfConverter();
            //var body = ParseVariablesInEmailTemplate(model.HtmlBody, DictionaryList);
            var body = "Buenas Estimado: Contratista le adjuntamos la siguiente orden de trabajo";
            var pdfBytes = HtmlPdfConverter.GeneratePdf(body);
            var fileStream = new StreamReader(new MemoryStream(pdfBytes), System.Text.Encoding.Default, false);
            var file = new Attachment(fileStream.BaseStream, "ContractExtension.pdf");

            return new SendEmailViewModel
            {
                EmailName = emailTemplateModel.fcEmailTitle,
                Subject = subjectEmailParsed,
                Body = BodyEmailParsed,
                DestinationEmail = model.CustomerEmail,
                Attachment = fileAttachment,
                AttachmentName = CustomerAttachmentName,
                List_CC = model.List_CC
            };
        }


        public async Task<bool> SendEmailToCustomer(EmailTemplateServiceModel model)
        {
            try
            {
                DictionaryList = GenerateDictionaryOfVariables(model.IDSolicitud, model.IdCliente, model.IdTransaccion, model.IDSolicitudContratrista);

                var emailGeneratedToSend = GenerateCustomerEmailToSend(model);

                var SendEmailResult = await _emailService.SendEmailAsync(emailGeneratedToSend);

                //_connection.autoLoanContext.EmailSendCustomer.Add(new EmailSendCustomer
                //{
                //    fcIDLoan = model.IdLoan,
                //    fcIDCustomer = model.IdCustomer,
                //    fcTO = emailGeneratedToSend.DestinationEmail,
                //    fcFROM = _emailService.From_Email,
                //    fcSUBJECT = emailGeneratedToSend.Subject,
                //    fcBODY = emailGeneratedToSend.Body,
                //    fcAttachmentName = emailGeneratedToSend.AttachmentName,
                //    fcComentario = model.Comment ?? "",
                //    fdDateCreated = DateTime.Now,
                //    FcUserNameCreated = HttpContext.Current.User.Identity.Name
                //});

                // var customer = _connection.autoLoanContext.Customer_Master.FirstOrDefault(z => z.fcIDCustomer == model.IdCustomer);
                // if (customer != null) customer.fcCustomerEmail = model.CustomerEmail;

                //   _connection.autoLoanContext.SaveChanges();

                return SendEmailResult;
            }
            catch (Exception e)
            {
                //await _emailService.SendEmailException(e, "Send Email");
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }


        }

        public async Task<bool> SendEmailToSolicitud(EmailTemplateServiceModel model)
        {
            try
            {

                var contenidoCorreo = "<table style=\"width: 600px; border-collapse: collapse; border-width: 0; border-style: none; border-spacing: 0; padding: 0;\">" +
                                  $"<tr><th style='text-align:left;'>1) Geolocalizacion:</th> <td> {MemoryLoadManager.UrlWeb}/DatosCliente/ViewFormMapa/" + model.IDSolicitud + "</td></tr>" +
                                  $"<tr><th style='text-align:left;'>2) Contrato y Firma:</th> <td>{MemoryLoadManager.UrlContrato}?" + model.IDFirma + "</td></tr>" +
                                  $"<tr><th style='text-align:left;'>3) Toma de Fotos:</th> <td>{MemoryLoadManager.UrlWeb}/DatosCliente/ViewFormCargarIdentidad/" + model.IDSolicitud + "</td></tr>" +

                                  "</table>";

                var htmlString = @"<!DOCTYPE html> " +
                        "<html>" +
                        "<body>" +
                        "<div style=\"width: 300px;\"><label>Estimado cliente,</label></div>" +
                        "<br/> <div style=\"width: 600px;\"><label>Es un placer proporcionarle los enlaces a los formularios necesarios para solicitar nuestros servicios." +
                        "<br/>1)La Geolocalizacion la tiene que realizar cuando este en el lugar donde se realizara la instalacion de servicios y tiene que dar permisos para poder utilizar el GPS para poder georeferenciar" +
                        "<br/>2)En el link de contrato y firma necesitara el token que se le envio a su telefono personal para poder habilitar la firma digital de su contrato." +
                        "<br/>3)En la Toma de fotos tiene que tomar la foto delantera de su Identidad y una selfi y subir cuando se termine el proceso saldra un mensaje de que se subieron las fotos" +
                        "<br/>A continuación, encontrará acceso directo a los formularios:</label></div>" +
                        "<div style=\"width: 600px;\">" +
                        " <table style=\"width: 600px; border-collapse: collapse; border-width: 0; border-style: none; border-spacing: 0; padding: 0;\">" +
                        " <tr style=\"height: 30px; background-color:#F29E00; font-family: 'Microsoft Tai Le'; font-size: 14px; font-weight: bold; color: white;\">" +
                        " <td style=\"vertical-align: central; text-align:center;\">Formularios de Solicitud de Servicios</td>" +
                        " </tr>" +
                        " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                        " <td>&nbsp;</td>" +
                        " </tr>" +
                        " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                        " <td style=\"background-color:whitesmoke; text-align:center;\">LINK</td>" +
                        " </tr>" +
                        " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                        " <td>&nbsp;</td>" +
                        " </tr>" +
                        " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                        " <td style=\"vertical-align: central;\">" + contenidoCorreo + "</td>" +
                        " </tr>" +
                        " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                        " <td>&nbsp;</td>" +
                        " </tr>" +
                        " <tr style=\"height: 20px; font-family: 'Microsoft Tai Le'; font-size: 12px; text-align:center;\">" +
                        " <td>System Bot Prestadito</td>" +
                        " </tr>" +
                        " </table>" +
                        " </div>" +
                        "</body> " +
                        "</html> ";

                var emailGeneratedToSend = new SendEmailViewModel
                {
                    EmailName = "NovaNet",
                    Subject = "Formularios de Solicitud de Servicios",
                    Body = htmlString,
                    DestinationEmail = model.CustomerEmail,
                };

                var SendEmailResult = await _emailService.SendEmailAsync(emailGeneratedToSend);



                return SendEmailResult;
            }
            catch (Exception e)
            {
                //await _emailService.SendEmailException(e, "Send Email");
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }


        }


        public async Task<bool> SendEmailReporteDebito(List<ReporteDebitoAutomatico> clientes)
        {
            try
            {
                string fecha = DateTime.Now.ToString("dd/MM/yyyy");

                // Construcción de la tabla con los clientes
                var tabla = new StringBuilder();
                tabla.AppendLine("<table style='width: 100%; border-collapse: collapse;'>");
                tabla.AppendLine("<tr style='background-color: #F29E00; color: white; font-weight: bold;'>");
                tabla.AppendLine("<th style='padding: 8px; text-align: left;'>Nombre del Cliente</th>");
                tabla.AppendLine("<th style='padding: 8px; text-align: left;'>Monto Enrolado</th>");
                tabla.AppendLine("</tr>");

                foreach (var cliente in clientes)
                {
                    tabla.AppendLine("<tr style='background-color: #f9f9f9;'>");
                    tabla.AppendLine($"<td style='padding: 8px;'>{cliente.fcNombreCliente}</td>");
                    tabla.AppendLine($"<td style='padding: 8px;'>${cliente.fnMonto:F2}</td>");
                    tabla.AppendLine("</tr>");
                }

                tabla.AppendLine("</table>");

                // Cuerpo del correo
                string htmlString = $@"
                    <!DOCTYPE html>
                    <html>
                    <body style='font-family: Microsoft Tai Le, sans-serif; font-size: 14px;'>
                        <p>Estimados,</p>
                        <p>Por medio del presente, les comparto el detalle de los clientes que se han enrolado en el servicio de <strong>débito automático</strong> el día de hoy <strong>{fecha}</strong>:</p>
                        {tabla}
                        <p><strong>Total de clientes enrolados:</strong> {clientes.Count}</p>
                        <p>Este reporte corresponde únicamente a los registros generados durante la jornada. Cualquier novedad será comunicada oportunamente.</p>
                        <br/>
                        <p>Saludos cordiales,<br/>System Bot Prestadito</p>
                    </body>
                    </html>";

                var emailGeneratedToSend = new SendEmailViewModel
                {
                    EmailName = "NovaNet",
                    Subject = $"Reporte de Enrolamientos en Débito Automático – {fecha}",
                    Body = htmlString,
                    DestinationEmail = "kevin.santos@miprestadito.com" // <-- reemplazar por quien recibe
                };

                return await _emailService.SendEmailAsync(emailGeneratedToSend);
            }
            catch (Exception e)
            {
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel
                {
                    fcClase = "danger",
                    fcTipoTransaccion = "Error",
                    fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace
                });

                return false;
            }
        }



        public async Task<bool> SendEmailToSolicitudRenovacion(EmailTemplateServiceModel model)
        {
            try
            {

                var contenidoCorreo = "<table style=\"width: 600px; border-collapse: collapse; border-width: 0; border-style: none; border-spacing: 0; padding: 0;\">" +
                                  $"<tr><th style='text-align:left;'>1) Contrato y Firma:</th> <td>https://ptdto.com/docsRenovacion/Forms/SolitudFirma/SolicitudContratoFirma.aspx?" + model.IDSolicitud + "</td></tr>" +
                                  "</table>";

                var htmlString = @"<!DOCTYPE html> " +
                        "<html>" +
                        "<body>" +
                        "<div style=\"width: 300px;\"><label>Estimado cliente,</label></div>" +
                        "<br/> <div style=\"width: 600px;\"><label>Es un placer proporcionarle los enlaces a los formularios necesarios para solicitar nuestros servicios." +
                        "<br/>1)En el link de contrato y firma necesitara el token que se le envio a su telefono personal para poder habilitar la firma digital de su contrato." +
                        "<br/>A continuación, encontrará acceso directo a los formularios:</label></div>" +
                        "<div style=\"width: 600px;\">" +
                        " <table style=\"width: 600px; border-collapse: collapse; border-width: 0; border-style: none; border-spacing: 0; padding: 0;\">" +
                        " <tr style=\"height: 30px; background-color:#F29E00; font-family: 'Microsoft Tai Le'; font-size: 14px; font-weight: bold; color: white;\">" +
                        " <td style=\"vertical-align: central; text-align:center;\">Formularios de Solicitud de Servicios</td>" +
                        " </tr>" +
                        " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                        " <td>&nbsp;</td>" +
                        " </tr>" +
                        " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                        " <td style=\"background-color:whitesmoke; text-align:center;\">LINK</td>" +
                        " </tr>" +
                        " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                        " <td>&nbsp;</td>" +
                        " </tr>" +
                        " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                        " <td style=\"vertical-align: central;\">" + contenidoCorreo + "</td>" +
                        " </tr>" +
                        " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                        " <td>&nbsp;</td>" +
                        " </tr>" +
                        " <tr style=\"height: 20px; font-family: 'Microsoft Tai Le'; font-size: 12px; text-align:center;\">" +
                        " <td>System Bot Prestadito</td>" +
                        " </tr>" +
                        " </table>" +
                        " </div>" +
                        "</body> " +
                        "</html> ";

                var emailGeneratedToSend = new SendEmailViewModel
                {
                    EmailName = "NovaNet",
                    Subject = "Formularios de Solicitud de Servicios",
                    Body = htmlString,
                    DestinationEmail = model.CustomerEmail,
                };

                var SendEmailResult = await _emailService.SendEmailAsync(emailGeneratedToSend);



                return SendEmailResult;
            }
            catch (Exception e)
            {
                //await _emailService.SendEmailException(e, "Send Email");
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }


        }

        public async Task<bool> SendEmailToPassword(EmailTemplateNovanetModel model)
        {
            try
            {




                string htmlString = $@"
                                <html>
                                <head>
                                    <style>
                                        body {{ font-family: Arial, sans-serif; background-color: #f0f0f0; }}
                                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; background-color: #ffffff; }}
                                        .logo {{ text-align: center; font-size: 24px; color: #d9730d; margin-bottom: 20px; }}
                                        h1 {{ text-align: center; color: #333333; }}
                                        p {{ text-align: center; color: #666666; }}
                                        .button {{ display: block; width: 200px; margin: 20px auto; padding: 10px; background-color: #d9730d; color: #ffffff; text-align: center; text-decoration: none; border-radius: 5px; }}
                                        .footer {{ text-align: center; font-size: 12px; color: #999999; margin-top: 20px; }}
                                    </style>
                                </head>
                                <body>
                                    <div class='container'>
                                        <div class='logo'>Novanet</div>
                                        <h1>Cambio de Contraseña</h1>
                                        <p>A Continuacion se le mostrara su Nueva Contraseña</p>
                                        <span href='#' class='button'>{model.fcNuevaContrasenia}</span>
                                        <p class='footer'>
                                            Favor de Cambiar la contraseña una ves Acceda al sistema para evitar Problemas Futuros<br>
                                            Si usted no solicito este cambio de contraseña, favor de Avisar a Soporte 
                                        </p>
                                    </div>
                                </body>
                                </html>";



                var emailGeneratedToSend = new SendEmailViewModel
                {
                    EmailName = "Orion",
                    Subject = "Cambio de Contraseña",
                    Body = htmlString,
                    DestinationEmail = model.fcCorreoElectronico,
                };

                var SendEmailResult = await _emailService.SendEmailAsync(emailGeneratedToSend);



                return SendEmailResult;
            }
            catch (Exception e)
            {
                await _emailService.SendEmailException(e, "Send Email");

                return false;
            }


        }

        public async Task<bool> SendEmailToSolicitudFactura(EmailTemplateServiceModel model)
        {
            try
            {

                //var contenidoCorreo = "<table style=\"width: 600px; border-collapse: collapse; border-width: 0; border-style: none; border-spacing: 0; padding: 0;\">" +
                //                  $"<tr><th style='text-align:left;'>1) Factura:</th> <td> https://ptdto.com/dt/fac.aspx?"+model.IdTransaccion+" </td></tr>" +

                //                  "</table>";

                //var htmlString = @"<!DOCTYPE html> " +
                //        "<html>" +
                //        "<body>" +
                //        "<div style=\"width: 300px;\"><label>Estimado cliente,</label></div>" +
                //        "<br/> <div style=\"width: 600px;\"><label>Puede descargar su factura en el siguiente enlace." +
                //        "<div style=\"width: 600px;\">" +
                //        " <table style=\"width: 600px; border-collapse: collapse; border-width: 0; border-style: none; border-spacing: 0; padding: 0;\">" +
                //        " <tr style=\"height: 30px; background-color:#F29E00; font-family: 'Microsoft Tai Le'; font-size: 14px; font-weight: bold; color: white;\">" +
                //        " <td style=\"vertical-align: central; text-align:center;\">Factura</td>" +
                //        " </tr>" +
                //        " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                //        " <td>&nbsp;</td>" +
                //        " </tr>" +
                //        " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                //        " <td style=\"background-color:whitesmoke; text-align:center;\">Enlace: </td>" +
                //        " </tr>" +
                //        " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                //        " <td>&nbsp;</td>" +
                //        " </tr>" +
                //        " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                //        " <td style=\"vertical-align: central;\">" + contenidoCorreo + "</td>" +
                //        " </tr>" +
                //        " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                //        " <td>&nbsp;</td>" +
                //        " </tr>" +
                //        " <tr style=\"height: 20px; font-family: 'Microsoft Tai Le'; font-size: 12px; text-align:center;\">" +
                //        " <td>Novanet</td>" +
                //        " </tr>" +
                //        " </table>" +
                //        " </div>" +
                //        "</body> " +
                //        "</html> ";


                var htmlString = "<html lang='es'>" +
                                    "<head>" +
                                        "<meta charset='UTF-8'>" +
                                        "<meta name='viewport' content='width=device-width, initial-scale=1.0'>" +
                                        "<title>Descargar Factura</title>" +
                                        "<style>" +
                                            "body {" +
                                                "font-family: Arial, sans-serif;" +
                                                "background-color: #f4f4f4;" +
                                                "margin: 0;" +
                                                "padding: 0;" +
                                            "}" +
                                            ".container {" +
                                                "max-width: 600px;" +
                                                "margin: 0 auto;" +
                                                "background-color: #ffffff;" +
                                                "border: 1px solid #dddddd;" +
                                                "border-radius: 10px;" +
                                                "overflow: hidden;" +
                                                "box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);" +
                                            "}" +
                                            ".header {" +
                                                "background-color: #ff6600;" +
                                                "color: #ffffff;" +
                                                "padding: 20px;" +
                                                "text-align: center;" +
                                            "}" +
                                            ".header img {" +
                                                "max-width: 150px;" +
                                            "}" +
                                            ".content {" +
                                                "padding: 20px;" +
                                                "text-align: left;" +
                                                "color: #333333;" +
                                            "}" +
                                            ".content p {" +
                                                "font-size: 16px;" +
                                                "line-height: 1.5;" +
                                            "}" +
                                            ".content a {" +
                                                "display: inline-block;" +
                                                "margin-top: 20px;" +
                                                "padding: 10px 20px;" +
                                                "background-color: #ff6600;" +
                                                "color: #ffffff;" +
                                                "text-decoration: none;" +
                                                "border-radius: 5px;" +
                                                "font-size: 16px;" +
                                            "}" +
                                            ".footer {" +
                                                "text-align: center;" +
                                                "padding: 10px;" +
                                                "font-size: 12px;" +
                                                "color: #777777;" +
                                                "background-color: #333333;" +
                                                "color: #ffffff;" +
                                            "}" +
                                        "</style>" +
                                    "</head>" +
                                    "<body>" +
                                        "<div class='container'>" +
                                            "<div class='header'>" +
                                                "<img src='https://novanetgroup.com/wp/telnet/wp-content/img/logo_novanet.png' alt='NovaNet Logo'>" +
                                            "</div>" +
                                            "<div class='content'>" +
                                                "<p>Estimado cliente,</p>" +
                                                "<p>Gracias por su preferencia. Puede descargar su factura haciendo clic en el siguiente enlace:</p>" +
                                                "<a href='https://ptdto.com/dt/fac.aspx?" + model.IdTransaccion + "'>Descargar Factura</a>" +
                                            "</div>" +
                                            "<div class='footer'>" +
                                                "<p>© 2024 NovaNet. Todos los derechos reservados.</p>" +
                                            "</div>" +
                                        "</div>" +
                                    "</body>" +
                                    "</html>";


                var emailGeneratedToSend = new SendEmailViewModel
                {
                    EmailName = "NovaNet",
                    Subject = "Factura",
                    Body = htmlString,
                    DestinationEmail = model.CustomerEmail,
                };

                var SendEmailResult = await _emailService.SendEmailAsync(emailGeneratedToSend);



                return SendEmailResult;
            }
            catch (Exception e)
            {
                //await _emailService.SendEmailException(e, "Send Email");
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }


        }


        public async Task<bool> SendEmailPresonalizado(EmailTemplateServiceModel model)
        {
            try
            {

                var emailGeneratedToSend = new SendEmailViewModel
                {
                    EmailName = "NovaNet",
                    Subject = model.Comment,
                    Body = model.HtmlBody,
                    DestinationEmail = model.CustomerEmail,
                };

                var SendEmailResult = await _emailService.SendEmailAsync(emailGeneratedToSend);
                return SendEmailResult;
            }
            catch (Exception e)
            {
                //await _emailService.SendEmailException(e, "Send Email");
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }


        }


        public async Task<bool> SendEmailPresonalizadoAVarios(EmailTemplateServiceModel model)
        {
            try
            {

                var emailGeneratedToSend = new SendEmailViewModel
                {
                    EmailName = "NovaNet",
                    Subject = model.Comment,
                    Body = model.HtmlBody,
                    ListDestinationEmail = model.ListCustomerEmail,
                    List_CC = model.List_CC
                };

                var SendEmailResult = await _emailService.SendEmailManyDestinationsAsync(emailGeneratedToSend);
                return SendEmailResult;
            }
            catch (Exception e)
            {
                //await _emailService.SendEmailException(e, "Send Email");
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }


        }

        public async Task<bool> SendEmailToCustomerFirmado(EmailTemplateServiceModel model)
        {
            try
            {
                DictionaryList = GenerateDictionaryOfVariables(model.IDSolicitud, model.IdCliente, model.IdTransaccion, model.IDSolicitudContratrista);
                var datosCliente = _connection.OrionContext.sp_OrionContratista_DetalleBySolicitud(1, model.IDSolicitud,0,0).FirstOrDefault();
                DictionaryList.Add("fcFirma", datosCliente.fcFirma);
                var emailGeneratedToSend = GenerateCustomerEmailToSendFromado(model);

                var SendEmailResult = await _emailService.SendEmailAsync(emailGeneratedToSend);

                DictionaryList = GenerateDictionaryOfVariables(model.IDSolicitud, model.IdCliente , model.IdTransaccion, model.IDSolicitudContratrista);
                DictionaryList.Add("fcFirma", model.firma);
                emailGeneratedToSend = GenerateCustomerEmailToSendFromadoFirmado(model);

                SendEmailResult = await _emailService.SendEmailAsync(emailGeneratedToSend);

                return SendEmailResult;
            }
            catch (Exception e)
            {
                //await _emailService.SendEmailException(e, "Send Email");
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }


        }


        #region Adendum Firmado Adicion Equipos
       
        public async Task<bool> SendEmailToSolicitudFirmaAdicionEquipos(EmailTemplateServiceModel model)
        {
            try
            {

                string urlOriginal = $"https://orion.novanetgroup.com/Reportes/Adendum?solicitud={model.IDSolicitud}";
                string urlEncriptada = Convert.ToBase64String(Encoding.UTF8.GetBytes(urlOriginal));
                var contenidoCorreo = "<table style=\"width: 600px; border-collapse: collapse;\">" +
                                      $"<tr><th style='text-align:left;'>Adendum al Contrato:</th> " +
                                      $"<td><a href='https://orion.novanetgroup.com/Reportes/Adendum?solicitud={WebUtility.UrlEncode(urlEncriptada)}'>Ver Documento</a></td></tr>" +
                                      "</table>";

                var htmlString = @"<!DOCTYPE html> " +
                                 "<html>" +
                                 "<body>" +
                                 "<div style=\"width: 300px;\"><label>Estimado cliente,</label></div>" +
                                 "<br/> <div style=\"width: 600px;\"><label>Le informamos que, para continuar con el proceso de actualización de su servicio, es necesario que firme el ADENDUM AL CONTRATO DE SERVICIOS Y FINANCIAMIENTO." +
                                 "<br/> Para completar este proceso, deberá ingresar al enlace proporcionado para realizar su firma digital." +
                                 "<br/>A continuación, encontrará el acceso al documento:</label></div>" +
                                 "<div style=\"width: 600px;\">" +
                                 " <table style=\"width: 600px; border-collapse: collapse; border-width: 0; border-style: none; border-spacing: 0; padding: 0;\">" +
                                 " <tr style=\"height: 30px; background-color:#F29E00; font-family: 'Microsoft Tai Le'; font-size: 14px; font-weight: bold; color: white;\">" +
                                 " <td style=\"vertical-align: central; text-align:center;\">Adendum al Contrato de Servicios y Financiamiento</td>" +
                                 " </tr>" +
                                 " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                                 " <td>&nbsp;</td>" +
                                 " </tr>" +
                                 " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                                 " <td style=\"background-color:whitesmoke; text-align:center;\">ENLACE</td>" +
                                 " </tr>" +
                                 " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                                 " <td>&nbsp;</td>" +
                                 " </tr>" +
                                 " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                                 " <td style=\"vertical-align: central;\">" + contenidoCorreo + "</td>" +
                                 " </tr>" +
                                 " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                                 " <td>&nbsp;</td>" +
                                 " </tr>" +
                                 " <tr style=\"height: 20px; font-family: 'Microsoft Tai Le'; font-size: 12px; text-align:center;\">" +
                                 " <td>System Bot Prestadito</td>" +
                                 " </tr>" +
                                 " </table>" +
                                 " </div>" +
                                 "</body> " +
                                 "</html> ";

                var emailGeneratedToSend = new SendEmailViewModel
                {
                    EmailName = "NovaNet",
                    Subject = "Firma del Adendum al Contrato de Servicios y Financiamiento",
                    Body = htmlString,
                    DestinationEmail = model.CustomerEmail,
                };

                var SendEmailResult = await _emailService.SendEmailAsync(emailGeneratedToSend);

                return SendEmailResult;
            }
            catch (Exception e)
            {
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }
        }

        public async Task<bool> SendEmailToCustomerAdendumFirmado(EmailTemplateServiceModel model)
        {
            try
            {
                DictionaryList = GenerateDictionaryOfVariables(model.IDSolicitud, model.IdCliente, model.IdTransaccion, model.IDSolicitudContratrista);
                var datosCliente = _connection.OrionContext.sp_OrionContratista_DetalleBySolicitud(1, model.IDSolicitud, 0, 0).FirstOrDefault();
                DictionaryList.Add("fcFirma", datosCliente.fcFirma);//firma existende en la Db
                DictionaryList.Add("fcFirmaErick", datosCliente.fcFirmaErick);//firma existende en la Db
                var emailGeneratedToSend = GenerateCustomerEmailToSendAdendum(model);
                var SendEmailResult = await _emailService.SendEmailAsync(emailGeneratedToSend);


                DictionaryList = GenerateDictionaryOfVariables(model.IDSolicitud, model.IdCliente, model.IdTransaccion, model.IDSolicitudContratrista);
                DictionaryList.Add("fcFirmaErick", datosCliente.fcFirmaErick);//firma existende en la Db
                DictionaryList.Add("fcFirma", model.firma); //Firma digital
                emailGeneratedToSend = GenerateCustomerEmailToSendAdendumFirmado(model);

                SendEmailResult = await _emailService.SendEmailAsync(emailGeneratedToSend);

                return SendEmailResult;
            }
            catch (Exception e)
            {
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }

        }

        public SendEmailViewModel GenerateCustomerEmailToSendAdendum(EmailTemplateServiceModel model)
        {
            var emailTemplateModel = _connection.OrionContext.sp_OrionSolicitud_detallePlantillaCorreo(1, model.IdEmailTemplate).FirstOrDefault();

            Attachment fileAttachment = null;
            var CustomerAttachmentName = $"{model.IDSolicitud}_PlantillaAdendumContratodeServicioyFinanciamientoFirmado.pdf";
            emailTemplateModel.fcEmailAttachmentName = "PlantillaAdendumContratodeServicioyFinanciamientoFirmado.docx";
            if (!string.IsNullOrEmpty(emailTemplateModel.fcEmailAttachmentName))
            {
                pathToOpen = Path.Combine(HttpContext.Current.Server.MapPath(MemoryLoadManager.VirtualPathServerToEmailTemplates), "PlantillaAdendumContratodeServicioyFinanciamientoFirmado.docx");
                pathToSave = Path.Combine(HttpContext.Current.Server.MapPath("~/Solicitudes/Solicitud_" + model.IDSolicitud + "/"), CustomerAttachmentName);
               
                var pdfDoc = GeneratePDFAttachmentAdendumFirmado();
                fileAttachment = pdfDoc;
                using (var context = new ORIONDBEntities())
                {
                   context.sp_Crear_DocumentosOrion(281, model.IDSolicitud, CustomerAttachmentName.Replace(".pdf", ""), ".pdf", pathToSave, HttpContext.Current.Server.MapPath("~/Solicitudes/Solicitud_" + model.IDSolicitud + "/"), 3, "Adendum Firmado", model.IDSolicitud);
                }
            }

            var BodyEmailParsed = ParseVariablesInEmailTemplate(emailTemplateModel.fcEmailBody, DictionaryList);
            var subjectEmailParsed = ParseVariablesInEmailTemplate(emailTemplateModel.fcEmailSubject, DictionaryList);
            var HtmlPdfConverter = new HtmlToPdfConverter();
            var body = "Buenas Estimado: Cliente le adjuntamos el Adendum de la Solicitud de Adición de Equipo ya firmado";
            var pdfBytes = HtmlPdfConverter.GeneratePdf(body);
            var fileStream = new StreamReader(new MemoryStream(pdfBytes), System.Text.Encoding.Default, false);
            var file = new Attachment(fileStream.BaseStream, "ContractExtensionFirmado.pdf");

            return new SendEmailViewModel
            {
                EmailName = emailTemplateModel.fcEmailTitle,
                Subject = subjectEmailParsed,
                Body = body,
                DestinationEmail = model.CustomerEmail,
                Attachment = fileAttachment,
                AttachmentName = CustomerAttachmentName,
                firma = model.firma
            };
        }

        public SendEmailViewModel GenerateCustomerEmailToSendAdendumFirmado(EmailTemplateServiceModel model)
        {
            var emailTemplateModel = _connection.OrionContext.sp_OrionSolicitud_detallePlantillaCorreo(1, model.IdEmailTemplate).FirstOrDefault();

            Attachment fileAttachment = null;
            var CustomerAttachmentName = $"{model.IDSolicitud}_PlantillaAdendumContratodeServicioyFinanciamientoFirmadoTerceros.pdf";
            emailTemplateModel.fcEmailAttachmentName = "PlantillaAdendumContratodeServicioyFinanciamientoFirmadoTerceros.docx";
            if (!string.IsNullOrEmpty(emailTemplateModel.fcEmailAttachmentName))
            {
                pathToOpen = Path.Combine(HttpContext.Current.Server.MapPath(MemoryLoadManager.VirtualPathServerToEmailTemplates), "PlantillaAdendumContratodeServicioyFinanciamientoFirmadoTerceros.docx");
                pathToSave = Path.Combine(HttpContext.Current.Server.MapPath("~/Solicitudes/Solicitud_" + model.IDSolicitud + "/"), CustomerAttachmentName);


                var pdfDoc = GeneratePDFAttachmentAdendumFirmado();
                fileAttachment = pdfDoc;

                using (var context = new ORIONDBEntities())
                {

                   context.sp_Crear_DocumentosOrion(281, model.IDSolicitud, CustomerAttachmentName.Replace(".pdf", ""), ".pdf", pathToSave, HttpContext.Current.Server.MapPath("~/Solicitudes/Solicitud_" + model.IDSolicitud + "/"), 7, "Anexo Firmado Terceros", model.IDSolicitud);
                }
            }

            var BodyEmailParsed = ParseVariablesInEmailTemplate(emailTemplateModel.fcEmailBody, DictionaryList);
            var subjectEmailParsed = ParseVariablesInEmailTemplate(emailTemplateModel.fcEmailSubject, DictionaryList);
            var HtmlPdfConverter = new HtmlToPdfConverter();
            var body = "Buenas Estimado: Cliente le adjuntamos el Adendum de la Solicitud de Adición de Equipo ya firmado";
            var pdfBytes = HtmlPdfConverter.GeneratePdf(body);
            var fileStream = new StreamReader(new MemoryStream(pdfBytes), System.Text.Encoding.Default, false);
            var file = new Attachment(fileStream.BaseStream, "ContratoExtensionResivido.pdf");

            return new SendEmailViewModel
            {
                EmailName = emailTemplateModel.fcEmailTitle,
                Subject = subjectEmailParsed,
                Body = body,
                DestinationEmail = model.CustomerEmail,
                Attachment = fileAttachment,
                AttachmentName = CustomerAttachmentName,
                List_CC = model.List_CC,
                firma = model.firma
            };
        }
        #endregion


        #region Generar documento orden de trabajo 

        public async Task<bool> SendEmailToCustomerFirmadoOrdenTrabajo(EmailTemplateServiceModel model)
        {
            try
            {
                //DictionaryList = GenerateDictionaryOfVariables(model.IDSolicitud, model.IdCliente, model.IdTransaccion, model.IDSolicitudContratrista);
                //var datosCliente = _connection.OrionContext.sp_OrionContratista_DetalleBySolicitud(1, model.IDSolicitud, 0, 0).FirstOrDefault();
                //DictionaryList.Add("fcFirma", datosCliente.fcFirma);
                //var emailGeneratedToSend = GenerateCustomerEmailToSendFromado(model);


                //var SendEmailResult = await _emailService.SendEmailAsync(emailGeneratedToSend);

                DictionaryList = GenerateDictionaryOfVariables(model.IDSolicitud, model.IdCliente, model.IdTransaccion, model.IDSolicitudContratrista);
                DictionaryList.Add("fcFirma", model.firma);

                var emailGeneratedToSend = GenerateCustomerEmailToSendFromadoFirmadoOrdenTRabajo(model);
                var SendEmailResult = await _emailService.SendEmailAsync(emailGeneratedToSend);

                return SendEmailResult;
            }
            catch (Exception e)
            {
                //await _emailService.SendEmailException(e, "Send Email");
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }


        }

        public SendEmailViewModel GenerateCustomerEmailToSendFromadoFirmadoOrdenTRabajo(EmailTemplateServiceModel model)
        {
            var emailTemplateModel = _connection.OrionContext.sp_OrionSolicitud_detallePlantillaCorreo(1, model.IdEmailTemplate).FirstOrDefault();

            Attachment fileAttachment = null;
            var CustomerAttachmentName = $"{model.IDSolicitud}_PlantillaOrdenTrabajoIncidente.pdf";
            emailTemplateModel.fcEmailAttachmentName = "PlantillaOrdenTrabajoIncidente.docx";
            if (!string.IsNullOrEmpty(emailTemplateModel.fcEmailAttachmentName))
            {
                pathToOpen = Path.Combine(HttpContext.Current.Server.MapPath(MemoryLoadManager.VirtualPathServerToEmailTemplates), "PlantillaOrdenTrabajoIncidente.docx");
                pathToSave = Path.Combine(HttpContext.Current.Server.MapPath("~/Solicitudes/Solicitud_" + model.IDSolicitud + "/"), CustomerAttachmentName);


                var pdfDoc = GeneratePDFAttachmentFrimadoRevisado();
                fileAttachment = pdfDoc;

                //using (var connection = (new ORIONDBEntities()).Database.Connection)
                //{

                //    connection.Open();
                //    var command = connection.CreateCommand();
                //    command.CommandText = $"EXEC sp_OrionSolicitud_PasoFinal 281, {model.IDSolicitud}, '{CustomerAttachmentName.Replace(".pdf", "")}', '.pdf', '{pathToSave}', '{HttpContext.Current.Server.MapPath("~/Solicitudes/Solicitud_" + model.IDSolicitud + "/")}', 'Anexo Resivido', {model.IDSolicitud}";
                //    using (var reader = command.ExecuteReader())
                //    {

                //    }

                //    connection.Close();
                //}

                using (var context = new ORIONDBEntities())
                {


                    //context.sp_OrionSolicitud_PasoFinal(281, model.IDSolicitud, CustomerAttachmentName.Replace(".pdf", ""), ".pdf", pathToSave, HttpContext.Current.Server.MapPath("~/Solicitudes/Solicitud_" + model.IDSolicitud + "/"), "Anexo Resivido", model.IDSolicitud);
                    //context.sp_Crear_DocumentosOrion(281, model.IDSolicitud, CustomerAttachmentName.Replace(".pdf", ""), ".pdf", pathToSave, HttpContext.Current.Server.MapPath("~/Solicitudes/Solicitud_" + model.IDSolicitud + "/"), 7, "Anexo Resivido", model.IDSolicitud);
                }
            }

            var BodyEmailParsed = ParseVariablesInEmailTemplate(emailTemplateModel.fcEmailBody, DictionaryList);
            var subjectEmailParsed = ParseVariablesInEmailTemplate(emailTemplateModel.fcEmailSubject, DictionaryList);
            //ConvertHtmlToPDF
            // var pdfBytes = new HtmlToPdfConverter().GeneratePdf(testBody);
            var HtmlPdfConverter = new HtmlToPdfConverter();
            //var body = ParseVariablesInEmailTemplate(model.HtmlBody, DictionaryList);
            var body = "Buenas Estimado: Cliente le adjuntamos el Anexo de la orden de trabajo ya firmado";
            var pdfBytes = HtmlPdfConverter.GeneratePdf(body);
            var fileStream = new StreamReader(new MemoryStream(pdfBytes), System.Text.Encoding.Default, false);
            var file = new Attachment(fileStream.BaseStream, "ContratoExtensionResivido.pdf");

            return new SendEmailViewModel
            {
                EmailName = emailTemplateModel.fcEmailTitle,
                Subject = subjectEmailParsed,
                Body = BodyEmailParsed,
                DestinationEmail = model.CustomerEmail,
                Attachment = fileAttachment,
                AttachmentName = CustomerAttachmentName,
                List_CC = model.List_CC,
                firma = model.firma
            };
        }


        #endregion




        public SendEmailViewModel GenerateCustomerEmailToSendFromado(EmailTemplateServiceModel model)
        {
            var emailTemplateModel = _connection.OrionContext.sp_OrionSolicitud_detallePlantillaCorreo(1, model.IdEmailTemplate).FirstOrDefault();

            Attachment fileAttachment = null;
            var CustomerAttachmentName = $"{model.IDSolicitud}_PlantillaOrdenTrabajoFirmado.pdf";
            emailTemplateModel.fcEmailAttachmentName = "PlantillaOrdenTrabajoFirmado.docx";
            if (!string.IsNullOrEmpty(emailTemplateModel.fcEmailAttachmentName))
            {
                pathToOpen = Path.Combine(HttpContext.Current.Server.MapPath(MemoryLoadManager.VirtualPathServerToEmailTemplates), "PlantillaOrdenTrabajoFirmado.docx");
                pathToSave = Path.Combine(HttpContext.Current.Server.MapPath("~/Solicitudes/Solicitud_" + model.IDSolicitud + "/"), CustomerAttachmentName);

                var pdfDoc = GeneratePDFAttachmentFrimado();
                fileAttachment = pdfDoc;
                using (var context = new ORIONDBEntities())
                {
                    context.sp_Crear_DocumentosOrion(281, model.IDSolicitud, CustomerAttachmentName.Replace(".pdf", ""), ".pdf", pathToSave, HttpContext.Current.Server.MapPath("~/Solicitudes/Solicitud_" + model.IDSolicitud + "/"), 3, "Anexo Firmado", model.IDSolicitud);
                }
            }

            var BodyEmailParsed = ParseVariablesInEmailTemplate(emailTemplateModel.fcEmailBody, DictionaryList);
            var subjectEmailParsed = ParseVariablesInEmailTemplate(emailTemplateModel.fcEmailSubject, DictionaryList);
            //ConvertHtmlToPDF
            // var pdfBytes = new HtmlToPdfConverter().GeneratePdf(testBody);
            var HtmlPdfConverter = new HtmlToPdfConverter();
            //var body = ParseVariablesInEmailTemplate(model.HtmlBody, DictionaryList);
            var body = "Buenas Estimado: Cliente le adjuntamos el Anexo de la orden de trabajo ya firmado";
            var pdfBytes = HtmlPdfConverter.GeneratePdf(body);
            var fileStream = new StreamReader(new MemoryStream(pdfBytes), System.Text.Encoding.Default, false);
            var file = new Attachment(fileStream.BaseStream, "ContractExtensionFirmado.pdf");

            return new SendEmailViewModel
            {
                EmailName = emailTemplateModel.fcEmailTitle,
                Subject = subjectEmailParsed,
                Body = BodyEmailParsed,
                DestinationEmail = model.CustomerEmail,
                Attachment = fileAttachment, 
                AttachmentName = CustomerAttachmentName,
                firma = model.firma
            };
        }

        public SendEmailViewModel GenerateCustomerEmailToSendFromadoFirmado(EmailTemplateServiceModel model)
        {
            var emailTemplateModel = _connection.OrionContext.sp_OrionSolicitud_detallePlantillaCorreo(1, model.IdEmailTemplate).FirstOrDefault();

            Attachment fileAttachment = null;
            var CustomerAttachmentName = $"{model.IDSolicitud}_PlantillaOrdenTrabajoFirmadoResivido.pdf";
            emailTemplateModel.fcEmailAttachmentName = "PlantillaOrdenTrabajoFirmadoResivido.docx";
            if (!string.IsNullOrEmpty(emailTemplateModel.fcEmailAttachmentName))
            {
                pathToOpen = Path.Combine(HttpContext.Current.Server.MapPath(MemoryLoadManager.VirtualPathServerToEmailTemplates), "PlantillaOrdenTrabajoFirmadoResivido.docx");
                pathToSave = Path.Combine(HttpContext.Current.Server.MapPath("~/Solicitudes/Solicitud_" + model.IDSolicitud + "/"), CustomerAttachmentName);


                var pdfDoc = GeneratePDFAttachmentFrimadoRevisado();
                fileAttachment = pdfDoc;

                //using (var connection = (new ORIONDBEntities()).Database.Connection)
                //{

                //    connection.Open();
                //    var command = connection.CreateCommand();
                //    command.CommandText = $"EXEC sp_OrionSolicitud_PasoFinal 281, {model.IDSolicitud}, '{CustomerAttachmentName.Replace(".pdf", "")}', '.pdf', '{pathToSave}', '{HttpContext.Current.Server.MapPath("~/Solicitudes/Solicitud_" + model.IDSolicitud + "/")}', 'Anexo Resivido', {model.IDSolicitud}";
                //    using (var reader = command.ExecuteReader())
                //    {

                //    }

                //    connection.Close();
                //}

                using (var context = new ORIONDBEntities())
                {


                    //context.sp_OrionSolicitud_PasoFinal(281, model.IDSolicitud, CustomerAttachmentName.Replace(".pdf", ""), ".pdf", pathToSave, HttpContext.Current.Server.MapPath("~/Solicitudes/Solicitud_" + model.IDSolicitud + "/"), "Anexo Resivido", model.IDSolicitud);
                    context.sp_Crear_DocumentosOrion(281, model.IDSolicitud, CustomerAttachmentName.Replace(".pdf", ""), ".pdf", pathToSave, HttpContext.Current.Server.MapPath("~/Solicitudes/Solicitud_" + model.IDSolicitud + "/"), 7, "Anexo Resivido", model.IDSolicitud);
                }
            }

            var BodyEmailParsed = ParseVariablesInEmailTemplate(emailTemplateModel.fcEmailBody, DictionaryList);
            var subjectEmailParsed = ParseVariablesInEmailTemplate(emailTemplateModel.fcEmailSubject, DictionaryList);
            //ConvertHtmlToPDF
            // var pdfBytes = new HtmlToPdfConverter().GeneratePdf(testBody);
            var HtmlPdfConverter = new HtmlToPdfConverter();
            //var body = ParseVariablesInEmailTemplate(model.HtmlBody, DictionaryList);
            var body = "Buenas Estimado: Cliente le adjuntamos el Anexo de la orden de trabajo ya firmado";
            var pdfBytes = HtmlPdfConverter.GeneratePdf(body);
            var fileStream = new StreamReader(new MemoryStream(pdfBytes), System.Text.Encoding.Default, false);
            var file = new Attachment(fileStream.BaseStream, "ContratoExtensionResivido.pdf");

            return new SendEmailViewModel
            {
                EmailName = emailTemplateModel.fcEmailTitle,
                Subject = subjectEmailParsed,
                Body = BodyEmailParsed,
                DestinationEmail = model.CustomerEmail,
                Attachment = fileAttachment,
                AttachmentName = CustomerAttachmentName,
                List_CC = model.List_CC,
                firma = model.firma
            };
        }

        

        private Dictionary<string, string> GenerateDictionaryOfVariablesReciboCliente(int fiIDTransaccion)
        {

            var ci = new CultureInfo("es-ES");

            using (var contexto = new CoreFinancieroEntities2())
            {
                var modelDb = contexto.sp_OperacionesCaja_ConsultarAbono(fiIDTransaccion, 0)?.FirstOrDefault() ?? null;


                DictionaryList.Clear();
                DictionaryList.Add("txtAgencia", modelDb.fcNombreAgencia);
                DictionaryList.Add("txtreferencia", modelDb.fiIDTransaccion.ToString());
                DictionaryList.Add("txtTransaccion", modelDb.fdFechaTransaccion.ToString("dd/MM/yyyy hh:mm tt"));
                DictionaryList.Add("txtCliente", modelDb.fcNombreCliente);
                DictionaryList.Add("txtPrestamo", modelDb.fcIDPrestamo);
                DictionaryList.Add("txtProducto", modelDb.fcProducto);
                DictionaryList.Add("txtMoneda", modelDb.fcMoneda);
                DictionaryList.Add("txtVencimiento", Convert.ToDateTime(modelDb.fdFechaVencimiento).ToString("dd/MM/yyyy"));
                DictionaryList.Add("txtFrecuencia", modelDb.fcFrecuencia);
                DictionaryList.Add("txtcuota", modelDb.fcSimboloMoneda + " " + Convert.ToDecimal(modelDb.fnValorCuota).ToString("n"));
                DictionaryList.Add("txtTotal", modelDb.fcSimboloMoneda + " " + Convert.ToDecimal(modelDb.fnTotalAbonado).ToString("n"));
                DictionaryList.Add("txtCuotas", modelDb.fcAvancedeCuotas);
                DictionaryList.Add("txtCajero", modelDb.fcUsuarioDominio);

                return DictionaryList;
            }

        }

        public SendEmailViewModel GenerarCorreoDeReciboDePago(EmailTemplateServiceModel model)
        {
            DictionaryList = GenerateDictionaryOfVariablesReciboCliente(model.IdTransaccion);
            using (var recibo = new StreamReader(System.IO.Path.Combine(MemoryLoadManager.URL, @"Documento\Recursos\Recibos\Recibo.html")))
            {


                var texto = ParseVariablesInEmailTemplate(recibo.ReadToEnd(), DictionaryList);
                Attachment fileAttachment = null;
                var CustomerAttachmentName = $"Recibo_{DictionaryList["txtCliente"]}.pdf";
                var reciboRpt = new ReportesTemplateService();
                var fileStream = new StreamReader(reciboRpt.GenerarRecibo(model.IdTransaccion), System.Text.Encoding.Default, false);
                var file = new Attachment(fileStream.BaseStream, CustomerAttachmentName);
                return new SendEmailViewModel
                {
                    EmailName = "Recibo Novanet",
                    Subject = "Recibo",
                    Body = texto,
                    DestinationEmail = "feduardopo21@gmail.com",
                    Attachment = file,
                    AttachmentName = CustomerAttachmentName,
                    List_CC = new List<string>(),
                    firma = ""
                };
            }
        }





        public async Task<bool> SendEmailToRecibo(EmailTemplateServiceModel model)
        {
            try
            {
                var emailGeneratedToSend = GenerarCorreoDeReciboDePago(model);
                var SendEmailResult = await _emailService.SendEmailAsync(emailGeneratedToSend);
                return SendEmailResult;
            }
            catch (Exception e)
            {
                //await _emailService.SendEmailException(e, "Send Email");
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }


        }


        public SendEmailViewModel GenerarCorreoContratoPagare(EmailTemplateServiceModel model)
        {

            var comandList = $"sp_OrionSolicitudes_InformacionDocumentacion {model.IDSolicitud}, 0, 1";
            var datosCliente = _connection.LoadListDataWithDbContext<Models.Reportes.sp_OrionSolicitudes_InformacionDocumentacion_ResultV2>(comandList).FirstOrDefault();
            Attachment fileAttachment = null;

            var contratoTemplate = new ContratoTemplateService();

            var attachments = new List<ListAttachmentViewModel>();

            var CustomerAttachmentNameContrato = $"Contrato_Solicitud{model.IDSolicitud}_Identidad{datosCliente.fcIdentidad}.pdf";
            var fileStreamContrato = new StreamReader(contratoTemplate.GenerarContrato(model.IDSolicitud, 0), System.Text.Encoding.Default, false);
            var fileContrato = new Attachment(fileStreamContrato.BaseStream, CustomerAttachmentNameContrato);
            attachments.Add(new ListAttachmentViewModel { Archivo = fileContrato, AttachmentName = CustomerAttachmentNameContrato });

            var CustomerAttachmentNamePagare = $"Pagare_Solicitud{model.IDSolicitud}_Identidad{datosCliente.fcIdentidad}.pdf";
            var fileStreamPagare = new StreamReader(contratoTemplate.GenerarPagare(model.IDSolicitud, 0), System.Text.Encoding.Default, false);
            var filePagare = new Attachment(fileStreamPagare.BaseStream, CustomerAttachmentNamePagare);
            attachments.Add(new ListAttachmentViewModel { Archivo = filePagare, AttachmentName = CustomerAttachmentNamePagare });
            var modelCorreo = new SendEmailViewModel
            {
                EmailName = "Documentos NOVANET",
                Subject = "Contrato y Pagare Firmados",
                Body = "SE ADJUNTA LOS ARCHIVOS DEL CONTRATO Y PAGARE FIRMADOS",
                DestinationEmail = model.CustomerEmail,

                List_CC = new List<string>(),
                firma = "",
                ArchivosVarios = new List<ListAttachmentViewModel>()
            };

            modelCorreo.ArchivosVarios.AddRange(attachments);
            return modelCorreo;

        }

        public async Task<bool> SendEmailToContratoPagare(EmailTemplateServiceModel model)
        {
            try
            {
                var emailGeneratedToSend = GenerarCorreoContratoPagare(model);
                var SendEmailResult = await _emailService.SendEmailManyAttachmentAsync(emailGeneratedToSend);
                return SendEmailResult;
            }
            catch (Exception e)
            {
                //await _emailService.SendEmailException(e, "Send Email");
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }


        }



        public SendEmailViewModel GenerarCorreoSalidaInventario(EmailTemplateServiceModel model)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Inventario_Movimiento_Maestro_GetById(model.IdTransaccion).FirstOrDefault();


                Attachment fileAttachment = null;

                var contratoTemplate = new ReportesTemplateService();


                var CustomerAttachmentName = $"Salida de inventario {modelDb.fdFechaCreacion:dd-mm-yyyy}.pdf";
                var fileStreamInventario = new StreamReader(contratoTemplate.GenerarSalidaInventario(model.IdTransaccion), System.Text.Encoding.Default, false);
                fileAttachment = new Attachment(fileStreamInventario.BaseStream, CustomerAttachmentName);

                var modelCorreo = new SendEmailViewModel
                {
                    EmailName = "Documentos NOVANET",
                    Subject = "Entrega de Inventario",
                    Body = $"Entrega de Inventario a {modelDb.fcDestinatario}",
                    DestinationEmail = model.CustomerEmail,

                    List_CC = model.List_CC,
                    firma = "",
                    Attachment = fileAttachment
                };

                //modelCorreo.ArchivosVarios.AddRange(attachments);
                return modelCorreo;
            }


        }


        public async Task<bool> SendEmailToSalidaInventario(EmailTemplateServiceModel model)
        {
            try
            {
                var emailGeneratedToSend = GenerarCorreoSalidaInventario(model);
                var SendEmailResult = await _emailService.SendEmailAsync(emailGeneratedToSend);
                return SendEmailResult;
            }
            catch (Exception e)
            {
                //await _emailService.SendEmailException(e, "Send Email");
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }


        }


        public SendEmailViewModel GenerarCorreoGerencia(EmailTemplateServiceModel model)
        {

            Attachment fileAttachment = null;

            var CustomerAttachmentName = $"Reporte Gerencia Novanet del  {DateTime.Now:dd-mm-yyyy}.pdf";
            var fileStreamInventario = new StreamReader(model.Archivo.InputStream, System.Text.Encoding.Default, false);
            fileAttachment = new Attachment(fileStreamInventario.BaseStream, CustomerAttachmentName);

            var modelCorreo = new SendEmailViewModel
            {
                EmailName = "Reporte Gerencial",
                Subject = "Reporte Dia General Novanet",
                Body = $"Reporte de Gerencia ",
                ListDestinationEmail = model.ListCustomerEmail,

                List_CC = new List<string>(),
                firma = "",
                Attachment = fileAttachment
            };

            return modelCorreo;



        }


        public async Task<bool> SendEmailToGerencia(EmailTemplateServiceModel model)
        {
            try
            {
                var emailGeneratedToSend = GenerarCorreoGerencia(model);
                var SendEmailResult = await _emailService.SendEmailManyDestinationsAsync(emailGeneratedToSend);
                return SendEmailResult;
            }
            catch (Exception e)
            {
                //await _emailService.SendEmailException(e, "Send Email");
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }


        }

        public SendEmailViewModel GenerarCorreoDeVenta(EmailTemplateServiceModel model)
        {

            var baseController = new BaseController();
            var bodega = baseController.GetConfiguracion<string>("CorreoDeBodega");
            var reciboRpt = new ReportesTemplateService();
            var modelo = reciboRpt.InformacionDeFactura(model.IdTransaccion);
            var CustomerAttachmentName = $"Factura_{modelo.fcReferenciaDocumento}.pdf";
            var fileStream = new StreamReader(reciboRpt.GenerarFacturaDeVenta(model.IdTransaccion), System.Text.Encoding.Default, false);
            var file = new Attachment(fileStream.BaseStream, CustomerAttachmentName);
            var emailCC = new List<string>();

            emailCC.Add("angel.bautista@miprestadito.com");
            emailCC.Add("eduardo.perez@miprestadito.com");


            var body = $"Preparar salida de inventario para la factura #{modelo.fcReferenciaDocumento} del cliente {modelo.fcNombreCliente}<br/>";
            body += $"Los siguientes Productos: <br/>";
            foreach(var item in modelo.DetalleFacturacion)
            {
                body += $"- {item.fnCantidad:0.##} {item.fcProducto}<br/>";
            }

            return new SendEmailViewModel
            {
                EmailName = "Venta de Inventario Novanet",
                Subject = $"Factura #{modelo.fcReferenciaDocumento}",
                Body = body,
                DestinationEmail = bodega,
                Attachment = file,
                AttachmentName = CustomerAttachmentName,
                List_CC = emailCC,
                firma = ""
            };
        }


        public async Task<bool> SendEmailVenta(EmailTemplateServiceModel model)
        {
            try
            {
                var emailGeneratedToSend = GenerarCorreoDeVenta(model);
                var SendEmailResult = await _emailService.SendEmailAsync(emailGeneratedToSend);
                return SendEmailResult;
            }
            catch (Exception e)
            {
                //await _emailService.SendEmailException(e, "Send Email");
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }
        }

        #region vendedor
        public async Task<bool> SendEmailToAcuerdoColaboracionVendedor(EmailTemplateServiceModel model)
        {
            try
            {

               
                var contenidoCorreo = "<table style=\"width: 600px; border-collapse: collapse;\">" +
                                      $"<tr><th style='text-align:left;'>Firma documento acuerdo de colaboración con Novanet:</th> " +
                                      $"<td><a href='https://orion.novanetgroup.com/Reportes/FirmaAcuerdoVendedor?id={model.fiIDVendedorRepartidor}'>Ver Documento</a></td></tr>" +
                                      "</table>";

                var htmlString = @"<!DOCTYPE html> " +
                                "<html>" +
                                "<body>" +
                                "<div style=\"width: 300px;\"><label>Estimado(a) " +  model.IdCustomer + ",</label></div>" +
                                "<br/> <div style=\"width: 600px;\"><label>Gracias por tu interés en formar parte de nuestro equipo de Agentes Externos de NovaNet. Para continuar con el proceso de colaboración, es necesario que firmes digitalmente el documento correspondiente." +
                                "<br/> Para ello, deberás ingresar al siguiente enlace, donde encontrarás el documento disponible para tu firma digital." +
                                "<br/>A continuación, te compartimos el acceso directo:</label></div>" +
                                "<div style=\"width: 600px;\">" +
                                " <table style=\"width: 600px; border-collapse: collapse; border-width: 0; border-style: none; border-spacing: 0; padding: 0;\">" +
                                " <tr style=\"height: 30px; background-color:#F29E00; font-family: 'Microsoft Tai Le'; font-size: 14px; font-weight: bold; color: white;\">" +
                                " <td style=\"vertical-align: central; text-align:center;\">Documento de colaboración</td>" +
                                " </tr>" +
                                " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                                " <td>&nbsp;</td>" +
                                " </tr>" +
                                " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                                " <td style=\"background-color:whitesmoke; text-align:center;\">ENLACE</td>" +
                                " </tr>" +
                                " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                                " <td>&nbsp;</td>" +
                                " </tr>" +
                                " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                                " <td style=\"vertical-align: central;\">" + contenidoCorreo + "</td>" +
                                " </tr>" +
                                " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                                " <td>&nbsp;</td>" +
                                " </tr>" +
                                " <tr style=\"height: 20px; font-family: 'Microsoft Tai Le'; font-size: 12px; text-align:center;\">" +
                                " <td>System Bot Prestadito</td>" +
                                " </tr>" +
                                " </table>" +
                                " </div>" +
                                "</body> " +
                                "</html> ";


                var emailGeneratedToSend = new SendEmailViewModel
                {
                    EmailName = "NovaNet",
                    Subject = "Firma de documento de colaboración con Novanet.",
                    Body = htmlString,
                    DestinationEmail = model.CustomerEmail,
                };

                var SendEmailResult = await _emailService.SendEmailAsync(emailGeneratedToSend);

                return SendEmailResult;
            }
            catch (Exception e)
            {
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }
        }


        public async Task<bool> SendEmailToVendedorDocumentoFirmado(EmailTemplateServiceModel model)
        {
            try
            {
                DictionaryList = GenerateDictionaryOfVariablesVendedores(model.fiIDVendedorRepartidor);
                var datosCliente = _connection.OrionContext.sp_OrionSolicitud_Repartidor_Listar(1).FirstOrDefault(x => x.fiIDVendedorRepartidor == model.fiIDVendedorRepartidor);
                DictionaryList.Add("fcFirma", model.firma);
                DictionaryList.Add("fcFirmaErick", datosCliente.fcFirmaErick);
                var emailGeneratedToSend = GenerateVendedorEmailToSendDoc(model);
                var SendEmailResult = await _emailService.SendEmailAsync(emailGeneratedToSend);

                return SendEmailResult;
            }
            catch (Exception e)
            {
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }

        }

        public SendEmailViewModel GenerateVendedorEmailToSendDoc(EmailTemplateServiceModel model)
        {
            var emailTemplateModel = _connection.OrionContext.sp_OrionSolicitud_detallePlantillaCorreo(1, model.IdEmailTemplate).FirstOrDefault();
            var datosCliente = _connection.OrionContext.sp_OrionSolicitud_Repartidor_Listar(1).FirstOrDefault(x=>x.fiIDVendedorRepartidor == model.fiIDVendedorRepartidor);
            var configuraciones = _baseController.GetConfigurationJson<ConfigDistribuidorVendedor>("Orion_Ventas_Vendedor_Distribuidor");
            Attachment fileAttachment = null;
            var CustomerAttachmentName = $"{model.fiIDVendedorRepartidor}_PlantillaAcuerdoColaboracionComercialAgenteExternoFirmado.pdf";
            emailTemplateModel.fcEmailAttachmentName = "PlantillaAcuerdoColaboracionComercialAgenteExternoFirmado.docx";
            if (!string.IsNullOrEmpty(emailTemplateModel.fcEmailAttachmentName))
            {
                pathToOpen = Path.Combine(HttpContext.Current.Server.MapPath(MemoryLoadManager.VirtualPathServerToEmailTemplates), "PlantillaAcuerdoColaboracionComercialAgenteExternoFirmado.docx");
                pathToSave = Path.Combine(HttpContext.Current.Server.MapPath("~/DocumentosVendedores/Vendedor_" + model.IdCustomer + "/"), CustomerAttachmentName);

                var pdfDoc = GeneratePDFAttachmentAdendumFirmado();
                fileAttachment = pdfDoc;
                using (var context = new ORIONDBEntities())
                {
                    foreach (var config in configuraciones)
                    {
                        context.sp_OrionVendedores_Documentos_Insertar(model.fiIDVendedorRepartidor, CustomerAttachmentName.Replace(".pdf", ""), ".pdf", pathToSave, HttpContext.Current.Server.MapPath("~/DocumentosVendedores/Vendedor_" + model.fiIDVendedorRepartidor + "/"), config.fiIDOrion_DocumentoVendedor, "Acuerdo de Colaboración con Novanet", _baseController.GetIdUser());
                        context.sp_Firmas_Orion_Vendedores_Insertar(model.fiIDVendedorRepartidor, model.firma);
                    }
                }
            }

            var BodyEmailParsed = ParseVariablesInEmailTemplate(emailTemplateModel.fcEmailBody, DictionaryList);
            var subjectEmailParsed = ParseVariablesInEmailTemplate(emailTemplateModel.fcEmailSubject, DictionaryList);
            var HtmlPdfConverter = new HtmlToPdfConverter();
            var body = $"Buenas estimado(a) {datosCliente.fcNombreVendedor}, le adjuntamos el documento ya firmado de colaboración con Novanet.";
            var pdfBytes = HtmlPdfConverter.GeneratePdf(body);
            var fileStream = new StreamReader(new MemoryStream(pdfBytes), System.Text.Encoding.Default, false);
            var file = new Attachment(fileStream.BaseStream, "ContractExtensionFirmado.pdf");

            return new SendEmailViewModel
            {
                EmailName = emailTemplateModel.fcEmailTitle,
                Subject = subjectEmailParsed,
                Body = body,
                DestinationEmail = model.CustomerEmail,
                Attachment = fileAttachment,
                AttachmentName = CustomerAttachmentName,
                firma = model.firma
            };
        }

        #endregion

        #region Distribuidor
        public async Task<bool> SendEmailToAcuerdoColaboracionDistribuidor(EmailTemplateServiceModel model)
        {
            try
            {


                var contenidoCorreo = "<table style=\"width: 600px; border-collapse: collapse;\">" +
                                      $"<tr><th style='text-align:left;'>Firma documento acuerdo de colaboración con Novanet:</th> " +
                                      $"<td><a href='https://orion.novanetgroup.com/Reportes/FirmaAcuerdoDistribuidor?id={model.fiIDDistribuidor}'>Ver Documento</a></td></tr>" +
                                      "</table>";

                var htmlString = @"<!DOCTYPE html> " +
                                "<html>" +
                                "<body>" +
                                "<div style=\"width: 300px;\"><label>Estimado(a) " + model.IdCustomer + ",</label></div>" +
                                "<br/> <div style=\"width: 600px;\"><label>Gracias por tu interés en formar parte de nuestro equipo de distribuidores de NovaNet. Para continuar con el proceso de colaboración, es necesario que firmes digitalmente el documento correspondiente." +
                                "<br/> Para ello, deberás ingresar al siguiente enlace, donde encontrarás el documento disponible para tu firma digital." +
                                "<br/>A continuación, te compartimos el acceso directo:</label></div>" +
                                "<div style=\"width: 600px;\">" +
                                " <table style=\"width: 600px; border-collapse: collapse; border-width: 0; border-style: none; border-spacing: 0; padding: 0;\">" +
                                " <tr style=\"height: 30px; background-color:#F29E00; font-family: 'Microsoft Tai Le'; font-size: 14px; font-weight: bold; color: white;\">" +
                                " <td style=\"vertical-align: central; text-align:center;\">Documento de colaboración</td>" +
                                " </tr>" +
                                " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                                " <td>&nbsp;</td>" +
                                " </tr>" +
                                " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                                " <td style=\"background-color:whitesmoke; text-align:center;\">ENLACE</td>" +
                                " </tr>" +
                                " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                                " <td>&nbsp;</td>" +
                                " </tr>" +
                                " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                                " <td style=\"vertical-align: central;\">" + contenidoCorreo + "</td>" +
                                " </tr>" +
                                " <tr style=\"height: 24px; font-family: 'Microsoft Tai Le'; font-size: 12px; font-weight: bold;\">" +
                                " <td>&nbsp;</td>" +
                                " </tr>" +
                                " <tr style=\"height: 20px; font-family: 'Microsoft Tai Le'; font-size: 12px; text-align:center;\">" +
                                " <td>System Bot Prestadito</td>" +
                                " </tr>" +
                                " </table>" +
                                " </div>" +
                                "</body> " +
                                "</html> ";


                var emailGeneratedToSend = new SendEmailViewModel
                {
                    EmailName = "NovaNet",
                    Subject = "Firma de documento de colaboración con Novanet.",
                    Body = htmlString,
                    DestinationEmail = model.CustomerEmail,
                };

                var SendEmailResult = await _emailService.SendEmailAsync(emailGeneratedToSend);

                return SendEmailResult;
            }
            catch (Exception e)
            {

                _baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }
        }


        public async Task<bool> SendEmailToDistribuidorDocumentoFirmado(EmailTemplateServiceModel model)
        {
            try
            {
                DictionaryList = GenerateDictionaryOfVariablesDistribuidor(model.fiIDDistribuidor);
                var data = _connection.OrionContext.Database.SqlQuery<ListDistribuidorViewModel>("Exec sp_Orion_Ventas_Distribuidor_Listado").Where(x => x.fiIDDistribuidor == model.fiIDDistribuidor).FirstOrDefault();
                DictionaryList.Add("fcFirma", model.firma);
                DictionaryList.Add("fcFirmaErick", data.fcFirmaErick);
                var emailGeneratedToSend = GenerateDistribuidorEmailToSendDoc(model);
                var SendEmailResult = await _emailService.SendEmailAsync(emailGeneratedToSend);

                return SendEmailResult;
            }
            catch (Exception e)
            {
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }

        }

        public SendEmailViewModel GenerateDistribuidorEmailToSendDoc(EmailTemplateServiceModel model)
        {
            var emailTemplateModel = _connection.OrionContext.sp_OrionSolicitud_detallePlantillaCorreo(1, model.IdEmailTemplate).FirstOrDefault();
            var data = _connection.OrionContext.Database.SqlQuery<ListDistribuidorViewModel>("Exec sp_Orion_Ventas_Distribuidor_Listado").FirstOrDefault(x => x.fiIDDistribuidor == model.fiIDDistribuidor);         
            var configuraciones = _baseController.GetConfigurationJson<ConfigDistribuidorVendedor>("Orion_Ventas_Vendedor_Distribuidor");

         
            Attachment fileAttachment = null;
            var CustomerAttachmentName = $"{model.fiIDDistribuidor}_PlantillaAcuerdoColaboracionComercialDistribuidorFirmado.pdf";
            emailTemplateModel.fcEmailAttachmentName = "PlantillaAcuerdoColaboracionComercialDistribuidorFirmado.docx";
            if (!string.IsNullOrEmpty(emailTemplateModel.fcEmailAttachmentName))
            {
                pathToOpen = Path.Combine(HttpContext.Current.Server.MapPath(MemoryLoadManager.VirtualPathServerToEmailTemplates), "PlantillaAcuerdoColaboracionComercialDistribuidorFirmado.docx");
                pathToSave = Path.Combine(HttpContext.Current.Server.MapPath("~/DocumentosDistribuidor/Distribuidor_" + model.IdCustomer + "/"), CustomerAttachmentName);

                var pdfDoc = GeneratePDFAttachmentAdendumFirmado();
                fileAttachment = pdfDoc;
                using (var context = new ORIONDBEntities())
                {
                    foreach (var config in configuraciones)
                    {
                        context.sp_OrionDistribuidor_Documentos_Insertar(model.fiIDDistribuidor, CustomerAttachmentName.Replace(".pdf", ""), ".pdf", pathToSave, HttpContext.Current.Server.MapPath("~/DocumentosDistribuidor/Distribuidor_" + model.fiIDDistribuidor + "/"), config.fiIDOrion_DocumentoDistribuidor, "Acuerdo de Colaboración con Novanet", _baseController.GetIdUser());
                        context.sp_Firmas_Orion_Distribuidores_Insertar(model.fiIDDistribuidor, model.firma);
                    }
                   
                }
            }

            var BodyEmailParsed = ParseVariablesInEmailTemplate(emailTemplateModel.fcEmailBody, DictionaryList);
            var subjectEmailParsed = ParseVariablesInEmailTemplate(emailTemplateModel.fcEmailSubject, DictionaryList);
            var HtmlPdfConverter = new HtmlToPdfConverter();
            var body = $"Buenas estimado(a) {data.NombreRepresentante}, le adjuntamos el documento ya firmado de colaboración con Novanet.";
            var pdfBytes = HtmlPdfConverter.GeneratePdf(body);
            var fileStream = new StreamReader(new MemoryStream(pdfBytes), System.Text.Encoding.Default, false);
            var file = new Attachment(fileStream.BaseStream, "ContractExtensionFirmado.pdf");

            return new SendEmailViewModel
            {
                EmailName = emailTemplateModel.fcEmailTitle,
                Subject = subjectEmailParsed,
                Body = body,
                DestinationEmail = model.CustomerEmail,
                Attachment = fileAttachment,
                AttachmentName = CustomerAttachmentName,
                firma = model.firma
            };
        }

        #endregion

        #region Inventario
        ///INGRESO DE INVENTARIO
        private Dictionary<string, string> GenerateDictionaryOfVariablesIngresoInventario(int fiIDMovimientoMaestro, GuardarMovimientoInventarioViewModel inventarioModificado = null)
        {

            var ci = new CultureInfo("es-ES");

            using (var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Producto_Movimiento_Maestro_GetById(fiIDMovimientoMaestro)?.FirstOrDefault() ?? null;


                DictionaryList.Clear();
               
                if(inventarioModificado == null)
                {
                    DictionaryList.Add("txtFactura", modelDb.fcNumeroFactura);
                    DictionaryList.Add("txtFecha", modelDb.fdFechaFactura.ToString("dd/MM/yyyy"));
                    DictionaryList.Add("txtProveedor", modelDb.fcReferenciaFactura);
                    DictionaryList.Add("txtUbicacion", modelDb.fcUbicacion);
                    DictionaryList.Add("txtDescripcion", modelDb.fcDescripcionFactura);
                    DictionaryList.Add("rowsProductos", DetalleInventarioIngresado(fiIDMovimientoMaestro));
                    DictionaryList.Add("txtMoneda", modelDb.fiIDMoneda == 1 ? "Lempira" : "Dolar");
                    DictionaryList.Add("txtUsuario", modelDb.fcUsuarioCreacion);
                    DictionaryList.Add("txtImporte", modelDb.fnImporteGravado.ToString("n"));
                    DictionaryList.Add("txtExento", modelDb.fnImporteExento.ToString("n"));
                    DictionaryList.Add("txtExonerado", modelDb.fnImporteExonerado.ToString("n"));
                    DictionaryList.Add("txtSubTotal", modelDb.fnSaldo.ToString("n"));
                    DictionaryList.Add("txtISV", modelDb.fnISV.ToString("n"));
                    DictionaryList.Add("txtTotal", modelDb.fnTotalFactura.ToString("n"));
                }
                else
                {
                    var basecontroller = new BaseController();
                    var ubicaciones = basecontroller.ObtenerInfoUbicacionPorTipo(inventarioModificado.fiIDUbicacion);
                    DictionaryList.Add("txtFactura", modelDb.fcNumeroFactura + " | <span style='color:green;'>" + inventarioModificado.fcNumeroFactura + "</span>");
                    DictionaryList.Add("txtFecha", modelDb.fdFechaFactura.ToString("dd/MM/yyyy") + " | <span style='color:green;'>" + inventarioModificado.fdFechaFactura.ToString("dd/MM/yyyy") + "</span>");
                    DictionaryList.Add("txtProveedor", modelDb.fcReferenciaFactura + $" | <span style='color:green;'>{inventarioModificado.fcReferenciaFactura}</span>");
                    DictionaryList.Add("txtUbicacion", modelDb.fcUbicacion + $" | <span style='color:green;'>{ubicaciones?.fcUbicacion ?? modelDb.fcUbicacion}</span>");
                    DictionaryList.Add("txtDescripcion", modelDb.fcDescripcionFactura + $" <br/><br/><span style='color:green;'>{inventarioModificado.fiIDUbicacion}</span>");
                    DictionaryList.Add("rowsProductos", DetalleInventarioIngresado(fiIDMovimientoMaestro));
                    DictionaryList.Add("txtMoneda", (modelDb.fiIDMoneda == 1 ? "Lempira" : "Dolar") + $" | <span style='color:green;'>{(inventarioModificado.fiIDMoneda == 1 ? "Lempira" : "Dolar")}</span>");
                    DictionaryList.Add("txtUsuario", modelDb.fcUsuarioCreacion + $" | <span style='color:green;'>{HttpContext.Current.User.Identity.Name}</span>");
                    DictionaryList.Add("txtImporte", modelDb.fnImporteGravado.ToString("n") + $" | <span style='color:green;'>{inventarioModificado.fnImporteGravado.ToString("n")}</span>");
                    DictionaryList.Add("txtExento", modelDb.fnImporteExento.ToString("n") + $" | <span style='color:green;'>{inventarioModificado.fnImporteExento.ToString("n")}</span>");
                    DictionaryList.Add("txtExonerado", modelDb.fnImporteExonerado.ToString("n") + $" | <span style='color:green;'>{inventarioModificado.fnImporteExonerado.ToString("n")}</span>");
                    DictionaryList.Add("txtSubTotal", modelDb.fnSaldo.ToString("n") + $" | <span style='color:green;'>{inventarioModificado.fnSaldo.ToString("n")}</span>");
                    DictionaryList.Add("txtISV", modelDb.fnISV.ToString("n") + $" | <span style='color:green;'>{inventarioModificado.fnISV.ToString("n")}</span>");
                    DictionaryList.Add("txtTotal", modelDb.fnTotalFactura.ToString("n") + $" | <span style='color:green;'>{inventarioModificado.fnTotalFactura.ToString("n")}</span>");
                }

                
                return DictionaryList;
            }
        }



        public string DetalleInventarioIngresado(int fiIDMovimientoMaestro, List<ListMovimientoDetalleViewModel> detalleModificaddo = null)
        {
            using(var contexto = new ORIONDBEntities())
            {
                var detalle = contexto.sp_Producto_Movimiento_Detalle_GetByIDMovimientoMaestro(fiIDMovimientoMaestro).ToList();
                var tr = "";

                if(detalleModificaddo == null)
                {
                    foreach (var item in detalle)
                    {
                        tr += $"<tr>";
                        tr += $"<td style='border: black 1px solid;'>{item.fcProducto}</td>";
                        tr += $"<td style='text-align: right;border: black 1px solid;'>{item.fnValorProductoMN ?? 0:n}</td>";
                        tr += $"<td style='text-align: right;border: black 1px solid;'>{item.fnValorProductoME ?? 0:n}</td>";
                        tr += $"<td style='text-align: center;border: black 1px solid;'>{item.fnCantidad}</td>";
                        tr += $"<td style='border: black 1px solid;'>{item.fcCodigoSerie1}</td>";
                        tr += $"<td style='border: black 1px solid;'>{item.fcCodigoSerie2}</td>";
                        tr += $"<td style='border: black 1px solid;'>{item.fcReferenciaMovimiento}</td>";
                        tr += $"</tr>";
                    }
                }
                else
                {
                    foreach (var item in detalle)
                    {
                        var dModificado = detalleModificaddo.FirstOrDefault(x => x.fiIDProducto == item.fiIDProducto && x.fiIDMovimiento == item.fiIDMovimiento);

                        if(dModificado == null)
                        {
                            tr += $"<tr style='color:red;'>";
                            tr += $"<td style='color:red;'>{item.fcProducto}</td>";
                            tr += $"<td style='text-align: right; color:red;'>{item.fnValorProductoMN ?? 0:n}</td>";
                            tr += $"<td style='text-align: right; color:red;'>{item.fnValorProductoME ?? 0:n}</td>";
                            tr += $"<td style='text-align: center;color:red;'>{item.fnCantidad}</td>";
                            tr += $"<td style='color:red;'>{item.fcCodigoSerie1}</td>";
                            tr += $"<td style='color:red;'>{item.fcCodigoSerie2}</td>";
                            tr += $"<td style='color:red;'>{item.fcReferenciaMovimiento}</td>";
                            tr += $"</tr>";
                        }
                        else
                        {
                            tr += $"<tr>";
                            tr += $"<td>{item.fcProducto}</td>";
                            tr += $"<td style='text-align: right;'>{item.fnValorProductoMN ?? 0:n} | <span style='color:green;'>{dModificado.fnValorProductoMN:n}</span></td>";
                            tr += $"<td style='text-align: right;'>{item.fnValorProductoME ?? 0:n} | <span style='color:green;'>{dModificado.fnValorProductoMN:n}</span></td>";
                            tr += $"<td style='text-align: center;'>{item.fnCantidad}</td> | <span style='color:green;'>{dModificado.fnCantidad}</span>";
                            tr += $"<td>{item.fcCodigoSerie1}</td> | <span style='color:green;'>{dModificado.fcCodigoSerie1}</span>";
                            tr += $"<td>{item.fcCodigoSerie2}</td> | <span style='color:green;'>{dModificado.fcCodigoSerie2}</span>";
                            tr += $"<td>{item.fcReferenciaMovimiento} | <span style='color:green;'>{dModificado.fcReferenciaMovimiento}</span></td>";
                            tr += $"</tr>";

                            detalleModificaddo.RemoveAll(x => x == dModificado);
                        }
                    }

                    if((detalleModificaddo?.Count ?? 0) > 0)
                    {
                        foreach(var item in detalleModificaddo)
                        {
                            tr += $"<tr style='color:green;'>";
                            tr += $"<td style='color:green;'>{item.fcProducto}</td>";
                            tr += $"<td style='text-align: right; color:green;'>{item.fnValorProductoMN:n}</td>";
                            tr += $"<td style='text-align: right; color:green;'>{item.fnValorProductoME:n}</td>";
                            tr += $"<td style='text-align: center;color:green;'>{item.fnCantidad}</td>";
                            tr += $"<td style='color:green;'>{item.fcCodigoSerie1}</td>";
                            tr += $"<td style='color:green;'>{item.fcCodigoSerie2}</td>";
                            tr += $"<td style='color:green;'>{item.fcReferenciaMovimiento}</td>";
                            tr += $"</tr>";
                        }
                    }

                }
                

                return tr;
            }
            
        }

        public SendEmailViewModel IngresoDeInventario(EmailTemplateServiceModel model, GuardarMovimientoInventarioViewModel inventarioModificado = null)
        {
            DictionaryList = GenerateDictionaryOfVariablesIngresoInventario(model.IdTransaccion);
            using (var recibo = new StreamReader(System.IO.Path.Combine(MemoryLoadManager.URL, @"Documento\Recursos\IngresoInventario.html")))
            {
                var baseController = new BaseController();
                var correos = baseController.GetConfiguracion<string>("Usuarios_Correos_Inventario", ',');

                
                var texto = ParseVariablesInEmailTemplate(recibo.ReadToEnd(), DictionaryList);
                Attachment fileAttachment = null;
                
                var reciboRpt = new ReportesTemplateService();
                var fileStream = new StreamReader(reciboRpt.GenerarRecibo(model.IdTransaccion), System.Text.Encoding.Default, false);

                //var file = new Attachment(fileStream.BaseStream, CustomerAttachmentName);
                return new SendEmailViewModel
                {
                    EmailName = "Ingreso de Inventario Novanet",
                    Subject = inventarioModificado == null ? "Ingreso Inventario #" + model.IdTransaccion : "Edicion de Ingreso de Inventario #" + model.IdTransaccion,
                    Body = texto,
                    ListDestinationEmail = correos,
                    List_CC = new List<string>(),
                    firma = ""
                };
            }
        }


        public async Task<bool> SendEmailToIngresoInventario(EmailTemplateServiceModel model, GuardarMovimientoInventarioViewModel inventarioModificado = null)
        {
            try
            {
                var emailGeneratedToSend = IngresoDeInventario(model, inventarioModificado);
                var SendEmailResult = await _emailService.SendEmailManyDestinationsAsync(emailGeneratedToSend);
                return SendEmailResult;
            }
            catch (Exception e)
            {
                //await _emailService.SendEmailException(e, "Send Email");
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }


        }


        ///SALIDA DE INVENTARIO

        private Dictionary<string, string> GenerateDictionaryOfVariablesSalidaInventario(int fiIDMovimientoMaestro)
        {

            var ci = new CultureInfo("es-ES");

            using (var contexto = new ORIONDBEntities())
            {
                using(var administrativo = new CoreAdministrativoEntities())
                {
                    var modelDb = contexto.sp_Inventario_Movimiento_Maestro_GetById(fiIDMovimientoMaestro)?.FirstOrDefault() ?? null;
                    var partida = administrativo.sp_PartidaDiario_VerMaestro(1, 1, 1, modelDb.fcPartida).FirstOrDefault();
                   
                    DictionaryList.Clear();

                   
                    DictionaryList.Add("txtPartida", modelDb.fcPartida);
                    DictionaryList.Add("txtSinopsis", partida.fcSinopsis);
                    DictionaryList.Add("txtComentario", partida.fcReferencia);
                    DictionaryList.Add("txtUbicacion", modelDb.fcUbicacion);
                    DictionaryList.Add("rowsProductos", DetalleInventarioIngresado(fiIDMovimientoMaestro));
                    DictionaryList.Add("txtUsuario", partida.fcCreador);

                    return DictionaryList;
                }
                
            }
        }


        public string DetalleInventarioIngresado(int fiIDMovimientoMaestro)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var detalle = contexto.sp_Inventario_Movimiento_Detalle_GetIDInventarioMovimientoMaestro(fiIDMovimientoMaestro).ToList();
                var tr = "";

                foreach(var item in detalle)
                {
                    tr += $"<tr>";
                    tr += $"<td style='border: black 1px solid;'>{item.fcProducto}</td>";
                    tr += $"<td style='text-align: right;border: black 1px solid;'>{item.fnValorProductoMN ?? 0:n}</td>";
                    tr += $"<td style='text-align: right;border: black 1px solid;'>{item.fnValorProductoME ?? 0:n}</td>";
                    tr += $"<td style='text-align: center;border: black 1px solid;'>{item.fnCantidad}</td>";
                    tr += $"<td style='border: black 1px solid;'>{item.fcCodigoSerie1}</td>";
                    tr += $"<td style='border: black 1px solid;'>{item.fcCodigoSerie2}</td>";
                    tr += $"<td style='border: black 1px solid;'>{item.fcDescripcion}</td>";
                    tr += $"</tr>";
                }

                



                return tr;
            }

        }


        public SendEmailViewModel SalidaDeInventarioPartida(EmailTemplateServiceModel model)
        {
            DictionaryList = GenerateDictionaryOfVariablesSalidaInventario(model.IdTransaccion);
            using (var recibo = new StreamReader(System.IO.Path.Combine(MemoryLoadManager.URL, @"Documento\Recursos\SalidaInventario.html")))
            {
                var baseController = new BaseController();
                var correos = baseController.GetConfiguracion<string>("Usuarios_Correos_SalidaInventario", ',');


                var texto = ParseVariablesInEmailTemplate(recibo.ReadToEnd(), DictionaryList);
                Attachment fileAttachment = null;

                var reciboRpt = new ReportesTemplateService();
                var fileStream = new StreamReader(reciboRpt.GenerarRecibo(model.IdTransaccion), System.Text.Encoding.Default, false);

                //var file = new Attachment(fileStream.BaseStream, CustomerAttachmentName);
                return new SendEmailViewModel
                {
                    EmailName = "Salida de Inventario Novanet",
                    Subject = $"Salida de Inventario #{model.IdTransaccion}",
                    Body = texto,
                    ListDestinationEmail = correos,
                    List_CC = new List<string>(),
                    firma = ""
                };
            }
        }

        public async Task<bool> SendEmailToSalidaInventarioPartida(EmailTemplateServiceModel model)
        {
            try {
                var emailGeneratedToSend = SalidaDeInventarioPartida(model);
                var SendEmailResult = await _emailService.SendEmailManyDestinationsAsync(emailGeneratedToSend);
                return SendEmailResult;
            }
            catch(Exception e)
            {
                var baseController = new BaseController();
                baseController.EscribirEnLogJson(new NotificacionViewModel { fcClase = "danger", fcTipoTransaccion = "Error", fcOperacion = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace });
                return false;
            }
        }

        #endregion
    }


    public class ProductosAdicionadosResponse
    {
        public List<ProductoAdicionado> ProductosAdicionados { get; set; }
    }

    public class ProductoAdicionado
    {
        public string Producto { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string TipoProducto { get; set; }
        public string Cantidad { get; set; }
        public string Precio { get; set; }
    }

}
