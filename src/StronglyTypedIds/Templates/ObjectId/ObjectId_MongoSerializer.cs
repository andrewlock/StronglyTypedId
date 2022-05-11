        class TESTIDMongoSerializer : MongoDB.Bson.Serialization.Serializers.SerializerBase<TESTID>
        {
            public override TESTID Deserialize(MongoDB.Bson.Serialization.BsonDeserializationContext context, MongoDB.Bson.Serialization.BsonDeserializationArgs args)
            {
                if (context.Reader.GetCurrentBsonType() == MongoDB.Bson.BsonType.ObjectId)
                {
                    return new TESTID(context.Reader.ReadObjectId());
                }
                else
                {
                    context.Reader.SkipValue();
                    return TESTID.Empty;
                }
            }
            
            public override void Serialize(MongoDB.Bson.Serialization.BsonSerializationContext context, MongoDB.Bson.Serialization.BsonSerializationArgs args, TESTID value)
            {
                if (value.Value == MongoDB.Bson.ObjectId.Empty)
                {
                    context.Writer.WriteNull();
                }
                else
                {
                    context.Writer.WriteObjectId(value.Value);
                }
            }
        }