[System.ComponentModel.TypeConverter(typeof(GuidIdTypeConverter))]
[Newtonsoft.Json.JsonConverter(typeof(GuidIdJsonConverter))]
readonly partial struct GuidId : System.IComparable<GuidId>, System.IEquatable<GuidId>
{
    public System.Guid Value { get; }

    public GuidId(System.Guid value)
    {
        Value = value;
    }

    public static GuidId New() => new GuidId(System.Guid.NewGuid());
    public static readonly GuidId Empty = new GuidId(System.Guid.Empty);

    public bool Equals(GuidId other) => this.Value.Equals(other.Value);
    public int CompareTo(GuidId other) => Value.CompareTo(other.Value);

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is GuidId other && Equals(other);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();
    public static bool operator ==(GuidId a, GuidId b) => a.CompareTo(b) == 0;
    public static bool operator !=(GuidId a, GuidId b) => !(a == b);

    class GuidIdTypeConverter : System.ComponentModel.TypeConverter
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
                return new GuidId(guid);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

    class GuidIdJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(System.Type objectType)
        {
            return objectType == typeof(GuidId);
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var id = (GuidId)value;
            serializer.Serialize(writer, id.Value);
        }

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var guid = serializer.Deserialize<System.Guid>(reader);
            return new GuidId(guid);
        }
    }
}