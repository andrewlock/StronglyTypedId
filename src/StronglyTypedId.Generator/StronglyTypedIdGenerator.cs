using CodeGeneration.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace StronglyTypedId.Generator
{
    public class StronglyTypedIdGenerator : IRichCodeGenerator
    {
        private readonly bool _generateJsonConverter;
        private readonly StronglyTypedIdBackingType _backingType;
        private readonly StronglyTypedIdJsonConverter _jsonProvider;

        public StronglyTypedIdGenerator(AttributeData attributeData)
        {
            if (attributeData == null) throw new ArgumentNullException(nameof(attributeData));
            _generateJsonConverter = (bool)attributeData.ConstructorArguments[0].Value;
            _backingType = (StronglyTypedIdBackingType)attributeData.ConstructorArguments[1].Value;
            _jsonProvider = (StronglyTypedIdJsonConverter)attributeData.ConstructorArguments[2].Value;
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
            return GetGenerator().CreateStronglyTypedIdSyntax(applyToClass, _generateJsonConverter, _jsonProvider);
        }

        private BaseSyntaxTreeGenerator GetGenerator()
        {
            switch (_backingType)
            {
                case StronglyTypedIdBackingType.Int:
                    return new IntSyntaxTreeGenerator();
                case StronglyTypedIdBackingType.String:
                    return new StringSyntaxTreeGenerator();
                case StronglyTypedIdBackingType.Guid:
                default:
                    return new GuidSyntaxTreeGenerator();
            }
        }
    }
}