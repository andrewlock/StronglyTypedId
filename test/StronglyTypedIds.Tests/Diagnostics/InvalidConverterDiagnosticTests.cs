using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StronglyTypedIds.Diagnostics;
using Xunit;

namespace StronglyTypedIds.Tests.Diagnostics
{
    public class InvalidConverterDiagnosticTests
    {
        [Fact]
        public void Create()
        {
            var diagnostic = InvalidConverterDiagnostic.Create(
                SyntaxFactory.ClassDeclaration("A")
                    .WithMembers(new SyntaxList<MemberDeclarationSyntax>(SyntaxFactory.StructDeclaration("A"))));

            Assert.Equal(InvalidConverterDiagnostic.Message, diagnostic.GetMessage());
            Assert.Equal(InvalidConverterDiagnostic.Title, diagnostic.Descriptor.Title);
            Assert.Equal(InvalidConverterDiagnostic.Id, diagnostic.Id);
            Assert.Equal(DiagnosticSeverity.Warning, diagnostic.Severity);
            Assert.Equal(Constants.Usage, diagnostic.Descriptor.Category);
        }
    }
}