using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using SqlAnalyzer.Net.Extensions;

namespace SqlAnalyzer.Net
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EntityFrameworkSaveChangesInLoopAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SQL005";

        public const string MessageFormat = "Call SaveChanges outside the loop";

        private const string Category = "API Guidance";

        private static readonly string Description = "Using 'SaveChanges' method in a loop can affect performance";

        private static readonly string Title = "Using 'SaveChanges' method in a loop can affect performance";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            true,
            Description,
            "https://github.com/olsh/sql-analyzer-net/rules/SQL005.md");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpressionSyntax = (InvocationExpressionSyntax)context.Node;

            if (!invocationExpressionSyntax.IsEntityFrameworkSaveChangesMethod(context.SemanticModel))
            {
                return;
            }

            var parentNodes = invocationExpressionSyntax.Ancestors();

            var inLoop = parentNodes.Any(n => n is ForStatementSyntax || n is ForEachStatementSyntax);
            if (inLoop)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, invocationExpressionSyntax.GetLocation(), MessageFormat));
            }
        }
    }
}
