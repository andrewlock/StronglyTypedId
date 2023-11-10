using System;
using System.Text;

namespace StronglyTypedIds
{
    internal static class SourceGenerationHelper
    {
        public static string CreateId(
            string idNamespace,
            string idName,
            ParentClass? parentClass,
            string template,
            bool addDefaultAttributes,
            StringBuilder? sb)
        {
            if (string.IsNullOrEmpty(idName))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(idName));
            }

            var hasNamespace = !string.IsNullOrEmpty(idNamespace);

            var parentsCount = 0;

            sb ??= new StringBuilder();
            sb.Append(EmbeddedSources.AutoGeneratedHeader);

            if (hasNamespace)
            {
                sb
                    .Append("namespace ")
                    .Append(idNamespace)
                    .AppendLine(@"
{");
            }

            var hasGenericParent = false;
            while (parentClass is { } parent)
            {
                sb.Append("    ");

                if (!string.IsNullOrEmpty(parent.Modifiers))
                {
                    sb.Append(parent.Modifiers).Append(' ');
                }

                if (parent.Modifiers.IndexOf("partial", StringComparison.Ordinal) == -1)
                {
                    sb.Append("partial ");
                }

                sb
                    .Append(parent.Keyword)
                    .Append(' ')
                    .Append(parent.Name)
                    .Append(' ')
                    .Append(parent.Constraints)
                    .AppendLine(@"
    {");
                parentsCount++;
                hasGenericParent |= parent.IsGeneric;
                parentClass = parent.Child;
            }
            
            if (addDefaultAttributes && !hasGenericParent)
            {
                sb.AppendLine("    [global::System.ComponentModel.TypeConverter(typeof(PLACEHOLDERIDTypeConverter))]");
                sb.AppendLine("    [global::System.Text.Json.Serialization.JsonConverter(typeof(PLACEHOLDERIDSystemTextJsonConverter))]");
            }

            sb.AppendLine(template);

            sb.Replace("PLACEHOLDERID", idName);

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

        internal static string CreateSourceName(StringBuilder sb, string nameSpace, ParentClass? parent, string name, string template)
        {
            sb.Clear();
            sb.Append(nameSpace).Append('.');
            while (parent is { } p)
            {
                var s = p.Name
                    .Replace(" ", "")
                    .Replace(",", "")
                    .Replace("<", "__")
                    .Replace(">", "");
                sb.Append(s).Append('.');
                parent = p.Child;
            }
            
            sb.Append(name);
            if (!string.IsNullOrEmpty(template))
            {
                sb.Append(template).Append('.');
            }

            return sb.Append(".g.cs").ToString();
        }
    }
}
