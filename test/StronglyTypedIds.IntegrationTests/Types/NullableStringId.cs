using StronglyTypedIds;

namespace StronglyTypedIds.IntegrationTests.Types
{
    [StronglyTypedId(backingType: StronglyTypedIdBackingType.NullableString)]
    partial struct NullableStringId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.None, backingType: StronglyTypedIdBackingType.NullableString)]
    public partial struct NoConvertersNullableStringId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.TypeConverter, backingType: StronglyTypedIdBackingType.NullableString)]
    public partial struct NoJsonNullableStringId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.NewtonsoftJson, backingType: StronglyTypedIdBackingType.NullableString)]
    public partial struct NewtonsoftJsonNullableStringId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.SystemTextJson, backingType: StronglyTypedIdBackingType.NullableString)]
    public partial struct SystemTextJsonNullableStringId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.NewtonsoftJson | StronglyTypedIdConverter.SystemTextJson, backingType: StronglyTypedIdBackingType.NullableString)]
    public partial struct BothJsonNullableStringId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.EfCoreValueConverter, backingType: StronglyTypedIdBackingType.NullableString)]
    public partial struct EfCoreNullableStringId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.DapperTypeHandler, backingType: StronglyTypedIdBackingType.NullableString)]
    public partial struct DapperNullableStringId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.NullableString, implementations: StronglyTypedIdImplementations.IEquatable | StronglyTypedIdImplementations.IComparable)]
    public partial struct BothNullableStringId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.NullableString, implementations: StronglyTypedIdImplementations.IEquatable)]
    public partial struct EquatableNullableStringId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.NullableString, implementations: StronglyTypedIdImplementations.IComparable)]
    public partial struct ComparableNullableStringId { }
}