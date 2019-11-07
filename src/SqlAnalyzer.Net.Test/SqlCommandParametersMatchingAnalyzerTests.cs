using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SqlAnalyzer.Net.Rules;
using SqlAnalyzer.Net.Test.Helpers;

namespace SqlAnalyzer.Net.Test
{
    [TestClass]
    public class SqlCommandParametersMatchingAnalyzerTests : DiagnosticVerifier
    {
        protected override string TestDataFolder => "SqlCommandParametersMatchingAnalyzer";

        [TestMethod]
        public void InlineSqlUnmatchedParameters_AnalyzerTriggered()
        {
            var code = ReadTestData("InlineSqlUnmatchedParameters.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = ParametersMatchingRule.DiagnosticId,
                                   Message = string.Format(
                                       ParametersMatchingRule.MessageFormatSqlVariableNotFound,
                                       "b"),
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 15, 13) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void InlineSqlPropertyUnmatchedParameters_AnalyzerTriggered()
        {
            var code = ReadTestData("InlineSqlPropertyUnmatchedParameters.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = ParametersMatchingRule.DiagnosticId,
                                   Message = string.Format(
                                       ParametersMatchingRule.MessageFormatSqlVariableNotFound,
                                       "b"),
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 16, 13) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void InlineSqlAddWithValueUnmatchedParameters_AnalyzerTriggered()
        {
            var code = ReadTestData("InlineSqlAddWithValueUnmatchedParameters.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = ParametersMatchingRule.DiagnosticId,
                                   Message = string.Format(
                                       ParametersMatchingRule.MessageFormatSqlVariableNotFound,
                                       "b"),
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 15, 13) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SqlCommandParametersMatchingAnalyzer();
        }
    }
}
