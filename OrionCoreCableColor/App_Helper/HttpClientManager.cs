using Newtonsoft.Json;
using OrionCoreCableColor.App_Helper.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace OrionCoreCableColor.App_Helper
{
    public class HttpClientManager
    {
        public readonly HttpClient _httpClient;

        public HttpClientManager()
        {

            _httpClient = new HttpClient
            {
                MaxResponseContentBufferSize = int.MaxValue,

            };

        }


        public async Task<TokenResponse> GetTokenDefault(string userName, string password, string urlAction)
        {
            try
            {

                var httpClient = _httpClient;

                var nvc = new List<KeyValuePair<string, string>>();

                nvc.Add(new KeyValuePair<string, string>("grant_type", "password"));

                nvc.Add(new KeyValuePair<string, string>("username", userName));

                nvc.Add(new KeyValuePair<string, string>("password", password));

                var req = new HttpRequestMessage(HttpMethod.Post, urlAction)
                {
                    Content = new FormUrlEncodedContent(nvc)
                };

                var resultadoDeLaConsulta = await httpClient.SendAsync(req);

                var lecturaDelMensajeDeRespuesta = await resultadoDeLaConsulta.Content.ReadAsStringAsync();

                var generatedTokenServiceModel = JsonConvert.DeserializeObject<TokenResponse>(lecturaDelMensajeDeRespuesta);

                return generatedTokenServiceModel;

            }
            catch (Exception ex)
            {
                return default;
            }

        }

        public async Task<T> GetResult<T>(string urlClient)
        {
            try
            {

                var direccionAlaCualSeMandaraElObjeto = urlClient;

                var resultadoDeLaConsulta = await _httpClient.GetAsync(direccionAlaCualSeMandaraElObjeto).ConfigureAwait(false);

                var lecturaDelMensajeDeRespuesta = await resultadoDeLaConsulta.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<T>(lecturaDelMensajeDeRespuesta);

            }
            catch (Exception e)
            {

                return default(T);

            }
        }

        public async Task<T> PostResult<T>(string urlClient, Dictionary<string, string> dictionaryModel)
        {

            try
            {


                using (var response = await _httpClient.PostAsync(urlClient, new FormUrlEncodedContent(dictionaryModel)))
                {

                    var result = await response.Content.ReadAsStringAsync();


                    return JsonConvert.DeserializeObject<T>(result);
                }



            }
            catch (Exception e)
            {

                return default(T);

            }

        }


    }
}