using StronglyTypedIds;

namespace StronglyTypedIds.IntegrationTests.Types
{
    [StronglyTypedId(backingType: StronglyTypedIdBackingType.String)]
    partial struct StringId { }

    [StronglyTypedId(generateJsonConverter: false, backingType: StronglyTypedIdBackingType.String)]
    public partial struct NoJsonStringId { }

    [StronglyTypedId(jsonConverter: StronglyTypedIdJsonConverter.NewtonsoftJson, backingType: StronglyTypedIdBackingType.String)]
    public partial struct NewtonsoftJsonStringId { }

    [StronglyTypedId(jsonConverter: StronglyTypedIdJsonConverter.SystemTextJson, backingType: StronglyTypedIdBackingType.String)]
    public partial struct SystemTextJsonStringId { }

    [StronglyTypedId(jsonConverter: StronglyTypedIdJsonConverter.NewtonsoftJson | StronglyTypedIdJsonConverter.SystemTextJson, backingType: StronglyTypedIdBackingType.String)]
    public partial struct BothJsonStringId { }
}