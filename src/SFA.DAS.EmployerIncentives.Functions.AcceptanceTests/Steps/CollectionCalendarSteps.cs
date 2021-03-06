using FluentAssertions;
using Microsoft.AspNetCore.Http;
using SFA.DAS.EmployerIncentives.Functions.AcceptanceTests.Services;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.EmployerIncentives.Functions.AcceptanceTests.Steps
{
    [Binding]
    [Scope(Feature = "CollectionCalendar")]
    public class CollectionCalendarSteps : StepsBase
    {
        private readonly TestContext _testContext;

        public CollectionCalendarSteps(TestContext testContext) : base(testContext)
        {
            _testContext = testContext;
        }


        [When(@"a collection calendar period update is triggered")]
        public async Task WhenACollectionCalendarPeriodUpdateIsTriggered()
        {
            _testContext.EmployerIncentivesApi.MockServer
               .Given(
                   Request
                       .Create()
                       .WithPath(x => x.Contains("collectionPeriods"))
                       .UsingPatch())
               .RespondWith(
                    Response.Create()
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithHeader("Content-Type", "application/json"));

            var context = new DefaultHttpContext();
            context.Request.QueryString = new QueryString("?AcademicYear=2021&PeriodNumber=1&Active=true");

            await _testContext.LegalEntitiesFunctions.HttpTriggerUpdateCollectionCalendarPeriod.RunHttp(context.Request, new TestLogger());
        }

        [Then(@"the Employer Incentives API is called to update the active period")]
        public void ThenTheEmployerIncentivesAPIIsCalledToUpdateTheActivePeriod()
        {
            var requests = _testContext
                .EmployerIncentivesApi
                .MockServer
                .FindLogEntries(
                    Request
                        .Create()
                        .WithPath(x => x.Contains("/collectionPeriods"))
                        .UsingPatch()).AsEnumerable();

            requests.Should().HaveCount(1, "Expected request to APIM was not found in Mock server logs");
        }
    }

}
