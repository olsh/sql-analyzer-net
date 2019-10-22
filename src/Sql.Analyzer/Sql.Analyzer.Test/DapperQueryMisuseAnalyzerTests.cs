using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TestHelper;

namespace Sql.Analyzer.Test
{
    [TestClass]
    public class DapperQueryMisuseAnalyzerTests : DiagnosticVerifier
    {
        protected override string TestDataFolder => "DapperQueryMisuseAnalyzer";

        [TestMethod]
        public void QueryWithFirstOrDefault_AnalyzerTriggered()
        {
            var code = ReadTestData("QueryWithFirstOrDefault.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = DapperQueryMisuseAnalyzer.DiagnosticId,
                                   Message = string.Format(
                                       DapperQueryMisuseAnalyzer.MessageFormat,
                                       "QueryFirstOrDefault"),
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 14, 26) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void QueryWithSingleAsync_AnalyzerTriggered()
        {
            var code = ReadTestData("QueryWithSingleAsync.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = DapperQueryMisuseAnalyzer.DiagnosticId,
                                   Message = string.Format(
                                       DapperQueryMisuseAnalyzer.MessageFormat,
                                       "QuerySingleAsync"),
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 14, 33) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DapperQueryMisuseAnalyzer();
        }
    }
}
