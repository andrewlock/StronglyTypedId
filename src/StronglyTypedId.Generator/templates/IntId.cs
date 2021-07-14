[System.ComponentModel.TypeConverter(typeof(TESTIDTypeConverter))]
[Newtonsoft.Json.JsonConverter(typeof(TESTIDNewtonsoftJsonConverter))]
[System.Text.Json.Serialization.JsonConverter(typeof(TESTIDSystemTextJsonConverter))]
readonly partial struct TESTID : System.IComparable<TESTID>, System.IEquatable<TESTID>
{
    public int Value { get; }

    public TESTID(int value)
    {
        Value = value;
    }

    public static readonly TESTID Empty = new TESTID(0);

    public bool Equals(TESTID other) => this.Value.Equals(other.Value);
    public int CompareTo(TESTID other) => Value.CompareTo(other.Value);

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is TESTID other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
    public static bool operator ==(TESTID a, TESTID b) => a.CompareTo(b) == 0;
    public static bool operator !=(TESTID a, TESTID b) => !(a == b);

    class TESTIDTypeConverter : System.ComponentModel.TypeConverter
    {
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        {
            return sourceType == typeof(int) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is int intValue)
            {
                return new TESTID(intValue);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

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
            return new TESTID(serializer.Deserialize<int>(reader));
        }
    }

    class TESTIDSystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<TESTID>
    {
        public override TESTID Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            return new TESTID(reader.GetInt32());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, TESTID value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.Value);
        }
    }
}