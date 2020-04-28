[System.ComponentModel.TypeConverter(typeof(LongIdTypeConverter))]
[Newtonsoft.Json.JsonConverter(typeof(LongIdNewtonsoftJsonConverter))]
[System.Text.Json.Serialization.JsonConverter(typeof(LongIdSystemTextJsonConverter))]
readonly partial struct LongId : System.IComparable<LongId>, System.IEquatable<LongId>
{
    public int Value { get; }

    public LongId(int value)
    {
        Value = value;
    }

    public static readonly LongId Empty = new LongId(0);

    public bool Equals(LongId other) => this.Value.Equals(other.Value);
    public int CompareTo(LongId other) => Value.CompareTo(other.Value);

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is LongId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
    public static bool operator ==(LongId a, LongId b) => a.CompareTo(b) == 0;
    public static bool operator !=(LongId a, LongId b) => !(a == b);

    class LongIdTypeConverter : System.ComponentModel.TypeConverter
    {
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        {
            return sourceType == typeof(int) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is int intValue)
            {
                return new LongId(intValue);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

    class LongIdNewtonsoftJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(System.Type objectType)
        {
            return objectType == typeof(LongId);
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var id = (LongId)value;
            serializer.Serialize(writer, id.Value);
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            return new LongId(serializer.Deserialize<int>(reader));
        }
    }
	
    class LongIdSystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<LongId>
    {
        public override LongId Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            return new LongId(reader.GetInt32());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, LongId value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value.Value);
        }
    }
}