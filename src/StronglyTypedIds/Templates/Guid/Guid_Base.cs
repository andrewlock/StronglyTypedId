    [System.ComponentModel.TypeConverter(typeof(TESTIDTypeConverter))]
    readonly partial struct TESTID : System.IComparable<TESTID>, System.IEquatable<TESTID>
    {
        public System.Guid Value { get; }

        public TESTID(System.Guid value)
        {
            Value = value;
        }

        public static TESTID New() => new TESTID(System.Guid.NewGuid());
        public static readonly TESTID Empty = new TESTID(System.Guid.Empty);

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
                return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                var stringValue = value as string;
                if (!string.IsNullOrEmpty(stringValue)
                    && System.Guid.TryParse(stringValue, out var guid))
                {
                    return new TESTID(guid);
                }

                return base.ConvertFrom(context, culture, value);
            }
        }
