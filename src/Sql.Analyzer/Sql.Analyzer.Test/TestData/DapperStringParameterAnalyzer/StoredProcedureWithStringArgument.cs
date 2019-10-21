using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

using Dapper;

namespace Sql.Analyzer.Test.TestData
{
    public class InlineSqlWithStringParameterTestData
    {
        private static async Task Main(string[] args)
        {
            var sql = new SqlConnection();
            sql.Execute("inline sql @param", new { param = "some_string" }, commandType: CommandType.StoredProcedure);
        }
    }
}
