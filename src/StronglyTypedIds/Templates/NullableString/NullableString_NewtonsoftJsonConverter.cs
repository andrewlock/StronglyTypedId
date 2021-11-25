
        class TESTIDNewtonsoftJsonConverter : Newtonsoft.Json.JsonConverter
        {
            public override bool CanConvert(System.Type objectType)
            {
                return objectType == typeof(TESTID);
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object? value, Newtonsoft.Json.JsonSerializer serializer)
            {
                if (value is null)
                {
                    serializer.Serialize(writer, null);
                }
                else
                {
                    var id = (TESTID)value;
                    serializer.Serialize(writer, id.Value);
                }
            }

            public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object? existingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                return new TESTID(serializer.Deserialize<string>(reader));
            }
        }