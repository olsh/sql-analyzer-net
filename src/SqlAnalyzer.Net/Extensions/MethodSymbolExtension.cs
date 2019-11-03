using Microsoft.CodeAnalysis;

namespace SqlAnalyzer.Net.Extensions
{
    public static class MethodSymbolExtension
    {
        public static bool IsDapperMethod(this IMethodSymbol symbol, SemanticModel semanticModel)
        {
            var sqlMapper = semanticModel.Compilation.GetTypeByMetadataName("Dapper.SqlMapper");
            if (symbol.ContainingType == sqlMapper)
            {
                return true;
            }

            var sqlGridReader = semanticModel.Compilation.GetTypeByMetadataName("Dapper.SqlMapper+GridReader");
            return symbol.ContainingType == sqlGridReader;
        }
    }
}
