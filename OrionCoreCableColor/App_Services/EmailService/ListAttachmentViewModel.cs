using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace OrionCoreCableColor.App_Services.EmailService
{
    public class ListAttachmentViewModel
    {
        public Attachment Archivo { get; set; }
        public string AttachmentName { get; set; }
    }
}