using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SqlAnalyzer.Net.Walkers
{
    public class DapperAddInvocationExpressionWalker : CSharpSyntaxWalker
    {
        private readonly string _variableName;

        public DapperAddInvocationExpressionWalker(string variableName)
        {
            _variableName = variableName;
            IsAllParametersStatic = true;
        }

        public List<string> SqlParameters { get; } = new List<string>();

        public bool IsAllParametersStatic { get; private set; }

        public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            if (node.Identifier.ValueText != _variableName)
            {
                return;
            }

            var constructors = node.DescendantNodes()
                .OfType<ObjectCreationExpressionSyntax>()
                .ToList();

            if (constructors.Count != 1)
            {
                return;
            }

            var constructor = constructors.First();
            if (constructor.ArgumentList == null)
            {
                return;
            }

            if (constructor.ArgumentList.Arguments.Count == 0)
            {
                return;
            }

            if (constructor.ArgumentList.Arguments[0].Expression is AnonymousObjectCreationExpressionSyntax objectCreationExpressionSyntax)
            {
                SqlParameters.AddRange(objectCreationExpressionSyntax
                    .DescendantNodes()
                    .OfType<AnonymousObjectMemberDeclaratorSyntax>()
                    .Select(n => n.NameEquals.Name.Identifier.ValueText));

                return;
            }

            IsAllParametersStatic = false;
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var identifierNames = node.DescendantNodes()
                .OfType<SimpleNameSyntax>()
                .ToList();

            if (identifierNames.Count < 2)
            {
                return;
            }

            if (identifierNames[0].Identifier.Text != _variableName)
            {
                return;
            }

            if (identifierNames[1].Identifier.Text != "Add")
            {
                IsAllParametersStatic = false;

                return;
            }

            if (node.ArgumentList.Arguments.Count < 1)
            {
                return;
            }

            if (!(node.ArgumentList.Arguments[0].Expression is LiteralExpressionSyntax literalExpression))
            {
                IsAllParametersStatic = false;

                return;
            }

            SqlParameters.Add(literalExpression.Token.ValueText);
        }
    }
}
