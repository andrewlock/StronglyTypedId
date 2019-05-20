using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StronglyTypedId.Generator
{
    public class StronglyTypedIdGenerator : IRichCodeGenerator
    {
        public StronglyTypedIdGenerator(AttributeData attributeData)
        {
            if (attributeData == null) throw new ArgumentNullException(nameof(attributeData));
        }

        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<RichGenerationResult> GenerateRichAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            var applyToClass = (StructDeclarationSyntax)context.ProcessingNode;

            var stronglyTypedId = SyntaxTreeGenerator.CreateStronglyTypedIdSyntax(applyToClass);

            // Figure out ancestry for the generated type, including nesting types and namespaces.
            var wrappedMembers = stronglyTypedId.WrapWithAncestors(context.ProcessingNode);

            return Task.FromResult(new RichGenerationResult
            {
                Members = wrappedMembers,
            });
        }
    }
}
