using Microsoft.CodeAnalysis;

namespace StronglyTypedIds.Diagnostics
{
    internal static class InvalidConverterAndBackingTypeCombinationDiagnostic
    {
        internal const string Id = "STI6";
        internal const string Message = "The StronglyTypedIdBackingType and StronglyTypedIdConverter values provided are not a valid combination of flags";
        internal const string Title = "Invalid converter and backing type combination";

        public static Diagnostic Create(SyntaxNode currentNode) =>
            Diagnostic.Create(
                new DiagnosticDescriptor(
                    Id, Title, Message, category: Constants.Usage, defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true),
                currentNode.GetLocation());
    }
}
