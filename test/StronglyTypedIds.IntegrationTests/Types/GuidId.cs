using StronglyTypedIds;
using StronglyTypedIds.Sources;

namespace StronglyTypedIds.IntegrationTests.Types
{
    [StronglyTypedId]
    partial struct GuidId1 { }

    [StronglyTypedId]
    public partial struct GuidId2 { }

    [StronglyTypedId(generateJsonConverter: false)]
    public partial struct NoJsonGuidId { }

    [StronglyTypedId(jsonConverter: StronglyTypedIdJsonConverter.NewtonsoftJson)]
    public partial struct NewtonsoftJsonGuidId { }

    [StronglyTypedId(jsonConverter: StronglyTypedIdJsonConverter.SystemTextJson)]
    public partial struct SystemTextJsonGuidId { }

    [StronglyTypedId(jsonConverter: StronglyTypedIdJsonConverter.NewtonsoftJson | StronglyTypedIdJsonConverter.SystemTextJson)]
    public partial struct BothJsonGuidId { }
}