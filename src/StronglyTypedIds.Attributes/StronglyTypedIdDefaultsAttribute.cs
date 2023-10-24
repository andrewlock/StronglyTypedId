using System;

namespace StronglyTypedIds
{
    /// <summary>
    /// Used to control the default strongly typed ID values. Apply to an assembly using
    /// <code>[assembly:StronglyTypedIdDefaults(Template.Int)]</code> for example
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    [System.Diagnostics.Conditional("STRONGLY_TYPED_ID_USAGES")]
    public sealed class StronglyTypedIdDefaultsAttribute : Attribute
    {
        /// <summary>
        /// Set the default values used for strongly typed ids
        /// </summary>
        /// <param name="backingType">The <see cref="Type"/> to use to store the strongly-typed ID value.
        /// Defaults to <see cref="StronglyTypedIdBackingType.Guid"/></param>
        /// <param name="converters">JSON library used to serialize/deserialize strongly-typed ID value.
        /// Defaults to <see cref="StronglyTypedIdConverter.NewtonsoftJson"/> and <see cref="StronglyTypedIdConverter.TypeConverter"/></param>
        /// <param name="implementations">Interfaces and patterns the strongly typed id should implement
        /// Defaults to <see cref="StronglyTypedIdImplementations.IEquatable"/> and <see cref="StronglyTypedIdImplementations.IComparable"/></param>
        [Obsolete("This overload is no longer used. Please use the StronglyTypedId(Template) or StronglyTypedId(string) constructor")]
        public StronglyTypedIdDefaultsAttribute(
            StronglyTypedIdBackingType backingType = StronglyTypedIdBackingType.Default,
            StronglyTypedIdConverter converters = StronglyTypedIdConverter.Default,
            StronglyTypedIdImplementations implementations = StronglyTypedIdImplementations.Default)
        {
            BackingType = backingType;
            Converters = converters;
            Implementations = implementations;
        }

        /// <summary>
        /// Set the default template to use for strongly typed IDs
        /// </summary>
        /// <param name="template">The built-in template to use to generate the ID.</param>
        public StronglyTypedIdDefaultsAttribute(Template template)
        {
            Template = template;
        }

        /// <summary>
        /// Set the default template to use for strongly typed IDs
        /// </summary>
        /// <param name="templateName">The name of the template to use to generate the ID.
        /// Templates must be added to the project using the format NAME.typedid,
        /// where NAME is the name of the template passed in <paramref name="templateName"/>.
        /// </param>
        public StronglyTypedIdDefaultsAttribute(string templateName)
        {
            TemplateName = templateName;
        }

        /// <summary>
        /// The default <see cref="Type"/> to use to store the strongly-typed ID values.
        /// </summary>
        public StronglyTypedIdBackingType BackingType { get; }

        /// <summary>
        /// The default converters to create for serializing/deserializing strongly-typed ID values.
        /// </summary>
        public StronglyTypedIdConverter Converters { get; }

        /// <summary>
        /// Interfaces and patterns the strongly typed id should implement
        /// </summary>
        public StronglyTypedIdImplementations Implementations { get; }

        /// <summary>
        /// The default template to use to generate the strongly-typed ID value.
        /// </summary>
        public string? TemplateName { get; }

        /// <summary>
        /// The default template to use to generate the strongly-typed ID value.
        /// </summary>
        public Template? Template { get; }
    }
}