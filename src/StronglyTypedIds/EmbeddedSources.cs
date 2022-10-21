using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace StronglyTypedIds
{
    internal static class EmbeddedSources
    {
        private static readonly Assembly ThisAssembly = typeof(EmbeddedSources).Assembly;
        internal static readonly string StronglyTypedIdAttributeSource = LoadTemplateForEmitting("StronglyTypedIdAttribute");
        internal static readonly string StronglyTypedIdDefaultsAttributeSource = LoadTemplateForEmitting("StronglyTypedIdDefaultsAttribute");
        internal static readonly string StronglyTypedIdBackingTypeSource = LoadTemplateForEmitting("StronglyTypedIdBackingType");
        internal static readonly string StronglyTypedIdConverterSource = LoadTemplateForEmitting("StronglyTypedIdConverter");
        internal static readonly string StronglyTypedIdImplementationsSource = LoadTemplateForEmitting("StronglyTypedIdImplementations");

        private static readonly string AutoGeneratedHeader = LoadEmbeddedResource("StronglyTypedIds.Templates.AutoGeneratedHeader.cs");

        internal static readonly ResourceCollection GuidResources = new(
            AutoGeneratedHeader,
            LoadEmbeddedResource("StronglyTypedIds.Templates.Guid.Guid_Base.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Guid.Guid_NewtonsoftJsonConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Guid.Guid_SystemTextJsonConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Guid.Guid_TypeConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Guid.Guid_EfCoreValueConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Guid.Guid_DapperTypeHandler.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Guid.Guid_IComparable.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Guid.Guid_SwaggerSchemaFilter.cs"),
            false
        );

        internal static readonly ResourceCollection IntResources = new(
            AutoGeneratedHeader,
            LoadEmbeddedResource("StronglyTypedIds.Templates.Int.Int_Base.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Int.Int_NewtonsoftJsonConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Int.Int_SystemTextJsonConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Int.Int_TypeConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Int.Int_EfCoreValueConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Int.Int_DapperTypeHandler.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Int.Int_IComparable.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Int.Int_SwaggerSchemaFilter.cs"),
            false
        );

        internal static readonly ResourceCollection LongResources = new(
            AutoGeneratedHeader,
            LoadEmbeddedResource("StronglyTypedIds.Templates.Long.Long_Base.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Long.Long_NewtonsoftJsonConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Long.Long_SystemTextJsonConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Long.Long_TypeConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Long.Long_EfCoreValueConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Long.Long_DapperTypeHandler.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Long.Long_IComparable.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.Long.Long_SwaggerSchemaFilter.cs"),
            false
        );

        internal static readonly ResourceCollection StringResources = new(
            AutoGeneratedHeader,
            LoadEmbeddedResource("StronglyTypedIds.Templates.String.String_Base.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.String.String_NewtonsoftJsonConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.String.String_SystemTextJsonConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.String.String_TypeConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.String.String_EfCoreValueConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.String.String_DapperTypeHandler.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.String.String_IComparable.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.String.String_SwaggerSchemaFilter.cs"),
            false
        );

        internal static readonly ResourceCollection NullableStringResources = new(
            AutoGeneratedHeader,
            LoadEmbeddedResource("StronglyTypedIds.Templates.NullableString.NullableString_Base.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.NullableString.NullableString_NewtonsoftJsonConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.NullableString.NullableString_SystemTextJsonConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.NullableString.NullableString_TypeConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.NullableString.NullableString_EfCoreValueConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.NullableString.NullableString_DapperTypeHandler.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.NullableString.NullableString_IComparable.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.NullableString.NullableString_SwaggerSchemaFilter.cs"),
            true
        );

        internal static readonly ResourceCollection NewIdResources = new(
            AutoGeneratedHeader,
            LoadEmbeddedResource("StronglyTypedIds.Templates.NewId.NewId_Base.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.NewId.NewId_NewtonsoftJsonConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.NewId.NewId_SystemTextJsonConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.NewId.NewId_TypeConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.NewId.NewId_EfCoreValueConverter.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.NewId.NewId_DapperTypeHandler.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.NewId.NewId_IComparable.cs"),
            LoadEmbeddedResource("StronglyTypedIds.Templates.NewId.NewId_SwaggerSchemaFilter.cs"),
            false
        );

        internal const string TypeConverterAttributeSource = "    [System.ComponentModel.TypeConverter(typeof(TESTIDTypeConverter))]";
        internal const string NewtonsoftJsonAttributeSource = "    [Newtonsoft.Json.JsonConverter(typeof(TESTIDNewtonsoftJsonConverter))]";
        internal const string SystemTextJsonAttributeSource = "    [System.Text.Json.Serialization.JsonConverter(typeof(TESTIDSystemTextJsonConverter))]";
        internal const string SwaggerSchemaFilterAttributeSource = "    [Swashbuckle.AspNetCore.Annotations.SwaggerSchemaFilter(typeof(TESTIDSchemaFilter))]";

        internal static string LoadEmbeddedResource(string resourceName)
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
            public string SwaggerSchemaFilter { get; }
            public string Header { get; }
            public bool NullableEnable { get; }
            public string BaseId { get; }
            public string Newtonsoft { get; }
            public string SystemTextJson { get; }
            public string TypeConverter { get; }
            public string EfCoreValueConverter { get; }
            public string DapperTypeHandler { get; }
            public string Comparable { get; }

            public ResourceCollection(
                string header,
                string baseId,
                string newtonsoft,
                string systemTextJson,
                string typeConverter,
                string efCoreValueConverter,
                string dapperTypeHandler,
                string comparable,
                string swaggerSchemaFilter,
                bool nullableEnable)
            {
                SwaggerSchemaFilter = swaggerSchemaFilter;
                BaseId = baseId;
                Newtonsoft = newtonsoft;
                SystemTextJson = systemTextJson;
                TypeConverter = typeConverter;
                EfCoreValueConverter = efCoreValueConverter;
                DapperTypeHandler = dapperTypeHandler;
                Comparable = comparable;
                NullableEnable = nullableEnable;
                Header = header;
            }
        }

        internal static string LoadTemplateForEmitting(string resourceName)
        {
            var resource = LoadEmbeddedResource($"StronglyTypedIds.Templates.Sources.{resourceName}.cs");
            return AutoGeneratedHeader + @"#if STRONGLY_TYPED_ID_EMBED_ATTRIBUTES

" + resource
                       .Replace("public sealed", "internal sealed")
                       .Replace("public enum", "internal enum")
                   + @"
#endif";
        }
    }
}