using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StronglyTypedIds.Diagnostics;
using StronglyTypedIds.Sources;
using IdInfo = System.Collections.Immutable.ImmutableDictionary<Microsoft.CodeAnalysis.ITypeSymbol,
    (System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Diagnostics,
    StronglyTypedIds.StronglyTypedIdConfiguration Configuration)>;

namespace StronglyTypedIds
{
    internal class StronglyTypedIdInformation
    {
        private StronglyTypedIdInformation(IdInfo ids, (ImmutableArray<Diagnostic>, StronglyTypedIdConfiguration?) defaults)
        {
            Ids = ids;
            Defaults = defaults;
        }

        public IdInfo Ids { get; }
        public (ImmutableArray<Diagnostic> Diagnostics, StronglyTypedIdConfiguration? Defaults) Defaults { get; }

        public static StronglyTypedIdInformation Create(StronglyTypedIdReceiver receiver, Compilation compilation)
        {
            var infoBuilder = ImmutableDictionary.CreateBuilder<ITypeSymbol, (ImmutableArray<Diagnostic>, StronglyTypedIdConfiguration)>(SymbolEqualityComparer.Default);
            PopulateIdInfo(receiver.Targets, compilation, infoBuilder);
            var defaults = GetDefaults(compilation);
            return new StronglyTypedIdInformation(infoBuilder.ToImmutable(), defaults);
        }

        private static void PopulateIdInfo(
            List<(SyntaxNode Origin, StructDeclarationSyntax Declaration)> targets,
            Compilation compilation,
            IdInfo.Builder idInfo)
        {
            var stronglyTypedIdAttributeSymbol = compilation.GetTypeByMetadataName(Constants.FullyQualifiedStronglyTypedIdAttribute);
            if (stronglyTypedIdAttributeSymbol is null)
            {
                // The attribute isn't part of the compilation for some reason...
                return;
            }

            foreach (var target in targets)
            {
                var structDeclaration = target.Declaration;
                var model = compilation.GetSemanticModel(structDeclaration.SyntaxTree);
                var source = ModelExtensions.GetDeclaredSymbol(model, structDeclaration);
                var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

                if (source is ITypeSymbol structSymbol)
                {
                    var stronglyTypedIdAttribute = source.GetAttributes().FirstOrDefault(
                        x => x.AttributeClass!.Equals(stronglyTypedIdAttributeSymbol, SymbolEqualityComparer.Default));

                    if (stronglyTypedIdAttribute is null)
                    {
                        return; // name clash, or error
                    }

                    if (!structDeclaration.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword))
                        && structSymbol.Locations.Length == 1)
                    {
                        diagnostics.Add(NotPartialDiagnostic.Create(target.Origin));
                    }

                    if (structSymbol.ContainingType is not null)
                    {
                        diagnostics.Add(NestedTypeDiagnostic.Create(target.Origin));
                    }

                    var positionalArgs = stronglyTypedIdAttribute.ConstructorArguments;
                    var namedArgs = stronglyTypedIdAttribute.NamedArguments;
                    var backingType = GetValueFromConstructorArguments(positionalArgs, namedArgs, StronglyTypedIdBackingType.Default, 0, "backingType");
                    var converter = GetValueFromConstructorArguments(positionalArgs, namedArgs, StronglyTypedIdConverter.Default, 1, "converters");

                    if (!converter.IsValidFlags())
                    {
                        diagnostics.Add(InvalidConverterDiagnostic.Create(target.Origin));
                    }

                    idInfo.Add(structSymbol, (diagnostics.ToImmutable(), new StronglyTypedIdConfiguration(backingType, converter)));
                }
            }
        }

        private static (ImmutableArray<Diagnostic>, StronglyTypedIdConfiguration?) GetDefaults(Compilation compilation)
        {
            var stronglyTypedIdDefaultsAttributeSymbol = compilation.GetTypeByMetadataName(Constants.FullyQualifiedStronglyTypedIdDefaultsAttribute);
            if (stronglyTypedIdDefaultsAttributeSymbol is null)
            {
                // The attribute isn't part of the compilation for some reason...
                return (ImmutableArray<Diagnostic>.Empty, null);
            }

            var assemblyAttribute = compilation.Assembly
                .GetAttributes()
                .FirstOrDefault(x => x.AttributeClass is not null
                                     && x.AttributeClass.Equals(stronglyTypedIdDefaultsAttributeSymbol, SymbolEqualityComparer.Default));

            if (assemblyAttribute is null)
            {
                // No global defaults
                return (ImmutableArray<Diagnostic>.Empty, null);
            }
            var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

            var positionalArgs = assemblyAttribute.ConstructorArguments;
            var namedArgs = assemblyAttribute.NamedArguments;
            // these values must mach the defaults
            var backingType = GetValueFromConstructorArguments(positionalArgs, namedArgs, StronglyTypedIdBackingType.Default, 0, "backingType");
            var converter = GetValueFromConstructorArguments(positionalArgs, namedArgs, StronglyTypedIdConverter.Default, 1, "converters");

            if (!converter.IsValidFlags())
            {
                diagnostics.Add(InvalidConverterDiagnostic.Create(assemblyAttribute.ApplicationSyntaxReference.GetSyntax()));
            }

            return (diagnostics.ToImmutable(), new StronglyTypedIdConfiguration(backingType, converter));
        }

        static T GetValueFromConstructorArguments<T>(
            ImmutableArray<TypedConstant> positionalArgs,
            ImmutableArray<KeyValuePair<string, TypedConstant>> namedArgs,
            T defaultValue,
            int positionalIndex,
            string namedArgName)
        {
            if (positionalArgs.Length > positionalIndex)
            {
                return (T) positionalArgs[positionalIndex].Value!;
            }

            var maybeKeyValue = namedArgs.FirstOrDefault(x => x.Key == namedArgName);

            if (maybeKeyValue.Key == namedArgName)
            {
                return (T) maybeKeyValue.Value.Value!;
            }

            return defaultValue;
        }
    }
}
