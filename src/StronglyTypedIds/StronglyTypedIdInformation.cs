using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using IdInfo = System.Collections.Immutable.ImmutableDictionary<Microsoft.CodeAnalysis.ITypeSymbol,
    (bool GenerateJsonConverter, StronglyTypedIds.StronglyTypedIdBackingType BackingType, StronglyTypedIds.StronglyTypedIdJsonConverter JsonConverter)>;

namespace StronglyTypedIds
{
    internal class StronglyTypedIdInformation
    {
        private StronglyTypedIdInformation(IdInfo ids)
        {
            Ids = ids;
        }

        public IdInfo Ids { get; set; }

        public static StronglyTypedIdInformation Create(StronglyTypedIdReceiver receiver, Compilation compilation)
        {
            var builder = ImmutableDictionary.CreateBuilder<ITypeSymbol, (bool, StronglyTypedIdBackingType, StronglyTypedIdJsonConverter)>(SymbolEqualityComparer.Default);
            PopulateIdInfo(receiver.StronglyTypedIdStructs, compilation, builder);
            return new StronglyTypedIdInformation(builder.ToImmutable());
        }

        private static void PopulateIdInfo(
            List<StructDeclarationSyntax> structDeclarations,
            Compilation compilation,
            IdInfo.Builder idInfo)
        {
            var stronglyTypedIdAttributeSymbol = compilation.GetTypeByMetadataName(Constants.FullyQualifiedTagNameAttribute);
            if (stronglyTypedIdAttributeSymbol is null)
            {
                // The attribute isn't part of the compilation for some reason...
                return;
            }

            foreach (var structDeclaration in structDeclarations)
            {
                var model = compilation.GetSemanticModel(structDeclaration.SyntaxTree);
                var source = model.GetDeclaredSymbol(structDeclaration);

                if (source is ITypeSymbol structSymbol)
                {
                    var stronglyTypedIdAttribute = source.GetAttributes().FirstOrDefault(
                        x => x.AttributeClass!.Equals(stronglyTypedIdAttributeSymbol, SymbolEqualityComparer.Default));

                    if (stronglyTypedIdAttribute is null)
                    {
                        return; // name clash, or error
                    }

                    var positionalArgs = stronglyTypedIdAttribute.ConstructorArguments;
                    var namedArgs = stronglyTypedIdAttribute.NamedArguments;
                    var generateJsonConverter = GetValueFromConstructorArguments(positionalArgs, namedArgs, true, 0, "generateJsonConverter");
                    var backingType = GetValueFromConstructorArguments(positionalArgs, namedArgs, StronglyTypedIdBackingType.Guid, 1, "backingType");
                    var jsonConverter = GetValueFromConstructorArguments(positionalArgs, namedArgs, StronglyTypedIdJsonConverter.NewtonsoftJson, 2, "backingType");

                    idInfo.Add(structSymbol, (generateJsonConverter, backingType, jsonConverter));
                }
            }
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
