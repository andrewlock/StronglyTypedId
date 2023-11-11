// Based on https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/1dd5ced072d7d870f6dd698a6c02ad509a122452/StyleCop.Analyzers/StyleCop.Analyzers.CodeFixes/Settings/UnknownTemplateCodeFixProvider.cs

#nullable disable

using System;
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace StronglyTypedIds.Diagnostics;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UnknownTemplateCodeFixProvider)), Shared]
internal class UnknownTemplateCodeFixProvider : CodeFixProvider
{
    internal const string DefaultHeader =
        """"
        // ACTION REQUIRED: This file was automatically added to your project, but it
        // will not take effect until additional steps are taken to enable it. See 
        // https://github.com/dotnet/roslyn/issues/4655 for more details.
        //
        // To enable the template, in Visual Studio 2017, 2019, and 2022:
        //   1. Select the file in Solution Explorer.
        //   2. In the 'Properties' window, set the value for 'Build Action' 
        //      to one of the following (whichever is available):
        //    - For .NET Core and .NET Standard projects: 'C# analyzer additional file'
        //    - For other projects: 'AdditionalFiles'
        //
        // Any instances of PLACEHOLDERID will be replaced with the target ID name
        // when generating code from this template.
        

        """";

    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(Diagnostics.UnknownTemplateDiagnostic.Id);

    /// <inheritdoc/>
    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var project = context.Document.Project;
        var workspace = project.Solution.Workspace;

        // if we can't add extra documents, there's nothing we can do
        if (!workspace.CanApplyChange(ApplyChangesKind.AddAdditionalDocument))
        {
            return Task.CompletedTask;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            if(!diagnostic.Properties.TryGetValue(UnknownTemplateDiagnostic.TemplateName, out var templateName))
            {
                // This shouldn't happen, but play it safe
                continue;
            }

            // check if the template file already exists
            var alreadyAdded = false;
            foreach (var document in project.AdditionalDocuments)
            {
                if (document.Name.Equals(templateName, StringComparison.OrdinalIgnoreCase))
                {
                    alreadyAdded = true;
                    break;
                }
            }

            if (alreadyAdded)
            {
                continue;
            }

            context.RegisterCodeFix(
                CodeAction.Create(
                    $"Add '{templateName}.typedid' template to the project",
                    cancellationToken => GetTransformedSolutionAsync(context.Document, templateName, cancellationToken),
                    nameof(UnknownTemplateCodeFixProvider)),
                diagnostic);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public override FixAllProvider GetFixAllProvider()
    {
        // This code fix does not support fix all actions.
        return null;
    }

    private static Task<Solution> GetTransformedSolutionAsync(Document document, string templateName, CancellationToken cancellationToken)
    {
        // Currently unused
        _ = cancellationToken;

        var project = document.Project;
        var solution = project.Solution;

        var newDocumentId = DocumentId.CreateNewId(project.Id);

        var templateContent = GetTemplateContent(templateName);

        var newSolution = solution.AddAdditionalDocument(newDocumentId, $"{templateName}.typedid", templateContent);

        return Task.FromResult(newSolution);
    }

    internal static string GetTemplateContent(string templateName)
    {
        var templateContent = templateName switch
        {
            { } x when x.Contains("int", StringComparison.OrdinalIgnoreCase) => EmbeddedSources.LoadEmbeddedTypedId("int-full.typedid"),
            { } x when x.Contains("long", StringComparison.OrdinalIgnoreCase) => EmbeddedSources.LoadEmbeddedTypedId("long-full.typedid"),
            { } x when x.Contains("string", StringComparison.OrdinalIgnoreCase) => EmbeddedSources.LoadEmbeddedTypedId("string-full.typedid"),
            _ => EmbeddedSources.LoadEmbeddedTypedId("guid-full.typedid"),
        };

        return DefaultHeader + templateContent;
    }
}