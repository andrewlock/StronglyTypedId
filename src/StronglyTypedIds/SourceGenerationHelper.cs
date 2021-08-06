using System;
using System.Text;
using StronglyTypedIds.Sources;

namespace StronglyTypedIds
{
    internal static class SourceGenerationHelper
    {
        public static string CreateGuidId(string idNamespace, string idName, StronglyTypedIdConverter converters) =>
            CreateId(
                idNamespace,
                idName,
                converters,
                EmbeddedSources.GuidBase,
                EmbeddedSources.GuidNewtonsoft,
                EmbeddedSources.GuidSystemTextJson,
                EmbeddedSources.GuidTypeConverter);

        public static string CreateIntId(string idNamespace, string idName, StronglyTypedIdConverter converters) =>
            CreateId(
                idNamespace,
                idName,
                converters,
                EmbeddedSources.IntBase,
                EmbeddedSources.IntNewtonsoft,
                EmbeddedSources.IntSystemTextJson,
                EmbeddedSources.IntTypeConverter);

        public static string CreateLongId(string idNamespace, string idName, StronglyTypedIdConverter converters) =>
            CreateId(
                idNamespace,
                idName,
                converters,
                EmbeddedSources.LongBase,
                EmbeddedSources.LongNewtonsoft,
                EmbeddedSources.LongSystemTextJson,
                EmbeddedSources.LongTypeConverter);

        public static string CreateStringId(string idNamespace, string idName, StronglyTypedIdConverter converters) =>
            CreateId(
                idNamespace,
                idName,
                converters,
                EmbeddedSources.StringBase,
                EmbeddedSources.StringNewtonsoft,
                EmbeddedSources.StringSystemTextJson,
                EmbeddedSources.StringTypeConverter);

        static string CreateId(
            string idNamespace,
            string idName,
            StronglyTypedIdConverter converters,
            string baseSource,
            string newtonsoftSource,
            string systemTextSource,
            string typeConverterSource)
        {
            if (string.IsNullOrEmpty(idName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(idName));
            }

            if (converters == StronglyTypedIdConverter.Default)
            {
                throw new ArgumentException("Cannot use default converter - must provide concrete values or None", nameof(idName));
            }

            var hasNamespace = !string.IsNullOrEmpty(idNamespace);

            var useTypeConverter = converters.IsSet(StronglyTypedIdConverter.TypeConverter);
            var useNewtonsoftJson = converters.IsSet(StronglyTypedIdConverter.NewtonsoftJson);
            var useSystemTextJson = converters.IsSet(StronglyTypedIdConverter.SystemTextJson);

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

            if (useTypeConverter)
            {
                sb.AppendLine(EmbeddedSources.TypeConverterAttributeSource);
            }

            sb.Append(baseSource);

            if (useTypeConverter)
            {
                sb.AppendLine(typeConverterSource);
            }

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