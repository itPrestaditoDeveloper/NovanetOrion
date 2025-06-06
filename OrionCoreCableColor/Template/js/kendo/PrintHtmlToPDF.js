function ExportHtmlToPdf(element, docName) {

    kendo.drawing.drawDOM(element,
        {
            forcePageBreak: ".page-break", // add this class to each element where you want manual page break
            paperSize: "letter",
            margin: { top: "1.5cm", bottom: "1cm", right:"0.5cm", left:"0.5cm" },
            scale: 0.5,
            height: 500,
            template: $("#page-template").html()
        })
        .then(function (group) {
            kendo.drawing.pdf.saveAs(group, docName + ".pdf")
        });



}