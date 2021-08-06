using System;

namespace StronglyTypedIds.Sources
{
    /// <summary>
    /// Converters used to to serialize/deserialize strongly-typed ID values
    /// </summary>
    [Flags]
    public enum StronglyTypedIdConverter
    {
        // Used with HasFlag, so needs to be 1, 2, 4 etc

        /// <summary>
        /// Don't create any converters for the strongly typed ID
        /// </summary>
        None = 0,

        /// <summary>
        /// Use the default converters for the strongly typed Id.
        /// This will be the value provided in the <see cref="StronglyTypedIdDefaultsAttribute"/>, which falls back to
        /// <see cref="TypeConverter"/> and <see cref="NewtonsoftJson"/>
        /// </summary>
        Default = 1,

        /// <summary>
        /// Creates a <see cref="TypeConverter"/> for converting from the strongly typed ID to and from a string
        /// </summary>
        TypeConverter = 2,

        /// <summary>
        /// Creates a Newtonsoft.Json.JsonConverter for serializing the strongly typed id to its primitive value
        /// </summary>
        NewtonsoftJson = 4,

        /// <summary>
        /// Creates a System.Text.Json.Serialization.JsonConverter for serializing the strongly typed id to its primitive value
        /// </summary>
        SystemTextJson = 8,
    }
}