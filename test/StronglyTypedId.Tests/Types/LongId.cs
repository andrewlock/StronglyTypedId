namespace StronglyTypedId.Tests.Types
{
    [StronglyTypedId(backingType: StronglyTypedIdBackingType.Long)]
    partial struct LongId { }

    [StronglyTypedId(generateJsonConverter: false, backingType: StronglyTypedIdBackingType.Long)]
    public partial struct NoJsonLongId { }

    [StronglyTypedId(jsonConverter: StronglyTypedIdJsonConverter.NewtonsoftJson, backingType: StronglyTypedIdBackingType.Long)]
    public partial struct NewtonsoftJsonLongId { }

    [StronglyTypedId(jsonConverter: StronglyTypedIdJsonConverter.SystemTextJson, backingType: StronglyTypedIdBackingType.Long)]
    public partial struct SystemTextJsonLongId { }

    [StronglyTypedId(jsonConverter: StronglyTypedIdJsonConverter.NewtonsoftJson | StronglyTypedIdJsonConverter.SystemTextJson, backingType: StronglyTypedIdBackingType.Long)]
    public partial struct BothJsonLongId { }
}