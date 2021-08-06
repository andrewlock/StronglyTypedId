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
                converters: StronglyTypedIdConverter.None
            ));
        }

        [Theory]
        [MemberData(nameof(Converters))]
        public Task GeneratesIntCorrectly(StronglyTypedIdConverter converter)
        {
            const string idNamespace = "Some.Namespace";
            const string idName = "MyTestId";
            var result = SourceGenerationHelper.CreateIntId(
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
        public Task GeneratesIntInGlobalNamespaceCorrectly(StronglyTypedIdConverter converter)
        {
            const string idName = "MyTestId";
            var result = SourceGenerationHelper.CreateIntId(
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