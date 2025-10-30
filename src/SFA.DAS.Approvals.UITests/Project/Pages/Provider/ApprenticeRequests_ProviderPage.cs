using Azure;
using SFA.DAS.Approvals.UITests.Project.Helpers.TestDataHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Approvals.UITests.Project.Pages.Provider
{
    internal class ApprenticeRequests_ProviderPage(ScenarioContext context) : ApprovalsBasePage(context)
    {
        public override async Task VerifyPage()
        {
            await Assertions.Expect(page.Locator("h1")).ToContainTextAsync("Apprentice requests");
        }

        internal async Task NavigateToBingoBoxAndVerifyCohortExists(ApprenticeRequests apprenticeRequests)
        {
            switch (apprenticeRequests)
            {
                case ApprenticeRequests.ReadyForReview:
                    await page.GetByRole(AriaRole.Link, new() { Name = "With transfer sending employers" }).ClickAsync();  //Has to toggle b/w boxes to make desired option clickable
                    await page.GetByRole(AriaRole.Link, new() { Name = "Ready for review" }).ClickAsync();
                    break;
                case ApprenticeRequests.WithEmployers:
                    await page.GetByRole(AriaRole.Link, new() { Name = "With employers" }).ClickAsync();
                    break;
                case ApprenticeRequests.Drafts:
                    await page.GetByRole(AriaRole.Link, new() { Name = "Drafts" }).ClickAsync();
                    break;
                case ApprenticeRequests.WithTransferSendingEmployers:
                    await page.GetByRole(AriaRole.Link, new() { Name = "With transfer sending employers" }).ClickAsync();
                    break;
            }

        }

        internal async Task<bool> VerifyCohortExistsAsync(string cohortRef)
        {
            await page.WaitForTimeoutAsync(1000);

            var locator = page.Locator($"#details_link_{cohortRef}");
            int count = await locator.CountAsync();
            return count > 0;
        }

        internal async Task<ApproveApprenticeDetailsPage> OpenEditableCohortAsync(string? cohortRef)
        {
            await ClickDetailsLinkAsync(cohortRef);
            return await VerifyPageAsync(() => new ApproveApprenticeDetailsPage(context));
        }

        internal async Task<ViewApprenticesDetails_ProviderPage> OpenNonEditableCohortAsync(string? cohortRef)
        {
            await ClickDetailsLinkAsync(cohortRef);
            return await VerifyPageAsync(() => new ViewApprenticesDetails_ProviderPage(context));
        }

        private async Task ClickDetailsLinkAsync(string? cohortRef)
        {
            if (cohortRef == null)
            {
                // Locate all "Details" links inside the table
                var detailsLinks = page.Locator("table.govuk-table a.cohort-details-link");

                // Click the last one
                await detailsLinks.Last.ClickAsync();
            }
            else
            {
                await page.Locator($"#details_link_{cohortRef}").ClickAsync();
            }

        }

    }
}
