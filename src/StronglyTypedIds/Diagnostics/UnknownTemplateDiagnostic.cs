using Microsoft.CodeAnalysis;

namespace StronglyTypedIds.Diagnostics;

internal static class UnknownTemplateDiagnostic
{
    internal const string Id = "STI7";
    internal const string Title = "Unknown .typedid template";

    public static DiagnosticInfo CreateInfo(LocationInfo location, string name)
        => new(new DiagnosticDescriptor(
                Id, Title, $"Could not find '{name}.typedid' template. Ensure the template exists and has a build action of 'Additional Files'.", 
                category: Constants.Usage, defaultSeverity: DiagnosticSeverity.Warning, isEnabledByDefault: true),
            location.ToLocation());
}