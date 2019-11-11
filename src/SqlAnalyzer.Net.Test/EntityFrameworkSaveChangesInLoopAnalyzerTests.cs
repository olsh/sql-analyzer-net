using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SqlAnalyzer.Net.Test.Helpers;

namespace SqlAnalyzer.Net.Test
{
    [TestClass]
    public class EntityFrameworkSaveChangesInLoopAnalyzerTests : DiagnosticVerifier
    {
        protected override string TestDataFolder => "EntityFrameworkSaveChangesInLoopAnalyzer";

        [TestMethod]
        public void SaveChangesInForLoop_AnalyzerTriggered()
        {
            var code = ReadTestData("SaveChangesInForLoop.cs");

            var expected = new DiagnosticResult
                               {
                                   Id = EntityFrameworkSaveChangesInLoopAnalyzer.DiagnosticId,
                                   Message = EntityFrameworkSaveChangesInLoopAnalyzer.MessageFormat,
                                   Severity = DiagnosticSeverity.Warning,
                                   Locations = new[] { new DiagnosticResultLocation("Test0.cs", 13, 23) }
                               };

            VerifyCSharpDiagnostic(code, expected);
        }

        [TestMethod]
        public void SaveChangesOutsideOfLoop_AnalyzerNotTriggered()
        {
            var code = ReadTestData("SaveChangesOutsideOfLoop.cs");

            VerifyCSharpDiagnostic(code);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new EntityFrameworkSaveChangesInLoopAnalyzer();
        }
    }
}
