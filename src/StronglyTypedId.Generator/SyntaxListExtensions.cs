using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StronglyTypedId.Generator
{
    public static class SyntaxListExtensions
    {
        public static SyntaxList<MemberDeclarationSyntax> WrapWithAncestors(this SyntaxList<MemberDeclarationSyntax> nodesToWrap, CSharpSyntaxNode sourceNode)
        {
            return sourceNode.Ancestors().Aggregate(nodesToWrap, WrapInAncestor);
        }

        private static SyntaxList<MemberDeclarationSyntax> WrapInAncestor(SyntaxList<MemberDeclarationSyntax> generatedMembers, SyntaxNode ancestor)
        {
            switch (ancestor)
            {
                case NamespaceDeclarationSyntax ancestorNamespace:
                    generatedMembers = SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                        CopyAsAncestor(ancestorNamespace)
                            .WithMembers(generatedMembers));
                    break;
                case ClassDeclarationSyntax nestingClass:
                    generatedMembers = SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                        CopyAsAncestor(nestingClass)
                            .WithMembers(generatedMembers));
                    break;
                case StructDeclarationSyntax nestingStruct:
                    generatedMembers = SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                        CopyAsAncestor(nestingStruct)
                            .WithMembers(generatedMembers));
                    break;
            }
            return generatedMembers;
        }

        private static NamespaceDeclarationSyntax CopyAsAncestor(NamespaceDeclarationSyntax syntax)
        {
            return SyntaxFactory.NamespaceDeclaration(syntax.Name.WithoutTrivia())
                .WithExterns(SyntaxFactory.List(syntax.Externs.Select(x => x.WithoutTrivia())))
                .WithUsings(SyntaxFactory.List(syntax.Usings.Select(x => x.WithoutTrivia())));
        }

        private static ClassDeclarationSyntax CopyAsAncestor(ClassDeclarationSyntax syntax)
        {
            return SyntaxFactory.ClassDeclaration(syntax.Identifier.WithoutTrivia())
                .WithModifiers(SyntaxFactory.TokenList(syntax.Modifiers.Select(x => x.WithoutTrivia())))
                .WithTypeParameterList(syntax.TypeParameterList);
        }

        private static StructDeclarationSyntax CopyAsAncestor(StructDeclarationSyntax syntax)
        {
            return SyntaxFactory.StructDeclaration(syntax.Identifier.WithoutTrivia())
                .WithModifiers(SyntaxFactory.TokenList(syntax.Modifiers.Select(x => x.WithoutTrivia())))
                .WithTypeParameterList(syntax.TypeParameterList);
        }
    }
}