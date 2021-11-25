using System;
using System.Collections.Generic;
using System.Linq;
using StronglyTypedIds.Sources;

namespace StronglyTypedIds.Tests
{
    public class EnumHelper
    {
        public static IEnumerable<StronglyTypedIdBackingType> AllBackingTypes(bool includeDefault = true)
        {
            foreach (StronglyTypedIdBackingType value in Enum.GetValues(typeof(StronglyTypedIdBackingType)))
            {
                if (value != StronglyTypedIdBackingType.Default || includeDefault)
                {
                    yield return value;
                }
            }
        }

        public static IEnumerable<StronglyTypedIdConverter> AllConverters(bool includeDefault = true, bool includeNone = true)
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

        public static IEnumerable<StronglyTypedIdImplementations> AllImplementations(bool includeDefault = true, bool includeNone = true)
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