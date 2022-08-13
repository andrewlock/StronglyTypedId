namespace StronglyTypedIds.IntegrationTests.Types
{
    [StronglyTypedId(backingType: StronglyTypedIdBackingType.Short)]
    partial struct ShortId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.None, backingType: StronglyTypedIdBackingType.Short)]
    public partial struct NoConverterShortId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.TypeConverter, backingType: StronglyTypedIdBackingType.Short)]
    public partial struct NoJsonShortId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.NewtonsoftJson, backingType: StronglyTypedIdBackingType.Short)]
    public partial struct NewtonsoftJsonShortId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.SystemTextJson, backingType: StronglyTypedIdBackingType.Short)]
    public partial struct SystemTextJsonShortId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.NewtonsoftJson | StronglyTypedIdConverter.SystemTextJson, backingType: StronglyTypedIdBackingType.Short)]
    public partial struct BothJsonShortId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.EfCoreValueConverter, backingType: StronglyTypedIdBackingType.Short)]
    public partial struct EfCoreShortId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.DapperTypeHandler, backingType: StronglyTypedIdBackingType.Short)]
    public partial struct DapperShortId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.Short, implementations: StronglyTypedIdImplementations.IEquatable | StronglyTypedIdImplementations.IComparable)]
    public partial struct BothShortId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.Short, implementations: StronglyTypedIdImplementations.IEquatable)]
    public partial struct EquatableShortId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.Short, implementations: StronglyTypedIdImplementations.IComparable)]
    public partial struct ComparableShortId { }
}
