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

    public static Result<(StructToGenerate info, bool valid)> GetStructSemanticTarget(GeneratorAttributeSyntaxContext ctx, CancellationToken ct)
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
        string? templateName = null;
        LocationInfo? templateLocation = null;

        foreach (AttributeData attribute in structSymbol.GetAttributes())
        {
            if (!((attribute.AttributeClass?.Name == "StronglyTypedIdAttribute" ||
                  attribute.AttributeClass?.Name == "StronglyTypedId") &&
                  attribute.AttributeClass.ToDisplayString() == StronglyTypedIdAttribute))
            {
                // wrong attribute
                continue;
            }

            hasMisconfiguredInput |= GetConstructorValues(attribute, out template, out templateName, out templateLocation, ref diagnostics);
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

        var toGenerate = template.HasValue
            ? new StructToGenerate(name: name, nameSpace: nameSpace, template: template.Value, parent: parentClass)
            : new StructToGenerate(name: name, nameSpace: nameSpace, templateName: templateName, templateLocation: templateLocation!, parent: parentClass);

        return new Result<(StructToGenerate, bool)>((toGenerate, true), errors);
    }

    public static Result<(Defaults defaults, bool valid)> GetDefaults(
        GeneratorAttributeSyntaxContext ctx, CancellationToken ct)
    {
        var assemblyAttributes = ctx.TargetSymbol.GetAttributes();
        if (assemblyAttributes.IsDefaultOrEmpty)
        {
            return Result<Defaults>.Fail();
        }

        // We only return the first config that we find
        string? templateName = null;
        Template? template = null;
        LocationInfo? templateLocation = null;
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

            if (!string.IsNullOrWhiteSpace(templateName) || template.HasValue)
            {
                hasMultiple = true;
                if (attribute.ApplicationSyntaxReference?.GetSyntax() is { } s)
                {
                    diagnostics ??= new();
                    diagnostics.Add(MultipleAssemblyAttributeDiagnostic.CreateInfo(s));
                }
            }

            hasMisconfiguredInput |= GetConstructorValues(attribute, out template, out templateName, out templateLocation, ref diagnostics);
        }

        var errors = diagnostics is null
            ? EquatableArray<DiagnosticInfo>.Empty
            : new EquatableArray<DiagnosticInfo>(diagnostics.ToArray());

        if (hasMisconfiguredInput)
        {
            return new Result<(Defaults, bool)>((default, false), errors);
        }

        var defaults = template.HasValue
            ? new Defaults(template.Value, hasMultiple)
            : new Defaults(templateName!, templateLocation!, hasMultiple);

        return new Result<(Defaults, bool)>((defaults, true), errors);
    }

    private static bool GetConstructorValues(AttributeData attribute, out Template? template, out string? templateName, out LocationInfo? templateLocation, ref List<DiagnosticInfo>? diagnostics)
    {
        var hasMisconfiguredInput = false;
        template = null;
        templateName = null;
        templateLocation = null;

        if (attribute.ConstructorArguments is { IsEmpty: false } args)
        {
            // make sure we don't have any errors
            foreach (TypedConstant arg in args)
            {
                if (arg.Kind == TypedConstantKind.Error)
                {
                    // have an error, so don't try and do any generation
                    hasMisconfiguredInput = true;
                    break;
                }
            }

            if (args[0].Value is int enumValue)
            {
                template = (Template) enumValue;
            }
            else
            {
                templateName = args[0].Value as string;
                var syntaxNode = attribute.ApplicationSyntaxReference?.GetSyntax(); 
                if (string.IsNullOrWhiteSpace(templateName))
                {
                    if (syntaxNode is { } s)
                    {
                        diagnostics ??= new();
                        diagnostics.Add(InvalidTemplateNameDiagnostic.CreateInfo(s));
                    }

                    hasMisconfiguredInput = true;
                }
                else
                {
                    if (syntaxNode is { } s)
                    {
                        templateLocation = LocationInfo.CreateFrom(s);
                    }
                }
            }
        }

        if (attribute.NamedArguments is { IsEmpty: false } namedArgs)
        {
            foreach (KeyValuePair<string, TypedConstant> arg in namedArgs)
            {
                TypedConstant typedConstant = arg.Value;
                if (typedConstant.Kind == TypedConstantKind.Error)
                {
                    hasMisconfiguredInput = true;
                    break;
                }

                if (typedConstant.Value is int enumValue)
                {
                    template = (Template) enumValue;
                }
                else
                {
                    templateName = typedConstant.Value as string;
                    var syntaxNode = attribute.ApplicationSyntaxReference?.GetSyntax(); 
                    if (string.IsNullOrWhiteSpace(templateName))
                    {
                        if (syntaxNode is { } s)
                        {
                            diagnostics ??= new();
                            diagnostics.Add(InvalidTemplateNameDiagnostic.CreateInfo(s));
                        }

                        hasMisconfiguredInput = true;
                    }
                    else
                    {
                        if (syntaxNode is { } s)
                        {
                            templateLocation = LocationInfo.CreateFrom(s);
                        }
                    }
                }

                break;
            }
        }

        return hasMisconfiguredInput;
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