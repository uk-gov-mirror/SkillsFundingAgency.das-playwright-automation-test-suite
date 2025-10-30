namespace SFA.DAS.Approvals.UITests.Project.Hooks
{
    internal class AfterTestHooks
    {
        [AfterTestRun(Order = 4)]
        public async Task StopEndpoints()
        {
            if (GlobalTestContext.ServiceBus is not null)
            {
                await GlobalTestContext.ServiceBus.DisposeAsync();
                GlobalTestContext.ServiceBus = null;
            }
        }
    }
}
