using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace StronglyTypedIds.Tests
{
    internal static class TestHelpers
    {
        public static (ImmutableArray<Diagnostic> Diagnostics, string Output) GetGeneratedOutput<T>(string source)
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
            var generator = new T();

            var driver = CSharpGeneratorDriver.Create(generator);
            driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

            var trees = outputCompilation.SyntaxTrees.ToList();

            return (diagnostics, trees.Count != originalTreeCount ? trees[^1].ToString() : string.Empty);
        }

        internal static string LoadEmbeddedResource(string resourceName)
        {
            var assembly = typeof(TestHelpers).Assembly;
            var resourceStream = assembly.GetManifestResourceStream(resourceName);
            if (resourceStream is null)
            {
                var existingResources = assembly.GetManifestResourceNames();
                throw new ArgumentException($"Could not find embedded resource {resourceName}. Available names: {string.Join(", ", existingResources)}");
            }

            using var reader = new StreamReader(resourceStream, Encoding.UTF8);

            return reader.ReadToEnd();
        }
    }
}