using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using SqlAnalyzer.Net.Extensions;

namespace SqlAnalyzer.Net
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DapperStringParameterAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SQL001";

        public const string MessageFormat = "SQL type is not specified for '{0}' argument";

        private const string Category = "API Guidance";

        private static readonly string Description = "Specify SQL type explicitly with DbString class. On SQL Server it is crucial to use the unicode when querying unicode and ANSI when querying non unicode.";

        private static readonly string Title = "SQL type is not specified";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            true,
            Description,
            "https://github.com/olsh/sql-analyzer-net#sql001");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpressionSyntax = (InvocationExpressionSyntax)context.Node;

            if (!invocationExpressionSyntax.IsDapperInlineSqlMethod(context.SemanticModel))
            {
                return;
            }

            ReportStringArgument(context, invocationExpressionSyntax);
        }

        private void ReportStringArgument(
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

                foreach (var property in symbolInfo.Parameters)
                {
                    if (property.Type.Equals(stringType))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, argument.GetLocation(), property.Name));
                    }
                }

                break;
            }
        }
    }
}
