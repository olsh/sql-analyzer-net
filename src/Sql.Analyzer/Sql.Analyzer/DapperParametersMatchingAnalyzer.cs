using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using Sql.Analyzer.Extensions;

namespace Sql.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DapperParametersMatchingAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "SQL002";

        public const string MessageFormatSqlVariableNotFound = "SQL variable not found for argument '{0}'";

        public const string MessageFormatCsharpArgumentNotFound = "Argument not found for SQL variable '{0}'";

        private const string Category = "API Guidance";

        private static readonly string Description = "Parameters mismatching";

        private static readonly string Title = "Parameters mismatching";

        private static readonly Regex SqlParameterRegex = new Regex(@"@(?<variable>\w+)", RegexOptions.Compiled);

        private static readonly DiagnosticDescriptor CsharpArgumentNotFoundRule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormatCsharpArgumentNotFound,
            Category,
            DiagnosticSeverity.Warning,
            true,
            Description);

        private static readonly DiagnosticDescriptor SqlParameterNotFoundRule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormatSqlVariableNotFound,
            Category,
            DiagnosticSeverity.Warning,
            true,
            Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(CsharpArgumentNotFoundRule, SqlParameterNotFoundRule);

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

            ReportDiagnostics(context, invocationExpressionSyntax);
        }

        private void ReportDiagnostics(
            SyntaxNodeAnalysisContext context,
            InvocationExpressionSyntax invocationExpressionSyntax)
        {
            List<string> sqlVariables = new List<string>();
            List<string> sharpParameters = null;
            foreach (var argument in invocationExpressionSyntax.ArgumentList.Arguments)
            {
                var parameter = argument.DetermineParameter(context.SemanticModel);
                if (string.Equals(parameter.Name, "sql"))
                {
                    var sourceText = argument.TryGetArgumentStringValue(context.SemanticModel);

                    // If SQL code is not constant, return
                    if (sourceText == null)
                    {
                        return;
                    }

                    var matches = SqlParameterRegex.Matches(sourceText);
                    foreach (Match match in matches)
                    {
                        sqlVariables.Add(match.Groups["variable"].Value);
                    }

                    continue;
                }

                if (string.Equals(parameter.Name, "param")
                    && context.SemanticModel.GetSymbolInfo(argument.Expression).Symbol is IMethodSymbol methodSymbol)
                {
                    sharpParameters = methodSymbol.Parameters.Select(p => p.Name.Trim('@')).ToList();
                }
            }

            if (sharpParameters == null)
            {
                return;
            }

            foreach (var notFoundArgument in sqlVariables.Except(sharpParameters))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        CsharpArgumentNotFoundRule,
                        invocationExpressionSyntax.GetLocation(),
                        notFoundArgument));
            }

            foreach (var notFoundVariable in sharpParameters.Except(sqlVariables))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        SqlParameterNotFoundRule,
                        invocationExpressionSyntax.GetLocation(),
                        notFoundVariable));
            }
        }
    }
}
