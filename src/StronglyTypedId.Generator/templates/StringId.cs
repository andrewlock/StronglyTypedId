[System.ComponentModel.TypeConverter(typeof(StringIdTypeConverter))]
[Newtonsoft.Json.JsonConverter(typeof(StringIdNewtonsoftJsonConverter))]
[System.Text.Json.Serialization.JsonConverter(typeof(StringIdSystemTextJsonConverter))]
readonly partial struct StringId : System.IComparable<StringId>, System.IEquatable<StringId>
{
    public string Value { get; }

    public StringId(string value)
    {
        Value = value;
    }

    public static readonly StringId Empty = new StringId(string.Empty);

    public bool Equals(StringId other) => this.Value.Equals(other.Value);
    public int CompareTo(StringId other) => Value.CompareTo(other.Value);

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is StringId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
    public static bool operator ==(StringId a, StringId b) => a.CompareTo(b) == 0;
    public static bool operator !=(StringId a, StringId b) => !(a == b);

    class StringIdTypeConverter : System.ComponentModel.TypeConverter
    {
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            var stringValue = value as string;
            if (!string.IsNullOrEmpty(stringValue))
            {
                return new StringId(stringValue);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

    class StringIdNewtonsoftJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(System.Type objectType)
        {
            return objectType == typeof(StringId);
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var id = (StringId)value;
            serializer.Serialize(writer, id.Value);
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            return new StringId(serializer.Deserialize<string>(reader));
        }
    }
	
    class StringIdSystemTextJsonConverter : System.Text.Json.Serialization.JsonConverter<StringId>
    {
        public override StringId Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
        {
            return new StringId(reader.GetString());
        }

        public override void Write(System.Text.Json.Utf8JsonWriter writer, StringId value, System.Text.Json.JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }
}