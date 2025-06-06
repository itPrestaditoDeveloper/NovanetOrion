using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace OrionCoreCableColor.App_Services.EmailService
{
    public class SendEmailViewModel
    {
        public string DestinationEmail { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        //optional

        public string EmailName { get; set; }

        public Attachment Attachment { get; set; }
        public string AttachmentName { get; set; }


        public List<string> List_CC = new List<string>();
        public string firma { get; set; }
        public HttpPostedFileBase Archivo { get; set; }
        public List<string> ListDestinationEmail { get; set; }

        public List<ListAttachmentViewModel> ArchivosVarios { get; set; }
    }
}