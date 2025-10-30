namespace SFA.DAS.QFAST.UITests.Project.Tests.Pages;

public class QfastHomePage(ScenarioContext context) : BasePage(context)
{
    public override async Task VerifyPage() => await Assertions.Expect(page.Locator("h1")).ToContainTextAsync("What do you want to do?");
}
