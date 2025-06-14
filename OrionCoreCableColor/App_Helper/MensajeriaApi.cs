
using OrionCoreCableColor.Controllers;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace OrionCoreCableColor.App_Helper
{
    public static class MensajeriaApi
    {
        public static string MensajesDigitales(string pcTelefonoDestino, string pcMensaje, string pcURLAdjunto = "")
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.rob.chat/v1/mensaje/enviar");
            request.Headers.Add("apiKey", "NGE3Mjk1YjExMjk1NTQ0ZWMyZTE2NzA5NGJkNmE2MDUuMTUuc3J2Mi42ODRjMzUyMDcwYjcx");

            var content = new MultipartFormDataContent();

            content.Add(new StringContent("504" + pcTelefonoDestino), "para");
            content.Add(new StringContent(pcMensaje), "mensaje");
            content.Add(new StringContent(pcURLAdjunto), "adjunto");
            request.Content = content;
            var response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsStringAsync().Result;
        }


        public async static Task<string> MensajeWhatsapp(string pcTelefonoDestino, string pcMensaje, string pcURLAdjunto = "")
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.rob.chat/v1/mensaje/enviar");
            request.Headers.Add("apiKey", "NGE3Mjk1YjExMjk1NTQ0ZWMyZTE2NzA5NGJkNmE2MDUuMTUuc3J2Mi42ODRjMzUyMDcwYjcx");


            var content = new MultipartFormDataContent();
            content.Add(new StringContent("504" + pcTelefonoDestino), "para");
            content.Add(new StringContent(pcMensaje), "mensaje");
            content.Add(new StringContent(pcURLAdjunto), "adjunto");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public static void EnviarNumeroToken(string nombreCliente, int IDSolicitud, string pcTelefono, string Token)
        {
            var mensaje = @"NOVANET le informa" + "\n Estimado Cliente: " + nombreCliente + " le proporcionaremos un token para poder habilitar la firma de sus documentos de forma digital. El token proporcionado deberá colocarlo en el formulario que se le proporciona en su correo electronico";
            MensajesDigitales(pcTelefono, mensaje, "");
            MensajesDigitales(pcTelefono, "Su token es: " + Token, "");
        }

        public static void EnviarSMSPersonalizado(string nombreCliente, string pcTelefono, string Mensaje)
        {
            var msj = $"NOVANET le informa\n Estimado Cliente: {nombreCliente} \n {Mensaje}";
            MensajesDigitales(pcTelefono, msj);
        }

        public static void EnviarNumero(string nombreCliente, int idEquiFax, string pcTelefono)
        {
            var msj = $"NOVANET le informa \nEstimado Cliente: {nombreCliente} nos ayuda llenando los siguientes datos del siguiente link: {MemoryLoadManager.UrlWeb}/DatosCliente/ViewFormDatosCliente/{idEquiFax}";
            MensajesDigitales(pcTelefono, msj);
        }

        public static void EnviarNumeroReferencias(int fiIDReferencia, string nombrerefencia, string nombreCliente, int idEquiFax, string pcTelefono, string mensaje)
        {

            var msj = $"NOVANET le informa \nEstimado: {nombrerefencia} ha recibido este mensaje, porque la persona  {nombreCliente} lo ha puesto como referencia, nos ayuda llenando los siguientes datos del siguiente link: {MemoryLoadManager.UrlWeb}/DatosCliente/ViewFormReferenciasCliente/{fiIDReferencia}";
            MensajesDigitales(pcTelefono, msj);

        }

        public static void EnviarLinkPago(string nombreCliente, int idEquiFax, string pcTelefono, string LinkPago, string correo, string link)
        {
            var msj = $"NOVANET le informa \nEstimado Cliente: {nombreCliente} Se le envio su link de pago a su correo Electronico: {correo} , y tambien por esta via : {link} ";
            MensajesDigitales(pcTelefono, msj);

        }
        public static void EnviarSMSPersonalizadoAvisoCobrosArticulo(string nombreCliente, string pcTelefono, string Mensaje, string Direccion, string Punteo)
        {
            string lat = Punteo.Split(',')[0];
            string longi = Punteo.Split(',')[1].Trim();
            string urlMaps = $"https://www.google.com/maps?z=12&t=k&q={lat},{longi}";
            var msj = $"NOVANET Informa que el Cliente Pago su primero cuota, Cliente: {nombreCliente} \n Direccion: {Direccion} \n Articulo: {Mensaje} \n {urlMaps} ";
            MensajesDigitales(pcTelefono, msj);
        }

        public static void EnviarmensajeAgenteArticulosExtra(string nombreCliente, string pcTelefono, string Mensaje, string Direccion, string Punteo)
        {
            var msj = "";
            if (Punteo != "")
            {
                string lat = Punteo.Split(',')[0];
                string longi = Punteo.Split(',')[1].Trim();
                string urlMaps = $"https://www.google.com/maps?z=12&t=k&q={lat},{longi}";
                msj = $"NOVANET Informa que ya se le instalo al cliente y hay que entregarle los articulos que lleva,\n Cliente: {nombreCliente} \n Direccion: {Direccion} \n Articulo: {Mensaje} \n {urlMaps} ";
            }
            else
            {
                msj = $"NOVANET Informa que ya se le instalo al cliente y hay que entregarle los articulos que lleva,\n Cliente: {nombreCliente} \n Direccion: {Direccion} \n Articulo: {Mensaje} ";
            }
            MensajesDigitales(pcTelefono, msj);
        }


        public static void EnviarmensajeEntregadoArticuloCliente(string nombreCliente, string pcTelefono, string Articulos)
        {
            var msj = "";
            msj = $"NOVANET Informa que ya se Le entrego los Articulos al Cliente ,\n Cliente: {nombreCliente} \n Articulo: {Articulos} \n ";
            MensajesDigitales(pcTelefono, msj);
        }


        public static void EnviarMensajeInstalacionaCliente(string nombreCliente, int idEquiFax, string pcTelefono, string tecnico, string informacionPaquete, string telefonoTecnico, string identidad)
        {
            try
            {
                string urlqr = MemoryLoadManager.UrlWeb + "/DatosCliente/ViewFormQR/" + idEquiFax;

                var msj = $"NOVANET le informa \nEstimado cliente: {nombreCliente} se le notifica que la instalacion de su servicio sera realizada por el tecnico: {tecnico}, con identidad: {identidad} \nSe comunicara con usted de este telefono: {telefonoTecnico} \n {informacionPaquete} \nTambien se le adjunta el codigo QR de su instalacion.";
                MensajesDigitales(pcTelefono, msj, HttpUtility.UrlEncode(urlqr));

            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public static void EnviarMensajeInstalacionaClienteCambioServicio(string nombreCliente, int idEquiFax, string pcTelefono)
        {
            try
            {
                string urlqr = MemoryLoadManager.UrlWeb + "/DatosCliente/ViewFormQR/" + idEquiFax;

                var msj = $"NOVANET le informa \nEstimado cliente: {nombreCliente} se le notifica que la instalacion de su servicio . Se le adjunta el codigo QR de su Cambio de Servicio. \n";
                MensajesDigitales(pcTelefono, msj + urlqr);

            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public static void EnviarMensajeSalidaInventarioGuardias(int fiIDInventarioMovimientoMaestro, string fcMensaje)
        {
            try
            {
                var baseController = new BaseController();
                string urlqr = MemoryLoadManager.UrlWeb + "/inventario/Documentos/MovimientoInventario_" + fiIDInventarioMovimientoMaestro;
                var telefonos = baseController.GetConfiguracion<string>("Numeros_SalidaInventarioBodega", ',');
                foreach (var pcTelefono in telefonos)
                {
                    MensajesDigitales(pcTelefono, fcMensaje, HttpUtility.UrlEncode(urlqr));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void EnviarMensajePersonalizado(string fctelefono, string mensaje)
        {
            try
            {
                MensajesDigitales(fctelefono, mensaje);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}