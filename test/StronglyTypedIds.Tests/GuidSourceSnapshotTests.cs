using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StronglyTypedIds.Sources;
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
            Assert.Throws<ArgumentException>(() => SourceGenerationHelper.CreateGuidId(
                idName: idName,
                idNamespace: idNamespace,
                converters: StronglyTypedIdConverter.None
            ));
        }

        [Fact]
        public void ThrowsWhenDefaultConverterIsUsed()
        {
            const string idNamespace = "Some.Namespace";
            Assert.Throws<ArgumentException>(() => SourceGenerationHelper.CreateGuidId(
                idName: "MyTestId",
                idNamespace: idNamespace,
                converters: StronglyTypedIdConverter.Default
            ));
        }

        [Theory]
        [MemberData(nameof(Converters))]
        public Task GeneratesGuidCorrectly(StronglyTypedIdConverter converter)
        {
            const string idNamespace = "Some.Namespace";
            const string idName = "MyTestId";
            var result = SourceGenerationHelper.CreateGuidId(
                idName: idName,
                idNamespace: idNamespace,
                converters: converter
            );

            return Verifier.Verify(result)
                .UseDirectory("Snapshots")
                .UseParameters(converter);
        }

        [Theory]
        [MemberData(nameof(Converters))]
        public Task GeneratesGuidInGlobalNamespaceCorrectly(StronglyTypedIdConverter converter)
        {
            const string idName = "MyTestId";
            var result = SourceGenerationHelper.CreateGuidId(
                idName: idName,
                idNamespace: string.Empty,
                converters: converter
            );

            return Verifier.Verify(result)
                .UseDirectory("Snapshots")
                .UseParameters(converter);
        }

        public static IEnumerable<object[]> Converters =>
            EnumHelper.AllConverters(includeDefault: false).Select(x => new object[] { x });
    }
}