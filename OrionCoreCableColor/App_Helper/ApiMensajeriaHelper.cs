using Newtonsoft.Json;
using OrionCoreCableColor.Models.Mensajeria;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace OrionCoreCableColor.App_Helper
{
    public static class ApiMensajeriaHelper
    {

        #region usuarios
        public async static Task<List<ListChatBotDetalleViewModel>> GetListUsuarios()
        {
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, MemoryLoadManager.urlMensajeria + "usuarios/ListarUsuarios"))
                {
                    request.Headers.Add("Authorization", "Bearer " + MemoryLoadManager.tokenMensajeria);
                    var response = await client.SendAsync(request).ConfigureAwait(false);
                    var lecturaDelMensajeDeRespuesta = await response.Content.ReadAsStringAsync();
                    var bots = JsonConvert.DeserializeObject<List<ListChatBotDetalleViewModel>>(lecturaDelMensajeDeRespuesta);
                    return bots;
                }
            }
        }


        public async static Task<List<ListInfoUsuariosViewModel>> GetTelefonosEmpleados()
        {
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, MemoryLoadManager.urlMensajeria + "usuarios/ListarInformacionUsuarios"))
                {
                    request.Headers.Add("Authorization", "Bearer " + MemoryLoadManager.tokenMensajeria);
                    var response = await client.SendAsync(request).ConfigureAwait(false);
                    var lecturaDelMensajeDeRespuesta = await response.Content.ReadAsStringAsync();
                    var bots = JsonConvert.DeserializeObject<List<ListInfoUsuariosViewModel>>(lecturaDelMensajeDeRespuesta);
                    return bots.Where(x=>x.fcTelefono != "").ToList();
                }
            }
        }

        public async static Task<List<ListInfoUsuariosViewModel>> GetCorreosEmpleados()
        {
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, MemoryLoadManager.urlMensajeria + "usuarios/ListarInformacionUsuarios"))
                {
                    request.Headers.Add("Authorization", "Bearer " + MemoryLoadManager.tokenMensajeria);
                    var response = await client.SendAsync(request).ConfigureAwait(false);
                    var lecturaDelMensajeDeRespuesta = await response.Content.ReadAsStringAsync();
                    var bots = JsonConvert.DeserializeObject<List<ListInfoUsuariosViewModel>>(lecturaDelMensajeDeRespuesta);
                    return bots.Where(x => x.fcCorreo != "").ToList();
                }
            }
        }

        #endregion

        #region telegram

        public async static  Task<List<ListTelegramViewModel>> GetListBotTelegram()
        {
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, MemoryLoadManager.urlMensajeria + "telegram/obtenerBots"))
                {
                    request.Headers.Add("Authorization", "Bearer " + MemoryLoadManager.tokenMensajeria);
                    var response = await client.SendAsync(request).ConfigureAwait(false);
                    var lecturaDelMensajeDeRespuesta = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ListTelegramViewModel>>(lecturaDelMensajeDeRespuesta);
                }
            }
        }

        

       

        
        public async static Task<bool> EnviarMensajesTelegram(MensajeTelegramViewModel model)
        {

            try
            {
                var destinatarios = await ApiMensajeriaHelper.GetListUsuarios();
                foreach (var item in model.fiIDDestinatarios)
                {
                    var destino = destinatarios.FirstOrDefault(x => x.fiIDChatBot == item);
                    var mensaje = new TelegramMensajeViewModel();

                    mensaje.fcCodigo = model.fcCodigo;
                    mensaje.fcTelefono = destino.fcNumeroTelefono;
                    mensaje.fcMensaje = model.fcMensaje;
                    mensaje.fcChatID = destino.fcChatID;
                    mensaje.fvArchivos = new List<TelegramArchivosViewModel>();
                    if (model.fvArchivos?.Any() ?? false)
                    {
                        mensaje.fvArchivos = new List<TelegramArchivosViewModel>();
                        foreach (var archivo in model.fvArchivos)
                        {
                            mensaje.fvArchivos.Add(new TelegramArchivosViewModel
                            {
                                fvArchivo = archivo,
                                fcCaption = Path.GetFileNameWithoutExtension(archivo.FileName),
                                fcNombreArchivo = archivo.FileName
                            });
                        }
                    }
                    var result = await MensajePorUsuario(mensaje);
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }

            
        }

        public async static Task<NotificacionMensajeriaViewModel> MensajePorUsuario(TelegramMensajeViewModel model)
        {
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, MemoryLoadManager.urlMensajeria + "telegram/EnviarMensaje"))
                {
                    request.Headers.Add("Authorization", "Bearer " + MemoryLoadManager.tokenMensajeria);
                    var content = new MultipartFormDataContent();
                    content.Add(new StringContent(model.fcCodigo), "fcCodigo");
                    content.Add(new StringContent(model.fcTelefono), "fcTelefono");
                    content.Add(new StringContent(model.fcMensaje), "fcMensaje");
                    content.Add(new StringContent(model.fcChatID), "fcChatID");

                    for (var i = 0; i < (model?.fvArchivos?.Count() ?? 0); i++)
                    {
                        content.Add(new StreamContent(model.fvArchivos[i].fvArchivo.InputStream), $"fvArchivos[{i}].fvArchivo", model.fvArchivos[i].fvArchivo.FileName);
                        content.Add(new StringContent(model.fvArchivos[i].fvArchivo.FileName), $"fvArchivos[{i}].fcNombreArchivo");
                        content.Add(new StringContent(Path.GetFileNameWithoutExtension(model.fvArchivos[i].fvArchivo.FileName)), $"fvArchivos[{i}].fcCaption");
                    }

                    request.Content = content;

                    var response = await client.SendAsync(request);
                    var lecturaDelMensajeDeRespuesta = await response.Content.ReadAsStringAsync();
                    var resultado = JsonConvert.DeserializeObject<NotificacionMensajeriaViewModel>(lecturaDelMensajeDeRespuesta);
                    return resultado;
                }
            }
        }

        #endregion

        #region SMS

        public async static Task<List<ListServiciosSmsViewModel>> GetListSmsService()
        {
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, MemoryLoadManager.urlMensajeria + "sms/ListarServiciosSMS"))
                {
                    request.Headers.Add("Authorization", "Bearer " + MemoryLoadManager.tokenMensajeria);
                    var response = await client.SendAsync(request).ConfigureAwait(false);
                    var lecturaDelMensajeDeRespuesta = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ListServiciosSmsViewModel>>(lecturaDelMensajeDeRespuesta);
                }
            }
        }

        public async static Task<bool> EnviarMensajesSMS(MensajeSmsViewModel model)
        {
            try { 
                foreach(var telefono in model.fcTelefono)
                {
                    using (var client = new HttpClient())
                    {
                        using (var request = new HttpRequestMessage(HttpMethod.Post, MemoryLoadManager.urlMensajeria + "sms/EnviarMensaje"))
                        {
                            request.Headers.Add("Authorization", "Bearer " + MemoryLoadManager.tokenMensajeria);
                            var content = new MultipartFormDataContent();
                            content.Add(new StringContent(model.fcCodigo), "fcCodigo");
                            content.Add(new StringContent(telefono), "fcTelefono");
                            content.Add(new StringContent(model.fcMensaje), "fcMensaje");
                            request.Content = content;
                            var response = await client.SendAsync(request);
                            var lecturaDelMensajeDeRespuesta = await response.Content.ReadAsStringAsync();
                        }
                    }
                }
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        #endregion

        #region Correos
        public async static Task<List<ListEMailsViewModel>> GetListEmails()
        {
            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, MemoryLoadManager.urlMensajeria + "correo/ListarCorreos"))
                {
                    request.Headers.Add("Authorization", "Bearer " + MemoryLoadManager.tokenMensajeria);
                    var response = await client.SendAsync(request).ConfigureAwait(false);
                    var lecturaDelMensajeDeRespuesta = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<ListEMailsViewModel>>(lecturaDelMensajeDeRespuesta);
                }
            }
        }

        public async static Task<bool> EnviarCorreo(SendEmailViewModel model)
        {

            try {
                var correos = await GetListEmails();
                var correo = correos.FirstOrDefault(x => x.fcCodigo == model.fcCodigo);
                model.fcEmailName = correo.fcName;
                using (var client = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Post, MemoryLoadManager.urlMensajeria + "correo/EnviarMensaje"))
                    {
                        request.Headers.Add("Authorization", "Bearer " + MemoryLoadManager.tokenMensajeria);
                        var content = new MultipartFormDataContent();
                        content.Add(new StringContent(model.fcCodigo), "fcCodigo");

                        for (var i = 0; i < model.fcDestinatarios.Count(); i++)
                        {
                            content.Add(new StringContent(model.fcDestinatarios[i]), $"fcDestinatarios[{i}]");
                        }

                        if (model.fcDestinatariosCC != null)
                        {
                            for (var i = 0; i < model.fcDestinatariosCC.Count(); i++)
                            {
                                content.Add(new StringContent(model.fcDestinatariosCC[i]), $"fcDestinatariosCC[{i}]");
                            }
                        }


                        if (model.fvArchivos != null)
                        {
                            for (var i = 0; i < (model?.fvArchivos?.Count() ?? 0); i++)
                            {
                                content.Add(new StreamContent(model.fvArchivos[i].fvArchivo.InputStream), $"fvArchivos[{i}].fvArchivo", model.fvArchivos[i].fcNombreArchivo);
                                content.Add(new StringContent(model.fvArchivos[i].fcNombreArchivo), $"fvArchivos[{i}].fcNombreArchivo");
                            }
                        }

                        content.Add(new StringContent(model.fcEmailName), "fcEmailName");
                        content.Add(new StringContent(model.fcAsunto), "fcAsunto");
                        content.Add(new StringContent(model.fcCuerpoCorreo), "fcCuerpoCorreo");

                        request.Content = content;

                        var response = await client.SendAsync(request);
                        var lecturaDelMensajeDeRespuesta = await response.Content.ReadAsStringAsync();
                        var resultado = JsonConvert.DeserializeObject<bool>(lecturaDelMensajeDeRespuesta);
                        return resultado;
                    }
                }

            }
            catch(Exception ex)
            {
                return false;
            }

            
        }

        #endregion


    }
}