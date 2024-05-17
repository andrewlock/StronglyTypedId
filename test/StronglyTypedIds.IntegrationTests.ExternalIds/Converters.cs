using StronglyTypedIds.IntegrationTests.Types;

namespace StronglyTypedIds.IntegrationTests;

[StronglyTypedIdConverters<GuidId1>("guid-dapper", "guid-efcore")]
internal partial struct Guid1Converters { }

[StronglyTypedIdConverters<IntId>("int-dapper", "int-efcore")]
internal partial struct IntConverters { }

[StronglyTypedIdConverters<LongId>("long-dapper", "long-efcore")]
internal partial struct LongConverters { }

[StronglyTypedIdConverters<StringId>("string-dapper", "string-efcore")]
internal partial struct StringConverters { }