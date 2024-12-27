using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace StronglyTypedIds;

internal readonly record struct StructToGenerate
{
    public StructToGenerate(DeclarationKind declarationKind, string name, string nameSpace, Template? template, string[]? templateNames, ParentClass? parent, LocationInfo templateLocation)
    {
        DeclarationKind = declarationKind;
        Name = name;
        NameSpace = nameSpace;
        TemplateNames = templateNames is null ? EquatableArray<string>.Empty : new EquatableArray<string>(templateNames);
        Template = template;
        Parent = parent;
        TemplateLocation = templateLocation;
    }

    public DeclarationKind DeclarationKind { get; }
    public string Name { get; }
    public string NameSpace { get; }
    public EquatableArray<string> TemplateNames { get; }
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
    public Defaults(Template? template, string[]? templateNames, LocationInfo location, bool hasMultiple)
    {
        TemplateNames = templateNames is null ? EquatableArray<string>.Empty : new EquatableArray<string>(templateNames);
        HasMultiple = hasMultiple;
        Template = template;
        TemplateLocation = location;
    }

    public EquatableArray<string> TemplateNames { get; }
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

internal enum DeclarationKind
{
    Class,
    Struct,
    RecordStruct,
    RecordClass,
    Record,
}