﻿using StronglyTypedIds;

namespace StronglyTypedIds.IntegrationTests.Types
{
    [StronglyTypedId(backingType: StronglyTypedIdBackingType.Int)]
    partial struct IntId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.None, backingType: StronglyTypedIdBackingType.Int)]
    public partial struct NoConverterIntId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.TypeConverter, backingType: StronglyTypedIdBackingType.Int)]
    public partial struct NoJsonIntId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.NewtonsoftJson, backingType: StronglyTypedIdBackingType.Int)]
    public partial struct NewtonsoftJsonIntId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.SystemTextJson, backingType: StronglyTypedIdBackingType.Int)]
    public partial struct SystemTextJsonIntId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.NewtonsoftJson | StronglyTypedIdConverter.SystemTextJson, backingType: StronglyTypedIdBackingType.Int)]
    public partial struct BothJsonIntId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.EfCoreValueConverter, backingType: StronglyTypedIdBackingType.Int)]
    public partial struct EfCoreIntId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.DapperTypeHandler, backingType: StronglyTypedIdBackingType.Int)]
    public partial struct DapperIntId { }

#if NET5_0_OR_GREATER
    [StronglyTypedId(converters: StronglyTypedIdConverter.SwaggerSchemaFilter, backingType: StronglyTypedIdBackingType.Int)]
    public partial struct SwaggerIntId { }
#endif

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.Int, implementations: StronglyTypedIdImplementations.IEquatable | StronglyTypedIdImplementations.IComparable)]
    public partial struct BothIntId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.Int, implementations: StronglyTypedIdImplementations.IEquatable)]
    public partial struct EquatableIntId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.Int, implementations: StronglyTypedIdImplementations.IComparable)]
    public partial struct ComparableIntId { }
}