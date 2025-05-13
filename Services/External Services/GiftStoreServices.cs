using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;
using Services.Dtos;

namespace Services.External_Services
{
    public class GiftStoreServices
    {
        private readonly HttpClient _httpClient;
    
        public GiftStoreServices(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
   
        public async Task<List<GiftDto>> GetAllGiftsItems()
        {
            const string url = "https://api.restful-api.dev/objects";
            Log.Information("Conectando a {Url}", url);

            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            Log.Information("Conexión exitosa a {Url}", url);
            if (!response.IsSuccessStatusCode)
            {
                Log.Error("Error al conectar a {Url}: {StatusCode}", url, response.StatusCode);
                return new List<GiftDto>();
            }
            Log.Information("Conexión exitosa a {Url}", url);
            Log.Information("Recibiendo respuesta de {Url}", url);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Log.Error("Error al recibir respuesta de {Url}: {StatusCode}", url, response.StatusCode);
                return new List<GiftDto>();
            }
            Log.Information("Respuesta recibida de {Url}", url);
            Log.Information("Leyendo contenido de la respuesta de {Url}", url);
            if (response.Content == null)
            {
                Log.Error("Contenido nulo en la respuesta de {Url}", url);
                return new List<GiftDto>();
            }
            Log.Information("Contenido leído de la respuesta de {Url}", url);

            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<GiftDto>>(body)!;
        }
    }
}
