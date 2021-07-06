using System;

/// <summary>
/// The <see cref="Type"/> to use to store the value of a strongly-typed ID
/// </summary>
public enum StronglyTypedIdBackingType
{
    Guid = 0,
    Int = 1,
    String = 2,
    Long = 3
}
