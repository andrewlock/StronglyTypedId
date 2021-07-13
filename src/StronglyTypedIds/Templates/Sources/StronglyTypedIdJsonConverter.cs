using System;

namespace StronglyTypedIds
{
    /// <summary>
    /// JSON library used to serialize/deserialize strongly-typed ID value
    /// </summary>
    [Flags]
    public enum StronglyTypedIdJsonConverter
    {
        // Used with HasFlag, so needs to be 1, 2, 4 etc
        NewtonsoftJson = 1,
        SystemTextJson = 2,
    }
}