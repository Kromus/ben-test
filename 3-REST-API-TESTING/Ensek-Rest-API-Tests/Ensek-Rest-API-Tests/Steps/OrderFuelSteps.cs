using Ensek_Rest_API_Tests.Clients;
using Ensek_Rest_API_Tests.Models;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Ensek_Rest_API_Tests
{
    [Binding]
    public class OrderFuelSteps
    {
        private ScenarioContext _scenarioContext;
        private EnsekTestAPI _ensekTestAPI;
        public OrderFuelSteps(ScenarioContext scenarioContext, EnsekTestAPI ensekTestAPI)
        {
            _scenarioContext = scenarioContext;
            _ensekTestAPI = ensekTestAPI;
        }

        [When(@"I order (.*) (.*) (.*)")]
        public async Task WhenIOrderEnergy(string energyName, int energyId, int quantity)
        {
            var requestedOrder = new Order()
            {
                EnergyName = energyName,
                EnergyId = energyId,
                Quantity = quantity
            };
            var response = await _ensekTestAPI.OrderEnergy(energyId, quantity:requestedOrder.Quantity);

            requestedOrder.Id = await FindOrderIdInResponseBody(response);

            _scenarioContext["OrderEnergyResponse"] = response;
            _scenarioContext["Order"] = requestedOrder;
        }

        [Then(@"the request is successful")]
        public void ThenTheRequestIsSuccessful()
        {
            HttpResponseMessage orderResponseMessage = _scenarioContext.Get<HttpResponseMessage>("OrderEnergyResponse");
            orderResponseMessage.EnsureSuccessStatusCode();
        }

        [Then(@"the order is stored")]
        public async Task ThenTheOrderIsStored()
        {
            var expectedOrder = _scenarioContext.Get<Order>("Order");
            var listOfOrders = await _ensekTestAPI.GetListOfOutstandingOrders();

            Assert.IsTrue(listOfOrders.Any(orderInList => orderInList.Id == expectedOrder.Id), "Order Id was not found in the list of orders, suggesting it was not stored");
            var recentOrderInResponse = listOfOrders.Single(orderInlist => orderInlist.Id == expectedOrder.Id);
            Assert.AreEqual(recentOrderInResponse.Fuel, expectedOrder.EnergyName);
            Assert.AreEqual(recentOrderInResponse.Quantity, expectedOrder.Quantity);
        }

        [Given(@"I have an existing order")]
        public async Task GivenIHaveAnExistingOrder()
        {
            await WhenIOrderEnergy("gas", 1, 1);
        }

        [Given(@"I am not authorised")]
        public void GivenIAmNotAuthorised()
        {
            _ensekTestAPI.ClearBearerToken();
        }

        [When(@"I reset data")]
        public async Task WhenIResetData()
        {
            var response = await _ensekTestAPI.ResetData();

            _scenarioContext["ResetDataResponse"] = response;
        }

        [Then(@"resetting data was unsuccessful")]
        public void ThenResettingDataWasUnsuccessful()
        {
            var resetDataResponse = _scenarioContext.Get<HttpResponseMessage>("ResetDataResponse");
            Assert.IsFalse(resetDataResponse.IsSuccessStatusCode);
        }

        [Then(@"I can no longer find that order")]
        public async Task ThenICanNoLongerFindThatOrder()
        {
            var order = _scenarioContext.Get<Order>("Order");
            var listOfOrders = await _ensekTestAPI.GetListOfOutstandingOrders();

            Assert.IsFalse(listOfOrders.Any(orderInList => orderInList.Id == order.Id), "Order Id was found but this was not expected");
        }

        [Then(@"the request is unsuccessful")]
        public void ThenTheRequestIsUnsuccessful()
        {
            HttpResponseMessage orderResponseMessage = _scenarioContext.Get<HttpResponseMessage>("OrderEnergyResponse");
            Assert.IsFalse(orderResponseMessage.IsSuccessStatusCode, "request was successful but expected a failure response");
        }


        public async Task<string> FindOrderIdInResponseBody(HttpResponseMessage responseMessage)
        {
            var orderIdRegex = Regex.Match(await responseMessage.Content.ReadAsStringAsync(), @"[0-9a-fA-F]{8}-(?:[0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}").Value;

            return orderIdRegex;
        }
    }
}
