        class TESTIDMongoSerializer : MongoDB.Bson.Serialization.Serializers.SerializerBase<TESTID>
        {
            public override TESTID Deserialize(MongoDB.Bson.Serialization.BsonDeserializationContext context, MongoDB.Bson.Serialization.BsonDeserializationArgs args)
            {
                return new TESTID(MassTransit.NewId.FromGuid(new System.Guid(context.Reader.ReadString())));
            }
            
            public override void Serialize(MongoDB.Bson.Serialization.BsonSerializationContext context, MongoDB.Bson.Serialization.BsonSerializationArgs args, TESTID value)
            {
                context.Writer.WriteString(value.Value.ToGuid().ToString());
            }
        }