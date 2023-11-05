using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using StronglyTypedIds.Diagnostics;

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

            IncrementalValuesProvider<StructToGenerate> structs = structAndDiagnostics
                .Where(static x => x.Value.valid)
                .Select((result, _) => result.Value.info);

            IncrementalValueProvider<(string? Content, bool isValid, DiagnosticInfo? Diagnostic)> defaultTemplateContent = defaultsAndDiagnostics
                .Where(static x => x.Value.valid)
                .Select((result, _) => result.Value.defaults)
                .Collect()
                .Combine(templates)
                .Select((all, _) =>
                {
                    if (all.Left.IsDefaultOrEmpty)
                    {
                        // no default attributes, valid, but no content
                        return (null, true, null);
                    }

                    // technically we can never have more than one `Defaults` here
                    // but check for it just in case
                    if (all.Left is {IsDefaultOrEmpty: false, Length: > 1})
                    {
                        return (null, false, null);
                    }

                    var defaults = all.Left[0];
                    if (defaults.HasMultiple)
                    {
                        // not valid
                        return (null, false, null);
                    }

                    if (defaults.Template is { } templateId)
                    {
                        // Explicit template
                        return (EmbeddedSources.GetTemplate(templateId), true, (DiagnosticInfo?) null);
                    }

                    // We have already checked for a null template name and flagged it as an error
                    var defaultsTemplateName = defaults.TemplateName;
                    if(!string.IsNullOrEmpty(defaultsTemplateName))
                    {
                        foreach (var templateDetails in all.Right)
                        {
                            if (string.Equals(templateDetails.Name, defaultsTemplateName, StringComparison.Ordinal))
                            {
                                // This _could_ be empty, but we use it anyway (and add a comment in the template)
                                return (templateDetails.Content ?? GetEmptyTemplateContent(defaultsTemplateName!), true, null);
                            }
                        }
                    
                        // Template name specified, but we don't have a template for it
                        return (null, false, UnknownTemplateDiagnostic.CreateInfo(defaults!.TemplateLocation!, defaultsTemplateName!));
                    }
            
                    // only get here if the template name was null/empty, which is already reported
                    return (null, false, null);
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
            (string? Content, bool IsValid, DiagnosticInfo? Diagnostics) defaults, 
            SourceProductionContext context)
        {
            var sb = new StringBuilder();
            if (defaults.Diagnostics is { } diagnostic)
            {
                // report error with the default template
                context.ReportDiagnostic(Diagnostic.Create(diagnostic.Descriptor, diagnostic.Location));
            }

            if (!TryGetTemplateContent(idToGenerate, templates, defaults.IsValid, defaults.Content, context, out var templateContent))
            {
                return;
            }

            var result = SourceGenerationHelper.CreateId(
                idToGenerate.NameSpace,
                idToGenerate.Name,
                idToGenerate.Parent,
                templateContent,
                sb);

            var fileName = SourceGenerationHelper.CreateSourceName(
                idToGenerate.NameSpace,
                idToGenerate.Parent,
                idToGenerate.Name);
            context.AddSource(fileName, SourceText.From(result, Encoding.UTF8));
        }

        private static bool TryGetTemplateContent(
            in StructToGenerate idToGenerate,
            in ImmutableArray<(string Path, string Name, string? Content)> templates,
            bool haveValidDefault,
            string? defaultContent,
            in SourceProductionContext context,
            [NotNullWhen(true)] out string? templateContent)
        {
            // built-in template specified
            if (idToGenerate.Template is { } templateId)
            {
                templateContent = EmbeddedSources.GetTemplate(templateId);
                return true;
            }

            // custom template specified
            if (!string.IsNullOrEmpty(idToGenerate.TemplateName))
            {
                foreach (var templateDetails in templates)
                {
                    if (string.Equals(templateDetails.Name, idToGenerate.TemplateName, StringComparison.Ordinal))
                    {
                        templateContent = templateDetails.Content ?? GetEmptyTemplateContent(idToGenerate.TemplateName!);
                        return true;
                    }
                }

                // the template wasn't found, so it must be invalid. Don't generate anything
                var info = UnknownTemplateDiagnostic.CreateInfo(idToGenerate.TemplateLocation!, idToGenerate.TemplateName!);
                context.ReportDiagnostic(Diagnostic.Create(info.Descriptor, info.Location));
                templateContent = null;
                return false;
            }

            // nothing specified, use the default (if we have one)
            if (haveValidDefault)
            {
                templateContent = defaultContent ?? EmbeddedSources.GetTemplate(Template.Guid);
                return true;
            }

            templateContent = null;
            return false;
        }

        private static string GetEmptyTemplateContent(string templateName)
            => $"// The {templateName}.typeid template was empty, you'll need to add some content";
    }
}