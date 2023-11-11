using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace StronglyTypedIds.Tests
{
    internal static class TestHelpers
    {
        public static (ImmutableArray<Diagnostic> Diagnostics, string Output) GetGeneratedOutput<T>(string source, bool includeAttributes = true)
            where T : IIncrementalGenerator, new()
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(source);
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
                .Select(_ => MetadataReference.CreateFromFile(_.Location))
                .Concat(new[] {MetadataReference.CreateFromFile(typeof(T).Assembly.Location)});
            var compilation = CSharpCompilation.Create(
                "generator",
                new[] {syntaxTree},
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var originalTreeCount = compilation.SyntaxTrees.Length;

            CSharpGeneratorDriver
                .Create(new T())
                .AddAdditionalTexts(LoadEmbeddedResourcesAsAdditionalText())
                .RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

            // If we don't want the attributes, we try to get all the potential resource streams and exclude them
            var countsToExclude = includeAttributes
                ? originalTreeCount
                : originalTreeCount + typeof(T).Assembly.GetManifestResourceNames().Count(x => x.StartsWith("StronglyTypedIds.Templates.Sources."));

            var output = string.Join("\n", outputCompilation.SyntaxTrees.Skip(countsToExclude).Select(t => t.ToString())); 

            return (diagnostics, output);
        }

        private static ImmutableArray<AdditionalText> LoadEmbeddedResourcesAsAdditionalText()
        {
            var texts = ImmutableArray.CreateBuilder<AdditionalText>();
            var assembly = typeof(TestHelpers).Assembly;

            texts.AddRange(assembly.GetManifestResourceNames()
                .Select(name => new TestAdditionalText(
                    text: LoadEmbeddedResource(assembly, name),
                    path: Path.Combine("c:", "test", "Templates", Path.GetExtension(Path.GetFileNameWithoutExtension(name)).Substring(1) + ".typedid"))));

            return texts.ToImmutable();
        }
        private static string LoadEmbeddedResource(Assembly assembly, string resourceName)
        {
            var resourceStream = assembly.GetManifestResourceStream(resourceName);
            if (resourceStream is null)
            {
                var existingResources = assembly.GetManifestResourceNames();
                throw new ArgumentException($"Could not find embedded resource {resourceName}. Available names: {string.Join(", ", existingResources)}");
            }

            using var reader = new StreamReader(resourceStream, Encoding.UTF8);

            return reader.ReadToEnd();
        }

        private sealed class TestAdditionalText : AdditionalText
        {
            private readonly SourceText _text;

            public TestAdditionalText(string path, SourceText text)
            {
                Path = path;
                _text = text;
            }

            public TestAdditionalText(string text = "", Encoding encoding = null, string path = "dummy", SourceHashAlgorithm checksumAlgorithm = SourceHashAlgorithm.Sha1)
                : this(path, new StringText(text, encoding, checksumAlgorithm: checksumAlgorithm))
            {
            }

            public override string Path { get; }

            public override SourceText GetText(CancellationToken cancellationToken = default) => _text;
        }

        /// <summary>
        /// Implementation of SourceText based on a <see cref="String"/> input
        /// </summary>
        internal sealed class StringText : SourceText
        {
            private readonly string _source;
            private readonly Encoding _encodingOpt;

            internal StringText(
                string source,
                Encoding encodingOpt,
                ImmutableArray<byte> checksum = default(ImmutableArray<byte>),
                SourceHashAlgorithm checksumAlgorithm = SourceHashAlgorithm.Sha1)
                : base(checksum, checksumAlgorithm)
            {
                _source = source;
                _encodingOpt = encodingOpt;
            }

            public override Encoding Encoding => _encodingOpt;

            /// <summary>
            /// Underlying string which is the source of this <see cref="StringText"/>instance
            /// </summary>
            public string Source => _source;

            /// <summary>
            /// The length of the text represented by <see cref="StringText"/>.
            /// </summary>
            public override int Length => _source.Length;

            /// <summary>
            /// Returns a character at given position.
            /// </summary>
            /// <param name="position">The position to get the character from.</param>
            /// <returns>The character.</returns>
            /// <exception cref="ArgumentOutOfRangeException">When position is negative or 
            /// greater than <see cref="Length"/>.</exception>
            public override char this[int position]
            {
                get
                {
                    // NOTE: we are not validating position here as that would not 
                    //       add any value to the range check that string accessor performs anyways.

                    return _source[position];
                }
            }

            /// <summary>
            /// Provides a string representation of the StringText located within given span.
            /// </summary>
            /// <exception cref="ArgumentOutOfRangeException">When given span is outside of the text range.</exception>
            public override string ToString(TextSpan span)
            {
                if (span.End > this.Source.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(span));
                }

                if (span.Start == 0 && span.Length == this.Length)
                {
                    return this.Source;
                }

                return this.Source.Substring(span.Start, span.Length);
            }

            public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
            {
                this.Source.CopyTo(sourceIndex, destination, destinationIndex, count);
            }

            public override void Write(TextWriter textWriter, TextSpan span, CancellationToken cancellationToken = default(CancellationToken))
            {
                if (span.Start == 0 && span.End == this.Length)
                {
                    textWriter.Write(this.Source);
                }
                else
                {
                    base.Write(textWriter, span, cancellationToken);
                }
            }
        }
    }
}