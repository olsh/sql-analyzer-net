using System.Data.Entity;
using System.Threading.Tasks;

namespace Sql.Analyzer.Test.TestData
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var db = new DbContext("test");
            for (int i = 0; i < 100; i++)
            {
            }

            await db.SaveChangesAsync();
        }
    }
}
