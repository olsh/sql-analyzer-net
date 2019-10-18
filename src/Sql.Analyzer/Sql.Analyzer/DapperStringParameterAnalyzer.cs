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

        public const string MessageFormat = "SQL type is not specified for '{0}' argument";

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
