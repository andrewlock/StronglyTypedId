using StronglyTypedIds;

namespace StronglyTypedIds.IntegrationTests.Types
{
    [StronglyTypedId(backingType: StronglyTypedIdBackingType.Long)]
    partial struct LongId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.None, backingType: StronglyTypedIdBackingType.Long)]
    public partial struct NoConverterLongId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.TypeConverter, backingType: StronglyTypedIdBackingType.Long)]
    public partial struct NoJsonLongId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.NewtonsoftJson, backingType: StronglyTypedIdBackingType.Long)]
    public partial struct NewtonsoftJsonLongId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.SystemTextJson, backingType: StronglyTypedIdBackingType.Long)]
    public partial struct SystemTextJsonLongId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.NewtonsoftJson | StronglyTypedIdConverter.SystemTextJson, backingType: StronglyTypedIdBackingType.Long)]
    public partial struct BothJsonLongId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.EfCoreValueConverter, backingType: StronglyTypedIdBackingType.Long)]
    public partial struct EfCoreLongId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.DapperTypeHandler, backingType: StronglyTypedIdBackingType.Long)]
    public partial struct DapperLongId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.Long, implementations: StronglyTypedIdImplementations.IEquatable | StronglyTypedIdImplementations.IComparable)]
    public partial struct BothLongId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.Long, implementations: StronglyTypedIdImplementations.IEquatable)]
    public partial struct EquatableLongId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.Long, implementations: StronglyTypedIdImplementations.IComparable)]
    public partial struct ComparableLongId { }
}