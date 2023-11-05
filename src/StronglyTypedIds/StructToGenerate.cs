using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace StronglyTypedIds;

internal readonly record struct StructToGenerate
{
    public StructToGenerate(string name, string nameSpace, string? templateName, ParentClass? parent, LocationInfo templateLocation)
    {
        Name = name;
        NameSpace = nameSpace;
        TemplateName = templateName;
        Template = null;
        Parent = parent;
        TemplateLocation = templateLocation;
    }

    public StructToGenerate(string name, string nameSpace, Template template, ParentClass? parent)
    {
        Name = name;
        NameSpace = nameSpace;
        TemplateName = null;
        Template = template;
        Parent = parent;
        TemplateLocation = null;
    }

    public string Name { get; }
    public string NameSpace { get; }
    public string? TemplateName { get; }
    public Template? Template { get; }
    public ParentClass? Parent { get; }
    public LocationInfo? TemplateLocation { get; }
}

internal sealed record Result<TValue>(TValue Value, EquatableArray<DiagnosticInfo> Errors)
    where TValue : IEquatable<TValue>?
{
    public static Result<(TValue, bool)> Fail()
        => new((default!, false), EquatableArray<DiagnosticInfo>.Empty);
}


internal readonly record struct Defaults
{
    public Defaults(string templateName, LocationInfo location, bool hasMultiple)
    {
        TemplateName = templateName;
        HasMultiple = hasMultiple;
        Template = null;
        TemplateLocation = location;
    }

    public Defaults(Template template, bool hasMultiple)
    {
        TemplateName = null;
        Template = template;
        HasMultiple = hasMultiple;
        TemplateLocation = null;
    }

    public string? TemplateName { get; }
    public Template? Template { get; }
    public LocationInfo? TemplateLocation { get; }
    public bool HasMultiple { get; }
}

internal record ParentClass(string Modifiers, string Keyword, string Name, string Constraints, ParentClass? Child, bool IsGeneric);

internal record LocationInfo(string FilePath, TextSpan TextSpan, LinePositionSpan LineSpan)
{
    public Location ToLocation()
        => Location.Create(FilePath, TextSpan, LineSpan);

    public static LocationInfo CreateFrom(SyntaxNode node)
    {
        var location = node.GetLocation();
        // assuming that source tree is always non-null here... hopefully that's the case
        return new LocationInfo(location.SourceTree!.FilePath, location.SourceSpan, location.GetLineSpan().Span);
    }
}