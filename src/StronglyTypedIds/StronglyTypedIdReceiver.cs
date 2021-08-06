using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StronglyTypedIds.Sources;

namespace StronglyTypedIds
{
    internal sealed class StronglyTypedIdReceiver : ISyntaxReceiver
    {
        public const string StronglyTypedIdAttributeShortName = "StronglyTypedId";
        public const string FullyQualifiedStronglyTypedIdAttributeShortName = Constants.Namespace + "." + StronglyTypedIdAttributeShortName;

        public const string StronglyTypedIdDefaultsAttributeShortName = "StronglyTypedIdDefaults";
        public const string FullyQualifiedStronglyTypedIdDefaultsAttributeShortName = Constants.Namespace + "." + StronglyTypedIdDefaultsAttributeShortName;

        public List<(SyntaxNode Origin, StructDeclarationSyntax Declaration)> Targets { get; } = new();
        public List<(SyntaxNode Origin, AttributeListSyntax AtttributeList)> Defaults { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is StructDeclarationSyntax structDeclarationSyntax)
            {
                if (structDeclarationSyntax.AttributeLists.Count > 0
                    && structDeclarationSyntax.AttributeLists
                        .SelectMany(attrList => attrList.Attributes)
                        .Select(attr => attr.Name.ToString())
                        .Any(attrName =>
                            attrName == StronglyTypedIdAttributeShortName
                            || attrName == FullyQualifiedStronglyTypedIdAttributeShortName
                            || attrName == Constants.StronglyTypedIdAttribute
                            || attrName == Constants.FullyQualifiedStronglyTypedIdAttribute))
                {
                    Targets.Add((syntaxNode, structDeclarationSyntax));
                }
            }
            // Doesn't seem to be necessary for generation
            else if (syntaxNode is AttributeListSyntax attributeList
                     && attributeList.Target is not null
                     && attributeList.Target.Identifier.IsKind(SyntaxKind.AssemblyKeyword)
                     && attributeList.Attributes
                         .Select(attr => attr.Name.ToString())
                         .Any(attrName =>
                             attrName == StronglyTypedIdDefaultsAttributeShortName
                             || attrName == FullyQualifiedStronglyTypedIdDefaultsAttributeShortName
                             || attrName == Constants.StronglyTypedIdDefaultsAttribute
                             || attrName == Constants.FullyQualifiedStronglyTypedIdDefaultsAttribute))
            {
                Defaults.Add((syntaxNode, attributeList));
            }
        }
    }
}