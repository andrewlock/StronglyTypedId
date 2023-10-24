using System.Text.Json;
using StronglyTypedIds;
#nullable enable
namespace StronglyTypedIds.IntegrationTests.Types
{
    [StronglyTypedId]
    partial struct DefaultId1 { }

    [StronglyTypedId]
    public partial struct DefaultId2 { }

    [StronglyTypedId(Template.Guid)]
    partial struct GuidId1 { }

    [StronglyTypedId(Template.Guid)]
    public partial struct GuidId2 { }

    // [StronglyTypedId(Template.Int)]
    // partial struct IntId { }
    //
    // [StronglyTypedId(Template.Long)]
    // partial struct LongId { }
    //
    // [StronglyTypedId("newid")]
    // partial struct NewIdId1 { }
    //
    // [StronglyTypedId("newid")]
    // public partial struct NewIdId2 { }
    //
    // [StronglyTypedId(Template.NullableString)]
    // partial struct NullableStringId { }
    //
    // [StronglyTypedId(Template.String)]
    // partial struct StringId { }

    // public partial class SomeType<T> where T : new()
    // {
    //     public partial record NestedType<TKey, TValue>
    //     {
    //         public partial struct MoreNesting
    //         {
    //             [StronglyTypedId]
    //             public partial struct VeryNestedIds
    //             {
    //             }
    //         }
    //     }
    // }
}