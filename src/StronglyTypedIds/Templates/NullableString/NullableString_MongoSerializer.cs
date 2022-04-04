﻿        class TESTIDMongoSerializer : MongoDB.Bson.Serialization.Serializers.SerializerBase<TESTID>
        {
            public override TESTID Deserialize(MongoDB.Bson.Serialization.BsonDeserializationContext context, MongoDB.Bson.Serialization.BsonDeserializationArgs args)
            {
                return new TESTID(context.Reader.ReadString());
            }
            
            public override void Serialize(MongoDB.Bson.Serialization.BsonSerializationContext context, MongoDB.Bson.Serialization.BsonSerializationArgs args, TESTID value)
            {
                if (value.Value is null)
                {
                    context.Writer.WriteNull();
                }
                else
                {
                    context.Writer.WriteString(value.Value);
                }
            }
        }