using System.Collections.Generic;
using System.Text;

namespace StronglyTypedId
{
    internal static class Sources
    {
        public const string StronglyTypedIdAttribute =
            @"using System;
using System.Diagnostics;

namespace StronglyTypedId
{
    /// <summary>
    /// Make the struct a strongly typed ID 
    /// </summary>
    /// <param name=""generateJsonConverter"">If true generates a JsonConverter for the strongly typed ID (requires a reference to Newtonsoft.Json in the project)</param>
    /// <param name=""backingType"">The <see cref=""Type""/> to use to store the strongly-typed ID value. Defaults to <see cref=""StronglyTypedIdBackingType.Guid""/></param>
    /// <param name=""jsonConverter"">JSON library used to serialize/deserialize strongly-typed ID value. Defaults to <see cref=""StronglyTypedIdJsonConverter.NewtonsoftJson""/></param>
    [AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    [Conditional(""CodeGeneration"")]
    public StronglyTypedIdAttribute(
        bool generateJsonConverter = true, 
        StronglyTypedIdBackingType backingType = StronglyTypedIdBackingType.Guid,
        StronglyTypedIdJsonConverter jsonConverter = StronglyTypedIdJsonConverter.NewtonsoftJson)
    {
        GenerateJsonConverter = generateJsonConverter;
        BackingType = backingType;
        JsonConverter = jsonConverter;
    }

    /// <summary>
    /// If true generates a JsonConverter for the strongly-typed ID 
    /// (requires a reference to Newtonsoft.Json and/or System.Text.Json in the project)
    /// </summary>
    public bool GenerateJsonConverter { get; }

    /// <summary>
    /// The <see cref=""Type""/> to use to store the strongly-typed ID value
    /// </summary>
    public StronglyTypedIdBackingType BackingType { get; }

    /// <summary>
    /// JSON library used to serialize/deserialize strongly-typed ID value
    /// </summary>
    public StronglyTypedIdJsonConverter JsonConverter { get; }
}";

        public static string CreateTagsList(
            string classNamespace,
            string className,
            IList<(string Property, string Tag)> propertyNames)
        {
            var sb = new StringBuilder();
            sb.Append("namespace ").Append(classNamespace).Append(@"
{
    partial class " + className + @"
    {
        public string GetTag(string key)
        {
            ");

            for (int i = 0; i < propertyNames.Count; i++)
            {
                var (property, tag) = propertyNames[i];
                sb.Append(i == 0 ? "if" : "else if");
                sb.Append($@" (key == ""{tag}"")");
                sb.Append($@"
            {{
                return {property};
            }}
            ");
            }

            sb.Append(@"
            return GetTagFromDictionary(key);
        }
    }
}");
            return sb.ToString();
        }
    }
}
