using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SqlAnalyzer.Net.Extensions
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

            if (!methodSymbol.IsDapperMethod(semanticModel))
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

        public static bool IsSqlCommandExecuteMethod(this InvocationExpressionSyntax invocationExpressionSyntax, SemanticModel semanticModel)
        {
            var methodSymbol = semanticModel.GetSymbolInfo(invocationExpressionSyntax).Symbol as IMethodSymbol;
            if (methodSymbol == null)
            {
                return false;
            }

            var sqlCommand = semanticModel.Compilation.GetTypeByMetadataName("System.Data.SqlClient.SqlCommand");
            if (methodSymbol.ContainingType == sqlCommand && (methodSymbol.Name.StartsWith("Execute") || methodSymbol.Name.StartsWith("BeginExecute")))
            {
                return true;
            }

            return false;
        }

        public static bool IsEntityFrameworkSaveChangesMethod(this InvocationExpressionSyntax invocationExpression, SemanticModel semanticModel)
        {
            var methodSymbol = semanticModel.GetSymbolInfo(invocationExpression).Symbol as IMethodSymbol;
            if (methodSymbol == null)
            {
                return false;
            }

            if (!methodSymbol.Name.StartsWith("SaveChanges"))
            {
                return false;
            }

            var type = semanticModel.Compilation.GetTypeByMetadataName("System.Data.Entity.DbContext");
            while (type != null)
            {
                if (methodSymbol.ContainingType == type)
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }
    }
}
