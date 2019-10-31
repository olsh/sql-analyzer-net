using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SqlAnalyzer.Net.Parsers;

namespace SqlAnalyzer.Net.Test.Parsers
{
    [TestClass]
    public class SqlParserTests
    {
        [TestMethod]
        public void FindSqlVariables_DeclareVariable()
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
        public void FindSqlVariables_Identity()
        {
            var sql = 
                @"SELECT id, @@IDENTITY 
                FROM dbo.table
                WHERE name = @name;";

            var parameters = SqlParser.FindParameters(sql);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual("name", parameters.First());
        }

        [TestMethod]
        public void FindSqlVariables_IfStatement()
        {
            var sql =
                @"declare @result tinyint = 0;

                    IF @ip = 'unknown'
                        SET @result = 0;
                    ELSE
                        SET @result = 1;

                    select @result;";

            var parameters = SqlParser.FindParameters(sql);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual("ip", parameters.First());
        }

        [TestMethod]
        public void FindSqlVariables_MultipleDeclaration()
        {
            var sql =
                @"DECLARE  @Planner1 VARCHAR(50) = '2566927',
                 @Planner2 varchar(10) = '12201704',
                 @OtherVar int = 42
                 SELECT * FROM table";

            var parameters = SqlParser.FindParameters(sql);

            Assert.AreEqual(0, parameters.Count);
        }

        [TestMethod]
        public void FindSqlVariables_ExecuteStoreProcedure()
        {
            var sql = @"EXEC sp_api_user_GetServicesSchedule @id = @uid, @adminID = @aid";

            var parameters = SqlParser.FindParameters(sql);

            Assert.AreEqual(2, parameters.Count);
            Assert.IsTrue(parameters.Contains("uid"));
            Assert.IsTrue(parameters.Contains("aid"));
        }

        [TestMethod]
        public void FindSqlVariables_DeclarationWithKeyWord()
        {
            var sql =
                @"DECLARE  @month_difference VARCHAR(50) = '2566927'
                 SELECT * FROM table WHERE a = @month_difference";

            var parameters = SqlParser.FindParameters(sql);

            Assert.AreEqual(0, parameters.Count);
        }

        [TestMethod]
        public void FindSqlVariables_SingleLineComment()
        {
            var sql =
                @"SELECT * FROM table 
                WHERE -- a = @month_difference AND 
                @z = 1";

            var parameters = SqlParser.FindParameters(sql);

            Assert.AreEqual(1, parameters.Count);
            Assert.IsTrue(parameters.Contains("z"));
        }

        [TestMethod]
        public void FindSqlVariables_MultiLineComment()
        {
            var sql =
                @"SELECT * FROM table 
                WHERE /*a = @month_difference AND 
                @z = 1  AND */ @b = 4
                -- @c = 2";

            var parameters = SqlParser.FindParameters(sql);

            Assert.AreEqual(1, parameters.Count);
            Assert.IsTrue(parameters.Contains("b"));
        }
    }
}
