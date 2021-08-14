using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StronglyTypedIds.Sources;
using VerifyXunit;
using Xunit;

namespace StronglyTypedIds.Tests
{
    [UsesVerify]
    public class SourceGenerationHelperSnapshotTests
    {
        private const string IdNamespace = "Some.Namespace";

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ThrowsWhenClassNameIsNullOrEmpty(string idName)
        {
            Assert.Throws<ArgumentException>(() => SourceGenerationHelper.CreateId(
                idName: idName,
                idNamespace: IdNamespace,
                converters: StronglyTypedIdConverter.None,
                backingType: StronglyTypedIdBackingType.Guid
            ));
        }

        [Fact]
        public void ThrowsWhenDefaultConverterIsUsed()
        {
            Assert.Throws<ArgumentException>(() => SourceGenerationHelper.CreateId(
                idName: "MyTestId",
                idNamespace: IdNamespace,
                converters: StronglyTypedIdConverter.Default,
                backingType: StronglyTypedIdBackingType.Guid
            ));
        }

        [Fact]
        public void ThrowsWhenDefaultBackingTypeIsUsed()
        {
            Assert.Throws<ArgumentException>(() => SourceGenerationHelper.CreateId(
                idName: "MyTestId",
                idNamespace: IdNamespace,
                converters: StronglyTypedIdConverter.None,
                backingType: StronglyTypedIdBackingType.Default
            ));
        }

        [Theory]
        [MemberData(nameof(Parameters))]
        public Task GeneratesIdCorrectly(
            string ns,
            StronglyTypedIdConverter converter,
            StronglyTypedIdBackingType backingType)
        {
            const string idName = "MyTestId";
            var result = SourceGenerationHelper.CreateId(
                idName: idName,
                idNamespace: ns,
                converters: converter,
                backingType: backingType
            );

            return Verifier.Verify(result)
                .UseDirectory("Snapshots")
                .UseParameters(ns, (int)converter, backingType);
        }

        public static IEnumerable<object[]> Parameters()
        {
            foreach (var converter in EnumHelper.AllConverters(includeDefault: false))
            foreach (var backingType in EnumHelper.AllBackingTypes(includeDefault: false))
            foreach (var ns in new[] { string.Empty, IdNamespace })
                yield return new object[] { ns, converter, backingType };
        }
    }
}