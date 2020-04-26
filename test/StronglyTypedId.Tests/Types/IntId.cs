namespace StronglyTypedId.Tests.Types
{
    [StronglyTypedId(backingType: StronglyTypedIdBackingType.Int)]
    partial struct IntId { }

    [StronglyTypedId(generateJsonConverter: false, backingType: StronglyTypedIdBackingType.Int)]
    public partial struct NoJsonIntId { }

    [StronglyTypedId(jsonConverter: StronglyTypedIdJsonConverter.NewtonsoftJson, backingType: StronglyTypedIdBackingType.Int)]
    public partial struct NewtonsoftJsonIntId { }

    [StronglyTypedId(jsonConverter: StronglyTypedIdJsonConverter.SystemTextJson, backingType: StronglyTypedIdBackingType.Int)]
    public partial struct SystemTextJsonIntId { }

    [StronglyTypedId(jsonConverter: StronglyTypedIdJsonConverter.NewtonsoftJson | StronglyTypedIdJsonConverter.SystemTextJson, backingType: StronglyTypedIdBackingType.Int)]
    public partial struct BothJsonIntId { }
}