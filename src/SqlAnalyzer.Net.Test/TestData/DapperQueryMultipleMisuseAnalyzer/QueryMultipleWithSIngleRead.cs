using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

namespace Sql.Analyzer.Test.TestData
{
    public class QueryWithFirstOrDefault
    {
        private static async Task Main(string[] args)
        {
            var sql = new SqlConnection();
            var multi = sql.QueryMultiple("sql");
            multi.Read();
        }
    }
}
