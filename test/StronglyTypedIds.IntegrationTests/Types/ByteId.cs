namespace StronglyTypedIds.IntegrationTests.Types
{
    [StronglyTypedId(backingType: StronglyTypedIdBackingType.Byte)]
    partial struct ByteId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.None, backingType: StronglyTypedIdBackingType.Byte)]
    public partial struct NoConverterByteId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.TypeConverter, backingType: StronglyTypedIdBackingType.Byte)]
    public partial struct NoJsonByteId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.NewtonsoftJson, backingType: StronglyTypedIdBackingType.Byte)]
    public partial struct NewtonsoftJsonByteId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.SystemTextJson, backingType: StronglyTypedIdBackingType.Byte)]
    public partial struct SystemTextJsonByteId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.NewtonsoftJson | StronglyTypedIdConverter.SystemTextJson, backingType: StronglyTypedIdBackingType.Byte)]
    public partial struct BothJsonByteId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.EfCoreValueConverter, backingType: StronglyTypedIdBackingType.Byte)]
    public partial struct EfCoreByteId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.DapperTypeHandler, backingType: StronglyTypedIdBackingType.Byte)]
    public partial struct DapperByteId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.Byte, implementations: StronglyTypedIdImplementations.IEquatable | StronglyTypedIdImplementations.IComparable)]
    public partial struct BothByteId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.Byte, implementations: StronglyTypedIdImplementations.IEquatable)]
    public partial struct EquatableByteId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.Byte, implementations: StronglyTypedIdImplementations.IComparable)]
    public partial struct ComparableByteId { }
}
