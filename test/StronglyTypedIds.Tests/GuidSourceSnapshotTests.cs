using System;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace StronglyTypedIds.Tests
{
    [UsesVerify]
    public class GuidGeneratorSnapshotTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ThrowsWhenClassNameIsNullOrEmpty(string idName)
        {
            const string idNamespace = "Some.Namespace";
            Assert.Throws<ArgumentException>(() => Sources.CreateGuidId(
                idName: idName,
                idNamespace: idNamespace,
                jsonConverter: null
            ));
        }

        [Theory]
        [MemberData(nameof(Converters))]
        public Task GeneratesGuidCorrectly(StronglyTypedIdJsonConverter? converter)
        {
            const string idNamespace = "Some.Namespace";
            const string idName = "MyTestId";
            var result = Sources.CreateGuidId(
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
        public Task GeneratesGuidInGlobalNamespaceCorrectly(StronglyTypedIdJsonConverter? converter)
        {
            const string idName = "MyTestId";
            var result = Sources.CreateGuidId(
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