using Polly;
using SFA.DAS.Approvals.UITests.Project.Helpers.TestDataHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Approvals.UITests.Project.Helpers.DataHelpers.ApprenticeshipModel
{
    internal class TrainingFactory : ITrainingFactory
    {
        private readonly Func<CoursesDataHelper, Task<Courses>> courseSelector;

        public TrainingFactory(Func<CoursesDataHelper, Task<Courses>> CourseSelector = null)
        {
            courseSelector = CourseSelector;

        }

        public async Task<Training> CreateTrainingAsync(EmployerType employerType, ApprenticeshipStatus? apprenticeshipStatus = null)
        {
            Training training = new Training();

            CoursesDataHelper coursesDataHelper = new CoursesDataHelper();
            var course = courseSelector == null ? await coursesDataHelper.GetRandomCourse() : await courseSelector(coursesDataHelper);

            if (course.ApprenticeshipType == "FoundationApprenticeship")
            {
                training.StartDate = course.EffectiveFrom;
            }
            else if (employerType == EmployerType.Levy)
            {
                training.StartDate = await GetStartDate(apprenticeshipStatus);
            }
            else
            {
                training.StartDate = DateTime.Now;
            }

            //training.StartDate = (employerType == EmployerType.Levy) ? await GetStartDate(apprenticeshipStatus) : DateTime.Now;
            training.EndDate = training.StartDate.AddMonths(15);
            training.AcademicYear = AcademicYearDatesHelper.GetCurrentAcademicYear();
            training.PercentageLearningToBeDelivered = 40;
            training.EpaoPrice = Convert.ToInt32(RandomDataGenerator.GenerateRandomNumber(3));
            training.TrainingPrice = Convert.ToInt32("2" + RandomDataGenerator.GenerateRandomNumber(3));
            training.TotalPrice = training.EpaoPrice + training.TrainingPrice;
            training.IsFlexiJob = false;
            training.PlannedOTJTrainingHours = 1200;
            training.StandardCode = course.StandardCode;
            training.ConsumerReference = "CR123456";

            await Task.Delay(100);

            return training;
        }

        private async Task<DateTime> GetStartDate(ApprenticeshipStatus? apprenticeshipStatus = null)
        {
            var lowerDateRangeForStartDate = AcademicYearDatesHelper.GetCurrentAcademicYearStartDate();
            var academicYearEndDate = AcademicYearDatesHelper.GetCurrentAcademicYearEndDate();
            var todaysDate = DateTime.Now;
            var upperDateRangeForStartDate = academicYearEndDate > todaysDate ? todaysDate : academicYearEndDate;

            await Task.Delay(100);


            if (apprenticeshipStatus == ApprenticeshipStatus.WaitingToStart)
            {
                return RandomDataGenerator.GenerateRandomDate(DateTime.Now, DateTime.Now.AddMonths(2));
            }
            else
            {
                return RandomDataGenerator.GenerateRandomDate(lowerDateRangeForStartDate, upperDateRangeForStartDate);
            }

        }

    }
}
