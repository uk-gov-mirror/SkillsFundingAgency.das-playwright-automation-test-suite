using SFA.DAS.API.Framework;
using SFA.DAS.API.Framework.Configs;
using SFA.DAS.Approvals.APITests.Project;
using SFA.DAS.Approvals.UITests.Project.Helpers.API;
using SFA.DAS.Approvals.UITests.Project.Helpers.SqlHelpers;
using SFA.DAS.ProviderPortal.UITests.Project.Helpers;

namespace SFA.DAS.Approvals.UITests.Project.Hooks
{
    [Binding]
    public class BeforeScenarioHooks(ScenarioContext context)
    {
        private readonly ScenarioContext _context = context;
        private readonly ObjectContext _objectcontext = context.Get<ObjectContext>();
        private readonly DbConfig _dbConfig = context.Get<DbConfig>();
        private readonly Outer_ApiAuthTokenConfig _outer_ApiAuthTokenConfig = context.Get<Outer_ApiAuthTokenConfig>();
        private readonly string[] _tags = context.ScenarioInfo.Tags;

        [BeforeScenario(Order = 30)]
        public void SetUpDependencyConfig()
        {
            //Get api config from approvals:
            var subscriptionKey = context.GetOuterApiAuthTokenConfig<OuterApiAuthTokenConfig>();

            if (subscriptionKey is null) return;

            //Set config for 'Outer_ApiAuthTokenConfig' in the context:
            Outer_ApiAuthTokenConfig outer_ApiAuthTokenConfig = new() { Apim_SubscriptionKey = subscriptionKey.Apim_SubscriptionKey };

            context.Set(outer_ApiAuthTokenConfig);
        }

        [BeforeScenario(Order = 31)]
        public void SetUpDbHelpers()
        {
            context.Set(new AccountsDbSqlHelper(_objectcontext, _dbConfig));

            context.Set(new CommitmentsDbSqlHelper(_objectcontext, _dbConfig));

            context.Set(new LearningDbSqlHelper(_objectcontext, _dbConfig));

            context.Set(new LearnerDataDbSqlHelper(_objectcontext, _dbConfig));

            context.Set(new RelationshipsSqlDataHelper(_objectcontext, _dbConfig));

        }

        [BeforeScenario(Order = 32)]
        public async Task StartAzureServiceBusHelper()
        {
            if (GlobalTestContext.ServiceBus is { IsRunning: true })
                return;

            var config = context.Get<NServiceBusConfig>();
            var serviceBusHelper = new ServiceBusHelper();
            await serviceBusHelper.Start(config.ServiceBusConnectionString);

            GlobalTestContext.ServiceBus = serviceBusHelper;
        }

        [BeforeScenario(Order = 34)]
        public void SetUpApiHelpers()
        {
            context.SetRestClient(new Outer_ApprovalsAPIClient(_objectcontext, context.GetOuter_ApiAuthTokenConfig()));
            
            context.Set(new LearnerDataOuterApiClient(_context, _outer_ApiAuthTokenConfig));

        }

    }
}
