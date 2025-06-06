using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;
using OrionCoreCableColor.App_Helper;
using OrionCoreCableColor.Controllers;
using OrionCoreCableColor.DbConnection.CoreFinancieroDB;
using OrionCoreCableColor.DbConnection.OrionDB;
using OrionCoreCableColor.Models.Prestamo;
using OrionCoreCableColor.Models.Productos;
using OrionCoreCableColor.Models.Solicitudes;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace OrionCoreCableColor.App_Services.ReportesService
{
    public class ReportesTemplateService
    {

        public Table Linea(int columnas)
        {
            var tabla = new Table(columnas).SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
            var linea = new Cell(1, columnas).SetBorder(Border.NO_BORDER);
            tabla.AddCell(linea.SetBorderBottom(new SolidBorder(Color.ConvertRgbToCmyk(new DeviceRgb(System.Drawing.Color.Gray)), 1f, 0.5f)));
            return tabla;
        }

        public Stream GenerarRecibo(int fiIDTransaccion)
        {
            using (var contexto = new CoreFinancieroEntities2())
            {

                var ms = new MemoryStream();
                var pw = new PdfWriter(ms);
                var pdfDocument = new PdfDocument(pw);
                var doc = new Document(pdfDocument, PageSize.LETTER);

                doc.SetMargins(20, 35, 5, 35);
                doc.SetProperty(Property.LEADING, new Leading(Leading.MULTIPLIED, 1f));
                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                string pathImageNovanetLogo = System.IO.Path.Combine(MemoryLoadManager.URL, @"Content\img\LogoPrestadito.png");
                var prestaditoLogo = new Image(ImageDataFactory.Create(pathImageNovanetLogo));

                string pathImageBackGround = System.IO.Path.Combine(MemoryLoadManager.URL, @"Content\img\HOJAS_NOVANET.png");
                var backGround = new Image(ImageDataFactory.Create(pathImageBackGround));
                doc.SetFontSize(12f).SetFont(font);

                pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, new BackgroundImageHandler(backGround));
                //pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, new HeaderEventHandler(novanetLogo));


                var modelDb = contexto.sp_OperacionesCaja_ConsultarAbono(fiIDTransaccion, 0)?.FirstOrDefault() ?? null;
                if (modelDb != null)
                {
                    /*CUERPO DEL REPORTE*/

                    var tblInformacionEmpresa = new Table(UnitValue.CreatePercentArray(new float[] { 25f, 45f })).SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                    var cellLogo = new Cell(4, 1).Add(prestaditoLogo.SetAutoScale(true).SetBorder(Border.NO_BORDER)).SetBorder(Border.NO_BORDER);
                    tblInformacionEmpresa.AddCell(cellLogo);

                    var cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text($"Prestadito S.A de C.V.").SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10f)).SetBorder(Border.NO_BORDER);
                    tblInformacionEmpresa.AddCell(cellInfoEmpresa);

                    cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text($"Tel:(504) 2540-1050").SetBold()).SetFontSize(10f)).SetBorder(Border.NO_BORDER);
                    tblInformacionEmpresa.AddCell(cellInfoEmpresa);

                    cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text($"RTN:05019016811399").SetBold()).SetFontSize(10f)).SetBorder(Border.NO_BORDER);
                    tblInformacionEmpresa.AddCell(cellInfoEmpresa);

                    cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text($"Barrio Rio de Piedras, 4ta calle, 25 y 26 Ave.")).SetFontSize(10f)).SetBorder(Border.NO_BORDER);
                    tblInformacionEmpresa.AddCell(cellInfoEmpresa);

                    doc.Add(tblInformacionEmpresa);

                    doc.Add(new Paragraph(""));
                    doc.Add(new Paragraph(new Text($"RECIBO").SetBold()).SetTextAlignment(TextAlignment.CENTER));
                    doc.Add(new Paragraph(""));

                    var tblEncabezadoRecibo = new Table(UnitValue.CreatePercentArray(new float[] { 35f, 45f })).SetWidth(UnitValue.CreatePercentValue(70)).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                    var estiloCeldas = new Style().SetTextAlignment(TextAlignment.LEFT); //.SetBackgroundColor(ColorConstants.LIGHT_GRAY)

                    var agencia = new Cell(1, 1).SetWidth(UnitValue.CreatePercentValue(35f)).AddStyle(estiloCeldas).Add(new Paragraph(new Text($"Agencia:"))).SetBorder(Border.NO_BORDER);
                    var agenciaValue = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph(new Text(modelDb.fcNombreAgencia))).SetBorder(Border.NO_BORDER);
                    tblEncabezadoRecibo.AddCell(agencia).AddCell(agenciaValue);

                    var referencia = new Cell(1, 1).SetWidth(UnitValue.CreatePercentValue(35f)).AddStyle(estiloCeldas).Add(new Paragraph(new Text("No.Referencia:"))).SetBorder(Border.NO_BORDER);
                    var referenciaValue = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph(new Text(modelDb.fiIDTransaccion.ToString()))).SetBorder(Border.NO_BORDER);
                    tblEncabezadoRecibo.AddCell(referencia).AddCell(referenciaValue);

                    var fechaTransaccion = new Cell(1, 1).SetWidth(UnitValue.CreatePercentValue(35f)).AddStyle(estiloCeldas).Add(new Paragraph(new Text("Fecha Transaccion:"))).SetBorder(Border.NO_BORDER);
                    var fechaTransaccionValue = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph(new Text(modelDb.fdFechaTransaccion.ToString("dd/MM/yyyy hh:mm tt")))).SetBorder(Border.NO_BORDER);
                    tblEncabezadoRecibo.AddCell(fechaTransaccion).AddCell(fechaTransaccionValue);

                    var espacio = new Cell(1, 2).SetBorder(Border.NO_BORDER).SetHeight(10f);
                    tblEncabezadoRecibo.AddCell(espacio).AddCell(espacio);

                    var cliente = new Cell(1, 1).SetWidth(UnitValue.CreatePercentValue(35f)).AddStyle(estiloCeldas).Add(new Paragraph(new Text("Cliente:"))).SetBorder(Border.NO_BORDER);
                    var clienteValue = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph(new Text(modelDb.fcNombreCliente))).SetBorder(Border.NO_BORDER);
                    tblEncabezadoRecibo.AddCell(cliente).AddCell(clienteValue);




                    var tblProducto = new Table(UnitValue.CreatePercentArray(new float[] { 35f, 45f })).SetWidth(UnitValue.CreatePercentValue(70)).SetHorizontalAlignment(HorizontalAlignment.LEFT);


                    //float[] widthColums = { widthCol1Tabla1.GetValue(), widthCol2Tabla1.GetValue() };
                    //tblProducto.SetWidth(UnitValue.CreatePercentValue(widthCol1Tabla1.GetValue() + widthCol2Tabla1.GetValue()));

                    var celdaDescripcion = new Cell(1, 2).SetBorder(Border.NO_BORDER).SetHeight(10f);
                    tblProducto.AddCell(celdaDescripcion);


                    celdaDescripcion = new Cell(1, 1).SetWidth(UnitValue.CreatePercentValue(35f)).AddStyle(estiloCeldas).Add(new Paragraph(new Text("Prestamo:"))).SetBorder(Border.NO_BORDER);
                    var celdaValor = new Cell(1, 1).Add(new Paragraph(new Text(modelDb.fcIDPrestamo))).SetBorder(Border.NO_BORDER);
                    tblProducto.AddCell(celdaDescripcion).AddCell(celdaValor);

                    celdaDescripcion = new Cell(1, 2).SetBorder(Border.NO_BORDER).SetHeight(10f);
                    tblProducto.AddCell(celdaDescripcion);


                    celdaDescripcion = new Cell(1, 1).SetWidth(UnitValue.CreatePercentValue(35f)).AddStyle(estiloCeldas).Add(new Paragraph(new Text("Producto:"))).SetBorder(Border.NO_BORDER);
                    celdaValor = new Cell(1, 1).Add(new Paragraph(new Text(modelDb.fcProducto))).SetBorder(Border.NO_BORDER);
                    tblProducto.AddCell(celdaDescripcion).AddCell(celdaValor);


                    celdaDescripcion = new Cell(1, 1).SetWidth(UnitValue.CreatePercentValue(35f)).AddStyle(estiloCeldas).Add(new Paragraph(new Text("Moneda:"))).SetBorder(Border.NO_BORDER);
                    celdaValor = new Cell(1, 1).Add(new Paragraph(new Text(modelDb.fcMoneda))).SetBorder(Border.NO_BORDER);
                    tblProducto.AddCell(celdaDescripcion).AddCell(celdaValor);

                    celdaDescripcion = new Cell(1, 1).SetWidth(UnitValue.CreatePercentValue(35f)).AddStyle(estiloCeldas).Add(new Paragraph(new Text("Fecha Vencimiento:"))).SetBorder(Border.NO_BORDER);
                    celdaValor = new Cell(1, 1).Add(new Paragraph(new Text(Convert.ToDateTime(modelDb.fdFechaVencimiento).ToString("dd/MM/yyyy")))).SetBorder(Border.NO_BORDER);
                    tblProducto.AddCell(celdaDescripcion).AddCell(celdaValor);

                    celdaDescripcion = new Cell(1, 2).SetBorder(Border.NO_BORDER).SetHeight(10f);
                    tblProducto.AddCell(celdaDescripcion);

                    celdaDescripcion = new Cell(1, 1).SetWidth(UnitValue.CreatePercentValue(35f)).AddStyle(estiloCeldas).Add(new Paragraph(new Text("Frecuencia de pago:"))).SetBorder(Border.NO_BORDER);
                    celdaValor = new Cell(1, 1).Add(new Paragraph(new Text(modelDb.fcFrecuencia))).SetBorder(Border.NO_BORDER);
                    tblProducto.AddCell(celdaDescripcion).AddCell(celdaValor);

                    celdaDescripcion = new Cell(1, 1).SetWidth(UnitValue.CreatePercentValue(35f)).AddStyle(estiloCeldas).Add(new Paragraph(new Text("Valor Cuota:"))).SetBorder(Border.NO_BORDER);
                    celdaValor = new Cell(1, 1).Add(new Paragraph(new Text(modelDb.fcSimboloMoneda + " " + Convert.ToDecimal(modelDb.fnValorCuota).ToString("n")))).SetBorder(Border.NO_BORDER);
                    tblProducto.AddCell(celdaDescripcion).AddCell(celdaValor);


                    doc.Add(tblEncabezadoRecibo);
                    doc.Add(Linea(2));
                    doc.Add(tblProducto);
                    doc.Add(new Paragraph(new Text($"<<DETALLE DE ABONO>>").SetBold()).SetTextAlignment(TextAlignment.CENTER));
                    doc.Add(Linea(2));


                    var tblDetalleAbono = new Table(2).SetWidth(UnitValue.CreatePercentValue(80)).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                    celdaDescripcion = new Cell(1, 1).Add(new Paragraph(new Text("TOTAL ABONADO").SetBold()).SetTextAlignment(TextAlignment.CENTER)).SetBorder(Border.NO_BORDER);
                    celdaValor = new Cell(1, 1).Add(new Paragraph(new Text(modelDb.fcSimboloMoneda + " " + Convert.ToDecimal(modelDb.fnTotalAbonado).ToString("n"))).SetTextAlignment(TextAlignment.CENTER)).SetBorder(Border.NO_BORDER);

                    tblDetalleAbono.AddCell(celdaDescripcion).AddCell(celdaValor);
                    doc.Add(tblDetalleAbono);
                    doc.Add(Linea(2));

                    var tblFinPagina = new Table(UnitValue.CreatePercentArray(new float[] { 35f, 45f })).SetWidth(UnitValue.CreatePercentValue(30)).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                    celdaDescripcion = new Cell(1, 2).SetBorder(Border.NO_BORDER).SetHeight(5f);
                    tblFinPagina.AddCell(celdaDescripcion);

                    celdaDescripcion = new Cell(1, 1).SetWidth(UnitValue.CreatePercentValue(35f)).Add(new Paragraph(new Text("Cuotas:"))).SetBorder(Border.NO_BORDER);
                    celdaValor = new Cell(1, 1).Add(new Paragraph(new Text(modelDb.fcAvancedeCuotas))).SetBorder(Border.NO_BORDER);
                    tblFinPagina.AddCell(celdaDescripcion).AddCell(celdaValor);

                    //celdaDescripcion = new Cell(1, 1).SetWidth(UnitValue.CreatePercentValue(35f)).Add(new Paragraph(new Text("Cajero:"))).SetBorder(Border.NO_BORDER);
                    //celdaValor = new Cell(1, 1).Add(new Paragraph(new Text(modelDb.fcUsuarioDominio))).SetBorder(Border.NO_BORDER);
                    //tblFinPagina.AddCell(celdaDescripcion).AddCell(celdaValor);
                    //doc.Add(tblFinPagina);

                    doc.Add(new Paragraph("\n\n\n"));

                    //var tblLineaFirmaYSello = new Table(new float[] { 20f, 60f, 20f }).SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                    //tblLineaFirmaYSello.AddCell(new Cell(1, 1).SetBorder(Border.NO_BORDER));
                    //tblLineaFirmaYSello.AddCell(new Cell(1, 1).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1f)));
                    //tblLineaFirmaYSello.AddCell(new Cell(1, 1).SetBorder(Border.NO_BORDER));
                    //doc.Add(tblLineaFirmaYSello);
                    //doc.Add(new Paragraph("Firma autorizada y Sello\n").SetTextAlignment(TextAlignment.CENTER).SetFontSize(10));
                    //doc.Add(new Paragraph(""));
                    //doc.Add(new Paragraph("Servicio de colecturia Fondos Prestadito\n").SetTextAlignment(TextAlignment.CENTER).SetFontSize(10));
                    //doc.Add(new Paragraph(""));
                    //doc.Add(new Paragraph("Este recibo no es valido sin la firma autorizada y el sello\n").SetTextAlignment(TextAlignment.CENTER).SetItalic().SetFontSize(10));
                    /*FIN DEL CUERPO DEL REPORTE*/
                }

                //OfficeConverter

                doc.Close();
                var bytesStream = ms.ToArray();
                ms = new MemoryStream();
                ms.Write(bytesStream, 0, bytesStream.Length);
                ms.Position = 0;
                return ms;
            }
        }

        public Stream GenerarFactura(int fiIDTransaccion)
        {
            using (var contexto = new CoreFinancieroEntities2())
            {
                var ms = new MemoryStream();
                var pw = new PdfWriter(ms);
                var pdfDocument = new PdfDocument(pw);
                var doc = new Document(pdfDocument, PageSize.LETTER);

                doc.SetMargins(20, 20, 5, 20);
                doc.SetProperty(Property.LEADING, new Leading(Leading.MULTIPLIED, 1f));
                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                doc.SetFontSize(12f).SetFont(font);
                string pathImageNovanetLogo = System.IO.Path.Combine(MemoryLoadManager.URL, @"Content\img\LogoPrestadito.png");
                var prestaditoLogo = new Image(ImageDataFactory.Create(pathImageNovanetLogo));

                string pathImageBackGround = System.IO.Path.Combine(MemoryLoadManager.URL, @"Content\img\HOJAS_NOVANET.png");
                var backGround = new Image(ImageDataFactory.Create(pathImageBackGround));

                pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, new BackgroundImageHandler(backGround));
                var modelDb = contexto.sp_Facturacion_ConsultarFactura_NovaNet(fiIDTransaccion, null)?.FirstOrDefault() ?? null;

                if (modelDb != null)
                {
                    var tblInformacionEmpresa = new Table(UnitValue.CreatePercentArray(new float[] { 25f, 45f })).SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                    var cellLogo = new Cell(6, 1).Add(prestaditoLogo.SetAutoScale(true).SetBorder(Border.NO_BORDER)).SetBorder(Border.NO_BORDER);
                    tblInformacionEmpresa.AddCell(cellLogo);

                    var cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text($"Prestadito S.A de C.V.").SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10f)).SetBorder(Border.NO_BORDER);
                    tblInformacionEmpresa.AddCell(cellInfoEmpresa);

                    cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text(modelDb.fcDireccionAgencia).SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10f)).SetBorder(Border.NO_BORDER);
                    tblInformacionEmpresa.AddCell(cellInfoEmpresa);

                    cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text("SAN PEDRO SULA").SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10f)).SetBorder(Border.NO_BORDER);
                    tblInformacionEmpresa.AddCell(cellInfoEmpresa);

                    cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text("Tel.(+504)2504-6682").SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10f)).SetBorder(Border.NO_BORDER);
                    tblInformacionEmpresa.AddCell(cellInfoEmpresa);

                    cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text("contabilidad@miprestadito.com").SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10f)).SetBorder(Border.NO_BORDER);
                    tblInformacionEmpresa.AddCell(cellInfoEmpresa);

                    cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text("Domicilio Fiscal: San Pedro Sula").SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10f)).SetBorder(Border.NO_BORDER);
                    tblInformacionEmpresa.AddCell(cellInfoEmpresa);

                    doc.Add(tblInformacionEmpresa);

                    doc.Add(new Paragraph(new Text($"RTN. 05019016811399").SetBold()));
                    doc.Add(new Paragraph(new Text($"Factura: {modelDb.fcFactura}").SetBold()));
                    doc.Add(new Paragraph(new Text($"CAI: {modelDb.fcCAI}")).SetBold());

                    doc.Add(new Paragraph());

                    var tablaInfoCliente = new Table(4).SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                    var celdaDescripcion = new Cell(1, 1).Add(new Paragraph(new Text("Cliente:")).SetBold()).SetBorder(Border.NO_BORDER);
                    var celdaValor = new Cell(1, 1).Add(new Paragraph(new Text(modelDb.fcNombreCliente))).SetBorder(Border.NO_BORDER);

                    tablaInfoCliente.AddCell(celdaDescripcion).AddCell(celdaValor);

                    celdaDescripcion = new Cell(1, 1).Add(new Paragraph(new Text("Fecha:")).SetBold()).SetBorder(Border.NO_BORDER);
                    celdaValor = new Cell(1, 1).Add(new Paragraph(new Text(modelDb.fdFechaFactura.ToString("dd/MM/yyyy")))).SetBorder(Border.NO_BORDER);
                    tablaInfoCliente.AddCell(celdaDescripcion).AddCell(celdaValor);



                    celdaDescripcion = new Cell(1, 1).Add(new Paragraph(new Text("RTN:")).SetBold()).SetBorder(Border.NO_BORDER);
                    celdaValor = new Cell(1, 3).Add(new Paragraph(new Text(modelDb.fcRTN))).SetBorder(Border.NO_BORDER);
                    tablaInfoCliente.AddCell(celdaDescripcion).AddCell(celdaValor);



                    celdaDescripcion = new Cell(1, 1).Add(new Paragraph(new Text("Direccion:")).SetBold()).SetBorder(Border.NO_BORDER);
                    celdaValor = new Cell(1, 3).Add(new Paragraph(new Text(""))).SetBorder(Border.NO_BORDER);
                    tablaInfoCliente.AddCell(celdaDescripcion).AddCell(celdaValor);


                    celdaDescripcion = new Cell(1, 1).Add(new Paragraph(new Text("Fecha Limite de Emisión:")).SetBold()).SetBorder(Border.NO_BORDER);
                    celdaValor = new Cell(1, 3).Add(new Paragraph(new Text(modelDb.fdFechaRangoFinalFacturacion.ToString("dd/MM/yyyy")))).SetBorder(Border.NO_BORDER);
                    tablaInfoCliente.AddCell(celdaDescripcion).AddCell(celdaValor);


                    celdaDescripcion = new Cell(1, 1).Add(new Paragraph(new Text("Rango de Autorización:")).SetBold()).SetBorder(Border.NO_BORDER);
                    celdaValor = new Cell(1, 3).Add(new Paragraph(new Text($"{modelDb.fcRangoInicialFacturacion} al {modelDb.fcRangoFinalFacturacion}"))).SetBorder(Border.NO_BORDER);
                    tablaInfoCliente.AddCell(celdaDescripcion).AddCell(celdaValor);


                    celdaDescripcion = new Cell(1, 1).Add(new Paragraph(new Text("No. Declaracion:")).SetBold()).SetBorder(Border.NO_BORDER);
                    celdaValor = new Cell(1, 3).Add(new Paragraph(new Text($""))).SetBorder(Border.NO_BORDER);
                    tablaInfoCliente.AddCell(celdaDescripcion).AddCell(celdaValor);



                    doc.Add(tablaInfoCliente);
                    doc.Add(new Paragraph(""));
                    var tablaProducto = new Table(UnitValue.CreatePercentArray(new float[] { 10f, 60f, 15f, 15f })).UseAllAvailableWidth().SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                    tablaProducto.AddHeaderCell(new Paragraph("CANT.").SetBold().SetTextAlignment(TextAlignment.CENTER));
                    tablaProducto.AddHeaderCell(new Paragraph("DESCRIPCION").SetBold());
                    tablaProducto.AddHeaderCell(new Paragraph("VALOR").SetBold().SetTextAlignment(TextAlignment.RIGHT));
                    tablaProducto.AddHeaderCell(new Paragraph("TOTAL").SetBold().SetTextAlignment(TextAlignment.RIGHT));

                    var celdaProdCantidad = new Cell(1, 1).Add(new Paragraph("1").SetTextAlignment(TextAlignment.CENTER));
                    var celdaProdDescripcion = new Cell(1, 1).Add(new Paragraph(modelDb.fcProductoDescripcion));
                    var celdaProdValor = new Cell(1, 1).Add(new Paragraph(new Text($"{modelDb.fcSimboloMoneda} {Convert.ToDecimal(modelDb.fnTotal):n}")).SetTextAlignment(TextAlignment.RIGHT));
                    var celdaProdTotal = new Cell(1, 1).Add(new Paragraph(new Text($"{modelDb.fcSimboloMoneda} {Convert.ToDecimal(modelDb.fnTotal):n}")).SetTextAlignment(TextAlignment.RIGHT));
                    tablaProducto.AddCell(celdaProdCantidad).AddCell(celdaProdDescripcion).AddCell(celdaProdValor).AddCell(celdaProdTotal);



                    celdaProdCantidad = new Cell(1, 1);
                    celdaProdDescripcion = new Cell(1, 1).Add(new Paragraph("Sub Total").SetBold().SetTextAlignment(TextAlignment.RIGHT));
                    celdaProdValor = new Cell(1, 1);
                    celdaProdTotal = new Cell(1, 1).Add(new Paragraph(new Text($"{modelDb.fcSimboloMoneda} {Convert.ToDecimal(modelDb.fnSubtotal).ToString("n")}")).SetTextAlignment(TextAlignment.RIGHT));
                    tablaProducto.AddCell(celdaProdCantidad).AddCell(celdaProdDescripcion).AddCell(celdaProdValor).AddCell(celdaProdTotal);


                    celdaProdCantidad = new Cell(1, 1);
                    celdaProdDescripcion = new Cell(1, 1).Add(new Paragraph("Descuento").SetBold().SetTextAlignment(TextAlignment.RIGHT));
                    celdaProdValor = new Cell(1, 1);
                    celdaProdTotal = new Cell(1, 1).Add(new Paragraph(new Text($"{modelDb.fcSimboloMoneda} 0.00")).SetTextAlignment(TextAlignment.RIGHT));
                    tablaProducto.AddCell(celdaProdCantidad).AddCell(celdaProdDescripcion).AddCell(celdaProdValor).AddCell(celdaProdTotal);

                    celdaProdCantidad = new Cell(1, 1);
                    celdaProdDescripcion = new Cell(1, 1).Add(new Paragraph("Exento").SetBold().SetTextAlignment(TextAlignment.RIGHT));
                    celdaProdValor = new Cell(1, 1);
                    celdaProdTotal = new Cell(1, 1).Add(new Paragraph(new Text($"{modelDb.fcSimboloMoneda} {Convert.ToDecimal(modelDb.fnImporteExento).ToString("n")}")).SetTextAlignment(TextAlignment.RIGHT));
                    tablaProducto.AddCell(celdaProdCantidad).AddCell(celdaProdDescripcion).AddCell(celdaProdValor).AddCell(celdaProdTotal);


                    celdaProdCantidad = new Cell(1, 1);
                    celdaProdDescripcion = new Cell(1, 1).Add(new Paragraph("Exonerado").SetBold().SetTextAlignment(TextAlignment.RIGHT));
                    celdaProdValor = new Cell(1, 1);
                    celdaProdTotal = new Cell(1, 1).Add(new Paragraph(new Text($"{modelDb.fcSimboloMoneda} {Convert.ToDecimal(modelDb.fnImporteExonerado).ToString("n")}")).SetTextAlignment(TextAlignment.RIGHT));
                    tablaProducto.AddCell(celdaProdCantidad).AddCell(celdaProdDescripcion).AddCell(celdaProdValor).AddCell(celdaProdTotal);


                    celdaProdCantidad = new Cell(1, 1);
                    celdaProdDescripcion = new Cell(1, 1).Add(new Paragraph("Gravado 15%").SetBold().SetTextAlignment(TextAlignment.RIGHT));
                    celdaProdValor = new Cell(1, 1);
                    celdaProdTotal = new Cell(1, 1).Add(new Paragraph(new Text($"{modelDb.fcSimboloMoneda} {Convert.ToDecimal(modelDb.fnImporteGravado).ToString("n")}")).SetTextAlignment(TextAlignment.RIGHT));
                    tablaProducto.AddCell(celdaProdCantidad).AddCell(celdaProdDescripcion).AddCell(celdaProdValor).AddCell(celdaProdTotal);

                    celdaProdCantidad = new Cell(1, 1);
                    celdaProdDescripcion = new Cell(1, 1).Add(new Paragraph("ISV 15%").SetBold().SetTextAlignment(TextAlignment.RIGHT));
                    celdaProdValor = new Cell(1, 1);
                    celdaProdTotal = new Cell(1, 1).Add(new Paragraph(new Text($"{modelDb.fcSimboloMoneda} {Convert.ToDecimal(modelDb.fnImpuestos).ToString("n")}")).SetTextAlignment(TextAlignment.RIGHT));
                    tablaProducto.AddCell(celdaProdCantidad).AddCell(celdaProdDescripcion).AddCell(celdaProdValor).AddCell(celdaProdTotal);

                    celdaProdCantidad = new Cell(1, 1);
                    celdaProdDescripcion = new Cell(1, 1).Add(new Paragraph("TOTAL").SetBold().SetTextAlignment(TextAlignment.RIGHT));
                    celdaProdValor = new Cell(1, 1);
                    celdaProdTotal = new Cell(1, 1).Add(new Paragraph(new Text($"{modelDb.fcSimboloMoneda} {Convert.ToDecimal(modelDb.fnTotal).ToString("n")}")).SetTextAlignment(TextAlignment.RIGHT).SetBold());
                    tablaProducto.AddCell(celdaProdCantidad).AddCell(celdaProdDescripcion).AddCell(celdaProdValor).AddCell(celdaProdTotal);


                    doc.Add(tablaProducto);

                    doc.Add(new Paragraph($"MONTO EN LETRAS: {modelDb.fcValorenLetras} CTVS. EXACTOS\n\n\n\n\n").SetFontSize(9f).SetBold());

                    var tblFirmas = new Table(UnitValue.CreatePercentArray(new float[] { 15f, 17f, 2f, 18f, 14f, 2f, 16f, 16f })).SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                    tblFirmas.AddCell(new Cell(1, 1).Add(new Paragraph("No. Orden Exenta:").SetBold().SetTextAlignment(TextAlignment.JUSTIFIED)).SetBorder(Border.NO_BORDER).SetFontSize(9));
                    tblFirmas.AddCell(new Cell(1, 1).Add(new Paragraph("")).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1f)));
                    tblFirmas.AddCell(new Cell(1, 1).SetBorder(Border.NO_BORDER));


                    tblFirmas.AddCell(new Cell(1, 1).Add(new Paragraph("Constancia Exonerada:").SetBold().SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER).SetFontSize(8));
                    tblFirmas.AddCell(new Cell(1, 1).Add(new Paragraph("")).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1f)));
                    tblFirmas.AddCell(new Cell(1, 1).SetBorder(Border.NO_BORDER));

                    tblFirmas.AddCell(new Cell(1, 1).Add(new Paragraph("No. Registro SAG:").SetBold().SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER).SetFontSize(9));
                    tblFirmas.AddCell(new Cell(1, 1).Add(new Paragraph("")).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1f)));

                    tblFirmas.AddCell(new Cell(1, 8).Add(new Paragraph("Original: Cliente").SetBold().SetFontSize(9)).SetBorder(Border.NO_BORDER));
                    tblFirmas.AddCell(new Cell(1, 8).Add(new Paragraph("Copia: Obligado Tributario").SetBold().SetFontSize(9)).SetBorder(Border.NO_BORDER));
                    doc.Add(tblFirmas);
                    doc.Add(new Paragraph("\n\n\n\n"));

                    var tblLineaFirmaYSello = new Table(new float[] { 20f, 60f, 20f }).SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                    tblLineaFirmaYSello.AddCell(new Cell(1, 1).SetBorder(Border.NO_BORDER));
                    tblLineaFirmaYSello.AddCell(new Cell(1, 1).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1f)));
                    tblLineaFirmaYSello.AddCell(new Cell(1, 1).SetBorder(Border.NO_BORDER));
                    doc.Add(tblLineaFirmaYSello);
                    doc.Add(new Paragraph("Firma y Sello\n").SetTextAlignment(TextAlignment.CENTER).SetFontSize(10));
                    doc.Add(new Paragraph("LA FACTURA ES BENEFICIO DE TODOS, EXIJALA").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8));

                }
                doc.Close();
                var bytesStream = ms.ToArray();
                ms = new MemoryStream();
                ms.Write(bytesStream, 0, bytesStream.Length);
                ms.Position = 0;
                return ms;
            }
        }


        public Stream GenerarSalidaInventario(int fiIDInventarioMovimientoMaestro)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var baseController = new BaseController();
                var ubicaciones = baseController.ListarUbicacionesPorTipo();
                var imagen = new MemoryStream();

                var client = new WebClient();
                var uri = baseController.GetContentDocument("img", @"inventario\firmas", "FirmaSalidaInventario-" + fiIDInventarioMovimientoMaestro + ".png");
                var content = client.DownloadData(uri);
                imagen = new MemoryStream(content);


                var modelDb = contexto.sp_Inventario_Movimiento_Maestro_GetById(fiIDInventarioMovimientoMaestro).FirstOrDefault();
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

                var modelDbDetalle = contexto.sp_Inventario_Movimiento_Detalle_GetIDInventarioMovimientoMaestro(fiIDInventarioMovimientoMaestro).ToList();
                var tipoMovimiento = contexto.sp_Catalogo_TipoMovimiento_GetById(modelDb.fiIDTipoMovimiento).FirstOrDefault();



                var ms = new MemoryStream();

                var pw = new PdfWriter(ms);

                var pdfDocument = new PdfDocument(pw);
                var doc = new Document(pdfDocument, PageSize.LETTER);


                doc.SetMargins(20, 20, 5, 20);

                string pathImageNovanetLogo = System.IO.Path.Combine(MemoryLoadManager.URL, @"Content\img\LogoPrestadito.png");
                var prestaditoLogo = new iText.Layout.Element.Image(ImageDataFactory.Create(pathImageNovanetLogo));
                //doc.Add()
                string pathImageBackGround = System.IO.Path.Combine(MemoryLoadManager.URL, @"Content\img\HOJAS_NOVANET.png");
                var backGround = new Image(ImageDataFactory.Create(pathImageBackGround));
                pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, new BackgroundImageHandler(backGround));


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



                if (!String.IsNullOrEmpty(modelDb.fcDocumentoFirma))
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
                doc.Add(new Paragraph($"{stringFirma}").SetTextAlignment(TextAlignment.CENTER));



                doc.Close();

                var bytesStream = ms.ToArray();
                ms = new MemoryStream();
                ms.Write(bytesStream, 0, bytesStream.Length);
                ms.Position = 0;
                return ms;
            }
        }


        public Stream GenerarReporteDiaInfoClientesFinales()
        {
            using (var contexto = new ORIONDBEntities())
            {
                var ms = new MemoryStream();
                var pw = new PdfWriter(ms);
                var pdfDocument = new PdfDocument(pw);
                var doc = new Document(pdfDocument, PageSize.LETTER);

                doc.SetMargins(20, 20, 5, 20);
                doc.SetProperty(Property.LEADING, new Leading(Leading.MULTIPLIED, 1f));
                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                doc.SetFontSize(9f).SetFont(font);
                string pathImageNovanetLogo = System.IO.Path.Combine(MemoryLoadManager.URL, @"Content\img\LogoPrestadito.png");
                var prestaditoLogo = new Image(ImageDataFactory.Create(pathImageNovanetLogo));

                string pathImageBackGround = System.IO.Path.Combine(MemoryLoadManager.URL, @"Content\img\HOJAS_NOVANET.png");
                var backGround = new Image(ImageDataFactory.Create(pathImageBackGround));

                pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, new BackgroundImageHandler(backGround));
                var modelDb = contexto.sp_Solicitudes_Bandeja_Instalados(1).ToList();

                var tblInformacionEmpresa = new Table(UnitValue.CreatePercentArray(new float[] { 5f, 25f, 5f, 5f, 35f, 10f, 5f, 5f, 5f })).SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                var celdaSolicitud = new Cell(1, 1).Add(new Paragraph(new Text("Solicitud").SetBold()));//.SetBorder(Border.NO_BORDER);
                var celdaCliente = new Cell(1, 1).Add(new Paragraph(new Text("Cliente").SetBold()));//.SetBorder(Border.NO_BORDER);
                var celdaFechaIngreso = new Cell(1, 1).Add(new Paragraph(new Text("Fecha Ingreso").SetBold()));//.SetBorder(Border.NO_BORDER);
                var celdaOficial = new Cell(1, 1).Add(new Paragraph(new Text("Oficial").SetBold()));//.SetBorder(Border.NO_BORDER);
                var celdaArticulo = new Cell(1, 1).Add(new Paragraph(new Text("Articulo")).SetBold());//.SetBorder(Border.NO_BORDER);
                var celdaServicio = new Cell(1, 1).Add(new Paragraph(new Text("Servicio").SetBold()));//.SetBorder(Border.NO_BORDER);
                var celdaEstado = new Cell(1, 1).Add(new Paragraph(new Text("Estado").SetBold()));//.SetBorder(Border.NO_BORDER);
                var celdaFechaInstalacion = new Cell(1, 1).Add(new Paragraph(new Text("Fecha Instalacion").SetBold()));//.SetBorder(Border.NO_BORDER);
                var celdaEstadoInstalacion = new Cell(1, 1).Add(new Paragraph(new Text("Estado Instalacion").SetBold()));//.SetBorder(Border.NO_BORDER);
                //var cellLogo = new Cell(6, 1).Add(prestaditoLogo.SetAutoScale(true).SetBorder(Border.NO_BORDER)).SetBorder(Border.NO_BORDER);
                //tblInformacionEmpresa.AddCell(cellLogo);
                tblInformacionEmpresa
                    .AddCell(celdaSolicitud)
                    .AddCell(celdaCliente)
                    .AddCell(celdaFechaIngreso)
                    .AddCell(celdaOficial)
                    .AddCell(celdaArticulo)
                    .AddCell(celdaServicio)
                    .AddCell(celdaEstado)
                    .AddCell(celdaFechaInstalacion)
                    .AddCell(celdaEstadoInstalacion);

                foreach (var item in modelDb)
                {

                    celdaSolicitud = new Cell(1, 1).Add(new Paragraph(new Text(item.fiIDSolicitud.ToString())));//.SetBorder(Border.NO_BORDER);
                    celdaCliente = new Cell(1, 1).Add(new Paragraph(new Text($"{item.fcNombre} {item.fcIdentidad} {item.fcTelefono}")));//.SetBorder(Border.NO_BORDER);
                    celdaFechaIngreso = new Cell(1, 1).Add(new Paragraph(new Text(Convert.ToDateTime(item.fdFechaCreacionSolicitud).ToString("dd/MM/yyyy"))));//.SetBorder(Border.NO_BORDER);
                    celdaOficial = new Cell(1, 1).Add(new Paragraph(new Text(item.NombreOficial)));//.SetBorder(Border.NO_BORDER);
                    celdaArticulo = new Cell(1, 1).Add(new Paragraph(new Text(item.fcArticulosdelContrato)));//.SetBorder(Border.NO_BORDER);
                    celdaServicio = new Cell(1, 1).Add(new Paragraph(new Text(item.fiTipoInstalacion == true ? "Servicio con Articulo" : "Servicio solo Internet")));//.SetBorder(Border.NO_BORDER);
                    celdaEstado = new Cell(1, 1).Add(new Paragraph(new Text(item.fiIDEstado == 1 ? "Pendiente" : "Aprobado")));//.SetBorder(Border.NO_BORDER);
                    celdaFechaInstalacion = new Cell(1, 1).Add(new Paragraph(new Text(Convert.ToDateTime(item.fdFechaInstalacionFinal).ToString("dd/MM/yyyy") == "01/01/1900" ? "" : Convert.ToDateTime(item.fdFechaInstalacionFinal).ToString("dd/MM/yyyy"))));//.SetBorder(Border.NO_BORDER);
                    celdaEstadoInstalacion = new Cell(1, 1).Add(new Paragraph(new Text(item.fcDescripcion)));//.SetBorder(Border.NO_BORDER);
                                                                                                             //var cellLogo = new Cell(6, 1).Add(prestaditoLogo.SetAutoScale(true).SetBorder(Border.NO_BORDER)).SetBorder(Border.NO_BORDER);
                                                                                                             //tblInformacionEmpresa.AddCell(cellLogo);
                    tblInformacionEmpresa
                        .AddCell(celdaSolicitud)
                        .AddCell(celdaCliente)
                        .AddCell(celdaFechaIngreso)
                        .AddCell(celdaOficial)
                        .AddCell(celdaArticulo)
                        .AddCell(celdaServicio)
                        .AddCell(celdaEstado)
                        .AddCell(celdaFechaInstalacion)
                        .AddCell(celdaEstadoInstalacion);

                }
                doc.Add(tblInformacionEmpresa);
                doc.Close();
                var bytesStream = ms.ToArray();
                ms = new MemoryStream();
                ms.Write(bytesStream, 0, bytesStream.Length);
                ms.Position = 0;
                return ms;
            }
        }


        public Stream GenerarReporteInstalacionCliente(int fiIDSolicitud)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var baseController = new BaseController();

                var listaDetalleCliente = new sp_OrionSolicitud_Detalle_ClienteListarViewModel();
                var listaEquifaxGarantia = new List<SolicitudesGarantiaViewModel>();


                var connection = contexto.Database.Connection;


                var command = connection.CreateCommand();
                command.CommandText = $"EXEC sp_OrionSolicitud_Detalle_ClienteListar 1, {fiIDSolicitud}";
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var db = ((IObjectContextAdapter)new ORIONDBEntities());
                    listaDetalleCliente = db.ObjectContext.Translate<sp_OrionSolicitud_Detalle_ClienteListarViewModel>(reader).FirstOrDefault();
                    reader.NextResult();
                    reader.NextResult();
                    listaEquifaxGarantia = db.ObjectContext.Translate<SolicitudesGarantiaViewModel>(reader).ToList();
                }



                var paquete = contexto.sp_Paquetes_List().FirstOrDefault(x => x.fiIDPaquete == listaEquifaxGarantia.FirstOrDefault().fiIDPaquete);

                var instalacion = contexto.sp_ProductosInstaladosPorSolicitud_Listar(fiIDSolicitud).ToList();
                var materiales = baseController.GetListProductosConsumibles();
                var solicitudInstalacion = contexto.sp_ContratistaSolicitudInstalacion_ObtenerPorSolicitud(fiIDSolicitud).FirstOrDefault(x => x.fiIDTipoSolicitud == 1);
                var usuarios = contexto.sp_Usuarios_Maestros_ListarUsuarios();

                var tecnico = usuarios.FirstOrDefault(x => x.fiIDUsuario == solicitudInstalacion.fiIDTecnicoAsignado);
                //var agente = usuarios.FirstOrDefault(x => x.fiIDUsuario == Convert.ToInt32(listaDetalleCliente.fcCodigoVendedor));
                var ms = new MemoryStream();

                var pw = new PdfWriter(ms);

                var pdfDocument = new PdfDocument(pw);
                var doc = new Document(pdfDocument, PageSize.LETTER);


                doc.SetMargins(75, 35, 70, 35);

                string pathImageBackGround = System.IO.Path.Combine(MemoryLoadManager.URL, @"Content\img\HOJAS_NOVANET.png");
                var backGround = new Image(ImageDataFactory.Create(pathImageBackGround));
                pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, new BackgroundImageHandler(backGround));


                /*------------------------*/
                var tblInformacionInstalacion = new Table(UnitValue.CreatePercentArray(new float[] { 75f, 25f })).SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                var cellInfo = new Cell(1, 1).Add(new Paragraph(new Text($"Cliente: {listaDetalleCliente.fcNombre}").SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12f)).SetBorder(Border.NO_BORDER);
                var cellFechca = new Cell(1, 1).Add(new Paragraph(new Text($"Fecha: {DateTime.Now:dd/MM/yyyy}").SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12f)).SetBorder(Border.NO_BORDER);
                tblInformacionInstalacion.AddCell(cellInfo).AddCell(cellFechca);
                cellInfo = new Cell(1, 2).Add(new Paragraph(new Text($"Identidad: {listaDetalleCliente.fcIdentidad}").SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12f)).SetBorder(Border.NO_BORDER);
                tblInformacionInstalacion.AddCell(cellInfo);
                cellInfo = new Cell(1, 2).Add(new Paragraph(new Text($"Telefono: {listaDetalleCliente.fcTelefono}").SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12f)).SetBorder(Border.NO_BORDER);
                tblInformacionInstalacion.AddCell(cellInfo);
                cellInfo = new Cell(1, 2).Add(new Paragraph(new Text($"Direccion: {listaDetalleCliente.fcDireccionDetallada}").SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12f)).SetBorder(Border.NO_BORDER);
                tblInformacionInstalacion.AddCell(cellInfo);
                if(paquete != null)
                {
                    cellInfo = new Cell(1, 2).Add(new Paragraph(new Text($"Paquete Contratado: {paquete.fcPaquete}").SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12f)).SetBorder(Border.NO_BORDER);
                    tblInformacionInstalacion.AddCell(cellInfo);
                }
                
                
                cellInfo = new Cell(1, 2).Add(new Paragraph(new Text($"Solicitud: {listaDetalleCliente.fiIDSolicitud}").SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12f)).SetBorder(Border.NO_BORDER);
                tblInformacionInstalacion.AddCell(cellInfo);
                cellInfo = new Cell(1, 2).Add(new Paragraph(new Text($"Vendedor: {listaDetalleCliente.fcCodigoVendedor}").SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12f)).SetBorder(Border.NO_BORDER);
                tblInformacionInstalacion.AddCell(cellInfo);
                cellInfo = new Cell(1, 2).Add(new Paragraph(new Text($"Tecnico: {tecnico.fcNombreCorto}").SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12f)).SetBorder(Border.NO_BORDER);
                tblInformacionInstalacion.AddCell(cellInfo);
                cellInfo = new Cell(1, 2).Add(new Paragraph(new Text($"Empresa: {tecnico.fcNombreComercial}").SetBold()).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12f)).SetBorder(Border.NO_BORDER);
                tblInformacionInstalacion.AddCell(cellInfo);
                doc.Add(tblInformacionInstalacion);
                doc.Add(new Paragraph());
                doc.Add(new Paragraph());
                doc.Add(new Paragraph());
                var tblInformacionProductosInstalacion = new Table(UnitValue.CreatePercentArray(new float[] { 55f, 15f, 15f, 15f })).SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                var encabezado = new Cell(1, 1).SetTextAlignment(TextAlignment.LEFT).Add(new Paragraph("PRODUCTO").SetBold());//.SetBorder(Border.NO_BORDER);
                tblInformacionProductosInstalacion.AddHeaderCell(encabezado);
                encabezado = new Cell(1, 1).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("CANTIDAD").SetBold());
                tblInformacionProductosInstalacion.AddHeaderCell(encabezado);
                encabezado = new Cell(1, 1).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("SERIE").SetBold());
                tblInformacionProductosInstalacion.AddHeaderCell(encabezado);
                encabezado = new Cell(1, 1).SetTextAlignment(TextAlignment.CENTER).Add(new Paragraph("MAC").SetBold());
                tblInformacionProductosInstalacion.AddHeaderCell(encabezado);
                var cellProducto = new Cell();
                var cellCantidad = new Cell();
                var cellCodigoSerie1 = new Cell();
                var cellCodigoSerie2 = new Cell();
                foreach (var item in instalacion)
                {
                    cellProducto = new Cell(1, 1).Add(new Paragraph(new Text(item.fcProducto)).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12f));
                    cellCantidad = new Cell(1, 1).Add(new Paragraph(new Text(item.fnCantidad?.ToString("n") ?? "0"))).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12f);
                    cellCodigoSerie1 = new Cell(1, 1).Add(new Paragraph(new Text(item.fcCodigoSerie1))).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12f);
                    cellCodigoSerie2 = new Cell(1, 1).Add(new Paragraph(new Text(item.fcCodigoSerie2))).SetTextAlignment(TextAlignment.LEFT).SetFontSize(12f);
                    tblInformacionProductosInstalacion.AddCell(cellProducto).AddCell(cellCantidad).AddCell(cellCodigoSerie1).AddCell(cellCodigoSerie2);
                }

                doc.Add(tblInformacionProductosInstalacion);


                doc.Close();
                /*------------------------*/
                var bytesStream = ms.ToArray();
                ms = new MemoryStream();
                ms.Write(bytesStream, 0, bytesStream.Length);
                ms.Position = 0;
                return ms;
            }
        }


        public CrearFacturacionViewModel InformacionDeFactura(int fiIDFacturacionMaestro)
        {
            using(var contexto = new ORIONDBEntities())
            {
                var modelDb = contexto.sp_Facturacion_Maestro_ObtenerPorIdFacturacion(fiIDFacturacionMaestro)?.FirstOrDefault() ?? null;
                var modelDbDetalle = contexto.sp_Facturacion_Detalle_ListarPorIDFacturacionMaestro(fiIDFacturacionMaestro).ToList();

                var modelo = new CrearFacturacionViewModel();

                modelo.fiIDEquifax = modelDb.fiIDEquifax ?? 0;
                modelo.fiIDFacturacionMaestro = modelDb.fiIDFacturacionMaestro;
                modelo.fdFechaRangoInicialFacturacion = modelDb.fdFechaRangoInicialFacturacion ?? DateTime.Now;
                modelo.fdFechaRangoFinalFacturacion = modelDb.fdFechaRangoFinalFacturacion ?? DateTime.Now;
                modelo.fcCAI = modelDb.fcCAI;
                modelo.fcRangoInicialFacturacion = modelDb.fcRangoInicialFacturacion;
                modelo.fcRangoFinalFacturacion = modelDb.fcRangoFinalFacturacion;
                modelo.fcReferenciaDocumento = modelDb.fcReferenciaDocumento;
                modelo.fdFechaTransaccion = modelDb.fdFechaTransaccion ?? DateTime.Now;
                modelo.fcCorrelativoDocumento = modelDb.fcCorrelativoDocumento;
                modelo.fcRTN = modelDb.fcRTN;
                modelo.fcIdentidad = modelDb.fcIdentidad;
                modelo.fcNombreCliente = modelDb.fcNombreCliente;
                modelo.fcTelefonoCliente = modelDb.fcTelefonoCliente;
                modelo.fcToken = modelDb.fcToken;
                modelo.fbEditar = true;
                modelo.fcConcepto = modelDb.fcConcepto;
                modelo.fnSubTotal = modelDb.fnSubTotal ?? 0;
                modelo.fnDescuento = 0;
                modelo.fnImporteExento = modelDb.fnImporteExento ?? 0;
                modelo.fnImporteExonerado = modelDb.fnImporteExonerado ?? 0;
                modelo.fnImporteGravado = modelDb.fnImporteGravado ?? 0;
                modelo.fnImpuestos = modelDb.fnImpuestos ?? 0;
                modelo.fnTotalFactura = modelDb.fnTotalFactura ?? 0;
                modelo.fcCorreoCliente = modelDb.fcCorreoCliente;
                modelo.fnTasadeCambio = modelDb.fnTasadeCambio ?? 0;
                modelo.fcValorEnLetras = NumerosALetras.ConvertirDolaresAletras(modelDb.fnTotalFactura?.ToString("n2") ?? "0");
                modelo.fnValorLempiras = (modelDb.fnTotalFactura ?? 0) * (modelDb.fnTasadeCambio ?? 0);
                modelo.fiIDInventarioMovimientoMaestro = modelDb.fiIDInventarioMovimientoMaestro ?? 0;
                modelo.DetalleFacturacion = new List<ListFacturacionDetalleViewModel>();
                foreach (var item in modelDbDetalle)
                {

                    var detalleFactura = new ListFacturacionDetalleViewModel
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
                        Foto = "",
                        DetalleInventario = new List<Models.Productos.ListInventarioMovimientoDetalleViewModel>()
                    };
                    modelo.DetalleFacturacion.Add(detalleFactura);

                    var detalleInventario = contexto.sp_Inventario_Movimiento_Detalle_GetIDInventarioMovimientoMaestro(modelo.fiIDInventarioMovimientoMaestro).ToList();

                    foreach(var itemInv in detalleInventario)
                    {
                        detalleFactura.DetalleInventario.Add(new ListInventarioMovimientoDetalleViewModel
                        {
                            fbEditado = true,
                            fbPorCodigo = true,
                            fcCodigoSerie1 = itemInv.fcCodigoSerie1,
                            fcCodigoSerie2 = itemInv.fcCodigoSerie2,
                            fcDescripcion = itemInv.fcDescripcion,
                            fcPrecios = itemInv.fcPrecios,
                            fcProducto = itemInv.fcProducto,
                            fcToken = itemInv.fcToken,
                            fcUbicacionDestino = itemInv.fcUbicacionDestino,
                            fcUbicacionInicial = itemInv.fcUbicacionInicial,
                            fiIDInventarioMovimientoDetalle = itemInv.fiIDInventarioMovimientoDetalle,
                            fiIDInventarioMovimientoMaestro = itemInv.fiIDInventarioMovimientoMaestro,
                            fiIDMovimiento = itemInv.fiIDMovimiento,
                            fiIdProducto = itemInv.fiIDProducto ?? 0,
                            fiIDTipoMovimiento = itemInv.fiIDTipoMovimiento,
                            fiIDUbicacionDestino = itemInv.fiIDUbicacionDestino,
                            fiIDUbicacionInicial = itemInv.fiIDUbicacionInicial,
                            fnCantidad = itemInv.fnCantidad,
                            index = 0
                        });
                    }

                }
                return modelo;
            }
        }


        public Stream GenerarFacturaDeVenta(int fiIDFacturacionMaestro)
        {
            using (var contexto = new ORIONDBEntities())
            {
                var ms = new MemoryStream();
                var pw = new PdfWriter(ms);
                var pdfDocument = new PdfDocument(pw);
                var doc = new Document(pdfDocument, PageSize.LETTER);

                doc.SetMargins(20, 20, 5, 20);
                doc.SetProperty(Property.LEADING, new Leading(Leading.MULTIPLIED, 1f));
                PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                doc.SetFontSize(12f).SetFont(font);
                string pathImageNovanetLogo = System.IO.Path.Combine(MemoryLoadManager.URL, @"Content\img\LogoPrestadito.png");
                var prestaditoLogo = new Image(ImageDataFactory.Create(pathImageNovanetLogo));

                string pathImageBackGround = System.IO.Path.Combine(MemoryLoadManager.URL, @"Content\img\HOJAS_NOVANET.png");
                var backGround = new Image(ImageDataFactory.Create(pathImageBackGround));

                pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, new BackgroundImageHandler(backGround));
                var model = InformacionDeFactura(fiIDFacturacionMaestro);

                if (model != null)
                {
                    var tblInformacionEmpresa = new Table(UnitValue.CreatePercentArray(new float[] { 25f, 45f, 30f })).SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                    var cellLogo = new Cell(6, 1).Add(prestaditoLogo.SetAutoScale(true).SetBorder(Border.NO_BORDER)).SetBorder(Border.NO_BORDER);
                    tblInformacionEmpresa.AddCell(cellLogo);

                    var cellInfoEmpresa = new Cell(1, 2).Add(new Paragraph(new Text($"Prestadito S.A de C.V.")).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10f)).SetBorder(Border.NO_BORDER);
                    tblInformacionEmpresa.AddCell(cellInfoEmpresa);

                    cellInfoEmpresa = new Cell(1, 2).Add(new Paragraph(new Text("Barrio Rio de Piedras, 4 calle, 25-26 avenida, Contiguo a Club Arabe")).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10f)).SetBorder(Border.NO_BORDER);
                    tblInformacionEmpresa.AddCell(cellInfoEmpresa);

                    cellInfoEmpresa = new Cell(1, 2).Add(new Paragraph(new Text("SAN PEDRO SULA")).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10f)).SetBorder(Border.NO_BORDER);
                    tblInformacionEmpresa.AddCell(cellInfoEmpresa);

                    cellInfoEmpresa = new Cell(1, 2).Add(new Paragraph(new Text("Tel.(+504)2540-1050")).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10f)).SetBorder(Border.NO_BORDER);
                    tblInformacionEmpresa.AddCell(cellInfoEmpresa);

                    cellInfoEmpresa = new Cell(1, 2).Add(new Paragraph(new Text("contabilidad@miprestadito.com")).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10f)).SetBorder(Border.NO_BORDER);
                    tblInformacionEmpresa.AddCell(cellInfoEmpresa);

                    cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text("Domicilio Fiscal: San Pedro Sula")).SetTextAlignment(TextAlignment.LEFT).SetFontSize(10f)).SetBorder(Border.NO_BORDER);
                    tblInformacionEmpresa.AddCell(cellInfoEmpresa);

                    cellInfoEmpresa = new Cell(1, 1).Add(new Paragraph(new Text($"Factura: Contado")).SetTextAlignment(TextAlignment.RIGHT).SetFontSize(10f)).SetBorder(Border.NO_BORDER);
                    tblInformacionEmpresa.AddCell(cellInfoEmpresa);

                    doc.Add(tblInformacionEmpresa);

                    doc.Add(new Paragraph(new Text($"RTN. 05019016811399")));
                    doc.Add(new Paragraph(new Text($"Factura: #{model.fcReferenciaDocumento}")));
                    doc.Add(new Paragraph(new Text($"CAI: {model.fcCAI}")));

                    doc.Add(new Paragraph());

                    var tablaInfoCliente = new Table(4).SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                    var celdaDescripcion = new Cell(1, 1).Add(new Paragraph(new Text("Cliente:"))).SetBorder(Border.NO_BORDER);
                    var celdaValor = new Cell(1, 1).Add(new Paragraph(new Text(model.fcNombreCliente))).SetBorder(Border.NO_BORDER);

                    tablaInfoCliente.AddCell(celdaDescripcion).AddCell(celdaValor);

                    celdaDescripcion = new Cell(1, 1).Add(new Paragraph(new Text("Fecha:"))).SetBorder(Border.NO_BORDER);
                    celdaValor = new Cell(1, 1).Add(new Paragraph(new Text(Convert.ToDateTime(model.fdFechaTransaccion).ToString("dd/MM/yyyy")))).SetBorder(Border.NO_BORDER);
                    tablaInfoCliente.AddCell(celdaDescripcion).AddCell(celdaValor);



                    celdaDescripcion = new Cell(1, 1).Add(new Paragraph(new Text($"RTN:"))).SetBorder(Border.NO_BORDER);
                    celdaValor = new Cell(1, 3).Add(new Paragraph(new Text($"{model.fcIdentidad}"))).SetBorder(Border.NO_BORDER);
                    tablaInfoCliente.AddCell(celdaDescripcion).AddCell(celdaValor);



                    celdaDescripcion = new Cell(1, 1).Add(new Paragraph(new Text("Direccion:"))).SetBorder(Border.NO_BORDER);
                    celdaValor = new Cell(1, 3).Add(new Paragraph(new Text(""))).SetBorder(Border.NO_BORDER);
                    tablaInfoCliente.AddCell(celdaDescripcion).AddCell(celdaValor);


                    celdaDescripcion = new Cell(1, 1).Add(new Paragraph(new Text("Fecha Limite de Emisión:"))).SetBorder(Border.NO_BORDER);
                    celdaValor = new Cell(1, 3).Add(new Paragraph(new Text(Convert.ToDateTime(model.fdFechaRangoFinalFacturacion).ToString("dd/MM/yyyy")))).SetBorder(Border.NO_BORDER);
                    tablaInfoCliente.AddCell(celdaDescripcion).AddCell(celdaValor);


                    celdaDescripcion = new Cell(1, 1).Add(new Paragraph(new Text("Rango de Autorización:"))).SetBorder(Border.NO_BORDER);
                    celdaValor = new Cell(1, 3).Add(new Paragraph(new Text($"{model.fcRangoInicialFacturacion} al {model.fcRangoFinalFacturacion}"))).SetBorder(Border.NO_BORDER);
                    tablaInfoCliente.AddCell(celdaDescripcion).AddCell(celdaValor);


                    celdaDescripcion = new Cell(1, 1).Add(new Paragraph(new Text("No. Declaracion:"))).SetBorder(Border.NO_BORDER);
                    celdaValor = new Cell(1, 3).Add(new Paragraph(new Text($"{model.fcCorrelativoDocumento}"))).SetBorder(Border.NO_BORDER);
                    tablaInfoCliente.AddCell(celdaDescripcion).AddCell(celdaValor);



                    doc.Add(tablaInfoCliente);
                    doc.Add(new Paragraph(""));
                    var tablaProducto = new Table(UnitValue.CreatePercentArray(new float[] { 10f, 60f, 15f, 15f })).UseAllAvailableWidth().SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                    tablaProducto.AddHeaderCell(new Paragraph("CANT.").SetTextAlignment(TextAlignment.CENTER));
                    tablaProducto.AddHeaderCell(new Paragraph("DESCRIPCION"));
                    tablaProducto.AddHeaderCell(new Paragraph("VALOR").SetTextAlignment(TextAlignment.RIGHT));
                    tablaProducto.AddHeaderCell(new Paragraph("TOTAL").SetTextAlignment(TextAlignment.RIGHT));

                    var detalleVenta = contexto.sp_Facturacion_Detalle_ListarPorIDFacturacionMaestro(fiIDFacturacionMaestro).ToList();

                    var celdaProdCantidad = new Cell(1, 1);
                    var celdaProdDescripcion = new Cell(1, 1);
                    var celdaProdValor = new Cell(1, 1);
                    var celdaProdTotal = new Cell(1, 1);
                    var i = 1; 
                    foreach (var item in model.DetalleFacturacion)
                    {
                        doc.SetFontSize(9f).SetFont(font);
                        celdaProdCantidad = new Cell(1, 1).Add(new Paragraph($"{item.fnCantidad:0.##}").SetTextAlignment(TextAlignment.CENTER));
                        celdaProdDescripcion = new Cell(1, 1).Add(new Paragraph(item.fcProducto));
                        celdaProdValor = new Cell(1, 1).Add(new Paragraph(new Text($"$ {Convert.ToDecimal(item.fnValorVentaDeContado):n2}")).SetTextAlignment(TextAlignment.RIGHT));
                        celdaProdTotal = new Cell(1, 1).Add(new Paragraph(new Text($"$ {Convert.ToDecimal(item.fnSubTotal):n2}")).SetTextAlignment(TextAlignment.RIGHT));
                        tablaProducto.AddCell(celdaProdCantidad).AddCell(celdaProdDescripcion).AddCell(celdaProdValor).AddCell(celdaProdTotal);

                        foreach(var itemINV in item.DetalleInventario)
                        {
                            celdaProdCantidad = new Cell(1, 1).Add(new Paragraph($"").SetTextAlignment(TextAlignment.CENTER));
                            celdaProdDescripcion = new Cell(1, 1).Add(new Paragraph($"SERIE: {itemINV.fcCodigoSerie1}"));
                            celdaProdValor = new Cell(1, 1).Add(new Paragraph(new Text($"")).SetTextAlignment(TextAlignment.RIGHT));
                            celdaProdTotal = new Cell(1, 1).Add(new Paragraph(new Text($"")).SetTextAlignment(TextAlignment.RIGHT));
                            tablaProducto.AddCell(celdaProdCantidad).AddCell(celdaProdDescripcion).AddCell(celdaProdValor).AddCell(celdaProdTotal);
                        }
                    }

                    



                    celdaProdCantidad = new Cell(1, 1);
                    celdaProdDescripcion = new Cell(1, 1).Add(new Paragraph("Sub Total").SetTextAlignment(TextAlignment.RIGHT));
                    celdaProdValor = new Cell(1, 1);
                    celdaProdTotal = new Cell(1, 1).Add(new Paragraph(new Text($"$ {Convert.ToDecimal(model.fnSubTotal):n2}")).SetTextAlignment(TextAlignment.RIGHT));
                    tablaProducto.AddCell(celdaProdCantidad).AddCell(celdaProdDescripcion).AddCell(celdaProdValor).AddCell(celdaProdTotal);


                    celdaProdCantidad = new Cell(1, 1);
                    celdaProdDescripcion = new Cell(1, 1).Add(new Paragraph("Descuento").SetTextAlignment(TextAlignment.RIGHT));
                    celdaProdValor = new Cell(1, 1);
                    celdaProdTotal = new Cell(1, 1).Add(new Paragraph(new Text($"$ 0.00")).SetTextAlignment(TextAlignment.RIGHT));
                    tablaProducto.AddCell(celdaProdCantidad).AddCell(celdaProdDescripcion).AddCell(celdaProdValor).AddCell(celdaProdTotal);

                    celdaProdCantidad = new Cell(1, 1);
                    celdaProdDescripcion = new Cell(1, 1).Add(new Paragraph("Exento").SetTextAlignment(TextAlignment.RIGHT));
                    celdaProdValor = new Cell(1, 1);
                    celdaProdTotal = new Cell(1, 1).Add(new Paragraph(new Text($"$ {Convert.ToDecimal(model.fnImporteExento):n2}")).SetTextAlignment(TextAlignment.RIGHT));
                    tablaProducto.AddCell(celdaProdCantidad).AddCell(celdaProdDescripcion).AddCell(celdaProdValor).AddCell(celdaProdTotal);


                    celdaProdCantidad = new Cell(1, 1);
                    celdaProdDescripcion = new Cell(1, 1).Add(new Paragraph("Exonerado").SetTextAlignment(TextAlignment.RIGHT));
                    celdaProdValor = new Cell(1, 1);
                    celdaProdTotal = new Cell(1, 1).Add(new Paragraph(new Text($"$ {Convert.ToDecimal(model.fnImporteExonerado).ToString("n2")}")).SetTextAlignment(TextAlignment.RIGHT));
                    tablaProducto.AddCell(celdaProdCantidad).AddCell(celdaProdDescripcion).AddCell(celdaProdValor).AddCell(celdaProdTotal);


                    celdaProdCantidad = new Cell(1, 1);
                    celdaProdDescripcion = new Cell(1, 1).Add(new Paragraph("Gravado 15%").SetTextAlignment(TextAlignment.RIGHT));
                    celdaProdValor = new Cell(1, 1);
                    celdaProdTotal = new Cell(1, 1).Add(new Paragraph(new Text($"$ {Convert.ToDecimal(model.fnImporteGravado).ToString("n2")}")).SetTextAlignment(TextAlignment.RIGHT));
                    tablaProducto.AddCell(celdaProdCantidad).AddCell(celdaProdDescripcion).AddCell(celdaProdValor).AddCell(celdaProdTotal);

                    celdaProdCantidad = new Cell(1, 1);
                    celdaProdDescripcion = new Cell(1, 1).Add(new Paragraph("ISV 15%").SetTextAlignment(TextAlignment.RIGHT));
                    celdaProdValor = new Cell(1, 1);
                    celdaProdTotal = new Cell(1, 1).Add(new Paragraph(new Text($"$ {Convert.ToDecimal(model.fnImpuestos).ToString("n2")}")).SetTextAlignment(TextAlignment.RIGHT));
                    tablaProducto.AddCell(celdaProdCantidad).AddCell(celdaProdDescripcion).AddCell(celdaProdValor).AddCell(celdaProdTotal);

                    celdaProdCantidad = new Cell(1, 1);
                    celdaProdDescripcion = new Cell(1, 1).Add(new Paragraph("TOTAL").SetTextAlignment(TextAlignment.RIGHT));
                    celdaProdValor = new Cell(1, 1);
                    celdaProdTotal = new Cell(1, 1).Add(new Paragraph(new Text($"$ {Math.Round(Convert.ToDecimal(model.fnTotalFactura), 0):n0}")).SetTextAlignment(TextAlignment.RIGHT));
                    tablaProducto.AddCell(celdaProdCantidad).AddCell(celdaProdDescripcion).AddCell(celdaProdValor).AddCell(celdaProdTotal);


                    celdaProdCantidad = new Cell(1, 1);
                    celdaProdDescripcion = new Cell(1, 1).Add(new Paragraph("Tasa de Cambio").SetTextAlignment(TextAlignment.RIGHT));
                    celdaProdValor = new Cell(1, 1);
                    celdaProdTotal = new Cell(1, 1).Add(new Paragraph(new Text($"L. {Convert.ToDecimal(model.fnTasadeCambio).ToString("n4")}")).SetTextAlignment(TextAlignment.RIGHT));
                    tablaProducto.AddCell(celdaProdCantidad).AddCell(celdaProdDescripcion).AddCell(celdaProdValor).AddCell(celdaProdTotal);

                    celdaProdCantidad = new Cell(1, 1);
                    celdaProdDescripcion = new Cell(1, 1).Add(new Paragraph("Valor en Lempiras").SetTextAlignment(TextAlignment.RIGHT));
                    celdaProdValor = new Cell(1, 1);
                    celdaProdTotal = new Cell(1, 1).Add(new Paragraph(new Text($"L. {Convert.ToDecimal(model.fnValorLempiras).ToString("n2")}")).SetTextAlignment(TextAlignment.RIGHT));
                    tablaProducto.AddCell(celdaProdCantidad).AddCell(celdaProdDescripcion).AddCell(celdaProdValor).AddCell(celdaProdTotal);

                    doc.Add(tablaProducto);

                    doc.Add(new Paragraph($"MONTO EN LETRAS: {NumerosALetras.ConvertirDolaresAletras(Convert.ToDecimal(model.fnTotalFactura).ToString("n2"))}. EXACTOS\n\n\n\n\n").SetFontSize(9f));


                    doc.SetFontSize(12f).SetFont(font);

                    var tblFirmas = new Table(UnitValue.CreatePercentArray(new float[] { 15f, 17f, 2f, 18f, 14f, 2f, 16f, 16f })).SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                    tblFirmas.AddCell(new Cell(1, 1).Add(new Paragraph("No. Orden Exenta:").SetTextAlignment(TextAlignment.JUSTIFIED)).SetBorder(Border.NO_BORDER).SetFontSize(9));
                    tblFirmas.AddCell(new Cell(1, 1).Add(new Paragraph("")).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1f)));
                    tblFirmas.AddCell(new Cell(1, 1).SetBorder(Border.NO_BORDER));


                    tblFirmas.AddCell(new Cell(1, 1).Add(new Paragraph("Constancia Exonerada:").SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER).SetFontSize(8));
                    tblFirmas.AddCell(new Cell(1, 1).Add(new Paragraph("")).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1f)));
                    tblFirmas.AddCell(new Cell(1, 1).SetBorder(Border.NO_BORDER));

                    tblFirmas.AddCell(new Cell(1, 1).Add(new Paragraph("No. Registro SAG:").SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER).SetFontSize(9));
                    tblFirmas.AddCell(new Cell(1, 1).Add(new Paragraph("")).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1f)));

                    tblFirmas.AddCell(new Cell(1, 8).Add(new Paragraph("Original: Cliente").SetFontSize(9)).SetBorder(Border.NO_BORDER));
                    tblFirmas.AddCell(new Cell(1, 8).Add(new Paragraph("Copia: Obligado Tributario").SetFontSize(9)).SetBorder(Border.NO_BORDER));
                    doc.Add(tblFirmas);
                    doc.Add(new Paragraph("\n\n\n\n"));

                    var tblLineaFirmaYSello = new Table(new float[] { 20f, 60f, 20f }).SetWidth(UnitValue.CreatePercentValue(100)).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                    tblLineaFirmaYSello.AddCell(new Cell(1, 1).SetBorder(Border.NO_BORDER));
                    tblLineaFirmaYSello.AddCell(new Cell(1, 1).SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(1f)));
                    tblLineaFirmaYSello.AddCell(new Cell(1, 1).SetBorder(Border.NO_BORDER));
                    doc.Add(tblLineaFirmaYSello);
                    doc.Add(new Paragraph("Firma y Sello\n").SetTextAlignment(TextAlignment.CENTER).SetFontSize(10));
                    doc.Add(new Paragraph("LA FACTURA ES BENEFICIO DE TODOS, EXIJALA").SetTextAlignment(TextAlignment.CENTER).SetFontSize(8));

                }
                doc.Close();
                var bytesStream = ms.ToArray();
                ms = new MemoryStream();
                ms.Write(bytesStream, 0, bytesStream.Length);
                ms.Position = 0;
                return ms;
            }
        }
    }



}