using StronglyTypedIds.Sources;

namespace StronglyTypedIds
{
    internal static class StronglyTypedIdConverterExtensions
    {
        public static bool IsSet(this StronglyTypedIdConverter value, StronglyTypedIdConverter flag)
            => (value & flag) == flag;

        public static bool IsValidFlags(this StronglyTypedIdConverter value)
        {
            return (int)value >= 0
                   && (int)value < ((int)StronglyTypedIdConverter.SystemTextJson * 2); // Update this when adding a new converter
        }
    }
}