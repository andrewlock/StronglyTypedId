using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using StronglyTypedId.Analyzers;
using StronglyTypedId.Analyzers.Utilities;

namespace StronglyTypedId.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class StronglyTypedIdMustBePartial : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
            = ImmutableArray.Create(Descriptors.X1000_StronglyTypedIdMustBePartial);

        public override void Initialize(AnalysisContext context)
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSyntaxNodeAction(AnalyzeStruct, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(AnalyzeOuterType, SyntaxKind.StructDeclaration, SyntaxKind.ClassDeclaration);
        }

        private static void AnalyzeStruct(SyntaxNodeAnalysisContext context)
        {
            var structDeclaration = (StructDeclarationSyntax)context.Node;
            if (structDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                return;
            }
            // if no strongly typed id attributes, stop diagnostic
            if (!HasStronglyTypedIdAttributes(structDeclaration))
            {
                return;
            }
            context.ReportDiagnostic(CreateDiagnostic(structDeclaration));
        }

        private static void AnalyzeOuterType(SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = (TypeDeclarationSyntax)context.Node;
            if (typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                return;
            }
            // if no strongly typed id types, stop diagnostic
            if (!typeDeclaration.DescendantNodes().OfType<StructDeclarationSyntax>().Any(HasStronglyTypedIdAttributes))
            {
                return;
            }
            context.ReportDiagnostic(CreateDiagnostic(typeDeclaration));
        }

        private static Diagnostic CreateDiagnostic(TypeDeclarationSyntax typeSyntax)
        {
            return Diagnostic.Create(
                Descriptors.X1000_StronglyTypedIdMustBePartial,
                typeSyntax.Identifier.GetLocation(),
                typeSyntax.Identifier.ValueText);
        }

        private static bool HasStronglyTypedIdAttributes(TypeDeclarationSyntax typeDeclaration)
        {
            return
                typeDeclaration.AttributeLists
                .SelectMany(list => list.Attributes.Where(att => att.IsStronglyTypedIdSyntax()))
                .Any();
        }
    }
}
