using System;
using System.Collections.Generic;
using System.Linq;

namespace StronglyTypedIds.Tests
{
    public class EnumHelper
    {
        public static IEnumerable<StronglyTypedIdBackingType> AllBackingTypes(bool includeDefault = true) 
            => Enum.GetValues(typeof(StronglyTypedIdBackingType))
                .Cast<StronglyTypedIdBackingType>()
                .Where(value => value != StronglyTypedIdBackingType.Default || includeDefault);

        public static IEnumerable<StronglyTypedIdConverter> AllConverters(bool includeDefault = true) 
            => Enum.GetValues(typeof(StronglyTypedIdConverter))
                .Cast<StronglyTypedIdConverter>()
                .Where(value => value != StronglyTypedIdConverter.Default || includeDefault);

        public static IEnumerable<StronglyTypedIdImplementations> AllImplementations(bool includeDefault = true) 
            => Enum.GetValues(typeof(StronglyTypedIdImplementations))
                .Cast<StronglyTypedIdImplementations>()
                .Where(value => value != StronglyTypedIdImplementations.Default || includeDefault);

        public static IEnumerable<StronglyTypedIdConverter> AllConverterCombinations(bool includeDefault = true, bool includeNone = true)
        {
            // get highest value
            var highestValue = Enum.GetValues(typeof(StronglyTypedIdConverter))
                .Cast<int>()
                .Max();

            var upperBound = highestValue * 2;
            for (var i = 0; i < upperBound; i++)
            {
                var converter = (StronglyTypedIdConverter)i;
                if (converter.IsSet(StronglyTypedIdConverter.Default) && !includeDefault
                    || converter == StronglyTypedIdConverter.None && !includeNone)
                {
                    continue;
                }

                yield return converter;
            }
        }

        public static IEnumerable<StronglyTypedIdImplementations> AllImplementationCombinations(bool includeDefault = true, bool includeNone = true)
        {
            // get highest value
            var highestValue = Enum.GetValues(typeof(StronglyTypedIdImplementations))
                .Cast<int>()
                .Max();

            var upperBound = highestValue * 2;
            for (var i = 0; i < upperBound; i++)
            {
                var implementations = (StronglyTypedIdImplementations)i;
                if (implementations.IsSet(StronglyTypedIdImplementations.Default) && !includeDefault
                    || implementations == StronglyTypedIdImplementations.None && !includeNone)
                {
                    continue;
                }

                yield return implementations;
            }
        }
    }
}