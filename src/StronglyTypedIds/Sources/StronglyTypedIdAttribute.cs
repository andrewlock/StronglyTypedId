using System;

namespace StronglyTypedIds.Sources
{
    /// <summary>
    /// Place on partial structs to make the type a strongly-typed ID
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public class StronglyTypedIdAttribute : Attribute
    {
        /// <summary>
        /// Make the struct a strongly typed ID
        /// </summary>
        /// <param name="backingType">The <see cref="Type"/> to use to store the strongly-typed ID value.
        /// If not set, uses <see cref="StronglyTypedIdDefaultsAttribute.BackingType"/>, which defaults to <see cref="StronglyTypedIdBackingType.Guid"/></param>
        /// <param name="converters">Converters to create for serializing/deserializing the strongly-typed ID value.
        /// If not set, uses <see cref="StronglyTypedIdDefaultsAttribute.Converters"/>, which defaults to <see cref="StronglyTypedIdConverter.NewtonsoftJson"/></param>
        public StronglyTypedIdAttribute(
            StronglyTypedIdBackingType backingType = StronglyTypedIdBackingType.Default,
            StronglyTypedIdConverter converters = StronglyTypedIdConverter.Default)
        {
            BackingType = backingType;
            Converters = converters;
        }

        /// <summary>
        /// The <see cref="Type"/> to use to store the strongly-typed ID value
        /// </summary>
        public StronglyTypedIdBackingType BackingType { get; }

        /// <summary>
        /// JSON library used to serialize/deserialize strongly-typed ID value
        /// </summary>
        public StronglyTypedIdConverter Converters { get; }
    }
}