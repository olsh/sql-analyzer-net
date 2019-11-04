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
            var entity = (await sql.QueryAsync<int, int, int>("some sql", (i, i1) => i + i1)).Single();
        }
    }
}
