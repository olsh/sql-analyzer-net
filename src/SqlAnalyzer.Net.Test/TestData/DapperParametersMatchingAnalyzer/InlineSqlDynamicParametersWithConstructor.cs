using System.Data.SqlClient;
using System.Threading.Tasks;

using Dapper;

namespace Sql.Analyzer.Test.TestData
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var sql = new SqlConnection();
            var parameters = new DynamicParameters(new { a = 1 });
            sql.Execute(@"DECLARE @id VARCHAR(50);
                            select 1 FROM table WHERE @a = 1", parameters);
        }
    }
}
