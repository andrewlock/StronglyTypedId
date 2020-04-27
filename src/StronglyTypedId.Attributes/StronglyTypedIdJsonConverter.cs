using System;

/// <summary>
/// JSON library used to serialize/deserialize strongly-typed ID value
/// </summary>
[Flags]
public enum StronglyTypedIdJsonConverter
{
    NewtonsoftJson = 0,
    SystemTextJson = 1
}