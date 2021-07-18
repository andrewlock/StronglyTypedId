
        class TESTIDSystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<TESTID>
        {
            public override TESTID Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
            {
                return new TESTID(reader.GetString());
            }

            public override void Write(System.Text.Json.Utf8JsonWriter writer, TESTID value, System.Text.Json.JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.Value);
            }
        }