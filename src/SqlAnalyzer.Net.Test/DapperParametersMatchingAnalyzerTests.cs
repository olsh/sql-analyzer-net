using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SqlAnalyzer.Net.Test.Helpers;

namespace SqlAnalyzer.Net.Test
{
    [TestClass]
    public class DapperParametersMatchingAnalyzerTests : DiagnosticVerifier
    {
        protected override string TestDataFolder => "DapperParametersMatchingAnalyzer";

        [TestMethod]
        public void InlineSqlDeclaredVariableParameters_NotTriggered()
        {
            var code = ReadTestData("InlineSqlDeclaredVariableParameters.cs");

            VerifyCSharpDiagnostic(code);
        }

        [TestMethod]
        public void InlineSqlUnmatchedParameters_AnalyzerTriggered()
        {
            var code = ReadTestData("InlineSqlUnmatchedParameters.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = DapperParametersMatchingAnalyzer.DiagnosticId,
                                   Message = string.Format(
                                       DapperParametersMatchingAnalyzer.MessageFormatCsharpArgumentNotFound,
                                       "not_found"),
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 13, 13) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void InlineSqlUnmatchedSqlVariable_AnalyzerTriggered()
        {
            var code = ReadTestData("InlineSqlUnmatchedSqlVariable.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = DapperParametersMatchingAnalyzer.DiagnosticId,
                                   Message = string.Format(
                                       DapperParametersMatchingAnalyzer.MessageFormatSqlVariableNotFound,
                                       "param"),
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 13, 13) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void InlineSqlWithVariableUnmatchedParameters_AnalyzerTriggered()
        {
            var code = ReadTestData("InlineSqlWithConstUnmatchedParameters.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = DapperParametersMatchingAnalyzer.DiagnosticId,
                                   Message = string.Format(
                                       DapperParametersMatchingAnalyzer.MessageFormatCsharpArgumentNotFound,
                                       "not_found"),
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 14, 13) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void InlineSqlDynamicParameters_AnalyzerTriggered()
        {
            var code = ReadTestData("InlineSqlDynamicParameters.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = DapperParametersMatchingAnalyzer.DiagnosticId,
                                   Message = string.Format(
                                       DapperParametersMatchingAnalyzer.MessageFormatSqlVariableNotFound,
                                       "not_found"),
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 15, 13) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void InlineSqlDynamicParametersWithConstructor_AnalyzerNotTriggered()
        {
            var code = ReadTestData("InlineSqlDynamicParametersWithConstructor.cs");

            VerifyCSharpDiagnostic(code);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DapperParametersMatchingAnalyzer();
        }
    }
}
