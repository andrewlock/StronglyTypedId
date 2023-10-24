using System;

namespace StronglyTypedIds
{
    /// <summary>
    /// Place on partial structs to make the type a strongly-typed ID
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
    [System.Diagnostics.Conditional("STRONGLY_TYPED_ID_USAGES")]
    public sealed class StronglyTypedIdAttribute : Attribute
    {
        /// <summary>
        /// Make the struct a strongly typed ID
        /// </summary>
        /// <param name="backingType">The <see cref="Type"/> to use to store the strongly-typed ID value.
        /// If not set, uses <see cref="StronglyTypedIdDefaultsAttribute.BackingType"/>, which defaults to <see cref="StronglyTypedIdBackingType.Guid"/></param>
        /// <param name="converters">Converters to create for serializing/deserializing the strongly-typed ID value.
        /// If not set, uses <see cref="StronglyTypedIdDefaultsAttribute.Converters"/>, which defaults to <see cref="StronglyTypedIdConverter.NewtonsoftJson"/>
        /// and <see cref="StronglyTypedIdConverter.TypeConverter"/></param>
        /// <param name="implementations">Interfaces and patterns the strongly typed id should implement
        /// If not set, uses <see cref="StronglyTypedIdDefaultsAttribute.Implementations"/>, which defaults to <see cref="StronglyTypedIdImplementations.IEquatable"/>
        /// and <see cref="StronglyTypedIdImplementations.IComparable"/></param>
        [Obsolete("This overload is no longer used. Please use the StronglyTypedId(Template) or StronglyTypedId(string) constructor")]
        public StronglyTypedIdAttribute(
            StronglyTypedIdBackingType backingType = StronglyTypedIdBackingType.Default,
            StronglyTypedIdConverter converters = StronglyTypedIdConverter.Default,
            StronglyTypedIdImplementations implementations = StronglyTypedIdImplementations.Default)
        {
            BackingType = backingType;
            Converters = converters;
            Implementations = implementations;
        }

        /// <summary>
        /// Make the struct a strongly typed ID.
        /// </summary>
        /// <param name="templateName">The name of the template to use to generate the ID.
        /// Templates must be added to the project using the format NAME.typedid,
        /// where NAME is the name of the template passed in <paramref name="templateName"/>.
        /// </param>
        public StronglyTypedIdAttribute(string templateName)
        {
            TemplateName = templateName;
        }

        /// <summary>
        /// Make the struct a strongly typed ID.
        /// </summary>
        /// <param name="template">The built-in template to use to generate the ID.</param>
        public StronglyTypedIdAttribute(Template template)
        {
            Template = template;
        }

        /// <summary>
        /// Make the struct a strongly typed ID, using the default template
        /// </summary>
        public StronglyTypedIdAttribute()
        {
        }

        /// <summary>
        /// The <see cref="Type"/> to use to store the strongly-typed ID value
        /// </summary>
        public StronglyTypedIdBackingType BackingType { get; }

        /// <summary>
        /// JSON library used to serialize/deserialize strongly-typed ID value
        /// </summary>
        public StronglyTypedIdConverter Converters { get; }

        /// <summary>
        /// Interfaces and patterns the strongly typed id should implement
        /// </summary>
        public StronglyTypedIdImplementations Implementations { get; }

        /// <summary>
        /// The template to use to generate the strongly-typed ID value.
        /// </summary>
        public string? TemplateName { get; }

        /// <summary>
        /// The template to use to generate the strongly-typed ID value.
        /// </summary>
        public Template? Template { get; }
    }
}