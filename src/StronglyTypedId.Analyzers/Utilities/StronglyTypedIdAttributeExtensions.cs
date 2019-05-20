using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StronglyTypedId.Analyzers.Utilities
{
    internal static class StronglyTypedIdAttributeExtensions
    {
        public static bool IsStronglyTypedIdSyntax(this AttributeSyntax attSyntax)
        {
            return attSyntax.GetUnqualifiedName()?.IsStronglyTypedIdAttributeName() ?? false;
        }

        public static bool IsStronglyTypedIdAttributeName(this string text)
        {
            return !string.IsNullOrWhiteSpace(text)
                && (text == "StronglyTypedId" || text == "StronglyTypedIdAttribute");
        }

        public static string GetUnqualifiedName(this AttributeSyntax attSyntax)
        {
            var identifierNameSyntax =
                attSyntax.Name
                .DescendantNodesAndSelf()
                .LastOrDefault(node => node is IdentifierNameSyntax) as IdentifierNameSyntax;
            return identifierNameSyntax?.Identifier.ValueText;
        }
    }
}