using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sql.Analyzer.Extensions
{
    public static class InvocationExpressionExtensions
    {
        public static bool IsDapperInlineSqlMethod(this InvocationExpressionSyntax invocationExpressionSyntax, SemanticModel semanticModel)
        {
            var methodSymbol = semanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol as IMethodSymbol;
            if (methodSymbol == null)
            {
                return false;
            }

            var dapperClass = semanticModel.GetDapperSqlMapperSymbol();
            if (methodSymbol.ContainingType != dapperClass)
            {
                return false;
            }

            foreach (var argument in invocationExpressionSyntax.ArgumentList.Arguments)
            {
                var parameter = argument.DetermineParameter(semanticModel);
                if (!string.Equals(parameter.Name, "commandType"))
                {
                    continue;
                }

                var symbolInfo = semanticModel.GetSymbolInfo(argument.Expression).Symbol;
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
    }
}
