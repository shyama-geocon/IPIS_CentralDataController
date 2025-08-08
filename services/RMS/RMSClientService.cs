using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.services.RMS
{
    public class RMSClientService
    {
        public HttpClient HttpClient { get; set; }

        public async Task<string> PostRMS(string endpoint, string headervalue, object JSON)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = false // Minified JSON
            };

            string jsonString = JsonSerializer.Serialize(JSON, options);
          //  var endpoint = new Uri("http://10.0.1.100:20000/IPIS/LiveData");
            var payload = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Create the request
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = payload
            };

            // Add headers
            request.Headers.Add("x-api-key", headervalue);

            // Send request
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode(); // Throws if not 2xx status

            // Read response
            string responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }


    }
}
