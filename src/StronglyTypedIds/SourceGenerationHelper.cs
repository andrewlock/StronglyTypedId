using System;
using System.Text;
using StronglyTypedIds.Sources;

namespace StronglyTypedIds
{
    internal static class SourceGenerationHelper
    {
        public static string CreateId(
            string idNamespace,
            string idName,
            StronglyTypedIdConverter converters,
            StronglyTypedIdBackingType backingType)
        {
            var resources = backingType switch
            {
                StronglyTypedIdBackingType.Guid => EmbeddedSources.GuidResources,
                StronglyTypedIdBackingType.Int => EmbeddedSources.IntResources,
                StronglyTypedIdBackingType.Long => EmbeddedSources.LongResources,
                StronglyTypedIdBackingType.String => EmbeddedSources.StringResources,
                _ => throw new ArgumentException("Unknown backing type: " + backingType, nameof(backingType)),
            };

            return CreateId(idNamespace, idName, converters, resources);
        }

        static string CreateId(
            string idNamespace,
            string idName,
            StronglyTypedIdConverter converters,
            EmbeddedSources.ResourceCollection resources)
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
            var useEfCoreValueConverter = converters.IsSet(StronglyTypedIdConverter.EfCoreValueConverter);

            var sb = new StringBuilder(resources.Header);
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

            sb.Append(resources.BaseId);

            if (useEfCoreValueConverter)
            {
                sb.AppendLine(resources.EfCoreValueConverter);
            }

            if (useTypeConverter)
            {
                sb.AppendLine(resources.TypeConverter);
            }

            if (useNewtonsoftJson)
            {
                sb.AppendLine(resources.Newtonsoft);
            }

            if (useSystemTextJson)
            {
                sb.AppendLine(resources.SystemTextJson);
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