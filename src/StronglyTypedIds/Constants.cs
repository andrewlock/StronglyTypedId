namespace StronglyTypedIds
{
    internal static class Constants
    {
        public const string Namespace = nameof(StronglyTypedIds);

        public const string StronglyTypedIdAttribute = nameof(StronglyTypedIds.Sources.StronglyTypedIdAttribute);
        public const string FullyQualifiedStronglyTypedIdAttribute = Namespace + "." + StronglyTypedIdAttribute;

        public const string StronglyTypedIdDefaultsAttribute = nameof(StronglyTypedIds.Sources.StronglyTypedIdDefaultsAttribute);
        public const string FullyQualifiedStronglyTypedIdDefaultsAttribute = Namespace + "." + StronglyTypedIdDefaultsAttribute;

        public const string Usage = nameof(Usage);
    }
}