using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace StronglyTypedIds
{
    internal static class Sources
    {
        private static readonly Assembly ThisAssembly = typeof(Sources).Assembly;
        internal static readonly string StronglyTypedIdAttributeSource = LoadEmbeddedResource($"{nameof(StronglyTypedIds)}.Sources.{nameof(StronglyTypedIdAttribute)}.cs");
        internal static readonly string StronglyTypedIdBackingTypeSource = LoadEmbeddedResource($"{nameof(StronglyTypedIds)}.Sources.{nameof(StronglyTypedIdBackingType)}.cs");
        internal static readonly string StronglyTypedIdJsonConverterSource = LoadEmbeddedResource($"{nameof(StronglyTypedIds)}.Sources.{nameof(StronglyTypedIdJsonConverter)}.cs");

        private static readonly string GuidBase = LoadEmbeddedResource($"{nameof(StronglyTypedIds)}.Templates.Guid.Guid_Base.cs");
        private static readonly string GuidNewtonsoftBase = LoadEmbeddedResource($"{nameof(StronglyTypedIds)}.Templates.Guid.Guid_NewtonsoftJsonConverter.cs");
        private static readonly string GuidSystemTextJsonBase = LoadEmbeddedResource($"{nameof(StronglyTypedIds)}.Templates.Guid.Guid_SystemTextJsonConverter.cs");

        private const string NewtonsoftJsonAttributeSource = "    [Newtonsoft.Json.JsonConverter(typeof(TESTIDNewtonsoftJsonConverter))]";
        private const string SystemTextJsonAttributeSource = "    [System.Text.Json.Serialization.JsonConverter(typeof(TESTIDSystemTextJsonConverter))]";

        public static string CreateGuidId(
            string idNamespace,
            string idName,
            StronglyTypedIdJsonConverter? jsonConverter
        )
        {
            if (string.IsNullOrEmpty(idName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(idName));
            }

            var hasNamespace = !string.IsNullOrEmpty(idNamespace);

            var useNewtonsoftJson = jsonConverter is not null && jsonConverter.Value.HasFlag(StronglyTypedIdJsonConverter.NewtonsoftJson);
            var useSystemTextJson = jsonConverter is not null && jsonConverter.Value.HasFlag(StronglyTypedIdJsonConverter.SystemTextJson);
            var sb = new StringBuilder();
            if (hasNamespace)
            {
                sb
                    .Append("namespace ")
                    .Append(idNamespace)
                    .AppendLine(@"
{");
            }

            if (useNewtonsoftJson)
            {
                sb.AppendLine(NewtonsoftJsonAttributeSource);
            }

            if (useSystemTextJson)
            {
                sb.AppendLine(SystemTextJsonAttributeSource);
            }

            sb.Append(GuidBase);

            if (useNewtonsoftJson)
            {
                sb.AppendLine(GuidNewtonsoftBase);
            }

            if (useSystemTextJson)
            {
                sb.AppendLine(GuidSystemTextJsonBase);
            }

            sb.Replace("TESTID", idName);
            sb.AppendLine(@"    }");
            if (hasNamespace)
            {
                sb.Append('}').AppendLine();
            }
            return sb.ToString();
        }

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