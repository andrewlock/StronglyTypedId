using System;

namespace StronglyTypedIds
{
    /// <summary>
    /// The <see cref="Type"/> to use to store the value of a strongly-typed ID
    /// </summary>
    public enum StronglyTypedIdBackingType
    {
        /// <summary>
        /// Use the default backing type (either the globally configured default, or Guid)
        /// </summary>
        Default = 0,
        
        Guid = 1,
        Int = 2,
        String = 3,
        Long = 4,
    }
}
