using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SqlAnalyzer.Net.Extensions
{
    public static class ExpressionExtensions
    {
        public static MethodDeclarationSyntax FindMethodDeclaration(this ExpressionSyntax expression)
        {
            return expression.AncestorsAndSelf()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault();
        }
    }
}
