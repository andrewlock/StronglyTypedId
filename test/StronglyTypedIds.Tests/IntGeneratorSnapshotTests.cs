using System;
using System.Threading.Tasks;
using StronglyTypedIds.Sources;
using VerifyXunit;
using Xunit;

namespace StronglyTypedIds.Tests
{
    [UsesVerify]
    public class IntGeneratorSnapshotTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ThrowsWhenClassNameIsNullOrEmpty(string idName)
        {
            const string idNamespace = "Some.Namespace";
            Assert.Throws<ArgumentException>(() => SourceGenerationHelper.CreateIntId(
                idName: idName,
                idNamespace: idNamespace,
                jsonConverter: null
            ));
        }

        [Theory]
        [MemberData(nameof(Converters))]
        public Task GeneratesIntCorrectly(StronglyTypedIdJsonConverter? converter)
        {
            const string idNamespace = "Some.Namespace";
            const string idName = "MyTestId";
            var result = SourceGenerationHelper.CreateIntId(
                idName: idName,
                idNamespace: idNamespace,
                jsonConverter: converter
            );

            return Verifier.Verify(result)
                .UseDirectory("Snapshots")
                .UseParameters(converter);
        }

        [Theory]
        [MemberData(nameof(Converters))]
        public Task GeneratesIntInGlobalNamespaceCorrectly(StronglyTypedIdJsonConverter? converter)
        {
            const string idName = "MyTestId";
            var result = SourceGenerationHelper.CreateIntId(
                idName: idName,
                idNamespace: string.Empty,
                jsonConverter: converter
            );

            return Verifier.Verify(result)
                .UseDirectory("Snapshots")
                .UseParameters(converter);
        }

        public static TheoryData<StronglyTypedIdJsonConverter?> Converters => new()
        {
            null,
            StronglyTypedIdJsonConverter.NewtonsoftJson,
            StronglyTypedIdJsonConverter.SystemTextJson,
            StronglyTypedIdJsonConverter.NewtonsoftJson | StronglyTypedIdJsonConverter.SystemTextJson,
        };
    }
}