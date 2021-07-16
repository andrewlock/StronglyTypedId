using System;
using System.Text;
using StronglyTypedIds.Sources;

namespace StronglyTypedIds
{
    internal static class SourceGenerationHelper
    {
        public static string CreateGuidId(string idNamespace, string idName, StronglyTypedIdJsonConverter? jsonConverter) =>
            CreateId(
                idNamespace,
                idName,
                jsonConverter,
                EmbeddedSources.GuidBase,
                EmbeddedSources.GuidNewtonsoftBase,
                EmbeddedSources.GuidSystemTextJsonBase);

        public static string CreateIntId(string idNamespace, string idName, StronglyTypedIdJsonConverter? jsonConverter) =>
            CreateId(
                idNamespace,
                idName,
                jsonConverter,
                EmbeddedSources.IntBase,
                EmbeddedSources.IntNewtonsoftBase,
                EmbeddedSources.IntSystemTextJsonBase);

        public static string CreateLongId(string idNamespace, string idName, StronglyTypedIdJsonConverter? jsonConverter) =>
            CreateId(
                idNamespace,
                idName,
                jsonConverter,
                EmbeddedSources.LongBase,
                EmbeddedSources.LongNewtonsoftBase,
                EmbeddedSources.LongSystemTextJsonBase);

        static string CreateId(
            string idNamespace,
            string idName,
            StronglyTypedIdJsonConverter? jsonConverter,
            string baseSource,
            string newtonsoftSource,
            string systemTextSource)
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
                sb.AppendLine(EmbeddedSources.NewtonsoftJsonAttributeSource);
            }

            if (useSystemTextJson)
            {
                sb.AppendLine(EmbeddedSources.SystemTextJsonAttributeSource);
            }

            sb.Append(baseSource);

            if (useNewtonsoftJson)
            {
                sb.AppendLine(newtonsoftSource);
            }

            if (useSystemTextJson)
            {
                sb.AppendLine(systemTextSource);
            }

            sb.Replace("TESTID", idName);
            sb.AppendLine(@"    }");
            if (hasNamespace)
            {
                sb.Append('}').AppendLine();
            }

            return sb.ToString();
        }
    }
}