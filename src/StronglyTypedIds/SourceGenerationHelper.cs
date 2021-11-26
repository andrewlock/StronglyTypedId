using System;
using System.Collections.Generic;
using System.Text;
using StronglyTypedIds.Sources;

namespace StronglyTypedIds
{
    internal static class SourceGenerationHelper
    {
        public static string CreateId(
            string idNamespace,
            string idName,
            ParentClass? parentClass,
            StronglyTypedIdConverter converters,
            StronglyTypedIdBackingType backingType,
            StronglyTypedIdImplementations implementations)
            => CreateId(idNamespace, idName, parentClass, converters, backingType, implementations, null);

        public static string CreateId(
            string idNamespace,
            string idName,
            ParentClass? parentClass,
            StronglyTypedIdConverter converters,
            StronglyTypedIdBackingType backingType,
            StronglyTypedIdImplementations implementations,
            StringBuilder? sb)
        {
            var resources = backingType switch
            {
                StronglyTypedIdBackingType.Guid => EmbeddedSources.GuidResources,
                StronglyTypedIdBackingType.Int => EmbeddedSources.IntResources,
                StronglyTypedIdBackingType.Long => EmbeddedSources.LongResources,
                StronglyTypedIdBackingType.String => EmbeddedSources.StringResources,
                StronglyTypedIdBackingType.NullableString => EmbeddedSources.NullableStringResources,
                _ => throw new ArgumentException("Unknown backing type: " + backingType, nameof(backingType)),
            };

            return CreateId(idNamespace, idName, parentClass, converters, implementations, resources, sb);
        }

        static string CreateId(
            string idNamespace,
            string idName,
            ParentClass? parentClass,
            StronglyTypedIdConverter converters,
            StronglyTypedIdImplementations implementations,
            EmbeddedSources.ResourceCollection resources,
            StringBuilder? sb)
        {
            if (string.IsNullOrEmpty(idName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(idName));
            }

            if (converters == StronglyTypedIdConverter.Default)
            {
                throw new ArgumentException("Cannot use default converter - must provide concrete values or None", nameof(converters));
            }

            if (implementations == StronglyTypedIdImplementations.Default)
            {
                throw new ArgumentException("Cannot use default implementations - must provide concrete values or None", nameof(implementations));
            }

            var hasNamespace = !string.IsNullOrEmpty(idNamespace);

            var useTypeConverter = converters.IsSet(StronglyTypedIdConverter.TypeConverter);
            var useNewtonsoftJson = converters.IsSet(StronglyTypedIdConverter.NewtonsoftJson);
            var useSystemTextJson = converters.IsSet(StronglyTypedIdConverter.SystemTextJson);
            var useEfCoreValueConverter = converters.IsSet(StronglyTypedIdConverter.EfCoreValueConverter);
            var useDapperTypeHandler = converters.IsSet(StronglyTypedIdConverter.DapperTypeHandler);

            var useIEquatable = implementations.IsSet(StronglyTypedIdImplementations.IEquatable);
            var useIComparable = implementations.IsSet(StronglyTypedIdImplementations.IComparable);

            var parentsCount = 0;

            sb ??= new StringBuilder();
            sb.Append(resources.Header);

            if (resources.NullableEnable)
            {
                sb.AppendLine("#nullable enable");
            }

            if (hasNamespace)
            {
                sb
                    .Append("namespace ")
                    .Append(idNamespace)
                    .AppendLine(@"
{");
            }

            while (parentClass is not null)
            {
                sb
                    .Append("    partial ")
                    .Append(parentClass.Keyword)
                    .Append(' ')
                    .Append(parentClass.Name)
                    .Append(' ')
                    .Append(parentClass.Constraints)
                    .AppendLine(@"
    {");
                parentsCount++;
                parentClass = parentClass.Child;
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
            ReplaceInterfaces(sb, useIEquatable, useIComparable);

            // IEquatable is already implemented whether or not the interface is implemented

            if (useIComparable)
            {
                sb.AppendLine(resources.Comparable);
            }

            if (useEfCoreValueConverter)
            {
                sb.AppendLine(resources.EfCoreValueConverter);
            }

            if (useDapperTypeHandler)
            {
                sb.AppendLine(resources.DapperTypeHandler);
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

            for (int i = 0; i < parentsCount; i++)
            {
                sb.AppendLine(@"    }");
            }

            if (hasNamespace)
            {
                sb.Append('}').AppendLine();
            }

            return sb.ToString();
        }

        private static void ReplaceInterfaces(StringBuilder sb, bool useIEquatable, bool useIComparable)
        {
            var interfaces = new List<string>();

            if (useIComparable)
            {
                interfaces.Add("System.IComparable<TESTID>");
            }

            if (useIEquatable)
            {
                interfaces.Add("System.IEquatable<TESTID>");
            }

            if (interfaces.Count > 0)
            {
                sb.Replace("INTERFACES", string.Join(", ", interfaces));
            }
            else
            {
                sb.Replace(": INTERFACES", string.Empty);
            }
        }
    }
}