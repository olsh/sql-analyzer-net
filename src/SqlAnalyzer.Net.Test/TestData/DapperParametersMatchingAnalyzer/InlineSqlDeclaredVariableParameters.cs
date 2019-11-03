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
            sql.Execute(@"DECLARE @id VARCHAR(50);
                            select 1 FROM table WHERE b = @not_found", 
                new { not_found = new DbString() { IsAnsi = true, Value = "some_string" } });
        }
    }
}
