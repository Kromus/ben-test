using Ensek_Rest_API_Tests.Clients;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Ensek_Rest_API_Tests.Settings
{
    [Binding]
    public class Hooks
    {
        private EnsekTestAPI _ensekTestAPI;

        public Hooks(EnsekTestAPI ensekTestAPI)
        {
            _ensekTestAPI = ensekTestAPI;
        }

        [BeforeScenario]
        public async Task ResetTestData()
        {
            await _ensekTestAPI.GetBearerToken();
            await _ensekTestAPI.ResetData();
        }
    }
}
