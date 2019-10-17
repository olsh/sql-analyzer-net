using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using Sql.Analyzer;
using Sql.Analyzer.Test.Helpers;

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
        public void AwaitStringGetAsync_AnalyzerTriggered()
        {
            var code = EmbeddedResourceHelper.ReadTestData("InlineSqlWithStringParameterTestData.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = DapperStringParameterAnalyzer.DiagnosticId,
                                   Message = string.Format(DapperStringParameterAnalyzer.MessageFormat, "StringGetAsync"),
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 12, 25) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new DapperStringParameterAnalyzer();
        }
    }
}
