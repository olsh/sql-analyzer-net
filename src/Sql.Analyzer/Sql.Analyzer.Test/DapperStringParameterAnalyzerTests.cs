using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Sql.Analyzer.Test.Helpers;

using TestHelper;

namespace Sql.Analyzer.Test
{
    [TestClass]
    public class DapperStringParameterAnalyzerTests : DiagnosticVerifier
    {
        [TestMethod]
        public void Empty_NotTriggered()
        {
            var test = string.Empty;

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void InlineSqlWithStringArgument_AnalyzerTriggered()
        {
            var code = EmbeddedResourceHelper.ReadTestData("InlineSqlWithStringArgumentTestData.cs");

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
        public void InlineSqlWithoutStringArgument_NotTriggered()
        {
            var code = EmbeddedResourceHelper.ReadTestData("InlineSqlWithDbStringArgumentTestData.cs");

            VerifyCSharpDiagnostic(code);
        }

        [TestMethod]
        public void StoredProcedureWithStringArgument_NotTriggered()
        {
            var code = EmbeddedResourceHelper.ReadTestData("StoredProcedureWithStringArgumentTestData.cs");

            VerifyCSharpDiagnostic(code);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DapperStringParameterAnalyzer();
        }
    }
}
