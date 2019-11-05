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
            var parameters = new DynamicParameters();
            parameters.Add("not_found", "some_string");
            sql.Execute(@"DECLARE @id VARCHAR(50);
                            select 1 FROM table", parameters);
        }
    }
}
