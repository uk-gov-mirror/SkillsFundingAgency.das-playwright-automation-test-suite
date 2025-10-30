using SFA.DAS.Login.Service.Project.Helpers;

namespace SFA.DAS.RAAEmployer.UITests.Project
{
    [Binding]
    public class RAAEmployerConfigurationSetup(ScenarioContext context)
    {
        private readonly ConfigSection _configSection = context.Get<ConfigSection>();

        [BeforeScenario(Order = 2)]
        public async Task SetUpRAAEmployerProjectConfiguration()
        {
            await context.SetEasLoginUser(
            [
                _configSection.GetConfigSection<RAAEmployerUser>(),

                _configSection.GetConfigSection<RAAEmployerProviderPermissionUser>(),

                _configSection.GetConfigSection<RAAEmployerProviderYesPermissionUser>(),
            ]);
        }
    }
}
