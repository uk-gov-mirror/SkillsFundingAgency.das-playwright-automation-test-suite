using SFA.DAS.DfeAdmin.Service.Project.Helpers.DfeSign;
using SFA.DAS.QFAST.UITests.Project.Tests.Pages;

namespace SFA.DAS.QFAST.UITests.Project.Helpers;

public class QfastHelpers(ScenarioContext context)
{
    protected readonly ScenarioContext context = context;

    public async Task<QfastHomePage> GoToQfastAdminHomePage()
    {
        await new DfeAdminLoginStepsHelper(context).LoginToQfastAsAdmin();

        return await VerifyPageHelper.VerifyPageAsync(() => new QfastHomePage(context));
    }
}
