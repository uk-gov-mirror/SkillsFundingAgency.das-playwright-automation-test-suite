namespace SFA.DAS.TestDataCleanup.Project.Helpers;

public class AllDbTestDataCleanUpHelper(ObjectContext objectContext, DbConfig dbConfig)
{
    private readonly List<string> usersdeleted = [];

    private readonly List<string> userswithconstraints = [];

    public async Task<(List<string>, List<string>)> CleanUpAllDbTestData(string email) => await CleanUpAllDbTestData([email]);

    public async Task<(List<string>, List<string>)> CleanUpAllDbTestData(List<string> email)
    {
        List<List<string>> userEmailListoflist = [];

        (var easAccDbSqlDataHelper, var userEmailListArray) = await GetUserEmailList(email);

        if (userEmailListArray.IsNoDataFound()) return (usersdeleted, userswithconstraints);

        var userEmailList = userEmailListArray.ListOfArrayToList(0);

        AddInUseEmails(userEmailList);

        int batchCount = 25;

        for (int i = 0; i < userEmailList.Count; i += batchCount) userEmailListoflist.Add(userEmailList.Skip(i).Take(batchCount).ToList());

        List<Task> tasks = [];

        foreach (var item in userEmailListoflist) tasks.Add(Task.Run(async () => await CleanUpTestData(easAccDbSqlDataHelper, item)));

        await Task.WhenAll(tasks);

        return (usersdeleted, userswithconstraints);
    }

    private async Task<(TestDataCleanUpEasAccDbSqlDataHelper, List<string[]>)> GetUserEmailList(List<string> email)
    {
        var easAccDbSqlDataHelper = new TestDataCleanUpEasAccDbSqlDataHelper(objectContext, dbConfig);

        return (easAccDbSqlDataHelper, await easAccDbSqlDataHelper.GetUserEmailList(email));
    }

    private static void AddInUseEmails(List<string> userEmailList) => TestDataCleanUpEmailsInUse.AddInUseEmails(userEmailList);

    private async Task CleanUpTestData(TestDataCleanUpEasAccDbSqlDataHelper easAccDbSqlDataHelper, List<string> userEmailList)
    {
        try
        {
            var accountIdsListArray = await easAccDbSqlDataHelper.GetAccountIds(userEmailList);

            var noOfRowsDeleted = await CleanUpUsersDbTestData(userEmailList);

            var appaccdbNameToTearDown = objectContext.GetDbNameToTearDown();

            if (appaccdbNameToTearDown.TryGetValue(CleanUpDbName.EasAppAccTestDataCleanUp, out HashSet<string> appaccemails))
            {
                noOfRowsDeleted += await CleanUpAppAccDbTestData([.. appaccemails]);
            }

            var accountidsTodelete = accountIdsListArray.ListOfArrayToList(0);

            if (!string.IsNullOrEmpty(accountidsTodelete[0])) noOfRowsDeleted += await CleanUpTestDataUsingAccountId(accountidsTodelete);

            noOfRowsDeleted += await CleanUpEasDbTestData(easAccDbSqlDataHelper, userEmailList);

            int x = userEmailList.Count;

            if (x < 15) usersdeleted.Add($"{userEmailList.ToString(",")}");
            if (accountidsTodelete.Count < 15) usersdeleted.Add($"{accountidsTodelete.ToString(",")}");
            usersdeleted.Add($"{noOfRowsDeleted} total rows deleted across the dbs");
            usersdeleted.Add($"{userEmailList.Count} email account{(x == 1 ? string.Empty : "s")} deleted");
        }
        catch (Exception ex)
        {
            userswithconstraints.Add($"{userEmailList.ToString(",")}{Environment.NewLine}{ex.Message}");
        }
    }

    private async Task<int> CleanUpTestDataUsingAccountId(List<string> accountidsTodelete)
    {
        var apprenticeIds = await new GetSupportDataHelper(objectContext, dbConfig).GetApprenticeIds(accountidsTodelete);

        List<Task<int>> tasks = [];

        tasks.Add(Task.Run(async () => await CleanUpRsvrTestData(accountidsTodelete)));
        tasks.Add(Task.Run(async () => await CleanUpPrelTestData(accountidsTodelete)));
        tasks.Add(Task.Run(async () => await CleanUpPsrTestData(accountidsTodelete)));
        tasks.Add(Task.Run(async () => await CleanUpPfbeTestData(accountidsTodelete)));
        tasks.Add(Task.Run(async () => await CleanUpEmpFcastTestData(accountidsTodelete)));
        tasks.Add(Task.Run(async () => await CleanUpAppfbTestData(apprenticeIds)));
        tasks.Add(Task.Run(async () => await CleanUpEmpFinTestData(accountidsTodelete)));
        tasks.Add(Task.Run(async () => await CleanUpEmpIncTestData(accountidsTodelete)));
        tasks.Add(Task.Run(async () => await CleanUpAComtTestData(apprenticeIds)));
        tasks.Add(Task.Run(async () => await CleanUpEasLtmTestData(accountidsTodelete)));
        tasks.Add(Task.Run(async () => await CleanUpComtTestData(accountidsTodelete)));

        var result = await Task.WhenAll(tasks);

        return await Task.FromResult(result.Sum());
    }

    private async Task<int> CleanUpEasDbTestData(TestDataCleanUpEasAccDbSqlDataHelper helper, List<string> emailsToDelete)
    {
        return await SetDebugMessage(async () => await helper.CleanUpEasDbTestData(emailsToDelete), helper);
    }

    private async Task<int> CleanUpComtTestData(List<string> accountidsTodelete)
    {
        var helper = new TestDataCleanupComtSqlDataHelper(objectContext, dbConfig);

        return await SetDebugMessage(async () => await helper.CleanUpComtTestData(accountidsTodelete), helper);
    }

    private async Task<int> CleanUpEasLtmTestData(List<string> accountidsTodelete)
    {
        var helper = new TestDataCleanUpEasLtmcSqlDataHelper(objectContext, dbConfig);

        return await SetDebugMessage(async () => await helper.CleanUpEasLtmTestData(accountidsTodelete), helper);
    }

    private async Task<int> CleanUpAppfbTestData(List<string[]> apprenticeIds)
    {
        var helper = new TestDataCleanupAppfbqlDataHelper(objectContext, dbConfig);

        return await SetDebugMessage(async () => await helper.CleanUpAppfbTestData(apprenticeIds), helper);
    }

    private async Task<int> CleanUpAComtTestData(List<string[]> apprenticeIds)
    {
        var helper = new TestDataCleanupAComtSqlDataHelper(objectContext, dbConfig);

        return await SetDebugMessage(async () => await helper.CleanUpAComtTestData(apprenticeIds), helper);
    }

    private async Task<int> CleanUpAppAccDbTestData(List<string> emailsToDelete)
    {
        var helper = new TestDataCleanUpAppAccDbSqlDataHelper(objectContext, dbConfig);

        return await SetDebugMessage(async () => await helper.CleanUpAppAccDbTestData(emailsToDelete), helper);
    }

    private async Task<int> CleanUpEmpIncTestData(List<string> accountidsTodelete)
    {
        var helper = new TestDataCleanUpEmpIncSqlDataHelper(objectContext, dbConfig);

        return await SetDebugMessage(async () => await helper.CleanUpEmpIncTestData(accountidsTodelete), helper);
    }

    private async Task<int> CleanUpEmpFinTestData(List<string> accountidsTodelete)
    {
        var helper = new TestDataCleanUpEmpFinSqlDataHelper(objectContext, dbConfig);

        return await SetDebugMessage(async () => await helper.CleanUpEmpFinTestData(accountidsTodelete), helper);
    }

    private async Task<int> CleanUpEmpFcastTestData(List<string> accountidsTodelete)
    {
        var helper = new TestDataCleanUpEmpFcastSqlDataHelper(objectContext, dbConfig);

        return await SetDebugMessage(async () => await helper.CleanUpEmpFcastTestData(accountidsTodelete), helper);
    }

    private async Task<int> CleanUpPfbeTestData(List<string> accountidsTodelete)
    {
        var helper = new TestDataCleanUpPfbeDbSqlDataHelper(objectContext, dbConfig);

        return await SetDebugMessage(async () => await helper.CleanUpPfbeTestData(accountidsTodelete), helper);
    }

    private async Task<int> CleanUpPsrTestData(List<string> accountidsTodelete)
    {
        var helper = new TestDataCleanUpPsrDbSqlDataHelper(objectContext, dbConfig);

        return await SetDebugMessage(async () => await helper.CleanUpPsrTestData(accountidsTodelete), helper);
    }

    private async Task<int> CleanUpPrelTestData(List<string> accountidsTodelete)
    {
        var helper = new TestDataCleanUpPrelDbSqlDataHelper(objectContext, dbConfig);

        return await SetDebugMessage(async () => await helper.CleanUpPrelTestData(accountidsTodelete), helper);
    }

    private async Task<int> CleanUpRsvrTestData(List<string> accountidsTodelete)
    {
        var helper = new TestDataCleanUpRsvrSqlDataHelper(objectContext, dbConfig);

        return await SetDebugMessage(async () => await helper.CleanUpRsvrTestData(accountidsTodelete), helper);
    }

    private async Task<int> CleanUpUsersDbTestData(List<string> emailsToDelete)
    {
        var helper = new TestDataCleanUpUsersDbSqlDataHelper(objectContext, dbConfig);

        return await SetDebugMessage(async () => await helper.CleanUpUsersDbTestData(emailsToDelete), helper);
    }

    private async Task<int> SetDebugMessage(Func<Task<int>> func, TestDataCleanUpSqlDataHelper helper)
    {
        var dbName = helper.dbName;

        var sqlFileName = helper.SqlFileName;

        int noOfrowsDeleted;

        string message;

        try
        {
            noOfrowsDeleted = await func();

            message = $"{noOfrowsDeleted} rows deleted from {dbName}";
        }
        catch (Exception ex)
        {
            noOfrowsDeleted = 0;

            message = $"FAILED to delete from {dbName}({sqlFileName})";

            userswithconstraints.Add($"{dbName}({sqlFileName}){Environment.NewLine}{ex.Message}");
        }

        usersdeleted.Add(message);

        return noOfrowsDeleted;
    }
}
