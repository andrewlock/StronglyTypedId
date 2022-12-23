
        class TESTIDNewtonsoftJsonConverter : Newtonsoft.Json.JsonConverter
        {
            public override bool CanConvert(System.Type objectType)
            {
                return objectType == typeof(TESTID);
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
            {
                var id = (TESTID)value;
                serializer.Serialize(writer, id.Value);
            }

            public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                if (objectType == typeof(TESTID?))
                {
                    var value = serializer.Deserialize<string?>(reader);

                    return value is null ? null : new TESTID(value);
                }

                return new TESTID(serializer.Deserialize<string>(reader));
            }
        }