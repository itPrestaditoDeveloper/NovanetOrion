using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace OrionCoreCableColor.App_Helper
{
    public static class MensajeDeTexto
    {

        public static void EnviarNumeroToken(string nombreCliente, int IDSolicitud, string pcTelefono,string Token)
        {
            //var pruebaTelefono = "99551041";
            var pcNombreCliente = "Angel";
            var lcParametros = "&IDSOL=" + IDSolicitud ;
            string resultado = DataCrypt.Encriptar(lcParametros);
            //var pruebaTelefono = "31733649";

            //string lcURLCoreFinanciero = "https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&DESTADDR=504" + pcTelefono.Trim() + @"&MESSAGE= Estimado Inversionista, se le notifica que tiene una transferencia pendiente del cliente: "+ pcNombreCliente +".Por un monto de: "+ pcValorAprestar + ", Estar pendiente de su Email, para mas informacion se puede ingresar a este link:" + "https://inversionistas.miprestadito.com/prestamo/index/"+ NumeroPrestamo.Trim();
            string lcURLCoreFinanciero = "https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&DESTADDR=504" +
                  pcTelefono.Trim() + @"&MESSAGE= NOVANET le informa" + "\n Estimado Cliente: " + nombreCliente + " le proporcionaremos un token para poder habilitar la firma de sus documentos de forma digital. El token proporcionado deberá colocarlo en el formulario que se le proporciona en su correo electronico";
            string lcURLCoreFinancieroToken = "https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&DESTADDR=504" + pcTelefono.Trim() + @"&MESSAGE=" + Token;

            HttpWebRequest request = WebRequest.Create(lcURLCoreFinanciero) as HttpWebRequest;
            request.Accept = "text/xml";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            WebHeaderCollection header = response.Headers;
            var encoding = ASCIIEncoding.ASCII;

            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                string responseText = reader.ReadToEnd();
            }

            HttpWebRequest requestToken = WebRequest.Create(lcURLCoreFinancieroToken) as HttpWebRequest;
            requestToken.Accept = "text/xml";
            HttpWebResponse responseToken = (HttpWebResponse)requestToken.GetResponse();
            WebHeaderCollection headerToken = responseToken.Headers;
            var encodingToken = ASCIIEncoding.ASCII;


            using (var readerToken = new System.IO.StreamReader(responseToken.GetResponseStream(), encoding))
            {
                string responseTextToken = readerToken.ReadToEnd();
            }

        }

        public static void EnviarSMSPersonalizado(string nombreCliente,string pcTelefono, string Mensaje)
        {
            //pcTelefono = "99551041";
            var pcNombreCliente = "Angel";
            //var pruebaTelefono = "31733649";

            //string lcURLCoreFinanciero = "https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&DESTADDR=504" + pcTelefono.Trim() + @"&MESSAGE= Estimado Inversionista, se le notifica que tiene una transferencia pendiente del cliente: "+ pcNombreCliente +".Por un monto de: "+ pcValorAprestar + ", Estar pendiente de su Email, para mas informacion se puede ingresar a este link:" + "https://inversionistas.miprestadito.com/prestamo/index/"+ NumeroPrestamo.Trim();
            string lcURLCoreFinanciero = "https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&DESTADDR=504" +
                  pcTelefono.Trim() + @"&MESSAGE= NOVANET le informa" + "\n Estimado Cliente: " + nombreCliente + "\n" + Mensaje;
            
            HttpWebRequest request = WebRequest.Create(lcURLCoreFinanciero) as HttpWebRequest;
            request.Accept = "text/xml";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            WebHeaderCollection header = response.Headers;
            var encoding = ASCIIEncoding.ASCII;

            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                string responseText = reader.ReadToEnd();
            }

            

        }

        public static void EnviarNumero(string nombreCliente, int idEquiFax, string pcTelefono)
        {
            var pruebaTelefono = "88136509";
            var pcNombreCliente = "Angel";
            //var pruebaTelefono = "31733649";
            //string lcURLCoreFinanciero = "https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&DESTADDR=504" + pcTelefono.Trim() + @"&MESSAGE= Estimado Inversionista, se le notifica que tiene una transferencia pendiente del cliente: "+ pcNombreCliente +".Por un monto de: "+ pcValorAprestar + ", Estar pendiente de su Email, para mas informacion se puede ingresar a este link:" + "https://inversionistas.miprestadito.com/prestamo/index/"+ NumeroPrestamo.Trim();
            string lcURLCoreFinanciero = "https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&DESTADDR=504" +
                  pcTelefono.Trim() + @"&MESSAGE= NOVANET le informa" + "\nEstimado Cliente: " + nombreCliente + "nos ayuda llenando los siguientes datos del siguiente link: " + MemoryLoadManager.UrlWeb + "/DatosCliente/ViewFormDatosCliente/" + idEquiFax;

            HttpWebRequest request = WebRequest.Create(lcURLCoreFinanciero) as HttpWebRequest;
            request.Accept = "text/xml";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            WebHeaderCollection header = response.Headers;
            var encoding = ASCIIEncoding.ASCII;

            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                string responseText = reader.ReadToEnd();
            }
        }
        public static void EnviarNumeroReferencias(int fiIDReferencia,string nombrerefencia,string nombreCliente, int idEquiFax, string pcTelefono,string mensaje)
        {
            var pruebaTelefono = "88136509";
            var pcNombreCliente = "Angel";
          
            //var pruebaTelefono = "31733649";
            //string lcURLCoreFinanciero = "https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&DESTADDR=504" + pcTelefono.Trim() + @"&MESSAGE= Estimado Inversionista, se le notifica que tiene una transferencia pendiente del cliente: "+ pcNombreCliente +".Por un monto de: "+ pcValorAprestar + ", Estar pendiente de su Email, para mas informacion se puede ingresar a este link:" + "https://inversionistas.miprestadito.com/prestamo/index/"+ NumeroPrestamo.Trim();
            string lcURLCoreFinanciero = "https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&DESTADDR=504" +
                  pcTelefono.Trim() + @"&MESSAGE= NOVANET le informa" + "\nEstimado : " + nombrerefencia + " ha recibido este mensaje, porque la persona  " + nombreCliente+ " lo ha puesto como referencia, nos ayuda llenando los siguientes datos del siguiente link " + MemoryLoadManager.UrlWeb + "/DatosCliente/ViewFormReferenciasCliente/" + fiIDReferencia;



            HttpWebRequest request = WebRequest.Create(lcURLCoreFinanciero) as HttpWebRequest;
            request.Accept = "text/xml";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            WebHeaderCollection header = response.Headers;
            var encoding = ASCIIEncoding.ASCII;

            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                string responseText = reader.ReadToEnd();
            }
        }

        public static void EnviarLinkPago(string nombreCliente, int idEquiFax, string pcTelefono, string LinkPago, string correo)
        {
            var pruebaTelefono = "88136509";
            var pcNombreCliente = "Angel";

            //var pruebaTelefono = "31733649";

            //string lcURLCoreFinanciero = "https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&DESTADDR=504" +
            //pruebaTelefono.Trim() + @"&MESSAGE= Estimado Cliente: " + nombreCliente + "  Se le envio su link de pago a su correo Electronico:" + correo;

            string lcURLCoreFinanciero = "https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&DESTADDR=504" +
            pcTelefono.Trim() + @"&MESSAGE= NOVANET le informa" + "\nEstimado Cliente: " + nombreCliente + "  Se le envio su link de pago a su correo Electronico:" + correo;

            HttpWebRequest request = WebRequest.Create(lcURLCoreFinanciero) as HttpWebRequest;
            request.Accept = "text/xml";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            WebHeaderCollection header = response.Headers;
            var encoding = ASCIIEncoding.ASCII;

            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                string responseText = reader.ReadToEnd();
            }
        }


        public static void EnviarLinkGeoLocation(string nombreCliente, int idEquiFax, string pcTelefono, string LinkGeoLocation)
        {
            var pruebaTelefono = "31766687";
            var pcNombreCliente = "Angel";
            //var pruebaTelefono = "31733649";
            //string lcURLCoreFinanciero = "https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&DESTADDR=504" +
            //pruebaTelefono.Trim() + @"&MESSAGE= Estimado Cliente: " + pcNombreCliente + "Su link de pago es el siguiente: " + HttpUtility.UrlEncode(LinkPago) + idEquiFax;

            string lcURLCoreFinanciero = "https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&DESTADDR=504" +
            pcTelefono.Trim() + @"&MESSAGE= NOVANET le informa" + "\nEstimado Cliente: " + nombreCliente + " envie su ubicacion de domicilio cuando este en su hogar, por favor asegurese de tener la ubicación de su telefono activada y permitir el acceso a ella: "+ MemoryLoadManager.UrlWeb + HttpUtility.UrlEncode("/DatosCliente/ViewFormMapa/") + idEquiFax;

            HttpWebRequest request = WebRequest.Create(lcURLCoreFinanciero) as HttpWebRequest;
            request.Accept = "text/xml";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            WebHeaderCollection header = response.Headers;
            var encoding = ASCIIEncoding.ASCII;

            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                string responseText = reader.ReadToEnd();
            }
        }
        public static void EnviarMensajeATecnico(string nombreCliente, int idEquiFax, string pcTelefono,string ubicacion,string tecnico,string informacionPaquete)
        {
            try
            {
                string lat = ubicacion.Split(',')[0];
                string longi = ubicacion.Split(',')[1];
                string urlMaps = $"https://www.google.com/maps?z=12&t=k&q={lat},{longi}";
                //var pruebaTelefono = "31733649";
                //string lcURLCoreFinanciero = "https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&DESTADDR=504" +
                //pruebaTelefono.Trim() + @"&MESSAGE= Estimado Cliente: " + pcNombreCliente + "Su link de pago es el siguiente: " + HttpUtility.UrlEncode(LinkPago) + idEquiFax;

                string lcURLCoreFinanciero = "https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&DESTADDR=504" +
                pcTelefono.Trim() + @"&MESSAGE= NOVANET le informa"+ "\nEstimado: " + tecnico + " se le ha asignado la siguiente instalación: Cliente: " + nombreCliente + "\nUbicación:" + HttpUtility.UrlEncode(urlMaps) +
               "\n"+ informacionPaquete;


                HttpWebRequest request = WebRequest.Create(lcURLCoreFinanciero) as HttpWebRequest;
                request.Accept = "text/xml";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                WebHeaderCollection header = response.Headers;
                var encoding = ASCIIEncoding.ASCII;

                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                {
                    string responseText = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        
        }

        public static void EnviarMensajeInstalacionaCliente(string nombreCliente, int idEquiFax, string pcTelefono, string tecnico, string informacionPaquete,string telefonoTecnico,string identidad)
        {
            try
            {
                string urlqr = MemoryLoadManager.UrlWeb + "/DatosCliente/ViewFormQR/" + idEquiFax;
                //var pruebaTelefono = "31733649";
                //string lcURLCoreFinanciero = "https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&DESTADDR=504" +
                //pruebaTelefono.Trim() + @"&MESSAGE= Estimado Cliente: " + pcNombreCliente + "Su link de pago es el siguiente: " + HttpUtility.UrlEncode(LinkPago) + idEquiFax;

                string lcURLCoreFinanciero = "https://mpp.movitext.com:8084/bin/send?USERNAME=prestadito&PASSWORD=XP92wB3j&DESTADDR=504" +
                pcTelefono.Trim() + @"&MESSAGE= NOVANET le informa" + "\nEstimado cliente: " + nombreCliente + " se le notifica que la instalacion de su servicio sera realizada por el tecnico: " + tecnico + " ,con identidad: "+identidad+"\nSe comunicara con usted de este telefono:" + telefonoTecnico +
               "\n" + informacionPaquete + "\nTambien se le adjunta el codigo QR de su instalacion." + HttpUtility.UrlEncode(urlqr);


                HttpWebRequest request = WebRequest.Create(lcURLCoreFinanciero) as HttpWebRequest;
                request.Accept = "text/xml";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                WebHeaderCollection header = response.Headers;
                var encoding = ASCIIEncoding.ASCII;

                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                {
                    string responseText = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {

                throw e;
            }

        }
    }
}