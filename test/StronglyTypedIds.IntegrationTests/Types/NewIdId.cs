using StronglyTypedIds;

namespace StronglyTypedIds.IntegrationTests.Types
{
    [StronglyTypedId(backingType: StronglyTypedIdBackingType.MassTransitNewId)]
    partial struct NewIdId1 { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.MassTransitNewId)]
    public partial struct NewIdId2 { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.MassTransitNewId, converters: StronglyTypedIdConverter.None)]
    public partial struct NoConverterNewIdId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.MassTransitNewId, converters: StronglyTypedIdConverter.TypeConverter)]
    public partial struct NoJsonNewIdId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.MassTransitNewId, converters: StronglyTypedIdConverter.NewtonsoftJson)]
    public partial struct NewtonsoftJsonNewIdId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.MassTransitNewId, converters: StronglyTypedIdConverter.TypeConverter | StronglyTypedIdConverter.SystemTextJson)]
    public partial struct SystemTextJsonNewIdId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.MassTransitNewId, converters: StronglyTypedIdConverter.NewtonsoftJson | StronglyTypedIdConverter.SystemTextJson)]
    public partial struct BothJsonNewIdId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.MassTransitNewId, converters: StronglyTypedIdConverter.EfCoreValueConverter)]
    public partial struct EfCoreNewIdId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.MassTransitNewId, converters: StronglyTypedIdConverter.DapperTypeHandler)]
    public partial struct DapperNewIdId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.MassTransitNewId, implementations: StronglyTypedIdImplementations.IEquatable | StronglyTypedIdImplementations.IComparable)]
    public partial struct BothNewIdId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.MassTransitNewId, implementations: StronglyTypedIdImplementations.IEquatable)]
    public partial struct EquatableNewIdId { }

    [StronglyTypedId(backingType: StronglyTypedIdBackingType.MassTransitNewId, implementations: StronglyTypedIdImplementations.IComparable)]
    public partial struct ComparableNewIdId { }
}