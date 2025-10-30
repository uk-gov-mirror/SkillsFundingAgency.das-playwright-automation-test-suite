using Polly;
using SFA.DAS.DfeAdmin.Service.Project.Helpers.DfeSign.User;
using SFA.DAS.Login.Service.Project.Helpers;
using SFA.DAS.QFAST.UITests.Project.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SFA.DAS.QFAST.UITests.Project.Tests.Steps
{
    [Binding]
    public class LoginSteps(ScenarioContext context)
    {
       private readonly QfastHelpers _qfastHelpers = new(context);

        [Given("the admin user log in to the portal")]
        public async Task GivenTheAdminUserLogInToThePortal()
        {
            await _qfastHelpers.GoToQfastAdminHomePage();


        }
    }
}
