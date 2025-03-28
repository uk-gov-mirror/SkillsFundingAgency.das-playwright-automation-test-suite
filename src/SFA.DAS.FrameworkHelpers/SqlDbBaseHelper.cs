using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using static SFA.DAS.FrameworkHelpers.WaitConfigurationHelper;

namespace SFA.DAS.FrameworkHelpers;

public class SqlDbBaseHelper(ObjectContext objectContext, string connectionString)
{
    protected bool waitForResults = false;

    protected readonly string connectionString = connectionString;

    protected readonly ObjectContext objectContext = objectContext;

    #region ExecuteSqlCommand

    public async Task<int> ExecuteSqlCommand(string queryToExecute) => await ExecuteSqlCommand(queryToExecute, connectionString);

    public async Task<int> ExecuteSqlCommand(string queryToExecute, string connectionString) => await ExecuteSqlCommand(queryToExecute, connectionString, null);

    public async Task<int> ExecuteSqlCommand(string queryToExecute, Dictionary<string, string> parameters) => await ExecuteSqlCommand(queryToExecute, connectionString, parameters);

    public async Task<int> ExecuteSqlCommand(string queryToExecute, string connectionString, Dictionary<string, string> parameters)
    {
        string dbName = SqlDbConfigHelper.GetDbName(connectionString);

        SetDebugInformation($"ExecuteSqlCommand : {dbName}{Environment.NewLine}{queryToExecute}");

        try
        {
            using SqlConnection databaseConnection = await GetSqlConnection(connectionString);

            databaseConnection.Open();

            using SqlCommand command = new(queryToExecute, databaseConnection);
            if (parameters != null)
            {
                foreach (KeyValuePair<string, string> param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            command.CommandTimeout = 300;

            int noOfrowsaffected = await command.ExecuteNonQueryAsync();

            SetDebugInformation($"{noOfrowsaffected} rows affected in {dbName}");

            return noOfrowsaffected;
        }
        catch (Exception exception)
        {
            throw new Exception($"Exception occurred while executing SQL query:({dbName}){Environment.NewLine}{queryToExecute}{Environment.NewLine}Exception: " + exception);
        }
    }

    #endregion

    protected async Task<List<object[]>> GetListOfData(string queryToExecute)
    {
        var (data, _) = await GetListOfData(queryToExecute, connectionString, null);

        return data;
    }

    protected async Task<(List<object[]> data, int noOfColumns)> GetListOfData(string queryToExecute, string connectionString, Dictionary<string, string> parameters)
    {
        var result = await GetMultipleListOfData([queryToExecute], connectionString, parameters);

        return result.FirstOrDefault();
    }


    protected async Task<List<(List<object[]> data, int noOfColumns)>> GetMultipleListOfData(List<string> queryToExecute) =>
        await GetMultipleListOfData(queryToExecute, connectionString, null);

    private async Task<List<(List<object[]> data, int noOfColumns)>> GetMultipleListOfData(List<string> queryToExecute, string connectionString, Dictionary<string, string> parameters)
    {
        SetDebugInformation($"ReadDataFromDataBase : {SqlDbConfigHelper.GetDbName(connectionString)}{Environment.NewLine}{string.Join(Environment.NewLine, queryToExecute)}");

        try
        {
            using SqlConnection dbConnection = await GetSqlConnection(connectionString);

            using SqlCommand command = new(string.Join(string.Empty, queryToExecute), dbConnection);
            command.CommandType = CommandType.Text;

            if (parameters != null)
            {
                foreach (KeyValuePair<string, string> param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            }

            var result = await RetriveData(queryToExecute, dbConnection, command);

            if (waitForResults)
            {
                WaitHelper.WaitForIt(async () =>
                {
                    if (result.Any(x => x.data.Any(y => !string.IsNullOrEmpty(y?.ToString())))) return true;

                    result = await RetriveData(queryToExecute, dbConnection, command);

                    return false;
                }, $"{queryToExecute.FirstOrDefault()}{Environment.NewLine}{SqlDbConfigHelper.WriteDebugMessage(connectionString)}").Wait();
            }

            return result;
        }
        catch (Exception exception)
        {
            throw new Exception("Exception occurred while executing SQL query"
                + "\n Exception: " + exception);
        }
    }

    private static async Task<List<(List<object[]> data, int noOfColumns)>> RetriveData(List<string> queryToExecute, SqlConnection dbConnection, SqlCommand command)
    {
        List<(List<object[]>, int)> multiresult = [];

        dbConnection.Open();

        SqlDataReader dataReader = await command.ExecuteReaderAsync();

        foreach (var _ in queryToExecute)
        {
            List<object[]> result = [];
            int noOfColumns = dataReader.FieldCount;
            while (dataReader.Read())
            {
                object[] items = new object[noOfColumns];
                dataReader.GetValues(items);
                result.Add(items);
            }

            multiresult.Add((result, noOfColumns));
            dataReader.NextResult();
        }

        dbConnection.Close();

        return multiresult;
    }

    private static async Task<SqlConnection> GetSqlConnection(string connectionString) => await GetSqlConnectionHelper.GetSqlConnection(connectionString);

    private void SetDebugInformation(string x) => objectContext.SetDebugInformation(x);
}
