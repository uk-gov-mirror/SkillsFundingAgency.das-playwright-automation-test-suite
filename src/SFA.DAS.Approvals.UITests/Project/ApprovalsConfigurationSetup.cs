namespace SFA.DAS.Approvals.UITests.Project
{
    [Binding]
    public class ApprovalsConfigurationSetup(ScenarioContext context)
    {
        private readonly ConfigSection _configSection = context.Get<ConfigSection>();

        [BeforeScenario(Order = 20)]
        public async Task SetUpApprovalsConfiguration()
        {
            //if (NoNeedToSetUpConfiguration()) return;

            context.SetApprovalsConfig(_configSection.GetConfigSection<ApprovalsConfig>());

            await context.SetEasLoginUser(
            [
                _configSection.GetConfigSection<ProviderPermissionLevyUser>(),
                _configSection.GetConfigSection<EmployerWithMultipleAccountsUser>(),
                _configSection.GetConfigSection<FlexiJobUser>(),
                _configSection.GetConfigSection<NonLevyUserAtMaxReservationLimit>(),
                _configSection.GetConfigSection<EmployerConnectedToPortableFlexiJobProvider>()
            ]);

            context.Set(_configSection.GetConfigSection<NServiceBusConfig>());
        }

        [BeforeScenario(Order = 2)]
        public void SetUpOuterApiAuthTokenConfiguration()
        {
            context.SetOuterApiAuthTokenConfig(_configSection.GetConfigSection<OuterApiAuthTokenConfig>());

        }

        //private bool NoNeedToSetUpConfiguration()
        //{
        //    if (context.ScenarioInfo.Tags.Contains("deletecohortviaemployerportal"))
        //    {
        //        context.SetEasLoginUser([_configSection.GetConfigSection<DeleteCohortLevyUser>()]);
        //    }

        //    return new TestDataSetUpConfigurationHelper(context).NoNeedToSetUpConfiguration();
        //}
    }
}
