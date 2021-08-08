using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StronglyTypedIds.Diagnostics;
using Xunit;

namespace StronglyTypedIds.Tests.Diagnostics
{
    public class InvalidBackingTypeDiagnosticTests
    {
        [Fact]
        public void Create()
        {
            var diagnostic = InvalidBackingTypeDiagnostic.Create(
                SyntaxFactory.ClassDeclaration("A")
                    .WithMembers(new SyntaxList<MemberDeclarationSyntax>(SyntaxFactory.StructDeclaration("A"))));

            Assert.Equal(InvalidBackingTypeDiagnostic.Message, diagnostic.GetMessage());
            Assert.Equal(InvalidBackingTypeDiagnostic.Title, diagnostic.Descriptor.Title);
            Assert.Equal(InvalidBackingTypeDiagnostic.Id, diagnostic.Id);
            Assert.Equal(DiagnosticSeverity.Warning, diagnostic.Severity);
            Assert.Equal(Constants.Usage, diagnostic.Descriptor.Category);
        }
    }
}