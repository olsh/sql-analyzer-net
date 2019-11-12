using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using SqlAnalyzer.Net.Extensions;

namespace SqlAnalyzer.Net
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DapperQueryMisuseAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SQL003";

        public const string MessageFormat = "Consider using '{0}' method here";

        private const string Category = "API Guidance";

        private static readonly string Description =
            "The 'Query' method is designed to select a collection. For a single entity, there are most efficient alternatives like 'QueryFirst' or  'QuerySingle'.";

        private static readonly string Title = "Using 'Query' method is not optimal here";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            true,
            Description,
            "https://github.com/olsh/sql-analyzer-net/rules/SQL003.md");

        private static readonly Regex DapperQueryRegex = new Regex(
            @"^(?<MethodPrefix>Query|Read).*",
            RegexOptions.Compiled);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpressionSyntax = (InvocationExpressionSyntax)context.Node;

            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax)
                                   .Symbol as IMethodSymbol;
            if (methodSymbol == null)
            {
                return;
            }

            if (!methodSymbol.IsDapperMethod(context.SemanticModel))
            {
                return;
            }

            var methodMatch = DapperQueryRegex.Match(methodSymbol.Name);
            if (!methodMatch.Success)
            {
                return;
            }

            var firstInvocationExpression = invocationExpressionSyntax.Ancestors()
                .TakeWhile(n => !(n is StatementSyntax))
                .OfType<InvocationExpressionSyntax>()
                .FirstOrDefault();

            if (firstInvocationExpression == null || firstInvocationExpression.ArgumentList.Arguments.Count != 0)
            {
                return;
            }

            var linqExtensionMethodSymbol = context.SemanticModel.GetSymbolInfo(firstInvocationExpression)
                                                .Symbol as IMethodSymbol;
            var linqEnumerableSymbol = context.SemanticModel.GetLinqEnumerableSymbol();
            if (linqExtensionMethodSymbol == null || linqExtensionMethodSymbol.ContainingType != linqEnumerableSymbol)
            {
                return;
            }

            var alternativeMethod = methodMatch.Groups["MethodPrefix"] + linqExtensionMethodSymbol.Name;
            const string AsyncPostfix = "Async";
            if (methodSymbol.Name.EndsWith(AsyncPostfix))
            {
                alternativeMethod += AsyncPostfix;
            }

            // Found method in Dapper library by name
            var alternativeMethodSymbol = methodSymbol.ContainingType.GetMembers(alternativeMethod);

            // Check that generics count match
            if (alternativeMethodSymbol.Any(m => m is IMethodSymbol ms && ms.TypeArguments.Length == methodSymbol.TypeArguments.Length))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(Rule, invocationExpressionSyntax.Expression.GetLocation(), alternativeMethod));
            }
        }
    }
}
