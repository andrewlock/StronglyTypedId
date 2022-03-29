﻿using StronglyTypedIds;

namespace StronglyTypedIds.IntegrationTests.Types
{
    [StronglyTypedId(backingType: StronglyTypedIdBackingType.String)]
    partial struct StringId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.None, backingType: StronglyTypedIdBackingType.String)]
    public partial struct NoConvertersStringId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.TypeConverter, backingType: StronglyTypedIdBackingType.String)]
    public partial struct NoJsonStringId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.NewtonsoftJson, backingType: StronglyTypedIdBackingType.String)]
    public partial struct NewtonsoftJsonStringId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.SystemTextJson, backingType: StronglyTypedIdBackingType.String)]
    public partial struct SystemTextJsonStringId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.NewtonsoftJson | StronglyTypedIdConverter.SystemTextJson, backingType: StronglyTypedIdBackingType.String)]
    public partial struct BothJsonStringId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.EfCoreValueConverter, backingType: StronglyTypedIdBackingType.String)]
    public partial struct EfCoreStringId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.DapperTypeHandler, backingType: StronglyTypedIdBackingType.String)]
    public partial struct DapperStringId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.String, implementations: StronglyTypedIdImplementations.IEquatable | StronglyTypedIdImplementations.IComparable)]
    public partial struct BothStringId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.String, implementations: StronglyTypedIdImplementations.IEquatable)]
    public partial struct EquatableStringId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.String, implementations: StronglyTypedIdImplementations.IComparable)]
    public partial struct ComparableStringId { }
    
    [StronglyTypedId(backingType: StronglyTypedIdBackingType.String, converters: StronglyTypedIdConverter.MongoObjectIdSerializer)]
    public partial struct MongoStringId { }
}