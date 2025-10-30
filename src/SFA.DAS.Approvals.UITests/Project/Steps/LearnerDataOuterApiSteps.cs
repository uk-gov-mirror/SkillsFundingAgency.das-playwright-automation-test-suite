using Polly;
using SFA.DAS.Approvals.UITests.Project.Helpers.API;
using SFA.DAS.Approvals.UITests.Project.Helpers.DataHelpers.ApprenticeshipModel;
using SFA.DAS.Approvals.UITests.Project.Helpers.StepsHelper;
using SFA.DAS.Approvals.UITests.Project.Helpers.TestDataHelpers;
using SFA.DAS.Approvals.UITests.Project.Pages.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Approvals.UITests.Project.Steps
{
    [Binding]
    public class LearnerDataOuterApiSteps
    {
        private readonly ScenarioContext context;
        private readonly CommonStepsHelper commonStepsHelper;
        private readonly LearnerDataOuterApiHelper learnerDataOuterApiHelper;
        private ProviderStepsHelper providerStepsHelper;
        private ApprenticeDataHelper apprenticeDataHelper;

        public LearnerDataOuterApiSteps(ScenarioContext _context)
        {
            context = _context;
            commonStepsHelper = new CommonStepsHelper(context);
            providerStepsHelper = new ProviderStepsHelper(context);
            learnerDataOuterApiHelper = new LearnerDataOuterApiHelper(context);
            apprenticeDataHelper = new ApprenticeDataHelper(context);
        }

        [Given("Provider successfully submits (\\d+) ILR record containing a learner record for a \"(.*)\" Employer")]
        public async Task ProviderSubmitsAnILRRecord(int NoOfApprentices, string type)
        {
            EmployerType employerType;

            switch (type.ToLower())
            {
                case "levy":
                    employerType = EmployerType.Levy;
                    break;
                case "nonlevy":
                    employerType = EmployerType.NonLevy;
                    break;
                case "nonlevyuseratmaxreservationlimit":
                    employerType = EmployerType.NonLevyUserAtMaxReservationLimit;
                    break;
                default:
                    throw new ArgumentException($"Unknown employer type: {type}");
            }

            var listOfApprenticeship = await new ApprenticeDataHelper(context).CreateApprenticeshipAsync(employerType, NoOfApprentices);

            context.Set(listOfApprenticeship);
        }


        [Given("SLD push its data into AS")]
        public async Task SLDPushDataIntoAS()
        {
            var listOfApprenticeship = context.GetValue<List<Apprenticeship>>();

            var academicYear = listOfApprenticeship.FirstOrDefault().TrainingDetails.AcademicYear;

            var listOflearnerData = await learnerDataOuterApiHelper.ConvertToLearnerDataAPIDataModel(listOfApprenticeship);

            await learnerDataOuterApiHelper.PushNewLearnersDataToAS(listOflearnerData, academicYear);
        }

        [Given(@"Provider sends an apprentice request \(cohort\) to an employer")]
        public async Task GivenProviderSendsAnApprenticeRequestCohortToAnEmployer()
        {
            await ProviderSubmitsAnILRRecord(1, EmployerType.Levy.ToString());
            await SLDPushDataIntoAS();

            var page = await new ProviderStepsHelper(context).ProviderCreateAndApproveACohortViaIlrRoute();
            var cohortRef = context.GetValue<List<Apprenticeship>>().FirstOrDefault().Cohort.Reference;

            await page.NavigateToBingoBoxAndVerifyCohortExists(ApprenticeRequests.WithEmployers);
            await page.VerifyCohortExistsAsync(cohortRef);

        }

        [Given("Provider adds an apprentice aged (.*) years using Foundation level standard")]
        public async Task GivenProviderAddsAnApprenticeAgedYearsUsingFoundationLevelStandard(int age)
        {
            var coursesDataHelper = new CoursesDataHelper();
            var employerType = EmployerType.Levy;

            //create apprenticeships object with Foundation level standard and a learner aged > 25 years:
            var foundationTrainingDetails = new TrainingFactory(coursesDataHelper => coursesDataHelper.GetRandomFoundationCourses());
            var apprenticeDetails = new ApprenticeFactory(age+1);

            var listOfApprenticeship = await apprenticeDataHelper.CreateApprenticeshipAsync(employerType, 1, null, null, apprenticeFactory: apprenticeDetails, trainingFactory: foundationTrainingDetails);
            context.Set(listOfApprenticeship);
            await SLDPushDataIntoAS();
        }

        [Then("system allows to approve apprentice details with a warning if their age is in range of (.*) - (.*) years")]
        public async Task ThenSystemAllowsToapproveApprenticeDetailsWithAWarningIfTheirAgeIsInRangeOf_Years(int lowerAgeLimit, int upperAgeLimit)
        {
            var page = await UpdateDobAndReprocessData(lowerAgeLimit, upperAgeLimit);
            var warningMsg = "! Warning Check apprentices are eligible for foundation apprenticeships If someone is aged between 22 and 24, to be funded for a foundation apprenticeship they must either: have an Education, Health and Care (EHC) plan be or have been in the care of their local authority be a prisoner or have been in prison";
            await page.ValidateWarningMessageForFoundationCourses(warningMsg);
            await providerStepsHelper.ProviderApproveCohort(page);
            await commonStepsHelper.SetCohortDetails(null, "Under review with Employer", "Ready for approval");

        }
       
        [Given("new learner details are processed in ILR for (\\d+) apprentices")]
        [Given("the employer has (\\d+) apprentice ready to start training")]
        public async Task ProcessedLearnersInILR(int NoOfApprentices)
        {
            var employerType = context.ScenarioInfo.Title.ToLower().Contains("nonlevy") ? EmployerType.NonLevy : EmployerType.Levy;
            await ProviderSubmitsAnILRRecord(NoOfApprentices, employerType.ToString());
            await SLDPushDataIntoAS();
        }

        [Then(@"apprentice\/learner record is available on Learning endpoint for SLD \(so they do not resubmit it\)")]
        public async Task ThenApprenticeLearnerRecordIsAvailableOnLearningEndpointForSLDSoTheyDoNotResubmitIt()
        {
            var listOfApprenticeship = context.GetValue<List<Apprenticeship>>();
            var academicYear = listOfApprenticeship.FirstOrDefault().TrainingDetails.AcademicYear;
            await learnerDataOuterApiHelper.CheckApprenticeIsAvailableInApprovedLearnersList(listOfApprenticeship.FirstOrDefault());
            
        }

        private async Task<ApproveApprenticeDetailsPage> UpdateDobAndReprocessData(int lowerAgeLimit, int upperAgeLimit)
        {
            var currentDate = DateTime.Now;
            var listOfApprenticeship = context.GetValue<List<Apprenticeship>>();

            foreach (var apprentice in listOfApprenticeship)
            {
                var newDoB = RandomDataGenerator.GenerateRandomDate(currentDate.AddYears(-upperAgeLimit), currentDate.AddYears(-lowerAgeLimit));
                apprentice.ApprenticeDetails.DateOfBirth = newDoB;
            }
            context["listOfApprenticeship"] = listOfApprenticeship;

            await SLDPushDataIntoAS();

            var page = await providerStepsHelper.GoToSelectApprenticeFromILRPage();
            return await providerStepsHelper.AddFirstApprenticeFromILRList(page);

        }



    }

}
