namespace SFA.DAS.DfeAdmin.Service.Project.Tests.Pages;

public abstract class SignInBasePage(ScenarioContext context) : BasePage(context)
{
    public async Task EnterPassword(string password)
    {
        await Assertions.Expect(page.Locator("h1")).ToContainTextAsync("Enter your password");

        await page.GetByLabel("Password", new() { Exact = true }).FillAsync(password);

        objectContext.SetDebugInformation($"Entered password - '{password}'");
    }

    protected virtual async Task ClickSignInButton() => await page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
}
