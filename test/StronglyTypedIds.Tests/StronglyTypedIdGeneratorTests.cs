using System;
using System.Threading.Tasks;
using StronglyTypedIds.Diagnostics;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace StronglyTypedIds.Tests
{
    [UsesVerify]
    public class StronglyTypedIdGeneratorTests
    {
        readonly ITestOutputHelper _output;

        public StronglyTypedIdGeneratorTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public Task CanGenerateDefaultIdInGlobalNamespace()
        {
            const string input = @"using StronglyTypedIds;

[StronglyTypedId]
public partial struct MyId {}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

            Assert.Empty(diagnostics);

            return Verifier.Verify(output)
                .UseDirectory("Snapshots");
        }

        [Theory]
        [InlineData("")]
        [InlineData("Template.Guid")]
        [InlineData("template: Template.Guid")]
        public Task CanGenerateIdInNamespace(string template)
        {
            var input = $$"""
                using StronglyTypedIds;
                namespace SomeNamespace
                {
                    [StronglyTypedId({{template}})]
                    public partial struct MyId {}
                }
                """;
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

            Assert.Empty(diagnostics);

            return Verifier.Verify(output)
                .DisableRequireUniquePrefix()
                .UseDirectory("Snapshots");
        }

        [Theory]
        [InlineData("Template.Int")]
        [InlineData("template: Template.Int")]
        public Task CanGenerateNonDefaultIdInNamespace(string template)
        {
            var input = $$"""
                using StronglyTypedIds;
                namespace SomeNamespace
                {
                    [StronglyTypedId({{template}})]
                    public partial struct MyId {}
                }
                """;
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

            Assert.Empty(diagnostics);

            return Verifier.Verify(output)
                .DisableRequireUniquePrefix()
                .UseDirectory("Snapshots");
        }

        [Theory]
        [InlineData("\"newid-full\"")]
        [InlineData("templateNames: \"newid-full\"")]
        public Task CanGenerateForCustomTemplate(string templateName)
        {
            var input = $$"""
                using StronglyTypedIds;
                namespace SomeNamespace
                {
                    [StronglyTypedId({{templateName}})]
                    public partial struct MyId {}
                }
                """;
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

            Assert.Empty(diagnostics);

            return Verifier.Verify(output)
                .DisableRequireUniquePrefix()
                .UseDirectory("Snapshots");
        }

        [Fact]
        public Task CanGenerateIdInFileScopedNamespace()
        {
            const string input = @"using StronglyTypedIds;

namespace SomeNamespace;
[StronglyTypedId]
public partial struct MyId {}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

            Assert.Empty(diagnostics);

            return Verifier.Verify(output)
                .UseDirectory("Snapshots");
        }

        [Fact]
        public Task CanGenerateNestedIdInFileScopeNamespace()
        {
            const string input = @"using StronglyTypedIds;

namespace SomeNamespace;

public class ParentClass
{
    [StronglyTypedId]
    public partial struct MyId {}
}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

            Assert.Empty(diagnostics);

            return Verifier.Verify(output)
                .UseDirectory("Snapshots");
        }

        [Fact]
        public Task CanGenerateVeryNestedIdInFileScopeNamespace()
        {
            const string input = @"using StronglyTypedIds;

namespace SomeNamespace;

public partial class ParentClass
{
    internal partial record InnerClass
    {
        public readonly partial record struct InnerStruct
        {
            [StronglyTypedId]
            public readonly partial struct MyId {}
        }
    }
}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

            Assert.Empty(diagnostics);

            return Verifier.Verify(output)
                .UseDirectory("Snapshots");
        }

        [Fact]
        public Task CanGenerateGenericVeryNestedIdInFileScopeNamespace()
        {
            const string input = @"using StronglyTypedIds;

namespace SomeNamespace;

public class ParentClass<T> 
    where T: new() 
{
    internal record InnerClass<TKey, TValue>
    {
        public struct InnerStruct
        {
            [StronglyTypedId]
            public partial struct MyId {}
        }
    }
}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

            Assert.Empty(diagnostics);

            return Verifier.Verify(output)
                .UseDirectory("Snapshots");
        }

        [Theory]
        [InlineData("Template.Int")]
        [InlineData("template: Template.Int")]
        public Task CanOverrideDefaultsWithTemplateUsingGlobalAttribute(string template)
        {
            var input = $$"""
                using StronglyTypedIds;
                [assembly:StronglyTypedIdDefaults({{template}})]
                
                [StronglyTypedId]
                public partial struct MyId {}
                """;

            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

            Assert.Empty(diagnostics);

            return Verifier.Verify(output)
                .DisableRequireUniquePrefix()
                .UseDirectory("Snapshots");
        }

        [Theory]
        [InlineData("\"newid-full\"")]
        [InlineData("templateName: \"newid-full\"")]
        public Task CanOverrideDefaultsWithCustomTemplateUsingGlobalAttribute(string templateNames)
        {
            var input = $$"""
                using StronglyTypedIds;
                [assembly:StronglyTypedIdDefaults({{templateNames}})]
                
                [StronglyTypedId]
                public partial struct MyId {}
                """;

            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

            Assert.Empty(diagnostics);

            return Verifier.Verify(output)
                .DisableRequireUniquePrefix()
                .UseDirectory("Snapshots");
        }

        [Fact]
        public Task CanGenerateMultipleIdsWithSameName()
        {
            // https://github.com/andrewlock/StronglyTypedId/issues/74
            const string input = @"using StronglyTypedIds;

namespace MyContracts.V1
{
    [StronglyTypedId]
    public partial struct MyId {}
}

namespace MyContracts.V2
{
    [StronglyTypedId]
    public partial struct MyId {}
}";

            // This only includes the last ID but that's good enough for this
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input, includeAttributes: false);

            Assert.Empty(diagnostics);

            return Verifier.Verify(output)
                .UseDirectory("Snapshots");
        }
    }
}