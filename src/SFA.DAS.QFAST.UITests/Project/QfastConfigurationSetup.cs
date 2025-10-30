using SFA.DAS.Login.Service.Project;

namespace SFA.DAS.QFAST.UITests.Project;

[Binding]
public class QfastConfigurationSetup(ScenarioContext context)
{
    [BeforeScenario(Order = 12)]
    public async Task SetUpQfastConfiguration()
    {
        var dfeAdminUsers = context.Get<FrameworkList<DfeAdminUsers>>();

        context.SetNonEasLoginUser(new List<NonEasAccountUser>
        {
            SetDfeAdminCredsHelper.SetDfeAdminCreds(dfeAdminUsers, new QfastDfeAdminUser()),
        });

        await Task.CompletedTask;
    }
}