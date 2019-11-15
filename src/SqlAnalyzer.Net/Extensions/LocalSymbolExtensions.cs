using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SqlAnalyzer.Net.Extensions
{
    public static class LocalSymbolExtensions
    {
        public static bool IsDapperDynamicParameter(this ILocalSymbol symbol, SemanticModel semanticModel)
        {
            return symbol.Type.Equals(semanticModel.GetDapperDynamicParametersSymbol());
        }

        public static SyntaxNode GetVariableScope(this ILocalSymbol symbol)
        {
            if (symbol.DeclaringSyntaxReferences.Length != 1)
            {
                return null;
            }

            var declarationSyntax = symbol.DeclaringSyntaxReferences.First().GetSyntax();

            var syntaxNodes = declarationSyntax.Ancestors();

            foreach (var syntaxNode in syntaxNodes)
            {
                if (syntaxNode is UsingStatementSyntax us || syntaxNode is MethodDeclarationSyntax || syntaxNode is BlockSyntax)
                {
                    return syntaxNode;
                }
            }

            return null;
        }
    }
}
