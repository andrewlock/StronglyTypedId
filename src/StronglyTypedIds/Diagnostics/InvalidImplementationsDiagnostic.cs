using Microsoft.CodeAnalysis;

namespace StronglyTypedIds.Diagnostics
{
    internal static class InvalidImplementationsDiagnostic
    {
        internal const string Id = "STI5";
        internal const string Message = "The StronglyTypedIdImplementations value provided is not a valid combination of flags";
        internal const string Title = "Invalid implementations value";

        public static Diagnostic Create(SyntaxNode currentNode) =>
            Diagnostic.Create(
                new DiagnosticDescriptor(
                    Id, Title, Message, category: Constants.Usage, defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true),
                currentNode.GetLocation());
    }
}