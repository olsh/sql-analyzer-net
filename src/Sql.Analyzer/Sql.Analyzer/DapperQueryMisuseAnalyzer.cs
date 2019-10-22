using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using Sql.Analyzer.Extensions;

namespace Sql.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DapperQueryMisuseAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SQL003";

        public const string MessageFormat = "Consider using '{0}' method here";

        private const string Category = "API Guidance";

        private static readonly string Description = "The 'Query' method is designed to select a collection. For a single entity, there are most efficient alternatives like 'QueryFirst' or  'QuerySingle'.";

        private static readonly string Title = "Using 'Query' method is not optimal here";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            true,
            Description,
            "https://github.com/StackExchange/Dapper#performance");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpressionSyntax = (InvocationExpressionSyntax)context.Node;

            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol as IMethodSymbol;
            if (methodSymbol == null)
            {
                return;
            }

            var dapperClass = context.SemanticModel.GetDapperSqlMapperSymbol();
            const string QueryPrefix = "Query";
            if (methodSymbol.ContainingType != dapperClass || !methodSymbol.Name.StartsWith(QueryPrefix))
            {
                return;
            }

            var firstInvocationExpression = invocationExpressionSyntax
                    .Ancestors()
                    .TakeWhile(n => !(n is StatementSyntax))
                    .OfType<InvocationExpressionSyntax>()
                    .FirstOrDefault();

            if (firstInvocationExpression == null || firstInvocationExpression.ArgumentList.Arguments.Count != 0)
            {
                return;
            }

            var linqExtensionMethodSymbol = context.SemanticModel.GetSymbolInfo(firstInvocationExpression).Symbol as IMethodSymbol;
            var linqEnumerableSymbol = context.SemanticModel.GetLinqEnumerableSymbol();
            if (linqExtensionMethodSymbol == null || linqExtensionMethodSymbol.ContainingType != linqEnumerableSymbol)
            {
                return;
            }

            var alternativeMethod = QueryPrefix + linqExtensionMethodSymbol.Name;
            const string AsyncPostfix = "Async";
            if (methodSymbol.Name.EndsWith(AsyncPostfix))
            {
                alternativeMethod += AsyncPostfix;
            }

            var queryMethod = methodSymbol.ContainingType.GetMembers(alternativeMethod);
            if (queryMethod.Any())
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, invocationExpressionSyntax.Expression.GetLocation(), alternativeMethod));
            }
        }
    }
}
