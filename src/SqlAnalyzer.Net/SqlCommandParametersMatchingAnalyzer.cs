using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using SqlAnalyzer.Net.Extensions;
using SqlAnalyzer.Net.Models;
using SqlAnalyzer.Net.Rules;
using SqlAnalyzer.Net.Walkers;

namespace SqlAnalyzer.Net
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class SqlCommandParametersMatchingAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(ParametersMatchingRule.CsharpArgumentNotFoundRule, ParametersMatchingRule.SqlParameterNotFoundRule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            var invocationExpressionSyntax = (InvocationExpressionSyntax)context.Node;

            if (!invocationExpressionSyntax.IsSqlCommandExecuteMethod(context.SemanticModel))
            {
                return;
            }

            var sqlCommandIdentifier = invocationExpressionSyntax.DescendantNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();
            if (sqlCommandIdentifier == null)
            {
                return;
            }

            var symbolInfo = context.SemanticModel.GetSymbolInfo(sqlCommandIdentifier);
            if (!(symbolInfo.Symbol is ILocalSymbol localSymbol))
            {
                return;
            }

            var scope = localSymbol.GetVariableScope();

            var sqlCommandParametersWalker = new SqlCommandParametersWalker(localSymbol, context.SemanticModel);
            sqlCommandParametersWalker.Visit(scope);

            if (!sqlCommandParametersWalker.IsInlineSql || !sqlCommandParametersWalker.IsAllParametersStatic || string.IsNullOrEmpty(sqlCommandParametersWalker.SqlText))
            {
                return;
            }

            ParametersMatchingRule.TryReportDiagnostics(
                sqlCommandParametersWalker.SqlText,
                sqlCommandParametersWalker.SqlParameters,
                invocationExpressionSyntax.GetLocation(),
                context,
                Orm.AdoNet);
        }
    }
}
