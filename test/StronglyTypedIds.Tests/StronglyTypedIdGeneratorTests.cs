using System;
using System.Threading.Tasks;
using StronglyTypedIds.Sources;
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
        public Task CanOverrideDefaultsUsingGlobalAttribute()
        {
            const string input = @"using StronglyTypedIds;
[assembly:StronglyTypedIdDefaults(backingType: StronglyTypedIdBackingType.Int, converters: StronglyTypedIdConverter.None)]

[StronglyTypedId]
public partial struct MyId {}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input);

            Assert.Empty(diagnostics);

            return Verifier.Verify(output)
                .UseDirectory("Snapshots");
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public Task CanGenerateIdWithNamedParameters(StronglyTypedIdBackingType backingType, StronglyTypedIdConverter? converter)
        {
            var type = $"backingType: {nameof(StronglyTypedIdBackingType)}.{backingType.ToString()}";
            var converters = converter.HasValue
                ? $", converters: {ToArgument(converter.Value)}"
                : string.Empty;
            var attribute = $"[StronglyTypedId({type}{converters})]";

            _output.WriteLine(attribute);

            var input = @"using StronglyTypedIds;

namespace MyTests.TestNameSpace
{
    " + attribute + @"
    public partial struct MyId {}
}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input);

            Assert.Empty(diagnostics);

            return Verifier.Verify(output)
                .UseParameters(backingType, converter)
                .UseDirectory("Snapshots");
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public Task CanGenerateIdWithPositionalParameters(StronglyTypedIdBackingType backingType, StronglyTypedIdConverter? converter)
        {
            var type = $"{nameof(StronglyTypedIdBackingType)}.{backingType.ToString()}";
            var args = converter.HasValue ? $"{type}, {ToArgument(converter.Value)}" : $"{type}";
            var attribute = $"[StronglyTypedId({args})]";

            _output.WriteLine(attribute);

            var input = @"using StronglyTypedIds;

namespace MyTests.TestNameSpace
{
    " + attribute + @"
    public partial struct MyId {}
}";
            var (diagnostics, output) = TestHelpers.GetGeneratedOutput<StronglyTypedIdGenerator>(input);

            Assert.Empty(diagnostics);

            return Verifier.Verify(output)
                .UseParameters(backingType, converter)
                .UseDirectory("Snapshots");
        }

        public static TheoryData<StronglyTypedIdBackingType, StronglyTypedIdConverter?> GetData => new()
        {
            {StronglyTypedIdBackingType.Guid, null},
            {StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.NewtonsoftJson},
            {StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.SystemTextJson},
            {StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.TypeConverter},
            {StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.SystemTextJson | StronglyTypedIdConverter.NewtonsoftJson},
        };

        public static string ToArgument(StronglyTypedIdConverter converter) =>
            converter switch
            {
                StronglyTypedIdConverter.NewtonsoftJson => "StronglyTypedIdConverter.NewtonsoftJson",
                StronglyTypedIdConverter.SystemTextJson => "StronglyTypedIdConverter.SystemTextJson",
                StronglyTypedIdConverter.TypeConverter => "StronglyTypedIdConverter.TypeConverter",
                _ when converter.HasFlag(StronglyTypedIdConverter.NewtonsoftJson) &&
                       converter.HasFlag(StronglyTypedIdConverter.SystemTextJson) =>
                    "StronglyTypedIdConverter.NewtonsoftJson | StronglyTypedIdConverter.SystemTextJson",
                _ => throw new InvalidOperationException("Unknown converter " + converter),
            };
    }
}