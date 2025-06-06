using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace OrionCoreCableColor.App_Services.EmailService
{
    public class SendEmailService
    {

        //public string From_Email = "notifications@miprestadito.com";// "no-responder@miprestadito.com";
        //private string From_Password = "P@ssword155";
        
        //public string From_Email = "systembot@miprestadito.com";// "no-responder@miprestadito.com";
        //private string From_Password = "iPwf@p3q";
        
        
        //private string Host = "mail.miprestadito.com";
        //private int Port = 587;

        public string From_Email = "systembot@novanetgroup.com";
        private string From_Password = "SystemBot2024#";
        private string Host = "mail.novanetgroup.com";
        private int Port = 25;

        // private string From_User = "sac@miprestadito.com";
        public async Task<bool> SendEmailAsync(SendEmailViewModel model)
        {
            // Initialization.  
            bool isSend = false;

            try
            {
                // Initialization.  
                var body = model.Body;
                var message = new MailMessage();

                // Settings.  
                message.To.Add(new MailAddress(model.DestinationEmail));
                message.From = new MailAddress(From_Email, string.IsNullOrEmpty(model.EmailName) ? "Prestadito" : model.EmailName);
                message.Subject = !string.IsNullOrEmpty(model.Subject) ? model.Subject : "Notification";
                message.IsBodyHtml = true;
                message.Body = body;

                foreach (var email in model.List_CC)
                {
                    message.CC.Add(new MailAddress(email));
                }

                if (model.Attachment != null) message.Attachments.Add(model.Attachment);

                using (var smtp = new SmtpClient(Host, Port))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(From_Email, From_Password);

                    smtp.EnableSsl = false;
                    //   smtp.ClientCertificates.Add(new X509Certificate());
                    //smtp.ConnectionProtocols = ConnectionProtocols.Ssl;

                    // var certificate = ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                    //  ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                    await smtp.SendMailAsync(message);
                    isSend = true;
                }


            }
            catch (Exception ex)
            {
                await SendEmailException(ex, "Send email");
                throw ex;
            }

            // info.  
            return isSend;
        }


        public async Task<bool> SendEmailManyDestinationsAsync(SendEmailViewModel model)
        {
            // Initialization.  
            bool isSend = false;

            try
            {
                // Initialization.  
                var body = model.Body;
                var message = new MailMessage();

                // Settings.  
                foreach(var destinatario in model.ListDestinationEmail)
                {
                    message.To.Add(new MailAddress(destinatario));
                }
                
                message.From = new MailAddress(From_Email, string.IsNullOrEmpty(model.EmailName) ? "Prestadito" : model.EmailName);
                message.Subject = !string.IsNullOrEmpty(model.Subject) ? model.Subject : "Notification";
                message.IsBodyHtml = true;
                message.Body = body;

                foreach (var email in model.List_CC)
                {
                    message.CC.Add(new MailAddress(email));
                }

                if (model.Attachment != null) message.Attachments.Add(model.Attachment);

                using (var smtp = new SmtpClient(Host, Port))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(From_Email, From_Password);

                    smtp.EnableSsl = false;
                    //   smtp.ClientCertificates.Add(new X509Certificate());
                    //smtp.ConnectionProtocols = ConnectionProtocols.Ssl;

                    // var certificate = ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                    //  ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                    await smtp.SendMailAsync(message);
                    isSend = true;
                }


            }
            catch (Exception ex)
            {
                await SendEmailException(ex, "Send email");
                throw ex;
            }

            // info.  
            return isSend;
        }


        public async Task<bool> SendEmailManyAttachmentAsync(SendEmailViewModel model)
        {
            // Initialization.  
            bool isSend = false;

            try
            {
                // Initialization.  
                var body = model.Body;
                var message = new MailMessage();

                // Settings.  
                message.To.Add(new MailAddress(model.DestinationEmail));
                message.From = new MailAddress(From_Email, string.IsNullOrEmpty(model.EmailName) ? "Prestadito" : model.EmailName);
                message.Subject = !string.IsNullOrEmpty(model.Subject) ? model.Subject : "Notification";
                message.IsBodyHtml = true;
                message.Body = body;

                foreach (var email in model.List_CC)
                {
                    message.CC.Add(new MailAddress(email));
                }

                foreach(var item in model.ArchivosVarios)
                {
                    if (item != null) message.Attachments.Add(item.Archivo);
                }
                

                using (var smtp = new SmtpClient(Host, Port))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(From_Email, From_Password);

                    smtp.EnableSsl = false;
                    //   smtp.ClientCertificates.Add(new X509Certificate());
                    //smtp.ConnectionProtocols = ConnectionProtocols.Ssl;

                    // var certificate = ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                    //  ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

                    await smtp.SendMailAsync(message);
                    isSend = true;
                }


            }
            catch (Exception ex)
            {
                await SendEmailException(ex, "Send email");
                throw ex;
            }

            // info.  
            return isSend;
        }


        public async Task<bool> SendEmailException(Exception e, string title)
        {
            // Initialization.  
            bool isSend = false;

            try
            {
                var message = new MailMessage();

                // Settings.  
                message.To.Add(new MailAddress("sistemas@miprestadito.com"));
                message.To.Add(new MailAddress("kevin.santos@miprestadito.com"));
                message.From = new MailAddress(From_Email, "Exception Error");
                message.Subject = "Exception Error!";
                message.Body = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace;


                using (var smtp = new SmtpClient(Host, Port))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(From_Email, From_Password);
                    smtp.EnableSsl = false;

                    // var certificate = ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                    //   smtp.ClientCertificates.Add(new X509Certificate());
                    await smtp.SendMailAsync(message);
                    isSend = true;
                }
            }
            catch (Exception ex)
            {
                // Info  
                throw ex;
            }

            // info.  
            return isSend;
        }


        public bool SendEmailExceptionWithOutAsync(Exception e, string title)
        {
            // Initialization.  
            bool isSend = false;

            try
            {
                var message = new MailMessage();

                // Settings.  
                message.To.Add(new MailAddress("sistemas@miprestadito.com"));
                message.From = new MailAddress(From_Email, "Exception Error");
                message.Subject = "Exception Error!";
                message.Body = (e.InnerException?.Message ?? e.Message) + ": " + e.StackTrace;


                using (var smtp = new SmtpClient(Host, Port))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(From_Email, From_Password);
                    smtp.EnableSsl = false;

                    // var certificate = ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                    //   smtp.ClientCertificates.Add(new X509Certificate());
                    smtp.Send(message);
                    isSend = true;
                }
            }
            catch (Exception ex)
            {
                // Info  
                throw ex;
            }

            // info.  
            return isSend;
        }

        internal Task SendEmailAsync(Dictionary<string, string> emailGeneratedToSendReferencia)
        {
            throw new NotImplementedException();
        }
    }

}