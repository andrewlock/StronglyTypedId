using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StronglyTypedIds.Sources;

namespace StronglyTypedIds
{
    internal sealed class StronglyTypedIdReceiver : ISyntaxReceiver
    {
        public const string StronglyTypedIdAttributeName = nameof(StronglyTypedIdAttribute);

        public static readonly string StronglyTypedIdAttributeShortName = nameof(StronglyTypedIdAttribute)
            .Replace(nameof(Attribute), string.Empty);

        public List<(SyntaxNode Origin, StructDeclarationSyntax Declaration)> Targets { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is StructDeclarationSyntax structDeclarationSyntax)
                if (structDeclarationSyntax.AttributeLists.Count > 0
                && structDeclarationSyntax.AttributeLists
                    .SelectMany(attrList => attrList.Attributes)
                    .Select(attr => attr.Name.ToString())
                    .Any(attrName => attrName == StronglyTypedIdAttributeShortName || attrName == StronglyTypedIdAttributeName))
                {
                    Targets.Add((syntaxNode, structDeclarationSyntax));
                }
        }
    }
}