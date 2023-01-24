using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace StronglyTypedIds
{
    /// <inheritdoc />
    [Generator]
    public class StronglyTypedIdGenerator : IIncrementalGenerator
    {
        /// <inheritdoc />
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Register the attribute and enum sources
            context.RegisterPostInitializationOutput(i =>
            {
                i.AddSource("StronglyTypedIdAttribute.g.cs", EmbeddedSources.StronglyTypedIdAttributeSource);
                i.AddSource("StronglyTypedIdDefaultsAttribute.g.cs", EmbeddedSources.StronglyTypedIdDefaultsAttributeSource);
                i.AddSource("StronglyTypedIdBackingType.g.cs", EmbeddedSources.StronglyTypedIdBackingTypeSource);
                i.AddSource("StronglyTypedIdConverter.g.cs", EmbeddedSources.StronglyTypedIdConverterSource);
                i.AddSource("StronglyTypedIdImplementations.g.cs", EmbeddedSources.StronglyTypedIdImplementationsSource);
            });

            IncrementalValuesProvider<Result<(StructToGenerate info, bool valid)>> structAndDiagnostics = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    Parser.StronglyTypedIdAttribute,
                    predicate: (node, _) => node is StructDeclarationSyntax,
                    transform: Parser.GetStructSemanticTarget)
                .Where(static m => m is not null);

            IncrementalValuesProvider<Result<(StronglyTypedIdConfiguration defaults, bool valid)>> defaultsAndDiagnostics = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    Parser.StronglyTypedIdDefaultsAttribute,
                    predicate: (node, _) => node is CompilationUnitSyntax,
                    transform: Parser.GetDefaults)
                .Where(static m => m is not null);

            context.RegisterSourceOutput(
                structAndDiagnostics.SelectMany((x, _) => x.Errors),
                static (context, info) => context.ReportDiagnostic(Diagnostic.Create(info.Descriptor, info.Location)));

            context.RegisterSourceOutput(
                defaultsAndDiagnostics.SelectMany((x, _) => x.Errors),
                static (context, info) => context.ReportDiagnostic(Diagnostic.Create(info.Descriptor, info.Location)));

            IncrementalValuesProvider<StructToGenerate> structs = structAndDiagnostics
                .Where(static x => x.Value.valid)
                .Select((result, _) => result.Value.info);

            IncrementalValueProvider<ImmutableArray<StronglyTypedIdConfiguration>> allDefaults = defaultsAndDiagnostics
                .Where(static x => x.Value.valid)
                .Select((result, _) => result.Value.defaults)
                .Collect();

            // we can only use one default attribute
            // more than one is an error, but lets do our best
            IncrementalValueProvider<StronglyTypedIdConfiguration?> selectedDefaults = allDefaults
                .Select((all, _) => all.IsDefaultOrEmpty ? (StronglyTypedIdConfiguration?)null : all[0]);

            var structsWithDefaults = structs.Combine(selectedDefaults);

            context.RegisterSourceOutput(structsWithDefaults,
                static (spc, source) => Execute(source.Left, source.Right, spc));
        }

        static void Execute(
            StructToGenerate idToGenerate,
            StronglyTypedIdConfiguration? defaults,
            SourceProductionContext context)
        {
            var sb = new StringBuilder();
            var values = StronglyTypedIdConfiguration.Combine(idToGenerate.Config, defaults);

            var result = SourceGenerationHelper.CreateId(
                idToGenerate.NameSpace,
                idToGenerate.Name,
                idToGenerate.Parent,
                values.Converters,
                values.BackingType,
                values.Implementations,
                sb);

            var fileName = SourceGenerationHelper.CreateSourceName(
                idToGenerate.NameSpace,
                idToGenerate.Parent,
                idToGenerate.Name);
            context.AddSource(fileName, SourceText.From(result, Encoding.UTF8));
        }
    }
}