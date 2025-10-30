using RestSharp;
using SFA.DAS.API.Framework;
using SFA.DAS.API.Framework.Configs;
using SFA.DAS.API.Framework.RestClients;
using SFA.DAS.Approvals.APITests.Project;
using System.Net;

namespace SFA.DAS.Approvals.UITests.Project.Helpers.API
{
    internal class LearnerDataOuterApiClient : Outer_BaseApiRestClient
    {
        protected override string ApiName => "/learnerdata";
        private readonly ScenarioContext _context;
        private readonly ObjectContext _objectContext;  
        private readonly Outer_ApprovalsAPIClient _restClient;
        private RestResponse _restResponse = null;

        public LearnerDataOuterApiClient(ScenarioContext context, Outer_ApiAuthTokenConfig config) : base(context.Get<ObjectContext>(), config.Apim_SubscriptionKey)
        {
            _context = context;
            _objectContext = context.Get<ObjectContext>();
            _restClient = _context.GetRestClient<Outer_ApprovalsAPIClient>();
        }

        public async Task<RestResponse> GetLearners(string resource)
        {
            await _restClient.CreateRestRequest(Method.Get, resource);
            _restResponse = await SendRequestAsync(HttpStatusCode.OK);
            return _restResponse;
        }

        private async Task<RestResponse> SendRequestAsync(HttpStatusCode responseCode) => await _restClient.Execute(responseCode);

    }

}
