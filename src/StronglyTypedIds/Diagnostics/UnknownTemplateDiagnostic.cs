using Microsoft.CodeAnalysis;

namespace StronglyTypedIds.Diagnostics;

internal static class UnknownTemplateDiagnostic
{
    internal const string Id = "STRONGID002";
    internal const string Title = "Unknown .typedid template";
    internal const string TemplateName = nameof(TemplateName);

    public static DiagnosticInfo CreateInfo(LocationInfo location, string name)
        => new(CreateDescriptor(),
            location.ToLocation(),
            name,
            new EquatableArray<(string, string)>(new[] {(TemplateName, name)}));

    public static DiagnosticDescriptor CreateDescriptor()
        => new(
            Id, Title, "Could not find '{0}.typedid' template. Ensure the template exists in the project and has a build action of 'Additional Files'.",
            category: Constants.Usage, defaultSeverity: DiagnosticSeverity.Error, isEnabledByDefault: true);
}