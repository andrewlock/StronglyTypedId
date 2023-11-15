using Microsoft.CodeAnalysis;

namespace StronglyTypedIds;

internal sealed record DiagnosticInfo
{
    public DiagnosticInfo(DiagnosticDescriptor descriptor, Location location, string? messageArg = null, EquatableArray<(string, string)> properties = default)
    {
        Descriptor = descriptor;
        Location = location;
        MessageArg = messageArg;
        Properties = properties;
    }

    public DiagnosticDescriptor Descriptor { get; }
    public Location Location { get; }
    public string? MessageArg { get; }
    public EquatableArray<(string, string)> Properties { get; }
}