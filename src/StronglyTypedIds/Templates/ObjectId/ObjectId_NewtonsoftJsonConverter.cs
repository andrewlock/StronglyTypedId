
        class TESTIDNewtonsoftJsonConverter : Newtonsoft.Json.JsonConverter
        {
            public override bool CanConvert(System.Type objectType)
            {
                return objectType == typeof(TESTID);
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
            {
                var id = (TESTID)value;
                serializer.Serialize(writer, id.Value.ToString());
            }

            public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                var result = serializer.Deserialize<string>(reader);
                return string.IsNullOrEmpty(result) ? null : new TESTID(new MongoDB.Bson.ObjectId(result));
            }
        }