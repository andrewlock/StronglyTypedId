using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using StronglyTypedIds.Sources;

namespace StronglyTypedIds
{
    /// <inheritdoc />
    [Generator]
    public class StronglyTypedIdGenerator : ISourceGenerator
    {
        /// <inheritdoc />
        public void Initialize(GeneratorInitializationContext context)
        {
            // Register the attribute and enum sources
            context.RegisterForPostInitialization((i) =>
            {
                i.AddSource("StronglyTypedIdAttribute", EmbeddedSources.StronglyTypedIdAttributeSource);
                i.AddSource("StronglyTypedIdDefaultsAttribute", EmbeddedSources.StronglyTypedIdDefaultsAttributeSource);
                i.AddSource("StronglyTypedIdBackingType", EmbeddedSources.StronglyTypedIdBackingTypeSource);
                i.AddSource("StronglyTypedIdConverter", EmbeddedSources.StronglyTypedIdConverterSource);
            });

            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new StronglyTypedIdReceiver());
        }

        /// <inheritdoc />
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not StronglyTypedIdReceiver receiver)
            {
                return;
            }

            var compilation = context.Compilation;
            var results = GenerateStronglyTypedId(receiver, compilation, context.AnalyzerConfigOptions);

            foreach (var result in results)
            {
                foreach (var diagnostic in result.Diagnostics)
                {
                    context.ReportDiagnostic(diagnostic);
                }

                if (result.SourceName is not null && result.SourceText is not null)
                {
                    context.AddSource(result.SourceName, result.SourceText);
                }
            }
        }

        private static ImmutableArray<GenerationResult> GenerateStronglyTypedId(
            StronglyTypedIdReceiver receiver,
            Compilation compilation,
            AnalyzerConfigOptionsProvider optionsProvider)
        {
            var results = ImmutableArray.CreateBuilder<GenerationResult>();
            var information = StronglyTypedIdInformation.Create(receiver, compilation);
            var globalDefaults = information.Defaults.Defaults ;
            if (!information.Defaults.Diagnostics.IsDefaultOrEmpty)
            {
                results.Add(new GenerationResult(information.Defaults.Diagnostics, null, null));
            }

            foreach (var stronglyTypedIdInfo in information.Ids)
            {
                var structType = stronglyTypedIdInfo.Key;
                var info = stronglyTypedIdInfo.Value;

                var classNameSpace = structType.ContainingNamespace.IsGlobalNamespace
                    ? string.Empty
                    : structType.ContainingNamespace.ToDisplayString();

                var className = structType.Name;

                var values = StronglyTypedIdConfiguration.Combine(info.Configuration, globalDefaults);
                var source = SourceGenerationHelper.CreateId(classNameSpace, className, values.Converters, values.BackingType);

                if (!string.IsNullOrEmpty(source))
                {
                    var sourceText = SourceText.From(source, Encoding.UTF8);
                    var sourceFileName = $"{className}_id.g.cs";
                    results.Add(new GenerationResult(info.Diagnostics, sourceFileName, sourceText));
                }
            }

            return results.ToImmutable();
        }

        private class GenerationResult
        {
            public GenerationResult(ImmutableArray<Diagnostic> diagnostics, string? sourceName, SourceText? sourceText)
            {
                Diagnostics = diagnostics;
                SourceName = sourceName;
                SourceText = sourceText;
            }

            public ImmutableArray<Diagnostic> Diagnostics { get; }

            public string? SourceName { get; }

            public SourceText? SourceText { get; }
        }
    }
}