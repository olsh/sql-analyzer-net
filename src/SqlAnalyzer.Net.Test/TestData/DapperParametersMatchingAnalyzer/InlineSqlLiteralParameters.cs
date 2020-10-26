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
            sql.Execute(@"DECLARE 1 VARCHAR(50);
                            select 1 FROM table WHERE b = {=found} OR {=not_found}", 
                new { found = new DbString() { IsAnsi = true, Value = "some_string" } });
        }
    }
}
