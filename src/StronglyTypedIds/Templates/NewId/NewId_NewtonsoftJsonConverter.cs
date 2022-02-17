﻿
        class TESTIDNewtonsoftJsonConverter : Newtonsoft.Json.JsonConverter
        {
            public override bool CanConvert(System.Type objectType)
            {
                return objectType == typeof(TESTID);
            }

            public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
            {
                var id = (TESTID)value;
                serializer.Serialize(writer, id.Value.ToGuid());
            }

            public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                var guid = serializer.Deserialize<System.Guid?>(reader);
                return guid.HasValue ? new TESTID(MassTransit.NewId.FromGuid(guid.Value)) : null;
            }
        }