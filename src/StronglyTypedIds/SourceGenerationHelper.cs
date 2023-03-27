using System;
using System.Collections.Generic;
using System.Text;

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
            StronglyTypedIdImplementations implementations,
            StronglyTypedIdConstructor constructor)
            => CreateId(idNamespace, idName, parentClass, converters, backingType, implementations, constructor, null);

        public static string CreateId(
            string idNamespace,
            string idName,
            ParentClass? parentClass,
            StronglyTypedIdConverter converters,
            StronglyTypedIdBackingType backingType,
            StronglyTypedIdImplementations implementations,
            StronglyTypedIdConstructor constructor,
            StringBuilder? sb)
        {
            var resources = backingType switch
            {
                StronglyTypedIdBackingType.Guid => EmbeddedSources.GuidResources,
                StronglyTypedIdBackingType.Int => EmbeddedSources.IntResources,
                StronglyTypedIdBackingType.Long => EmbeddedSources.LongResources,
                StronglyTypedIdBackingType.String => EmbeddedSources.StringResources,
                StronglyTypedIdBackingType.NullableString => EmbeddedSources.NullableStringResources,
                StronglyTypedIdBackingType.MassTransitNewId => EmbeddedSources.NewIdResources,
                _ => throw new ArgumentException("Unknown backing type: " + backingType, nameof(backingType)),
            };

            return CreateId(idNamespace, idName, parentClass, converters, implementations, constructor, resources, sb);
        }

        static string CreateId(
            string idNamespace,
            string idName,
            ParentClass? parentClass,
            StronglyTypedIdConverter converters,
            StronglyTypedIdImplementations implementations,
            StronglyTypedIdConstructor constructor,
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

            if (constructor == StronglyTypedIdConstructor.Default)
            {
                throw new ArgumentException("Cannot use default constructor - must provide concrete value or Public", nameof(constructor));
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
            ReplaceConstructor(sb, resources.Constructor, constructor);

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
        private static void ReplaceConstructor(StringBuilder sb, string template, StronglyTypedIdConstructor constructor)
        {
            var visibility = constructor switch
            {
                StronglyTypedIdConstructor.Public => "public",
                StronglyTypedIdConstructor.Private => "private",
                StronglyTypedIdConstructor.Internal => "internal",
                StronglyTypedIdConstructor.None => null,
                _ => null,
            };

            if (visibility is null)
            {
                sb.Replace("CONSTRUCTOR", string.Empty);
            }
            else
            {
                sb.Replace("CONSTRUCTOR", template.Replace("CONSTRUCTOR_VISIBILITY", visibility));
            }
        }

        internal static string CreateSourceName(string nameSpace, ParentClass? parent, string name)
        {
            var sb = new StringBuilder(nameSpace).Append('.');
            while (parent != null)
            {
                var s = parent.Name
                    .Replace(" ", "")
                    .Replace(",", "")
                    .Replace("<", "__")
                    .Replace(">", "");
                sb.Append(s).Append('.');
                parent = parent.Child;
            }
            return sb.Append(name).Append(".g.cs").ToString();
        }
    }
}
