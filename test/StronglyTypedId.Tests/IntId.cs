namespace StronglyTypedId
{
    [StronglyTypedId(backingType: StronglyTypedIdBackingType.Int)]
    partial struct IntId { }

    [StronglyTypedId(generateJsonConverter: false, backingType: StronglyTypedIdBackingType.Int)]
    partial struct NoJsonIntId { }
}