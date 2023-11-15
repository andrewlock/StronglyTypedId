using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
                static (context, info) => context.ReportDiagnostic(info));

            context.RegisterSourceOutput(
                defaultsAndDiagnostics.SelectMany((x, _) => x.Errors),
                static (context, info) => context.ReportDiagnostic(info));

            IncrementalValuesProvider<StructToGenerate> structs = structAndDiagnostics
                .Where(static x => x.Value.valid)
                .Select((result, _) => result.Value.info);

            IncrementalValueProvider<(EquatableArray<(string Name, string Content)> Content, bool isValid, DiagnosticInfo? Diagnostic)> defaultTemplateContent = defaultsAndDiagnostics
                .Where(static x => x.Value.valid)
                .Select((result, _) => result.Value.defaults)
                .Collect()
                .Combine(templates)
                .Select(ProcessDefaults);

            var structsWithDefaultsAndTemplates = structs
                .Combine(templates)
                .Combine(defaultTemplateContent);

            context.RegisterSourceOutput(structsWithDefaultsAndTemplates,
                static (spc, source) => Execute(source.Left.Left, source.Left.Right, source.Right, spc));
        }
        private static void Execute(
            StructToGenerate idToGenerate,
            ImmutableArray<(string Path, string Name, string? Content)> templates,
            (EquatableArray<(string Name, string Content)>, bool IsValid, DiagnosticInfo? Diagnostic) defaults, 
            SourceProductionContext context)
        {
            if (defaults.Diagnostic is { } diagnostic)
            {
                // report error with the default template
                context.ReportDiagnostic(diagnostic);
            }

            if (!TryGetTemplateContent(idToGenerate, templates, defaults, in context, out var templateContents))
            {
                return;
            }

            var sb = new StringBuilder();
            foreach (var (name, content) in templateContents.Distinct())
            {
                var result = SourceGenerationHelper.CreateId(
                    idToGenerate.NameSpace,
                    idToGenerate.Name,
                    idToGenerate.Parent,
                    content,
                    addDefaultAttributes: string.IsNullOrEmpty(name),
                    sb);

                var fileName = SourceGenerationHelper.CreateSourceName(
                    sb,
                    idToGenerate.NameSpace,
                    idToGenerate.Parent,
                    idToGenerate.Name,
                    name);

                context.AddSource(fileName, SourceText.From(result, Encoding.UTF8));
            }
        }


        private static (EquatableArray<(string Name, string Content)>, bool, DiagnosticInfo?) ProcessDefaults((ImmutableArray<Defaults> Left, ImmutableArray<(string Path, string Name, string? Content)> Right) all, CancellationToken _)
        {
            if (all.Left.IsDefaultOrEmpty)
            {
                // no default attributes, valid, but no content
                return (EquatableArray<(string Name, string Content)>.Empty, true, null);
            }

            // technically we can never have more than one `Defaults` here
            // but check for it just in case
            if (all.Left is {IsDefaultOrEmpty: false, Length: > 1})
            {
                return (EquatableArray<(string Name, string Content)>.Empty, false, null);
            }

            var defaults = all.Left[0];
            if (defaults.HasMultiple)
            {
                // not valid
                return (EquatableArray<(string Name, string Content)>.Empty, false, null);
            }

            (string, string)? builtInTemplate = null;
            if (defaults.Template is { } templateId)
            {
                // Explicit template
                builtInTemplate = (string.Empty, EmbeddedSources.GetTemplate(templateId));
            }

            var templateNames = defaults.TemplateNames.GetArray();
            if (templateNames is null or {Length: 0})
            {
                if (builtInTemplate.HasValue)
                {
                    // valid, only built-in
                    var template = new EquatableArray<(string Name, string Content)>(new[] {builtInTemplate.Value});
                    return (template, true, (DiagnosticInfo?) null);
                }

                // not valid, need something
                return (EquatableArray<(string Name, string Content)>.Empty, false, null);
            }

            // We have already checked for null/empty template name and flagged it as an error
            if (!GetContent(templateNames, defaults.TemplateLocation!, builtInTemplate.HasValue, in all.Right, out var contents, out var diagnostic))
            {
                return (EquatableArray<(string Name, string Content)>.Empty, false, diagnostic);
            }

            if (builtInTemplate.HasValue)
            {
                contents[^1] = builtInTemplate.Value;
            }

            // Ok, we have all the templates
            return (new EquatableArray<(string Name, string Content)>(contents), true, null);
        }

        private static bool TryGetTemplateContent(
            in StructToGenerate idToGenerate,
            in ImmutableArray<(string Path, string Name, string? Content)> templates,
            (EquatableArray<(string Name, string Content)> Contents, bool IsValid, DiagnosticInfo? Diagnostics) defaults,
            in SourceProductionContext context,
            [NotNullWhen(true)] out (string Name, string Content)[]? templateContents)
        {
            (string, string)? builtIn = null;
            if (idToGenerate.Template is { } templateId)
            {
                // built-in template specified
                var content = EmbeddedSources.GetTemplate(templateId);
                builtIn = (string.Empty, content);
            }

            if (idToGenerate.TemplateNames.GetArray() is {Length: > 0} templateNames)
            {
                // custom template specified
                if (GetContent(
                        templateNames,
                        idToGenerate.TemplateLocation,
                        builtIn.HasValue,
                        in templates,
                        out templateContents,
                        out var diagnostic))
                {
                    if (builtIn.HasValue)
                    {
                        templateContents[^1] = builtIn.Value;
                    }
                    
                    return true;
                }

                // One of the templates wasn't found, so it must be invalid. Don't generate anything
                if (diagnostic is { })
                {
                    context.ReportDiagnostic(diagnostic);
                }

                templateContents = null;
                return false;
            }

            if (builtIn.HasValue)
            {
                templateContents = new[] {builtIn.Value};
                return true;
            }

            // nothing specified, use the defaults (if we have them)
            if (defaults.IsValid)
            {
                if (defaults.Contents.GetArray() is {Length: > 0} allContent)
                {
                    templateContents = allContent;
                }
                else
                {
                    templateContents = new[] {(string.Empty, EmbeddedSources.GetTemplate(Template.Guid))};
                }

                return true;
            }

            // not valid
            templateContents = null;
            return false;
        }

        private static string GetEmptyTemplateContent(string templateName)
            => $"// The {templateName}.typeid template was empty, you'll need to add some content";

        private static bool GetContent(
            string[] templateNames,
            LocationInfo? location,
            bool haveStandardTemplate,
            in ImmutableArray<(string Path, string Name, string? Content)> templates,
            [NotNullWhen(true)] out (string Name, string Content)[]? output,
            out DiagnosticInfo? diagnostic)
        {
            // Length + 1 to optionally add the extra template 
            var totalTemplates = haveStandardTemplate ? templateNames.Length + 1 : templateNames.Length;
            (string, string)[]? contents = null;
            for (int i = 0; i < templateNames.Length; i++)
            {
                var templateName = templateNames[i];
                if (string.IsNullOrEmpty(templateName))
                {
                    output = null;
                    diagnostic = location is not null
                        ? InvalidTemplateNameDiagnostic.CreateInfo(location)
                        : null;
                    return false;
                }
                
                var foundTemplate = false;

                foreach (var templateDetails in templates)
                {
                    if (string.Equals(templateDetails.Name, templateName, StringComparison.Ordinal))
                    {
                        // This _could_ be empty, but we use it anyway (and add a comment in the template)
                        var content = templateDetails.Content ?? GetEmptyTemplateContent(templateName);
                        contents ??= new (string, string)[totalTemplates];
                        contents[i] = (templateName, content);
                        foundTemplate = true;
                        break;
                    }
                }

                if (!foundTemplate)
                {
                    // Template name specified, but we don't have a template for it
                    // bail out early
                    output = null;
                    diagnostic = location is not null
                        ? UnknownTemplateDiagnostic.CreateInfo(location, templateName)
                        : null;
                    return false;
                }
            }

            output = contents!; // only case this wouldn't be true is if templateNames is empty
            diagnostic = null;
            return true;
        }
    }
}