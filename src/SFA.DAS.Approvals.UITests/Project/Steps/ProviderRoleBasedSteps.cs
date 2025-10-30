using Azure;
using Microsoft.Playwright;
using SFA.DAS.Approvals.UITests.Project.Helpers.DataHelpers.ApprenticeshipModel;
using SFA.DAS.Approvals.UITests.Project.Helpers.StepsHelper;
using SFA.DAS.Approvals.UITests.Project.Helpers.TestDataHelpers;
using SFA.DAS.Approvals.UITests.Project.Pages;
using SFA.DAS.Approvals.UITests.Project.Pages.Common;
using SFA.DAS.Approvals.UITests.Project.Pages.Provider;
using SFA.DAS.ProviderLogin.Service.Project.Helpers;
using SFA.DAS.ProviderLogin.Service.Project.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Approvals.UITests.Project.Steps
{
    [Binding]
    internal class ProviderRoleBasedSteps
    {
        private readonly ScenarioContext context;
        private readonly ProviderStepsHelper providerStepsHelper;
        private readonly DbSteps dbSteps;
        private const string ChangesForReviewApprentice = "DoNotUse_TestData ChangesForReviewApprentice";
        private const string ChangesPendingApprentice = "DoNotUse_TestData ChangesPendingApprentice";
        private const string ILRDataMisMatchAskEmployerToFix = "DoNotUse_TestData ILRDataMisMatchAskEmployerToFix";
        private const string ILRDataMisMatchRequestDetails = "DoNotUse_TestData ILRDataMisMatchRequestDetails";
        private const string LiveApprentice = "DoNotUse_TestData LiveApprentice";
        private const string StoppedApprentice = "DoNotUse_TestData StoppedApprentice";

        public ProviderRoleBasedSteps(ScenarioContext _context)
        {
            context = _context;
            providerStepsHelper = new ProviderStepsHelper(context);
            dbSteps = new DbSteps(context);
        }

        [Then("the user can view apprentice details from items under section: \"(.*)\"")]
        public async Task ThenTheUserCanViewApprenticeDetailsFromItemsUnderSection(string sectionName)
        {
            var apprenticeRequests_ProviderPage = new ApprenticeRequests_ProviderPage(context);
            IPageWithABackLink page;

            switch (sectionName)
            {
                case "Ready for review":
                    await apprenticeRequests_ProviderPage.NavigateToBingoBoxAndVerifyCohortExists(ApprenticeRequests.ReadyForReview);
                    page = await apprenticeRequests_ProviderPage.OpenEditableCohortAsync(null);
                    break;
                case "With employers":
                    await apprenticeRequests_ProviderPage.NavigateToBingoBoxAndVerifyCohortExists(ApprenticeRequests.WithEmployers);
                    page = await apprenticeRequests_ProviderPage.OpenNonEditableCohortAsync(null);
                    break;
                case "Drafts":
                    await apprenticeRequests_ProviderPage.NavigateToBingoBoxAndVerifyCohortExists(ApprenticeRequests.Drafts);
                    page = await apprenticeRequests_ProviderPage.OpenEditableCohortAsync(null);
                    break;
                case "With transfer sending employers":
                    await apprenticeRequests_ProviderPage.NavigateToBingoBoxAndVerifyCohortExists(ApprenticeRequests.WithTransferSendingEmployers);
                    page = await apprenticeRequests_ProviderPage.OpenNonEditableCohortAsync(null);
                    break;
                default:
                    throw new ArgumentException($"Unknown section name: {sectionName}");

            }
            await page.ClickOnBackLinkAsync();
        }

        [Then("the user can create a cohort by selecting learners from ILR")]
        public async Task ThenTheUserCanCreateACohortBySelectingLearnersFromILR()
        {
            await dbSteps.FindAvailableLearner();
            var page = await providerStepsHelper.GoToSelectApprenticeFromILRPage(false);
            await providerStepsHelper.AddFirstApprenticeFromILRList(page);
        }


        [Then("the user can edit email address of the apprentice before approval")]
        public async Task ThenTheUserCanEditEmailAddressOfTheApprenticeBeforeApprovalAsync()
        {
            var apprentice = context.GetValue<List<Apprenticeship>>().FirstOrDefault();
            var page = await new ApproveApprenticeDetailsPage(context).ClickOnEditApprenticeLink(apprentice.ApprenticeDetails.FullName);
            var page1 = await page.UpdateEmail(apprentice.ApprenticeDetails.Email + ".uk");
            var page3 = await page1.SelectNoForRPL();
        }

        [Then("the user can send a cohort to employer")]
        public async Task ThenTheUserCanSendACohortToEmployer()
        {
            await new ApproveApprenticeDetailsPage(context).VerifyCohortCanBeApproved();
        }

        [Then("the user can delete an apprentice in a cohort")]
        public async Task ThenTheUserCanDeleteAnApprenticeInACohort()
        {
            var page = await new ApproveApprenticeDetailsPage(context).ClickOnDeleteApprenticeLink("");
            var page1 = await page.ConfirmDeletion();
            await page1.VerifyBanner("Apprentice record deleted");
        }

        [Then("the user can delete a cohort")]
        public async Task ThenTheUserCanDeleteACohort()
        {
            var page = await new ApproveApprenticeDetailsPage(context).ClickOnDeleteCohortLink();
            await page.ConfirmDeletion();
        }

        [Then("the user cannot start add apprentice journey")]
        public async Task ThenTheUserCannotStartAddApprenticeJourney()
        {
            await new ApprenticeRequests_ProviderPage(context).ClickOnNavBarLinkAsync("Home");
            await new ProviderHomePage(context).AddNewApprentices();
            var page = new ProviderAccessDeniedPage(context);
            await page.VerifyPage();
            await page.GoBackToTheServiceHomePage();
        }

        [Then("the user cannot edit apprentice details in an existing cohort")]
        public async Task ThenTheUserCannotEditApprenticeDetailsInAnExistingCohort()
        {
            var page = await new ProviderHomePage(context).GoToApprenticeRequestsPage();
            await page.OpenLastCohortFromTheList();
            var page1 = await new ApproveApprenticeDetailsPage(context).TryOpenLink("Edit");
            await page1.NavigateBrowserBack();
        }

        [Then("the user cannot add another apprentice to a cohort")]
        public async Task ThenTheUserCannotAddAnotherApprenticeToACohort()
        {
            var page = await new ApproveApprenticeDetailsPage(context).ClickOnAddAnotherApprenticeLink_ToSelectEntryMthodPage();
            await page.SelectOptionToAddApprenticeFromILRAndContinue();
            var page1 = new ProviderAccessDeniedPage(context);
            await page1.VerifyPage();
            await page1.NavigateBrowserBack();
            await page.NavigateBrowserBack();
        }

        [Then("the user cannot delete an apprentice in an existing cohort")]
        public async Task ThenTheUserCannotDeleteAnApprenticeInAnExistingCohort()
        {
            var page1 = await new ApproveApprenticeDetailsPage(context).TryOpenLink("Delete");
            await page1.NavigateBrowserBack();
        }

        [Then("the user cannot delete an existing cohort")]
        public async Task ThenTheUserCannotDeleteAnExistingCohort()
        {
            var page1 = await new ApproveApprenticeDetailsPage(context).TryOpenLink("Delete this cohort");
            await page1.NavigateBrowserBack();
        }

        [Then("the user cannot send an existing cohort to employer")]
        public async Task ThenTheUserCannotSendAnExistingCohortToEmployer()
        {
            await new ApproveApprenticeDetailsPage(context).SelectFirstRadioButtonAndSubmit();
            var page = new ProviderAccessDeniedPage(context);
            await page.VerifyPage();
            await page.GoBackToTheServiceHomePage();
        }


        [When("user naviagates to FundingForNonLevyEmployers page")]
        public async Task WhenUserNaviagatesToFundingForNonLevyEmployersPage()
        {
            await new ProviderHomePage(context).GoToManageYourFunding();
        }

        [Then("the user \"(.*)\" reserve new funding")]
        public async Task ThenTheUserCanReserveNewFunding(string status)
        {
            var page = new FundingForNonLevyEmployersPage(context);
            await page.ClickOnReserveMoreFundingLink();

            if (status == "can")
            {
                await page.VerifyPageAsync(() => new ReserveFundingForNonLevyEmployersPage(context));
            }
            else
            {
                await page.VerifyPageAsync(() => new ProviderAccessDeniedPage(context));
            }

            await page.NavigateBrowserBack();
        }

        [Then("the user \"(.*)\" delete existing reservervations")]
        public async Task ThenTheUserCanDeleteExistingReservervations(string status)
        {
            var page = new FundingForNonLevyEmployersPage(context);
            await page.ClickOnDeleteReservationLink();

            if (status == "can")
            {
                await page.VerifyPageAsync(() => new ProviderDeleteReservationPage(context));
            }
            else
            {
                await page.VerifyPageAsync(() => new ProviderAccessDeniedPage(context));                
            }
            
            await page.NavigateBrowserBack();
        }

        [Then("the user \"(.*)\" add apprentices to an existing reservation")]
        public async Task ThenTheUserCanAddApprenticesToAnExistingReservation(string status)
        {
            var page = new FundingForNonLevyEmployersPage(context);
            await page.ClickOnAddApprenticeLink();

            if (status ==  "can")
            {
                await page.VerifyPageAsync(() => new AddApprenticeDetails_SelectJourneyPage(context));
            }
            else
            {
                await page.VerifyPageAsync(() => new ProviderAccessDeniedPage(context));
            }

            await page.NavigateBrowserBack();
        }

        [Then("the user can download csv file")]
        public async Task ThenTheUserCanDownloadCsvFile()
        {
            await new ManageYourApprentices_ProviderPage(context).DownloadCsv();
        }


        [Then("the user can view details of the apprenticeship on apprenticeship details page")]
        public async Task ThenTheUserCanViewDetailsOfTheApprenticeshipOnApprenticeshipDetailsPage()
        {
            var page = await new ManageYourApprentices_ProviderPage(context).SelectViewCurrentApprenticeDetails(LiveApprentice);
            await page.ReturnBackToManageYourApprenticesPage();
        }

        [Then("the user can view changes via view changes link in the banner")]
        public async Task ThenTheUserCanViewChangesViaViewChangesLinkInTheBanner()
        {
            var page = await new ManageYourApprentices_ProviderPage(context).SelectViewCurrentApprenticeDetails(ChangesForReviewApprentice);
            await page.AssertBanner("Action required", "The employer has made a change which needs to be reviewed and approved by you.");
            await page.ClickOnReviewChanges();
        }

        [Then("the user can view details of ILR mismatch via view details link in the ILR data mismatch banner")]
        public async Task ThenTheUserCanViewDetailsOfILRMismatchViaViewDetailsLinkInTheILRDataMismatchBanner()
        {
            var page = await new ManageYourApprentices_ProviderPage(context).SelectViewCurrentApprenticeDetails(ILRDataMisMatchRequestDetails);
            await page.AssertBanner("Action required", $"ILR data mismatch: Payment for {ILRDataMisMatchRequestDetails} can't be made until this is resolved.");
            await page.ClickOnViewDetails();
        }

        [Then("the user \"(.*)\" take action on details of ILR mismatch page by selecting any radio buttons on the page")]
        public async Task ThenTheUserTakeActionOnDetailsOfILRMismatchPageBySelectingAnyRadioButtonsOnThePage(string status)
        {
            var page = new DetailsOfIlrDataMismatchPage(context);
            await page.SelectILRDataMismatchOptions();

            if (status == "can")
            {
                await page.VerifyPageAsync(() => new DetailsOfIlrDataMismatchPage(context));
            }
            else
            {
                await page.VerifyPageAsync(() => new ProviderAccessDeniedPage(context));
                await page.NavigateBrowserBack();
            }
            await page.ClickOnBackLink();
            await new ApprenticeDetails_ProviderPage(context, ILRDataMisMatchRequestDetails).ReturnBackToManageYourApprenticesPage();

        }


        [Then("the user can view details of ILR mismatch request restart via view details link in the ILR data mismatch banner")]
        public async Task ThenTheUserCanViewDetailsOfILRMismatchRequestRestartViaViewDetailsLinkInTheILRDataMismatchBanner()
        {
            var page = await new ManageYourApprentices_ProviderPage(context).SelectViewCurrentApprenticeDetails(ILRDataMisMatchAskEmployerToFix);
            await page.AssertBanner("Action required", $"ILR data mismatch: Payment for {ILRDataMisMatchAskEmployerToFix} can't be made until this is resolved.");
            await page.ClickOnViewDetails();
            
        }

        [Then("the user \"(.*)\" take action on details of ILR mismatch request restart via view details link in the ILR data mismatch banner")]
        public async Task ThenTheUserTakeActionOnDetailsOfILRMismatchRequestRestartViaViewDetailsLinkInTheILRDataMismatchBanner(string status)
        {
            var page = new DetailsOfIlrDataMismatchPage(context);
            await page.SelectILRDataMismatchOptions();

            if (status == "can")
            {
                await page.VerifyPageAsync(() => new DetailsOfIlrDataMismatchPage(context));
            }
            else
            {
                await page.VerifyPageAsync(() => new ProviderAccessDeniedPage(context));
                await page.NavigateBrowserBack();
            }
            await page.ClickOnBackLink();
            await new ApprenticeDetails_ProviderPage(context, ILRDataMisMatchAskEmployerToFix).ReturnBackToManageYourApprenticesPage();

        }


        [Then("the user can view review changes via review details link in the banner")]
        public async Task ThenTheUserCanViewReviewChangesViaReviewDetailsLinkInTheBanner()
        {
            var page = await new ManageYourApprentices_ProviderPage(context).SelectViewCurrentApprenticeDetails(ChangesForReviewApprentice);
            await page.AssertBanner("Action required", $"The employer has made a change which needs to be reviewed and approved by you.");
            await page.ClickOnReviewChanges();
        }

        [Then("the user \"(.*)\" take action on review changes page")]
        public async Task ThenTheUserTakeActionOnReviewChangesPage(string status)
        {
            var page = new ReviewChangesPage(context);
            await page.SelectReviewChangesOptions();

            if (status == "can")
            {
                await page.VerifyPageAsync(() => new ReviewChangesPage(context));               
            }
            else
            {
                await page.VerifyPageAsync(() => new ProviderAccessDeniedPage(context));
                await page.NavigateBrowserBack();
            }
            await page.ClickOnCancelAndReturn();
            await new ApprenticeDetails_ProviderPage(context, ChangesForReviewApprentice).ReturnBackToManageYourApprenticesPage();

        }


        [Then("the user can view view changes nonCoE page via view changes link in the banner")]
        public async Task ThenTheUserCanViewViewChangesNonCoEPageViaViewChangesLinkInTheBanner()
        {
            var page = await new ManageYourApprentices_ProviderPage(context).SelectViewCurrentApprenticeDetails(ChangesPendingApprentice);
            await page.AssertBanner2("Changes to this apprenticeship", $"You have made a change which needs to be approved by the employer.");
            await page.ClickOnViewChanges();
        }

        [Then("the user \"(.*)\" take action on View changes on nonCoE page")]
        public async Task ThenTheUserTakeActionOnViewChangesOnNonCoEPage(string status)
        {
            var page = new ViewChangesPage(context);
            await page.SelectViewChangesOptions();

            if (status == "can")
            {
                await page.VerifyPageAsync(() => new ViewChangesPage(context));
            }
            else
            {
                await page.VerifyPageAsync(() => new ProviderAccessDeniedPage(context));
                await page.NavigateBrowserBack();
            }
            await page.ClickOnCancelAndReturn();
            await new ApprenticeDetails_ProviderPage(context, ChangesPendingApprentice).ReturnBackToManageYourApprenticesPage();

        }


        [Then("the user \"(.*)\" trigger change of employer journey using change link against the employer field")]
        public async Task ThenTheUserTriggerChangeOfEmployerJourneyUsingChangeLinkAgainstTheEmployerField(string status)
        {
            var page = await new ManageYourApprentices_ProviderPage(context).SelectViewCurrentApprenticeDetails(StoppedApprentice);
            await page.ClickOnChangeEmployerLink();
            var page1 = new ChangeOfEmployerInformationPage(context);
            if (status == "can")
            {
                await page1.VerifyPageAsync(() => new ChangeOfEmployerInformationPage(context));
            }
            else
            {
                await page.VerifyPageAsync(() => new ProviderAccessDeniedPage(context));
                await page.NavigateBrowserBack();
            }
            await page1.ClickOnBackLink();
            await page.ReturnBackToManageYourApprenticesPage();

        }


        [Then("the user \"(.*)\" edit an existing apprenticeship record by selecting edit apprentice link under manage apprentices")]
        public async Task ThenTheUserEditAnExistingApprenticeshipRecordBySelectingEditApprenticeLinkUnderManageApprentices(string status)
        {
            var page = await new ManageYourApprentices_ProviderPage(context).SelectViewCurrentApprenticeDetails(LiveApprentice);
            await page.ClickOnEditApprenticeDetailsLink();
            var page1 = new EditApprenticeDetails_ProviderPage(context);

            if (status == "can")
            {
                await page.VerifyPageAsync(() => new EditApprenticeDetails_ProviderPage(context));
                await page1.ClickOnCancelAndReturnLink();
            }
            else 
            {
                await page.VerifyPageAsync(() => new ProviderAccessDeniedPage(context));
                await page.NavigateBrowserBack();
            }            
            await page.ReturnBackToManageYourApprenticesPage();

        }




    }
}
