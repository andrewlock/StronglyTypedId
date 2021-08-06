using StronglyTypedIds.Sources;

namespace StronglyTypedIds
{
    internal readonly struct StronglyTypedIdConfiguration
    {
        public StronglyTypedIdBackingType BackingType { get; }

        public StronglyTypedIdConverter Converters { get; }

        public StronglyTypedIdConfiguration(
            StronglyTypedIdBackingType backingType,
            StronglyTypedIdConverter converters)
        {
            BackingType = backingType;
            Converters = converters;
        }

        /// <summary>
        /// Gets the default values for when a default attribute is not used.
        /// Should be kept in sync with the default values referenced in <see cref="StronglyTypedIdDefaultsAttribute"/>
        /// and <see cref="StronglyTypedIdAttribute"/>, but should always be "definite" values (not "Default")
        /// </summary>
        public static readonly StronglyTypedIdConfiguration Defaults = new(
            backingType: StronglyTypedIdBackingType.Guid,
            converters: StronglyTypedIdConverter.TypeConverter | StronglyTypedIdConverter.NewtonsoftJson);

        /// <summary>
        /// Combines multiple <see cref="StronglyTypedIdConfiguration"/> values associated
        /// with a given <see cref="StronglyTypedIdAttribute"/>, returning definite values.
        /// </summary>
        /// <returns></returns>
        public static StronglyTypedIdConfiguration Combine(
            StronglyTypedIdConfiguration attributeValues,
            StronglyTypedIdConfiguration? globalValues)
        {
            var backingType = (attributeValues.BackingType, globalValues?.BackingType) switch
            {
                (StronglyTypedIdBackingType.Default, null) => Defaults.BackingType,
                (StronglyTypedIdBackingType.Default, StronglyTypedIdBackingType.Default) => Defaults.BackingType,
                (StronglyTypedIdBackingType.Default, var globalDefault) => globalDefault.Value,
                (var specificValue, _) => specificValue
            };

            var converter = (attributeValues.Converters, globalValues?.Converters) switch
            {
                (StronglyTypedIdConverter.Default, null) => Defaults.Converters,
                (StronglyTypedIdConverter.Default, StronglyTypedIdConverter.Default) => Defaults.Converters,
                (StronglyTypedIdConverter.Default, var globalDefault) => globalDefault.Value,
                (var specificValue, _) => specificValue
            };

            return new StronglyTypedIdConfiguration(backingType, converter);
        }
    }
}