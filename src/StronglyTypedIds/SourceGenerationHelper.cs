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
                StronglyTypedIdBackingType.MassTransitNewId => EmbeddedSources.NewIdResources,
                _ => throw new ArgumentException("Unknown backing type: " + backingType, nameof(backingType)),
            };

            return CreateId(idNamespace, idName, parentClass, converters, implementations, resources,backingType, sb);
        }

        static string CreateId(
            string idNamespace,
            string idName,
            ParentClass? parentClass,
            StronglyTypedIdConverter converters,
            StronglyTypedIdImplementations implementations,
            EmbeddedSources.ResourceCollection resources,
            StronglyTypedIdBackingType backingType,
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

            var useSchemaFilter = converters.IsSet(StronglyTypedIdConverter.SwaggerSchemaFilter);
            var useTypeConverter = converters.IsSet(StronglyTypedIdConverter.TypeConverter);
            var useNewtonsoftJson = converters.IsSet(StronglyTypedIdConverter.NewtonsoftJson);
            var useSystemTextJson = converters.IsSet(StronglyTypedIdConverter.SystemTextJson);
            var useEfCoreValueConverter = converters.IsSet(StronglyTypedIdConverter.EfCoreValueConverter);
            var useDapperTypeHandler = converters.IsSet(StronglyTypedIdConverter.DapperTypeHandler);
            var useAutoMapperTypeHandler = converters.IsSet(StronglyTypedIdConverter.AutoMapper);
            var useLinqToDbTypeHandler = converters.IsSet(StronglyTypedIdConverter.LinqToDb);

            var useIEquatable = implementations.IsSet(StronglyTypedIdImplementations.IEquatable);
            var useIComparable = implementations.IsSet(StronglyTypedIdImplementations.IComparable);
            var useIParsable = implementations.IsSet(StronglyTypedIdImplementations.IParsable);
            var useIConvertible = implementations.IsSet(StronglyTypedIdImplementations.IConvertible);
            var useIStronglyTypedId = implementations.IsSet(StronglyTypedIdImplementations.IStronglyTypedId);

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

            if (useSchemaFilter)
            {
                sb.AppendLine(EmbeddedSources.SwaggerSchemaFilterAttributeSource);
            }


            sb.Append(resources.BaseId);
            ReplaceInterfaces(sb, useIEquatable, useIComparable, useIParsable, useIConvertible, useIStronglyTypedId, backingType);

            // IEquatable is already implemented whether or not the interface is implemented

            if (useIComparable)
            {
                sb.AppendLine(resources.Comparable);
            }

            if (useIParsable)
            {
                sb.AppendLine(resources.Parsable);
            }

            if (useIConvertible)
            {
                sb.AppendLine(resources.Convertible);
            }

            if (useIStronglyTypedId)
            {
                sb.AppendLine(resources.StronglyTypedId);
            }

            if (useEfCoreValueConverter)
            {
                sb.AppendLine(resources.EfCoreValueConverter);
            }

            if (useDapperTypeHandler)
            {
                sb.AppendLine(resources.DapperTypeHandler);
            }

            if (useAutoMapperTypeHandler)
            {
                sb.AppendLine(resources.AutoMapperTypeHandler);
            }

            if (useLinqToDbTypeHandler)
            {
                sb.AppendLine(resources.LinqToDbTypeHandler);
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

            if (useSchemaFilter)
            {
                sb.AppendLine(resources.SwaggerSchemaFilter);
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


        private static string BackingType(StronglyTypedIdBackingType backingType)
        {
            var resources = backingType switch
            {
                StronglyTypedIdBackingType.Guid => "System.Guid",
                StronglyTypedIdBackingType.Int => "int",
                StronglyTypedIdBackingType.Long => "long",
                StronglyTypedIdBackingType.String => "string",
                StronglyTypedIdBackingType.NullableString => "string?",
                StronglyTypedIdBackingType.MassTransitNewId => "MassTransit.NewId",
                _ => throw new ArgumentException("Unknown backing type: " + backingType, nameof(backingType)),
            };
            return resources;
        }

        private static void ReplaceInterfaces(StringBuilder sb, bool useIEquatable, bool useIComparable, bool useIParsable, bool useIConvertible, bool useIStronglyTypedId, StronglyTypedIdBackingType backingType)
        {
            var interfaces = new List<string>();

            if (useIComparable)
            {
                interfaces.Add("System.IComparable<TESTID>");
                interfaces.Add("System.IComparable");
            }

            if (useIEquatable)
            {
                interfaces.Add("System.IEquatable<TESTID>");
            }

            if (useIParsable)
            {
                interfaces.Add("System.IParsable<TESTID>");
            }

            if (useIConvertible)
            {
                interfaces.Add("System.IConvertible");
            }

            if (useIStronglyTypedId)
            {
                interfaces.Add($"IStronglyTypedId<{BackingType(backingType)}>");
            }

            interfaces.Add($"StronglyTypedIds.IInternalStronglyTypedId<{BackingType(backingType)}>");

            if (interfaces.Count > 0)
            {
                sb.Replace("INTERFACES", string.Join(", ", interfaces));
            }
            else
            {
                sb.Replace(": INTERFACES", string.Empty);
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
