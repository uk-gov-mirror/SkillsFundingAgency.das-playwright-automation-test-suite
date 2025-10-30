using SFA.DAS.Framework.Hooks;


namespace SFA.DAS.QFAST.UITests.Project;

[Binding]
public class Hooks(ScenarioContext context) : FrameworkBaseHooks(context)
{
    private readonly DbConfig _dbConfig = context.Get<DbConfig>();

    private readonly ObjectContext _objectContext = context.Get<ObjectContext>();

    [BeforeScenario(Order = 22)]
    public async Task Navigate() => await Navigate(UrlConfig.QFAST_BaseUrl);
}