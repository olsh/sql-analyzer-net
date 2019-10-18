using System.Data.SqlClient;
using System.Threading.Tasks;

using Dapper;

namespace Sql.Analyzer.Test.TestData
{
    public class InlineSqlWithDbStringArgumentTestData
    {
        private static async Task Main(string[] args)
        {
            var sql = new SqlConnection();
            sql.Execute("inline sql @param", new { param = new DbString() { IsAnsi = true, Value = "some_string" } });
        }
    }
}
