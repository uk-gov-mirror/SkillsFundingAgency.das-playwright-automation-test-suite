using System;

namespace SFA.DAS.Approvals.UITests.Project.Helpers.TestDataHelpers;

internal class CoursesDataHelper()
{
    internal async Task<Courses> GetRandomCourse()
    {
        var courses = new List<Courses>
        {
            new() {
                StandardCode = 274,
                Title = "Abattoir worker",
                MaxFunding = 6000,
                EffectiveFrom = new DateTime(2018, 05, 08),
                Version = "1.1"
            },
            new() {
                StandardCode = 297,
                Title = "Teaching assistant",
                MaxFunding = 7000,
                EffectiveFrom = new DateTime(2018, 06, 26),
                Version = "1.1"
            },
            new() {
                StandardCode = 567,
                Title = "Florist",
                MaxFunding = 18000,
                EffectiveFrom = new DateTime(2020, 06, 10),
                Version = "1.0"
            },
            new() {
                StandardCode = 101,
                Title = "Retailer",
                MaxFunding = 5000,
                EffectiveFrom = new DateTime(2015, 08, 01),
                Version = "1.2"
            },
            new() {
                StandardCode = 2,
                Title = "Software developer",
                MaxFunding = 18000,
                EffectiveFrom = new DateTime(2014, 08, 01),
                Version = "1.1"
            },
            new() {
                StandardCode = 530,
                Title = "Fire safety inspector",
                MaxFunding = 11000,
                EffectiveFrom = new DateTime(2019, 11, 20),
                Version = "1.1"
            },
            new() {
                StandardCode = 409,
                Title = "Registered nurse degree",
                MaxFunding = 26000,
                EffectiveFrom = new DateTime(2019, 02, 13),
                Version = "1.1"
            }
        };

        await Task.Delay(100);
        var random = new Random();
        return courses[random.Next(courses.Count)];
    }

    internal async Task<Courses> GetRandomFoundationCourses()
    {
        var courses = new List<Courses>
        {
            new() {
                StandardCode = 805,
                Title = "Building service engineering foundation apprenticeship",
                MaxFunding = 4000,
                EffectiveFrom = new DateTime(2025, 08, 01),
                ApprenticeshipType = "FoundationApprenticeship",
                Version = "1.0"
            },
            new() {
                StandardCode = 806,
                Title = "Finishing trades foundation apprenticeship",
                MaxFunding = 4000,
                EffectiveFrom = new DateTime(2025, 08, 01),
                ApprenticeshipType = "FoundationApprenticeship",
                Version = "1.0"
            },
            new() {
                StandardCode = 807,
                Title = "Onsite trades foundation apprenticeship",
                MaxFunding = 4000,
                EffectiveFrom = new DateTime(2025, 08, 01),
                ApprenticeshipType = "FoundationApprenticeship",
                Version = "1.0"
            },
            new() {
                StandardCode = 808,
                Title = "Hardware, network and infrastructure foundation apprenticeship",
                MaxFunding = 4000,
                EffectiveFrom = new DateTime(2025, 08, 01),
                ApprenticeshipType = "FoundationApprenticeship",
                Version = "1.0"
            },
            new() {
                StandardCode = 809,
                Title = "Software and data foundation apprenticeship",
                MaxFunding = 4000,
                EffectiveFrom = new DateTime(2025, 08, 01),
                ApprenticeshipType = "FoundationApprenticeship",
                Version = "1.0"
            },
            new() {
                StandardCode = 810,
                Title = "Engineering and manufacturing foundation apprenticeship",
                MaxFunding = 4500,
                EffectiveFrom = new DateTime(2025, 08, 01),
                ApprenticeshipType = "FoundationApprenticeship",
                Version = "1.0"
            },
            new() {
                StandardCode = 811,
                Title = "Health and social care foundation apprenticeship",
                MaxFunding = 3000,
                EffectiveFrom = new DateTime(2025, 08, 01),
                ApprenticeshipType = "FoundationApprenticeship",
                Version = "1.0"
            }
        };

        await Task.Delay(100);
        var random = new Random();
        return courses[random.Next(courses.Count)];
    }

    internal async Task<Courses> GetRandomTestCourseWithOptions()
    {
        var courses = new List<Courses>
        {
            new() {
                StandardCode = 117,
                Title = "Professional accounting or taxation technician",
                MaxFunding = 12000,
                EffectiveFrom = new DateTime(2015, 08, 01),
                Version = "1.1",
                Options = new List<string> { "Accounting", "Tax" }
            },
            new() {
                StandardCode = 87,
                Title = "Aviation ground operative",
                MaxFunding = 3000,
                EffectiveFrom = new DateTime(2015, 08, 01),
                Version = "1.0",
                Options = new List<string> { "Aircraft Handling", "Aircraft Movement", "Fire Fighter", "Flight Operations", "Passenger Services" }
            },
            new() {
                StandardCode = 57,
                Title = "Gas network craftsperson",
                MaxFunding = 27000,
                EffectiveFrom = new DateTime(2015, 08, 01),
                Version = "1.3",
                Options = new List<string> { "Emergency Response Craftsperson", "Network Pipelines Maintenance Craftsperson", "Network Maintenance Craftsperson (Pressure Management)", "Network Maintenance Craftsperson (Electrical & Instrumentation)" }
            }
        };

        await Task.Delay(100);
        var random = new Random();
        return courses[random.Next(courses.Count)];


    }

}

internal class Courses
{
    public int StandardCode { get; set; }
    public string Title { get; set; }
    public int MaxFunding { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; } = null;
    public string ApprenticeshipType { get; set; } = "Apprenticeship";
    public string Version { get; set; } = null;
    public List<string> Options { get; set; } = null;
}
