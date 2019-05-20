using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace StronglyTypedId.Analyzers.CodeActions
{
    internal static class Actions
    {
        private class MakePartialCodeAction : CodeAction
        {
            public MakePartialCodeAction(Document document, SyntaxNode root, StructDeclarationSyntax structDeclaration)
            {
                Document = document;
                Root = root;
                StructDeclaration = structDeclaration;
            }

            public override string Title => "Make Partial";
            public override string EquivalenceKey => Title;

            public Document Document { get; }
            public SyntaxNode Root { get; }
            public StructDeclarationSyntax StructDeclaration { get; }

            protected override Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
            {
                var modifiedNode = StructDeclaration.AddModifiers(Token(SyntaxKind.PartialKeyword));
                return Task.FromResult(Document.WithSyntaxRoot(Root.ReplaceNode(StructDeclaration, modifiedNode)));
            }
        }

        public static CodeAction MakePartial(Document document, SyntaxNode root, StructDeclarationSyntax structDeclaration)
        {
            return new MakePartialCodeAction(document, root, structDeclaration);
        }
    }
}