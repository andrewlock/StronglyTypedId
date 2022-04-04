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

    public static bool IsStructTargetForGeneration(SyntaxNode node)
        => node is StructDeclarationSyntax m && m.AttributeLists.Count > 0;

    public static bool IsAttributeTargetForGeneration(SyntaxNode node)
        => node is AttributeListSyntax attributeList
           && attributeList.Target is not null
           && attributeList.Target.Identifier.IsKind(SyntaxKind.AssemblyKeyword);

    public static StructDeclarationSyntax? GetStructSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        // we know the node is a EnumDeclarationSyntax thanks to IsSyntaxTargetForGeneration
        var structDeclarationSyntax = (StructDeclarationSyntax)context.Node;

        // loop through all the attributes on the method
        foreach (AttributeListSyntax attributeListSyntax in structDeclarationSyntax.AttributeLists)
        {
            foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
            {
                if (ModelExtensions.GetSymbolInfo(context.SemanticModel, attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    // weird, we couldn't get the symbol, ignore it
                    continue;
                }

                INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                string fullName = attributeContainingTypeSymbol.ToDisplayString();

                // Is the attribute the [StronglyTypedId] attribute?
                if (fullName == StronglyTypedIdAttribute)
                {
                    // return the enum
                    return structDeclarationSyntax;
                }
            }
        }

        // we didn't find the attribute we were looking for
        return null;
    }

    public static AttributeSyntax? GetAssemblyAttributeSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        // we know the node is a AttributeListSyntax thanks to IsSyntaxTargetForGeneration
        var attributeListSyntax = (AttributeListSyntax)context.Node;

        // loop through all the attributes in the list
        foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
        {
            if (ModelExtensions.GetSymbolInfo(context.SemanticModel, attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
            {
                // weird, we couldn't get the symbol, ignore it
                continue;
            }

            INamedTypeSymbol attributeContainingTypeSymbol = attributeSymbol.ContainingType;
            string fullName = attributeContainingTypeSymbol.ToDisplayString();

            // Is the attribute the [StronglyTypedIdDefaultsAttribute] attribute?
            if (fullName == StronglyTypedIdDefaultsAttribute)
            {
                // return the attribute
                return attributeSyntax;
            }
        }

        // we didn't find the attribute we were looking for
        return null;
    }

    public static List<(string Name, string NameSpace, StronglyTypedIdConfiguration Config, ParentClass? Parent)> GetTypesToGenerate(
        Compilation compilation,
        ImmutableArray<StructDeclarationSyntax> targets,
        Action<Diagnostic> reportDiagnostic,
        CancellationToken ct)
    {
        var idsToGenerate = new List<(string Name, string NameSpace, StronglyTypedIdConfiguration Config, ParentClass? Parent)>();
        INamedTypeSymbol? idAttribute = compilation.GetTypeByMetadataName(StronglyTypedIdAttribute);
        if (idAttribute == null)
        {
            // nothing to do if this type isn't available
            return idsToGenerate;
        }

        foreach (StructDeclarationSyntax structDeclarationSyntax in targets)
        {
            // stop if we're asked to
            ct.ThrowIfCancellationRequested();

            SemanticModel semanticModel = compilation.GetSemanticModel(structDeclarationSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(structDeclarationSyntax) is not INamedTypeSymbol structSymbol)
            {
                // something went wrong
                continue;
            }

            StronglyTypedIdConfiguration? config = null;
            var hasMisconfiguredInput = false;

            foreach (AttributeData attribute in structSymbol.GetAttributes())
            {
                if (!idAttribute.Equals(attribute.AttributeClass, SymbolEqualityComparer.Default))
                {
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
                    reportDiagnostic(InvalidConverterDiagnostic.Create(structDeclarationSyntax));
                }

                if (!Enum.IsDefined(typeof(StronglyTypedIdBackingType), backingType))
                {
                    reportDiagnostic(InvalidBackingTypeDiagnostic.Create(structDeclarationSyntax));
                }

                if (!implementations.IsValidFlags())
                {
                    reportDiagnostic(InvalidImplementationsDiagnostic.Create(structDeclarationSyntax));
                }

                config = new StronglyTypedIdConfiguration(backingType, converter, implementations);
                break;
            }

            if (config is null || hasMisconfiguredInput)
            {
                continue; // name clash, or error
            }

            var hasPartialModifier = false;
            foreach (var modifier in structDeclarationSyntax.Modifiers)
            {
                if (modifier.IsKind(SyntaxKind.PartialKeyword))
                {
                    hasPartialModifier = true;
                    break;
                }
            }

            if (!hasPartialModifier)
            {
                reportDiagnostic(NotPartialDiagnostic.Create(structDeclarationSyntax));
            }

            string nameSpace = GetNameSpace(structDeclarationSyntax);
            var parentClass = GetParentClasses(structDeclarationSyntax);
            var name = structSymbol.Name;

            idsToGenerate.Add((Name: name, NameSpace: nameSpace, Config: config.Value, Parent: parentClass));
        }

        return idsToGenerate;
    }

    public static StronglyTypedIdConfiguration? GetDefaults(
        ImmutableArray<AttributeSyntax> defaults,
        Compilation compilation,
        Action<Diagnostic> reportDiagnostic)
    {
        if (defaults.IsDefaultOrEmpty)
        {
            // No global defaults
            return null;
        }

        var assemblyAttributes = compilation.Assembly.GetAttributes();
        if (assemblyAttributes.IsDefaultOrEmpty)
        {
            return null;
        }

        INamedTypeSymbol? defaultsAttribute = compilation.GetTypeByMetadataName(StronglyTypedIdDefaultsAttribute);
        if (defaultsAttribute is null)
        {
            // The attribute isn't part of the compilation for some reason...
            return null;
        }

        foreach (AttributeData attribute in assemblyAttributes)
        {
            if (!defaultsAttribute.Equals(attribute.AttributeClass, SymbolEqualityComparer.Default))
            {
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
                    reportDiagnostic(InvalidConverterDiagnostic.Create(syntax));
                }
            }

            if (!Enum.IsDefined(typeof(StronglyTypedIdBackingType), backingType))
            {
                syntax ??= attribute.ApplicationSyntaxReference?.GetSyntax();
                if (syntax is not null)
                {
                    reportDiagnostic(InvalidBackingTypeDiagnostic.Create(syntax));
                }
            }

            if (!implementations.IsValidFlags())
            {
                syntax ??= attribute.ApplicationSyntaxReference?.GetSyntax();
                if (syntax is not null)
                {
                    reportDiagnostic(InvalidImplementationsDiagnostic.Create(syntax));
                }
            }

            return new StronglyTypedIdConfiguration(backingType, converter, implementations);
        }

        return null;
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