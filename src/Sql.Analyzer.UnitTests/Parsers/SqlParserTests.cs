using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Sql.Analyzer.Parsers;

namespace Sql.Analyzer.UnitTests.Parsers
{
    [TestClass]
    public class SqlParserTests
    {
        [TestMethod]
        public void FindSqlVariables_DeclareVariable_NotDetected()
        {
            var sql = 
                @"DECLARE @ids udt_ids_list;

                INSERT INTO @ids
                SELECT id 
                FROM dbo.table
                WHERE name = @name;

                DECLARE @idsa udt_ids_list;

                INSERT INTO @idsa
                SELECT id 
                FROM dbo.table
                WHERE name = @name;";

            var parameters = SqlParser.FindParameters(sql);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual("name", parameters.First());
        }

        [TestMethod]
        public void FindSqlVariables_Identity_NotDetected()
        {
            var sql = 
                @"SELECT id, @@IDENTITY 
                FROM dbo.table
                WHERE name = @name;";

            var parameters = SqlParser.FindParameters(sql);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual("name", parameters.First());
        }
    }
}
