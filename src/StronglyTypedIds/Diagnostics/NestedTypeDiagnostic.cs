using Microsoft.CodeAnalysis;

namespace StronglyTypedIds.Diagnostics
{
    internal static class NestedTypeDiagnostic
    {
        public const string Id = "STI1";
        public const string Message = "The StronglyTypedId attribute is not supported on nested types.";
        public const string Title = "Nested types";

        public static Diagnostic Create(SyntaxNode currentNode) =>
            Diagnostic.Create(
                new DiagnosticDescriptor(
                    Id, Title, Message, category: Constants.Usage, defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true),
                currentNode.GetLocation());
    }
}
