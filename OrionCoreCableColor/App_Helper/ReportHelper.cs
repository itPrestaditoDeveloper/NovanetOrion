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
using OrionCoreCableColor.DbConnection.CoreFinancieroDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.App_Helper
{
    public class ReportHelper
    {
    }

    public class HeaderEventHandler : IEventHandler
    {
        public Image Img { get; set; }
        public HeaderEventHandler(Image _img)
        {
            Img = _img;

        }


        public void HandleEvent(Event @event)
        {
            var docEvent = (PdfDocumentEvent)@event;
            var pdfDoc = docEvent.GetDocument();
            var page = docEvent.GetPage();
            //var pageSize = page.GetPageSize();

            Rectangle rootArea = new Rectangle(35, page.GetPageSize().GetTop() - 70, page.GetPageSize().GetRight() - 70, 50);




            var canvas = new Canvas(page, rootArea);
            canvas.Add(GetTable(docEvent)).Close();
        }


        #region Codigo para tenerlo de referencia
        ///codigo para usar despues
        //float[] cellWidth = { 20f, 80f };
        //var tableEvent = new Table(UnitValue.CreatePercentArray(cellWidth)).UseAllAvailableWidth();
        //Style styleCell = new Style().SetBorder(Border.NO_BORDER);
        //Style styleText = new Style().SetTextAlignment(TextAlignment.RIGHT);
        //Cell cell = new Cell().Add(Img.SetAutoScale(true)).SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

        //tableEvent.AddCell(cell.AddStyle(styleCell).SetTextAlignment(TextAlignment.LEFT));


        //var bold = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);
        //cell = new Cell().Add(new Paragraph("Reporte diario\n").SetFont(bold))
        //    .Add(new Paragraph("Departamento de cobros \n"))
        //    .Add(new Paragraph("fecha de emision: "))
        //    .AddStyle(styleText).AddStyle(styleCell);

        ////cell = new Cell().Add()
        //tableEvent.AddCell(cell);

        //return tableEvent;
        #endregion

        public Table GetTable(PdfDocumentEvent docEvent)
        {
            float[] cellWidth = { 100f };
            var tableEvent = new Table(UnitValue.CreatePercentArray(cellWidth)).UseAllAvailableWidth();
            Style styleCell = new Style().SetBorder(Border.NO_BORDER);
            Style styleText = new Style().SetTextAlignment(TextAlignment.CENTER);
            Cell cell = new Cell().Add(Img.SetAutoScale(true).SetBorder(Border.NO_BORDER).SetHorizontalAlignment(HorizontalAlignment.CENTER)).SetBorder(Border.NO_BORDER);//.SetBorder(new SolidBorder(ColorConstants.BLACK, 1));

            tableEvent.AddCell(cell.AddStyle(styleCell).SetTextAlignment(TextAlignment.CENTER));


            var bold = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);
            cell = new Cell().Add(new Paragraph("\n").SetFont(bold)).AddStyle(styleText).AddStyle(styleCell);

            //cell = new Cell().Add()
            tableEvent.AddCell(cell);

            return tableEvent;
        }
    }

    public class BackgroundImageHandler : IEventHandler
    {
        protected PdfExtGState gState;
        public Image img { get; set; }
        public BackgroundImageHandler(Image _img)
        {
            gState = new PdfExtGState().SetFillOpacity(0.7f);
            this.img = _img;
        }

        public void HandleEvent(Event @event)
        {
            var docEvent = (PdfDocumentEvent)@event;
            var pdfDoc = docEvent.GetDocument();
            var page = docEvent.GetPage();
            var pageSize = page.GetPageSize();


            var pdfCanvas = new PdfCanvas(page.GetLastContentStream(), page.GetResources(), pdfDoc);
            pdfCanvas.SaveState().SetExtGState(gState);

            var canvas = new Canvas(pdfCanvas, pageSize);
            canvas.Add(img.ScaleAbsolute(pageSize.GetWidth(), pageSize.GetHeight()));
        }
    }
}