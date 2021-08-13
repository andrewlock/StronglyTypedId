using StronglyTypedIds;

namespace StronglyTypedIds.IntegrationTests.Types
{
    [StronglyTypedId]
    partial struct GuidId1 { }

    [StronglyTypedId]
    public partial struct GuidId2 { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.None)]
    public partial struct NoConverterGuidId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.TypeConverter)]
    public partial struct NoJsonGuidId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.NewtonsoftJson)]
    public partial struct NewtonsoftJsonGuidId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.TypeConverter | StronglyTypedIdConverter.SystemTextJson)]
    public partial struct SystemTextJsonGuidId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.NewtonsoftJson | StronglyTypedIdConverter.SystemTextJson)]
    public partial struct BothJsonGuidId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.EfCoreValueConverter)]
    public partial struct EfCoreGuidId { }

    [StronglyTypedId(converters: StronglyTypedIdConverter.DapperTypeHandler)]
    public partial struct DapperGuidId { }
}