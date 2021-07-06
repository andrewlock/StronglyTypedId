using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StronglyTypedId
{
    internal sealed class StronglyTypedIdReceiver : ISyntaxReceiver
    {
        public const string StronglyTypedIdAttributeName = nameof(Sources.StronglyTypedIdAttribute);

        public static readonly string StronglyTypedIdAttributeShortName = nameof(Sources.StronglyTypedIdAttribute)
            .Replace(nameof(Attribute), string.Empty);

        public List<StructDeclarationSyntax> StronglyTypedIdStructs { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // any property with at least one attribute is a candidate for property generation
            if (syntaxNode is StructDeclarationSyntax structDeclarationSyntax
                && structDeclarationSyntax.AttributeLists.Count > 0
                && structDeclarationSyntax.AttributeLists
                    .SelectMany(attrList => attrList.Attributes)
                    .Select(attr => attr.Name.ToString())
                    .Any(attrName => attrName == StronglyTypedIdAttributeShortName || attrName == StronglyTypedIdAttributeName))
            {
                StronglyTypedIdStructs.Add(structDeclarationSyntax);
            }
        }
    }
}