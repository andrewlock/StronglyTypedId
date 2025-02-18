#nullable enable
using System;

namespace StronglyTypedIds.IntegrationTests.Types;


[StronglyTypedId]
public partial struct DefaultId1 { }

[StronglyTypedId]
public partial struct DefaultId2 { }

[StronglyTypedId(Template.Guid)]
public partial struct GuidId1 { }

[StronglyTypedId("guid-full")]
public partial struct ConvertersGuidId { }

[StronglyTypedId(Template.Guid, "guid-efcore", "guid-dapper", "guid-newtonsoftjson")]
public partial struct ConvertersGuidId2 { }

[StronglyTypedId(Template.Guid)]
public partial struct GuidId2 { }

[StronglyTypedId(Template.Int)]
public partial struct IntId { }

[StronglyTypedId("int-full")]
public partial struct ConvertersIntId { }

[StronglyTypedId(Template.Int, "int-efcore", "int-dapper", "int-newtonsoftjson")]
public partial struct ConvertersIntId2 { }

[StronglyTypedId(Template.Long)]
public partial struct LongId { }

[StronglyTypedId("long-full")]
public partial struct ConvertersLongId { }

[StronglyTypedId(Template.Long, "long-efcore", "long-dapper", "long-newtonsoftjson")]
public partial struct ConvertersLongId2 { }

[StronglyTypedId("newid-full")]
public partial struct NewIdId1 { }

[StronglyTypedId("newid-full")]
public partial struct NewIdId2 { }

[StronglyTypedId(Template.String)]
public partial struct StringId { }

[StronglyTypedId("string-full")]
public partial struct ConvertersStringId { }

[StronglyTypedId(Template.String, "string-efcore", "string-dapper", "string-newtonsoftjson")]
public partial struct ConvertersStringId2 { }

[StronglyTypedId("nullablestring-full")]
public partial struct NullableStringId { }

[StronglyTypedId("simple")]
public partial struct SimpleCustomId { }

public partial class SomeType<T> where T : new()
{
    public partial record struct NestedType<TKey, TValue>
    {
        public partial struct MoreNesting
        {
            [StronglyTypedId]
            public readonly partial struct VeryNestedId
            {
            }
        }
    }
}

