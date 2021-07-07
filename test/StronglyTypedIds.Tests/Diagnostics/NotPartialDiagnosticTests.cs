using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using StronglyTypedIds.Diagnostics;
using Xunit;

namespace StronglyTypedIds.Tests.Diagnostics
{
    public class NotPartialDiagnosticTests
    {
        [Fact]
        public void Create()
        {
            var diagnostic = NotPartialDiagnostic.Create(
                SyntaxFactory.StructDeclaration("A"));

            Assert.Equal(NotPartialDiagnostic.Message, diagnostic.GetMessage());
            Assert.Equal(NotPartialDiagnostic.Title, diagnostic.Descriptor.Title);
            Assert.Equal(NotPartialDiagnostic.Id, diagnostic.Id);
            Assert.Equal(DiagnosticSeverity.Warning, diagnostic.Severity);
            Assert.Equal(Constants.Usage, diagnostic.Descriptor.Category);
        }
    }
}