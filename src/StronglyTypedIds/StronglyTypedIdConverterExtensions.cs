using System;
using System.Linq;

namespace StronglyTypedIds
{
    internal static class StronglyTypedIdConverterExtensions
    {
        private static readonly int _maxConverterId = Enum.GetValues(typeof(StronglyTypedIdConverter)).Cast<int>().Max() * 2;
        private static readonly int _maxImplementationsId = Enum.GetValues(typeof(StronglyTypedIdImplementations)).Cast<int>().Max() * 2;

        public static bool IsSet(this StronglyTypedIdConverter value, StronglyTypedIdConverter flag)
            => (value & flag) == flag;

        public static bool IsValidFlags(this StronglyTypedIdConverter value)
        {
            return (int)value >= 0 && (int)value < _maxConverterId;
        }

        public static bool IsSet(this StronglyTypedIdImplementations value, StronglyTypedIdImplementations flag)
            => (value & flag) == flag;

        public static bool IsValidFlags(this StronglyTypedIdImplementations value)
        {
            return (int)value >= 0 && (int)value < _maxImplementationsId;
        }
    }
}