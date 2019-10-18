using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Sql.Analyzer.Test.Helpers;

using TestHelper;

namespace Sql.Analyzer.Test
{
    [TestClass]
    public class DapperParametersMatchingAnalyzerTests : DiagnosticVerifier
    {
        [TestMethod]
        public void InlineSqlUnmatchedParameters_AnalyzerTriggered()
        {
            var code = EmbeddedResourceHelper.ReadTestData("InlineSqlUnmatchedParameters.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = DapperParametersMatchingAnalyzer.DiagnosticId,
                                   Message = string.Format(DapperParametersMatchingAnalyzer.MessageFormatCsharpArgumentNotFound, "not_found"),
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 13, 13) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void InlineSqlWithVariableUnmatchedParameters_AnalyzerTriggered()
        {
            var code = EmbeddedResourceHelper.ReadTestData("InlineSqlWithConstUnmatchedParameters.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = DapperParametersMatchingAnalyzer.DiagnosticId,
                                   Message = string.Format(DapperParametersMatchingAnalyzer.MessageFormatCsharpArgumentNotFound, "not_found"),
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 14, 13) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }


        [TestMethod]
        public void InlineSqlUnmatchedSqlVariable_AnalyzerTriggered()
        {
            var code = EmbeddedResourceHelper.ReadTestData("InlineSqlUnmatchedSqlVariable.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = DapperParametersMatchingAnalyzer.DiagnosticId,
                                   Message = string.Format(DapperParametersMatchingAnalyzer.MessageFormatSqlVariableNotFound, "param"),
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 13, 13) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void InlineSqlDeclaredVariableParameters_NotTriggered()
        {
            var code = EmbeddedResourceHelper.ReadTestData("InlineSqlDeclaredVariableParameters.cs");

            VerifyCSharpDiagnostic(code);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DapperParametersMatchingAnalyzer();
        }
    }
}
