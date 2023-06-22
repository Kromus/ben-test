using Ensek_Rest_API_Tests.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ensek_Rest_API_Tests.Clients
{
    public class EnsekTestAPI
    {
        private readonly HttpClient _ensekApi;
        private readonly LoginResource _loginDetails = new LoginResource() { Username = "test", Password = "testing" };

        public EnsekTestAPI()
        {
            _ensekApi = new HttpClient();
            _ensekApi.BaseAddress = new Uri("https://qacandidatetest.ensek.io/ENSEK/");
            _ensekApi.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task GetBearerToken()
        {
            var response = await _ensekApi.PostAsync("login", new StringContent(JsonConvert.SerializeObject(_loginDetails, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            }),
            Encoding.Default,
            "application/json"));

            JsonDocument jsonDoc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            string accessToken = jsonDoc.RootElement.GetProperty("access_token").ToString();

            _ensekApi.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public void ClearBearerToken()
        {
            _ensekApi.DefaultRequestHeaders.Authorization = null;
        }

        public async Task<List<GetOrdersResponse>> GetListOfOutstandingOrders()
        {
            var response = await _ensekApi.GetAsync("orders");
            List<GetOrdersResponse> listOfOrders = JsonConvert.DeserializeObject<List<GetOrdersResponse>>(await response.Content.ReadAsStringAsync());
            return listOfOrders;
        }

        public async Task<HttpResponseMessage> OrderEnergy(int energyId, int quantity)
        {
            var response = await _ensekApi.PutAsync($"buy/{energyId}/{quantity}", null);
            return response;
        }

        public async Task<HttpResponseMessage> ResetData()
        {
            var response = await _ensekApi.PostAsync($"reset", null);
            return response;
        }
    }
}
