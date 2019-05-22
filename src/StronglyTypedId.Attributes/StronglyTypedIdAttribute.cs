using System;
using System.Diagnostics;
using CodeGeneration.Roslyn;

[AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
[CodeGenerationAttribute("StronglyTypedId.Generator.StronglyTypedIdGenerator, StronglyTypedId.Generator")]
[Conditional("CodeGeneration")]
public class StronglyTypedIdAttribute : Attribute
{
    /// <summary>
    /// Make the struct a strongly typed ID 
    /// </summary>
    /// <param name="generateJsonConverter">If true generates a JsonConverter for the strongly typed ID (requires a reference to Newtonsoft.Json in the project)</param>
    /// <param name="backingType">The <see cref="Type"/> to use to store the strongly-typed ID value. Defaults to <see cref="StronglyTypedIdBackingType.Guid"/></param>
    public StronglyTypedIdAttribute(
        bool generateJsonConverter = true, 
        StronglyTypedIdBackingType backingType = StronglyTypedIdBackingType.Guid)
    {
        GenerateJsonConverter = generateJsonConverter;
        BackingType = backingType;
    }

    /// <summary>
    /// If true generates a JsonConverter for the strongly typed ID 
    /// (requires a reference to Newtonsoft.Json in the project)
    /// </summary>
    public bool GenerateJsonConverter { get; }

    /// <summary>
    /// The <see cref="Type"/> to use to store the strongly-typed ID value
    /// </summary>
    public StronglyTypedIdBackingType BackingType { get; }

}
