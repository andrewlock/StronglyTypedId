using Microsoft.CodeAnalysis;

namespace StronglyTypedIds;

internal sealed record DiagnosticInfo
{
    public DiagnosticInfo(DiagnosticDescriptor descriptor, Location location)
    {
        Descriptor = descriptor;
        Location = location;
    }

    public DiagnosticDescriptor Descriptor { get; }
    public Location Location { get; }
}