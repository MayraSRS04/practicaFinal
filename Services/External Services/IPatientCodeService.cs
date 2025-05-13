using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Services.External_Services
{
    public interface IPatientCodeService
    {
        Task<string> GetPatientCodeAsync(string name, string lastName, string ci);

    }

    public class PatientCodeService : IPatientCodeService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PatientCodeService> _logger;

        public PatientCodeService(HttpClient httpClient, ILogger<PatientCodeService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> GetPatientCodeAsync(string name, string lastName, string ci)
        {
            var response = await _httpClient.GetAsync($"PatientCode/Generate?name={name}&lastName={lastName}&ci={ci}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PatientCodeResponse>();
            return result.PatientCode;
        }

        private class PatientCodeResponse
        {
            public string PatientCode { get; set; }
        }
    }
}
