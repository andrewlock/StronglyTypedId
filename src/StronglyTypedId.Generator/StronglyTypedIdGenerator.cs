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
        private readonly bool _generateJsonConverter;
        private readonly StronglyTypedIdBackingType _backingType;

        public StronglyTypedIdGenerator(AttributeData attributeData)
        {
            if (attributeData == null) throw new ArgumentNullException(nameof(attributeData));
            _generateJsonConverter = (bool)attributeData.ConstructorArguments[0].Value;
            _backingType = (StronglyTypedIdBackingType)attributeData.ConstructorArguments[1].Value;
        }

        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<RichGenerationResult> GenerateRichAsync(TransformationContext context, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            var applyToClass = (StructDeclarationSyntax)context.ProcessingNode;
            SyntaxList<MemberDeclarationSyntax> stronglyTypedId = GetSyntax(applyToClass);

            // Figure out ancestry for the generated type, including nesting types and namespaces.
            var wrappedMembers = stronglyTypedId.WrapWithAncestors(context.ProcessingNode);

            return Task.FromResult(new RichGenerationResult
            {
                Members = wrappedMembers,
            });
        }

        private SyntaxList<MemberDeclarationSyntax> GetSyntax(StructDeclarationSyntax applyToClass)
        {
            switch (_backingType)
            {
                case StronglyTypedIdBackingType.Int:
                    return IntSyntaxTreeGenerator.CreateStronglyTypedIdSyntax(applyToClass, _generateJsonConverter);
                case StronglyTypedIdBackingType.String:
                    return StringSyntaxTreeGenerator.CreateStronglyTypedIdSyntax(applyToClass, _generateJsonConverter);
                case StronglyTypedIdBackingType.Guid:
                default:
                    return GuidSyntaxTreeGenerator.CreateStronglyTypedIdSyntax(applyToClass, _generateJsonConverter);
            }
        }
    }
}
