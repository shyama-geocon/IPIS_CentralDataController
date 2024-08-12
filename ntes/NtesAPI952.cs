using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IpisCentralDisplayController.ntes
{
    public class NtesAPI952
    {
        private static readonly HttpClient client = new HttpClient();

        // Fetch trains scheduled for a specific date at a station
        public async Task<NtesApiResponse952> GetTrainsByDateAsync(string station, string scheduledDate)
        {
            try
            {
                var requestBody = new
                {
                    station = station,
                    scheduledDate = scheduledDate
                };

                var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("authToken", "RF085DKA5215");

                var response = await client.PostAsync("https://enquiry.indianrail.gov.in/ntessrvc/TrainMaster?action=SchTrainsAtStation", content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<NtesApiResponse952>(responseString);

                return apiResponse;
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception("HTTP Request Error: " + httpEx.Message, httpEx);
            }
            catch (JsonSerializationException jsonEx)
            {
                throw new Exception("JSON Serialization Error: " + jsonEx.Message, jsonEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred: " + ex.Message, ex);
            }
        }

        // Fetch all scheduled trains for a station
        public async Task<NtesApiResponse952> GetTrainsAsync(string station)
        {
            try
            {
                var requestBody = new
                {
                    station = station
                };

                var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("authToken", "RF085DKA5215");

                var response = await client.PostAsync("https://enquiry.indianrail.gov.in/ntessrvc/TrainMaster?action=SchTrainsAtStation", content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<NtesApiResponse952>(responseString);

                return apiResponse;
            }
            catch (HttpRequestException httpEx)
            {
                throw new Exception("HTTP Request Error: " + httpEx.Message, httpEx);
            }
            catch (JsonSerializationException jsonEx)
            {
                throw new Exception("JSON Serialization Error: " + jsonEx.Message, jsonEx);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred: " + ex.Message, ex);
            }
        }
    }
}
