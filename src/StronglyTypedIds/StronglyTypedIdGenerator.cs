using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
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
        const string TemplateSuffix = ".typedid"; 

        /// <inheritdoc />
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Register the attribute and enum sources
            context.RegisterPostInitializationOutput(i =>
            {
                i.AddSource("StronglyTypedIdAttribute.g.cs", EmbeddedSources.StronglyTypedIdAttributeSource);
                i.AddSource("StronglyTypedIdDefaultsAttribute.g.cs", EmbeddedSources.StronglyTypedIdDefaultsAttributeSource);
                i.AddSource("Template.g.cs", EmbeddedSources.TemplateSource);
            });

            IncrementalValuesProvider<(string Path, string Name, string? Content)> allTemplates = context.AdditionalTextsProvider
                .Where(template => Path.GetExtension(template.Path).Equals(TemplateSuffix, StringComparison.OrdinalIgnoreCase))
                .Select((template, ct) => (
                    Path: template.Path,
                    Name: Path.GetFileNameWithoutExtension(template.Path),
                    Content: template.GetText(ct)?.ToString()));

            var templatesWithErrors = allTemplates
                .Where(template => string.IsNullOrWhiteSpace(template.Name) || template.Content is null);

            var templates = allTemplates
                .Where(template => !string.IsNullOrWhiteSpace(template.Name) && template.Content is not null)
                .Collect();
                
            IncrementalValuesProvider<Result<(StructToGenerate info, bool valid)>> structAndDiagnostics = context.SyntaxProvider
                .ForAttributeWithMetadataName(
                    Parser.StronglyTypedIdAttribute,
                    predicate: (node, _) => node is StructDeclarationSyntax,
                    transform: Parser.GetStructSemanticTarget)
                .Where(static m => m is not null);

            IncrementalValuesProvider<Result<(Defaults defaults, bool valid)>> defaultsAndDiagnostics = context.SyntaxProvider
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

            // context.RegisterSourceOutput(
            //     templatesWithErrors,
            //     static (context, info) => context.ReportDiagnostic(Diagnostic.Create(info.Descriptor, info.Location)));

            IncrementalValuesProvider<StructToGenerate> structs = structAndDiagnostics
                .Where(static x => x.Value.valid)
                .Select((result, _) => result.Value.info);

            
            IncrementalValueProvider<string?> defaultTemplateContent = defaultsAndDiagnostics
                .Where(static x => x.Value.valid)
                .Select((result, _) => result.Value.defaults)
                .Collect()
                .Combine(templates)
                .Select((all, _) =>
                {
                    // we can only use one default attribute
                    // more than one is an error, but lets do our best
                    var defaults = all.Left.IsDefaultOrEmpty ? (Defaults?) null : all.Left[0];
                    if (defaults?.Template is { } templateId)
                    {
                        return EmbeddedSources.GetTemplate(templateId);
                    }

                    var defaultsTemplateName = defaults?.TemplateName;
                    if(!string.IsNullOrEmpty(defaultsTemplateName))
                    {
                        foreach (var templateDetails in all.Right)
                        {
                            if (string.Equals(templateDetails.Name, defaultsTemplateName, StringComparison.Ordinal))
                            {
                                // This _could_ be empty, but we use it anyway (and add a warning in the template)
                                return templateDetails.Content;
                            }
                        }

                        // TODO: Add diagnostic that couldn't find a template with the right name
                        // TODO: Add a warning in a comment to the code too?
                        // context.ReportDiagnostic();
                    }

                    // no default
                    return null;
                });

            var structsWithDefaultsAndTemplates = structs
                .Combine(templates)
                .Combine(defaultTemplateContent);

            context.RegisterSourceOutput(structsWithDefaultsAndTemplates,
                static (spc, source) => Execute(source.Left.Left, source.Left.Right, source.Right, spc));
        }

        private static void Execute(
            StructToGenerate idToGenerate,
            ImmutableArray<(string Path, string Name, string? Content)> templates,
            string? defaultTemplateContent, 
            SourceProductionContext context)
        {
            var sb = new StringBuilder();

            string? template = null!;
            if (idToGenerate.Template is { } templateId)
            {
                template = EmbeddedSources.GetTemplate(templateId);
            }
            else if(!string.IsNullOrEmpty(idToGenerate.TemplateName))
            {
                foreach (var templateDetails in templates)
                {
                    if (string.Equals(templateDetails.Name, idToGenerate.TemplateName, StringComparison.Ordinal))
                    {
                        template = templateDetails.Content;
                        break;
                    }
                }

                // TODO: Add diagnostic that couldn't find a template with the right name
                // context.ReportDiagnostic();
            }

            if (string.IsNullOrEmpty(template))
            {
                template = defaultTemplateContent ?? EmbeddedSources.GetTemplate(Template.Guid);
            }

            var result = SourceGenerationHelper.CreateId(
                idToGenerate.NameSpace,
                idToGenerate.Name,
                idToGenerate.Parent,
                template!,
                sb);

            var fileName = SourceGenerationHelper.CreateSourceName(
                idToGenerate.NameSpace,
                idToGenerate.Parent,
                idToGenerate.Name);
            context.AddSource(fileName, SourceText.From(result, Encoding.UTF8));
        }
    }
}