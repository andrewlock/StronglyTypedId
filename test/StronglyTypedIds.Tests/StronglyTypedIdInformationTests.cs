using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using StronglyTypedIds.Diagnostics;
using StronglyTypedIds.Tests.Diagnostics;
using Xunit;

namespace StronglyTypedIds.Tests
{
    public class StronglyTypedIdInformationTests
    {
        private static readonly StronglyTypedIdAttribute DefaultAttribute = new();

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
            Assert.Equal(DefaultAttribute.GenerateJsonConverter, kvp.Value.GenerateJsonConverter);
            Assert.Equal(DefaultAttribute.JsonConverter, kvp.Value.JsonConverter);
            Assert.Equal(DefaultAttribute.GenerateJsonConverter, kvp.Value.GenerateJsonConverter);
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

        private static StronglyTypedIdInformation GetInformation(string source)
        {
            var attributeSyntaxTree = CSharpSyntaxTree.ParseText(Sources.StronglyTypedIdAttributeSource);
            var backingTypeSyntaxTree = CSharpSyntaxTree.ParseText(Sources.StronglyTypedIdBackingTypeSource);
            var converterTree = CSharpSyntaxTree.ParseText(Sources.StronglyTypedIdJsonConverterSource);
            var sourceSyntaxTree = CSharpSyntaxTree.ParseText(source);
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(_ => !_.IsDynamic && !string.IsNullOrWhiteSpace(_.Location))
                .Select(_ => MetadataReference.CreateFromFile(_.Location))
                .Concat(new[] { MetadataReference.CreateFromFile(typeof(Sources).Assembly.Location) });

            var compilation = CSharpCompilation.Create(
                "generator",
                new[] { attributeSyntaxTree, backingTypeSyntaxTree, converterTree, sourceSyntaxTree },
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