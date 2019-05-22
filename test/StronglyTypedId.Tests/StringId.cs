namespace StronglyTypedId
{
    [StronglyTypedId(backingType: StronglyTypedIdBackingType.String)]
    partial struct StringId { }

    [StronglyTypedId(generateJsonConverter: false, backingType: StronglyTypedIdBackingType.String)]
    partial struct NoJsonStringId { }
}