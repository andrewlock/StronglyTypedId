using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StronglyTypedId.Analyzers.CodeActions;

namespace StronglyTypedId.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp), Shared]
    public sealed class StronglyTypedIdMustBePartialFixer : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; }
            = ImmutableArray.Create(Descriptors.X1000_StronglyTypedIdMustBePartial.Id);

        public override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var structDeclaration = root.FindNode(context.Span).FirstAncestorOrSelf<StructDeclarationSyntax>();
            var document = context.Document;
            context.RegisterCodeFix(
                Actions.MakePartial(document, root, structDeclaration),
                context.Diagnostics);
        }
    }
}