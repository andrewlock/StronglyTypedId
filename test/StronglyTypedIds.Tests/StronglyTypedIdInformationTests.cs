using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using StronglyTypedIds.Diagnostics;
using StronglyTypedIds.Sources;
using Xunit;

namespace StronglyTypedIds.Tests
{
    public class StronglyTypedIdInformationTests
    {
        private static readonly StronglyTypedIdAttribute DefaultAttribute = new();
        private static readonly StronglyTypedIdDefaultsAttribute DefaultDefaultsAttribute = new();

        [Theory]
        [InlineData("public")]
        [InlineData("internal")]
        [InlineData("private")]
        public void CreateForStruct(string visibility)
        {
            var code = $@"
using StronglyTypedIds;

[StronglyTypedId]
{visibility} partial struct TestId {{}}";

            var information = GetInformation(code);

            var kvp = Assert.Single(information.Ids);
            Assert.Equal("TestId", kvp.Key.Name);
            Assert.Equal(DefaultAttribute.Converters, kvp.Value.Configuration.Converters);
            Assert.Equal(DefaultAttribute.BackingType, kvp.Value.Configuration.BackingType);
        }

        [Fact]
        public void CreateForMultipleStructs()
        {
            const string code = @"
using StronglyTypedIds;

[StronglyTypedId]
public partial struct TestId1 {}

[StronglyTypedId]
public partial struct TestId2 {}
";

            var information = GetInformation(code);

            Assert.Equal(2, information.Ids.Count);
            Assert.Single(information.Ids, x => x.Key.Name == "TestId1");
            Assert.Single(information.Ids, x => x.Key.Name == "TestId2");
        }

        [Fact]
        public void DoesNotCreateWhenCompilationErrorDueToMissingNamespace()
        {
            const string code = @"
// using StronglyTypedIds;

[StronglyTypedId]
public partial struct TestId1 {}";

            var information = GetInformation(code);

            Assert.Empty(information.Ids);
        }

        [Fact]
        public void DoesNotCreateWhenNoAttributes()
        {
            const string code = @"
public partial struct TestId1 {}

public partial struct TestId2 {}
";

            var information = GetInformation(code);

            Assert.Empty(information.Ids);
        }

        [Fact]
        public void AddsDiagnosticWhenNoPartial()
        {
            const string code = @"
using StronglyTypedIds;

[StronglyTypedId]
public struct TestId1 {}
";

            var information = GetInformation(code);

            var id = Assert.Single(information.Ids);
            Assert.Equal("TestId1", id.Key.Name);
            var diag = id.Value.Diagnostics;
            Assert.NotEmpty(diag);
            Assert.Single(diag, x => x.Id == NotPartialDiagnostic.Id);
        }

        [Fact]
        public void AddsDiagnosticWhenNested()
        {
            const string code = @"
using StronglyTypedIds;

public class Outer
{
    [StronglyTypedId]
    public partial struct TestId1 {}
}
";

            var information = GetInformation(code);

            var id = Assert.Single(information.Ids);
            Assert.Equal("TestId1", id.Key.Name);
            var diag = id.Value.Diagnostics;
            Assert.NotEmpty(diag);
            Assert.Single(diag, x => x.Id == NestedTypeDiagnostic.Id);
        }

        [Fact]
        public void AddsDiagnosticWhenInvalidConverter()
        {
            const string code = @"
using StronglyTypedIds;

[StronglyTypedId(converters: (StronglyTypedIdConverter)1923)]
public partial struct TestId1 {}
";

            var information = GetInformation(code);

            var id = Assert.Single(information.Ids);
            Assert.Equal("TestId1", id.Key.Name);
            var diag = id.Value.Diagnostics;
            Assert.NotEmpty(diag);
            Assert.Single(diag, x => x.Id == InvalidConverterDiagnostic.Id);
        }

        [Fact]
        public void AddsDiagnosticWhenInvalidBackingType()
        {
            const string code = @"
using StronglyTypedIds;

[StronglyTypedId(backingType: (StronglyTypedIdBackingType)1923)]
public partial struct TestId1 {}
";

            var information = GetInformation(code);

            var id = Assert.Single(information.Ids);
            Assert.Equal("TestId1", id.Key.Name);
            var diag = id.Value.Diagnostics;
            Assert.NotEmpty(diag);
            Assert.Single(diag, x => x.Id == InvalidBackingTypeDiagnostic.Id);
        }

        [Fact]
        public void CreatesDefaultsForAssemblyAttribute()
        {
            var code = @"
using StronglyTypedIds;

[assembly:StronglyTypedIdDefaults]";

            var information = GetInformation(code);

            Assert.NotNull(information.Defaults.Defaults);
            var defaults = information.Defaults.Defaults.Value;
            Assert.Equal(DefaultDefaultsAttribute.Converters, defaults.Converters);
            Assert.Equal(DefaultDefaultsAttribute.BackingType, defaults.BackingType);
        }

        [Fact]
        public void DoesNotCreateDefaultsWhenCompilationErrorDueToMissingNamespace()
        {
            const string code = @"
// using StronglyTypedIds;

[assembly:StronglyTypedIdDefaults]";

            var information = GetInformation(code);

            Assert.Null(information.Defaults.Defaults);
        }

        [Fact]
        public void DoesNotCreateDefaultsWhenNoAttributes()
        {
            const string code = @"
public partial struct TestId1 {}

public partial struct TestId2 {}
";

            var information = GetInformation(code);

            Assert.Null(information.Defaults.Defaults);
        }

        [Fact]
        public void ReturnsResultsWhenMultipleAssemblyAttributes()
        {
            const string code = @"
using StronglyTypedIds;

[assembly:StronglyTypedIdDefaults]
[assembly:StronglyTypedIdDefaults]
";

            var information = GetInformation(code);

            Assert.NotNull(information.Defaults.Defaults);
        }

        [Fact]
        public void AddsDiagnosticWhenInvalidConverterInAssemblyAttribute()
        {
            const string code = @"
using StronglyTypedIds;

[assembly:StronglyTypedIdDefaults(converters: (StronglyTypedIdConverter)1923)]
";

            var information = GetInformation(code);

            Assert.NotNull(information.Defaults.Defaults);
            Assert.NotEmpty(information.Defaults.Diagnostics);
            Assert.Single(information.Defaults.Diagnostics, x => x.Id == InvalidConverterDiagnostic.Id);
        }

        [Fact]
        public void AddsDiagnosticWhenInvalidBackingTypeInBackingTypes()
        {
            const string code = @"
using StronglyTypedIds;

[assembly:StronglyTypedIdDefaults(backingType: (StronglyTypedIdBackingType)1923)]
";

            var information = GetInformation(code);

            Assert.NotNull(information.Defaults.Defaults);
            Assert.NotEmpty(information.Defaults.Diagnostics);
            Assert.Single(information.Defaults.Diagnostics, x => x.Id == InvalidBackingTypeDiagnostic.Id);
        }

        private static StronglyTypedIdInformation GetInformation(string source)
        {
            var attributeSyntaxTree = CSharpSyntaxTree.ParseText(EmbeddedSources.StronglyTypedIdAttributeSource);
            var defaultsSyntaxTree = CSharpSyntaxTree.ParseText(EmbeddedSources.StronglyTypedIdDefaultsAttributeSource);
            var backingTypeSyntaxTree = CSharpSyntaxTree.ParseText(EmbeddedSources.StronglyTypedIdBackingTypeSource);
            var converterTree = CSharpSyntaxTree.ParseText(EmbeddedSources.StronglyTypedIdConverterSource);
            var sourceSyntaxTree = CSharpSyntaxTree.ParseText(source);
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
                .Select(_ => MetadataReference.CreateFromFile(_.Location))
                .Concat(new[] { MetadataReference.CreateFromFile(typeof(EmbeddedSources).Assembly.Location) });

            var compilation = CSharpCompilation.Create(
                "generator",
                new[] { attributeSyntaxTree, defaultsSyntaxTree, backingTypeSyntaxTree, converterTree, sourceSyntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var receiver = new StronglyTypedIdReceiver();

            foreach (var node in sourceSyntaxTree.GetRoot().DescendantNodes(descendIntoChildren: _ => true))
            {
                receiver.OnVisitSyntaxNode(node);
            }

            return StronglyTypedIdInformation.Create(receiver, compilation);
        }
    }
}