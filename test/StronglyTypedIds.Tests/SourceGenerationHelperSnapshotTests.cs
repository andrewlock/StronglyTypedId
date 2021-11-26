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
                parentClass: null,
                converters: StronglyTypedIdConverter.None,
                backingType: StronglyTypedIdBackingType.Guid,
                implementations: StronglyTypedIdImplementations.None
            ));
        }

        [Fact]
        public void ThrowsWhenDefaultConverterIsUsed()
        {
            Assert.Throws<ArgumentException>(() => SourceGenerationHelper.CreateId(
                idName: "MyTestId",
                idNamespace: IdNamespace,
                parentClass: null,
                converters: StronglyTypedIdConverter.Default,
                backingType: StronglyTypedIdBackingType.Guid,
                implementations: StronglyTypedIdImplementations.None
            ));
        }

        [Fact]
        public void ThrowsWhenDefaultBackingTypeIsUsed()
        {
            Assert.Throws<ArgumentException>(() => SourceGenerationHelper.CreateId(
                idName: "MyTestId",
                idNamespace: IdNamespace,
                parentClass: null,
                converters: StronglyTypedIdConverter.None,
                backingType: StronglyTypedIdBackingType.Default,
                implementations: StronglyTypedIdImplementations.None
            ));
        }

        [Fact]
        public void ThrowsWhenDefaultImplementationsIsUsed()
        {
            Assert.Throws<ArgumentException>(() => SourceGenerationHelper.CreateId(
                idName: "MyTestId",
                idNamespace: IdNamespace,
                parentClass: null,
                converters: StronglyTypedIdConverter.None,
                backingType: StronglyTypedIdBackingType.Guid,
                implementations: StronglyTypedIdImplementations.Default
            ));
        }

        [Theory]
        [MemberData(nameof(Parameters))]
        public Task GeneratesIdCorrectly(
            string ns,
            StronglyTypedIdConverter converter,
            StronglyTypedIdBackingType backingType,
            StronglyTypedIdImplementations implementations)
        {
            const string idName = "MyTestId";
            var result = SourceGenerationHelper.CreateId(
                idName: idName,
                idNamespace: ns,
                parentClass: null,
                converters: converter,
                backingType: backingType,
                implementations: implementations
            );

            return Verifier.Verify(result)
                .UseDirectory("Snapshots")
                .UseParameters(ns, (int)converter, backingType, (int)implementations);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public Task GeneratesIdWithNestedClassCorrectly(int nestedClassCount)
        {
            var parent = new ParentClass("record", "InnerMost<T>", string.Empty, child: null);
            for (int i = 0; i < nestedClassCount; i++)
            {
                parent = new ParentClass("class", "OuterLayer" + i, string.Empty, parent);
            }

            var result = SourceGenerationHelper.CreateId(
                idName: "MyTestId",
                idNamespace: "MyTestNamespace",
                parentClass: parent,
                converters: StronglyTypedIdConverter.SystemTextJson,
                backingType: StronglyTypedIdBackingType.Guid,
                implementations: StronglyTypedIdImplementations.None
            );

            return Verifier.Verify(result)
                .UseDirectory("Snapshots")
                .UseParameters(nestedClassCount);
        }

        public static IEnumerable<object[]> Parameters()
        {
            foreach (var converter in EnumHelper.AllConverters(includeDefault: false))
            foreach (var backingType in EnumHelper.AllBackingTypes(includeDefault: false))
            foreach (var implementations in EnumHelper.AllImplementations(includeDefault: false))
            foreach (var ns in new[] { string.Empty, IdNamespace })
                yield return new object[] { ns, converter, backingType, implementations };
        }
    }
}