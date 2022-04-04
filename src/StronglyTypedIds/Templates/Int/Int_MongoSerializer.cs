        class TESTIDMongoSerializer : MongoDB.Bson.Serialization.Serializers.SerializerBase<TESTID>
        {
            public override TESTID Deserialize(MongoDB.Bson.Serialization.BsonDeserializationContext context, MongoDB.Bson.Serialization.BsonDeserializationArgs args)
            {
                return new TESTID(context.Reader.ReadInt32());
            }
            
            public override void Serialize(MongoDB.Bson.Serialization.BsonSerializationContext context, MongoDB.Bson.Serialization.BsonSerializationArgs args, TESTID value)
            {
                context.Writer.WriteInt32(value.Value);
            }
        }