using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IpisCentralDisplayController.ntes;
using Newtonsoft.Json;

namespace IpisCentralDisplayController
{
    public class NtesAPI951
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<NtesApiResponse951> GetTrainsAsync(string station, int nextMins)
        {
            try
            {
                var requestBody = new
                {
                    station = station,
                    nextMins = nextMins
                };

                var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("authToken", "RF085DKA5215");

                var response = await client.PostAsync("https://enquiry.indianrail.gov.in/ntessrvc/TrainService?action=LiveStation", content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<NtesApiResponse951>(responseString);

                return apiResponse;
            }
            catch (HttpRequestException httpEx)
            {
                // Log or handle HTTP request exceptions
                throw new Exception("HTTP Request Error: " + httpEx.Message, httpEx);
            }
            catch (JsonSerializationException jsonEx)
            {
                // Log or handle JSON serialization exceptions
                throw new Exception("JSON Serialization Error: " + jsonEx.Message, jsonEx);
            }
            catch (Exception ex)
            {
                // Log or handle any other exceptions
                throw new Exception("An unexpected error occurred: " + ex.Message, ex);
            }
        }
    }

}
