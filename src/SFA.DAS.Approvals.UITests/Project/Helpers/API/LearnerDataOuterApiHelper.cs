using RestSharp;
using SFA.DAS.Approvals.UITests.Project.Helpers.DataHelpers.ApprenticeshipModel;
using SFA.DAS.LearnerData.Events;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SFA.DAS.Approvals.UITests.Project.Helpers.API
{
    internal class LearnerDataOuterApiHelper
    {
        private readonly ScenarioContext context;
        private readonly ObjectContext objectContext;
        private readonly LearnerDataOuterApiClient learnerDataOuterApiClient;
        private RestResponse restResponse = null;

        public LearnerDataOuterApiHelper(ScenarioContext _context)
        {
            context = _context;
            objectContext = context.Get<ObjectContext>();
            learnerDataOuterApiClient = context.Get<LearnerDataOuterApiClient>();
        }

        public async Task PushNewLearnersDataToAS(List<LearnerDataAPIDataModel> learnersData, int academicYear)
        {
            var serviceBusHelper = GlobalTestContext.ServiceBus;

            foreach (var learner in learnersData)
            {
                var learnerDataEvent = new LearnerDataEvent
                {
                    ULN = long.Parse(learner.uln),
                    UKPRN = long.Parse(learner.ukprn),
                    FirstName = learner.firstname,
                    LastName = learner.lastname,
                    Email = learner.learnerEmail,
                    DoB = DateTime.Parse(learner.dateOfBirth),
                    StartDate = DateTime.Parse(learner.startDate),
                    PlannedEndDate = DateTime.Parse(learner.plannedEndDate),
                    PercentageLearningToBeDelivered = learner.percentageLearningToBeDelivered,
                    EpaoPrice = learner.epaoPrice,
                    TrainingPrice = learner.trainingPrice,
                    AgreementId = learner.agreementId,
                    IsFlexiJob = learner.isFlexiJob,
                    StandardCode = learner.standardCode,
                    CorrelationId = Guid.NewGuid(),
                    ReceivedDate = DateTime.UtcNow,
                    ConsumerReference = learner.consumerReference,
                    PlannedOTJTrainingHours = learner.plannedOTJTrainingHours,
                    AcademicYear = academicYear
                };

                await serviceBusHelper.Publish(learnerDataEvent);
            }
        }

        public async Task CheckApprenticeIsAvailableInApprovedLearnersList(Apprenticeship apprenticeship)
        {
            var resource = $"/Learners/providers/{apprenticeship.ProviderDetails.Ukprn}/academicyears/{apprenticeship.TrainingDetails.AcademicYear}/learners";
            var learnerKey = await GetLearnerKeyByUlnAsync(resource, apprenticeship.ApprenticeDetails.ULN);
            var expectedLearningIdKey = apprenticeship.ApprenticeDetails.LearningIdKey.Trim();
            Assert.AreEqual(learnerKey.Trim(), expectedLearningIdKey, $"LearningIdKey key extracted from db [{expectedLearningIdKey}] differs from api response: [{learnerKey.Trim()}]");

        }

        public async Task<List<LearnerDataAPIDataModel>> ConvertToLearnerDataAPIDataModel(List<Apprenticeship> listOfApprenticeships)
        {
            List<LearnerDataAPIDataModel> listOfLearnerData = new List<LearnerDataAPIDataModel>();
            await Task.Delay(100);

            foreach (var apprenticeship in listOfApprenticeships)
            {
                listOfLearnerData.Add(await ConvertToLearnerDataAPIDataModel(apprenticeship));
            }

            return listOfLearnerData;
        }


        private async Task<LearnerDataAPIDataModel> ConvertToLearnerDataAPIDataModel(Apprenticeship apprenticeship)
        {
            LearnerDataAPIDataModel learnerData = new LearnerDataAPIDataModel();

            learnerData.ukprn = apprenticeship.ProviderDetails.Ukprn.ToString();
            learnerData.uln = apprenticeship.ApprenticeDetails.ULN;
            learnerData.firstname = apprenticeship.ApprenticeDetails.FirstName;
            learnerData.lastname = apprenticeship.ApprenticeDetails.LastName;
            learnerData.learnerEmail = apprenticeship.ApprenticeDetails.Email;
            learnerData.dateOfBirth = apprenticeship.ApprenticeDetails.DateOfBirth.ToString("yyyy-MM-dd");
            learnerData.startDate = apprenticeship.TrainingDetails.StartDate.ToString("yyyy-MM-dd");
            learnerData.plannedEndDate = apprenticeship.TrainingDetails.EndDate.ToString("yyyy-MM-dd");
            learnerData.percentageLearningToBeDelivered = apprenticeship.TrainingDetails.PercentageLearningToBeDelivered;
            learnerData.epaoPrice = apprenticeship.TrainingDetails.EpaoPrice;
            learnerData.trainingPrice = apprenticeship.TrainingDetails.TrainingPrice;
            learnerData.agreementId = apprenticeship.EmployerDetails.AgreementId;
            learnerData.isFlexiJob = apprenticeship.TrainingDetails.IsFlexiJob;
            learnerData.plannedOTJTrainingHours = apprenticeship.TrainingDetails.PlannedOTJTrainingHours;
            learnerData.standardCode = apprenticeship.TrainingDetails.StandardCode;
            learnerData.consumerReference = apprenticeship.TrainingDetails.ConsumerReference;

            await Task.Delay(100);
            return learnerData;

        }


        private async Task<string?> GetLearnerKeyByUlnAsync(string resource, string targetUln)
        {
            int page = 1;
            const int pageSize = 100;

            restResponse = await learnerDataOuterApiClient.GetLearners($"{resource}?page={page}&pageSize={pageSize}");
            var content = JsonSerializer.Deserialize<LearnerResponse>(restResponse.Content!);
            var totalPages = content?.TotalPages ?? 1;

            for (int i = totalPages; i > 0; i--)
            {
                var url = $"{resource}?page={i}&pageSize={pageSize}";
                restResponse = await learnerDataOuterApiClient.GetLearners(url);
                content = JsonSerializer.Deserialize<LearnerResponse>(restResponse.Content!);

                if (content?.Learners != null)
                {
                    var match = content.Learners.FirstOrDefault(l => l.Uln == targetUln);
                    if (match != null)
                    {
                        objectContext.SetDebugInformation($"ULN {targetUln} found on page# {i} with key:[{match.Key}]");
                        return match.Key;
                    }
                    else
                    {
                        objectContext.SetDebugInformation($"ULN {targetUln} not found on page# {i}.");
                    }
                }
            }

            return null; // ULN not found


        }


    }

    public class LearnerDataAPIDataModel
    {
        public string uln { get; set; }
        public string ukprn { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string learnerEmail { get; set; }
        public string dateOfBirth { get; set; }
        public string startDate { get; set; }
        public string plannedEndDate { get; set; }
        public int percentageLearningToBeDelivered { get; set; }
        public int epaoPrice { get; set; }
        public int trainingPrice { get; set; }
        public string agreementId { get; set; }
        public bool isFlexiJob { get; set; }
        public int plannedOTJTrainingHours { get; set; }
        public int standardCode { get; set; }
        public string consumerReference { get; set; }


    }

    internal class LearnerResponse
    {
        [JsonPropertyName("learners")]
        public List<Learner> Learners { get; set; } = new();

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }
    }

    internal class Learner
    {
        [JsonPropertyName("uln")]
        public string Uln { get; set; } = string.Empty;

        [JsonPropertyName("key")]
        public string Key { get; set; } = string.Empty;
    }



}
