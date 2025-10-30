using Azure;
using Microsoft.Playwright;
using SFA.DAS.ManagingStandards.UITests.Project.Tests.Pages;

namespace SFA.DAS.ManagingStandards.UITests.Project.Tests.Steps;

[Binding]
public class MS_ContactDetails_Steps(ScenarioContext context)
{
    private readonly ManagingStandardsDataHelpers managingStandardsDataHelpers = new();

    [Then("the provider is able to update contact details")]
    public async Task ThenTheProviderIsAbleToUpdateContactDetails()
    {
        var page = new ManagingStandardsProviderHomePage(context);

        var page1 = await page.NavigateToYourStandardsAndTrainingVenuesPage();

        var page2 = await page1.AccessContactDetails();

        var page3 = await page2.ChangeContactDetails();

        var page4 = await page3.ChangeEmailAndPhonenumberContactDetails();

        var page5 = await page4.NoDontUpdateExistingStandards();

        var page6 = await page5.ConfirmUpdateContactDetailsAndContinue();

        await page6.VerifyUpdatedContactDetails(managingStandardsDataHelpers.UpdatedEmailAddress,managingStandardsDataHelpers.ContactNumber);

        await page6.ReturnToManagingStandardsDashboard();
    }

    [Then("the provider is able to update phone number only")]
    public async Task ThenTheProviderIsAbleToUpdatePhoneNumberOnly()
    {
        var page = new YourStandardsAndTrainingVenuesPage(context);

        var page1 = await page.AccessContactDetails();

        var page2 = await page1.ChangeContactDetails();

        var page3 = await page2.ChangePhonenumberOnly();

        var page4 = await page3.NoDontUpdateExistingStandards();

        var page5 = await page4.ConfirmUpdateContactDetailsAndContinue();

        await page5.VerifyUpdatedContactDetails(managingStandardsDataHelpers.UpdatedContactNumber);

        await page5.ReturnToManagingStandardsDashboard();

    }

    [Then("the provider is able to update email only")]
    public async Task ThenTheProviderIsAbleToUpdateEmailOnly()
    {
        var page = new YourStandardsAndTrainingVenuesPage(context);

        var page1 = await page.AccessContactDetails();

        var page2 = await page1.ChangeContactDetails();

        var page3 = await page2.ChangeEmailOnly();

        var page4 = await page3.NoDontUpdateExistingStandards();

        var page5 = await page4.ConfirmUpdateContactDetailsAndContinue();

        await page5.VerifyUpdatedContactDetails( managingStandardsDataHelpers.NewlyUpdatedEmailAddress);

        await page5.ReturnToManagingStandardsDashboard();

    }

    [Then("the provider is able to update contact details to all the standards")]
    public async Task ThenTheProviderIsAbleToUpdateContactDetailsToAllTheStandards()
    {
        var page = new YourStandardsAndTrainingVenuesPage(context);

        var page1 = await page.AccessContactDetails();

        var page2 = await page1.ChangeContactDetails();

        var page3 = await page2.ChangeEmailAndPhonenumberContactDetails_Latest();

        var page4 = await page3.YesUpdateExistingStandards();

        var page5 = await page4.YesUpdateAllStandardsContactDetails();

        var page6 = await page5.ConfirmUpdateContactDetailsAndContinue();

        await page6.VerifyUpdatedContactDetails(managingStandardsDataHelpers.EmailAddress, managingStandardsDataHelpers.NewlyUpdatedContactNumber);

        await page6.ReturnToManagingStandardsDashboard();
    }

    [Then("the updated contact details are visible in the standard information section")]
    public async Task ThenTheUpdatedContactDetailsAreVisibleInTheStandardInformationSection()
    {

        var page = new YourStandardsAndTrainingVenuesPage(context);

        var page1 = await page.AccessStandards();
        var page2 = await page1.AccessPodiatrist();
        await page2.VerifyUpdatedContactDetailsVisibleInStandard(managingStandardsDataHelpers.EmailAddress, managingStandardsDataHelpers.NewlyUpdatedContactNumber);
    }
}