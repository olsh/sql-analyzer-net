using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SqlAnalyzer.Net.Test.Helpers;

namespace SqlAnalyzer.Net.Test
{
    [TestClass]
    public class DapperStringParameterAnalyzerTests : DiagnosticVerifier
    {
        protected override string TestDataFolder => "DapperStringParameterAnalyzer";

        [TestMethod]
        public void Empty_NotTriggered()
        {
            var test = string.Empty;

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void InlineSqlWithoutStringArgument_NotTriggered()
        {
            var code = ReadTestData("InlineSqlWithDbStringArgument.cs");

            VerifyCSharpDiagnostic(code);
        }

        [TestMethod]
        public void InlineSqlWithStringArgument_AnalyzerTriggered()
        {
            var code = ReadTestData("InlineSqlWithStringArgument.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = DapperStringParameterAnalyzer.DiagnosticId,
                                   Message = string.Format(DapperStringParameterAnalyzer.MessageFormat, "id"),
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 13, 46) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void InlineSqlQueryMultipleWithStringArgument_AnalyzerTriggered()
        {
            var code = ReadTestData("InlineSqlQueryMultipleWithStringArgument.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = DapperStringParameterAnalyzer.DiagnosticId,
                                   Message = string.Format(DapperStringParameterAnalyzer.MessageFormat, "id"),
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 13, 52) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void StoredProcedureWithStringArgument_NotTriggered()
        {
            var code = ReadTestData("StoredProcedureWithStringArgument.cs");

            VerifyCSharpDiagnostic(code);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DapperStringParameterAnalyzer();
        }
    }
}
