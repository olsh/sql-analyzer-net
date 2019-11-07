using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sql.Analyzer.Test.TestData
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var sql = new SqlCommand("select * from test where @a = 1 AND @b = 2");
            sql.CommandType = CommandType.Text;
            sql.Parameters["@a"] = "value";
            sql.Parameters.Add("@b", SqlDbType.VarChar).Value = "value";
            sql.CommandText = "select * from test where @a = 1";
            sql.ExecuteNonQuery();
        }
    }
}
