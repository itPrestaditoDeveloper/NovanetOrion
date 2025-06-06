using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Threading.Tasks;
using OrionCoreCableColor.Models.Tecnico;
using Newtonsoft.Json;

namespace OrionCoreCableColor.App_Services.TecnicoService
{
    public class ApiTecnicosService
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<RequesApiListaTecnico> GetApiListadoTecnico(int idrol, int idusuario)
        {
            // consulta a la api como tal
            string url = $"https://api.novanetgroup.com/api/Novanet/Tecnico/ListaSolicitudes?IdRol={idrol}&IdUser={idusuario}"; // manda a llamar este procedimiento como tal sp_SolicitudesAsignadas_By_Tecnico

            // Envía la solicitud GET a la API
            HttpResponseMessage response = await client.GetAsync(url);

            // Asegúrate de que la respuesta sea exitosa
            response.EnsureSuccessStatusCode();

            // Lee el contenido de la respuesta como una cadena
            string responseBody = await response.Content.ReadAsStringAsync();
            RequesApiListaTecnico data = JsonConvert.DeserializeObject<RequesApiListaTecnico>(responseBody);


            return data;
        }

        public async Task<RequesApiInstalacion> GetApiInfoInstalacion(int idContratistaSolicitud)
        {
            // consulta a la api como tal
            string url = $"https://api.novanetgroup.com/api/Novanet/Tecnico/Instalacion_TecnicosExternos?piIDContratistaSolicitud={idContratistaSolicitud}"; // manda a llamar este procedimiento como tal

            // Envía la solicitud GET a la API
            HttpResponseMessage response = await client.GetAsync(url);

            // Asegúrate de que la respuesta sea exitosa
            response.EnsureSuccessStatusCode();

            // Lee el contenido de la respuesta como una cadena
            string responseBody = await response.Content.ReadAsStringAsync();
            RequesApiInstalacion data = JsonConvert.DeserializeObject<RequesApiInstalacion>(responseBody);


            return data;
        }


    }
}