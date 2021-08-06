using System;
using System.Diagnostics;

namespace StronglyTypedIds.Sources
{
    /// <summary>
    /// Used to control the default Place on partial structs to make the type a strongly-typed ID
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    public sealed class StronglyTypedIdDefaultsAttribute : Attribute
    {
        /// <summary>
        /// Set the default values used for strongly typed ids
        /// </summary>
        /// <param name="backingType">The <see cref="Type"/> to use to store the strongly-typed ID value.
        /// Defaults to <see cref="StronglyTypedIdBackingType.Guid"/></param>
        /// <param name="converters">JSON library used to serialize/deserialize strongly-typed ID value.
        /// Defaults to <see cref="StronglyTypedIdConverter.NewtonsoftJson"/> and <see cref="StronglyTypedIdConverter.TypeConverter"/></param>
        public StronglyTypedIdDefaultsAttribute(
            StronglyTypedIdBackingType backingType = StronglyTypedIdBackingType.Default,
            StronglyTypedIdConverter converters = StronglyTypedIdConverter.Default)
        {
            BackingType = backingType;
            Converters = converters;
        }

        /// <summary>
        /// The default <see cref="Type"/> to use to store the strongly-typed ID values.
        /// </summary>
        public StronglyTypedIdBackingType BackingType { get; }

        /// <summary>
        /// The default converters to create for serializing/deserializing strongly-typed ID values.
        /// </summary>
        public StronglyTypedIdConverter Converters { get; }
    }
}