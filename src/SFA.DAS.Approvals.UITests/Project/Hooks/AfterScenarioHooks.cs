using SFA.DAS.Approvals.UITests.Project.Helpers;
using SFA.DAS.ProviderPortal.UITests.Project.Helpers;
using System;


namespace SFA.DAS.Approvals.UITests.Project.Hooks
{
    [Binding]
    public class AfterScenarioHooks
    {
        private readonly ScenarioContext _context;
        private readonly FeatureContext _featureContext;

        public AfterScenarioHooks(ScenarioContext context, FeatureContext featureContext)
        {
            _context = context;
            _featureContext = featureContext;
        }

        [AfterScenario(Order = 31)]
        public void SaveScenarioContextInFeatureContext()
        {
            if (_featureContext.FeatureInfo.Tags.Contains("linkedScenarios"))
            {
                _featureContext["ResultOfPreviousScenario"] = _context.ScenarioExecutionStatus;

                if (_featureContext.ContainsKey("ScenarioContextofPreviousScenario"))
                    _featureContext["ScenarioContextofPreviousScenario"] = _context;
                else
                    _featureContext.Add("ScenarioContextofPreviousScenario", _context);
            }
        }

        [AfterScenario(Order = 32)]
        [Scope(Tag = "providerpermissions")]
        public async Task ClearProviderRelatins()
        {
            await new DeleteProviderRelationinDbHelper(_context).DeleteProviderRelation();
        }


    }
}
