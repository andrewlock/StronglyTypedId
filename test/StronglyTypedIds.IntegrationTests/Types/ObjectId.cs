namespace StronglyTypedIds.IntegrationTests.Types;

[StronglyTypedId(backingType: StronglyTypedIdBackingType.ObjectId)]
partial struct ObjectIdId1 { }

[StronglyTypedId(backingType: StronglyTypedIdBackingType.ObjectId)]
partial struct ObjectIdId2 { }
    
[StronglyTypedId(backingType: StronglyTypedIdBackingType.ObjectId, converters: StronglyTypedIdConverter.None)]
public partial struct NoConverterObjectIdId { }
    
[StronglyTypedId(backingType: StronglyTypedIdBackingType.ObjectId, converters: StronglyTypedIdConverter.TypeConverter)]
public partial struct NoJsonObjectIdId { }
    
[StronglyTypedId(backingType: StronglyTypedIdBackingType.ObjectId, converters: StronglyTypedIdConverter.NewtonsoftJson)]
public partial struct NewtonsoftJsonObjectIdId { }
    
[StronglyTypedId(backingType: StronglyTypedIdBackingType.ObjectId, converters: StronglyTypedIdConverter.SystemTextJson)]
public partial struct SystemTextJsonObjectIdId { }
    
[StronglyTypedId(backingType: StronglyTypedIdBackingType.ObjectId, converters: StronglyTypedIdConverter.NewtonsoftJson | StronglyTypedIdConverter.SystemTextJson)]
public partial struct BothJsonObjectIdId { }
    
[StronglyTypedId(backingType: StronglyTypedIdBackingType.ObjectId, converters: StronglyTypedIdConverter.EfCoreValueConverter)]
public partial struct EfCoreObjectIdId { }

[StronglyTypedId(backingType: StronglyTypedIdBackingType.ObjectId, converters: StronglyTypedIdConverter.DapperTypeHandler)]
public partial struct DapperObjectIdId { }
    
[StronglyTypedId(backingType: StronglyTypedIdBackingType.ObjectId, implementations: StronglyTypedIdImplementations.IEquatable | StronglyTypedIdImplementations.IComparable)]
public partial struct BothObjectIdId { }

[StronglyTypedId(backingType: StronglyTypedIdBackingType.ObjectId, implementations: StronglyTypedIdImplementations.IEquatable)]
public partial struct EquatableObjectIdId { }

[StronglyTypedId(backingType: StronglyTypedIdBackingType.ObjectId, implementations: StronglyTypedIdImplementations.IComparable)]
public partial struct ComparableObjectIdId { }
    
[StronglyTypedId(backingType: StronglyTypedIdBackingType.ObjectId, converters: StronglyTypedIdConverter.MongoSerializer)]
public partial struct MongoObjectIdId { }