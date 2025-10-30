using NUnit.Framework;
using SFA.DAS.DfeAdmin.Service.Project.Helpers.DfeSign.User;
using SFA.DAS.Framework.Helpers;
using System;
using System.Threading;

namespace SFA.DAS.DfeAdmin.Service.Project.Tests.Pages;

// 24/09/2025 - PLEASE DO NOT REMOVE COMMENTED CODE
public class DfeSignInPage(ScenarioContext context) : SignInBasePage(context)
{
    //public class CheckEnterPasswordMFAOrStandardPage(ScenarioContext context) : CheckMultipleHomePage(context)
    //{
    //    public override string[] PageIdentifierCss => [EnterPasswordMFAPageIdentifierCss, EnterPasswordPageIdentifierCss];

    //    public override string[] PageTitles => [EnterPasswordMFAPageTitle, EnterPasswordPageTitle];

    //    public async Task<bool> IsEnterPasswordMFADisplayed()
    //    {
    //        // Wait for the page to load completely
    //        await page.WaitForLoadStateAsync(LoadState.Load);

    //        await Assertions.Expect(page.Locator(Identifier)).ToContainTextAsync("password", new LocatorAssertionsToContainTextOptions { Timeout = 15000, UseInnerText = true });

    //        return await ActualDisplayedPage(EnterPasswordMFAPageTitle);
    //    }
    //}

    //public static string EnterPasswordMFAPageIdentifierCss => "div[id='loginHeader']";

    //public static string EnterPasswordPageIdentifierCss => "#content h1.govuk-heading-xl";

    //public static string EnterPasswordMFAPageTitle => "Enter password";

    //public static string EnterPasswordPageTitle => "Enter your password";

    static readonly SemaphoreSlim _semaphore = new(1, 1);

    static readonly List<string> usedCodes = [];

    public static string DfePageTitle => "Access the DfE Sign-in service";

    public ILocator DfePageIdentifier => page.Locator("h1");

    public static string DfePageIdentifierCss => ".govuk-heading-xl";

    public override async Task VerifyPage() => await Assertions.Expect(DfePageIdentifier).ToContainTextAsync(DfePageTitle);

    public async Task SubmitValidLoginDetails(DfeAdminUser dfeAdminUser)
    {
        await SubmitValidLoginDetails(dfeAdminUser.Username, dfeAdminUser.Password);
    }

    protected async Task SubmitValidLoginDetails(string username, string password)
    {
        if (EnvironmentConfig.IsPPEnvironment)// && await new CheckEnterPasswordMFAOrStandardPage(context).IsEnterPasswordMFADisplayed())
        {
            try
            {
                await _semaphore.WaitAsync();

                await SubmitUsername(username);

                await SubmitMFAPassword(password);

                await SubmitMFAIdentity();

                var dateTime = DateTime.Now.AddSeconds(-6);

                await SubmitMFACode(username, dateTime);

                objectContext.SetDebugInformation("****DFE MFA Sign completed****");
            }
            catch (Exception)
            {
                objectContext.SetDebugInformation("Exception thrown in DFE MFA sign");

                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        else
        {

            await SubmitUsername(username);

            await EnterPassword(password);

            try
            {
                await page.GetByRole(AriaRole.Button, new() { Name = "Sign in" }).ClickAsync(new() { Timeout = 300000 });
            }
            catch (TimeoutException ex)
            {
                //do nothing 
                objectContext.SetDebugInformation($"{DfePageTitle} resulted in {ex.Message}");
            }
        }

        await Assertions.Expect(page.Locator("body")).Not.ToContainTextAsync(DfePageTitle, new() { Timeout = 300000 });
    }

    private async Task SubmitUsername(string username)
    {
        await VerifyPage();

        await page.GetByLabel("Email address").FillAsync(username);

        await page.GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();

        objectContext.SetDebugInformation($"Entered username - '{username}'");
    }


    private async Task SubmitMFAPassword(string password)
    {
        await Assertions.Expect(page.GetByRole(AriaRole.Heading)).ToContainTextAsync("Enter password", new LocatorAssertionsToContainTextOptions { Timeout = 15000 });

        await page.GetByRole(AriaRole.Textbox, new() { Name = "Enter the password" }).FillAsync(password);

        await page.GetByRole(AriaRole.Button, new() { Name = "Sign in" }).ClickAsync();

        objectContext.SetDebugInformation($"Entered {password}");
    }

    private async Task SubmitMFAIdentity()
    {
        await Assertions.Expect(page.Locator("#credentialPickerTitle")).ToContainTextAsync("Verify your identity", new LocatorAssertionsToContainTextOptions { Timeout = 15000 });

        await page.GetByTestId("Email").ClickAsync();

        objectContext.SetDebugInformation($"Clicked verify your identity email link");
    }

    private async Task SubmitMFACode(string username, DateTime dateTime)
    {
        await Assertions.Expect(page.Locator("#oneTimeCodeTitle")).ToContainTextAsync("Enter code", new LocatorAssertionsToContainTextOptions { Timeout = 15000 });

        await Assertions.Expect(page.Locator("#oneTimeCodeDescription")).ToContainTextAsync("We emailed a code");

        await Assertions.Expect(page.Locator("#oneTimeCodeDescription")).ToContainTextAsync("Please enter the code to sign in");

        await retryHelper.RetryOnDfeSignMFAAuthCode(async () =>
        {
            var codes = await context.Get<MailosaurApiHelper>().GetCodes(username, "Your DfE Sign-in (PREPROD) account verification code", "Account verification code:", dateTime);

            objectContext.SetDebugInformation($"Used codes are ({usedCodes.Select(x => $"'{x}'").ToString(",")})");

            objectContext.SetDebugInformation($"Codes from email are ({codes.Select(x => $"'{x}'").ToString(",")})");

            var notusedcodes = codes.Except(usedCodes);

            Assert.That(notusedcodes.Any(), "All email codes are used or no code received");

            await EnterMFACode(notusedcodes);

            await Assertions.Expect(page.Locator("#oneTimeCodeTitle")).ToBeHiddenAsync(new LocatorAssertionsToBeHiddenOptions { Timeout = 10000 });
        });
    }

    private async Task EnterMFACode(IEnumerable<string> notusedcodes)
    {
        var codeerrorlocator = "div[id='undefinedError'][role='alert']";

        var codeerror = page.Locator(codeerrorlocator);

        objectContext.SetDebugInformation($"Codes to try ({notusedcodes.Select(x => $"'{x}'").ToString(",")})");

        foreach (var notusedcode in notusedcodes)
        {
            try
            {
                await page.GetByRole(AriaRole.Textbox, new() { Name = "Enter code" }).FillAsync(notusedcode);

                await page.GetByRole(AriaRole.Button, new() { Name = "Verify" }).ClickAsync();

                objectContext.SetDebugInformation($"Entered code - '{notusedcode}'");

                await Assertions.Expect(page.GetByTestId("bannerLogoText")).ToBeHiddenAsync(new LocatorAssertionsToBeHiddenOptions { Timeout = 10000 });

                await Assertions.Expect(page.Locator("#userDisplayName")).ToBeHiddenAsync(new LocatorAssertionsToBeHiddenOptions { Timeout = 10000 });

                await Assertions.Expect(codeerror).ToBeHiddenAsync(new LocatorAssertionsToBeHiddenOptions { Timeout = 10000 });

                usedCodes.Add(notusedcode);

                objectContext.SetDebugInformation($"No code error found - completing Task");

                return;
            }
            catch (Exception)
            {
                // do nothing, the loop will try the next code
            }
        }
    }
}
