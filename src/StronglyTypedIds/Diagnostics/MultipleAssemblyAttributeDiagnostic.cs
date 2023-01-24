using Microsoft.CodeAnalysis;

namespace StronglyTypedIds.Diagnostics;

internal static class MultipleAssemblyAttributeDiagnostic
{
    internal const string Id = "STI6";
    internal const string Message = "You may only have one instance of the StronglyTypedIdDefaults assembly attribute";
    internal const string Title = "Multiple assembly attributes";

    public static DiagnosticInfo CreateInfo(SyntaxNode currentNode) =>
        new(new DiagnosticDescriptor(
                Id, Title, Message, category: Constants.Usage, defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true),
            currentNode.GetLocation());
}