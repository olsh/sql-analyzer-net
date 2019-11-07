using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sql.Analyzer.Test.TestData
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var sql = new SqlCommand("select * from test where @a = 1");
            sql.CommandType = CommandType.Text;
            sql.Parameters.Add("@a", SqlDbType.VarChar).Value = "value";
            sql.Parameters.Add("@b", SqlDbType.VarChar).Value = "value";
            sql.ExecuteNonQuery();
        }
    }
}
