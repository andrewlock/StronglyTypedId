using System;
using System.Collections.Generic;
using System.Linq;
using StronglyTypedIds.Sources;
using Xunit;

namespace StronglyTypedIds.Tests
{
    public class StronglyTypedIdConfigurationTests
    {
        [Theory]
        [MemberData(nameof(ExpectedBackingTypes))]
        public void ReturnsCorrectBackingType_WhenNoDefaultAttribute(StronglyTypedIdBackingType attributeValue, StronglyTypedIdBackingType expected)
        {
            var attributeValues = new StronglyTypedIdConfiguration(attributeValue, StronglyTypedIdConverter.Default);

            var result = StronglyTypedIdConfiguration.Combine(attributeValues, null);

            Assert.Equal(expected, result.BackingType);
        }

        [Theory]
        [MemberData(nameof(ExpectedBackingTypes))]
        public void ReturnsCorrectBackingType_WhenHaveDefaultAttributeThatUsesDefault(StronglyTypedIdBackingType attributeValue, StronglyTypedIdBackingType expected)
        {
            var defaultAttribute = new StronglyTypedIdConfiguration(StronglyTypedIdBackingType.Default, StronglyTypedIdConverter.Default);
            var attributeValues = new StronglyTypedIdConfiguration(attributeValue, StronglyTypedIdConverter.Default);

            var result = StronglyTypedIdConfiguration.Combine(attributeValues, defaultAttribute);

            Assert.Equal(expected, result.BackingType);
        }

        [Theory]
        [MemberData(nameof(ExpectedBackingTypesWithDefault))]
        public void ReturnsCorrectBackingType_WhenHaveDefaultAttribute(StronglyTypedIdBackingType attributeValue, StronglyTypedIdBackingType defaultValue, StronglyTypedIdBackingType expected)
        {
            var defaultAttribute = new StronglyTypedIdConfiguration(defaultValue, StronglyTypedIdConverter.Default);
            var attributeValues = new StronglyTypedIdConfiguration(attributeValue, StronglyTypedIdConverter.Default);

            var result = StronglyTypedIdConfiguration.Combine(attributeValues, defaultAttribute);

            Assert.Equal(expected, result.BackingType);
        }

        [Theory]
        [MemberData(nameof(ExpectedConverters))]
        public void ReturnsCorrectConverters_WhenNoDefaultAttribute(StronglyTypedIdConverter attributeValue, StronglyTypedIdConverter expected)
        {
            var attributeValues = new StronglyTypedIdConfiguration(StronglyTypedIdBackingType.Default, attributeValue);

            var result = StronglyTypedIdConfiguration.Combine(attributeValues, null);

            Assert.Equal(expected, result.Converters);
        }

        [Theory]
        [MemberData(nameof(ExpectedConverters))]
        public void ReturnsCorrectConverters_WhenHaveDefaultAttributeThatUsesDefault(StronglyTypedIdConverter attributeValue, StronglyTypedIdConverter expected)
        {
            var defaultAttribute = new StronglyTypedIdConfiguration(StronglyTypedIdBackingType.Default, StronglyTypedIdConverter.Default);
            var attributeValues = new StronglyTypedIdConfiguration(StronglyTypedIdBackingType.Default, attributeValue);

            var result = StronglyTypedIdConfiguration.Combine(attributeValues, defaultAttribute);

            Assert.Equal(expected, result.Converters);
        }

        [Theory]
        [MemberData(nameof(ExpectedConvertersWithDefault))]
        public void ReturnsCorrectConverters_WhenHaveDefaultAttribute(StronglyTypedIdConverter attributeValue, StronglyTypedIdConverter defaultValue, StronglyTypedIdConverter expected)
        {
            var defaultAttribute = new StronglyTypedIdConfiguration(StronglyTypedIdBackingType.Default, defaultValue);
            var attributeValues = new StronglyTypedIdConfiguration(StronglyTypedIdBackingType.Default, attributeValue);

            var result = StronglyTypedIdConfiguration.Combine(attributeValues, defaultAttribute);

            Assert.Equal(expected, result.Converters);
        }

        public static IEnumerable<object[]> ExpectedBackingTypes()
        {
            foreach (var backingType in EnumHelper.AllBackingTypes(includeDefault: false))
            {
                // attribute, expected
                yield return new object[] { backingType, backingType };
            }

            yield return new object[] { StronglyTypedIdBackingType.Default, StronglyTypedIdConfiguration.Defaults.BackingType };
        }

        public static IEnumerable<object[]> ExpectedBackingTypesWithDefault()
        {
            foreach (var attributeType in EnumHelper.AllBackingTypes(includeDefault: false))
            {
                foreach (var defaultType in EnumHelper.AllBackingTypes(includeDefault: true))
                {
                    // attribute, default, expected
                    yield return new object[] { attributeType, defaultType, attributeType };
                }
            }

            foreach (var defaultType in EnumHelper.AllBackingTypes(includeDefault: false))
            {
                // attribute, default, expected
                yield return new object[] { StronglyTypedIdBackingType.Default, defaultType, defaultType };
            }

            yield return new object[] { StronglyTypedIdBackingType.Default, StronglyTypedIdBackingType.Default, StronglyTypedIdConfiguration.Defaults.BackingType };
        }


        public static IEnumerable<object[]> ExpectedConverters()
        {
            foreach (var backingType in EnumHelper.AllConverters(includeDefault: false, includeNone: false))
            {
                // attribute, expected
                yield return new object[] { backingType, backingType };
            }

            yield return new object[] { StronglyTypedIdConverter.None, StronglyTypedIdConverter.None };
            yield return new object[] { StronglyTypedIdConverter.Default, StronglyTypedIdConfiguration.Defaults.Converters };
        }

        public static IEnumerable<object[]> ExpectedConvertersWithDefault()
        {
            foreach (var attributeType in EnumHelper.AllConverters(includeDefault: false))
            {
                foreach (var defaultType in EnumHelper.AllConverters(includeDefault: true))
                {
                    // attribute, default, expected
                    yield return new object[] { attributeType, defaultType, attributeType };
                }
            }

            foreach (var defaultType in EnumHelper.AllConverters(includeDefault: false))
            {
                // attribute, default, expected
                yield return new object[] { StronglyTypedIdConverter.Default, defaultType, defaultType };
            }

            yield return new object[] { StronglyTypedIdConverter.Default, StronglyTypedIdConverter.Default, StronglyTypedIdConfiguration.Defaults.Converters };
        }
    }
}