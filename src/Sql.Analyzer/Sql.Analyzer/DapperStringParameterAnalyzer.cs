using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Sql.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DapperStringParameterAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SQL001";

        private const string Category = "API Guidance";

        public static readonly string MessageFormat = "Parameter {0} ";

        private static readonly string Description = "You should specify type of SQL parameter explicitly";

        private static readonly string Title = "Specify SQL type explicitly with DbString class";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            true,
            Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var immutableArray = context.SemanticModel.GetDiagnostics();

            var invocationExpressionSyntax = (InvocationExpressionSyntax)context.Node;

            var symbolInfo = context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax);
            var s = symbolInfo.Symbol is IMethodSymbol;

            var memberAccessExpressionSyntax = invocationExpressionSyntax.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpressionSyntax == null)
            {
                return;
            }

            if (IsDangerousMethod(context, memberAccessExpressionSyntax))
            {
            }
        }

        private bool IsDangerousMethod(
            SyntaxNodeAnalysisContext context,
            MemberAccessExpressionSyntax memberAccessExpressionSyntax)
        {
            var memberName = memberAccessExpressionSyntax.Expression;
            var symbolInfo = context.SemanticModel.GetSymbolInfo(memberName);
            var symbolInfoSymbol = symbolInfo.Symbol as ILocalSymbol;
            var methodName = memberAccessExpressionSyntax.Name.ToString();
            var info = context.SemanticModel.GetSymbolInfo(memberAccessExpressionSyntax.Name);
            var declaredSymbol = context.SemanticModel.GetDeclaredSymbol(memberAccessExpressionSyntax.Name);

            return methodName != "ExecuteAsync";
        }
    }
}
