using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sql.Analyzer.UnitTests
{
    [TestClass]
    public class DapperParametersMatchingAnalyzerTests
    {
        [TestMethod]
        public void FindSqlVariables_DeclareVariable_NotDetected()
        {
            var sql = @"DECLARE @ids udt_ids_list;

                INSERT INTO @ids
                SELECT id 
                FROM dbo.table
                WHERE name = @name;";
        }
    }
}
