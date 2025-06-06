using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.EmailTemplateService
{
    public class EmailTemplateServiceModel
    {
        public int IdEmailTemplate { get; set; }
        public string IdCustomer { get; set; }
        public string IdLoan { get; set; }
        public string CustomerEmail { get; set; }

        public List<string> ListCustomerEmail { get; set; }
        public string Comment { get; set; }
        public string Token { get; set; }
        public List<string> List_CC = new List<string>();
        public string AttachmentBaseName { get; set; }
        public string HtmlBody { get; set; }
        public int IdCliente { get; set; }
        public Nullable<int> idEquifax { get; set; }
        public int IDSolicitud { get; set; }
        public int IDFirma { get; set; }
        public string firma { get; set; }
        public int IdTransaccion { get; set; }
        public int TipoSolicitud { get; set; }
        public int IDSolicitudContratrista { get; set; }
        public int fiIDVendedorRepartidor { get; set; }
        public int fiIDDistribuidor { get; set; }

        public HttpPostedFileBase  Archivo { get; set; }
    }
}