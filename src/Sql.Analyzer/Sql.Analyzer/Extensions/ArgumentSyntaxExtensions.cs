using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sql.Analyzer.Extensions
{
    public static class ArgumentSyntaxExtensions
    {
        /// <summary>
        ///     Returns the parameter to which this argument is passed. If <paramref name="allowParams" />
        ///     is true, the last parameter will be returned if it is params parameter and the index of
        ///     the specified argument is greater than the number of parameters.
        /// </summary>
        public static IParameterSymbol DetermineParameter(
            this ArgumentSyntax argument,
            SemanticModel semanticModel,
            bool allowParams = false,
            CancellationToken cancellationToken = default)
        {
            if (!(argument.Parent is BaseArgumentListSyntax argumentList))
            {
                return null;
            }

            if (!(argumentList.Parent is ExpressionSyntax invocableExpression))
            {
                return null;
            }

            var symbol = semanticModel.GetSymbolInfo(invocableExpression, cancellationToken).Symbol as IMethodSymbol;
            if (symbol == null)
            {
                return null;
            }

            var parameters = symbol.Parameters;

            // Handle named argument
            if (argument.NameColon != null && !argument.NameColon.IsMissing)
            {
                var name = argument.NameColon.Name.Identifier.ValueText;
                return parameters.FirstOrDefault(p => p.Name == name);
            }

            // Handle positional argument
            var index = argumentList.Arguments.IndexOf(argument);
            if (index < 0)
            {
                return null;
            }

            if (index < parameters.Length)
            {
                return parameters[index];
            }

            if (allowParams)
            {
                var lastParameter = parameters.LastOrDefault();
                if (lastParameter == null)
                {
                    return null;
                }

                if (lastParameter.IsParams)
                {
                    return lastParameter;
                }
            }

            return null;
        }

        public static string TryGetArgumentStringValue(this ArgumentSyntax argument, SemanticModel semanticModel)
        {
            if (argument.Expression is LiteralExpressionSyntax literalExpressionSyntax)
            {
                return literalExpressionSyntax.Token.Text;
            }

            var symbolVariable = semanticModel.GetConstantValue(argument.Expression);
            if (symbolVariable.HasValue)
            {
                return symbolVariable.Value?.ToString() ?? string.Empty;
            }

            return null;
        }
    }
}
