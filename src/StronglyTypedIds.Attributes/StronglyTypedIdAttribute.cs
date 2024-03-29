//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the StronglyTypedId source generator
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#pragma warning disable 1591 // publicly visible type or member must be documented

#nullable enable

namespace StronglyTypedIds
{
    /// <summary>
    /// Place on partial structs to make the type a strongly-typed ID
    /// </summary>
    [global::System.AttributeUsage(global::System.AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    [global::System.Diagnostics.Conditional("STRONGLY_TYPED_ID_USAGES")]
    public sealed class StronglyTypedIdAttribute : global::System.Attribute
    {
        /// <summary>
        /// Make the struct a strongly typed ID.
        /// </summary>
        /// <param name="template">The built-in template to use to generate the ID.</param>
        /// <param name="templateNames">The names of additional custom templates to use to generate the ID.
        /// Templates must be added to the project using the format NAME.typedid,
        /// where NAME is the name of the template passed in <paramref name="templateNames"/>.
        /// </param>
        public StronglyTypedIdAttribute(global::StronglyTypedIds.Template template, params string[] templateNames)
        {
        }

        /// <summary>
        /// Make the struct a strongly typed ID.
        /// </summary>
        /// <param name="templateNames">The names of the template to use to generate the ID.
        /// Templates must be added to the project using the format NAME.typedid,
        /// where NAME is the name of the template passed in <paramref name="templateNames"/>.
        /// If no templates are provided, the default value is used, as specified by
        /// <see cref="StronglyTypedIdDefaultsAttribute"/>, or alternatively the
        /// <see cref="Template.Guid"/> template.
        /// </param>
        public StronglyTypedIdAttribute(params string[] templateNames)
        {
        }
    }
}