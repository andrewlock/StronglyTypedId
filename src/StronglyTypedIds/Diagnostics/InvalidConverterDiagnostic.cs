using Microsoft.CodeAnalysis;

namespace StronglyTypedIds.Diagnostics
{
    internal static class InvalidConverterDiagnostic
    {
        internal const string Id = "STI3";
        internal const string Message = "The StronglyTypedIdConverter value provided is not a valid combination of flags";
        internal const string Title = "Invalid converter";

        public static Diagnostic Create(SyntaxNode currentNode) =>
            Diagnostic.Create(
                new DiagnosticDescriptor(
                    Id, Title, Message, category: Constants.Usage, defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true),
                currentNode.GetLocation());
        
        public static DiagnosticInfo CreateInfo(SyntaxNode currentNode)
            => new(new DiagnosticDescriptor(
                    Id, Title, Message, category: Constants.Usage, defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true),
                currentNode.GetLocation());
    }
}