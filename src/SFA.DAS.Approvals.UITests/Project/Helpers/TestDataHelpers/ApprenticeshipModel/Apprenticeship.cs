using SFA.DAS.Approvals.UITests.Project.Helpers.TestDataHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Approvals.UITests.Project.Helpers.DataHelpers.ApprenticeshipModel
{
    public class Apprenticeship
    {
        public Apprenticeship()
        {
            Cohort = new Cohort();
            EmployerDetails = new Employer();
            ProviderDetails = new Provider();
            ApprenticeDetails = new Apprentice();
            TrainingDetails = new Training();
            RPLDetails = new RPL();
        }

        public Apprenticeship(Apprentice apprentice, Training training, RPL rpl)
        {
            Cohort = new Cohort();
            ApprenticeDetails = apprentice;
            TrainingDetails = training;
            RPLDetails = rpl;
        }

        public string ReservationID { get; set; }
        public Cohort Cohort { get; set; }
        public Employer EmployerDetails { get; set; }
        public Provider ProviderDetails { get; set; }
        public Apprentice ApprenticeDetails { get; set; }
        public Training TrainingDetails { get; set; }
        public RPL RPLDetails { get; set; }

    }

    public class Cohort
    {
        public string Reference { get; set; }
        public string Status_Provider { get; set; }
        public string Status_Employer { get; set; }
    }

    public class Employer
    {
        public string AgreementId { get; set; }
        public int AccountLegalEntityId { get; set; }
        public string EmployerName { get; set; }
        public EmployerType EmployerType { get; set; }
        public string Email { get; set; }
    }

    public class Provider
    {
        public string ProviderName { get; set; }
        public int Ukprn { get; set; }
        public string Email { get; set; }
    }


    public class Apprentice
    {
        public string ULN { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => FirstName + " " + LastName;
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int ApprenticeshipId { get; set; }
        public string LearningIdKey { get; set; }
        public int LearnerDataId { get; set; }    
    }

    public class Training
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int AcademicYear { get; set; }
        public int PercentageLearningToBeDelivered { get; set; }
        public int EpaoPrice { get; set; }
        public int TrainingPrice { get; set; }
        public int TotalPrice { get; set; }
        public bool IsFlexiJob { get; set; }
        public int PlannedOTJTrainingHours { get; set; }
        public int StandardCode { get; set; }
        public string ConsumerReference { get; set; }
    }

    public class RPL
    {
        public string RecognisePriorLearning { get; set; }
        public int TrainingTotalHours { get; set; }
        public int TrainingHoursReduction { get; set; }
        public bool IsDurationReducedByRPL { get; set; }
        public int DurationReducedBy { get; set; }
        public int PriceReducedBy { get; set; }
    }


}
