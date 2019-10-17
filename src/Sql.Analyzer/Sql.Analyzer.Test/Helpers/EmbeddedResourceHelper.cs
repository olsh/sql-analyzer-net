using System.IO;
using System.Reflection;

namespace Sql.Analyzer.Test.Helpers
{
    public class EmbeddedResourceHelper
    {
        public static string ReadTestData(string testDataFileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"Sql.Analyzer.Test.TestData.{testDataFileName}";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
