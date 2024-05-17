using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StronglyTypedIds.Diagnostics;

namespace StronglyTypedIds;

internal static class Parser
{
    public const string StronglyTypedIdAttribute = "StronglyTypedIds.StronglyTypedIdAttribute";
    public const string StronglyTypedIdDefaultsAttribute = "StronglyTypedIds.StronglyTypedIdDefaultsAttribute";
    public const string StronglyTypedIdConvertersAttribute = "StronglyTypedIds.StronglyTypedIdConvertersAttribute`1";
    public const string StronglyTypedIdConvertersDefaultsAttribute = "StronglyTypedIds.StronglyTypedIdConvertersDefaultsAttribute";

    public static Result<(StructToGenerate info, bool valid)> GetIdSemanticTarget(GeneratorAttributeSyntaxContext ctx, CancellationToken ct)
    {
        var structSymbol = ctx.TargetSymbol as INamedTypeSymbol;
        if (structSymbol is null)
        {
            return Result<StructToGenerate>.Fail();
        }

        var structSyntax = (StructDeclarationSyntax)ctx.TargetNode;

        var hasMisconfiguredInput = false;
        List<DiagnosticInfo>? diagnostics = null;
        Template? template = null;
        string[]? templateNames = null;
        LocationInfo? attributeLocation = null;

        foreach (AttributeData attribute in structSymbol.GetAttributes())
        {
            if (!((attribute.AttributeClass?.Name == "StronglyTypedIdAttribute" ||
                  attribute.AttributeClass?.Name == "StronglyTypedId") &&
                  attribute.AttributeClass.ToDisplayString() == StronglyTypedIdAttribute))
            {
                // wrong attribute
                continue;
            }

            (var result, (template, templateNames)) = GetConstructorValues(attribute);
            hasMisconfiguredInput |= result;

            if (attribute.ApplicationSyntaxReference?.GetSyntax() is { } s)
            {
                attributeLocation = LocationInfo.CreateFrom(s);
            }
        }

        var hasPartialModifier = false;
        foreach (var modifier in structSyntax.Modifiers)
        {
            if (modifier.IsKind(SyntaxKind.PartialKeyword))
            {
                hasPartialModifier = true;
                break;
            }
        }

        if (!hasPartialModifier)
        {
            diagnostics ??= new();
            diagnostics.Add(NotPartialDiagnostic.CreateInfo(structSyntax));
        }

        var errors = diagnostics is null
            ? EquatableArray<DiagnosticInfo>.Empty
            : new EquatableArray<DiagnosticInfo>(diagnostics.ToArray());

        if (hasMisconfiguredInput)
        {
            return new Result<(StructToGenerate, bool)>((default, false), errors);
        }

        string nameSpace = GetNameSpace(structSyntax);
        ParentClass? parentClass = GetParentClasses(structSyntax);
        var name = structSymbol.Name;

        var toGenerate =new StructToGenerate(
            name: name, 
            nameSpace: nameSpace, 
            template: template, 
            templateNames: templateNames, 
            templateLocation: attributeLocation!,
            parent: parentClass);

        return new Result<(StructToGenerate, bool)>((toGenerate, true), errors);
    }

    public static Result<(Defaults defaults, bool valid)> GetIdDefaults(
        GeneratorAttributeSyntaxContext ctx, CancellationToken ct)
    {
        var assemblyAttributes = ctx.TargetSymbol.GetAttributes();
        if (assemblyAttributes.IsDefaultOrEmpty)
        {
            return Result<Defaults>.Fail();
        }

        // We only return the first config that we find
        string[]? templateNames = null;
        Template? template = null;
        LocationInfo? attributeLocation = null;
        List<DiagnosticInfo>? diagnostics = null;
        bool hasMisconfiguredInput = false;
        bool hasMultiple = false;

        // if we have multiple attributes we still check them, so that we can add extra diagnostics if necessary
        // the "first" one found won't be flagged as a duplicate though.
        foreach (AttributeData attribute in assemblyAttributes)
        {
            if (!((attribute.AttributeClass?.Name == "StronglyTypedIdDefaultsAttribute" ||
                   attribute.AttributeClass?.Name == "StronglyTypedIdDefaults") &&
                  attribute.AttributeClass.ToDisplayString() == StronglyTypedIdDefaultsAttribute))
            {
                // wrong attribute
                continue;
            }

            var syntax = attribute.ApplicationSyntaxReference?.GetSyntax();
            if (templateNames is not null || template.HasValue || hasMisconfiguredInput)
            {
                hasMultiple = true;
                if (syntax is not null)
                {
                    diagnostics ??= new();
                    diagnostics.Add(MultipleAssemblyAttributeDiagnostic.CreateInfo(syntax));
                }
            }

            (var result, (template, templateNames)) = GetConstructorValues(attribute);
            hasMisconfiguredInput |= result;

            if (syntax is not null)
            {
                attributeLocation = LocationInfo.CreateFrom(syntax);
            }
        }

        var errors = diagnostics is null
            ? EquatableArray<DiagnosticInfo>.Empty
            : new EquatableArray<DiagnosticInfo>(diagnostics.ToArray());

        if (hasMisconfiguredInput)
        {
            return new Result<(Defaults, bool)>((default, false), errors);
        }

        var defaults = new Defaults(template, templateNames, attributeLocation!, hasMultiple);
        return new Result<(Defaults, bool)>((defaults, true), errors);
    }

    public static Result<(ConverterToGenerate info, bool valid)> GetConvertersSemanticTarget(GeneratorAttributeSyntaxContext ctx, CancellationToken ct)
    {
        var structSymbol = ctx.TargetSymbol as INamedTypeSymbol;
        if (structSymbol is null)
        {
            return Result<ConverterToGenerate>.Fail();
        }

        var structSyntax = (StructDeclarationSyntax)ctx.TargetNode;

        var hasMisconfiguredInput = false;
        List<DiagnosticInfo>? diagnostics = null;
        string[]? templateNames = null;
        LocationInfo? attributeLocation = null;
        string? idName = null;

        foreach (AttributeData attribute in structSymbol.GetAttributes())
        {
            if (!((attribute.AttributeClass?.Name == "StronglyTypedIdConvertersAttribute" ||
                  attribute.AttributeClass?.Name == "StronglyTypedIdConverters") &&
                  attribute.AttributeClass.IsGenericType &&
                  attribute.AttributeClass.TypeArguments.Length == 1))
            {
                // wrong attribute
                continue;
            }

            // Can never have template
            (var result, (_, templateNames)) = GetConstructorValues(attribute);
            hasMisconfiguredInput |= result;

            if (attribute.ApplicationSyntaxReference?.GetSyntax() is { } s)
            {
                attributeLocation = LocationInfo.CreateFrom(s);
            }

            var typeParameter = attribute.AttributeClass.TypeArguments[0];
            idName = typeParameter.ToString();
        }

        var hasPartialModifier = false;
        foreach (var modifier in structSyntax.Modifiers)
        {
            if (modifier.IsKind(SyntaxKind.PartialKeyword))
            {
                hasPartialModifier = true;
                break;
            }
        }

        if (!hasPartialModifier)
        {
            diagnostics ??= new();
            diagnostics.Add(NotPartialDiagnostic.CreateInfo(structSyntax));
        }

        var errors = diagnostics is null
            ? EquatableArray<DiagnosticInfo>.Empty
            : new EquatableArray<DiagnosticInfo>(diagnostics.ToArray());

        if (hasMisconfiguredInput || idName is null)
        {
            return new Result<(ConverterToGenerate, bool)>((default, false), errors);
        }

        string nameSpace = GetNameSpace(structSyntax);
        ParentClass? parentClass = GetParentClasses(structSyntax);
        var name = structSymbol.Name;

        var toGenerate = new ConverterToGenerate(
            name: name,
            nameSpace: nameSpace,
            idName: idName,
            templateNames: templateNames,
            templateLocation: attributeLocation!,
            parent: parentClass);

        return new Result<(ConverterToGenerate, bool)>((toGenerate, true), errors);
    }

    public static Result<(Defaults defaults, bool valid)> GetConverterDefaults(
        GeneratorAttributeSyntaxContext ctx, CancellationToken ct)
    {
        var assemblyAttributes = ctx.TargetSymbol.GetAttributes();
        if (assemblyAttributes.IsDefaultOrEmpty)
        {
            return Result<Defaults>.Fail();
        }

        // We only return the first config that we find
        string[]? templateNames = null;
        LocationInfo? attributeLocation = null;
        List<DiagnosticInfo>? diagnostics = null;
        bool hasMisconfiguredInput = false;
        bool hasMultiple = false;

        // if we have multiple attributes we still check them, so that we can add extra diagnostics if necessary
        // the "first" one found won't be flagged as a duplicate though.
        foreach (AttributeData attribute in assemblyAttributes)
        {
            if (!((attribute.AttributeClass?.Name == "StronglyTypedIdConvertersDefaultsAttribute" ||
                   attribute.AttributeClass?.Name == "StronglyTypedIdConvertersDefaults") &&
                  attribute.AttributeClass.ToDisplayString() == StronglyTypedIdConvertersDefaultsAttribute))
            {
                // wrong attribute
                continue;
            }

            var syntax = attribute.ApplicationSyntaxReference?.GetSyntax();
            if (templateNames is not null || hasMisconfiguredInput)
            {
                hasMultiple = true;
                if (syntax is not null)
                {
                    diagnostics ??= new();
                    diagnostics.Add(MultipleAssemblyAttributeDiagnostic.CreateInfo(syntax));
                }
            }

            (var result, (_, templateNames)) = GetConstructorValues(attribute);
            hasMisconfiguredInput |= result;

            if (syntax is not null)
            {
                attributeLocation = LocationInfo.CreateFrom(syntax);
            }
        }

        var errors = diagnostics is null
            ? EquatableArray<DiagnosticInfo>.Empty
            : new EquatableArray<DiagnosticInfo>(diagnostics.ToArray());

        if (hasMisconfiguredInput)
        {
            return new Result<(Defaults, bool)>((default, false), errors);
        }

        var defaults = new Defaults(template: null, templateNames, attributeLocation!, hasMultiple);

        return new Result<(Defaults, bool)>((defaults, true), errors);
    }

    private static (bool HasMisconfiguredInput, (Template? Template, string[]? TemplateNames)) GetConstructorValues(AttributeData attribute)
    {
        (Template? Template, string? Name, string[]? Names)? results1 = null;
        (Template? Template, string? Name, string[]? Names)? results2 = null;

        if (attribute.ConstructorArguments is { IsEmpty: false } args)
        {
            // we should have at most 2 args (params count as one arg)
            if (args.Length > 2)
            {
                // have an error, so don't try and do any generation
                return (true, default);
            }

            // Should always have at least one arg, but it might be an empty array
            var (success, results) = TryGetTypedConstant(args[0]);

            if (success)
            {
                results1 = results;
            }
            else
            {
                // have an error, so don't try and do any generation
                return (true, default);
            }

            if (args.Length == 2)
            {
                (success, results) = TryGetTypedConstant(args[1]);

                if (success)
                {
                    results2 = results;
                }
                else
                {
                    // have an error, so don't try and do any generation
                    return (true, default);
                }
            }
        }
        
        if (attribute.NamedArguments is { IsEmpty: false } namedArgs)
        {
            foreach (KeyValuePair<string, TypedConstant> arg in namedArgs)
            {
                // Should always have at least one arg, but it might be an empty array
                var (success, results) = TryGetTypedConstant(arg.Value);

                if (success)
                {
                    if (results1 is null)
                    {
                        results1 = results;
                    }
                    else if(results2 is null)
                    {
                        results2 = results;
                    }
                    else
                    {
                        // must be an error
                        return (true, default);
                    }
                }
                else
                {
                    // have an error, so don't try and do any generation
                    return (true, default);
                }
            }
        }


        // consolidate
        var template = results1?.Template ?? results2?.Template;
        var name = results1?.Name ?? results2?.Name;
        var names = results1?.Names ?? results2?.Names;

        if (name is not null)
        {
            if (names is {Length: > 0} ns)
            {
                names = new string[ns.Length + 1];
                ns.CopyTo(names, 0);
                names[^1] = name;
            }
            else
            {
                names = new[] {name};
            }
        }

        return (false, (template, names));

        static (bool IsValid, (Template? Template, string? Name, string[]? Names)) TryGetTypedConstant(in TypedConstant arg)
        {
            if (arg.Kind is TypedConstantKind.Error)
            {
                return (false, default);
            }

            if (arg.Kind is TypedConstantKind.Primitive
                && arg.Value is string stringValue)
            {
                var name = string.IsNullOrWhiteSpace(stringValue)
                    ? string.Empty
                    : stringValue;

                return (true, (null, name, null));
            }

            if (arg.Kind is TypedConstantKind.Enum
                && arg.Value is int intValue)
            {
                return (true, ((Template) intValue, null, null));
            }

            if (arg.Kind is TypedConstantKind.Array)
            {
                var values = arg.Values;

                // if null is passed, it's treated the same as an empty array   
                if (values.IsDefaultOrEmpty)
                {
                    return (true, (null, null, Array.Empty<string>()));
                }

                var names = new string[values.Length];
                for (var i = 0; i < values.Length; i++)
                {
                    if (values[i].Kind == TypedConstantKind.Error)
                    {
                        // Abandon generation
                        return (false, default);
                    }

                    var value = values[i].Value as string;
                    names[i] = string.IsNullOrWhiteSpace(value)
                        ? string.Empty
                        : value!;
                }

                return (true, (null, null, names));
            }

            // Some other type, weird, shouldn't be able to get this
            return (false, default);
        }
    }

    private static string GetNameSpace(StructDeclarationSyntax structSymbol)
    {
        // determine the namespace the struct is declared in, if any
        SyntaxNode? potentialNamespaceParent = structSymbol.Parent;
        while (potentialNamespaceParent != null &&
               potentialNamespaceParent is not NamespaceDeclarationSyntax
               && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
        {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }

        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
        {
            string nameSpace = namespaceParent.Name.ToString();
            while (true)
            {
                if(namespaceParent.Parent is not NamespaceDeclarationSyntax namespaceParentParent)
                {
                    break;
                }

                namespaceParent = namespaceParentParent;
                nameSpace = $"{namespaceParent.Name}.{nameSpace}";
            }

            return nameSpace;
        }
        return string.Empty;
    }

    private static ParentClass? GetParentClasses(StructDeclarationSyntax structSymbol)
    {
        TypeDeclarationSyntax? parentIdClass = structSymbol.Parent as TypeDeclarationSyntax;
        ParentClass? parentClass = null;

        while (parentIdClass != null && IsAllowedKind(parentIdClass.Kind()))
        {
            var keyword = parentIdClass is RecordDeclarationSyntax record
                ? record.ClassOrStructKeyword.Kind() switch
                {
                    SyntaxKind.StructKeyword => "record struct",
                    SyntaxKind.ClassKeyword => "record class",
                    _ => "record",
                }
                : parentIdClass.Keyword.ValueText;

            parentClass = new ParentClass(
                Modifiers: parentIdClass.Modifiers.ToString(),
                Keyword: keyword,
                Name: parentIdClass.Identifier.ToString() + parentIdClass.TypeParameterList,
                Constraints: parentIdClass.ConstraintClauses.ToString(),
                Child: parentClass,
                IsGeneric: parentIdClass.Arity > 0);

            parentIdClass = (parentIdClass.Parent as TypeDeclarationSyntax);
        }

        return parentClass;

        static bool IsAllowedKind(SyntaxKind kind) =>
            kind == SyntaxKind.ClassDeclaration ||
            kind == SyntaxKind.StructDeclaration ||
            kind == SyntaxKind.RecordStructDeclaration ||
            kind == SyntaxKind.RecordDeclaration;
    }
}