using Polly;
using Polly.Retry;
using SFA.DAS.Approvals.UITests.Project.Helpers.DataHelpers.ApprenticeshipModel;
using SFA.DAS.Approvals.UITests.Project.Helpers.SqlHelpers;
using SFA.DAS.Approvals.UITests.Project.Helpers.StepsHelper;
using SFA.DAS.Approvals.UITests.Project.Helpers.TestDataHelpers;
using SFA.DAS.EmployerPortal.UITests.Project.Pages.CreateAccount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Approvals.UITests.Project.Steps
{
    [Binding]
    internal class DbSteps
    {
        protected readonly ScenarioContext context;
        protected readonly ObjectContext objectContext; 
        private readonly AccountsDbSqlHelper accountsDbSqlHelper;
        private readonly CommitmentsDbSqlHelper commitmentsDbSqlHelper;
        private readonly LearningDbSqlHelper learningDbSqlHelper;
        private readonly LearnerDataDbSqlHelper learnerDataDbSqlHelper;
        private readonly ApprenticeDataHelper apprenticeDataHelper;
        private List<Apprenticeship> listOfApprenticeship;


        public DbSteps(ScenarioContext context)
        {
            this.context = context;
            objectContext = context.Get<ObjectContext>();
            accountsDbSqlHelper = context.Get<AccountsDbSqlHelper>();
            commitmentsDbSqlHelper = context.Get<CommitmentsDbSqlHelper>();
            learningDbSqlHelper = context.Get<LearningDbSqlHelper>();
            learnerDataDbSqlHelper = context.Get<LearnerDataDbSqlHelper>();
            accountsDbSqlHelper = context.Get<AccountsDbSqlHelper>();
            apprenticeDataHelper = new ApprenticeDataHelper(context);
        }

        [Then("a record is created in LearnerData Db for each learner")]
        public async Task ThenARecordIsCreatedInLearnerDataDbForEachLearner()
        {
            listOfApprenticeship = context.GetValue<List<Apprenticeship>>();
            var retryPolicy = DbRetryPolicy("LearnerDataId", "LearnerData db");

            foreach (var apprenticeship in listOfApprenticeship)
            {
                var uln = apprenticeship.ApprenticeDetails.ULN;
                var learnerDataId = await retryPolicy.ExecuteAsync(() => learnerDataDbSqlHelper.GetLearnerDataId(uln));
                Assert.IsNotEmpty(learnerDataId, $"No record found in LearnerData db for ULN: {uln}");
                apprenticeship.ApprenticeDetails.LearnerDataId = Convert.ToInt32(learnerDataId);
                await Task.Delay(100);
                context.Set(apprenticeship, "Apprenticeship");
                objectContext.SetDebugInformation($"[{learnerDataId} set as learnerDataId for ULN: {uln}]");
            }

        }


        [Then("Commitments Db is updated with respective LearnerData Id")]
        public async Task ThenCommitmentsDbIsUpdatedWithRespectiveLearnerDataId()
        {
            listOfApprenticeship = context.GetValue<List<Apprenticeship>>();

            foreach (var apprenticeship in listOfApprenticeship)
            {
                var uln = apprenticeship.ApprenticeDetails.ULN;
                var learnerDataIdExpected = apprenticeship.ApprenticeDetails.LearnerDataId;
                var learnerDataIdActual = await commitmentsDbSqlHelper.GetValueFromApprenticeshipTable("LearnerDataId", uln);
                Assert.AreEqual(learnerDataIdExpected.ToString(), learnerDataIdActual, $"[LearnerDataId] from Commitments db ({learnerDataIdActual}) does not match with [Id] in LearnerData db ({learnerDataIdExpected})");
                var apprenticehipId = await commitmentsDbSqlHelper.GetValueFromApprenticeshipTable("Id", uln);
                apprenticeship.ApprenticeDetails.ApprenticeshipId = Convert.ToInt32(apprenticehipId);
                await Task.Delay(100);
                context.Set(apprenticeship, "Apprenticeship");
                objectContext.SetDebugInformation($"[{apprenticehipId} set as AprenticeshipID for ULN: {uln}]");
            }
            
        }

        [Then("LearnerData Db is updated with respective Apprenticeship Id")]
        public async Task ThenLearnerDataDbIsUpdatedWithRespectiveApprenticeshipId()
        {
            listOfApprenticeship = context.GetValue<List<Apprenticeship>>();
            var retryPolicy = DbRetryPolicy("ApprenticeshipId", "LearnerData db");

            foreach (var apprenticeship in listOfApprenticeship)
            {
                var uln = apprenticeship.ApprenticeDetails.ULN;
                var learnerDataId = apprenticeship.ApprenticeDetails.LearnerDataId;
                var apprenticeshipIdExpected = apprenticeship.ApprenticeDetails.ApprenticeshipId;
                var apprenticeshipIdActual = await retryPolicy.ExecuteAsync(() => learnerDataDbSqlHelper.GetApprenticeshipIdLinkedWithLearnerData(learnerDataId));
                Assert.AreEqual(apprenticeshipIdExpected.ToString(), apprenticeshipIdActual, $"[Id] from LearnerData db ({apprenticeshipIdActual}) does not match with [LearnerDataId] in Apprenticeship > Commitments db ({apprenticeshipIdExpected})");
            }

        }


        [Then("Apprenticeship record is created in Learning Db")]
        public async Task ThenApprenticeshipRecordIsCreatedInLearningDb()
        {
            listOfApprenticeship = context.GetValue<List<Apprenticeship>>();
            var retryPolicy = DbRetryPolicy("ApprenticeshipRecord", "Learning db");

            foreach (var apprenticeship in listOfApprenticeship)
            {
                var uln = apprenticeship.ApprenticeDetails.ULN;
                var apprenticeshipId = apprenticeship.ApprenticeDetails.ApprenticeshipId;
                var result = await retryPolicy.ExecuteAsync(() => learningDbSqlHelper.CheckIfApprenticeshipRecordCreatedInLearningDb(apprenticeshipId, uln));
                Assert.IsNotEmpty(result, $"Apprenticeship record not found in Learning Db for ApprenticeshipId: {apprenticeshipId}");
                apprenticeship.ApprenticeDetails.LearningIdKey = result;
                await Task.Delay(100);
                context.Set(apprenticeship, "Apprenticeship");
                objectContext.SetDebugInformation($"[{result} set as LearningIdKey for ULN: {uln}]");
            }
        }

        [Given("A live apprentice record exists for an apprentice on Foundation level course")]
        public async Task GivenALiveApprenticeRecordExistsForAnApprenticeOnFoundationLevelCourse()
        {
            listOfApprenticeship = new List<Apprenticeship>();

            var additionalWhereFilter = @"AND c.CreatedOn > DATEADD(month, -12, GETDATE())
                                            AND c.IsDeleted = 0
                                            And c.Approvals = 3
                                            AND c.ChangeOfPartyRequestId is null             
                                            AND c.PledgeApplicationId is null
                                            AND a.PaymentStatus = 1
                                            AND a.HasHadDataLockSuccess = 0
                                            AND a.PendingUpdateOriginator is null
                                            AND a.CloneOf is null
                                            AND a.ContinuationOfId is null
                                            AND a.DeliveryModel = 0
                                            AND a.TrainingCode IN('803','804','805','806','807','808','809', '810', '811')";

            await FindEditableApprenticeFromDbAndSaveItInContext(EmployerType.Levy, additionalWhereFilter);
        }

        [Given(@"a live apprentice record exists with startdate of <(.*)> months and endDate of <\+(.*)> months from current date")]
        public async Task GivenALiveApprenticeRecordExistsWithStartdateOfMonthsAndEndDateOfMonthsFromCurrentDate(int startDateFromNow, int endDateFromNow)
        {
            listOfApprenticeship = new List<Apprenticeship>();

            var additionalWhereFilter = @$"AND c.CreatedOn > DATEADD(month, -12, GETDATE())
                                            AND c.IsDeleted = 0
                                            And c.Approvals = 3
                                            AND c.ChangeOfPartyRequestId is null             
                                            AND c.PledgeApplicationId is null
                                            AND a.PaymentStatus = 1
                                            AND a.HasHadDataLockSuccess = 0
                                            AND a.PendingUpdateOriginator is null
                                            AND a.CloneOf is null
                                            AND a.ContinuationOfId is null
                                            AND a.DeliveryModel = 0
                                            AND a.StartDate < DATEADD(month, {startDateFromNow}, GETDATE()) 
                                            AND a.EndDate > DATEADD(month, {endDateFromNow}, GETDATE())
                                            AND a.TrainingCode < 800";

            await FindEditableApprenticeFromDbAndSaveItInContext(EmployerType.Levy, additionalWhereFilter);
        }

        internal async Task FindAvailableLearner()
        {
            listOfApprenticeship = new List<Apprenticeship>();
            var providerConfig = context.GetProviderConfig<ProviderConfig>();
            Apprenticeship apprenticeship = await apprenticeDataHelper.CreateEmptyCohortAsync(EmployerType.Levy, providerConfig);
            apprenticeship = await learnerDataDbSqlHelper.GetEditableApprenticeDetails(apprenticeship);
            listOfApprenticeship.Add(apprenticeship);
            context.Set(listOfApprenticeship);
        }

        private async Task FindEditableApprenticeFromDbAndSaveItInContext(EmployerType employerType, string additionalWhereFilter, string ukprn = null)
        {
            var providerConfig = context.GetProviderConfig<ProviderConfig>();
            Apprenticeship apprenticeship = await apprenticeDataHelper.CreateEmptyCohortAsync(employerType, providerConfig);
            apprenticeship = await commitmentsDbSqlHelper.GetApprenticeDetailsFromCommitmentsDb(apprenticeship, additionalWhereFilter);
            listOfApprenticeship.Add(apprenticeship);
            context.Set(listOfApprenticeship);
        }

        private AsyncRetryPolicy<string> DbRetryPolicy(string value, string dbName)
        {
            return Policy
                .HandleResult<string>(result => string.IsNullOrEmpty(result)) // Retry if result is null or empty  
                .WaitAndRetryAsync(
                    retryCount: 5,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(1),
                    onRetry: (result, timeSpan, retryCount, context) =>
                    {
                        objectContext.SetDebugInformation(
                            $"Retry {retryCount} - {value} not found in {dbName}. Waiting {timeSpan.TotalSeconds}s before next attempt.");
                    });
        }

    }
}
