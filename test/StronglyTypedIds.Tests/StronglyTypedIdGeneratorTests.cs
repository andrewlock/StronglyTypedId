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
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input);

            Assert.Empty(diagnostics);

            return Verifier.Verify(output)
                .UseDirectory("Snapshots");
        }

        [Fact]
        public Task CanGenerateIdInNamespace()
        {
            const string input = @"using StronglyTypedIds;

namespace SomeNamespace
{
    [StronglyTypedId()]
    public partial struct MyId {}
}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input);

            Assert.Empty(diagnostics);

            return Verifier.Verify(output)
                .UseDirectory("Snapshots");
        }

        [Fact]
        public Task CanGenerateIdInFileScopedNamespace()
        {
            const string input = @"using StronglyTypedIds;

namespace SomeNamespace;
[StronglyTypedId]
public partial struct MyId {}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input);

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
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input);

            Assert.Empty(diagnostics);

            return Verifier.Verify(output)
                .UseDirectory("Snapshots");
        }

        [Fact]
        public Task CanGenerateVeryNestedIdInFileScopeNamespace()
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
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input);

            Assert.Empty(diagnostics);

            return Verifier.Verify(output)
                .UseDirectory("Snapshots");
        }

        [Fact]
        public Task CanOverrideDefaultsUsingGlobalAttribute()
        {
            const string input = """
                using StronglyTypedIds;
                [assembly:StronglyTypedIdDefaults("int")]
                
                [StronglyTypedId]
                public partial struct MyId {}
                """;

            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input);

            Assert.Empty(diagnostics);

            return Verifier.Verify(output)
                .UseDirectory("Snapshots");
        }

        [Fact]
        public Task MultipleAssemblyAttributesGeneratesWithDefault()
        {
            const string input = """
                using StronglyTypedIds;
                [assembly:StronglyTypedIdDefaults("int")]
                [assembly:StronglyTypedIdDefaults("long")]
                
                [StronglyTypedId]
                public partial struct MyId {}
                """;
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input);

            Assert.Contains(diagnostics, diagnostic => diagnostic.Id == MultipleAssemblyAttributeDiagnostic.Id);

            return Verifier.Verify(output)
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
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input);

            Assert.Empty(diagnostics);

            return Verifier.Verify(output)
                .UseDirectory("Snapshots");
        }
    }
}