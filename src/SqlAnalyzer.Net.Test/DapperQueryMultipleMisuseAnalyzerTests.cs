using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SqlAnalyzer.Net.Test.Helpers;

namespace SqlAnalyzer.Net.Test
{
    [TestClass]
    public class DapperQueryMultipleMisuseAnalyzerTests : DiagnosticVerifier
    {
        protected override string TestDataFolder => "DapperQueryMultipleMisuseAnalyzer";

        [TestMethod]
        public void QueryMultipleWithSingleRead_AnalyzerTriggered()
        {
            var code = ReadTestData("QueryMultipleWithSingleRead.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = DapperQueryMultipleMisuseAnalyzer.DiagnosticId,
                                   Message = string.Format(DapperQueryMultipleMisuseAnalyzer.MessageFormat),
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 14, 25) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void QueryMultipleWithMultipleReads_AnalyzerNotTriggered()
        {
            var code = ReadTestData("QueryMultipleWithMultipleReads.cs");

            VerifyCSharpDiagnostic(code);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DapperQueryMultipleMisuseAnalyzer();
        }
    }
}
