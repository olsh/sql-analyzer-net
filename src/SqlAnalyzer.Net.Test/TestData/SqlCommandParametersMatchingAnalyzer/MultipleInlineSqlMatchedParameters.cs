using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sql.Analyzer.Test.TestData
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            using (var sql = new SqlCommand("select * from test where @a = 1"))
            {
                sql.CommandType = CommandType.Text;
                sql.Parameters.Add("@a", SqlDbType.VarChar).Value = "value";
                sql.ExecuteNonQuery();
            }

            using (var sql = new SqlCommand("select * from test where @b = 1"))
            {
                sql.CommandType = CommandType.Text;
                sql.Parameters.AddWithValue("@b", "1");
                sql.ExecuteNonQuery();
            }
        }
    }
}
