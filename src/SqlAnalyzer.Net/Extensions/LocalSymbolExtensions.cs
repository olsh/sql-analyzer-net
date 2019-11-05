using Microsoft.CodeAnalysis;

namespace SqlAnalyzer.Net.Extensions
{
    public static class LocalSymbolExtensions
    {
        public static bool IsDapperDynamicParameter(this ILocalSymbol symbol, SemanticModel semanticModel)
        {
            return symbol.Type.Equals(semanticModel.GetDapperDynamicParametersSymbol());
        }
    }
}
