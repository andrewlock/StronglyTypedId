using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace StronglyTypedIds
{
    internal static class EmbeddedSources
    {
        private static readonly Assembly ThisAssembly = typeof(EmbeddedSources).Assembly;
        internal static readonly string StronglyTypedIdAttributeSource = LoadEmbeddedResource("StronglyTypedIds.Templates.Sources.StronglyTypedIdAttribute.cs");
        internal static readonly string StronglyTypedIdDefaultsAttributeSource = LoadEmbeddedResource("StronglyTypedIds.Templates.Sources.StronglyTypedIdDefaultsAttribute.cs");
        internal static readonly string StronglyTypedIdBackingTypeSource = LoadEmbeddedResource("StronglyTypedIds.Templates.Sources.StronglyTypedIdBackingType.cs");
        internal static readonly string StronglyTypedIdConverterSource = LoadEmbeddedResource("StronglyTypedIds.Templates.Sources.StronglyTypedIdConverter.cs");

        internal static readonly string GuidBase = LoadEmbeddedResource("StronglyTypedIds.Templates.Guid.Guid_Base.cs");
        internal static readonly string GuidNewtonsoft = LoadEmbeddedResource("StronglyTypedIds.Templates.Guid.Guid_NewtonsoftJsonConverter.cs");
        internal static readonly string GuidSystemTextJson = LoadEmbeddedResource("StronglyTypedIds.Templates.Guid.Guid_SystemTextJsonConverter.cs");
        internal static readonly string GuidTypeConverter = LoadEmbeddedResource("StronglyTypedIds.Templates.Guid.Guid_TypeConverter.cs");

        internal static readonly string IntBase = LoadEmbeddedResource("StronglyTypedIds.Templates.Int.Int_Base.cs");
        internal static readonly string IntNewtonsoft = LoadEmbeddedResource("StronglyTypedIds.Templates.Int.Int_NewtonsoftJsonConverter.cs");
        internal static readonly string IntSystemTextJson = LoadEmbeddedResource("StronglyTypedIds.Templates.Int.Int_SystemTextJsonConverter.cs");
        internal static readonly string IntTypeConverter = LoadEmbeddedResource("StronglyTypedIds.Templates.Int.Int_TypeConverter.cs");

        internal static readonly string LongBase = LoadEmbeddedResource("StronglyTypedIds.Templates.Long.Long_Base.cs");
        internal static readonly string LongNewtonsoft = LoadEmbeddedResource("StronglyTypedIds.Templates.Long.Long_NewtonsoftJsonConverter.cs");
        internal static readonly string LongSystemTextJson = LoadEmbeddedResource("StronglyTypedIds.Templates.Long.Long_SystemTextJsonConverter.cs");
        internal static readonly string LongTypeConverter = LoadEmbeddedResource("StronglyTypedIds.Templates.Long.Long_TypeConverter.cs");

        internal static readonly string StringBase = LoadEmbeddedResource("StronglyTypedIds.Templates.String.String_Base.cs");
        internal static readonly string StringNewtonsoft = LoadEmbeddedResource("StronglyTypedIds.Templates.String.String_NewtonsoftJsonConverter.cs");
        internal static readonly string StringSystemTextJson = LoadEmbeddedResource("StronglyTypedIds.Templates.String.String_SystemTextJsonConverter.cs");
        internal static readonly string StringTypeConverter = LoadEmbeddedResource("StronglyTypedIds.Templates.String.String_TypeConverter.cs");

        internal const string TypeConverterAttributeSource = "    [System.ComponentModel.TypeConverter(typeof(TESTIDTypeConverter))]";
        internal const string NewtonsoftJsonAttributeSource = "    [Newtonsoft.Json.JsonConverter(typeof(TESTIDNewtonsoftJsonConverter))]";
        internal const string SystemTextJsonAttributeSource = "    [System.Text.Json.Serialization.JsonConverter(typeof(TESTIDSystemTextJsonConverter))]";

        private static string LoadEmbeddedResource(string resourceName)
        {
            var resourceStream = ThisAssembly.GetManifestResourceStream(resourceName);
            if (resourceStream is null)
            {
                var existingResources = ThisAssembly.GetManifestResourceNames();
                throw new ArgumentException($"Could not find embedded resource {resourceName}. Available names: {string.Join(", ", existingResources)}");
            }

            using var reader = new StreamReader(resourceStream, Encoding.UTF8);

            return reader.ReadToEnd();
        }
    }
}