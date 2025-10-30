using SFA.DAS.Approvals.UITests.Project.Helpers.TestDataHelpers;
using SFA.DAS.ProviderLogin.Service.Project.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Approvals.UITests.Project.Steps
{
    [Binding]
    public class ProviderLoginSteps(ScenarioContext context)
    {
        private readonly ProviderHomePageStepsHelper _providerHomePageStepsHelper = new(context);

        [Given(@"the provider logs into portal")]
        [When("Provider logs into Provider-Portal")]
        public async Task GivenTheProviderLogsIntoPortal() => await _providerHomePageStepsHelper.GoToProviderHomePage(false);

        
        [Given(@"the provider logs in as a (Contributor|ContributorWithApproval|AccountOwner|Viewer)")]
        [When(@"the provider logs in as a (Contributor|ContributorWithApproval|AccountOwner|Viewer)")]
        public async Task GivenTheProviderLogsInAs(ProviderConfig config) => await _providerHomePageStepsHelper.GoToProviderHomePage(config, false);

        [StepArgumentTransformation(@"(Contributor|ContributorWithApproval|AccountOwner|Viewer)")]
        public ProviderConfig GetProviderUserRole(string providerUserRoles)
        {
            var userRole = Enum.Parse<ProviderUserRoles>(providerUserRoles, true);

            return true switch
            {
                bool _ when (userRole == ProviderUserRoles.Contributor) => context.GetUser<ProviderContributorUser>(),
                bool _ when (userRole == ProviderUserRoles.ContributorWithApproval) => context.GetUser<ProviderContributorWithApprovalUser>(),
                bool _ when (userRole == ProviderUserRoles.AccountOwner) => context.GetUser<ProviderAccountOwnerUser>(),
                bool _ when (userRole == ProviderUserRoles.Viewer) => context.GetUser<ProviderViewOnlyUser>(),
                _ => null,
            };
        }



    }
}
