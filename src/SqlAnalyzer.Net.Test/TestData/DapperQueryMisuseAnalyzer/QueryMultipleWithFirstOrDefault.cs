using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

namespace Sql.Analyzer.Test.TestData
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var sql = new SqlConnection();
            using (var multi = await sql.QueryMultipleAsync("sql"))
            {
                multi.Read<int>().FirstOrDefault();
                await multi.ReadAsync<int>().ConfigureAwait(false);
            }
        }
    }
}
