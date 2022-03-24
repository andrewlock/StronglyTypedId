        class TESTIDMongoObjectIdSerializer : MongoDB.Bson.Serialization.Serializers.SerializerBase<TESTID>
        {
            public override TESTID Deserialize(MongoDB.Bson.Serialization.BsonDeserializationContext context, MongoDB.Bson.Serialization.BsonDeserializationArgs args)
            {
                var objectId = context.Reader.ReadObjectId();
                return new TESTID(objectId.ToString());
            }
            
            public override void Serialize(MongoDB.Bson.Serialization.BsonSerializationContext context, MongoDB.Bson.Serialization.BsonSerializationArgs args, TESTID value)
            {
                context.Writer.WriteObjectId(string.IsNullOrWhiteSpace(value.Value)
                    ? MongoDB.Bson.ObjectId.GenerateNewId()
                    : new MongoDB.Bson.ObjectId(value.Value));
            }
        }