using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SqlAnalyzer.Net.Walkers
{
    public class LocalMemberAccessExpressionWalker : CSharpSyntaxWalker
    {
        public readonly List<string> MethodCalls = new List<string>();

        private readonly string _variableName;

        public LocalMemberAccessExpressionWalker(string variableName)
        {
            _variableName = variableName;
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            var identifierNames = node.DescendantNodes().OfType<SimpleNameSyntax>().ToList();
            if (identifierNames.Count < 2)
            {
                return;
            }

            if (identifierNames[0].Identifier.Text != _variableName)
            {
                return;
            }

            MethodCalls.Add(identifierNames[1].Identifier.Text);
        }
    }
}
