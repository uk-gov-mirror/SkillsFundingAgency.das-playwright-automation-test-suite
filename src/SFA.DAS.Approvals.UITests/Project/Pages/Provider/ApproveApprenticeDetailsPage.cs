using Azure;
using SFA.DAS.Approvals.UITests.Project.Helpers.DataHelpers.ApprenticeshipModel;
using SFA.DAS.FrameworkHelpers;
using SFA.DAS.ProviderLogin.Service.Project.Pages;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace SFA.DAS.Approvals.UITests.Project.Pages.Provider
{
    internal class ApproveApprenticeDetailsPage(ScenarioContext context) : ApprovalsBasePage(context), IPageWithABackLink

    {
        #region locators
        private ILocator banner => page.Locator("#main-content");
        private ILocator employerName => page.Locator("dt:has-text('Employer') + dd");
        private ILocator cohortReference => page.Locator("dt:has-text('Cohort reference') + dd");
        private ILocator status => page.Locator("dt:has-text('Status') + dd");
        private ILocator message => page.Locator("h2:has-text('Message') + div.govuk-inset-text");
        private ILocator row(string ULN) => page.Locator($"table tbody tr:has-text('{ULN}')");
        private ILocator editLink(string name) => page.GetByRole(AriaRole.Link, new() { Name = $"View{name}" }).First;
        private ILocator deleteLink(string name) => page.GetByRole(AriaRole.Link, new() { Name = $"Delete{name}" }).First;
        private ILocator AddAnotherApprenticeLink => page.Locator("a:has-text('Add another apprentice')");
        private ILocator DeleteThisCohortLink => page.GetByRole(AriaRole.Link, new() { Name = "Delete this cohort" }).First;
        private ILocator approveRadioOption => page.Locator("label:has-text('Yes, approve and notify employer')");
        private ILocator firstRadioOption => page.Locator("div.govuk-radios__item input[type='radio']").First;
        private ILocator doNotApproveRadioOption => page.Locator("label:has-text('No, save and return to apprentice requests')");
        private ILocator messageToEmployerTextBox => page.Locator(".govuk-textarea").First;
        private ILocator saveAndSubmitButton => page.Locator("button:has-text('Save and submit')");
        private ILocator saveAndexitLink => page.Locator("a:has-text('Save and exit')");
        private ILocator Name(ILocator apprenticeRow) => apprenticeRow.Locator("td:nth-child(1)");
        private ILocator Uln(ILocator apprenticeRow) => apprenticeRow.Locator("td:nth-child(2)");
        private ILocator Dob(ILocator apprenticeRow) => apprenticeRow.Locator("td:nth-child(3)");
        private ILocator TrainingDates(ILocator apprenticeRow) => apprenticeRow.Locator("td:nth-child(4)");
        private ILocator Price(ILocator apprenticeRow) => apprenticeRow.Locator("td:nth-child(5)");
        private ILocator sendToEmployerRadioOption => page.Locator("label:has-text('No, send to employer to review or add details')");
        private ILocator messageToEmployerToReviewTextBox => page.Locator(".govuk-textarea").Last;
        #endregion


        public override async Task VerifyPage()
        {
            var headerText = await page.Locator(".govuk-heading-xl").First.TextContentAsync();
            Assert.IsTrue(Regex.IsMatch(headerText ?? "", "Approve apprentice details|Approve 2 apprentices' details"));
        }

        public async Task ClickOnBackLinkAsync() => await page.Locator("a.govuk-back-link").ClickAsync();

        internal async Task VerifyCohort(Apprenticeship apprenticeship)
        {
            await Assertions.Expect(employerName).ToHaveTextAsync(apprenticeship.EmployerDetails.EmployerName.ToString());
            await Assertions.Expect(cohortReference).ToHaveTextAsync(apprenticeship.Cohort.Reference);
            await Assertions.Expect(status).ToHaveTextAsync("New request");
            //await Assertions.Expect(message).ToHaveTextAsync("No message added.");

            var expectedName = apprenticeship.ApprenticeDetails.FullName;
            var expectedULN = apprenticeship.ApprenticeDetails.ULN.ToString();
            var expectedDOB = apprenticeship.ApprenticeDetails.DateOfBirth.ToString("d MMM yyyy", CultureInfo.InvariantCulture);
            var expectedTrainingDates = apprenticeship.TrainingDetails.StartDate.ToString("d MMM yyyy", CultureInfo.InvariantCulture) + " to " + apprenticeship.TrainingDetails.EndDate.ToString("MMM yyyy", CultureInfo.InvariantCulture);
            var expectedPrice = apprenticeship.TrainingDetails.TotalPrice.ToString("C0");

            var apprenticeRow = row(apprenticeship.ApprenticeDetails.ULN.ToString());
            await Assertions.Expect(Name(apprenticeRow)).ToHaveTextAsync(expectedName.Trim());
            await Assertions.Expect(Uln(apprenticeRow)).ToHaveTextAsync(expectedULN.Trim());
            await Assertions.Expect(Dob(apprenticeRow)).ToHaveTextAsync(expectedDOB.Trim());
            await Assertions.Expect(TrainingDates(apprenticeRow)).ToHaveTextAsync(expectedTrainingDates.Trim());
            await Assertions.Expect(Price(apprenticeRow)).ToHaveTextAsync(expectedPrice.Trim());
        
        }

        internal async Task GetCohortId(Apprenticeship apprenticeship)
        {
            var cohortRef = await cohortReference.InnerTextAsync();
            apprenticeship.Cohort.Reference = cohortRef;

            await Task.Delay(100);
            context.Set(apprenticeship, "Apprenticeship");
        }

        internal async Task<ViewApprenticeDetails_ProviderPage> ClickOnEditApprenticeLink(string name)
        {
            await editLink("  " + name).ClickAsync();
            return await VerifyPageAsync(() => new ViewApprenticeDetails_ProviderPage(context));
        }

        internal async Task<SelectLearnerFromILRPage> ClickOnAddAnotherApprenticeLink()
        {
            await AddAnotherApprenticeLink.ClickAsync();
            return await VerifyPageAsync(() => new SelectLearnerFromILRPage(context));
        }

        internal async Task<ProviderSelectAReservationPage> ClickOnAddAnotherApprenticeLink_SelectReservationRoute()
        {
            await AddAnotherApprenticeLink.ClickAsync();
            return await VerifyPageAsync(() => new ProviderSelectAReservationPage(context));
        }

        internal async Task<AddApprenticeDetails_EntryMothodPage> ClickOnAddAnotherApprenticeLink_ToSelectEntryMthodPage()
        {
            await AddAnotherApprenticeLink.ClickAsync();
            return await VerifyPageAsync(() => new AddApprenticeDetails_EntryMothodPage(context));            
        }
        internal async Task<CohortApprovedAndSentToEmployerPage> ProviderApproveCohort()
        {
            await approveRadioOption.ClickAsync();
            await messageToEmployerTextBox.FillAsync("Please review the details and approve the request.");
            await saveAndSubmitButton.ClickAsync();
            return await VerifyPageAsync(() => new CohortApprovedAndSentToEmployerPage(context));
        }

        internal async Task<CohortSentToEmployerForReview> ProviderSendCohortForEmployerReview()
        {
            await sendToEmployerRadioOption.ClickAsync();
            await messageToEmployerToReviewTextBox.FillAsync("Please review the details and approve the request.");
            await saveAndSubmitButton.ClickAsync();
            return await VerifyPageAsync(() => new CohortSentToEmployerForReview(context));
        }

        internal async Task ValidateWarningMessageForFoundationCourses(string warningMsg) => await Assertions.Expect(page.Locator("#main-content")).ToContainTextAsync(warningMsg);

        internal async Task<CohortApproved> ProviderApprovesCohortAfterEmployerApproval()
        {
            await approveRadioOption.ClickAsync();
            await saveAndSubmitButton.ClickAsync();
            return await VerifyPageAsync(() => new CohortApproved(context));
        }

        internal async Task VerifyCohortCanBeApproved()
        {
            await approveRadioOption.ClickAsync();
            await messageToEmployerTextBox.FillAsync("Please review the details and approve the request.");
        }

        internal async Task<ConfirmApprenticeDeletionPage> ClickOnDeleteApprenticeLink(string name)
        {
            await deleteLink("  " + name).ClickAsync();
            return await VerifyPageAsync(() => new ConfirmApprenticeDeletionPage(context));
        }

        internal async Task<ConfirmCohortDeletionPage> ClickOnDeleteCohortLink()
        {
            await DeleteThisCohortLink.ClickAsync();
            return await VerifyPageAsync(() => new ConfirmCohortDeletionPage(context));
        }

        internal async Task VerifyBanner(string text) => await Assertions.Expect(banner).ToContainTextAsync(text);

        internal async Task SelectFirstRadioButtonAndSubmit(string optionalMsg=null)
        {
            await firstRadioOption.CheckAsync();
            await saveAndSubmitButton.ClickAsync();
        }


        internal async Task<ProviderAccessDeniedPage> TryOpenLink(string linkName)
        {
            await page.GetByRole(AriaRole.Link, new() { Name = linkName }).Last.ClickAsync();
            return await VerifyPageAsync(() => new ProviderAccessDeniedPage(context));
        }

    }
}
