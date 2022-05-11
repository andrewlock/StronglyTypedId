
        class TESTIDSystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<TESTID>
        {
            public override TESTID Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
            {
                var result = reader.GetString();
                return string.IsNullOrEmpty(result) ? TESTID.Empty : new TESTID(new MongoDB.Bson.ObjectId(result));
            }

            public override void Write(System.Text.Json.Utf8JsonWriter writer, TESTID value, System.Text.Json.JsonSerializerOptions options)
            {
                if (value.Value == MongoDB.Bson.ObjectId.Empty)
                {
                    writer.WriteNullValue();
                }
                else
                {
                    writer.WriteStringValue(value.Value.ToString());
                }
            }
        }