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

        internal static readonly ResourceCollection GuidResources = new(
            LoadEmbeddedResource("StronglyTypedIds.Templates.Guid.Guid_Base.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Guid.Guid_NewtonsoftJsonConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Guid.Guid_SystemTextJsonConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Guid.Guid_TypeConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Guid.Guid_EfCoreValueConverter.cs")
        );

        internal static readonly ResourceCollection IntResources = new(
            LoadEmbeddedResource("StronglyTypedIds.Templates.Int.Int_Base.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Int.Int_NewtonsoftJsonConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Int.Int_SystemTextJsonConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Int.Int_TypeConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Int.Int_EfCoreValueConverter.cs")
        );

        internal static readonly ResourceCollection LongResources = new(
            LoadEmbeddedResource("StronglyTypedIds.Templates.Long.Long_Base.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Long.Long_NewtonsoftJsonConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Long.Long_SystemTextJsonConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Long.Long_TypeConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Long.Long_EfCoreValueConverter.cs")
        );

        internal static readonly ResourceCollection StringResources = new(
            LoadEmbeddedResource("StronglyTypedIds.Templates.String.String_Base.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.String.String_NewtonsoftJsonConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.String.String_SystemTextJsonConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.String.String_TypeConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.String.String_EfCoreValueConverter.cs")
        );

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

        public readonly struct ResourceCollection
        {
            public string BaseId { get; }
            public string Newtonsoft { get; }
            public string SystemTextJson { get; }
            public string TypeConverter { get; }
            public string EfCoreValueConverter { get; }

            public ResourceCollection(string baseId, string newtonsoft, string systemTextJson, string typeConverter, string efCoreValueConverter)
            {
                BaseId = baseId;
                Newtonsoft = newtonsoft;
                SystemTextJson = systemTextJson;
                TypeConverter = typeConverter;
                EfCoreValueConverter = efCoreValueConverter;
            }
        }
    }
}