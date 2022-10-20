    readonly partial struct TESTID : INTERFACES
    {
        public string Value { get; }

        public TESTID(string value)
        {
            Value = value ?? throw new System.ArgumentNullException(nameof(value));
        }

        public static readonly TESTID Empty = new TESTID(string.Empty);

        public bool Equals(TESTID other)
        {
            return (Value, other.Value) switch
            {
                (null, null) => true,
                (null, _) => false,
                (_, null) => false,
                (_, _) => Value.Equals(other.Value),
            };
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TESTID other && Equals(other);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;
        public static bool operator ==(TESTID a, TESTID b) => a.Equals(b);
        public static bool operator !=(TESTID a, TESTID b) => !(a == b);

        public static TESTID Parse(string value) => new TESTID(value.Trim());
        public static bool TryParse(string value, out TESTID result)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                result = default;
                return false;
            }
            result = new TESTID(value.Trim());
            return true;
        }
