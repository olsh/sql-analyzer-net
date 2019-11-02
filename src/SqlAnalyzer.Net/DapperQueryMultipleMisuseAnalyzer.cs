using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FindSymbols;

using SqlAnalyzer.Net.Extensions;
using SqlAnalyzer.Net.Walkers;

namespace SqlAnalyzer.Net
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DapperQueryMultipleMisuseAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SQL004";

        public const string MessageFormat = "Consider using 'Query' method here";

        private const string Category = "API Guidance";

        private static readonly string Description = "The 'QueryMultiple' method is designed to read multiple sets. For a single set it's better to use 'Query' command.";

        private static readonly string Title = "Using 'QueryMultiple' method is not optimal here";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            true,
            Description,
            "https://github.com/olsh/sql-analyzer-net#sql003-using-query-method-is-not-optimal-here");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpressionSyntax = (InvocationExpressionSyntax)context.Node;

            if (!(context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol is IMethodSymbol methodSymbol))
            {
                return;
            }

            var dapperClass = context.SemanticModel.GetDapperSqlMapperSymbol();
            const string QueryPrefix = "QueryMultiple";
            if (methodSymbol.ContainingType != dapperClass || !methodSymbol.Name.StartsWith(QueryPrefix))
            {
                return;
            }

            var declarationStatementSyntax = invocationExpressionSyntax
                .AncestorsAndSelf()
                .OfType<VariableDeclarationSyntax>()
                .FirstOrDefault();
            if (declarationStatementSyntax == null)
            {
                return;
            }

            if (declarationStatementSyntax.Variables.Count != 1)
            {
                return;
            }

            var multiVariable = context.SemanticModel.GetDeclaredSymbol(declarationStatementSyntax.Variables.Single());

            var methodDeclaration = invocationExpressionSyntax
                .AncestorsAndSelf()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault();
            if (methodDeclaration == null)
            {
                return;
            }

            var localMemberAccessExpressionWalker = new LocalMemberAccessExpressionWalker(multiVariable.Name);
            localMemberAccessExpressionWalker.Visit(methodDeclaration.Body);

            if (localMemberAccessExpressionWalker.MethodCalls.Count(m => m.StartsWith("Read")) < 2)
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, invocationExpressionSyntax.Expression.GetLocation()));
            }
        }
    }
}
