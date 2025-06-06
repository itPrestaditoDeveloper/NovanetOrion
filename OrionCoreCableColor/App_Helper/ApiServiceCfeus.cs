using RestSharp;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class ApiServiceCfeus
{
    private readonly string _baseUrl = "http://192.168.20.23:8984/API/serviciosActivos";
    private readonly string _usuario = "us.nova";
    private readonly string _clave = "24s/GKT29T";
    private readonly string _authToken = "Basic VVNSX1JFU0xMRVJJTlRFUjowbFhwOThAJQ=="; 

    //Consultar Api de los clientes activos/inactivos en cfeus 
    public async Task<ApiResponse> ConsultarServiciosActivosAsync(string codigoCliente)
    {
        try
        {
            var options = new RestClientOptions(_baseUrl)
            {
                ThrowOnAnyError = false,
                MaxTimeout = int.MaxValue
            };

            var client = new RestClient(options);
            var request = new RestRequest("/", Method.Post);

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Authorization", _authToken);

            request.AddParameter("cliente", codigoCliente.Trim());
            request.AddParameter("usuario", _usuario);
            request.AddParameter("clave", _clave);

            RestResponse response = await client.ExecuteAsync(request);

            if (response.IsSuccessful && !string.IsNullOrEmpty(response.Content))
            {
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(response.Content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse != null)
                {
                    return apiResponse; 
                }
            }

            return new ApiResponse
            {
                Codigo = "500",
                Mensaje = $"Error en la consulta: {response.StatusCode}",
                Resultado = null
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                Codigo = "500",
                Mensaje = $"Excepcion en la solicitud API: {ex.Message}",
                Resultado = null
            };
        }
    }
}

public class ApiResponse
{
    [JsonPropertyName("codigo")]
    public string Codigo { get; set; }

    [JsonPropertyName("mensaje")]
    public string Mensaje { get; set; }

    [JsonPropertyName("resultado")]
    public Resultado[] Resultado { get; set; }
}

public class Resultado
{
    [JsonPropertyName("CS")]
    public int CS { get; set; }

    [JsonPropertyName("SERVICIO")]
    public string Servicio { get; set; }
}




