[System.ComponentModel.TypeConverter(typeof(FooIdTypeConverter))]
[Newtonsoft.Json.JsonConverter(typeof(FooIdJsonConverter))]
readonly partial struct FooId : System.IComparable<FooId>, System.IEquatable<FooId>
{
    public System.Guid Value { get; }

    public FooId(System.Guid value)
    {
        Value = value;
    }

    public static FooId New() => new FooId(System.Guid.NewGuid());
    public static FooId Empty { get; } = new FooId(System.Guid.Empty);

    public bool Equals(FooId other) => this.Value.Equals(other.Value);
    public int CompareTo(FooId other) => Value.CompareTo(other.Value);

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is FooId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
    public static bool operator ==(FooId a, FooId b) => a.CompareTo(b) == 0;
    public static bool operator !=(FooId a, FooId b) => !(a == b);

    class FooIdTypeConverter : System.ComponentModel.TypeConverter
    {
        public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            var stringValue = value as string;
            if (!string.IsNullOrEmpty(stringValue)
                && System.Guid.TryParse(stringValue, out var guid))
            {
                return new FooId(guid);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

    class FooIdJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(System.Type objectType)
        {
            return objectType == typeof(FooId);
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var id = (FooId)value;
            serializer.Serialize(writer, id.Value);
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var guid = serializer.Deserialize<System.Guid>(reader);
            return new FooId(guid);
        }
    }
}