using Microsoft.CodeAnalysis;

namespace StronglyTypedIds.Diagnostics;

internal static class InvalidTemplateNameDiagnostic
{
    internal const string Id = "STI8";
    internal const string Message = "The template name must not be null or whitespace.";
    internal const string Title = "Invalid template name";

    public static DiagnosticInfo CreateInfo(SyntaxNode currentNode)
        => new(new DiagnosticDescriptor(
                Id, Title, Message, category: Constants.Usage, defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true),
            currentNode.GetLocation());
}