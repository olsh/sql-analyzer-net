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
            var entity = sql.Query<dynamic>("some sql").FirstOrDefault();
        }
    }
}
