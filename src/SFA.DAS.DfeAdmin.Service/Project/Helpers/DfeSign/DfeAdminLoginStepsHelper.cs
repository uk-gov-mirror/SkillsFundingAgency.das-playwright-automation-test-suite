using SFA.DAS.DfeAdmin.Service.Project.Helpers.DfeSign.User;
using SFA.DAS.DfeAdmin.Service.Project.Tests.LandingPage;
using SFA.DAS.DfeAdmin.Service.Project.Tests.Pages;
using SFA.DAS.Login.Service.Project;

namespace SFA.DAS.DfeAdmin.Service.Project.Helpers.DfeSign;

public class DfeAdminLoginStepsHelper(ScenarioContext context)
{

    #region Login
    public async Task NavigateAndLoginToASAdmin()
    {
        var driver = context.Get<Driver>();

        var url = UrlConfig.Admin_BaseUrl;

        context.Get<ObjectContext>().SetDebugInformation(url);

        await driver.Page.GotoAsync(url);

        await LoginToAsAdmin();
    }

    public async Task LoginToAsAssessor1() => await CheckAndLoginToAsAdmin(context.GetUser<AsAssessor1User>());

    public async Task LoginToAsAssessor2() => await CheckAndLoginToAsAdmin(context.GetUser<AsAssessor2User>());

    public async Task LoginToQfastAsAdmin() => await SubmitValidLoginDetails(context.GetUser<QfastDfeAdminUser>());
    public async Task LoginToAsAdmin() => await SubmitValidLoginDetails(new ASAdminLandingPage(context), GetAsAdminUser());

    public async Task LoginToSupportTool(DfeAdminUser dfeAdminUser) => await SubmitValidLoginDetails(new ASEmpSupportToolLandingPage(context), dfeAdminUser);

    public async Task SubmitValidAsLoginDetails(ASLandingCheckBasePage landingPage) => await SubmitValidLoginDetails(landingPage, GetAsAdminUser());

    #endregion

    #region CheckAndLogin

    public async Task CheckAndLoginToAsAdmin() => await CheckAndLoginToAsAdmin(GetAsAdminUser());
    
    public async Task CheckAndLoginToAsAdmin(DfeAdminUser dfeAdminUser) => await CheckAndLoginTo(new ASAdminLandingPage(context), dfeAdminUser);

    public async Task CheckAndLoginToSupportTool(DfeAdminUser dfeAdminUser) => await CheckAndLoginTo(new ASEmpSupportToolLandingPage(context), dfeAdminUser);

    #endregion

    #region CheckAndLoginToASVacancyQa

    //public void CheckAndLoginToASVacancyQa()
    //{
    //    if (new CheckASVacancyQaLandingPage(context).IsPageDisplayed()) new ASVacancyQaLandingPage(context).ClickStartNowButton();

    //    if (new CheckDfeSignInOrReviewHomePage(context).IsDfeSignPageDisplayed()) SubmitValidLoginDetails(context.GetUser<VacancyQaUser>());
    //}

    #endregion

    
    private async Task CheckAndLoginTo(ASLandingCheckBasePage landingPage, DfeAdminUser dfeAdminUser)
    {
        if (await landingPage.IsPageDisplayed()) await landingPage.ClickStartNowButton();

        if (await new CheckDfeSignInPage(context).IsPageDisplayed()) await SubmitValidLoginDetails(dfeAdminUser);
    }

    private async Task SubmitValidLoginDetails(ASLandingCheckBasePage landingPage, DfeAdminUser dfeAdminUser)
    {
        await landingPage.VerifyPage();

        await landingPage.ClickStartNowButton();

        await SubmitValidLoginDetails(dfeAdminUser);
    }

    private async Task SubmitValidLoginDetails(DfeAdminUser dfeAdminUser) => await new DfeSignInPage(context).SubmitValidLoginDetails(dfeAdminUser);

    private AsAdminUser GetAsAdminUser() => context.GetUser<AsAdminUser>();
}