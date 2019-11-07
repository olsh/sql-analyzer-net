using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SqlAnalyzer.Net.Extensions;

namespace SqlAnalyzer.Net.Walkers
{
    public class SqlCommandParametersWalker : CSharpSyntaxWalker
    {
        private readonly SemanticModel _semanticModel;

        private readonly ILocalSymbol _sqlCommandSymbol;

        public SqlCommandParametersWalker(ILocalSymbol sqlCommandSymbol, SemanticModel semanticModel)
        {
            _sqlCommandSymbol = sqlCommandSymbol;
            _semanticModel = semanticModel;
            IsAllParametersStatic = true;
            IsInlineSql = true;
        }

        public bool IsAllParametersStatic { get; private set; }

        public bool IsInlineSql { get; private set; }

        public List<string> SqlParameters { get; } = new List<string>();

        public string SqlText { get; private set; }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            if (node.Left is MemberAccessExpressionSyntax leftAccessExpression
                && leftAccessExpression.Expression is SimpleNameSyntax leftIdentifier
                && leftIdentifier.Identifier.Text == _sqlCommandSymbol.Name )
            {
                const string CommandTypeIdentifier = "CommandType";
                if (leftAccessExpression.Name.Identifier.Text == CommandTypeIdentifier
                    && node.Right is MemberAccessExpressionSyntax rightAccessExpression
                    && rightAccessExpression.Expression is SimpleNameSyntax rightIdentifier
                    && rightIdentifier.Identifier.Text == CommandTypeIdentifier)
                {
                    IsInlineSql = rightAccessExpression.Name.Identifier.Text == "Text";
                }
                else if (leftAccessExpression.Name.Identifier.Text == "CommandText")
                {
                    SqlText = node.Right.TryGetArgumentStringValue(_semanticModel);
                }
            }

            base.VisitAssignmentExpression(node);
        }

        public override void VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {
            var identifierNames = node.DescendantNodes()
                .OfType<SimpleNameSyntax>()
                .ToList();

            if (identifierNames.Count > 1
                && identifierNames[0].Identifier.Text == _sqlCommandSymbol.Name
                && identifierNames[1].Identifier.Text == "Parameters"
                && node.ArgumentList?.Arguments.Count == 1
                && node.ArgumentList.Arguments.Single().Expression is LiteralExpressionSyntax literalExpression)
            {
                SqlParameters.Add(literalExpression.Token.ValueText);
            }

            base.VisitElementAccessExpression(node);
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var identifierNames = node.DescendantNodes()
                .OfType<SimpleNameSyntax>()
                .ToList();
            if (identifierNames.Count < 3)
            {
                return;
            }

            if (identifierNames[0]
                    .Identifier.Text != _sqlCommandSymbol.Name)
            {
                return;
            }

            var methodIdentifierName = identifierNames[2].Identifier.Text;
            if (methodIdentifierName != "Add" && methodIdentifierName != "AddWithValue")
            {
                return;
            }

            if (node.ArgumentList.Arguments.Count < 1)
            {
                return;
            }

            if (!(node.ArgumentList.Arguments[0]
                      .Expression is LiteralExpressionSyntax literalExpression))
            {
                IsAllParametersStatic = false;

                return;
            }

            SqlParameters.Add(literalExpression.Token.ValueText);
        }

        public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            if (node.Identifier.ValueText != _sqlCommandSymbol.Name)
            {
                return;
            }

            var constructors = node.DescendantNodes()
                .OfType<ObjectCreationExpressionSyntax>()
                .ToList();

            var constructor = constructors.FirstOrDefault();
            if (constructor?.ArgumentList == null)
            {
                return;
            }

            if (constructor.ArgumentList.Arguments.Count == 0)
            {
                return;
            }

            SqlText = constructor.ArgumentList.Arguments[0]
                .TryGetArgumentStringValue(_semanticModel);
        }
    }
}
