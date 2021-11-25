using StronglyTypedIds;
[assembly: StronglyTypedIdDefaults(converters: StronglyTypedIdConverter.None, implementations: StronglyTypedIdImplementations.None)]

namespace StronglyTypedIds.IntegrationTests.Types
{
    [StronglyTypedId]
    partial struct DefaultId1 { }

    [StronglyTypedId]
    public partial struct DefaultId2 { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.None)]
    public partial struct NoConverterDefaultId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.TypeConverter)]
    public partial struct NoJsonDefaultId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.NewtonsoftJson)]
    public partial struct NewtonsoftJsonDefaultId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.TypeConverter | StronglyTypedIdConverter.SystemTextJson)]
    public partial struct SystemTextJsonDefaultId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.NewtonsoftJson | StronglyTypedIdConverter.SystemTextJson)]
    public partial struct BothJsonDefaultId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.EfCoreValueConverter)]
    public partial struct EfCoreDefaultId { }
}