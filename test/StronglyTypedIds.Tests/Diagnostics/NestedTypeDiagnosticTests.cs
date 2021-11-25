using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StronglyTypedIds.Diagnostics;
using Xunit;

namespace StronglyTypedIds.Tests.Diagnostics
{
    public class NestedTypeDiagnosticTests
    {
        [Fact]
        public void Create()
        {
            var diagnostic = NestedTypeDiagnostic.Create(
                SyntaxFactory.ClassDeclaration("A")
                    .WithMembers(new SyntaxList<MemberDeclarationSyntax>(SyntaxFactory.StructDeclaration("A"))));

            Assert.Equal(NestedTypeDiagnostic.Message, diagnostic.GetMessage());
            Assert.Equal(NestedTypeDiagnostic.Title, diagnostic.Descriptor.Title);
            Assert.Equal(NestedTypeDiagnostic.Id, diagnostic.Id);
            Assert.Equal(DiagnosticSeverity.Warning, diagnostic.Severity);
            Assert.Equal(Constants.Usage, diagnostic.Descriptor.Category);
        }
    }
}