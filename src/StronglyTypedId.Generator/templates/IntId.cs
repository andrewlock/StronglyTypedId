[System.ComponentModel.TypeConverter(typeof(IntIdTypeConverter))]
[Newtonsoft.Json.JsonConverter(typeof(IntIdJsonConverter))]
readonly partial struct IntId : System.IComparable<IntId>, System.IEquatable<IntId>
{
    public int Value { get; }

    public IntId(int value)
    {
        Value = value;
    }

    public static readonly IntId Empty = new IntId(0);

    public bool Equals(IntId other) => this.Value.Equals(other.Value);
    public int CompareTo(IntId other) => Value.CompareTo(other.Value);

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is IntId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
    public static bool operator ==(IntId a, IntId b) => a.CompareTo(b) == 0;
    public static bool operator !=(IntId a, IntId b) => !(a == b);

    class IntIdTypeConverter : System.ComponentModel.TypeConverter
    {
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        {
            return sourceType == typeof(int) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is int intValue)
            {
                return new IntId(intValue);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

    class IntIdJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(System.Type objectType)
        {
            return objectType == typeof(IntId);
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var id = (IntId)value;
            serializer.Serialize(writer, id.Value);
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            return new IntId(serializer.Deserialize<int>(reader));
        }
    }
}