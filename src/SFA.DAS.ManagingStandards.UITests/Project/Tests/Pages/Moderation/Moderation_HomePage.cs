namespace SFA.DAS.ManagingStandards.UITests.Project.Tests.Pages.Moderation;

public class Moderation_HomePage(ScenarioContext context) : ManagingStandardsBasePage(context)
{
    public override async Task VerifyPage()
    {
        await Assertions.Expect(page.Locator("h1")).ToContainTextAsync("Staff dashboard");
    }

    public async Task<Moderation_SearchPage> SearchForTrainingProvider()
    {
        await page.GetByRole(AriaRole.Link, new() { Name = "Search for an apprenticeship training provider" }).ClickAsync();

        return await VerifyPageAsync(() => new Moderation_SearchPage(context));
    }
}
