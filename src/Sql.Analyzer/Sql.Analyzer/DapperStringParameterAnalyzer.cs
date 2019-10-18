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
    public class DapperStringParameterAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SQL001";

        public const string MessageFormat = "Argument with unspecified string type";

        private const string Category = "API Guidance";

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
            var invocationExpressionSyntax = (InvocationExpressionSyntax)context.Node;

            var methodSymbol = context.SemanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol as IMethodSymbol;
            if (methodSymbol == null)
            {
                return;
            }

            var dapperClass = context.SemanticModel.Compilation.GetTypeByMetadataName("Dapper.SqlMapper");
            if (methodSymbol.ContainingType != dapperClass)
            {
                return;
            }

            if (!CheckIfInlineSqlCommand(context, invocationExpressionSyntax))
            {
                return;
            }

            var badStringArgument = FindBadStringArgument(context, invocationExpressionSyntax);
            if (badStringArgument == null)
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, badStringArgument.GetLocation()));
        }

        private bool CheckIfInlineSqlCommand(
            SyntaxNodeAnalysisContext context,
            InvocationExpressionSyntax invocationExpressionSyntax)
        {
            foreach (var argument in invocationExpressionSyntax.ArgumentList.Arguments)
            {
                var parameter = argument.DetermineParameter(context.SemanticModel);
                if (!string.Equals(parameter.Name, "commandType"))
                {
                    continue;
                }

                var symbolInfo = context.SemanticModel.GetSymbolInfo(argument.Expression).Symbol;
                if (symbolInfo == null)
                {
                    continue;
                }

                if (string.Equals(symbolInfo.Name, "Text"))
                {
                    break;
                }

                return false;
            }

            return true;
        }

        private ArgumentSyntax FindBadStringArgument(
            SyntaxNodeAnalysisContext context,
            InvocationExpressionSyntax invocationExpressionSyntax)
        {
            var stringType = context.SemanticModel.Compilation.GetTypeByMetadataName("System.String");
            foreach (var argument in invocationExpressionSyntax.ArgumentList.Arguments)
            {
                var parameter = argument.DetermineParameter(context.SemanticModel);
                if (!string.Equals(parameter.Name, "param"))
                {
                    continue;
                }

                var symbolInfo = context.SemanticModel.GetSymbolInfo(argument.Expression).Symbol as IMethodSymbol;
                if (symbolInfo == null)
                {
                    break;
                }

                if (symbolInfo.Parameters.Any(p => p.Type.Equals(stringType)))
                {
                    return argument;
                }

                break;
            }

            return null;
        }
    }
}
