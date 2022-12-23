using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            StronglyTypedIdBackingType type,
            StronglyTypedIdConverter c,
            StronglyTypedIdImplementations i)
        {
            const string idName = "MyTestId";
            var result = SourceGenerationHelper.CreateId(
                idName: idName,
                idNamespace: "",
                parentClass: null,
                converters: c,
                backingType: type,
                implementations: i
            );

            return Verifier.Verify(result)
                .UseDirectory("Snapshots")
                .UseParameters(type, c, i);
        }

        [Theory]
        [MemberData(nameof(BackingTypes))]
        public Task GeneratesFullIdCorrectly(StronglyTypedIdBackingType type)
        {
            // combine them all
            var combinedConverter = EnumHelper.AllConverters(includeDefault: false)
                .Aggregate(StronglyTypedIdConverter.None, (prev, current) => prev | current);

            // combine them all
            var combinedImplementation = EnumHelper.AllImplementations(includeDefault: false)
                .Aggregate(StronglyTypedIdImplementations.None, (prev, current) => prev | current);

            const string idName = "MyTestId";
            
            var result = SourceGenerationHelper.CreateId(
                idName: idName,
                idNamespace: "",
                parentClass: null,
                converters: combinedConverter,
                backingType: type,
                implementations: combinedImplementation
            );

            return Verifier.Verify(result)
                .UseDirectory("Snapshots")
                .UseParameters(type);
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

        public static IEnumerable<object[]> BackingTypes()
            => EnumHelper.AllBackingTypes(includeDefault: false)
                .Select(x => new object[] { x });

        public static IEnumerable<object[]> Parameters()
        {
            foreach (var backingType in EnumHelper.AllBackingTypes(includeDefault: false))
            {
                // All individual convert types
                foreach (var converter in EnumHelper.AllConverters(includeDefault: false))
                {
                    yield return new object[] { backingType, converter, StronglyTypedIdImplementations.None };
                }

                // All individual implementations
                foreach (var implementation in EnumHelper.AllImplementations(includeDefault: false))
                {
                    yield return new object[] { backingType, StronglyTypedIdConverter.None, implementation };
                }
            }
        }
    }
}