using System;

namespace StronglyTypedIds
{
    /// <summary>
    /// The built-in template to use to generate the strongly-typed ID
    /// </summary>
    public enum Template
    {
        Guid,
        Int,
        String,
        Long,
        NullableString,
    }
}