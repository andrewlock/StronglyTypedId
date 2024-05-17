using Microsoft.CodeAnalysis;

namespace StronglyTypedIds.Diagnostics;

internal static class MissingDefaultsDiagnostic
{
    internal const string Id = "STRONGID005";
    internal const string Message = "You must specify the default template to use for converters using [StronglyTypedIdConvertersDefaults]";
    internal const string Title = "Missing [StronglyTypedIdConvertersDefaults] attribute";

    public static DiagnosticInfo CreateInfo(Location location) =>
        new(new DiagnosticDescriptor(
                Id, Title, Message, category: Constants.Usage, defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true),
            location);
}