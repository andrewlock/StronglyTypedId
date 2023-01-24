using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        StronglyTypedIdConfiguration? config = null;

        foreach (AttributeData attribute in structSymbol.GetAttributes())
        {
            if (!((attribute.AttributeClass?.Name == "StronglyTypedIdAttribute" ||
                  attribute.AttributeClass?.Name == "StronglyTypedId") &&
                  attribute.AttributeClass.ToDisplayString() == StronglyTypedIdAttribute))
            {
                // wrong attribute
                continue;
            }

            StronglyTypedIdBackingType backingType = StronglyTypedIdBackingType.Default;
            StronglyTypedIdConverter converter = StronglyTypedIdConverter.Default;
            StronglyTypedIdImplementations implementations = StronglyTypedIdImplementations.Default;

            if (!attribute.ConstructorArguments.IsEmpty)
            {
                // make sure we don't have any errors
                ImmutableArray<TypedConstant> args = attribute.ConstructorArguments;

                foreach (TypedConstant arg in args)
                {
                    if (arg.Kind == TypedConstantKind.Error)
                    {
                        // have an error, so don't try and do any generation
                        hasMisconfiguredInput = true;
                    }
                }

                switch (args.Length)
                {
                    case 3:
                        implementations = (StronglyTypedIdImplementations)args[2].Value!;
                        goto case 2;
                    case 2:
                        converter = (StronglyTypedIdConverter)args[1].Value!;
                        goto case 1;
                    case 1:
                        backingType = (StronglyTypedIdBackingType)args[0].Value!;
                        break;
                }
            }

            if (!attribute.NamedArguments.IsEmpty)
            {
                foreach (KeyValuePair<string, TypedConstant> arg in attribute.NamedArguments)
                {
                    TypedConstant typedConstant = arg.Value;
                    if (typedConstant.Kind == TypedConstantKind.Error)
                    {
                        hasMisconfiguredInput = true;
                    }
                    else
                    {
                        switch (arg.Key)
                        {
                            case "backingType":
                                backingType = (StronglyTypedIdBackingType)typedConstant.Value!;
                                break;
                            case "converters":
                                converter = (StronglyTypedIdConverter)typedConstant.Value!;
                                break;
                            case "implementations":
                                implementations = (StronglyTypedIdImplementations)typedConstant.Value!;
                                break;
                        }
                    }
                }
            }

            if (hasMisconfiguredInput)
            {
                // skip further generator execution and let compiler generate the errors
                break;
            }

            if (!converter.IsValidFlags())
            {
                diagnostics ??= new();
                diagnostics.Add(InvalidConverterDiagnostic.CreateInfo(structSyntax));
            }

            if (!Enum.IsDefined(typeof(StronglyTypedIdBackingType), backingType))
            {
                diagnostics ??= new();
                diagnostics.Add(InvalidBackingTypeDiagnostic.CreateInfo(structSyntax));
            }

            if (!implementations.IsValidFlags())
            {
                diagnostics ??= new();
                diagnostics.Add(InvalidImplementationsDiagnostic.CreateInfo(structSyntax));
            }

            config = new StronglyTypedIdConfiguration(backingType, converter, implementations);
            break;
        }

        if (config is null || hasMisconfiguredInput)
        {
            var errors = diagnostics is null
                ? EquatableArray<DiagnosticInfo>.Empty
                : new EquatableArray<DiagnosticInfo>(diagnostics.ToArray());
            return new Result<(StructToGenerate, bool)>((default, false), errors);
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

        string nameSpace = GetNameSpace(structSyntax);
        ParentClass? parentClass = GetParentClasses(structSyntax);
        var name = structSymbol.Name;

        var errs = diagnostics is null
            ? EquatableArray<DiagnosticInfo>.Empty
            : new EquatableArray<DiagnosticInfo>(diagnostics.ToArray());
        var toGenerate = new StructToGenerate(name: name, nameSpace: nameSpace, config: config.Value, parent: parentClass);
        return new Result<(StructToGenerate, bool)>((toGenerate, true), errs);
    }

    public static Result<(StronglyTypedIdConfiguration defaults, bool valid)> GetDefaults(
        GeneratorAttributeSyntaxContext ctx, CancellationToken ct)
    {
        var assemblyAttributes = ctx.TargetSymbol.GetAttributes();
        if (assemblyAttributes.IsDefaultOrEmpty)
        {
            return Result<StronglyTypedIdConfiguration>.Fail();
        }

        // We only return the first config that we find
        StronglyTypedIdConfiguration? config = null;
        List<DiagnosticInfo>? diagnostics = null;

        foreach (AttributeData attribute in assemblyAttributes)
        {
            if (!((attribute.AttributeClass?.Name == "StronglyTypedIdDefaultsAttribute" ||
                   attribute.AttributeClass?.Name == "StronglyTypedIdDefaults") &&
                  attribute.AttributeClass.ToDisplayString() == StronglyTypedIdDefaultsAttribute))
            {
                // wrong attribute
                continue;
            }

            if (config.HasValue)
            {
                if (attribute.ApplicationSyntaxReference?.GetSyntax() is { } s)
                {
                    diagnostics ??= new();
                    diagnostics.Add(MultipleAssemblyAttributeDiagnostic.CreateInfo(s));
                }

                continue;
            }

            StronglyTypedIdBackingType backingType = StronglyTypedIdBackingType.Default;
            StronglyTypedIdConverter converter = StronglyTypedIdConverter.Default;
            StronglyTypedIdImplementations implementations = StronglyTypedIdImplementations.Default;
            bool hasMisconfiguredInput = false;
        
            if (!attribute.ConstructorArguments.IsEmpty)
            {
                // make sure we don't have any errors
                ImmutableArray<TypedConstant> args = attribute.ConstructorArguments;
            
                foreach (TypedConstant arg in args)
                {
                    if (arg.Kind == TypedConstantKind.Error)
                    {
                        // have an error, so don't try and do any generation
                        hasMisconfiguredInput = true;
                    }
                }
            
                switch (args.Length)
                {
                    case 3:
                        implementations = (StronglyTypedIdImplementations)args[2].Value!;
                        goto case 2;
                    case 2:
                        converter = (StronglyTypedIdConverter)args[1].Value!;
                        goto case 1;
                    case 1:
                        backingType = (StronglyTypedIdBackingType)args[0].Value!;
                        break;
                }
            }
            
            if (!attribute.NamedArguments.IsEmpty)
            {
                foreach (KeyValuePair<string, TypedConstant> arg in attribute.NamedArguments)
                {
                    TypedConstant typedConstant = arg.Value;
                    if (typedConstant.Kind == TypedConstantKind.Error)
                    {
                        hasMisconfiguredInput = true;
                    }
                    else
                    {
                        switch (arg.Key)
                        {
                            case "backingType":
                                backingType = (StronglyTypedIdBackingType)typedConstant.Value!;
                                break;
                            case "converters":
                                converter = (StronglyTypedIdConverter)typedConstant.Value!;
                                break;
                            case "implementations":
                                implementations = (StronglyTypedIdImplementations)typedConstant.Value!;
                                break;
                        }
                    }
                }
            }
            
            if (hasMisconfiguredInput)
            {
                // skip further generator execution and let compiler generate the errors
                break;
            }
        
            SyntaxNode? syntax = null;

            if (!converter.IsValidFlags())
            {
                syntax = attribute.ApplicationSyntaxReference?.GetSyntax();
                if (syntax is not null)
                {
                    diagnostics ??= new();
                    diagnostics.Add(InvalidConverterDiagnostic.CreateInfo(syntax));
                }
            }
        
            if (!Enum.IsDefined(typeof(StronglyTypedIdBackingType), backingType))
            {
                syntax ??= attribute.ApplicationSyntaxReference?.GetSyntax();
                if (syntax is not null)
                {
                    diagnostics ??= new();
                    diagnostics.Add(InvalidBackingTypeDiagnostic.CreateInfo(syntax));
                }
            }
        
            if (!implementations.IsValidFlags())
            {
                syntax ??= attribute.ApplicationSyntaxReference?.GetSyntax();
                if (syntax is not null)
                {
                    diagnostics ??= new();
                    diagnostics.Add(InvalidImplementationsDiagnostic.CreateInfo(syntax));
                }
            }

            config = new StronglyTypedIdConfiguration(backingType, converter, implementations);
        }

        var errors = diagnostics is null
            ? EquatableArray<DiagnosticInfo>.Empty
            : new EquatableArray<DiagnosticInfo>(diagnostics.ToArray());

        return config.HasValue
            ? new Result<(StronglyTypedIdConfiguration, bool)>((config.Value, true), errors)
            : Result<StronglyTypedIdConfiguration>.Fail();
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
            parentClass = new ParentClass(
                keyword: parentIdClass.Keyword.ValueText,
                name: parentIdClass.Identifier.ToString() + parentIdClass.TypeParameterList,
                constraints: parentIdClass.ConstraintClauses.ToString(),
                child: parentClass);

            parentIdClass = (parentIdClass.Parent as TypeDeclarationSyntax);
        }

        return parentClass;

        static bool IsAllowedKind(SyntaxKind kind) =>
            kind == SyntaxKind.ClassDeclaration ||
            kind == SyntaxKind.StructDeclaration ||
            kind == SyntaxKind.RecordDeclaration;
    }
}