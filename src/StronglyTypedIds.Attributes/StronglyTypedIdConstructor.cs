namespace StronglyTypedIds
{
    /// <summary>
    /// Controls how the constructor is generated
    /// </summary>
    public enum StronglyTypedIdConstructor
    {
        /// <summary>
        /// Use the default constructor implementation (either the globally configured default, or a public constructor that sets the backing value)
        /// </summary>
        Default = 0,

        None = 1,
        Public = 2,
        Private = 3,
        Internal = 4,
    }
}