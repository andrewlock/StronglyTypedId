using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace StronglyTypedIds.Diagnostics;

internal static class DiagnosticHelper
{
    public static void ReportDiagnostic(this SourceProductionContext context, DiagnosticInfo info)
    {
        var diagnostic = CreateDiagnostic(info);

        context.ReportDiagnostic(diagnostic);
    }

    public static Diagnostic CreateDiagnostic(DiagnosticInfo info)
    {
        var messageArgs = info.MessageArg is { } arg
            ? new object[] {arg}
            : null;

        ImmutableDictionary<string, string?>? properties = null;
        if (info.Properties is {Count: > 0} props)
        {
            var dict = ImmutableDictionary.CreateBuilder<string, string>();
            foreach (var prop in props.GetArray()!)
            {
                dict.Add(prop.Item1, prop.Item2);
            }

            properties = dict.ToImmutable()!;
        }


        var diagnostic = Diagnostic.Create(
            descriptor: info.Descriptor,
            location: info.Location,
            messageArgs: messageArgs,
            properties: properties);
        return diagnostic;
    }
}